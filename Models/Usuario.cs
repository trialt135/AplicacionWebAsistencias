using System;
using System.Collections.Generic;

namespace AspnetCoreMvcFull.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public int IdRol { get; set; }

    public virtual ICollection<Administrador> Administradors { get; set; } = new List<Administrador>();

    public virtual ICollection<Alumno> Alumnos { get; set; } = new List<Alumno>();

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<Profesor> Profesors { get; set; } = new List<Profesor>();
}
