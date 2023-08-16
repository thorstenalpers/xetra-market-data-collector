using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MarketData.Infratructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "TradeX");

            migrationBuilder.CreateTable(
                name: "AssetMetrics",
                schema: "TradeX",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetMetrics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                schema: "TradeX",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Symbol = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Isin = table.Column<string>(type: "text", nullable: true),
                    Mnemonic = table.Column<string>(type: "text", nullable: true),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetRecords",
                schema: "TradeX",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    AssetId = table.Column<int>(type: "integer", nullable: false),
                    AssetMetricId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetRecords_AssetMetrics_AssetMetricId",
                        column: x => x.AssetMetricId,
                        principalSchema: "TradeX",
                        principalTable: "AssetMetrics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetRecords_Assets_AssetId",
                        column: x => x.AssetId,
                        principalSchema: "TradeX",
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "TradeX",
                table: "AssetMetrics",
                columns: new[] { "Id", "CreatedAt", "Type" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "PriceClose" },
                    { 2, new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "PriceOpen" },
                    { 101, new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "PriceDeltaPercentage_PriceCloseToPriceClose_1Day" },
                    { 201, new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "PriceDeltaPercentage_PriceOpenToPriceClose_1Day" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_AssetId",
                schema: "TradeX",
                table: "AssetRecords",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_AssetMetricId",
                schema: "TradeX",
                table: "AssetRecords",
                column: "AssetMetricId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_Date_AssetId_AssetMetricId",
                schema: "TradeX",
                table: "AssetRecords",
                columns: new[] { "Date", "AssetId", "AssetMetricId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_Isin",
                schema: "TradeX",
                table: "Assets",
                column: "Isin",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetRecords",
                schema: "TradeX");

            migrationBuilder.DropTable(
                name: "AssetMetrics",
                schema: "TradeX");

            migrationBuilder.DropTable(
                name: "Assets",
                schema: "TradeX");
        }
    }
}
