
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proje_mvc.Models;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;



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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Index()
        {
            var personelIdStr = HttpContext.Session.GetString("PersonelID");
            if (string.IsNullOrEmpty(personelIdStr)) return RedirectToAction("Giris");

            long personelId = Convert.ToInt64(personelIdStr);

            // 1. İzinleri Çek (Mavi)
            var izinler = _context.mvc_izin_kayit
                .Where(i => i.personel_id == personelId && !i.is_deleted)
                .ToList()
                .Select(i => new {
                    title = "🌴 " + i.izin_turu,
                    start = i.izin_baslangic_tarihi?.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = i.izin_bitis_tarihi?.ToString("yyyy-MM-ddTHH:mm:ss"),
                    className = "bg-primary border-primary text-white",
                    extendedProps = new { type = "Izin", description = i.izin_aciklama }
                });

            // 2. Görevleri Çek (Turuncu)
            var gorevler = _context.mvc_gorev_kayit
                .Where(g => g.personel_id == personelId && !g.is_deleted)
                .ToList()
                .Select(g => new {
                    title = "🎯 " + (g.gorev_adi ?? "Görev"),
                    start = g.gorev_baslangic_tarihi?.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = g.gorev_bitis_tarihi?.ToString("yyyy-MM-ddTHH:mm:ss") ?? g.gorev_baslangic_tarihi?.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ss"),
                    className = "bg-warning border-warning text-dark",
                    extendedProps = new { type = "Gorev", description = g.gorev_aciklama }
                });

            // 3. Doğum Günlerini Çek (Pembe)
            var currentYear = DateTime.Now.Year;
            var dogumGunleri = _context.mvc_personel_kayit
                .Where(p => !p.is_deleted && p.dogum_tarihi.HasValue)
                .ToList()
                .Select(p => new {
                    title = "🎂 " + p.ad + " " + p.soyad,
                    start = new DateTime(currentYear, p.dogum_tarihi.Value.Month, p.dogum_tarihi.Value.Day).ToString("yyyy-MM-dd"),
                    allDay = true,
                    className = "bg-info border-info text-white",
                    extendedProps = new { type = "DogumGunu" }
                });

            // 4. Tümünü Birleştir
            var allEvents = izinler.Cast<object>()
                .Concat(gorevler.Cast<object>())
                .Concat(dogumGunleri.Cast<object>())
                .ToList();

            ViewBag.AllEvents = allEvents;
            ViewBag.Duyurular = new List<string>
            {
                "⚠️ Sistem bakım çalışması 15 Mayıs'ta yapılacaktır.",
                "✅ Yeni personel modülü aktif edilmiştir.",
                "📢 19 Mayıs resmi tatil sebebiyle kurum kapalı olacaktır."
            };

            return View();
        }

        public IActionResult Giris()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Giris(string kullanici_adi, string sifre)
        {

            // Kullanıcı doğrulama işlemi
            var kullanici = _context.mvc_personel_kayit
           .FirstOrDefault(k => k.kullanici_adi.Trim().ToLower() == kullanici_adi.Trim().ToLower() &&
                            k.sifre.Trim() == sifre.Trim());


            if (kullanici != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, kullanici.kullanici_adi),
                    new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
                    new Claim(ClaimTypes.Role, kullanici.yetki ?? "Personel")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                ); 

                string kullaniciAdiSoyadi = $"{kullanici.ad} {kullanici.soyad}";
                HttpContext.Session.SetString("UserName", kullaniciAdiSoyadi);
                HttpContext.Session.SetString("PersonelID", kullanici.Id.ToString());
                HttpContext.Session.SetString("DogumTarihi", kullanici.dogum_tarihi?.ToString() ?? "");

                return RedirectToAction("Index", "Home");
            }

            // Giriş başarısızsa
            ViewBag.Hata = "Kullanıcı adı veya şifre hatalı.";
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult gorev_kaydet(GorevModel model)
        {
            if (model != null)
            {
                var personelIdStr = HttpContext.Session.GetString("PersonelID");
                var userName = HttpContext.Session.GetString("UserName");

                if (!string.IsNullOrEmpty(personelIdStr))
                {
                    model.personel_id = Convert.ToInt64(personelIdStr);
                    model.ad = userName;
                    model.soyad = userName;
                    model.gorev_id = Guid.NewGuid();
                    model.is_deleted = false;
                    
                    _context.mvc_gorev_kayit.Add(model);
                    _context.SaveChanges();
                    
                    TempData["SuccessMessage"] = "Görev başarıyla planlandı.";
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Cikis()
        {
            // Çıkış işlemi yapılıyor
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Session bilgilerini temizle
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("PersonelID");

            // Cookie'yi manuel olarak temizle
            Response.Cookies.Delete(".AspNetCore.Cookies");

            return RedirectToAction("Index", "Home"); // Anasayfaya yönlendir
        }


        public JsonResult GetDashboardStats()
        {
            var birAyOnce = DateTime.Now.AddMonths(-1);
            var kurumCount = _context.mvc_kurum_kayit.Count(k => !k.is_deleted);
            var personelCount = _context.mvc_personel_kayit.Count();
            var kartCount = _context.mvc_kart_kayit.Count();

            return Json(new
            {
                kurum = kurumCount,
                personel = personelCount,
                kart = kartCount
            });
        }

        [HttpGet("counts")]
        public IActionResult GetCounts()
        {
            var totalKurum = _context.mvc_kurum_kayit.Count(x => !x.is_deleted);
            var totalPersonel = _context.mvc_personel_kayit.Count(x => !x.is_deleted);
            var totalKart = _context.mvc_kart_kayit.Count(x => !x.is_deleted);

            return Ok(new
            {
                totalKurum,
                totalPersonel,
                totalKart
            });
        }
        [Authorize]
        public IActionResult kurum_ekle()
        {
            return View(); // View için bir model göndermiyorsanız, sadece boş bir view döndürüyoruz
        }

        [HttpPost] // POST metodunu belirtmek için [HttpPost] ekliyoruz
        public IActionResult kurum_ekle(KurumModel kurum)
        {
            if (ModelState.IsValid)
            {
                kurum.kurum_id = Guid.NewGuid(); // Yeni bir GUID oluştur
                _context.mvc_kurum_kayit.Add(kurum);
                _context.SaveChanges();
                return RedirectToAction("kurum_listele"); // Kayıt sonrası listeleme sayfasına yönlendir
            }

            // ModelState hatalarını konsola yazdır
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(kurum); // Eğer model geçerli değilse, formu tekrar göster
        }


        [Authorize]
        [HttpPost]
        public IActionResult kurum_sil(Guid kurum_id)
        {
            var kurum = _context.mvc_kurum_kayit.FirstOrDefault(k => k.kurum_id == kurum_id);

            if (kurum != null)
            {
                kurum.is_deleted = true;
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Kurum başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Kurum bulunamadı.";
            }

            return RedirectToAction("kurum_listele");
        }


        [Authorize]
        [HttpGet]
        public IActionResult kurum_guncelle(Guid kurum_id)
        {
            var kurum = _context.mvc_kurum_kayit.FirstOrDefault(k => k.kurum_id == kurum_id);
            if (kurum == null)
            {
                return NotFound();
            }

            return View(kurum); // Güncelleme formuna gönder
        }



        [Authorize]
        [HttpPost]
        public IActionResult kurum_guncelle(KurumModel model)
        {
            if (ModelState.IsValid)
            {
                var mevcut = _context.mvc_kurum_kayit.FirstOrDefault(k => k.kurum_id == model.kurum_id);
                if (mevcut == null)
                    return NotFound();

                // Güncelleme işlemi
                mevcut.kurum_adi = model.kurum_adi;
                mevcut.iletisim_no = model.iletisim_no;

                _context.SaveChanges();

                // Güncel veriyi gösteren sayfaya yönlendir
                return RedirectToAction("kurum_listele");
            }

            // Eğer model geçerli değilse, tekrar formu göster
            return View(model);
        }




        [Authorize]
        public IActionResult kurum_listele(string searchId, string searchName, string searchPhone)
        {
            var kurumlar = _context.mvc_kurum_kayit.AsQueryable();

            // Sadece 'IsDeleted' = false olan kurumları getir
            kurumlar = kurumlar.Where(k => k.is_deleted == false);

            // Eğer searchId sağlanmışsa ve geçerli bir GUID ise, filtreleme yap
            if (!string.IsNullOrEmpty(searchId) && Guid.TryParse(searchId, out Guid kurum_id))
            {
                kurumlar = kurumlar.Where(k => k.kurum_id == kurum_id);
            }

            // Eğer searchName sağlanmışsa, kurum adı içeriği filtrele
            if (!string.IsNullOrEmpty(searchName))
            {
                kurumlar = kurumlar.Where(k => k.kurum_adi.Contains(searchName));
            }

            // Eğer searchPhone sağlanmışsa, iletişim numarası içeriği filtrele
            if (!string.IsNullOrEmpty(searchPhone))
            {
                kurumlar = kurumlar.Where(k => k.iletisim_no.Contains(searchPhone));
            }

            // Filtrelenmiş kurumları listele
            return View(kurumlar.ToList());
        }



        public IActionResult personel_ekle()
        {
            var kurumlar = _context.mvc_kurum_kayit
               .Where(k => !k.is_deleted)
               .Select(k => new
               {
                   Id = k.Id,
                   kurum_adi = k.Id + " - " + k.kurum_adi
               })
               .ToList();
            ViewBag.Kurumlar = kurumlar;

            var model = new PersonelModel
            {
                personel_id = Guid.NewGuid()
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult personel_ekle(PersonelModel model)
        {
            HttpContext.Session.SetString("Id", model.Id.ToString());

            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString("KurumId", model.Id.ToString());

                _context.mvc_personel_kayit.Add(model);
                _context.SaveChanges();

                return RedirectToAction("personel_listele");
            }

            ViewBag.Kurumlar = _context.mvc_kurum_kayit
                   .Where(k => !k.is_deleted)
                   .Select(k => new
                   {
                       Id = k.Id,
                       kurum_adi = k.Id + " - " + k.kurum_adi
                   })
                   .ToList();

            return RedirectToAction("model");
        }




        [Authorize]
        public IActionResult personel_guncelle(Guid personel_id)
        {
            var personel = _context.mvc_personel_kayit.FirstOrDefault(p => p.personel_id == personel_id);
            if (personel == null)
            {
                return NotFound();
            }

            return View(personel);
        }

        [Authorize]
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



        [Authorize]
        [HttpPost]
        public IActionResult personel_sil(Guid personel_id)
        {
            var personel = _context.mvc_personel_kayit.FirstOrDefault(p => p.personel_id == personel_id);

            if (personel != null)
            {
                personel.is_deleted = true;
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Personel başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Personel bulunamadı.";
            }

            return RedirectToAction("personel_listele");
        }

        public IActionResult personel_listele()
        {
            var personeller = _context.mvc_personel_kayit.Where(p => p.is_deleted == false).ToList();

            return View(personeller);
        }




        [Authorize]
        [HttpGet]
        public IActionResult kart_ekle()
        {
            var kurumlar = _context.mvc_kurum_kayit
                .Where(k => !k.is_deleted)
                .Select(k => new
                {
                    Id = k.Id,
                    kurum_adi = k.Id + " - " + k.kurum_adi
                })
                .ToList();
            ViewBag.Kurumlar = kurumlar;

            var personeller = _context.mvc_personel_kayit
                .Where(p => !p.is_deleted)
                .Select(p => new
                {
                    Id = p.Id,
                    ad_soyad = p.Id + " - " + p.ad + " " + p.soyad
                })
                .ToList();
            ViewBag.Personeller = personeller;

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult kart_ekle(KartModel model)
        {
            HttpContext.Session.SetString("Id", model.Id.ToString());

            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString("KurumId", model.Id.ToString());

                _context.mvc_kart_kayit.Add(model);
                _context.SaveChanges();

                return RedirectToAction("kart_listesi");
            }

            // Model valid değilse tekrar listeleri yükle
            ViewBag.Kurumlar = _context.mvc_kurum_kayit
                .Where(k => !k.is_deleted)
                .Select(k => new
                {
                    Id = k.Id,
                    kurum_adi = k.Id + " - " + k.kurum_adi
                })
                .ToList();

            ViewBag.Personeller = _context.mvc_personel_kayit
                .Where(p => !p.is_deleted)
                .Select(p => new
                {
                    Id = p.Id,
                    ad_soyad = p.Id + " - " + p.ad + " " + p.soyad
                })
                .ToList();

            return View(model);
        }





        [Authorize]
        [HttpGet]
        public IActionResult kart_guncelle(Guid kart_id)
        {
            var kart = _context.mvc_kart_kayit.FirstOrDefault(k => k.kart_id == kart_id);
            if (kart == null)
                return NotFound();
            return View(kart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult kart_guncelle(KartModel model)
        {
            if (ModelState.IsValid)
            {
                // Mevcut kaydı bul
                var mevcutKart = _context.mvc_kart_kayit.FirstOrDefault(k => k.kart_id == model.kart_id);

                if (mevcutKart == null)
                    return NotFound();

                // Alanları güncelle
                mevcutKart.kurum_id = model.kurum_id;
                mevcutKart.personel_id = model.personel_id;
                mevcutKart.kayit_tarihi = model.kayit_tarihi;

                // Güncelle ve kaydet
                _context.SaveChanges();

                return RedirectToAction("kart_listesi");
            }

            return View(model);
        }


        [Authorize]
        [HttpPost]
        public IActionResult kart_sil(Guid kart_id)
        {
            var kart = _context.mvc_kart_kayit.FirstOrDefault(k => k.kart_id == kart_id);
            if (kart == null)
                return NotFound();

            kart.is_deleted = true;
            _context.SaveChanges();

            return RedirectToAction("kart_listesi");
        }




        [Authorize]
        public IActionResult kart_listesi(Guid? searchId, DateTime? searchDate)
        {
            var kartlar = _context.mvc_kart_kayit.AsQueryable();

            kartlar = kartlar.Where(k => k.is_deleted == false);

            // Eğer searchId sağlanmışsa, filtreleme kartlar
            if (searchId.HasValue)
            {
                kartlar = kartlar.Where(k => k.kart_id == searchId.Value);
            }

            // Eğer searchDate sağlanmışsa, filtreleme kartlar
            if (searchDate.HasValue)
            {
                kartlar = kartlar.Where(k => k.kayit_tarihi.Date == searchDate.Value.Date);
            }

            // Kart listesini view'a gönderme
            var kart_listesi = kartlar.ToList();
            return View(kart_listesi);
        }





        [Authorize]
        public IActionResult izin_ekle()
        {
            // Session'dan kullanıcı bilgilerini al
            var kullaniciAdiSoyadi = HttpContext.Session.GetString("UserName");
            var personelId = HttpContext.Session.GetString("PersonelID");

            // Eğer kullanıcı bilgisi mevcutsa, bu bilgileri form modeline ekleyelim
            var model = new IzinModel
            {
                izin_id = Guid.NewGuid(), // Yeni GUID oluşturuluyor
                ad = kullaniciAdiSoyadi,  // Ad soyad bilgisi
                soyad = kullaniciAdiSoyadi,  // Soyad bilgisi
                personel_id = Convert.ToInt64(personelId),  // Personel ID
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult izin_ekle(IzinModel izin)
        {
            var kullaniciAdiSoyadi = HttpContext.Session.GetString("UserName");
            var personelId = HttpContext.Session.GetString("PersonelID");

            // Kullanıcı bilgilerini modelimize ekliyoruz
            izin.ad = kullaniciAdiSoyadi;
            izin.soyad = kullaniciAdiSoyadi;
            izin.personel_id = Convert.ToInt64(personelId);

            if (ModelState.IsValid)
            {
                // Izin modelini kaydediyoruz
                izin.izin_id = Guid.NewGuid();  // GUID otomatik olarak atanır, her durumda yeni bir GUID oluşturulur
                _context.mvc_izin_kayit.Add(izin);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "İzin talebi başarıyla oluşturuldu.";
                return RedirectToAction("Index");  // Dashboard'a döner ki takvimde görsün
            }

            // Eğer model geçerli değilse, hata mesajlarını kullanıcıya göster
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                // Hataları daha iyi bir şekilde görüntülemek için
                ModelState.AddModelError("", error.ErrorMessage);  // Hata mesajlarını formda göstermek için
            }

            return View(izin);  // Eğer model geçerli değilse, formu tekrar göster
        }


        [Authorize]
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



        [Authorize]
        public IActionResult izin_listele()
        {
            var izinler = _context.mvc_izin_kayit.Where(i => i.is_deleted == false).ToList();
            return View(izinler);
        }

        [Authorize]
        [HttpPost]
        public IActionResult izin_sil(Guid izin_id)
        {
            var izin = _context.mvc_izin_kayit.FirstOrDefault(i => i.izin_id == izin_id);
            if (izin == null)
            {
                return NotFound();
            }

            izin.is_deleted = true;
            _context.SaveChanges();
            return RedirectToAction("izin_listele");
        }

        [Authorize]
        [HttpGet]

        public IActionResult izin_guncelle(Guid id)
        {
            var izin = _context.mvc_izin_kayit.FirstOrDefault(i => i.izin_id == id);
            if (izin == null)
            {
                return NotFound(); // Kaydın bulunamaması durumunda 404 döner
            }

            return View(izin); // İzin kaydını view'e gönderir
        }

        [Authorize]
        [HttpPost]
        public IActionResult izin_guncelle(IzinModel model)
        {
            if (ModelState.IsValid)  // Formun geçerli olup olmadığını kontrol edin
            {
                var izin = _context.mvc_izin_kayit.FirstOrDefault(i => i.izin_id == model.izin_id);

                if (izin != null)
                {
                    // İzin kaydını güncelleme
                    izin.personel_id = model.personel_id;
                    izin.ad = model.ad;
                    izin.soyad = model.soyad;
                    izin.izin_turu = model.izin_turu;
                    izin.izin_aciklama = model.izin_aciklama;
                    izin.izin_baslangic_tarihi = model.izin_baslangic_tarihi;
                    izin.izin_bitis_tarihi = model.izin_bitis_tarihi;
                    izin.kurum_id = model.kurum_id;

                    _context.SaveChanges();  // Değişiklikleri kaydet
                    TempData["SuccessMessage"] = "İzin başarıyla güncellendi.";  // Başarı mesajı
                }
                else
                {
                    TempData["ErrorMessage"] = "İzin kaydı bulunamadı.";  // Hata mesajı
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Formda hata var.";  // Geçerli değilse hata mesajı
            }

            return RedirectToAction("izin_listele"); // İzinlerin listelendiği sayfaya yönlendirme
        }


        [Authorize]
        // GET: görev ekleme formunu göster
        [Authorize]
        [Authorize]
        public IActionResult gorev_ekle()
        {
            var kullaniciAdiSoyadi = HttpContext.Session.GetString("UserName");
            var personelId = HttpContext.Session.GetString("PersonelID");

            var model = new GorevModel
            {
                gorev_id = Guid.NewGuid(),
                gorev_baslangic_tarihi = DateTime.Now,  // Bugünün tarihi
                ad = kullaniciAdiSoyadi,
                soyad = kullaniciAdiSoyadi,
                personel_id = Convert.ToInt64(personelId)
            };

            return View(model);
        }


        [Authorize]
        [HttpPost]
        public IActionResult gorev_ekle(GorevModel gorev)
        {
            var kullaniciAdiSoyadi = HttpContext.Session.GetString("UserName");
            var personelId = HttpContext.Session.GetString("PersonelID");

            // Kullanıcı bilgilerini yeniden güvenceye alıyoruz
            gorev.ad = kullaniciAdiSoyadi;
            gorev.soyad = kullaniciAdiSoyadi;
            gorev.personel_id = Convert.ToInt64(personelId);

            // Tarih kontrolü: Eğer kullanıcı tarihi göndermemişse, bugünün tarihini ekleyin
            if (gorev.gorev_baslangic_tarihi == default(DateTime))
            {
                gorev.gorev_baslangic_tarihi = DateTime.Now;  // Bugünün tarihi
            }

            if (ModelState.IsValid)
            {
                gorev.gorev_id = Guid.NewGuid(); // yeni ID
                _context.mvc_gorev_kayit.Add(gorev);
                _context.SaveChanges();
                return RedirectToAction("gorev_listele");
            }

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(gorev);
        }


        [Authorize]
        public IActionResult gorev_listele()
        {


            var gorevler = _context.mvc_gorev_kayit.Where(g => g.is_deleted == false).ToList();
            return View(gorevler);
        }



        [Authorize]
        [HttpPost]
        public IActionResult gorev_sil(Guid gorev_id)
        {
            var gorev = _context.mvc_gorev_kayit.FirstOrDefault(g => g.gorev_id == gorev_id);
            if (gorev != null)
            {
                gorev.is_deleted = true;
                _context.SaveChanges();
            }
            return RedirectToAction("gorev_listele");
        }

        [Authorize]
        [HttpGet]
        public IActionResult gorev_guncelle(Guid id)
        {
            var model = _context.mvc_gorev_kayit.FirstOrDefault(x => x.gorev_id == id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Görev kaydı bulunamadı.";
                return RedirectToAction("gorev_listele");
            }
            return View("gorev_guncelle", model);
        }

        [HttpPost]
        public IActionResult gorev_guncelle(GorevModel model)
        {
            var gorev = _context.mvc_gorev_kayit.FirstOrDefault(x => x.gorev_id == model.gorev_id);
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


        [Authorize]
        [HttpGet]
        public IActionResult Profil()
        {
            var kullaniciAdi = User.Identity.Name;

            var kullanici = _context.mvc_personel_kayit
                .FirstOrDefault(k => k.kullanici_adi == kullaniciAdi);

            if (kullanici == null)
            {
                return RedirectToAction("Giris");
            }

            return View(kullanici);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profil(PersonelModel model, IFormFile fotograf)
        {
            var kullaniciAdi = User.Identity.Name;

            var kullanici = _context.mvc_personel_kayit
                .FirstOrDefault(k => k.kullanici_adi == kullaniciAdi);

            if (kullanici == null)
            {
                return RedirectToAction("Giris");
            }

            if (fotograf != null && fotograf.Length > 0)
            {
                try
                {
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    Directory.CreateDirectory(uploads);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(fotograf.FileName);
                    var filePath = Path.Combine(uploads, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await fotograf.CopyToAsync(stream);
                    }

                    kullanici.foto = "/uploads/" + fileName;
                    _logger.LogInformation("Fotoğraf başarıyla yüklendi: {FotoYolu}", filePath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fotoğraf yüklenirken bir hata oluştu.");
                    ModelState.AddModelError("", "Fotoğraf yüklenirken bir hata oluştu.");
                    return View(kullanici); // Hata olursa form tekrar gösterilsin
                }
            }

            // Diğer alanları güncelle (gerekirse modelden gelenleri set et)
            kullanici.ad = model.ad;
            kullanici.soyad = model.soyad;
            kullanici.telefon = model.telefon;


            _context.SaveChanges();

            TempData["Mesaj"] = "Profil güncellendi.";
            return RedirectToAction("Profil");
        }



        [HttpGet("Etkinlikler")]
        public IActionResult Etkinlikler()
        {
            var izinlerDb = _context.mvc_izin_kayit
                .Where(i => !i.is_deleted)
                .ToList(); // Verileri önce belleğe al

            var izinler = izinlerDb
                .Select(i => new
                {
                    title = "İzin - " + i.ad + " " + i.soyad,
                    start = i.izin_baslangic_tarihi?.ToString("yyyy-MM-dd"),
                    end = i.izin_bitis_tarihi?.AddDays(1).ToString("yyyy-MM-dd"),
                    description = i.izin_turu + " - " + i.izin_aciklama,
                    color = "#3498db"
                })
                .ToList();

            var gorevlerDb = _context.mvc_gorev_kayit
                .Where(g => !g.is_deleted)
                .ToList(); // Aynı şekilde belleğe al

            var gorevler = gorevlerDb
                .Select(g => new
                {
                    title = "Görev - " + g.ad + " " + g.soyad,
                    start = g.gorev_baslangic_tarihi?.ToString("yyyy-MM-dd"),
                    end = g.gorev_baslangic_tarihi?.ToString("yyyy-MM-dd"),
                    description = g.gorev_adi + " - " + g.gorev_aciklama,
                    color = "#2ecc71"
                })
                .ToList();

            var resmiTatiller = new List<object>
    {
        new {
            title = "23 Nisan Ulusal Egemenlik ve Çocuk Bayramı",
            start = "2025-04-23",
            end = "2025-04-24",
            description = "Resmi Tatil",
            color = "#e74c3c"
        },
        new {
            title = "1 Mayıs Emek ve Dayanışma Günü",
            start = "2025-05-01",
            end = "2025-05-02",
            description = "Resmi Tatil",
            color = "#e74c3c"
        }
    };

            var etkinlikler = izinler.Concat(gorevler).Concat(resmiTatiller).ToList();

            return Ok(etkinlikler);
        }

        public IActionResult gizlilik_politikası()
        {
            return View(); // Views/Home/Gizlilik.cshtml
        }

        public IActionResult iletisim()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Ayarlar()
        {
            var personelIdStr = HttpContext.Session.GetString("PersonelID");
            if (string.IsNullOrEmpty(personelIdStr) || !long.TryParse(personelIdStr, out long personelId))
            {
                return RedirectToAction("Giris");
            }

            var kullanici = _context.mvc_personel_kayit.FirstOrDefault(p => p.Id == personelId);

            if (kullanici == null)
            {
                return RedirectToAction("Giris");
            }

            return View(kullanici);
        }


        [Authorize]
        [HttpPost]
        public IActionResult Ayarlar(PersonelModel model, string MevcutSifre, string YeniSifre, string YeniSifreTekrar)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var personel = _context.mvc_personel_kayit.FirstOrDefault(p => p.Id == model.Id);
            if (personel == null)
            {
                return RedirectToAction("ayarlar");
            }

            // Normal bilgileri güncelle
            personel.ad = model.ad;
            personel.soyad = model.soyad;
            personel.kullanici_adi = model.kullanici_adi;
            personel.telefon = model.telefon;
            personel.adres = model.adres;
            personel.TC = model.TC;

            // Şifre Güncelleme Kontrolü
            if (!string.IsNullOrEmpty(MevcutSifre) && !string.IsNullOrEmpty(YeniSifre) && !string.IsNullOrEmpty(YeniSifreTekrar))
            {
                if (personel.sifre != MevcutSifre)
                {
                    ViewBag.Mesaj = "❌ Mevcut şifre hatalı!";
                    return View(personel);
                }

                if (YeniSifre != YeniSifreTekrar)
                {
                    ViewBag.Mesaj = "❌ Yeni şifreler uyuşmuyor!";
                    return View(personel);
                }

                personel.sifre = YeniSifre; // Şifre değiştirildi
            }

            _context.SaveChanges();

            ViewBag.Mesaj = "✅ Bilgileriniz başarıyla güncellendi!";
            return View(personel);
        }



        [HttpGet("GetEtkinlikler")]
        public IActionResult GetEtkinlikler(DateTime start, DateTime end)
        {
            using (var context = new ProjeDbContext())
            {
                var tatil = context.mvc_tatil
                                           .Where(t => t.is_deleted == false)
                                           .ToList();
            }

            var izinler = _context.mvc_izin_kayit
                .Where(i => !i.is_deleted)
                .Select(i => new
                {
                    title = "İzin: " + i.ad + " " + i.soyad,
                    start = i.izin_baslangic_tarihi,
                    end = i.izin_bitis_tarihi,
                    color = "#2196f3"
                });

            var gorevler = _context.mvc_gorev_kayit
                .Where(g => !g.is_deleted)
                .Select(g => new
                {
                    title = "Görev: " + g.gorev_adi + " (" + g.ad + " " + g.soyad + ")",
                    start = g.gorev_baslangic_tarihi,
                    end = g.gorev_baslangic_tarihi,
                    color = "#4caf50"
                });

            var etkinlikler = izinler.Concat(gorevler).ToList();

            return Json(etkinlikler);
        }

        [HttpGet]
        public IActionResult departman()
        {
            // Departmanları ve bağlı yöneticileri alıyoruz
            var departmanlar = _context.mvc_departmanlar
             .Select(d => new DepartmanModel
             {
                 departman_id = d.departman_id,
                 departman_adi = d.departman_adi,
                 yonetici_id = d.yonetici_id,
                 yonetici_adSoyad = d.yonetici != null ? d.yonetici.ad + " " + d.yonetici.soyad : "Yönetici yok"
             }).ToList();

            ViewBag.PersonelListesi = new SelectList(_context.mvc_personel_kayit.ToList(), "personel_id", "adSoyad");

            return View(departmanlar);
        }


        // Departman Ekleme - POST
        [HttpPost]
        public IActionResult departman(string departman_adi, Guid? yonetici_id)
        {
            if (!string.IsNullOrEmpty(departman_adi))
            {
                var yeniDepartman = new DepartmanModel
                {
                    departman_adi = departman_adi,
                    yonetici_id = yonetici_id
                };

                _context.mvc_departmanlar.Add(yeniDepartman);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Departman başarıyla eklendi.";
                return RedirectToAction("departman");
            }

            TempData["ErrorMessage"] = "Departman adı boş olamaz.";
            return RedirectToAction("departman");
        }
        [HttpPost]
        public IActionResult departman_sil(int id)
        {
            var departman = _context.mvc_departmanlar.Find(id);
            if (departman != null)
            {
                _context.mvc_departmanlar.Remove(departman);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Departman başarıyla silindi.";
            }
            return RedirectToAction("departman");
        }

        [HttpPost]
        public IActionResult departman_guncelle(int departman_id, string departman_adi, Guid? yonetici_id)
        {
            try
            {
                var departman = _context.mvc_departmanlar.FirstOrDefault(d => d.departman_id == departman_id);
                if (departman != null)
                {
                    departman.departman_adi = departman_adi;
                    departman.yonetici_id = yonetici_id;

                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Departman başarıyla güncellendi.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Departman bulunamadı.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Güncelleme sırasında hata oluştu: " + ex.Message;
            }

            return RedirectToAction("departman");
        }

        [Authorize]
        public IActionResult Mesajlar()
        {
            var personeller = _context.mvc_personel_kayit
                .Where(p => !p.is_deleted)
                .OrderBy(p => p.ad)
                .ToList();
            
            ViewBag.Personeller = personeller;
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetPrivateMessages(string otherUser)
        {
            var currentUser = User.Identity.Name;
            
            var messages = _context.mvc_mesajlar
                .Where(m => 
                    (m.gonderen_ad == currentUser && m.alici_ad == otherUser) ||
                    (m.gonderen_ad == otherUser && m.alici_ad == currentUser))
                .OrderBy(m => m.tarih)
                .Select(m => new {
                    m.gonderen_ad,
                    m.alici_ad,
                    m.icerik,
                    m.tarih
                })
                .ToList();
            
            return Json(messages);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AyarlarKaydet([FromBody] System.Text.Json.JsonElement data)
        {
            // Tema ve dil tercihlerini burada kaydedebilirsiniz. 
            // Şu an için başarılı dönüyoruz.
            return Ok(new { success = true });
        }

    }
}


















