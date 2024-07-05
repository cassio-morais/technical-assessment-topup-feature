using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.TopUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "top_up_beneficiaries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nickname = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    phone_number = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_top_up_beneficiaries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "top_up_transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    top_up_beneficiary_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    transaction_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
                    reason = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_top_up_transactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_top_up_transactions_top_up_beneficiaries_top_up_beneficiary~",
                        column: x => x.top_up_beneficiary_id,
                        principalTable: "top_up_beneficiaries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_top_up_transactions_top_up_beneficiary_id",
                table: "top_up_transactions",
                column: "top_up_beneficiary_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "top_up_transactions");

            migrationBuilder.DropTable(
                name: "top_up_beneficiaries");
        }
    }
}
