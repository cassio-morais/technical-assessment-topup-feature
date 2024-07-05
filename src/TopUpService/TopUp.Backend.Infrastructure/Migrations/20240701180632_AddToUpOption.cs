using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.TopUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddToUpOption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "top_up_options",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    currency_abbreviation = table.Column<string>(type: "CHAR(3)", nullable: false),
                    value = table.Column<decimal>(type: "numeric", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_top_up_options", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_top_up_options_currency_abbreviation",
                table: "top_up_options",
                column: "currency_abbreviation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "top_up_options");
        }
    }
}
