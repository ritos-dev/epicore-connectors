using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RTS.Service.Connector.Migrations
{
    /// <inheritdoc />
    public partial class SummaryInvoiceReportTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SummaryReportId",
                table: "Invoices",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SummaryInvoiceReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CrmId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExpectedInvoices = table.Column<int>(type: "int", nullable: false),
                    InvoiceCount = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Currency = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummaryInvoiceReports", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_SummaryReportId",
                table: "Invoices",
                column: "SummaryReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_SummaryInvoiceReports_SummaryReportId",
                table: "Invoices",
                column: "SummaryReportId",
                principalTable: "SummaryInvoiceReports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_SummaryInvoiceReports_SummaryReportId",
                table: "Invoices");

            migrationBuilder.DropTable(
                name: "SummaryInvoiceReports");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_SummaryReportId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "SummaryReportId",
                table: "Invoices");
        }
    }
}
