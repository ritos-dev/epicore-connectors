using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RTS.Service.Connector.Migrations
{
    /// <inheritdoc />
    public partial class CrmIdChangedToCrmNumberForClarity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CrmId",
                table: "TracelinkOrders",
                newName: "CrmNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CrmNumber",
                table: "TracelinkOrders",
                newName: "CrmId");
        }
    }
}
