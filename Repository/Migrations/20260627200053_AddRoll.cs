using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddRoll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // פקודה להוספת העמודה בלבד לטבלה הקיימת
            migrationBuilder.AddColumn<string>(
                name: "ROLE",
                table: "USERS",
                type: "varchar(max)",
                unicode: false,
                nullable: false,
                defaultValue: "User"); // ערך דיפולטיבי כפי שביקשת
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // פקודה שתדע להסיר את העמודה במידת הצורך (Rollback)
            migrationBuilder.DropColumn(
                name: "ROLE",
                table: "USERS");
        }
    }
}