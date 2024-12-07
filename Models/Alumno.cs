using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AspnetCoreMvcFull.Models;

public partial class Alumno
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Configura el autoincremento
  public int IdAlumno { get; set; }

    public int IdUsuario { get; set; }

    public DateOnly? FechaIngreso { get; set; }

    public virtual ICollection<Asistencium> Asistencia { get; set; } = new List<Asistencium>();

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Rfid> Rfids { get; set; } = new List<Rfid>();

    public virtual ICollection<Grupo> IdGrupos { get; set; } = new List<Grupo>();

  public virtual ICollection<Materium> Materias { get; set; }
}
