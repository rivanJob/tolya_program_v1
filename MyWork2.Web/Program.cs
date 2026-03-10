using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using MyWork2.Web.Data;
using MyWork2.Web.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var mariadbVersion = builder.Configuration["Database:MariaDbVersion"] ?? "10.4.32";

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MariaDbServerVersion(Version.Parse(mariadbVersion))));
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});
builder.Services.AddScoped<IRepairOrderService, RepairOrderService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IStockService, StockService>();

var app = builder.Build();

app.UseForwardedHeaders();

var pathBase = builder.Configuration["ReverseProxy:PathBase"];
if (!string.IsNullOrWhiteSpace(pathBase))
{
    app.UsePathBase(pathBase);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
