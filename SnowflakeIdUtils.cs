using System;
using System.Text;

namespace DemoApi;

public class SnowflakeId
{
    #region 私有字段
    // 开始时间截((new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)-Jan1st1970).TotalMilliseconds)
    private const long twepoch = 1577836800000L;

    // 机器id所占的位数
    private const int workerIdBits = 5;

    // 数据标识id所占的位数
    private const int datacenterIdBits = 5;

    // 支持的最大机器id，结果是31 (这个移位算法可以很快的计算出几位二进制数所能表示的最大十进制数) 
    private const long maxWorkerId = -1L ^ -1L << workerIdBits;

    // 支持的最大数据标识id，结果是31 
    private const long maxDatacenterId = -1L ^ -1L << datacenterIdBits;

    // 序列在id中占的位数 
    private const int sequenceBits = 12;

    // 数据标识id向左移17位(12+5) 
    private const int datacenterIdShift = sequenceBits + workerIdBits;

    // 机器ID向左移12位 
    private const int workerIdShift = sequenceBits;


    // 时间截向左移22位(5+5+12) 
    private const int timestampLeftShift = sequenceBits + workerIdBits + datacenterIdBits;

    // 生成序列的掩码，这里为4095 (0b111111111111=0xfff=4095) 
    private const long sequenceMask = -1L ^ -1L << sequenceBits;
    // 单例实例
    private static SnowflakeId? _instance;

    // 数据中心ID(0~31) 
    private static long _datacenterId;
    // 工作机器ID(0~31) 
    private static long _workerId;
    // 锁
    private static object lockObj = new();
    #endregion
    #region 公有属性
    // 毫秒内序列(0~4095) 
    public long Sequence { get; private set; }

    // 上次生成ID的时间截 
    public long LastTimestamp { get; private set; }
    #endregion
    #region 对外暴露的方法
    /// <summary>
    /// 雪花ID
    /// </summary>
    /// <param name="datacenterId">数据中心ID</param>
    /// <param name="workerId">工作机器ID</param>
    public SnowflakeId(long datacenterId, long workerId)
    {
        if (datacenterId > maxDatacenterId || datacenterId < 0)
            throw new Exception(string.Format("datacenter Id can't be greater than {0} or less than 0", maxDatacenterId));
        if (workerId > maxWorkerId || workerId < 0)
            throw new Exception(string.Format("worker Id can't be greater than {0} or less than 0", maxWorkerId));
        _workerId = workerId;
        _datacenterId = datacenterId;
        Sequence = 0L;
        LastTimestamp = -1L;
        _instance = this;
    }
    #region 单例实例
    public static SnowflakeId Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (lockObj)
                {
                    if (_instance == null)
                    {
                        if (_datacenterId == 0 && _workerId == 0)
                            throw new InvalidOperationException("SnowflakeId instance has not been initialized. Call Initialize first.");
                        _instance = new SnowflakeId(_datacenterId, _workerId);
                    }
                }
            }
            return _instance;
        }
    }

    public static void Initialize(long datacenterId, long workerId)
    {
        if (_instance != null)
            throw new InvalidOperationException("SnowflakeId instance has already been initialized.");
        _datacenterId = datacenterId;
        _workerId = workerId;
    }
    #endregion
    /// <summary>
    /// 获得下一个ID
    /// </summary>
    /// <returns></returns>
    public long NextId()
    {
        lock (this)
        {
            long timestamp = GetCurrentTimestamp();
            if (timestamp > LastTimestamp) //时间戳改变，毫秒内序列重置
                Sequence = 0L;
            else if (timestamp == LastTimestamp) //如果是同一时间生成的，则进行毫秒内序列
            {
                Sequence = Sequence + 1 & sequenceMask;
                if (Sequence == 0) //毫秒内序列溢出
                    timestamp = GetNextTimestamp(LastTimestamp); //阻塞到下一个毫秒,获得新的时间戳
            }
            else   //当前时间小于上一次ID生成的时间戳，证明系统时钟被回拨，此时需要做回拨处理
            {
                Sequence = Sequence + 1 & sequenceMask;
                if (Sequence > 0)
                    timestamp = LastTimestamp;     //停留在最后一次时间戳上，等待系统时间追上后即完全度过了时钟回拨问题。
                else   //毫秒内序列溢出
                {
                    timestamp = LastTimestamp + 1;   //直接进位到下一个毫秒                          
                }
                //throw new Exception(string.Format("Clock moved backwards.  Refusing to generate id for {0} milliseconds", lastTimestamp - timestamp));
            }

            LastTimestamp = timestamp;       //上次生成ID的时间截

            //移位并通过或运算拼到一起组成64位的ID
            var id = timestamp - twepoch << timestampLeftShift
           | _datacenterId << datacenterIdShift
           | _workerId << workerIdShift
           | Sequence;
            return id;
        }
    }

    /// <summary>
    /// 解析雪花ID
    /// </summary>
    /// <returns></returns>
    public static string AnalyzeId(long Id)
    {
        StringBuilder sb = new StringBuilder();

        var timestamp = Id >> timestampLeftShift;
        var time = Jan1st1970.AddMilliseconds(timestamp + twepoch);
        sb.Append(time.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss:fff"));

        var datacenterId = (Id ^ timestamp << timestampLeftShift) >> datacenterIdShift;
        sb.Append("_" + datacenterId);

        var workerId = (Id ^ (timestamp << timestampLeftShift | datacenterId << datacenterIdShift)) >> workerIdShift;
        sb.Append("_" + workerId);

        var sequence = Id & sequenceMask;
        sb.Append("_" + sequence);

        return sb.ToString();
    }
    #endregion
    #region 私有方法
    /// <summary>
    /// 阻塞到下一个毫秒，直到获得新的时间戳
    /// </summary>
    /// <param name="lastTimestamp">上次生成ID的时间截</param>
    /// <returns>当前时间戳</returns>
    private static long GetNextTimestamp(long lastTimestamp)
    {
        long timestamp = GetCurrentTimestamp();
        while (timestamp <= lastTimestamp)
        {
            timestamp = GetCurrentTimestamp();
        }
        return timestamp;
    }

    /// <summary>
    /// 获取当前时间戳
    /// </summary>
    /// <returns></returns>
    private static long GetCurrentTimestamp()
    {
        return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
    }

    private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    #endregion
}
