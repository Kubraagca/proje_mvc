using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proje_mvc.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace proje_mvc.Controllers
{
    public class FinansController : Controller
    {
        private readonly ProjeDbContext _context;

        public FinansController(ProjeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var islemler = await _context.mvc_finans.Where(f => !f.is_deleted).OrderByDescending(f => f.tarih).ToListAsync();
            
            ViewBag.ToplamGelir = islemler.Where(i => i.tip == "Gelir").Sum(i => i.tutar);
            ViewBag.ToplamGider = islemler.Where(i => i.tip == "Gider").Sum(i => i.tutar);
            ViewBag.NetDurum = ViewBag.ToplamGelir - ViewBag.ToplamGider;
            
            return View(islemler);
        }

        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ekle(FinansModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Sil(Guid id)
        {
            var islem = await _context.mvc_finans.FindAsync(id);
            if (islem != null)
            {
                islem.is_deleted = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
