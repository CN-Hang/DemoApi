﻿// <auto-generated />
using System;
using DemoApi.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DemoApi.Migrations
{
    [DbContext(typeof(SimpleDbContext))]
    partial class SimpleDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("DemoApi.Entities.System.SysUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasComment("Id");

                    b.Property<string>("Account")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasComment("账号");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("TEXT")
                        .HasComment("创建时间");

                    b.Property<long>("CreatedUserId")
                        .HasColumnType("INTEGER")
                        .HasComment("创建人Id");

                    b.Property<string>("CreatedUserName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasComment("创建人姓名");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT")
                        .HasComment("邮箱");

                    b.Property<string>("NickName")
                        .HasColumnType("TEXT")
                        .HasComment("昵称");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasComment("密码");

                    b.Property<int>("Sort")
                        .HasColumnType("INTEGER")
                        .HasComment("排序");

                    b.Property<DateTime>("UpdatedTime")
                        .HasColumnType("TEXT")
                        .HasComment("最后变更时间");

                    b.Property<long>("UpdatedUserId")
                        .HasColumnType("INTEGER")
                        .HasComment("最后变更人Id");

                    b.Property<string>("UpdatedUserName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasComment("最后变更人姓名");

                    b.HasKey("Id");

                    b.HasIndex("Account")
                        .IsUnique();

                    b.ToTable("sys_user");
                });
#pragma warning restore 612, 618
        }
    }
}