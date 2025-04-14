
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proje_mvc.Models;
using System.Diagnostics;

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

        public IActionResult index()
        {
            return View();
        }




        public IActionResult kurum_ekle(KurumModel kurum)
        {
            if (ModelState.IsValid)
            {
                kurum.kurum_id = Guid.NewGuid(); // Yeni bir GUID oluþtur
                _context.mvc_kurum_kayit.Add(kurum);
                _context.SaveChanges();
                return RedirectToAction("kurum_listele");
            }
            return View(kurum);
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



        public IActionResult kurum_guncelle(Guid kurum_id)
        {
            var kurum = _context.mvc_kurum_kayit.FirstOrDefault(k => k.kurum_id == kurum_id);

            if (kurum == null)
            {
                return NotFound(); // Eðer kurum bulunamazsa
            }

            return View(kurum); // Kurumu View'a gönder
        }



        [HttpPost]
        public IActionResult guncelle_kurum(KurumModel kurum)
        {
            if (!ModelState.IsValid)
            {
                return View(kurum); // Model geçerli deðilse, güncellenen verileri geri gönder
            }

            var mevcutKurum = _context.mvc_kurum_kayit.FirstOrDefault(k => k.kurum_id == kurum.kurum_id);

            if (mevcutKurum == null)
            {
                return NotFound(); // Eðer kurum bulunamazsa hata mesajý döndürüyoruz.
            }

            mevcutKurum.kurum_adi = kurum.kurum_adi;
            mevcutKurum.iletisim_no = kurum.iletisim_no;

            _context.SaveChanges();

            // Güncellenen veriyi listeleme sayfasýna yönlendir
            return RedirectToAction("kurum_listele");
        }




        public IActionResult kurum_listele(string searchId, string searchName, string searchPhone)
        {
            var kurumlar = _context.mvc_kurum_kayit.AsQueryable();

            // Eðer searchId saðlanmýþsa ve geçerli bir GUID ise, filtreleme yap
            if (!string.IsNullOrEmpty(searchId) && Guid.TryParse(searchId, out Guid kurumId))
            {
                kurumlar = kurumlar.Where(k => k.kurum_id == kurumId);
            }

            // Eðer searchName saðlanmýþsa, kurum adý içeriði filtrele
            if (!string.IsNullOrEmpty(searchName))
            {
                kurumlar = kurumlar.Where(k => k.kurum_adi.Contains(searchName));
            }

            // Eðer searchPhone saðlanmýþsa, iletiþim numarasý içeriði filtrele
            if (!string.IsNullOrEmpty(searchPhone))
            {
                kurumlar = kurumlar.Where(k => k.iletisim_no.Contains(searchPhone));
            }

            // Filtrelenmiþ kurumlarý listele
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


        public IActionResult personel_guncelle(Guid personel_id)
        {
            var personel = _context.mvc_personel_kayit.FirstOrDefault(p => p.personel_id == personel_id);

            if (personel == null)
            {
                return NotFound(); // Eðer kurum bulunamazsa
            }

            return View(personel); // Kurumu View'a gönder
        }

        [HttpPost]
        public IActionResult personel_guncelle(PersonelModel personel)
        {
            if (!ModelState.IsValid)
            {
                return View(personel); // Model geçerli deðilse, güncellenen verileri geri gönder
            }
            var mevcutPersonel = _context.mvc_personel_kayit.FirstOrDefault(p => p.personel_id == personel.personel_id);

            if (mevcutPersonel == null)
            {
                return NotFound(); // Eðer kurum bulunamazsa hata mesajý döndürüyoruz.
            }

            // Personel bilgilerini modelde dolduruyoruz

            mevcutPersonel.personel_id = mevcutPersonel.personel_id;
            mevcutPersonel.ad = mevcutPersonel.ad;
            mevcutPersonel.soyad = mevcutPersonel.soyad;
            mevcutPersonel.adres = mevcutPersonel.adres;
            mevcutPersonel.dogum_tarihi = mevcutPersonel.dogum_tarihi;
            mevcutPersonel.telefon = mevcutPersonel.telefon;
            mevcutPersonel.TC = mevcutPersonel.TC;
            mevcutPersonel.ise_baslama_tarihi = mevcutPersonel.ise_baslama_tarihi;
            mevcutPersonel.kurum_id = mevcutPersonel.kurum_id;



            _context.SaveChanges();

            // Güncellenen veriyi listeleme sayfasýna yönlendir
            return RedirectToAction("kurum_listele");
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
            return View(personeller); // Bu þekilde personel listesini view'a gönderin
        }






        public IActionResult kart_ekle(KartModel kart)
        {
            if (ModelState.IsValid)
            {
                kart.kart_id = Guid.NewGuid(); // Yeni bir GUID oluþtur
                _context.mvc_kart_kayit.Add(kart);
                _context.SaveChanges();
                return RedirectToAction("kart_listesi");
            }

            // ModelState hatalarýný konsola yazdýr
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(kart); // Eðer model geçerli deðilse, formu tekrar göster
        }


        [HttpPost]
        public IActionResult kart_sil(Guid kart_id)
        {
            var kart = _context.mvc_kart_kayit.FirstOrDefault(k => k.kart_id == kart_id);

            if (kart != null)
            {
                _context.mvc_kart_kayit.Remove(kart);
                _context.SaveChanges();
            }

            return RedirectToAction("kart_listesi");
        }



        public IActionResult kart_guncelle(Guid kart_id)
        {
            var kart = _context.mvc_kart_kayit.FirstOrDefault(k => k.kart_id == kart_id);

            if (kart == null)
            {
                return NotFound(); // Eðer kurum bulunamazsa
            }

            return View(kart); // Kurumu View'a gönder
        }



        [HttpPost]
        public IActionResult guncelle_kart(KartModel kart)
        {
            if (!ModelState.IsValid)
            {
                return View(kart); // Model geçerli deðilse, güncellenen verileri geri gönder
            }

            var mevcutKart = _context.mvc_kart_kayit.FirstOrDefault(k => k.kart_id == kart.kart_id);

            if (mevcutKart == null)
            {
                return NotFound(); // Eðer kurum bulunamazsa hata mesajý döndürüyoruz.
            }

            mevcutKart.kart_id = kart.kart_id;
            mevcutKart.kayit_tarihi = kart.kayit_tarihi;
            mevcutKart.kurum_id = kart.kurum_id;


            _context.SaveChanges();

            // Güncellenen veriyi listeleme sayfasýna yönlendir
            return RedirectToAction("kart_listesi");
        }





        public IActionResult kart_listesi(string searchId, string searchDate)
        {
            var kartlar = _context.mvc_kart_kayit.AsQueryable();

            // Eðer searchId saðlanmýþsa ve geçerli bir GUID ise, filtreleme kartlar
            if (!string.IsNullOrEmpty(searchId) && Guid.TryParse(searchId, out Guid kartId))
            {
                kartlar = kartlar.Where(k => k.kart_id == kartId);
            }

            if (!string.IsNullOrEmpty(searchDate) && DateTime.TryParse(searchDate, out DateTime searchDateParsed))
            {
                kartlar = kartlar.Where(k => k.kayit_tarihi.Date == searchDateParsed.Date);
            }

            // Kart listesini view'a gönderme
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
                izin.izin_id = Guid.NewGuid(); // Yeni bir GUID oluþtur
                _context.mvc_izin_kayit.Add(izin);
                _context.SaveChanges();
                return RedirectToAction("izin_listele");
            }

            // ModelState hatalarýný konsola yazdýr
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(izin); // Eðer model geçerli deðilse, formu tekrar göster
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
            return View(izinler); // Bu þekilde personel listesini view'a gönderin
        }

        [HttpPost]
        public IActionResult izin_sil(Guid izin_id)
        {
            var izin = _context.mvc_izin_kayit.FirstOrDefault(i => i.izin_id == izin_id);

            if (izin != null)
            {
                _context.mvc_izin_kayit.Remove(izin);
                _context.SaveChanges();
            }

            return RedirectToAction("izin_listele");
        }

        public IActionResult izin_guncelle(Guid izin_id)
        {
            var izin = _context.mvc_izin_kayit.FirstOrDefault(i => i.izin_id == izin_id);

            if (izin == null)
            {
                return NotFound(); // Eðer izin kaydý bulunamazsa hata döndür
            }

            return View(izin); // Bulunan izin bilgilerini View'a gönder
        }

        [HttpPost]
        public IActionResult guncelle_izin(IzinModel izin)
        {
            if (!ModelState.IsValid)
            {
                return View(izin); // Model geçerli deðilse formu tekrar göster
            }

            var mevcutIzin = _context.mvc_izin_kayit.FirstOrDefault(i => i.izin_id == izin.izin_id);

            if (mevcutIzin == null)
            {
                return NotFound(); // Eðer izin kaydý yoksa hata döndür
            }

            // Mevcut izin kaydýný güncelle
            mevcutIzin.izin_baslangic_tarihi = izin.izin_baslangic_tarihi;
            mevcutIzin.izin_bitis_tarihi = izin.izin_bitis_tarihi;
            mevcutIzin.izin_turu = izin.izin_turu;
            mevcutIzin.izin_aciklama = izin.izin_aciklama;
            mevcutIzin.kurum_id = izin.kurum_id;
            mevcutIzin.personel_id = izin.personel_id;

            _context.SaveChanges(); // Deðiþiklikleri kaydet

            return RedirectToAction("izin_listesi"); // Güncellendikten sonra liste sayfasýna yönlendir
        }


        [HttpGet]
        public IActionResult gorev_ekle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult gorev_ekle(GorevModel gorev)
        {
            if (ModelState.IsValid)
            {
                gorev.gorev_id = Guid.NewGuid();
                _context.mvc_gorev_kayit.Add(gorev);
                _context.SaveChanges();
                return RedirectToAction("gorev_listele");
            }

            return View(gorev);
        }

        public IActionResult gorev_listele()
        {
            var gorevler = _context.mvc_gorev_kayit.ToList();
            return View(gorevler);
        }


        [HttpPost]
        public IActionResult gorev_sil(Guid gorev_id)
        {
            var gorev = _context.mvc_gorev_kayit.FirstOrDefault(g => g.gorev_id == gorev_id);

            if (gorev != null)
            {
                _context.mvc_gorev_kayit.Remove(gorev);
                _context.SaveChanges();
            }

            return RedirectToAction("gorev_listele");
        }


        public IActionResult gorev_guncelle(Guid gorev_id)
        {
            var gorev = _context.mvc_gorev_kayit.FirstOrDefault(g => g.gorev_id == gorev_id);

            if (gorev == null)
            {
                return NotFound();
            }

            return View(gorev);
        }
/*
        [HttpPost]
        public IActionResult guncelle_gorev(GorevModel gorev)
        {
            if (!ModelState.IsValid)
            {
                return View(gorev); // Model geçerli deðilse formu tekrar göster
            }

            var mevcutGorev = _context.mvc_gorev_kayit.FirstOrDefault(g => g.gorev_id == gorev.gorev_id);

            if (mevcutGorev == null)
            {
                return NotFound(); // Eðer görev kaydý yoksa hata döndür
            }

            // Mevcut görev kaydýný güncelle
            mevcutGorev.gorev_adi = gorev.gorev_adi;
            mevcutGorev.gorev_aciklama = gorev.gorev_aciklama;
            mevcutGorev.gorev_baslangic_tarihi = gorev.gorev_baslangic_tarihi;
            mevcutGorev.personel_id = gorev.personel_id;

            _context.SaveChanges(); // Deðiþiklikleri kaydet

            return RedirectToAction("gorev_listele"); // Güncellendikten sonra liste sayfasýna yönlendir
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
                        ViewData["ErrorMessage"] = "Geçersiz kullanýcý adý veya þifre.";
                    }
                }
                return View(model);
            }
        }*/
    }
}




