using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarMultimedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagenContentType",
                table: "Incidencias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagenRuta",
                table: "Incidencias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoContentType",
                table: "Incidencias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoRuta",
                table: "Incidencias",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenContentType",
                table: "Incidencias");

            migrationBuilder.DropColumn(
                name: "ImagenRuta",
                table: "Incidencias");

            migrationBuilder.DropColumn(
                name: "VideoContentType",
                table: "Incidencias");

            migrationBuilder.DropColumn(
                name: "VideoRuta",
                table: "Incidencias");
        }
    }
}
