using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSP.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class AddMerchantOrderIdToPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "merchant_order_id",
                table: "payments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "LEGACY");

            migrationBuilder.CreateIndex(
                name: "ix_payments_merchant_order_id",
                table: "payments",
                column: "merchant_order_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_payments_merchant_order_id",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "merchant_order_id",
                table: "payments");
        }
    }
}
