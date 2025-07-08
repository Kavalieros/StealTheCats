using Azure.Identity;
using StealTheCats;

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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
