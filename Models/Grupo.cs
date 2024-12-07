using System;
using System.Collections.Generic;

namespace AspnetCoreMvcFull.Models;

public partial class Grupo
{
    public int IdGrupo { get; set; }

    public string NombreGrupo { get; set; } = null!;

    public int IdMateria { get; set; }

    public virtual Materium IdMateriaNavigation { get; set; } = null!;

    public virtual ICollection<Registro> Registros { get; set; } = new List<Registro>();

    public virtual ICollection<Alumno> IdAlumnos { get; set; } = new List<Alumno>();
}
