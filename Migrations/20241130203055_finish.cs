using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspnetCoreMvcFull.Migrations
{
    /// <inheritdoc />
    public partial class finish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    IdRol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreRol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Rol__2A49584C47B6BA5F", x => x.IdRol);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Contraseña = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdRol = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuario__5B65BF97B97743B9", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK__Usuario__IdRol__4D94879B",
                        column: x => x.IdRol,
                        principalTable: "Rol",
                        principalColumn: "IdRol");
                });

            migrationBuilder.CreateTable(
                name: "Administrador",
                columns: table => new
                {
                    IdAdministrador = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    AreaTrabajo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Administ__2B3E34A8F6D2610D", x => x.IdAdministrador);
                    table.ForeignKey(
                        name: "FK__Administr__IdUsu__5629CD9C",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateTable(
                name: "Alumno",
                columns: table => new
                {
                    IdAlumno = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    FechaIngreso = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Alumno__460B474052CAE741", x => x.IdAlumno);
                    table.ForeignKey(
                        name: "FK__Alumno__IdUsuari__534D60F1",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateTable(
                name: "Profesor",
                columns: table => new
                {
                    IdProfesor = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    Departamento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Profesor__C377C3A103C2895A", x => x.IdProfesor);
                    table.ForeignKey(
                        name: "FK__Profesor__IdUsua__5070F446",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateTable(
                name: "RFID",
                columns: table => new
                {
                    IdRFID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoRFID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdAlumno = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RFID__B50B632619B56212", x => x.IdRFID);
                    table.ForeignKey(
                        name: "FK__RFID__IdAlumno__59FA5E80",
                        column: x => x.IdAlumno,
                        principalTable: "Alumno",
                        principalColumn: "IdAlumno");
                });

            migrationBuilder.CreateTable(
                name: "Materia",
                columns: table => new
                {
                    IdMateria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreMateria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdProfesor = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Materia__EC174670A96156B2", x => x.IdMateria);
                    table.ForeignKey(
                        name: "FK__Materia__IdProfe__5CD6CB2B",
                        column: x => x.IdProfesor,
                        principalTable: "Profesor",
                        principalColumn: "IdProfesor");
                });

            migrationBuilder.CreateTable(
                name: "Grupo",
                columns: table => new
                {
                    IdGrupo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreGrupo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdMateria = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Grupo__303F6FD992EB38A6", x => x.IdGrupo);
                    table.ForeignKey(
                        name: "FK__Grupo__IdMateria__5FB337D6",
                        column: x => x.IdMateria,
                        principalTable: "Materia",
                        principalColumn: "IdMateria");
                });

            migrationBuilder.CreateTable(
                name: "Grupo_Alumno",
                columns: table => new
                {
                    IdGrupo = table.Column<int>(type: "int", nullable: false),
                    IdAlumno = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Grupo_Al__245FDBADDC592434", x => new { x.IdGrupo, x.IdAlumno });
                    table.ForeignKey(
                        name: "FK__Grupo_Alu__IdAlu__6B24EA82",
                        column: x => x.IdAlumno,
                        principalTable: "Alumno",
                        principalColumn: "IdAlumno");
                    table.ForeignKey(
                        name: "FK__Grupo_Alu__IdGru__6A30C649",
                        column: x => x.IdGrupo,
                        principalTable: "Grupo",
                        principalColumn: "IdGrupo");
                });

            migrationBuilder.CreateTable(
                name: "Registro",
                columns: table => new
                {
                    IdRegistro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdGrupo = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Registro__FFA45A99602B90CC", x => x.IdRegistro);
                    table.ForeignKey(
                        name: "FK__Registro__IdGrup__628FA481",
                        column: x => x.IdGrupo,
                        principalTable: "Grupo",
                        principalColumn: "IdGrupo");
                });

            migrationBuilder.CreateTable(
                name: "Asistencia",
                columns: table => new
                {
                    IdAsistencia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRegistro = table.Column<int>(type: "int", nullable: false),
                    IdAlumno = table.Column<int>(type: "int", nullable: false),
                    Presente = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Asistenc__3956DEE6FF26E3B5", x => x.IdAsistencia);
                    table.ForeignKey(
                        name: "FK__Asistenci__IdAlu__6754599E",
                        column: x => x.IdAlumno,
                        principalTable: "Alumno",
                        principalColumn: "IdAlumno");
                    table.ForeignKey(
                        name: "FK__Asistenci__IdReg__66603565",
                        column: x => x.IdRegistro,
                        principalTable: "Registro",
                        principalColumn: "IdRegistro");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administrador_IdUsuario",
                table: "Administrador",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Alumno_IdUsuario",
                table: "Alumno",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencia_IdAlumno",
                table: "Asistencia",
                column: "IdAlumno");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencia_IdRegistro",
                table: "Asistencia",
                column: "IdRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_Grupo_IdMateria",
                table: "Grupo",
                column: "IdMateria");

            migrationBuilder.CreateIndex(
                name: "IX_Grupo_Alumno_IdAlumno",
                table: "Grupo_Alumno",
                column: "IdAlumno");

            migrationBuilder.CreateIndex(
                name: "IX_Materia_IdProfesor",
                table: "Materia",
                column: "IdProfesor");

            migrationBuilder.CreateIndex(
                name: "IX_Profesor_IdUsuario",
                table: "Profesor",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Registro_IdGrupo",
                table: "Registro",
                column: "IdGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_RFID_IdAlumno",
                table: "RFID",
                column: "IdAlumno");

            migrationBuilder.CreateIndex(
                name: "UQ__RFID__117C4055AD60DE98",
                table: "RFID",
                column: "CodigoRFID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_IdRol",
                table: "Usuario",
                column: "IdRol");

            migrationBuilder.CreateIndex(
                name: "UQ__Usuario__60695A19159A751F",
                table: "Usuario",
                column: "Correo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrador");

            migrationBuilder.DropTable(
                name: "Asistencia");

            migrationBuilder.DropTable(
                name: "Grupo_Alumno");

            migrationBuilder.DropTable(
                name: "RFID");

            migrationBuilder.DropTable(
                name: "Registro");

            migrationBuilder.DropTable(
                name: "Alumno");

            migrationBuilder.DropTable(
                name: "Grupo");

            migrationBuilder.DropTable(
                name: "Materia");

            migrationBuilder.DropTable(
                name: "Profesor");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Rol");
        }
    }
}
