using System.ComponentModel.DataAnnotations;

namespace proje_mvc.Models
{
    public class KurumModel
    {
        [Key] 
         public long Id { get; set; }

        public Guid kurum_id { get; set; } = Guid.NewGuid();

        public string? kurum_adi { get; set; }

        public string? iletisim_no { get; set; }

       
    }

}
