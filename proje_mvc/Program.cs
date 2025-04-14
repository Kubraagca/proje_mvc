using Microsoft.EntityFrameworkCore;
using proje_mvc.Models;

var builder = WebApplication.CreateBuilder(args);

// Veritaban� ba�lant�s�n� yap�land�r
builder.Services.AddDbContext<ProjeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session i�in gerekli servisleri ekliyoruz
builder.Services.AddDistributedMemoryCache();  // Cache servisi ekleniyor
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Oturum s�resi ayar�
    options.Cookie.HttpOnly = true;  // �erezin yaln�zca Http �zerinden eri�ilebilir olmas�
    options.Cookie.IsEssential = true;  // �erezin temel olmas�n� sa�l�yoruz
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Session kullan�m�n� aktif hale getiriyoruz
app.UseSession();  // Bu sat�r, session'� etkinle�tiriyor

// HTTP istekleri i�in pipeline'� yap�land�r�yoruz
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
