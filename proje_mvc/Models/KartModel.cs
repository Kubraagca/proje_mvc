using System.ComponentModel.DataAnnotations;

namespace proje_mvc.Models

{
    public class KartModel
    {
        [Key]
        public long Id { get; set; }
        public string kurum_id { get; set; }
        public string personel_id { get; set; }
        public DateTime kayit_tarihi { get; set; }
        public Guid kart_id { get; set; } = Guid.NewGuid();
    }

}
