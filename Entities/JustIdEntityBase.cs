using DemoApi.DbContextLocators;

using Microsoft.EntityFrameworkCore;

namespace DemoApi.Entities;
public class JustIdEntityBase<TDbContextLocator> : IEntityBase where TDbContextLocator : IDbContextLocator
{
    /// <summary>
    /// Id
    /// </summary>
    [Comment("Id")]
    public long Id { get; set; } = SnowflakeId.Instance.NextId();
}
