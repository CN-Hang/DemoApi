using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoApi.Migrations
{
    /// <inheritdoc />
    public partial class v100 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sys_user",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false, comment: "Id")
                        .Annotation("Sqlite:Autoincrement", true),
                    Account = table.Column<string>(type: "TEXT", nullable: false, comment: "账号"),
                    NickName = table.Column<string>(type: "TEXT", nullable: true, comment: "昵称"),
                    Password = table.Column<string>(type: "TEXT", nullable: false, comment: "密码"),
                    Email = table.Column<string>(type: "TEXT", nullable: true, comment: "邮箱"),
                    Sort = table.Column<int>(type: "INTEGER", nullable: false, comment: "排序"),
                    CreatedUserId = table.Column<long>(type: "INTEGER", nullable: false, comment: "创建人Id"),
                    CreatedUserName = table.Column<string>(type: "TEXT", nullable: false, comment: "创建人姓名"),
                    CreatedTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间"),
                    UpdatedUserId = table.Column<long>(type: "INTEGER", nullable: false, comment: "最后变更人Id"),
                    UpdatedUserName = table.Column<string>(type: "TEXT", nullable: false, comment: "最后变更人姓名"),
                    UpdatedTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "最后变更时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_user", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sys_user_Account",
                table: "sys_user",
                column: "Account",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sys_user");
        }
    }
}
