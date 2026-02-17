using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace proje_mvc.Models
{
    public class PersonelModel
    {
        [Key]
        public long Id { get; set; }

        public string? ad { get; set; }
        public string? soyad { get; set; }
        public string? adres { get; set; }
        public string? telefon { get; set; }
        public string? TC { get; set; }
        public DateTime? dogum_tarihi { get; set; }
        public DateTime? ise_baslama_tarihi { get; set; }

        public Guid personel_id { get; set; } = Guid.NewGuid();
        public long? kurum_id { get; set; }
        public string? kart_id { get; set; }
       

        public bool is_deleted { get; set; }
        public string? kullanici_adi { get; set; }
        public string? foto { get; set; }

        public string? sifre { get; set; }

        // Veritabanındaki kolonları modelinize ekledim
        public string? yetki { get; set; }
        public string? sifrekontrol { get; set; }

        public int? departman_id { get; set; }

        // Navigation Property (opsiyonel EF için)

        public DepartmanModel? departman { get; set; }
        [NotMapped]
        public string adSoyad => $"{ad} {soyad}";

    }
}

      
