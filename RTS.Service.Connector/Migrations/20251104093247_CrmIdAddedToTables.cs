using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RTS.Service.Connector.Migrations
{
    /// <inheritdoc />
    public partial class CrmIdAddedToTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CrmId",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CrmId",
                table: "Invoices");
        }
    }
}
