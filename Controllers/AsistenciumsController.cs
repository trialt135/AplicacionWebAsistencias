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
    public class AsistenciumsController : Controller
    {
        private readonly ManagmentRfidDbContext _context;

        public AsistenciumsController(ManagmentRfidDbContext context)
        {
            _context = context;
        }

    // GET: Asistenciums
    // GET: Materiums

    public async Task<IActionResult> GenerateExcel(int idRegistro)
    {
      // Obtén las asistencias filtradas por IdRegistro
      var asistencias = await _context.Asistencia
          .Where(a => a.IdRegistro == idRegistro)
          .Include(a => a.IdAlumnoNavigation) // Incluimos la relación con Alumno
          .ThenInclude(alumno => alumno.Rfids) // Incluimos la relación con Rfids
          .Include(a => a.IdRegistroNavigation) // Incluimos la relación con Registro
          .ToListAsync();

      // Ruta al archivo master.xlsm en wwwroot
      var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Master.xlsm");

      // Verificar que el archivo existe
      if (!System.IO.File.Exists(filePath))
      {
        return BadRequest("El archivo master.xlsm no existe en el servidor.");
      }

      // Cargar el archivo .xlsm utilizando EPPlus
      using (var package = new ExcelPackage(new FileInfo(filePath)))
      {
        // Verificar si el archivo tiene hojas
        if (package.Workbook.Worksheets.Count == 0)
        {
          return BadRequest("El archivo no tiene hojas de trabajo.");
        }

        // Listar los nombres de las hojas disponibles
        var sheetNames = package.Workbook.Worksheets.Select(ws => ws.Name).ToList();
        // Devolver la lista de hojas para debugging (si lo deseas)
        // return Json(sheetNames); 

        // Acceder a la hoja usando el nombre (en lugar de índice)
        var worksheet = package.Workbook.Worksheets["Asistencias"];

        // Si la hoja no existe, retornar un error
        if (worksheet == null)
        {
          return BadRequest("La hoja 'Asistencias' no se encuentra en el archivo.");
        }

        // Limpiar el contenido de la hoja antes de escribir nuevos datos
        worksheet.Cells.Clear(); // Esto borra todo el contenido de la hoja

        // Volver a escribir los encabezados si es necesario (opcional)
        worksheet.Cells[1, 1].Value = "Nombre del Alumno";
        worksheet.Cells[1, 2].Value = "Correo del Alumno";
        worksheet.Cells[1, 3].Value = "Fecha Registro";
        worksheet.Cells[1, 6].Value = "Presente";
        worksheet.Cells[1, 7].Value = "CodigoRFID";
        worksheet.Cells[1, 8].Value = "idAsistencia";

        // Comenzamos a modificar los datos en las celdas, comenzando desde la fila 2
        int row = 2; // La fila 1 tiene los encabezados
        foreach (var asistencia in asistencias)
        {
          // Columna A: Nombre del Alumno
          worksheet.Cells[row, 1].Value = asistencia.IdAlumnoNavigation?.IdUsuarioNavigation?.Nombre;

          // Columna B: Correo del Alumno
          worksheet.Cells[row, 2].Value = asistencia.IdAlumnoNavigation?.IdUsuarioNavigation?.Correo;

          // Columna C: Fecha del Registro
          worksheet.Cells[row, 3].Value = asistencia.IdRegistroNavigation.Fecha.ToString("dd/MM/yyyy");

          // Columna D: Presente (Columna F)
          worksheet.Cells[row, 6].Value = asistencia.Presente ? "Sí" : "No";

          // Columna E: Código RFID
          worksheet.Cells[row, 7].Value = asistencia.IdAlumnoNavigation.Rfids.FirstOrDefault()?.CodigoRfid;

          // Columna F: idAsistencia
          worksheet.Cells[row, 8].Value = asistencia.IdAsistencia;

          row++;
        }

        // Guardamos el archivo con los cambios en un nuevo archivo
        var updatedFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AsistenciasActualizadas.xlsm");
        package.SaveAs(new FileInfo(updatedFilePath));

        // Devolver el archivo actualizado para su descarga
        var fileBytes = System.IO.File.ReadAllBytes(updatedFilePath);
        return File(fileBytes, "application/vnd.ms-excel", "AsistenciasActualizadas.xlsm");
      }
    }






    public async Task<IActionResult> Index(int? idRegistro)
    {

      if (idRegistro == null)
      {
        return NotFound();  // Si no se proporciona un IdRegistro, retornamos un error
      }

      // Primero obtenemos el registro relacionado
      var registro = await _context.Registros
          .FirstOrDefaultAsync(r => r.IdRegistro == idRegistro); // Para obtener el registro relacionado y mostrar su fecha

      if (registro == null)
      {
        return NotFound();  // Si no encontramos el registro, retornamos un error
      }

      // Luego obtenemos las asistencias relacionadas con el IdRegistro
      var managmentRfidDbContext = await _context.Asistencia
          .Where(a => a.IdRegistro == idRegistro) // Filtra por el IdRegistro
          .Include(a => a.IdAlumnoNavigation)  // Incluye la relación con el Alumno
              .ThenInclude(alumno => alumno.IdUsuarioNavigation) // Incluye el Usuario del Alumno
          .Include(a => a.IdRegistroNavigation) // Incluye la relación con el Registro
          .ToListAsync();

      ViewData["RegistroFecha"] = registro.Fecha;  // Pasamos la fecha del registro a la vista
      ViewData["IdRegistro"] = idRegistro;         // Pasamos el idRegistro a la vista
      return View(managmentRfidDbContext);  // Pasamos las asistencias filtradas a la vista
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePresente(int idRegistro, string rfidCodigo)
    {
      // Buscar el alumno utilizando el código RFID
      var alumno = await _context.Alumnos
                                 .FirstOrDefaultAsync(a => a.Rfids.Any(r => r.CodigoRfid == rfidCodigo));

      if (alumno == null)
      {
        // Si el RFID no es válido, retornar la vista con una alerta roja
        TempData["AlertMessage"] = "RFID no válido.";
        TempData["AlertType"] = "danger";
        return RedirectToAction("Index", new { idRegistro });
      }

      // Buscar la asistencia del alumno para el registro
      var asistencia = await _context.Asistencia
                                     .FirstOrDefaultAsync(a => a.IdAlumno == alumno.IdAlumno && a.IdRegistro == idRegistro);

      if (asistencia == null)
      {
        // Si no se encuentra el registro de asistencia, retornar la vista con una alerta roja
        TempData["AlertMessage"] = "No se encuentra el registro de asistencia para el alumno.";
        TempData["AlertType"] = "danger";
        return RedirectToAction("Index", new { idRegistro });
      }

      // Actualizar el valor de 'Presente' a 1 (presente)
      asistencia.Presente = true;

      // Guardar los cambios en la base de datos
      _context.Update(asistencia);
      await _context.SaveChangesAsync();

      // Si todo se actualiza correctamente, retornar la vista con una alerta verde
      TempData["AlertMessage"] = "Asistencia registrada correctamente.";
      TempData["AlertType"] = "success";
      return RedirectToAction("Index", new { idRegistro });
    }



    // GET: Asistenciums/Details/5
    // GET: Asistenciums/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var asistencium = await _context.Asistencia
          .Include(a => a.IdAlumnoNavigation)
              .ThenInclude(alumno => alumno.IdUsuarioNavigation)
          .Include(a => a.IdRegistroNavigation)
          .FirstOrDefaultAsync(m => m.IdAsistencia == id);
      if (asistencium == null)
      {
        return NotFound();
      }

      return View(asistencium);
    }

    // GET: Asistenciums/Create
    public IActionResult Create()
    {
      // Incluye los datos necesarios para el formulario
      ViewData["IdAlumno"] = new SelectList(_context.Alumnos.Include(a => a.IdUsuarioNavigation), "IdAlumno", "IdUsuarioNavigation.Nombre");
      ViewData["IdRegistro"] = new SelectList(_context.Registros, "IdRegistro", "Fecha");
      return View();
    }


    // POST: Asistenciums/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAsistencia,IdRegistro,IdAlumno,Presente")] Asistencium asistencium)
        {
           
                _context.Add(asistencium);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
           
        }

        // GET: Asistenciums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asistencium = await _context.Asistencia.FindAsync(id);
            if (asistencium == null)
            {
                return NotFound();
            }
            ViewData["IdAlumno"] = new SelectList(_context.Alumnos, "IdAlumno", "IdAlumno", asistencium.IdAlumno);
            ViewData["IdRegistro"] = new SelectList(_context.Registros, "IdRegistro", "IdRegistro", asistencium.IdRegistro);
            return View(asistencium);
        }

        // POST: Asistenciums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAsistencia,IdRegistro,IdAlumno,Presente")] Asistencium asistencium)
        {
            if (id != asistencium.IdAsistencia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asistencium);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AsistenciumExists(asistencium.IdAsistencia))
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
            ViewData["IdAlumno"] = new SelectList(_context.Alumnos, "IdAlumno", "IdAlumno", asistencium.IdAlumno);
            ViewData["IdRegistro"] = new SelectList(_context.Registros, "IdRegistro", "IdRegistro", asistencium.IdRegistro);
            return View(asistencium);
        }

        // GET: Asistenciums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asistencium = await _context.Asistencia
                .Include(a => a.IdAlumnoNavigation)
                .Include(a => a.IdRegistroNavigation)
                .FirstOrDefaultAsync(m => m.IdAsistencia == id);
            if (asistencium == null)
            {
                return NotFound();
            }

            return View(asistencium);
        }
   
    // POST: Asistenciums/Delete/5
    [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asistencium = await _context.Asistencia.FindAsync(id);
            if (asistencium != null)
            {
                _context.Asistencia.Remove(asistencium);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AsistenciumExists(int id)
        {
            return _context.Asistencia.Any(e => e.IdAsistencia == id);
        }
    }



  }
