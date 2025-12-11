using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ABAK_NUEVO.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNumeroContactoToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NumeroContacto",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumeroContacto",
                table: "AspNetUsers");
        }
    }
}
