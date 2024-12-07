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
    public class MateriumsController : Controller
    {
        private readonly ManagmentRfidDbContext _context;

        public MateriumsController(ManagmentRfidDbContext context)
        {
            _context = context;
        }

    // GET: Materiums
    public async Task<IActionResult> Index()
    {
      // Obtén todos los objetos Materium e incluye las relaciones necesarias
      var managmentRfidDbContext = _context.Materia
          .Include(m => m.IdProfesorNavigation)       // Incluye el Profesor
          .ThenInclude(p => p.IdUsuarioNavigation)    // Incluye el Usuario del Profesor
          .ToListAsync();  // Obtén la lista de Materium

      return View(await managmentRfidDbContext);
    }


    // GET: Materiums/Details/5
    public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materium = await _context.Materia
                .Include(m => m.IdProfesorNavigation)
                .FirstOrDefaultAsync(m => m.IdMateria == id);
            if (materium == null)
            {
                return NotFound();
            }

            return View(materium);
        }

    // GET: Materiums/Create
    public IActionResult Create()
    {
      // Obtenemos los profesores y sus usuarios relacionados
      var profesoresConUsuario = _context.Profesors
          .Include(p => p.IdUsuarioNavigation)  // Aseguramos que se incluya el usuario relacionado
          .Select(p => new {
            p.IdProfesor,
            NombreCompleto = p.IdUsuarioNavigation.Nombre // Suponiendo que "Nombre" es una propiedad del modelo Usuario
          }).ToList();

      // Llenamos el ViewBag con el resultado para que se use en la vista
      ViewBag.IdProfesor = new SelectList(profesoresConUsuario, "IdProfesor", "NombreCompleto");

      return View();
    }


    // POST: Materiums/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMateria,NombreMateria,IdProfesor")] Materium materium)
        {
      var profesor = await _context.Profesors
                       .Include(p => p.IdUsuarioNavigation) // Si es necesario
                       .FirstOrDefaultAsync(p => p.IdProfesor == materium.IdProfesor);

      if (profesor != null)
      {
        materium.IdProfesorNavigation = profesor;
      }

      _context.Add(materium);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                 ViewData["IdProfesor"] = new SelectList(_context.Profesors, "IdProfesor", "IdProfesor", materium.IdProfesor);
                  return View(materium);
  

      
        }

        // GET: Materiums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materium = await _context.Materia.FindAsync(id);
            if (materium == null)
            {
                return NotFound();
            }
            ViewData["IdProfesor"] = new SelectList(_context.Profesors, "IdProfesor", "IdProfesor", materium.IdProfesor);
            return View(materium);
        }

        // POST: Materiums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMateria,NombreMateria,IdProfesor")] Materium materium)
        {
            if (id != materium.IdMateria)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(materium);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MateriumExists(materium.IdMateria))
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
            ViewData["IdProfesor"] = new SelectList(_context.Profesors, "IdProfesor", "IdProfesor", materium.IdProfesor);
            return View(materium);
        }

        // GET: Materiums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materium = await _context.Materia
                .Include(m => m.IdProfesorNavigation)
                .FirstOrDefaultAsync(m => m.IdMateria == id);
            if (materium == null)
            {
                return NotFound();
            }

            return View(materium);
        }

        // POST: Materiums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var materium = await _context.Materia.FindAsync(id);
            if (materium != null)
            {
                _context.Materia.Remove(materium);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MateriumExists(int id)
        {
            return _context.Materia.Any(e => e.IdMateria == id);
        }
    }
}
