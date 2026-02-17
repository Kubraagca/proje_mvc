using System;
using System.ComponentModel.DataAnnotations;

namespace proje_mvc.Models
{
    public class MusteriModel
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Müşteri/Firma adı zorunludur.")]
        [Display(Name = "Ünvan / Ad Soyad")]
        public string unvan { get; set; }

        [Display(Name = "Müşteri Tipi")]
        public string tip { get; set; } // Bireysel, Kurumsal

        [Display(Name = "Vergi Dairesi")]
        public string vergi_dairesi { get; set; }

        [Display(Name = "Vergi / TC No")]
        public string vergi_no { get; set; }

        [Display(Name = "Telefon")]
        public string telefon { get; set; }

        [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi.")]
        [Display(Name = "E-posta")]
        public string eposta { get; set; }

        [Display(Name = "Adres")]
        public string adres { get; set; }

        [Display(Name = "Cari Bakiye")]
        public decimal bakiye { get; set; } = 0; // Borç/Alacak takibi için

        public bool is_deleted { get; set; } = false;
        public DateTime olusturma_tarihi { get; set; } = DateTime.Now;
    }
}
