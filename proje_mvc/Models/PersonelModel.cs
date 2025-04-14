using System.ComponentModel.DataAnnotations;
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
        public string? kurum_id { get; set; }
        public string? kart_id { get; set; }
    }
}

      
