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
    public class AlumnoesController : Controller
    {
        private readonly ManagmentRfidDbContext _context;

        public AlumnoesController(ManagmentRfidDbContext context)
        {
            _context = context;
        }

        // GET: Alumnoes
        public async Task<IActionResult> Index()
        {
            var managmentRfidDbContext = _context.Alumnos.Include(a => a.IdUsuarioNavigation);
            return View(await managmentRfidDbContext.ToListAsync());
        }

        // GET: Alumnoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno = await _context.Alumnos
                .Include(a => a.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdAlumno == id);
            if (alumno == null)
            {
                return NotFound();
            }

            return View(alumno);
        }

        // GET: Alumnoes/Create
        public IActionResult Create()
        {
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario");
            return View();
        }

        // POST: Alumnoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAlumno,IdUsuario,FechaIngreso")] Alumno alumno)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alumno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", alumno.IdUsuario);
            return View(alumno);
        }

    // GET: Alumnoes/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
      // Cargar el alumno con las materias asociadas
      var alumno = await _context.Alumnos
          .Include(a => a.Materias) // Incluir las materias relacionadas
          .FirstOrDefaultAsync(a => a.IdAlumno == id);

      if (alumno == null)
      {
        return NotFound();
      }

      // Cargar todas las materias disponibles para mostrarlas en los checkboxes
      var todasLasMaterias = await _context.Materia.ToListAsync();

      // Cargar los usuarios para el campo IdUsuario en el formulario
      ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", alumno.IdUsuario);

      // Crear un diccionario con los Ids de las materias ya asociadas
      var materiasSeleccionadas = alumno.Materias.Select(m => m.IdMateria).ToArray();

      // Pasar todas las materias y las seleccionadas a la vista
      ViewData["TodasLasMaterias"] = todasLasMaterias;
      ViewData["MateriasSeleccionadas"] = materiasSeleccionadas;

      return View(alumno);
    }




    // POST: Alumnoes/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("IdAlumno,IdUsuario,FechaIngreso")] Alumno alumno, int[] materias)
    {
      if (id != alumno.IdAlumno)
      {
        return NotFound();
      }

      
        try
        {
          var alumnoExistente = await _context.Alumnos
              .Include(a => a.Materias) // Cargar las materias asociadas
              .FirstOrDefaultAsync(a => a.IdAlumno == id);

          if (alumnoExistente == null)
          {
            return NotFound();
          }

          // Actualizamos las propiedades del alumno
          alumnoExistente.FechaIngreso = alumno.FechaIngreso;
          alumnoExistente.IdUsuario = alumno.IdUsuario;

          // Actualizar la relación con las materias seleccionadas
          alumnoExistente.Materias.Clear(); // Limpiar las materias actuales

          if (materias != null)
          {
            foreach (var materiaId in materias)
            {
              var materia = await _context.Materia.FindAsync(materiaId);
              if (materia != null)
              {
                alumnoExistente.Materias.Add(materia); // Asociar las materias seleccionadas
              }
            }
          }

          _context.Update(alumnoExistente);
          await _context.SaveChangesAsync();

          return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!AlumnoExists(alumno.IdAlumno))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
      }

    
    


    // GET: Alumnoes/Delete/5
    public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno = await _context.Alumnos
                .Include(a => a.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdAlumno == id);
            if (alumno == null)
            {
                return NotFound();
            }

            return View(alumno);
        }

        // POST: Alumnoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alumno = await _context.Alumnos.FindAsync(id);
            if (alumno != null)
            {
                _context.Alumnos.Remove(alumno);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlumnoExists(int id)
        {
            return _context.Alumnos.Any(e => e.IdAlumno == id);
        }
    }
}
