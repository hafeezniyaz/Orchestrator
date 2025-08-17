using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchestrator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AppConfigAssetRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Configs_AppId",
                table: "Configs",
                column: "AppId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigAssets_ConfigId",
                table: "ConfigAssets",
                column: "ConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConfigAssets_Configs_ConfigId",
                table: "ConfigAssets",
                column: "ConfigId",
                principalTable: "Configs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Configs_Apps_AppId",
                table: "Configs",
                column: "AppId",
                principalTable: "Apps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfigAssets_Configs_ConfigId",
                table: "ConfigAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_Configs_Apps_AppId",
                table: "Configs");

            migrationBuilder.DropIndex(
                name: "IX_Configs_AppId",
                table: "Configs");

            migrationBuilder.DropIndex(
                name: "IX_ConfigAssets_ConfigId",
                table: "ConfigAssets");
        }
    }
}
