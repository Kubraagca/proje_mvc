using System.ComponentModel.DataAnnotations;

namespace proje_mvc.Models
{
    public class TakvimModel
    {
        [Key]
        public int tatil_id { get; set; }
        public string? tatil_adi { get; set; }
        public DateTime? baslangic_tarihi { get; set; }
        public DateTime? bitis_tarihi { get; set; }

        public bool is_deleted { get; set; }

    }
}
