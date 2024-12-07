
using AspnetCoreMvcFull.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class AuthController : Controller
{
  private readonly ManagmentRfidDbContext _context;

  public AuthController(ManagmentRfidDbContext context)
  {
    _context = context;
  }

  // Mostrar la vista de registro
  public IActionResult RegisterBasic()
  {
    return View();
  }

  // Procesar el registro
  [HttpPost]
  public IActionResult RegisterBasic(RegisterViewModel registerViewModel)
  {
    if (ModelState.IsValid)
{
    // Verificar si el correo ya existe
    var existingUser = _context.Usuarios.FirstOrDefault(u => u.Correo == registerViewModel.Correo);
    if (existingUser != null)
    {
        TempData["ErrorMessage"] = "El correo electrónico ya está registrado.";
        return View(registerViewModel);
    }

    // Crear un nuevo usuario a partir de RegisterViewModel
    var usuario = new Usuario
    {
        Nombre = registerViewModel.Nombre,
        Correo = registerViewModel.Correo,
        Contraseña = registerViewModel.Contraseña, // Normalmente deberías hashear la contraseña antes de guardarla
        IdRol = registerViewModel.IdRol
    };
      

    // Guardar el nuevo usuario en la base de datos
    _context.Usuarios.Add(usuario);
    _context.SaveChanges();

    // Imprimir en la consola los detalles del usuario y su id

    // Redirigir al registro adicional según el rol
    if (registerViewModel.IdRol == 1) // Si es Administrador (IdRol = 1)
    {
        
        return RedirectToAction("RegisterAdministrador", new { idUsuario = usuario.IdUsuario });
    }
    else if (registerViewModel.IdRol == 2) // Si es Profesor (IdRol = 2)
    {
        return RedirectToAction("RegisterProfesor", new { idUsuario = usuario.IdUsuario });
    }
    else if (registerViewModel.IdRol == 3) // Si es Alumno (IdRol = 3)
    {
        return RedirectToAction("RegisterAlumno", new { idUsuario = usuario.IdUsuario });
    }

    // En caso de rol desconocido, redirigir al inicio o a una vista general
    return RedirectToAction("Index", "Home");
}


    // Si los datos no son válidos, regresar a la vista de registro
    return View(registerViewModel);
  }


  public IActionResult RegisterAdministrador(int idUsuario)
  {    
    return View(new AdministradorViewModel { IdUsuario = idUsuario });
  }

  // Vista para registrar Profesor
  public IActionResult RegisterProfesor(int idUsuario)
  {
    // Obtener el usuario para que pueda ser editado (opcional)
    var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == idUsuario);
    if (usuario == null)
    {
      return NotFound();
    }

    // Mostrar vista específica para Profesor
    return View(new ProfesorViewModel { IdUsuario = idUsuario });
  }

  // Vista para registrar Alumno
  public IActionResult RegisterAlumno(int idUsuario)
  {

    // Mostrar vista específica para Alumno
    return View(new AlumnoViewModel { IdUsuario = idUsuario });
  }

  // Procesar el registro del Administrador
  [HttpPost]
  public IActionResult RegisterAdministrador(AdministradorViewModel viewModel)
  {
    if (ModelState.IsValid)
    {
      var administrador = new Administrador
      {
        IdAdministrador = viewModel.IdUsuario,
        IdUsuario = viewModel.IdUsuario,
        AreaTrabajo = viewModel.AreaTrabajo // Ejemplo de campo específico
      };

      _context.Administradors.Add(administrador);
      _context.SaveChanges();

      TempData["SuccessMessage"] = "Administrador registrado correctamente.";
      return RedirectToAction("Index", "Home");
    }

    return View(viewModel);
  }

  // Procesar el registro del Profesor
  [HttpPost]
  public IActionResult RegisterProfesor(ProfesorViewModel viewModel)
  {
    if (ModelState.IsValid)
    {
      var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == viewModel.IdUsuario);


      var profesor = new Profesor
      {
        IdUsuario = viewModel.IdUsuario,
        Departamento = viewModel.Departamento,
        IdProfesor = viewModel.IdUsuario,
        IdUsuarioNavigation = usuario

      };

      _context.Profesors.Add(profesor);
      _context.SaveChanges();

      TempData["SuccessMessage"] = "Profesor registrado correctamente.";
      return RedirectToAction("Index", "Home");
    }

    return View(viewModel);
  }

  // Procesar el registro del Alumno
  [HttpPost]
  public IActionResult RegisterAlumno(AlumnoViewModel viewModel)
  {
    if (ModelState.IsValid)
    {
      // Buscar el Usuario correspondiente por su IdUsuario
      var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == viewModel.IdUsuario);

      // Verificar que el Usuario exista
      if (usuario == null)
      {
        ModelState.AddModelError("", "Usuario no encontrado.");
        return View(viewModel);
      }

      // Crear el objeto Alumno
      var alumno = new Alumno
      {
        IdUsuario = viewModel.IdUsuario, // Guardamos el Id del Usuario
        IdUsuarioNavigation = usuario, // Asignamos el Usuario a la propiedad de navegación
        FechaIngreso = viewModel.FechaIngreso, // Asignamos la fecha de ingreso
        IdAlumno = viewModel.IdUsuario // Usando IdUsuario también como IdAlumno, si es así como está definido
      };

      // Crear el objeto Rfid
      var rfid = new Rfid
      {
        CodigoRfid = viewModel.CodigoRfid, // Asignamos el código RFID del viewModel
        IdAlumno = alumno.IdAlumno, // Asignamos el Id del Alumno al Rfid
        IdAlumnoNavigation = alumno // Establecemos la propiedad de navegación en el Rfid
      };

      // Añadimos el RFID a la colección de Rfids del Alumno
      alumno.Rfids.Add(rfid);

      // Agregar el Alumno al contexto y guardar los cambios
      _context.Alumnos.Add(alumno); // Agregar el Alumno al contexto
      _context.Rfids.Add(rfid); // Agregar el Rfid al contexto

      // Guardar los cambios en la base de datos
      _context.SaveChanges();

      // Mostrar mensaje de éxito
      TempData["SuccessMessage"] = "Alumno registrado correctamente.";

      // Redirigir al Index de la vista Home
      return RedirectToAction("Index", "Home");
    }

    // Si el modelo no es válido, regresar la vista con el modelo para corregir errores
    return View(viewModel);
  }



  public IActionResult LoginBasic()
  {
    return View();
  }

  // Procesar el login
  [HttpPost]
  public IActionResult LoginBasic(LoginViewModel loginViewModel)
  {
    if (ModelState.IsValid)
    {
      // Verificar si el usuario existe en la base de datos
      var usuario = _context.Usuarios
          .FirstOrDefault(u => u.Correo == loginViewModel.Correo && u.Contraseña == loginViewModel.Contraseña); // En una aplicación real, se recomienda usar un hash para las contraseñas.

      if (usuario == null)
      {
        TempData["ErrorMessage"] = "Credenciales incorrectas.";
        return View(loginViewModel);
      }

      // Aquí, podrías agregar lógica para manejar la sesión del usuario o un JWT, por ejemplo.

      // Redirigir al usuario a la página de inicio después de un login exitoso
      return RedirectToAction("Index", "Home");
    }

    // Si los datos no son válidos, regresar a la vista de login
    return View(loginViewModel);
  }
}



