#nullable disable

namespace MarketData.Infratructure.Migrations;
using Microsoft.EntityFrameworkCore.Migrations;

/// <inheritdoc />
public partial class AddMetaData : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(
            name: "AvgTradingVolume",
            schema: "TradeX",
            table: "Assets",
            type: "bigint",
            nullable: true);

        migrationBuilder.AddColumn<long>(
            name: "CntEmployees",
            schema: "TradeX",
            table: "Assets",
            type: "bigint",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Exchange",
            schema: "TradeX",
            table: "Assets",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ExchangeCloseTime",
            schema: "TradeX",
            table: "Assets",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ExchangeCountryIso",
            schema: "TradeX",
            table: "Assets",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Industry",
            schema: "TradeX",
            table: "Assets",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<long>(
            name: "MarketCapitalization",
            schema: "TradeX",
            table: "Assets",
            type: "bigint",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Sector",
            schema: "TradeX",
            table: "Assets",
            type: "text",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AvgTradingVolume",
            schema: "TradeX",
            table: "Assets");

        migrationBuilder.DropColumn(
            name: "CntEmployees",
            schema: "TradeX",
            table: "Assets");

        migrationBuilder.DropColumn(
            name: "Exchange",
            schema: "TradeX",
            table: "Assets");

        migrationBuilder.DropColumn(
            name: "ExchangeCloseTime",
            schema: "TradeX",
            table: "Assets");

        migrationBuilder.DropColumn(
            name: "ExchangeCountryIso",
            schema: "TradeX",
            table: "Assets");

        migrationBuilder.DropColumn(
            name: "Industry",
            schema: "TradeX",
            table: "Assets");

        migrationBuilder.DropColumn(
            name: "MarketCapitalization",
            schema: "TradeX",
            table: "Assets");

        migrationBuilder.DropColumn(
            name: "Sector",
            schema: "TradeX",
            table: "Assets");
    }
}
