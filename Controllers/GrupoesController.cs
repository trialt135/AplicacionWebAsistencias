using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AspnetCoreMvcFull.Models;
using OfficeOpenXml;

namespace AspnetCoreMvcFull.Controllers
{
  public class GrupoesController : Controller
  {
    private readonly ManagmentRfidDbContext _context;

    public GrupoesController(ManagmentRfidDbContext context)
    {
      _context = context;
    }

    // GET: Grupoes
    public async Task<IActionResult> Index()
    {
      // Incluir las relaciones necesarias para cargar Materium -> Profesor -> Usuario
      var managmentRfidDbContext = _context.Grupos
          .Include(g => g.IdMateriaNavigation)  // Incluir la relación con Materium
          .ThenInclude(m => m.IdProfesorNavigation)  // Incluir la relación con Profesor
          .ThenInclude(p => p.IdUsuarioNavigation);  // Incluir la relación con Usuario

      // Obtener los datos y pasarlos a la vista
      return View(await managmentRfidDbContext.ToListAsync());
    }


    // GET: Grupoes/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var grupo = await _context.Grupos
          .Include(g => g.IdMateriaNavigation)
          .FirstOrDefaultAsync(m => m.IdGrupo == id);
      if (grupo == null)
      {
        return NotFound();
      }

      return View(grupo);
    }

    // GET: Grupoes/Create
    public IActionResult Create()
    {
      // Obtener todas las materias y pasarlas a la vista
      var materias = _context.Materia.ToList();
      ViewBag.IdMateria = new SelectList(materias, "IdMateria", "NombreMateria"); // Asegúrate de que "NombreMateria" sea el nombre correcto de la propiedad

      return View();
    }
    public IActionResult GetAlumnosByMateria(int idMateria)
    {
      // Obtener los alumnos que están asociados con la materia seleccionada
      var alumnos = _context.Alumnos
                            .Include(a => a.IdUsuarioNavigation) // Incluir el Usuario relacionado
                            .Where(a => a.Materias.Any(m => m.IdMateria == idMateria)) // Relación entre Alumnos y Materias
                            .ToList();

      // Devolver los alumnos como un objeto JSON
      return Json(alumnos.Select(a => new
      {
        IdAlumno = a.IdAlumno,
        Nombre = a.IdUsuarioNavigation.Nombre // Nombre del usuario relacionado con el alumno
      }));
    }

    // Acción de Listado de Grupos (Opcional)





    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdGrupo,NombreGrupo,IdMateria,FechaInicio,FechaFin")] Grupo grupo, DateTime fechaInicio, DateTime fechaFin, string[] diasSeleccionados, string[] alumnosSeleccionados)
    {
      // Crear el grupo
      _context.Add(grupo);
      await _context.SaveChangesAsync();

      // Obtener el grupo recién creado
      var grupoCreado = await _context.Grupos
          .FirstOrDefaultAsync(g => g.IdGrupo == grupo.IdGrupo);

      if (grupoCreado == null)
      {
        ModelState.AddModelError("", "No se pudo encontrar el grupo después de la creación.");
        return View(grupo);
      }
     
        // Filtrar los valores nulos o vacíos antes de convertirlos a enteros
        var alumnoIds = alumnosSeleccionados
                        .Where(id => !string.IsNullOrWhiteSpace(id)) // Eliminar valores nulos o vacíos
                        .Select(id => int.Parse(id)) // Convertir los valores a enteros
                        .ToList();

        // Buscar los alumnos que corresponden a esos IDs
        var alumnos = await _context.Alumnos
                                     .Where(a => alumnoIds.Contains(a.IdAlumno)) // Filtrar los alumnos por los IDs
                                     .ToListAsync();

        // Aquí puedes continuar con el resto de la lógica para crear el grupo y los demás campos.
      
      // Lista para los registros
      var registros = new List<Registro>();

      // Crear registros solo para los días seleccionados dentro del rango de fechas
      for (var fecha = fechaInicio; fecha <= fechaFin; fecha = fecha.AddDays(1))
      {
        // Obtener el nombre del día de la semana (por ejemplo: Monday, Tuesday)
        var diaDeLaSemana = fecha.DayOfWeek.ToString();

        // Verificar si el día de la semana está seleccionado
        if (diasSeleccionados.Contains(diaDeLaSemana))
        {
          // Convertir fecha de DateTime a DateOnly
          var registro = new Registro
          {
            IdGrupo = grupo.IdGrupo, // Asignar el Id del grupo
            Fecha = DateOnly.FromDateTime(fecha), // Convertir de DateTime a DateOnly
            IdGrupoNavigation = grupoCreado // Asignar la propiedad de navegación
          };

          // Añadir el registro a la base de datos
          _context.Add(registro);
          await _context.SaveChangesAsync(); // Guardamos para tener el IdRegistro disponible

          // Crear las entradas de Asistencium para cada alumno
          foreach (var alumno in alumnos)
          {
            grupoCreado.IdAlumnos.Add(alumno);  // Relacionar el alumno con el grupo

            var asistencium = new Asistencium
            {
              IdRegistro = registro.IdRegistro, // Relacionar el Asistencium con el Registro
              IdAlumno = alumno.IdAlumno, // Relacionar el Asistencium con el Alumno
              Presente = false, // Puedes ajustar este valor (true/false)
              IdAlumnoNavigation = alumno, // Relacionar con el alumno
              IdRegistroNavigation = registro // Relacionar con el registro
            };

            // Añadir la asistencia a la base de datos
            _context.Add(asistencium);
          }
        }
      }

      // Guardar todos los registros y asistencias creados
      await _context.SaveChangesAsync();

      return RedirectToAction(nameof(Index)); // Redirigir al índice
    }




    // GET: Grupoes/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var grupo = await _context.Grupos.FindAsync(id);
      if (grupo == null)
      {
        return NotFound();
      }
      ViewData["IdMateria"] = new SelectList(_context.Materia, "IdMateria", "IdMateria", grupo.IdMateria);
      return View(grupo);
    }

    // POST: Grupoes/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("IdGrupo,NombreGrupo,IdMateria")] Grupo grupo)
    {
      if (id != grupo.IdGrupo)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(grupo);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!GrupoExists(grupo.IdGrupo))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
        return RedirectToAction(nameof(Index));
      }
      ViewData["IdMateria"] = new SelectList(_context.Materia, "IdMateria", "IdMateria", grupo.IdMateria);
      return View(grupo);
    }
   [HttpPost]
public async Task<IActionResult> GenerarExcel(int idGrupo)
{
    // Obtener el grupo basado en el idGrupo
    var grupo = await _context.Grupos
        .Include(g => g.Registros) // Incluir registros
        .ThenInclude(r => r.Asistencia) // Incluir asistencias de los registros
        .Include(g => g.IdAlumnos) // Incluir los alumnos del grupo
        .ThenInclude(a => a.IdUsuarioNavigation) // Incluir los datos del usuario del alumno (para obtener nombre)
        .FirstOrDefaultAsync(g => g.IdGrupo == idGrupo);


    if (grupo == null)
    {
        return BadRequest("Grupo no encontrado.");
    }

      var nombreGrupo = grupo.NombreGrupo;



      // Información sobre los registros y fechas
      var registros = grupo.Registros.OrderBy(r => r.Fecha).ToList();

    // Crear un archivo Excel en memoria
    using (var package = new ExcelPackage())
    {
        var worksheet = package.Workbook.Worksheets.Add("Asistencias");



        // Colocar encabezados
        worksheet.Cells[2, 3].Value = "GRUPO: ";
        worksheet.Cells[2, 3].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[2, 3].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[2, 3].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[2, 3].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[2, 3].Style.Font.Bold = true;
        worksheet.Cells[2, 3].Style.Font.Color.SetColor(System.Drawing.Color.White);
        worksheet.Cells[2, 3].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        worksheet.Cells[2, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);

        worksheet.Cells[2, 4].Value = nombreGrupo;
        worksheet.Cells[2, 4].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[2, 4].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[2, 4].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[2, 4].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[2, 4].Style.Font.Bold = true;
        worksheet.Cells[2, 4].Style.Font.Color.SetColor(System.Drawing.Color.White);
        worksheet.Cells[2, 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        worksheet.Cells[2, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);









        worksheet.Cells[2, 5].Value = "PROFESOR";
        worksheet.Cells[2, 5].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[2, 5].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[2, 5].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[2, 5].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[2, 5].Style.Font.Bold = true;
        worksheet.Cells[2, 5].Style.Font.Color.SetColor(System.Drawing.Color.White);
        worksheet.Cells[2, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        worksheet.Cells[2, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);





        // Fila 6 en adelante (comenzamos desde la fila 6)
        int row = 6;
        
        // Colocar encabezados
        worksheet.Cells[row, 1].Value = "ASISTENCIA ID";
        worksheet.Cells[row, 1].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[row, 1].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[row, 1].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[row, 1].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[row, 1].Style.Font.Bold = true;
        worksheet.Cells[row, 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
        worksheet.Cells[row, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);







        worksheet.Cells[row, 2].Value = "NOMBRE COMPLETO";
        worksheet.Cells[row, 2].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[row, 2].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[row, 2].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[row, 2].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[row, 2].Style.Font.Bold = true;
        worksheet.Cells[row, 2].Style.Font.Color.SetColor(System.Drawing.Color.White);
        worksheet.Cells[row, 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        worksheet.Cells[row, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);

        // Colocar las fechas de los registros como encabezados a partir de la columna C
        int col = 3;
        foreach (var registro in registros)
        {
            worksheet.Cells[5, col].Value = registro.Fecha.ToString("yyyy-MM-dd"); // Fecha de cada registro
            col++;
        }

        // Obtener los alumnos que están en el grupo
        var alumnos = grupo.IdAlumnos.ToList();

        // Iterar sobre cada alumno y agregar sus datos
        row = 7; // Comenzamos desde la fila 7 (debido a los encabezados)
        foreach (var alumno in alumnos)
        {
            // Colocar el "ASISTENCIA ID" y "NOMBRE COMPLETO"
            worksheet.Cells[row, 1].Value = alumno.IdAlumno; // Asistencia ID (puedes personalizarlo según tu estructura)
            worksheet.Cells[row, 2].Value = alumno.IdUsuarioNavigation.Nombre; // Nombre completo del alumno

            // Iterar sobre las fechas de los registros y agregar la asistencia
            col = 3;
            foreach (var registro in registros)
            {
                // Buscar la asistencia del alumno para la fecha actual
                var asistencia = registro.Asistencia.FirstOrDefault(a => a.IdAlumno == alumno.IdAlumno);
            if (asistencia != null)
            {
              // Asignar el valor de la celda
              worksheet.Cells[row, col].Value = asistencia.Presente ? "Presente" : "Ausente";

              // Definir el estilo de la celda
              var cell = worksheet.Cells[row, col];

              // Establecer bordes negros alrededor de la celda
              cell.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
              cell.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
              cell.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
              cell.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

              // Establecer la fuente en negrita y el color blanco para el texto
              cell.Style.Font.Bold = true;
              cell.Style.Font.Color.SetColor(System.Drawing.Color.White);

              // Definir el color de fondo dependiendo de si es "Presente" o "Ausente"
              if (asistencia.Presente)
              {
                // Fondo verde para presente
                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);
              }
              else
              {
                // Fondo rojo para ausente
                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
              }
            }



            col++; // Avanzar a la siguiente columna
            }

            row++; // Avanzar a la siguiente fila para el siguiente alumno
        }

        // Ajustar el ancho de las columnas para que se ajuste al contenido
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        // Configurar la respuesta HTTP para la descarga del archivo Excel
        var fileName = $"Asistencias_Grupo_{idGrupo}.xlsx";
        var fileStream = new MemoryStream(package.GetAsByteArray());
        return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}



    // GET: Grupoes/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var grupo = await _context.Grupos
          .Include(g => g.IdMateriaNavigation)
          .FirstOrDefaultAsync(m => m.IdGrupo == id);
      if (grupo == null)
      {
        return NotFound();
      }

      return View(grupo);
    }
   


    // POST: Grupoes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var grupo = await _context.Grupos.FindAsync(id);
      if (grupo != null)
      {
        _context.Grupos.Remove(grupo);
      }

      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool GrupoExists(int id)
    {
      return _context.Grupos.Any(e => e.IdGrupo == id);
    }
    }









 




  }
