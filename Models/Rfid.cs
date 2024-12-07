using System;
using System.Collections.Generic;

namespace AspnetCoreMvcFull.Models;

public partial class Rfid
{
    public int IdRfid { get; set; }

    public string CodigoRfid { get; set; } = null!;

    public int IdAlumno { get; set; }

    public virtual Alumno IdAlumnoNavigation { get; set; } = null!;
}
