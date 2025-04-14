using Microsoft.AspNetCore.Mvc;
using proje_mvc.Models;

namespace proje_mvc.Controllers
{
    public class KurumController : Controller
    {
        private readonly ProjeDbContext _context;

        public KurumController(ProjeDbContext context)
        {
            _context = context;
        }


    }

    }

