using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using proje_mvc.Models;

namespace proje_mvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly ProjeDbContext _context;

        public StatisticsController(ProjeDbContext context)
        {
            _context = context;
        }

        [HttpGet("counts")]
        public IActionResult GetCounts()
        {
            var totalKurum = _context.mvc_kurum_kayit.Count();
            var totalPersonel = _context.mvc_personel_kayit.Count(p => p.is_deleted == false);
            var totalKart = _context.mvc_kart_kayit.Count();
            
            // ERP Modülleri
            var totalUrun = _context.mvc_stok.Count(s => s.is_deleted == false);
            var totalMusteri = _context.mvc_musteriler.Count(m => m.is_deleted == false);
            
            var totalGelir = _context.mvc_finans
                .Where(f => f.tip == "Gelir" && f.is_deleted == false)
                .Sum(f => (decimal?)f.tutar) ?? 0;
                
            var totalGider = _context.mvc_finans
                .Where(f => f.tip == "Gider" && f.is_deleted == false)
                .Sum(f => (decimal?)f.tutar) ?? 0;

            var result = new
            {
                totalKurum,
                totalPersonel,
                totalKart,
                totalUrun,
                totalMusteri,
                totalGelir,
                totalGider,
                netDurum = totalGelir - totalGider
            };

            return Ok(result);
        }
    }
}
