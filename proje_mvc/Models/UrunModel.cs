using System;
using System.ComponentModel.DataAnnotations;

namespace proje_mvc.Models
{
    public class UrunModel
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [Display(Name = "Ürün Adı")]
        public string ad { get; set; }

        [Display(Name = "Ürün Kodu")]
        public string kod { get; set; }

        [Display(Name = "Kategori")]
        public string kategori { get; set; }

        [Display(Name = "Birim")]
        public string birim { get; set; } // Adet, KG, Metre vb.

        [Display(Name = "Stok Adedi")]
        public decimal stok_miktari { get; set; } = 0;

        [Display(Name = "Alış Fiyatı")]
        public decimal alis_fiyati { get; set; } = 0;

        [Display(Name = "Satış Fiyatı")]
        public decimal satis_fiyati { get; set; } = 0;

        [Display(Name = "KDV Oranı (%)")]
        public int kdv_orani { get; set; } = 18;

        public bool is_deleted { get; set; } = false;
        public DateTime olusturma_tarihi { get; set; } = DateTime.Now;
    }
}
