using P010Store.Data;
using P010Store.Data.Abstract;
using P010Store.Data.Concrete;
using P010Store.Service.Abstract;
using P010Store.Service.Concrete;
using Microsoft.AspNetCore.Authentication.Cookies; // oturum işlemi için gerekli kütüphane

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DatabaseContext>(); // Entityframework işlemlerini yapabilmek için bu satırı ekliyoruz
builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddTransient(typeof(IService<>), typeof(Service<>));// Veritabanı işlemleri yapacağımız servisleri ekledik. Burada .net core a eğer sana IService interface i kullanma isteği gelirse Service sınıfından bir nesne oluştur demiş olduk.
// .net core da 3 farklı yöntemle servisleri ekleyebiliyoruz:

// builder.Services.AddSingleton(); : AddSingleton kullanarak oluşturduğumuz nesneden 1 tane örnek oluşur ve her seferinde bu örnek kullanılır

// builder.Services.AddTransient() yönteminde ise önceden oluşmuş nesne varsa o kullanılır yoksa yenisi oluşturulur

// builder.Services.AddScoped() yönteminde ise yapılan her istek için yeni bir nesne oluşturulur

builder.Services.AddTransient<IProductService, ProductService>(); //product a özel yazdığımız servis
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // IHttpContextAccessor ile uygulama içerisindeki giriş yapan kullanıcı, session verileri, cookie ler gibi içeriklere view lardan veya controllerdan ulaşabilmemizi sağlar.

// Authentication : oturum açma servisi
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x => 
{
    x.LoginPath = "/Admin/Login"; // giriş yapma sayfası
    x.AccessDeniedPath = "/AccessDenied"; // giriş yapan kullanıcının admin yetkisi yoksa AccessDenied sayfasına yönlendir.
    x.LogoutPath = "/Admin/Login/Logout"; // çıkış sayfası
    x.Cookie.Name = "Administrator"; // oluşacak kukinin adı
    x.Cookie.MaxAge = TimeSpan.FromDays(1); // oluşacak kukinin yaşam süresi
});

//Authorization : yetkilendirme
builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("AdminPolicy", policy => policy.RequireClaim("Role", "Admin"));
	x.AddPolicy("UserPolicy", policy => policy.RequireClaim("Role", "User"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//Authentication : oturum açma - giriş yapma
app.UseAuthentication(); //admin login için. UseAuthentication ın UseAuthorization dan önce gelmesi zorunlu!
//Authorization : yetkilendirme (oturum açan kullanıcının admine giriş yetkisi var mı)
app.UseAuthorization();

app.MapControllerRoute(
            name: "admin",
            pattern: "{area:exists}/{controller=Main}/{action=Index}/{id?}"
          );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "custom",
	pattern: "{customurl?}/{controller=Home}/{action=Index}/{id?}");


app.Run();


// Katmanlı Mimari İlişki Hiyerarşisi

/*
 * Web UI üzerinde veritabanı işlemlerini yapabilmek için WebUI ın dependencies(referanslarına) Service katmanını dependencies e sağ tıklayıp add project references diyerek açılan pencereden Service katmanına tik koyup ok butonuyla pencereyi kapatıp bağlantı kurduk.
 * Service katmanı da veritabanı işlemlerini yapabilmek için Data katmanına erişmesi gerekiyor, yine dependencies e sağ tıklayıp add project references diyerek açılan pencereden Data katmanına işaret koyup ekliyoruz.
 * Data katmanının da entity lere erişmesi gerekiyor ki class ları kulanarak veritabanı işlemleri yapabilsin. yine aynı yolu izleyerek veya class ların üzerine gelip ampul e tıklayıp add project references diyerek data dan entities e erişim vermemiz gerekiyor.
 */