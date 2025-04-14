using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proje_mvc.Models
{
    public class GorevModel
    {
        [Key]
        public long? Id { get; set; }

        public long? personel_id { get; set; }
        public string? ad { get; set; }
        public string? soyad { get; set; }
        public string? gorev_adi { get; set; }
        public string? gorev_aciklama { get; set; }
       
        public DateTime? gorev_baslangic_tarihi { get; set; }

        public Guid gorev_id { get; set; } = Guid.NewGuid();
    }


}

