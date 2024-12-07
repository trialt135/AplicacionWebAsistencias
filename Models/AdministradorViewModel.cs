using System.ComponentModel.DataAnnotations;

namespace AspnetCoreMvcFull.Models
{
  public class AdministradorViewModel
  {
    [Required]
    public int IdUsuario { get; set; }

    [Required]
    [StringLength(100)]
    public string AreaTrabajo { get; set; }
  }
}
