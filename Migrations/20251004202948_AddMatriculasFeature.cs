using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamenParcial_Hidalgo.Migrations
{
    /// <inheritdoc />
    public partial class AddMatriculasFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Cursos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Cursos",
                type: "BLOB",
                nullable: true);
        }
    }
}
