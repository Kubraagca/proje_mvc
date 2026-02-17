using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using proje_mvc.Models;
using proje_mvc.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProjeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Giris";
        options.LogoutPath = "/Home/Cikis";
        options.AccessDeniedPath = "/Home/ErisimEngellendi";
    });

var app = builder.Build();

app.UseSession();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chatHub");

// Veritabanı tablolarını otomatik oluştur ve ERP tablolarını kontrol et
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ProjeDbContext>();
        context.Database.EnsureCreated();

        // ERP Tablo Kontrolleri
        try
        {
            // Mesajlaşma - Tabloyu düşür ve yeniden oluştur (WhatsApp tarzı özel mesajlaşma)
            context.Database.ExecuteSqlRaw(@"
                IF EXISTS (SELECT * FROM sys.tables WHERE name = 'mvc_mesajlar')
                    DROP TABLE mvc_mesajlar;
                
                CREATE TABLE mvc_mesajlar (
                    id INT PRIMARY KEY IDENTITY(1,1), 
                    gonderen_ad NVARCHAR(MAX) NOT NULL, 
                    alici_ad NVARCHAR(MAX) NULL,
                    icerik NVARCHAR(MAX) NOT NULL, 
                    tarih DATETIME2 NOT NULL DEFAULT GETDATE(), 
                    oda_adi NVARCHAR(MAX) NOT NULL DEFAULT 'Genel',
                    okundu BIT NOT NULL DEFAULT 0
                )");

            // Stok & Envanter
            context.Database.ExecuteSqlRaw(@"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'mvc_stok') CREATE TABLE mvc_stok (id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), ad NVARCHAR(MAX) NOT NULL, kod NVARCHAR(MAX), kategori NVARCHAR(MAX), birim NVARCHAR(MAX), stok_miktari DECIMAL(18,2) NOT NULL DEFAULT 0, alis_fiyati DECIMAL(18,2) NOT NULL DEFAULT 0, satis_fiyati DECIMAL(18,2) NOT NULL DEFAULT 0, kdv_orani INT NOT NULL DEFAULT 18, is_deleted BIT NOT NULL DEFAULT 0, olusturma_tarihi DATETIME2 NOT NULL DEFAULT GETDATE())");

            // CRM & Müşteri
            context.Database.ExecuteSqlRaw(@"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'mvc_musteriler') CREATE TABLE mvc_musteriler (id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), unvan NVARCHAR(MAX) NOT NULL, tip NVARCHAR(MAX), vergi_dairesi NVARCHAR(MAX), vergi_no NVARCHAR(MAX), telefon NVARCHAR(MAX), eposta NVARCHAR(MAX), adres NVARCHAR(MAX), bakiye DECIMAL(18,2) NOT NULL DEFAULT 0, is_deleted BIT NOT NULL DEFAULT 0, olusturma_tarihi DATETIME2 NOT NULL DEFAULT GETDATE())");

            // Finans & Kasa
            context.Database.ExecuteSqlRaw(@"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'mvc_finans') CREATE TABLE mvc_finans (id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), tarih DATETIME2 NOT NULL DEFAULT GETDATE(), aciklama NVARCHAR(MAX) NOT NULL, tip NVARCHAR(MAX), kategori NVARCHAR(MAX), tutar DECIMAL(18,2) NOT NULL DEFAULT 0, odeme_yontemi NVARCHAR(MAX), is_deleted BIT NOT NULL DEFAULT 0)");
        }
        catch (Exception ex) 
        { 
             var logger = services.GetRequiredService<ILogger<Program>>();
             logger.LogWarning("Bazı ERP tabloları oluşturulamadı veya zaten mevcut: " + ex.Message);
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabanı başlatılırken bir hata oluştu.");
    }
}

app.Run();
