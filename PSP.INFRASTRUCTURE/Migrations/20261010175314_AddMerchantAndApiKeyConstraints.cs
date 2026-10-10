using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSP.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class AddMerchantAndApiKeyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "merchants",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "merchants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "prefix",
                table: "api_keys",
                type: "character varying(12)",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "key_hash",
                table: "api_keys",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "ix_payments_merchant_id",
                table: "payments",
                column: "merchant_id");

            migrationBuilder.CreateIndex(
                name: "ix_api_keys_key_hash",
                table: "api_keys",
                column: "key_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_api_keys_merchant_id",
                table: "api_keys",
                column: "merchant_id");

            migrationBuilder.AddForeignKey(
                name: "fk_api_keys_merchants_merchant_id",
                table: "api_keys",
                column: "merchant_id",
                principalTable: "merchants",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_payments_merchants_merchant_id",
                table: "payments",
                column: "merchant_id",
                principalTable: "merchants",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_api_keys_merchants_merchant_id",
                table: "api_keys");

            migrationBuilder.DropForeignKey(
                name: "fk_payments_merchants_merchant_id",
                table: "payments");

            migrationBuilder.DropIndex(
                name: "ix_payments_merchant_id",
                table: "payments");

            migrationBuilder.DropIndex(
                name: "ix_api_keys_key_hash",
                table: "api_keys");

            migrationBuilder.DropIndex(
                name: "ix_api_keys_merchant_id",
                table: "api_keys");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "merchants",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "merchants",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "prefix",
                table: "api_keys",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(12)",
                oldMaxLength: 12);

            migrationBuilder.AlterColumn<string>(
                name: "key_hash",
                table: "api_keys",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);
        }
    }
}
