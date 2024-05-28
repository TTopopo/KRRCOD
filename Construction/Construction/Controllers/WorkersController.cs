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
    public class WorkersController : Controller
    {
        private readonly OobjectDBContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public WorkersController(OobjectDBContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        // GET: Workers
        public async Task<IActionResult> Index()
        {
            var oobjectDBContext = _context.Workers.Include(w => w.Foremen);
            return View(await oobjectDBContext.ToListAsync());
        }

        // GET: Workers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Workers == null)
            {
                return NotFound();
            }

            var worker = await _context.Workers
                .Include(w => w.Foremen)
                .FirstOrDefaultAsync(m => m.WorkerID == id);
            if (worker == null)
            {
                return NotFound();
            }

            return View(worker);
        }

        // GET: Workers/Create
        public IActionResult Create()
        {
            var workers = _context.Foremens
        .Select(c => new
        {
            c.ForemenID,
            DisplayValue = c.Name + ' ' + c.LastName 
        })
        .ToList();
            ViewData["ForemenId"] = new SelectList(workers, "ForemenID", "DisplayValue");
            return View();
        }

        // POST: Workers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkerID,Name,LastName,PhoneNamber,Position,Experience,ForemenId")] Worker worker)
        {
            if (ModelState.IsValid)
            {
                _context.Add(worker);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ForemenId"] = new SelectList(_context.Foremens, "ForemenID", "ForemenID", worker.ForemenId);
            return View(worker);
        }

        // GET: Workers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Workers == null)
            {
                return NotFound();
            }

            var worker = await _context.Workers.FindAsync(id);
            if (worker == null)
            {
                return NotFound();
            }
            var workers = _context.Foremens
        .Select(c => new
        {
            c.ForemenID,
            DisplayValue = c.Name + ' ' + c.LastName
        })
        .ToList();
            ViewData["ForemenId"] = new SelectList(workers, "ForemenID", "DisplayValue" , worker.ForemenId);
            return View(worker);
        }

        // POST: Workers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkerID,Name,LastName,PhoneNamber,Position,Experience,ForemenId")] Worker worker)
        {
            if (id != worker.WorkerID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(worker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkerExists(worker.WorkerID))
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
            ViewData["ForemenId"] = new SelectList(_context.Foremens, "ForemenID", "ForemenID", worker.ForemenId);
            return View(worker);
        }

        // GET: Workers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Workers == null)
            {
                return NotFound();
            }

            var worker = await _context.Workers
                .Include(w => w.Foremen)
                .FirstOrDefaultAsync(m => m.WorkerID == id);
            if (worker == null)
            {
                return NotFound();
            }

            return View(worker);
        }

        // POST: Workers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Workers == null)
            {
                return Problem("Entity set 'OobjectDBContext.Workers  is null.");
            }
            var worker = await _context.Workers.FindAsync(id);
            if (worker != null)
            {
                _context.Workers.Remove(worker);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkerExists(int id)
        {
            return (_context.Workers?.Any(e => e.WorkerID == id)).GetValueOrDefault();
        }
    }
}
