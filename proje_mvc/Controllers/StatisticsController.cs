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
            var totalKurum = _context.mvc_kurum_kayit.Count(); // Kurumlar tablonuzu ve modelinizi değiştirebilirsiniz.
            var totalPersonel = _context.mvc_personel_kayit.Count(); // Personel tablosu.
            var totalKart = _context.mvc_kart_kayit.Count(); // Kartlar tablosu.

            var result = new
            {
                totalKurum,
                totalPersonel,
                totalKart
            };

            return Ok(result);
        }
    }
}
