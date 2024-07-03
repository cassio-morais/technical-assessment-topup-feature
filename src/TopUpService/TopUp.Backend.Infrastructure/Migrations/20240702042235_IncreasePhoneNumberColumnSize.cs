using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.TopUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IncreasePhoneNumberColumnSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                table: "top_up_beneficiaries",
                type: "VARCHAR(16)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(15)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                table: "top_up_beneficiaries",
                type: "VARCHAR(15)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(16)");
        }
    }
}
