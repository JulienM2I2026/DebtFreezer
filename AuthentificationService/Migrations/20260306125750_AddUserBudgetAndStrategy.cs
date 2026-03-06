using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthentificationService.Migrations
{
    /// <inheritdoc />
    public partial class AddUserBudgetAndStrategy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyRepaymentBudget",
                table: "users",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "RepaymentStrategy",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthlyRepaymentBudget",
                table: "users");

            migrationBuilder.DropColumn(
                name: "RepaymentStrategy",
                table: "users");
        }
    }
}
