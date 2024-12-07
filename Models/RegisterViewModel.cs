namespace AspnetCoreMvcFull.Models
{
  public class RegisterViewModel
  {
    public string Nombre { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string Contraseña { get; set; } = string.Empty;

    public int IdRol { get; set; }
  }

}
