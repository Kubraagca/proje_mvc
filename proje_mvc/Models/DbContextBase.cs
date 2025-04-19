using Microsoft.EntityFrameworkCore;

namespace proje_mvc.Models
{
    public class DbContextBase
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) // Eğer konfigüre edilmemişse
            {
                optionsBuilder.UseSqlServer("Data Source=94.73.144.8;Initial Catalog=u6699064_db601;Persist Security Info=True;User ID=u6699064_user601;Password=:4Rbp3=4_9F:AwJr;TrustServerCertificate=True");
            }
        }
    }
}