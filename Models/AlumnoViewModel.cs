using System;
using System.ComponentModel.DataAnnotations;

namespace AspnetCoreMvcFull.Models
{
  public class AlumnoViewModel
  {
    [Required]
    public int IdUsuario { get; set; }

    [Required]
    public DateOnly FechaIngreso { get; set; }

    public string CodigoRfid { get; set; } = string.Empty;
  }
}
