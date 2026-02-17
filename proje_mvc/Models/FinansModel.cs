using System;
using System.ComponentModel.DataAnnotations;

namespace proje_mvc.Models
{
    public class FinansModel
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();

        [Display(Name = "İşlem Tarihi")]
        public DateTime tarih { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [Display(Name = "Açıklama")]
        public string aciklama { get; set; }

        [Display(Name = "İşlem Tipi")]
        public string tip { get; set; } // Gelir, Gider

        [Display(Name = "Kategori")]
        public string kategori { get; set; } // Maaş, Kira, Satış, Alış vb.

        [Required(ErrorMessage = "Tutar zorunludur.")]
        [Display(Name = "Tutar")]
        public decimal tutar { get; set; }

        [Display(Name = "Ödeme Yöntemi")]
        public string odeme_yontemi { get; set; } // Nakit, Banka, Kredi Kartı

        public bool is_deleted { get; set; } = false;
    }
}
