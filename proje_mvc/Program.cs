using Microsoft.EntityFrameworkCore;
using proje_mvc.Models;

var builder = WebApplication.CreateBuilder(args);

// Veritabaný baðlantýsýný yapýlandýr
builder.Services.AddDbContext<ProjeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session için gerekli servisleri ekliyoruz
builder.Services.AddDistributedMemoryCache();  // Cache servisi ekleniyor
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Oturum süresi ayarý
    options.Cookie.HttpOnly = true;  // Çerezin yalnýzca Http üzerinden eriþilebilir olmasý
    options.Cookie.IsEssential = true;  // Çerezin temel olmasýný saðlýyoruz
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Session kullanýmýný aktif hale getiriyoruz
app.UseSession();  // Bu satýr, session'ý etkinleþtiriyor

// HTTP istekleri için pipeline'ý yapýlandýrýyoruz
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
