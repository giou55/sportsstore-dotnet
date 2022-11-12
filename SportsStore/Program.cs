using Microsoft.EntityFrameworkCore;
using SportsStore.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


/*builder.Services.AddDbContext<StoreDbContext>(
    options => options.UseSqlServer(
        builder.Configuration["ConnectionStrings:SportsStoreConnectionLocal"])
);*/

/*builder.Services.AddDbContext<AppIdentityDbContext>(
    options => options.UseSqlServer(
        builder.Configuration["ConnectionStrings:IdentityConnectionLocal"])
);*/

builder.Services.AddScoped<IStoreRepository, EFStoreRepository>();
builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();

builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddServerSideBlazor();

var storeConnectionString = "server=linuxzone34.grserver.gr;port=3306;user=giourmet449939;password=85!Lh65FdqW;database=sportsstore_dotnet";
var identityConnectionString = "server=linuxzone34.grserver.gr;port=3306;user=giourmet8888;password=85!Lh65FdqW;database=identity_dotnet";
var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));

builder.Services.AddDbContext<StoreDbContext>(
    dbContextOptions => dbContextOptions.UseMySql(
        storeConnectionString,
        ServerVersion.AutoDetect(storeConnectionString),
        options => options.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: System.TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)
        )
);

builder.Services.AddDbContext<AppIdentityDbContext>(
    dbContextOptions => dbContextOptions.UseMySql(
        identityConnectionString,
        ServerVersion.AutoDetect(identityConnectionString),
        options => options.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: System.TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)
        )
);

/*builder.Services.AddDbContext<IdentityDbContext>(
    dbContextOptions => dbContextOptions.UseMySql(
        identityConnectionString, serverVersion)
);
*/

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppIdentityDbContext>();

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.UseStaticFiles();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute("catpage", "{category}/Page{productPage:int}",
    new { Controller = "Home", action = "Index" });

app.MapControllerRoute("page", "Page{productPage:int}",
    new { Controller = "Home", action = "Index", productPage = 1 });

app.MapControllerRoute("category", "{category}",
    new { Controller = "Home", action = "Index", productPage = 1 });

app.MapControllerRoute("pagination", "Products/Page{productPage}",
    new { Controller = "Home", action = "Index", productPage = 1 });

app.MapDefaultControllerRoute();
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/admin/{*catchall}", "/Admin/Index");

SeedData.EnsurePopulated(app);
IdentitySeedData.EnsurePopulated(app);

app.Run();
