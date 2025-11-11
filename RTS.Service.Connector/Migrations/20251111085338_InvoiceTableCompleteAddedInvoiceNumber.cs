using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RTS.Service.Connector.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceTableCompleteAddedInvoiceNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "Invoices");

            migrationBuilder.AddColumn<int>(
                name: "DraftInvoiceNumber",
                table: "Invoices",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DraftInvoiceNumber",
                table: "Invoices");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceId",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
