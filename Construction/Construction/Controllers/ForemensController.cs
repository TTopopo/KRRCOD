using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Construction.Models;
using Microsoft.AspNetCore.Authorization;

namespace Construction.Controllers
{
    [Authorize(Roles = "admin")]
    public class ForemensController : Controller
    {
        private readonly OobjectDBContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public ForemensController(OobjectDBContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        // GET: Foremen
        public async Task<IActionResult> Index()
        {
            return _context.Foremens != null ?
                          View(await _context.Foremens.ToListAsync()) :
                          Problem("Entity set 'OobjectDBContext.Foremens'  is null.");
        }

        // GET: Foremen/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Foremens == null)
            {
                return NotFound();
            }

            var foremen = await _context.Foremens
                .FirstOrDefaultAsync(m => m.ForemenID == id);
            if (foremen == null)
            {
                return NotFound();
            }

            return View(foremen);
        }

        // GET: Foremen/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Foremen/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ForemenID,Name,LastName,PhoneNamber,Qualification,Specialization,Skills")] Foremen foremen)
        {
            if (ModelState.IsValid)
            {
                _context.Add(foremen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(foremen);
        }

        // GET: Foremen/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Foremens == null)
            {
                return NotFound();
            }

            var foremen = await _context.Foremens.FindAsync(id);
            if (foremen == null)
            {
                return NotFound();
            }
            return View(foremen);
        }

        // POST: Foremen/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ForemenID,Name,LastName,PhoneNamber,Qualification,Specialization,Skills")] Foremen foremen)
        {
            if (id != foremen.ForemenID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(foremen);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ForemenExists(foremen.ForemenID))
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
            return View(foremen);
        }

        // GET: Foremen/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Foremens == null)
            {
                return NotFound();
            }

            var foremen = await _context.Foremens
                .FirstOrDefaultAsync(m => m.ForemenID == id);
            if (foremen == null)
            {
                return NotFound();
            }

            return View(foremen);
        }

        // POST: Foremen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Foremens == null)
            {
                return Problem("Entity set 'OobjectDBContext.Foremens'  is null.");
            }
            var foremen = await _context.Foremens.FindAsync(id);
            if (foremen != null)
            {
                _context.Foremens.Remove(foremen);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ForemenExists(int id)
        {
            return (_context.Foremens?.Any(e => e.ForemenID == id)).GetValueOrDefault();
        }
    }
}
