using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using UserManagement.Application.Interfaces;
using UserManagement.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
var firebaseCredentialPath = Path.Combine(Directory.GetCurrentDirectory(), "firebase_credentials.json");
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", firebaseCredentialPath);

// Inicializar Firebase App
// Nota: Verifica si ya existe para evitar errores en Hot Reload
if (FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile(firebaseCredentialPath)
    });
}
builder.Services.AddHttpClient(); // Necesario para el Login
builder.Services.AddScoped<IAuthService, FirebaseAuthService>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
