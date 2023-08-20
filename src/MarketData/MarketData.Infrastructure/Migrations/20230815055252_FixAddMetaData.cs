#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MarketData.Infratructure.Migrations;
using Microsoft.EntityFrameworkCore.Migrations;

/// <inheritdoc />
public partial class FixAddMetaData : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            schema: "TradeX",
            table: "AssetMetrics",
            keyColumn: "Id",
            keyValue: 101);

        migrationBuilder.DeleteData(
            schema: "TradeX",
            table: "AssetMetrics",
            keyColumn: "Id",
            keyValue: 201);

        migrationBuilder.AddColumn<string>(
            name: "Url",
            schema: "TradeX",
            table: "Assets",
            type: "text",
            nullable: true);

        migrationBuilder.InsertData(
            schema: "TradeX",
            table: "AssetMetrics",
            columns: new[] { "Id", "CreatedAt", "Type" },
            values: new object[] { 3, new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Volumen" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            schema: "TradeX",
            table: "AssetMetrics",
            keyColumn: "Id",
            keyValue: 3);

        migrationBuilder.DropColumn(
            name: "Url",
            schema: "TradeX",
            table: "Assets");

        migrationBuilder.InsertData(
            schema: "TradeX",
            table: "AssetMetrics",
            columns: new[] { "Id", "CreatedAt", "Type" },
            values: new object[,]
            {
                { 101, new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "PriceDeltaPercentage_PriceCloseToPriceClose_1Day" },
                { 201, new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "PriceDeltaPercentage_PriceOpenToPriceClose_1Day" }
            });
    }
}
