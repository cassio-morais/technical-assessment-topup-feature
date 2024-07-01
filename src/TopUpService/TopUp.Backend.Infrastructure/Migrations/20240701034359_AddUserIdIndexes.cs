using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.TopUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_top_up_transactions_user_id",
                table: "top_up_transactions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_top_up_beneficiaries_user_id",
                table: "top_up_beneficiaries",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_top_up_transactions_user_id",
                table: "top_up_transactions");

            migrationBuilder.DropIndex(
                name: "IX_top_up_beneficiaries_user_id",
                table: "top_up_beneficiaries");
        }
    }
}
