using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RTS.Service.Connector.Migrations
{
    /// <inheritdoc />
    public partial class SmallChangesToInvoiceLineTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "ProductNumber",
                table: "InvoiceLines");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercent",
                table: "InvoiceLines",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ProductNumber",
                table: "InvoiceLines",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
