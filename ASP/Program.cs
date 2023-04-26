using ASP.DATA;
using ASP.Middleware;
using ASP.Services;
using ASP.Services.Hash;
using ASP.Services.Kdf;
using ASP.Services.Random;
using ASP.Services.Validation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TimeService>();
builder.Services.AddTransient<DateService>();
builder.Services.AddScoped<DtService>();
builder.Services.AddSingleton<IHashService, MD5HashService>();
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MainDb")));
builder.Services.AddControllersWithViews();  // Add services to the container.
builder.Services.AddSingleton<IRandomService, RandomServiceV1>();
builder.Services.AddSingleton<IKdfService, HashKdfService>();
builder.Services.AddSingleton<IValidationService, ValidationServiceV1>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
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
app.UseAuthorization();
app.UseSession();
app.UseMiddleware<SessionAuthMiddleware>();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();