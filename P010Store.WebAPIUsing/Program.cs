using Microsoft.AspNetCore.Authentication.Cookies;
using P010Store.Data;
using P010Store.Service.Abstract;
using P010Store.Service.Concrete;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DatabaseContext>();
builder.Services.AddTransient(typeof(IService<>), typeof(Service<>));
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddSession(); // Uygulamada session kullanmamız gerekirse.
builder.Services.AddHttpClient(); // Web API'yi kullanabilmemiz için gerekli servis.
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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
app.UseSession(); // Uygulamada session kullanmamız gerekirse.

app.UseAuthentication(); // Önce oturum açma.
app.UseAuthorization(); // Sonra yetkilendirme.

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
