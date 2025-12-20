using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpClient();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Login/LoginIndex");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
string? connStr = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connStr))
{
    Console.WriteLine("Bağlantı dizesi bulunamadı!");
}
else
{
    Console.WriteLine("Bağlantı dizesi: " + connStr);
}
try
{
    using (var connection = new SqlConnection(connStr))
    {
        connection.Open();
        Console.WriteLine("Veritabanına başarıyla bağlanıldı.");
    }
}
catch (Exception ex)
{
    Console.WriteLine("Veritabanı bağlantı hatası: " + ex.Message);
    app.MapGet("/", () => Results.Problem("Veritabanına bağlanılamadı."));
    app.Run();
    return;
}


app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "login",
    pattern: "{controller=Login}/{action=LoginIndex}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "dashboard",
    pattern: "{controller=Dashboard}/{action=DashboardMain}/{id?}");
    
app.MapControllerRoute(
    name: "assetsn",
    pattern: "{controller=AssetSN}/{action=AssetSNMatcher}/{id?}");

app.MapControllerRoute(
    name: "commands",
    pattern: "{controller=Commands}/{action=CommandPage}/{compName?}");
    
app.Run();
