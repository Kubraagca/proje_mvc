using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace proje_mvc.Models
{
    public class ProjeDbContext : DbContext
    {
        public ProjeDbContext()
        {
        }

        public ProjeDbContext(DbContextOptions<ProjeDbContext> options) : base(options) { }
     

        // Veritabanındaki tabloları temsil eden DbSet'ler
        public DbSet<PersonelModel> mvc_personel_kayit { get; set; }
        public DbSet<KurumModel> mvc_kurum_kayit { get; set; }
        public DbSet<KartModel> mvc_kart_kayit { get; set; }
        public DbSet<GorevModel> mvc_gorev_kayit { get; set; }
        public DbSet<IzinModel> mvc_izin_kayit { get; set; }
        public DbSet<KullaniciModel> mvc_kullanici_giris { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) // Eğer konfigüre edilmemişse
            {
                optionsBuilder.UseSqlServer("Data Source=94.73.144.8;Initial Catalog=u6699064_db601;Persist Security Info=True;User ID=u6699064_user601;Password=:4Rbp3=4_9F:AwJr;TrustServerCertificate=True");
            }
        }
    }
}
