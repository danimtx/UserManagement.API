using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using UserManagement.Application.Interfaces;
using UserManagement.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

//CONFIGURACIÓN DE FIREBASE
var firebaseCredentialPath = Path.Combine(Directory.GetCurrentDirectory(), "firebase_credentials.json");
if (!File.Exists(firebaseCredentialPath))
{
    throw new FileNotFoundException($"No se encontró el archivo JSON en: {firebaseCredentialPath}");
}
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", firebaseCredentialPath);

//INICIALIZACIÓN DE SERVICIOS GOOGLE
//Firebase Auth
if (FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile(firebaseCredentialPath)
    });
}

string projectId = builder.Configuration["Firebase:ProjectId"];
builder.Services.AddSingleton(FirestoreDb.Create(projectId));



//INYECCIÓN DE DEPENDENCIAS
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAuthService, FirebaseAuthService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://securetoken.google.com/{projectId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{projectId}",
            ValidateAudience = true,
            ValidAudience = projectId,
            ValidateLifetime = true
        };
    });


//CONFIGURACIÓN DE API Y SWAGGER
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserManagement API", Version = "v1" });

    // Definir esquema de seguridad (Bearer Token)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Comentado por ahora para evitar problemas de puerto local

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();