using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RTS.Service.Connector.Migrations
{
    /// <inheritdoc />
    public partial class SmallChangesToTracelinkDbTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "TracelinkOrders",
                newName: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "TracelinkOrders",
                newName: "CompanyId");
        }
    }
}
