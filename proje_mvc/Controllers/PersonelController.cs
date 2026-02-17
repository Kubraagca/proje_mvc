using Microsoft.AspNetCore.Mvc;
using proje_mvc.Models; // Model sınıfınızı eklediğiniz namespace'i kullanın
using System.Linq;

namespace proje_mvc.Controllers
{
    public class PersonelController : Controller
    {
        private readonly ProjeDbContext _context;

        public PersonelController(ProjeDbContext context)
        {
            _context = context;
        }

        
    }
}
