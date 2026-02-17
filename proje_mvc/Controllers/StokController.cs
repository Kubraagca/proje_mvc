using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proje_mvc.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace proje_mvc.Controllers
{
    public class StokController : Controller
    {
        private readonly ProjeDbContext _context;

        public StokController(ProjeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var stoklar = await _context.mvc_stok.Where(s => !s.is_deleted).ToListAsync();
            return View(stoklar);
        }

        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ekle(UrunModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Duzenle(Guid id)
        {
            var urun = await _context.mvc_stok.FindAsync(id);
            if (urun == null) return NotFound();
            return View(urun);
        }

        [HttpPost]
        public async Task<IActionResult> Duzenle(UrunModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Sil(Guid id)
        {
            var urun = await _context.mvc_stok.FindAsync(id);
            if (urun != null)
            {
                urun.is_deleted = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
