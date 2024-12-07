using AspnetCoreMvcFull.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);
ExcelPackage.LicenseContext = LicenseContext.Commercial; // Si tienes una licencia comercial

// Registra el DbContext ManagmentRfidDbContext en el contenedor de servicios
builder.Services.AddDbContext<ManagmentRfidDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar IHttpContextAccessor para acceso al contexto HTTP
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Registrar servicios de controlador y vistas
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Cambiar la ruta predeterminada para que inicie en Auth/LoginBasic
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=LoginBasic}/{id?}");

app.Run();
