using System;
using System.Collections.Generic;

namespace AspnetCoreMvcFull.Models;

public partial class Registro
{
    public int IdRegistro { get; set; }

    public int IdGrupo { get; set; }

    public DateOnly Fecha { get; set; }

    public virtual ICollection<Asistencium> Asistencia { get; set; } = new List<Asistencium>();

    public virtual Grupo IdGrupoNavigation { get; set; } = null!;
}
