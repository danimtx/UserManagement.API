using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore; // <--- NECESARIO
using UserManagement.Application.Interfaces;
using UserManagement.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ==============================================
// 1. CONFIGURACIÓN DE CREDENCIALES
// ==============================================
var firebaseCredentialPath = Path.Combine(Directory.GetCurrentDirectory(), "firebase_credentials.json");

// Validación de seguridad: Que el archivo exista
if (!File.Exists(firebaseCredentialPath))
{
    throw new FileNotFoundException($"No se encontró el archivo JSON en: {firebaseCredentialPath}");
}

// Variable de entorno para las librerías de Google
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", firebaseCredentialPath);

// ==============================================
// 2. INICIALIZACIÓN DE SERVICIOS GOOGLE
// ==============================================

// A. Firebase Auth
if (FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile(firebaseCredentialPath)
    });
}

// B. Firestore (BASE DE DATOS) - ¡ESTO FALTABA!
// Registramos FirestoreDb como Singleton para que esté disponible en toda la app.
// Usamos tu Project ID exacto.
builder.Services.AddSingleton(FirestoreDb.Create("usermanagement-9a721"));


// ==============================================
// 3. INYECCIÓN DE DEPENDENCIAS
// ==============================================
builder.Services.AddHttpClient();
// Ahora sí funcionará porque ya registramos FirestoreDb arriba
builder.Services.AddScoped<IAuthService, FirebaseAuthService>();

// Inyectamos el servicio de Admin para que el controlador pueda usarlo
builder.Services.AddScoped<IAdminService, AdminService>();

// ==============================================
// 4. CONFIGURACIÓN DE API Y SWAGGER
// ==============================================
builder.Services.AddControllers();

// Usaremos SwaggerGen en lugar de OpenApi básico para tener la interfaz gráfica de prueba
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ==============================================
// 5. PIPELINE HTTP
// ==============================================
if (app.Environment.IsDevelopment())
{
    // Habilitamos la UI de Swagger para probar visualmente
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();