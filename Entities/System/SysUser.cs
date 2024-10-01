using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

using DemoApi.DbContextLocators;
using DemoApi.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoApi.Entities.System;
/// <summary>
/// 用户表
/// </summary>
[Table("sys_user")]
[Index(nameof(Account), IsUnique = true)]
public class SysUser : ConfigurableEntityBase<SysUser,SystemDbContextLocator>
{
    /// <summary>
    /// 账号
    /// </summary>
    [Comment("账号")]
    public required string Account { get; set; }
    /// <summary>
    /// 昵称
    /// </summary>
    [Comment("昵称")]
    public string? NickName { get; set; }
    /// <summary>
    /// 密码
    /// </summary>
    [Comment("密码")]
    public required string Password { get; set; }
    /// <summary>
    /// 邮箱
    /// </summary>
    [Comment("邮箱")]
    public string? Email { get; set; }
    /// <summary>
    /// 排序
    /// </summary>
    [Comment("排序")]
    [DefaultValue(100)]
    public int Sort { get; set; }
    public override void Configure(EntityTypeBuilder<SysUser> builder)
    {
        builder.Property(a => a.NickName).HasMaxLength(100);
        base.Configure(builder);
    }
}
