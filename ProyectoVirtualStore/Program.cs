using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using ProyectoVirtualStore.Data;
using ProyectoVirtualStore.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient(builder.Configuration.GetSection("KeyVault"));
});

//DEBEMOS RECUPERAR, DE FORMA EXPLICITA EL SECRETCLIENT INYECTADO 

SecretClient secretClient =builder.Services.BuildServiceProvider().GetService<SecretClient>();

KeyVaultSecret keyVaultSecretSql = await secretClient.GetSecretAsync("SqlAzure");
KeyVaultSecret keyVaultSecretStorage = await secretClient.GetSecretAsync("StorageAccount");
KeyVaultSecret keyVaultSecretCache = await secretClient.GetSecretAsync("CacheRedis");

// Add services to the container. 

string connectionString = keyVaultSecretSql.Value;
string azurestorage= keyVaultSecretStorage.Value;
string cnnCacheRedis = keyVaultSecretCache.Value;

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = cnnCacheRedis;
});
    
BlobServiceClient blobServiceClient = new BlobServiceClient(azurestorage);
builder.Services.AddTransient<BlobServiceClient>(x => blobServiceClient);



builder.Services.AddScoped<ServiceApiTienda>();

// Registrar ServiceCacheRedis con la inyección de dependencias
builder.Services.AddScoped<ServiceCacheRedis>();


// Add services to the container.

builder.Services.AddTransient<ServiceStorageBlobs>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(x => x.IdleTimeout = TimeSpan.FromMinutes(30));
builder.Services.AddAuthentication(options =>
{
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie();


builder.Services.AddControllersWithViews(x => x.EnableEndpointRouting = false).AddSessionStateTempDataProvider();


//string connectionString =
//  "Data Source=LOCALHOST\\DESARROLLO;Initial Catalog=TIENDAVIRTUAL;User ID=SA;Password=MCSD2023";








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

app.UseSession();

app.UseMvc(routes =>
{
    routes.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}/{id?}"
            );
    routes.MapRoute(
           name: "game",
           template: "{controller=Games}/{action=Index}/{idjuego?}"
           );


});


app.Run();

static partial class Keys { 


}