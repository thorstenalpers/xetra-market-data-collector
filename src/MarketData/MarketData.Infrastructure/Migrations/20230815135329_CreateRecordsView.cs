using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketData.Infratructure.Migrations;

/// <inheritdoc />
public partial class CreateRecordsView : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
                DROP VIEW IF EXISTS ""TradeX"".""AssetRecordsView"";
                CREATE OR REPLACE VIEW ""TradeX"".""AssetRecordsView"" AS
                SELECT t1.""Id"", t1.""Date"", t1.""Value""::numeric, t1.""AssetId"", t1.""AssetMetricId"", t2.""Symbol"", t2.""Name""
                FROM ""TradeX"".""AssetRecords"" t1
                LEFT JOIN 
                    (SELECT * FROM ""TradeX"".""Assets"") t2
                ON t1.""AssetId"" = t2.""Id"";
            ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {

    }
}
