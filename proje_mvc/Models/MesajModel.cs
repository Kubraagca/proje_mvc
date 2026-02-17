using System;
using System.ComponentModel.DataAnnotations;

namespace proje_mvc.Models
{
    public class MesajModel
    {
        [Key]
        public int id { get; set; }
        
        [Required]
        public string gonderen_ad { get; set; }
        
        public string alici_ad { get; set; } // Alıcı kullanıcı adı (null ise genel sohbet)
        
        [Required]
        public string icerik { get; set; }
        
        public DateTime tarih { get; set; } = DateTime.Now;
        
        public string oda_adi { get; set; } = "Genel";
        
        public bool okundu { get; set; } = false; // Mesaj okundu mu?
    }
}
