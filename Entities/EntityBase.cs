using DemoApi.DbContextLocators;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoApi.Entities;
public class EntityBase<TDbContextLocator> : IEntityBase where TDbContextLocator : IDbContextLocator
{
    /// <summary>
    /// Id
    /// </summary>
    [Comment("Id")]
    public long Id { get; set; } = SnowflakeId.Instance.NextId();
    /// <summary>
    /// 创建人Id
    /// </summary>
    [Comment("创建人Id")]
    public long CreatedUserId { get; set; }
    /// <summary>
    /// 创建人姓名
    /// </summary>
    [Comment("创建人姓名")]
    public required string CreatedUserName { get; set; }
    /// <summary>
    /// 创建时间
    /// </summary>
    [Comment("创建时间")]
    public DateTime CreatedTime { get; set; }
    /// <summary>
    /// 最后变更人Id
    /// </summary>
    [Comment("最后变更人Id")]
    public long UpdatedUserId { get; set; }
    /// <summary>
    /// 最后变更人姓名
    /// </summary>
    [Comment("最后变更人姓名")]
    public required string UpdatedUserName { get; set; }
    /// <summary>
    /// 最后变更时间
    /// </summary>
    [Comment("最后变更时间")]
    public DateTime UpdatedTime { get; set; }
}
public class ConfigurableEntityBase<TEntity, TDbContextLocator>
    : EntityBase<TDbContextLocator>, IEntityTypeConfiguration<TEntity>
    where TEntity : class, IEntityBase
    where TDbContextLocator : IDbContextLocator
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // 默认设置为简体中文 UTF-8 排序规则
        //builder.SetAllStringCollation()
        // 默认设置字符串属性最大长度为 100
        //       .SetAllStringMaxLength()
        // 默认设置字符串属性默认值为空而不是Null
        //       .SetAllStringDefaultValue();
    }
}