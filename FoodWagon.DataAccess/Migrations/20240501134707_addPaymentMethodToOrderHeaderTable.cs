using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodWagon.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addPaymentMethodToOrderHeaderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "OrderHeaders");
        }
    }
}
