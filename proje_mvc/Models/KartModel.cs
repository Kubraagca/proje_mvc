using System.ComponentModel.DataAnnotations;

namespace proje_mvc.Models

{
    public class KartModel
    {
        [Key]
        public long Id { get; set; }
        public string? kurum_id { get; set; }
        public long? personel_id { get; set; }
        public DateTime kayit_tarihi { get; set; }
        public Guid kart_id { get; set; } = Guid.NewGuid();
        public bool is_deleted { get; set; }
    }

}
