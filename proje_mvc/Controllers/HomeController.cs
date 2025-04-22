
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proje_mvc.Models;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace proje_mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProjeDbContext _context;





        public HomeController(ILogger<HomeController> logger, ProjeDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public IActionResult privacy()
        {
            return View();
        }

        public IActionResult Index()
        {
            // Burada gerekirse ViewBag ile veri ge�ilebilir
            // ViewBag.TotalKurum = 10;
            // ViewBag.TotalPersonel = 50;
            // ViewBag.TotalKart = 30;

            return View("Anasayfa"); // cshtml dosya ad� bu
        }

        public IActionResult Giris()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Giris(string kullaniciAdi, string sifre)
        {
            // Basit �rnek giri� kontrol�
            if (kullaniciAdi == "admin" && sifre == "123")
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Hata = "Kullan�c� ad� veya �ifre hatal�.";
            return View();
        }
        public IActionResult kurum_ekle()
        {
            return View(); // View i�in bir model g�ndermiyorsan�z, sadece bo� bir view d�nd�r�yoruz
        }

        [HttpPost] // POST metodunu belirtmek i�in [HttpPost] ekliyoruz
        public IActionResult kurum_ekle(KurumModel kurum)
        {
            if (ModelState.IsValid)
            {
                kurum.kurum_id = Guid.NewGuid(); // Yeni bir GUID olu�tur
                _context.mvc_kurum_kayit.Add(kurum);
                _context.SaveChanges();
                return RedirectToAction("kurum_listele"); // Kay�t sonras� listeleme sayfas�na y�nlendir
            }

            // ModelState hatalar�n� konsola yazd�r
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(kurum); // E�er model ge�erli de�ilse, formu tekrar g�ster
        }



        [HttpPost]
        public IActionResult kurum_sil(Guid kurum_id)
        {
            var kurum = _context.mvc_kurum_kayit.FirstOrDefault(k => k.kurum_id == kurum_id);

            if (kurum != null)
            {
                _context.mvc_kurum_kayit.Remove(kurum);
                _context.SaveChanges();
            }

            return RedirectToAction("kurum_listele");
        }



        [HttpGet]
        public IActionResult kurum_guncelle(Guid kurum_id)
        {
            var kurum = _context.mvc_kurum_kayit.FirstOrDefault(k => k.kurum_id == kurum_id);
            if (kurum == null)
            {
                return NotFound();
            }

            return View(kurum); // G�ncelleme formuna g�nder
        }




        [HttpPost]
        public IActionResult kurum_guncelle(KurumModel model)
        {
            if (ModelState.IsValid)
            {
                var mevcut = _context.mvc_kurum_kayit.FirstOrDefault(k => k.kurum_id == model.kurum_id);
                if (mevcut == null)
                    return NotFound();

                // G�ncelleme i�lemi
                mevcut.kurum_adi = model.kurum_adi;
                mevcut.iletisim_no = model.iletisim_no;

                _context.SaveChanges();

              
                return RedirectToAction("kurum_listele");
            }

            return View(model); 
        }





        public IActionResult kurum_listele(string searchId, string searchName, string searchPhone)
        {
            var kurumlar = _context.mvc_kurum_kayit.AsQueryable();

            // E�er searchId sa�lanm��sa ve ge�erli bir GUID ise, filtreleme yap
            if (!string.IsNullOrEmpty(searchId) && Guid.TryParse(searchId, out Guid kurumId))
            {
                kurumlar = kurumlar.Where(k => k.kurum_id == kurumId);
            }

            // E�er searchName sa�lanm��sa, kurum ad� i�eri�i filtrele
            if (!string.IsNullOrEmpty(searchName))
            {
                kurumlar = kurumlar.Where(k => k.kurum_adi.Contains(searchName));
            }

            // E�er searchPhone sa�lanm��sa, ileti�im numaras� i�eri�i filtrele
            if (!string.IsNullOrEmpty(searchPhone))
            {
                kurumlar = kurumlar.Where(k => k.iletisim_no.Contains(searchPhone));
            }

            // Filtrelenmi� kurumlar� listele
            return View(kurumlar.ToList());
        }



        public IActionResult personel_ekle()
        {
            var model = new PersonelModel
            {
                personel_id = Guid.NewGuid()
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult personel_ekle(PersonelModel model)
        {
            if (ModelState.IsValid)
            {
                model.personel_id = Guid.NewGuid();
                _context.mvc_personel_kayit.Add(model);
                _context.SaveChanges();
                return RedirectToAction("personel_listele");
            }
            return View(model);
        }


        // GET: G�ncelleme sayfas�n� a�ar
        public IActionResult personel_guncelle(Guid personel_id)
        {
            var personel = _context.mvc_personel_kayit.FirstOrDefault(p => p.personel_id == personel_id);
            if (personel == null)
            {
                return NotFound();
            }

            return View(personel);
        }

        // POST: G�ncelleme i�lemini yapar
        [HttpPost]
        public IActionResult personel_guncelle(PersonelModel model)
        {
            if (ModelState.IsValid)
            {
                var mevcut = _context.mvc_personel_kayit.FirstOrDefault(p => p.personel_id == model.personel_id);
                if (mevcut == null)
                    return NotFound();

                mevcut.ad = model.ad;
                mevcut.soyad = model.soyad;
                mevcut.adres = model.adres;
                mevcut.telefon = model.telefon;
                mevcut.TC = model.TC;
                mevcut.dogum_tarihi = model.dogum_tarihi;
                mevcut.ise_baslama_tarihi = model.ise_baslama_tarihi;
                mevcut.kurum_id = model.kurum_id;
                mevcut.kart_id = model.kart_id;

                _context.SaveChanges();
                return RedirectToAction("personel_listele");
            }

            return View(model);
        }




        [HttpPost]
        public IActionResult personel_sil(Guid personel_id)
        {
            var personel = _context.mvc_personel_kayit.FirstOrDefault(p => p.personel_id == personel_id);

            if (personel != null)
            {
                _context.mvc_personel_kayit.Remove(personel);
                _context.SaveChanges();
                return RedirectToAction("personel_listele");
            }

            return NotFound();
        }





        public IActionResult personel_listele()
        {
            var personeller = _context.mvc_personel_kayit.ToList();
            return View(personeller); // Bu �ekilde personel listesini view'a g�nderin
        }






        public IActionResult kart_ekle(KartModel kart)
        {
            if (ModelState.IsValid)
            {
                kart.kart_id = Guid.NewGuid(); // Yeni bir GUID olu�tur
                _context.mvc_kart_kayit.Add(kart);
                _context.SaveChanges();
                return RedirectToAction("kart_listesi");
            }

            // ModelState hatalar�n� konsola yazd�r
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(kart); // E�er model ge�erli de�ilse, formu tekrar g�ster
        }


        [HttpGet]
        public IActionResult kart_guncelle(Guid kart_id)
        {
            var kart = _context.mvc_kart_kayit.FirstOrDefault(k => k.kart_id == kart_id);
            if (kart == null)
                return NotFound();
            return View(kart);
        }

        [HttpPost]
        public IActionResult kart_guncelle(KartModel model)
        {
            if (ModelState.IsValid)
            {
                // Mevcut kayd� bul
                var mevcutKart = _context.mvc_kart_kayit.FirstOrDefault(k => k.kart_id == model.kart_id);

                if (mevcutKart == null)
                    return NotFound();

                // Alanlar� g�ncelle
                mevcutKart.kurum_id = model.kurum_id;
                mevcutKart.personel_id = model.personel_id;
                mevcutKart.kayit_tarihi = model.kayit_tarihi;

                // G�ncelle ve kaydet
                _context.SaveChanges();

                return RedirectToAction("kart_listesi");
            }

            return View(model);
        }


        [HttpPost]
        public IActionResult kart_sil(Guid kart_id)
        {
            var kart = _context.mvc_kart_kayit.FirstOrDefault(k => k.kart_id == kart_id);
            if (kart == null)
                return NotFound();

            _context.mvc_kart_kayit.Remove(kart);
            _context.SaveChanges();
            return RedirectToAction("kart_listesi");
        }






        public IActionResult kart_listesi(string searchId, string searchDate)
        {
            var kartlar = _context.mvc_kart_kayit.AsQueryable();

            // E�er searchId sa�lanm��sa ve ge�erli bir GUID ise, filtreleme kartlar
            if (!string.IsNullOrEmpty(searchId) && Guid.TryParse(searchId, out Guid kartId))
            {
                kartlar = kartlar.Where(k => k.kart_id == kartId);
            }

            if (!string.IsNullOrEmpty(searchDate) && DateTime.TryParse(searchDate, out DateTime searchDateParsed))
            {
                kartlar = kartlar.Where(k => k.kayit_tarihi.Date == searchDateParsed.Date);
            }

            // Kart listesini view'a g�nderme
            var kartListesi = kartlar.ToList();
            return View(kartListesi);
        }




        public IActionResult izin_ekle()
        {
            var model = new IzinModel
            {
                izin_id = Guid.NewGuid()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult izin_ekle(IzinModel izin)
        {
            if (ModelState.IsValid)
            {
                izin.izin_id = Guid.NewGuid(); // Yeni bir GUID olu�tur
                _context.mvc_izin_kayit.Add(izin);
                _context.SaveChanges();
                return RedirectToAction("izin_listele");
            }

            // ModelState hatalar�n� konsola yazd�r
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(izin); // E�er model ge�erli de�ilse, formu tekrar g�ster
        }


        [HttpPost]
        public IActionResult izin_id(IzinModel model)
        {
            if (ModelState.IsValid)
            {
                model.izin_id = Guid.NewGuid();
                _context.mvc_izin_kayit.Add(model);
                _context.SaveChanges();
                return RedirectToAction("izin_listele");
            }
            return View(model);
        }




        public IActionResult izin_listele()
        {
            var izinler = _context.mvc_izin_kayit.ToList();
            return View(izinler);
        }

        [HttpPost]
        [HttpPost]
        public IActionResult izin_sil(Guid izin_id)
        {
            var izin = _context.mvc_izin_kayit.FirstOrDefault(i => i.izin_id == izin_id);
            if (izin == null)
            {
                return NotFound();
            }

            _context.mvc_izin_kayit.Remove(izin);
            _context.SaveChanges();

            return RedirectToAction("izin_listele");
        }


    
        [HttpGet]
        public IActionResult izin_guncelle(Guid id)
        {
            var izin = _context.mvc_izin_kayit.FirstOrDefault(i => i.izin_id == id);
            if (izin == null)
            {
                return NotFound();
            }

            return View(izin);
        }


        [HttpPost]
        public IActionResult izin_guncelle(IzinModel model)
        {
            var izin = _context.mvc_izin_kayit.FirstOrDefault(i => i.izin_id == model.izin_id);
            if (izin != null)
            {
                izin.personel_id = model.personel_id;
                izin.ad = model.ad;
                izin.soyad = model.soyad;
                izin.izin_turu = model.izin_turu;
                izin.izin_aciklama = model.izin_aciklama;
                izin.izin_baslangic_tarihi = model.izin_baslangic_tarihi;
                izin.izin_bitis_tarihi = model.izin_bitis_tarihi;
                izin.kurum_id = model.kurum_id;

                _context.SaveChanges();
            }

            return RedirectToAction("izin_listele"); // �zinlerin listelendi�i sayfaya y�nlendir
        }



        // GET: g�rev ekleme formunu g�ster
        public IActionResult gorev_ekle()
        {
            var model = new GorevModel
            {
                gorev_id = Guid.NewGuid(),
          
                gorev_baslangic_tarihi = DateTime.Now
            };
            return View(model);
        }

        // POST: g�rev ekleme i�lemi
        [HttpPost]
        public IActionResult gorev_ekle(GorevModel gorev)
        {
            if (ModelState.IsValid)
            {
                gorev.gorev_id = Guid.NewGuid();
                gorev.gorev_baslangic_tarihi = DateTime.Now;

                _context.mvc_gorev_kayit.Add(gorev);
                _context.SaveChanges();
                return RedirectToAction("gorev_listele");
            }

            // Hatalar varsa konsola yaz
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(gorev);
        }


        public IActionResult gorev_listele()
        {
            var gorevler = _context.mvc_gorev_kayit.ToList();
            return View(gorevler);
        }



        [HttpPost]
        [HttpPost]
        public IActionResult GorevSil(Guid gorev_id)
        {
            var gorev = _context.mvc_gorev_kayit.FirstOrDefault(g => g.gorev_id == gorev_id);
            if (gorev != null)
            {
                _context.mvc_gorev_kayit.Remove(gorev);
                _context.SaveChanges();
            }
            return RedirectToAction("gorev_listele");
        }

        [HttpGet]
        public IActionResult gorev_guncelle(Guid id)
        {
            var gorev = _context.mvc_gorev_kayit.FirstOrDefault(g => g.gorev_id == id);
            if (gorev == null)
            {
                return NotFound();
            }

            return View(gorev);
        }
        [HttpPost]
        public IActionResult gorev_guncelle(GorevModel model)
        {
            var gorev = _context.mvc_gorev_kayit.FirstOrDefault(g => g.gorev_id == model.gorev_id);
            if (gorev != null)
            {
                gorev.personel_id = model.personel_id;
                gorev.ad = model.ad;
                gorev.soyad = model.soyad;
                gorev.gorev_adi = model.gorev_adi;
                gorev.gorev_aciklama = model.gorev_aciklama;
                gorev.gorev_baslangic_tarihi = model.gorev_baslangic_tarihi;

                _context.SaveChanges();
            }

            return RedirectToAction("gorev_listele");
        }



        /*
                [HttpPost]
                public IActionResult guncelle_gorev(GorevModel gorev)
                {
                    if (!ModelState.IsValid)
                    {
                        return View(gorev); // Model ge�erli de�ilse formu tekrar g�ster
                    }

                    var mevcutGorev = _context.mvc_gorev_kayit.FirstOrDefault(g => g.gorev_id == gorev.gorev_id);

                    if (mevcutGorev == null)
                    {
                        return NotFound(); // E�er g�rev kayd� yoksa hata d�nd�r
                    }

                    // Mevcut g�rev kayd�n� g�ncelle
                    mevcutGorev.gorev_adi = gorev.gorev_adi;
                    mevcutGorev.gorev_aciklama = gorev.gorev_aciklama;
                    mevcutGorev.gorev_baslangic_tarihi = gorev.gorev_baslangic_tarihi;
                    mevcutGorev.personel_id = gorev.personel_id;

                    _context.SaveChanges(); // De�i�iklikleri kaydet

                    return RedirectToAction("gorev_listele"); // G�ncellendikten sonra liste sayfas�na y�nlendir
                }

                  public class AuthController : Controller
                {
                    private readonly ILogger<AuthController> _logger;
                    private readonly ProjeDbContext _context;





                    public AuthController(ILogger<AuthController> logger, ProjeDbContext context)
                    {
                        _logger = logger;
                        _context = context;
                    }

                    [HttpGet]
                    public IActionResult Giris()
                    {
                        return View();
                    }

                    [HttpPost]
                    [ValidateAntiForgeryToken]
                    public IActionResult Giris(KullaniciModel model)
                    {
                        if (ModelState.IsValid)
                        {
                            var user = _context.mvc_kullanici_giris
                                .FirstOrDefault(u => u.kullanici_adi == model.kullanici_adi && u.sifre == model.sifre);
                            if (user != null)
                            {
                                HttpContext.Session.SetString("kullanici_adi", model.kullanici_adi);
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ViewData["ErrorMessage"] = "Ge�ersiz kullan�c� ad� veya �ifre.";
                            }
                        }
                        return View(model);
                    }
                }*/



    }
}




