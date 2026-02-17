using System;
using System.ComponentModel.DataAnnotations;

namespace proje_mvc.Models
{
    public class EtkinlikModel
    {
        [Key]
        public int id { get; set; }
        
        [Required]
        public string baslik { get; set; }
        
        public string aciklama { get; set; }
        
        public DateTime baslangic_tarihi { get; set; }
        
        public DateTime bitis_tarihi { get; set; }
        
        public string renk { get; set; } = "#3b82f6";
    }
}
