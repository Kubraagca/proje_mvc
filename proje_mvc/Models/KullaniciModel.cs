using System.ComponentModel.DataAnnotations;

namespace proje_mvc.Models
{
    public class KullaniciModel
    {
        [Key]
        public int personel_id { get; set; }

        public string? kullanici_adi { get; set; }

        public string? sifre { get; set; }
    }
}
