using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proje_mvc.Models
{
    [Table("mvc_kullanici_giris")]
    public class KullaniciModel
    {
        [Key]
        public int personel_id { get; set; }

        public string? kullanici_adi { get; set; }

        public string? sifre { get; set; }
    }
}
