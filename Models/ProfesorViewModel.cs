using System.ComponentModel.DataAnnotations;

namespace AspnetCoreMvcFull.Models
{
  public class ProfesorViewModel
  {
    [Required]
    public int IdUsuario { get; set; }

    [Required]
    [StringLength(100)]
    public string Departamento { get; set; }
  }
}
