using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AspnetCoreMvcFull.Models;

namespace AspnetCoreMvcFull.Controllers
{
    public class RegistroesController : Controller
    {
        private readonly ManagmentRfidDbContext _context;

        public RegistroesController(ManagmentRfidDbContext context)
        {
            _context = context;
        }

    // GET: Registroes
    public async Task<IActionResult> Index(int? idGrupo)
    {
      if (idGrupo == null)
      {
        return NotFound();  // Si no se proporciona un IdGrupo, retornamos un error
      }

      // Filtrar los registros según el IdGrupo
      var managmentRfidDbContext = _context.Registros
          .Where(r => r.IdGrupo == idGrupo)  // Filtra por IdGrupo
          .Include(r => r.IdGrupoNavigation)  // Incluye la relación con Grupo
          .ToListAsync();

      var registros = await managmentRfidDbContext;

      if (registros == null || registros.Count == 0)
      {
        return NotFound();  // Si no se encuentran registros, retornamos un error
      }

      return View(registros);  // Devuelve los registros relacionados con el IdGrupo
    }

    // GET: Registroes/Details/5
    public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registro = await _context.Registros
                .Include(r => r.IdGrupoNavigation)
                .FirstOrDefaultAsync(m => m.IdRegistro == id);
            if (registro == null)
            {
                return NotFound();
            }

            return View(registro);
        }

        // GET: Registroes/Create
        public IActionResult Create()
        {
            ViewData["IdGrupo"] = new SelectList(_context.Grupos, "IdGrupo", "IdGrupo");
            return View();
        }

    // POST: Registroes/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdRegistro,IdGrupo,Fecha")] Registro registro)
    {
     
      
        // Obtener el grupo seleccionado
        var grupoSeleccionado = await _context.Grupos
            .FirstOrDefaultAsync(g => g.IdGrupo == registro.IdGrupo);

        if (grupoSeleccionado == null)
        {
          // Si no se encuentra el grupo, se muestra un error
          ModelState.AddModelError("", "El grupo seleccionado no es válido.");
          return View(registro);
        }
      registro.IdGrupoNavigation = grupoSeleccionado;

      // Crear el registro en la base de datos
      _context.Add(registro);
        await _context.SaveChangesAsync();

        // Establecer el grupo en el registro

        // Obtener todos los alumnos
        var alumnos = _context.Alumnos.ToList();  // O agrega alguna condición si lo necesitas

        // Crear las entradas de Asistencium para cada alumno
        foreach (var alumno in alumnos)
        {
          var asistencium = new Asistencium
          {
            IdRegistro = registro.IdRegistro,
            IdAlumno = alumno.IdAlumno,
            Presente = false,  // Puedes ajustar este valor (true/false)

            // Establecer las propiedades de navegación
            IdAlumnoNavigation = alumno,
            IdRegistroNavigation = registro
          };

          _context.Add(asistencium);
        }

        // Guardar los cambios de Asistencium
        await _context.SaveChangesAsync();

        // Redirigir a la página de índice
        return RedirectToAction(nameof(Index));
      }

    


    // GET: Registroes/Edit/5
    public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registro = await _context.Registros.FindAsync(id);
            if (registro == null)
            {
                return NotFound();
            }
            ViewData["IdGrupo"] = new SelectList(_context.Grupos, "IdGrupo", "IdGrupo", registro.IdGrupo);
            return View(registro);
        }

        // POST: Registroes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRegistro,IdGrupo,Fecha")] Registro registro)
        {
            if (id != registro.IdRegistro)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(registro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistroExists(registro.IdRegistro))
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
            ViewData["IdGrupo"] = new SelectList(_context.Grupos, "IdGrupo", "IdGrupo", registro.IdGrupo);
            return View(registro);
        }

        // GET: Registroes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registro = await _context.Registros
                .Include(r => r.IdGrupoNavigation)
                .FirstOrDefaultAsync(m => m.IdRegistro == id);
            if (registro == null)
            {
                return NotFound();
            }

            return View(registro);
        }

        // POST: Registroes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var registro = await _context.Registros.FindAsync(id);
            if (registro != null)
            {
                _context.Registros.Remove(registro);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegistroExists(int id)
        {
            return _context.Registros.Any(e => e.IdRegistro == id);
        }
    }
}
