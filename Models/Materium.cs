using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AspnetCoreMvcFull.Models;

public partial class Materium
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Configura el autoincremento
  public int IdMateria { get; set; }

    public string NombreMateria { get; set; } = null!;

    public int IdProfesor { get; set; }

    public virtual ICollection<Grupo> Grupos { get; set; } = new List<Grupo>();

    public virtual Profesor IdProfesorNavigation { get; set; } = null!;
  public virtual ICollection<Alumno> Alumnos { get; set; }
}
