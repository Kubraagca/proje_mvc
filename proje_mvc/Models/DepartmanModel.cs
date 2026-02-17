using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace proje_mvc.Models
{
    public class DepartmanModel
    {
        [Key]
        public int departman_id { get; set; }
        public string? departman_adi { get; set; }

        [Column("yonetici_id")]
        public Guid? yonetici_id { get; set; }

        // Navigation Property (opsiyonel EF için)
        public PersonelModel? yonetici { get; set; }

        // Departmana ait personeller (opsiyonel EF için)
        [NotMapped]
        public List<PersonelModel>? Personeller { get; set; }

        public string? yonetici_adSoyad { get; set; }
    }
}

