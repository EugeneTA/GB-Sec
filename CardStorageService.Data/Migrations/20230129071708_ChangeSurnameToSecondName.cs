using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardStorageService.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSurnameToSecondName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Surname",
                table: "Clients",
                newName: "SecondName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SecondName",
                table: "Clients",
                newName: "Surname");
        }
    }
}
