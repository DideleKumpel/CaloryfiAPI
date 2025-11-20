using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaloryfiAPI.Migrations
{
    /// <inheritdoc />
    public partial class MealComponentWeightadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "MealComponents",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Weight",
                table: "MealComponents");
        }
    }
}
