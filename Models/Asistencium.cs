using System;
using System.Collections.Generic;

namespace AspnetCoreMvcFull.Models;

public partial class Asistencium
{

    public int IdAsistencia { get; set; }

    public int IdRegistro { get; set; }

    public int IdAlumno { get; set; }

    public bool Presente { get; set; }

    public virtual Alumno IdAlumnoNavigation { get; set; } = null!;

    public virtual Registro IdRegistroNavigation { get; set; } = null!;
}
