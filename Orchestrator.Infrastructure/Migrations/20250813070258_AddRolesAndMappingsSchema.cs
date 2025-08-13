using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Orchestrator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesAndMappingsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Roles",
                table: "ApiClients");

            migrationBuilder.CreateTable(
                name: "RoleMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApiClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ADGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "ModifiedAt", "ModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("0d52554b-b12a-4c87-9352-3781e1054948"), new DateTime(2025, 8, 12, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Administrator" },
                    { new Guid("71cb4b4c-e3f6-48b1-b800-cbf20acbf1ab"), new DateTime(2025, 8, 12, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Support" },
                    { new Guid("a1b2c3d4-5678-90ab-cdef-1234567890ab"), new DateTime(2025, 8, 12, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Viewer" },
                    { new Guid("e3b3b4a2-1e1b-4c1a-9b4e-8a3d11b02b3a"), new DateTime(2025, 8, 12, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Publisher" },
                    { new Guid("f5e4d3c2-b1a0-9876-5432-10fedcba9876"), new DateTime(2025, 8, 12, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleMappings");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.AddColumn<string>(
                name: "Roles",
                table: "ApiClients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
