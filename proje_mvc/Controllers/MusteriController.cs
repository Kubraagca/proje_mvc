using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proje_mvc.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace proje_mvc.Controllers
{
    public class MusteriController : Controller
    {
        private readonly ProjeDbContext _context;

        public MusteriController(ProjeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var musteriler = await _context.mvc_musteriler.Where(m => !m.is_deleted).ToListAsync();
            return View(musteriler);
        }

        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ekle(MusteriModel model)
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
            var musteri = await _context.mvc_musteriler.FindAsync(id);
            if (musteri == null) return NotFound();
            return View(musteri);
        }

        [HttpPost]
        public async Task<IActionResult> Duzenle(MusteriModel model)
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
            var musteri = await _context.mvc_musteriler.FindAsync(id);
            if (musteri != null)
            {
                musteri.is_deleted = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
