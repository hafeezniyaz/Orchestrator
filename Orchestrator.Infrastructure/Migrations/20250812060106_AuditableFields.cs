using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchestrator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AuditableFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "WebhookSubscriptions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "WebhookSubscriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "WebhookSubscriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "WebhookSubscriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Packages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Packages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Logs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Logs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Logs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Logs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AppId",
                table: "Configs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Configs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Configs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Configs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Configs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ConfigAssets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ConfigAssets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "ConfigAssets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "ConfigAssets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ClientHeartbeats",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ClientHeartbeats",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "ClientHeartbeats",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "ClientHeartbeats",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AuditLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "AuditLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Apps",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Apps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Apps",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Apps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ApiClients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ApiClients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "ApiClients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "ApiClients",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "WebhookSubscriptions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "WebhookSubscriptions");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "WebhookSubscriptions");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "WebhookSubscriptions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ConfigAssets");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ConfigAssets");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "ConfigAssets");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "ConfigAssets");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ClientHeartbeats");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ClientHeartbeats");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "ClientHeartbeats");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "ClientHeartbeats");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Apps");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Apps");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Apps");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Apps");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ApiClients");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ApiClients");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "ApiClients");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "ApiClients");

            migrationBuilder.AlterColumn<Guid>(
                name: "AppId",
                table: "Configs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
