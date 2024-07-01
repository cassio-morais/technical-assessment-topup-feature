using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.TopUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRequiredToManyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "top_up_transactions",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true,
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                table: "top_up_beneficiaries",
                type: "VARCHAR(15)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(15)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "nickname",
                table: "top_up_beneficiaries",
                type: "VARCHAR(20)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "top_up_transactions",
                type: "integer",
                nullable: true,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                table: "top_up_beneficiaries",
                type: "VARCHAR(15)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(15)");

            migrationBuilder.AlterColumn<string>(
                name: "nickname",
                table: "top_up_beneficiaries",
                type: "VARCHAR(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)");
        }
    }
}
