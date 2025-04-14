using System.ComponentModel.DataAnnotations;

namespace proje_mvc.Models
{
    public class IzinModel
    {
        [Key]
        public long Id { get; set; }
        public long personel_id { get; set; }
        public string? ad { get; set; }
        public string? soyad { get; set; }
        public string? izin_turu { get; set; }
        public string? izin_aciklama { get; set; }
        public DateTime? izin_baslangic_tarihi { get; set; }
        public DateTime? izin_bitis_tarihi { get; set; }
        public string? kurum_id { get; set; }
        public Guid izin_id { get; set; } = Guid.NewGuid();
    }


}



