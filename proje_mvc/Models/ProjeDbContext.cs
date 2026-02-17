using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace proje_mvc.Models
{
    public class ProjeDbContext : DbContext
    {
        public ProjeDbContext() { }

        public ProjeDbContext(DbContextOptions<ProjeDbContext> options) : base(options) { }

        public DbSet<PersonelModel> mvc_personel_kayit { get; set; }
        public DbSet<KurumModel> mvc_kurum_kayit { get; set; }
        public DbSet<KartModel> mvc_kart_kayit { get; set; }
        public DbSet<GorevModel> mvc_gorev_kayit { get; set; }
        public DbSet<IzinModel> mvc_izin_kayit { get; set; }
        public DbSet<KullaniciModel> mvc_kullanici_giris { get; set; }
        public DbSet<EtkinlikModel> mvc_etkinlikler { get; set; }
        public DbSet<TakvimModel> mvc_tatil { get; set; }
        public DbSet<DepartmanModel> mvc_departmanlar { get; set; }
        public DbSet<MesajModel> mvc_mesajlar { get; set; }

        // ERP Yeni Modüller
        public DbSet<UrunModel> mvc_stok { get; set; }
        public DbSet<MusteriModel> mvc_musteriler { get; set; }
        public DbSet<FinansModel> mvc_finans { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=94.73.144.8;Initial Catalog=u6699064_db601;Persist Security Info=True;User ID=u6699064_user601;Password=:4Rbp3=4_9F:AwJr;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Departman - Yonetici (Personel) ilişkisi
            modelBuilder.Entity<DepartmanModel>()
                .HasOne(d => d.yonetici)
                .WithMany() // Yonetici olan personelin departman olarak ilişkisi yok burada
                .HasForeignKey(d => d.yonetici_id)
                .HasPrincipalKey(p => p.personel_id) // Burada personel_id ile ilişki kuruluyor
                .OnDelete(DeleteBehavior.Restrict);

            // Personel - Departman ilişkisi
            modelBuilder.Entity<PersonelModel>()
                .HasOne(p => p.departman)
                .WithMany(d => d.Personeller)
                .HasForeignKey(p => p.departman_id)
                .OnDelete(DeleteBehavior.SetNull);
        }




    }
}
