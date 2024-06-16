using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Models;

using PruebaTecnica.Services.Contracts;
using PruebaTecnica.Services.Implementations;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Conexión DB.
builder.Services.AddDbContext<DBPruebaTecnicaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBPruebaTecnica")
));

// UsuarioService.
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// PersonaService.
builder.Services.AddScoped<IPersonaService, PersonaService>();

// Configuración de autenticación Usario.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Inicio/InicioSesion";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

// Configuración para deshabilitar el caché.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(
        new ResponseCacheAttribute
        {
            NoStore = true,
            Location = ResponseCacheLocation.None
        });
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Inicio}/{action=InicioSesion}/{id?}");
app.Run();
