using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AspnetCoreMvcFull.Models;

public partial class Profesor
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Configura el autoincremento
  public int IdProfesor { get; set; }

    public int IdUsuario { get; set; }

    public string? Departamento { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Materium> Materia { get; set; } = new List<Materium>();
}
