using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Construction.Models;
using OfficeOpenXml;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace Construction.Controllers
{
     
    public class OobjectsController : Controller
    {
        private readonly OobjectDBContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public OobjectsController(OobjectDBContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }   

        // GET: Oobjects
        public async Task<IActionResult> Index()
        {
            var oobjectDBContext = _context.Oobjects.Include(o => o.Customer).Include(o => o.Foremen);
            Dictionary<int, byte[]> Photos = new Dictionary<int, byte[]>();

            foreach (Oobject oobject in oobjectDBContext)
            {
                if (oobject.Photo != null)
                {
                    byte[] photodata = System.IO.File.ReadAllBytes(_appEnvironment.WebRootPath + oobject.Photo);
                    Photos[oobject.OobjectID] = photodata;
                }
            }

            ViewBag.Photos = Photos; 
            return View(await oobjectDBContext.ToListAsync());
        }

        // GET: Oobjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Oobjects == null)
            {
                return NotFound();
            }

            var oobject = await _context.Oobjects
                .Include(o => o.Customer)
                .Include(o => o.Foremen)
                .FirstOrDefaultAsync(m => m.OobjectID == id);
            if (oobject == null)
            {
                return NotFound();
            }

            return View(oobject);
        }

        // GET: Oobjects/Create
        [Authorize(Roles = "admin,guest")]
        public IActionResult Create()
        {
            var customers = _context.Customers
        .Select(c => new
        {
            c.CustomerID,
            DisplayValue = c.Name + ' ' + c.LastName
        })
        .ToList();
            var foremens = _context.Foremens
        .Select(c => new
        {
            c.ForemenID,
            DisplayValue = c.Name + ' ' + c.LastName
        })
        .ToList();
            ViewData["CustomerId"] = new SelectList(customers, "CustomerID", "DisplayValue");
            ViewData["ForemenId"] = new SelectList(foremens, "ForemenID", "DisplayValue");
            return View();
        }

        // POST: Oobjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,guest")]
        public async Task<IActionResult> Create([Bind("OobjectID,Title,Adress,Type,Status,Photo,ForemenId,CustomerId")] Oobject oobject, IFormFile upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null) // если файл загружен, сохраняем его
                {
                    string path = "/Files/" + upload.FileName;
                    using (var fileStream = new
                   FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await upload.CopyToAsync(fileStream);
                    }
                    oobject.Photo = path;
                }
                _context.Add(oobject);
                await _context.SaveChangesAsync();
                if (!oobject.Photo.IsNullOrEmpty())
                {
                    byte[] photodata =
                    System.IO.File.ReadAllBytes(_appEnvironment.WebRootPath + oobject.Photo);
                    ViewBag.Photodata = photodata;
                }
                else
                {
                    ViewBag.Photodata = null;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerID", "CustomerID", oobject.CustomerId);
            ViewData["ForemenId"] = new SelectList(_context.Foremens, "ForemenID", "ForemenID", oobject.ForemenId);
            return View(oobject);
        }

        // GET: Oobjects/Edit/5
        [Authorize(Roles = "admin,guest")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Oobjects == null)
            {
                return NotFound();
            }

            var oobject = await _context.Oobjects.FindAsync(id);
            if (oobject == null)
            {
                return NotFound();
            }
            if (!oobject.Photo.IsNullOrEmpty())
            {
                byte[] photodata =
               System.IO.File.ReadAllBytes(_appEnvironment.WebRootPath + oobject.Photo);
                ViewBag.Photodata = photodata;
            }
            else
            {
                ViewBag.Photodata = null;
            }
            var customers = _context.Customers
        .Select(c => new
        {
            c.CustomerID,
            DisplayValue = c.Name + ' ' + c.LastName
        })
        .ToList();
            var foremens = _context.Foremens
        .Select(c => new
        {
            c.ForemenID,
            DisplayValue = c.Name + ' ' + c.LastName
        })
        .ToList();
            ViewData["CustomerId"] = new SelectList(customers, "CustomerID", "DisplayValue", oobject.CustomerId);
            ViewData["ForemenId"] = new SelectList(foremens, "ForemenID", "DisplayValue", oobject.ForemenId); 
            return View(oobject);
        }

        // POST: Oobjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,guest")]
        public async Task<IActionResult> Edit(int id, [Bind("OobjectID,Title,Adress,Type,Status,Photo,ForemenId,CustomerId")] Oobject oobject, IFormFile upload)
        {
            if (id != oobject.OobjectID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (upload != null)
                {
                    string path = "/Files/" + upload.FileName;
                    using (var fileStream = new
                   FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await upload.CopyToAsync(fileStream);
                    }

                    if (!oobject.Photo.IsNullOrEmpty())
                    {
                        System.IO.File.Delete(_appEnvironment.WebRootPath + oobject.Photo);
                    }
                    oobject.Photo = path;
                }

                try
                {
                    _context.Update(oobject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OobjectExists(oobject.OobjectID))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerID", "CustomerID", oobject.CustomerId);
            ViewData["ForemenId"] = new SelectList(_context.Foremens, "ForemenID", "ForemenID", oobject.ForemenId);
            return View(oobject);
        }

        // GET: Oobjects/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Oobjects == null)
            {
                return NotFound();
            }

            var oobject = await _context.Oobjects
                .Include(o => o.Customer)
                .Include(o => o.Foremen)
                .FirstOrDefaultAsync(m => m.OobjectID == id);
            if (oobject == null)
            {
                return NotFound();
            }

            return View(oobject);
        }

        // POST: Oobjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Oobjects == null)
            {
                return Problem("Entity set 'OobjectDBContext.Oobjects'  is null.");
            }
            var oobject = await _context.Oobjects.FindAsync(id);
            if (oobject != null)
            {
                _context.Oobjects.Remove(oobject);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OobjectExists(int id)
        {
            return (_context.Oobjects?.Any(e => e.OobjectID == id)).GetValueOrDefault();
        }
        public FileResult GetReport()
        {
            // Путь к файлу с шаблоном
            string path = "/Reports/report_template.xlsx";
            //Путь к файлу с результатом
            string result = "/Reports/report.xlsx";
            FileInfo fi = new FileInfo(_appEnvironment.WebRootPath + path);
            FileInfo fr = new FileInfo(_appEnvironment.WebRootPath + result);
            //будем использовть библитотеку не для коммерческого использования
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //открываем файл с шаблоном
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = "Лиинючев Н.А.";
                excelPackage.Workbook.Properties.Title = "Список объектов";
                excelPackage.Workbook.Properties.Subject = "Объекты";
                excelPackage.Workbook.Properties.Created = DateTime.Now;
                //плучаем лист по имени.
                ExcelWorksheet worksheet =
               excelPackage.Workbook.Worksheets["Oobject"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 3;
                List<Oobject> Oobjects = _context.Oobjects.ToList();

                foreach (Oobject oobject in Oobjects)
                {
                    Foremen foremen = _context.Foremens.Find(oobject.ForemenId);
                    worksheet.Cells[startLine, 1].Value = startLine - 2;
                    worksheet.Cells[startLine, 2].Value = oobject.OobjectID;
                    worksheet.Cells[startLine, 3].Value = oobject.Title;
                    worksheet.Cells[startLine, 4].Value = oobject.Adress;
                    worksheet.Cells[startLine, 5].Value = oobject.Type;
                    worksheet.Cells[startLine, 6].Value = oobject.Status;
                    worksheet.Cells[startLine, 7].Value = foremen.Name + ' ' + foremen.LastName;
                    startLine++;
                }
                //созраняем в новое место
                excelPackage.SaveAs(fr);
            }
            // Тип файла - content-type
            string file_type =
           "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet";
            // Имя файла - необязательно
            string file_name = "report.xlsx";
            return File(result, file_type, file_name);
        }
    }
}
