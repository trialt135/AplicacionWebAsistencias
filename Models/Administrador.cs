using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AspnetCoreMvcFull.Models;

public partial class Administrador
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Configura el autoincremento
  public int IdAdministrador { get; set; }

    public int IdUsuario { get; set; }

    public string? AreaTrabajo { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
