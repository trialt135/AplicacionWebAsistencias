using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspnetCoreMvcFull.Migrations
{
    /// <inheritdoc />
    public partial class AlumnosMateria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlumnoMaterium",
                columns: table => new
                {
                    AlumnosIdAlumno = table.Column<int>(type: "int", nullable: false),
                    MateriasIdMateria = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlumnoMaterium", x => new { x.AlumnosIdAlumno, x.MateriasIdMateria });
                    table.ForeignKey(
                        name: "FK_AlumnoMaterium_Alumno_AlumnosIdAlumno",
                        column: x => x.AlumnosIdAlumno,
                        principalTable: "Alumno",
                        principalColumn: "IdAlumno",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlumnoMaterium_Materia_MateriasIdMateria",
                        column: x => x.MateriasIdMateria,
                        principalTable: "Materia",
                        principalColumn: "IdMateria",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlumnoMaterium_MateriasIdMateria",
                table: "AlumnoMaterium",
                column: "MateriasIdMateria");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlumnoMaterium");
        }
    }
}
