using Azure.Identity;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using StealTheCats;
using StealTheCats.Data;
using StealTheCats.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Load user secrets
builder.Configuration.AddUserSecrets<Program>();

// Try to load Azure Key Vault
var keyVaultUri = builder.Configuration["CatVaultUri"] ?? Environment.GetEnvironmentVariable("CatVaultUri");

if (!string.IsNullOrEmpty(keyVaultUri))
{
    try
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUri),
            new DefaultAzureCredential()
        );
        Console.WriteLine(AppResources.SuccessAzureLogin);
    }
    catch (Exception ex)
    {
        Console.WriteLine(string.Format(AppResources.FailedKeyVaultConnection, ex.Message));
    }
}
else
    Console.WriteLine(AppResources.UserSecretsConf);

// Add services to the container.

builder.Services.AddAutoMapper(typeof(CatMappingProfile));

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Hangfire services with SQL Server storage (adjust connection string)
builder.Services.AddHangfire(config => config.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

// Add Hangfire server to process jobs in background
builder.Services.AddHangfireServer();

builder.Services.AddHttpClient();

builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.InNamespaces("StealTheCats.Services"))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");

app.MapControllers();

app.Run();
