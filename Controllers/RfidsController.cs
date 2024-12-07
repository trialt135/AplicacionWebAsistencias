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
    public class RfidsController : Controller
    {
        private readonly ManagmentRfidDbContext _context;

        public RfidsController(ManagmentRfidDbContext context)
        {
            _context = context;
        }

    // GET: Rfids
    // GET: Rfids/Index
    public async Task<IActionResult> Index()
    {
      // Incluir las relaciones: Alumno -> Usuario -> RFID (si existe)
      var alumnosConRfid = await _context.Alumnos
          .Include(a => a.IdUsuarioNavigation) // Incluir la relación con Usuario
          .Include(a => a.Rfids)               // Incluir los RFIDs asociados
          .ToListAsync();

      return View(alumnosConRfid);
    }


    // GET: Rfids/Details/5
    public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rfid = await _context.Rfids
                .Include(r => r.IdAlumnoNavigation)
                .FirstOrDefaultAsync(m => m.IdRfid == id);
            if (rfid == null)
            {
                return NotFound();
            }

            return View(rfid);
        }

        // GET: Rfids/Create
        public IActionResult Create()
        {
      ViewData["IdAlumno"] = new SelectList(_context.Alumnos.Include(a => a.IdUsuarioNavigation), "IdAlumno", "IdUsuarioNavigation.Nombre"); return View();
        }

    // POST: Rfids/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdRfid,CodigoRfid,IdAlumno")] Rfid rfid)
    {
      // Cargar el Alumno desde la base de datos usando el IdAlumno
      var alumno = await _context.Alumnos
          .FirstOrDefaultAsync(a => a.IdAlumno == rfid.IdAlumno);

      if (alumno != null)
      {
        // Asignar el Alumno al campo de navegación IdAlumnoNavigation
        rfid.IdAlumnoNavigation = alumno;
        alumno.Rfids.Add(rfid);

        // Guardar el Rfid en la base de datos
        _context.Add(rfid);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
      }

      // Si el Alumno no existe, retornar un error o alguna respuesta apropiada
      ModelState.AddModelError("", "Alumno no encontrado");
      return View(rfid);
    }

   




    // GET: Rfids/Edit/5
    public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rfid = await _context.Rfids.FindAsync(id);
            if (rfid == null)
            {
                return NotFound();
            }
            ViewData["IdAlumno"] = new SelectList(_context.Alumnos, "IdAlumno", "IdAlumno", rfid.IdAlumno);
            return View(rfid);
        }

        // POST: Rfids/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRfid,CodigoRfid,IdAlumno")] Rfid rfid)
        {
            if (id != rfid.IdRfid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rfid);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RfidExists(rfid.IdRfid))
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
            ViewData["IdAlumno"] = new SelectList(_context.Alumnos, "IdAlumno", "IdAlumno", rfid.IdAlumno);
            return View(rfid);
        }

        // GET: Rfids/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rfid = await _context.Rfids
                .Include(r => r.IdAlumnoNavigation)
                .FirstOrDefaultAsync(m => m.IdRfid == id);
            if (rfid == null)
            {
                return NotFound();
            }

            return View(rfid);
        }

        // POST: Rfids/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rfid = await _context.Rfids.FindAsync(id);
            if (rfid != null)
            {
                _context.Rfids.Remove(rfid);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RfidExists(int id)
        {
            return _context.Rfids.Any(e => e.IdRfid == id);
        }
    }
}
