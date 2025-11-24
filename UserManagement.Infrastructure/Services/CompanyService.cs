using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using UserManagement.Application.DTOs.Company;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Entities.ValueObjects;
using UserManagement.Domain.Enums;

public class CompanyService : ICompanyService
{
    private readonly FirebaseAuth _firebaseAuth;
    private readonly FirestoreDb _firestoreDb;
    private const string CollectionName = "users";

    public CompanyService(FirestoreDb firestoreDb)
    {
        _firebaseAuth = FirebaseAuth.DefaultInstance;
        _firestoreDb = firestoreDb;
    }

    public async Task<string> CreateEmployeeAsync(string companyId, CreateEmployeeDto dto)
    {
        // 1. Validar que la "Empresa Padre" exista y sea válida
        DocumentReference companyRef = _firestoreDb.Collection(CollectionName).Document(companyId);
        DocumentSnapshot companySnap = await companyRef.GetSnapshotAsync();

        if (!companySnap.Exists) throw new Exception("La empresa no existe.");
        User empresa = companySnap.ConvertTo<User>();

        if (empresa.TipoUsuario != UserType.Empresa.ToString())
            throw new Exception("Solo las cuentas de Empresa pueden crear empleados.");

        // Variable de seguridad para el Rollback
        string createdUid = "";

        try
        {
            // ---------------------------------------------------------
            // PASO 2: Crear usuario en Firebase Auth
            // ---------------------------------------------------------
            var userArgs = new UserRecordArgs
            {
                Email = dto.Email,
                Password = dto.PasswordTemporal,
                DisplayName = $"{dto.Nombres} {dto.ApellidoPaterno}",
                Disabled = false
            };

            UserRecord userRecord = await _firebaseAuth.CreateUserAsync(userArgs);
            createdUid = userRecord.Uid; // Guardamos el ID por si hay que borrarlo luego

            // ---------------------------------------------------------
            // PASO 3: Preparar Datos y Permisos
            // ---------------------------------------------------------
            var permisosMapeados = new Dictionary<string, ModuleAccess>();

            foreach (var item in dto.Permisos)
            {
                // REGLA DE SEGURIDAD: Ignorar Wallet y Market para subcuentas
                if (item.Key.ToLower() == "wallet" || item.Key.ToLower() == "market") continue;

                // REGLA DE SEGURIDAD: Solo dar acceso si la empresa lo tiene contratado
                if (empresa.ModulosHabilitados == null || !empresa.ModulosHabilitados.Contains(item.Key)) continue;

                permisosMapeados.Add(item.Key, new ModuleAccess
                {
                    TieneAcceso = item.Value.Acceso,
                    FuncionalidadesPermitidas = item.Value.Funcionalidades,
                    RecursosEspecificosAllowed = item.Value.RecursosIds
                });
            }

            // ---------------------------------------------------------
            // PASO 4: Crear la Entidad User para Firestore
            // ---------------------------------------------------------
            var newEmployee = new User
            {
                Id = createdUid,
                Email = dto.Email,
                TipoUsuario = UserType.SubCuenta.ToString(),
                Estado = UserStatus.Activo.ToString(),
                FechaRegistro = DateTime.UtcNow,
                CuentaPadreId = companyId,
                AreaTrabajo = dto.AreaTrabajo,
                EsSuperAdminEmpresa = dto.EsSuperAdmin,

                // Datos Personales
                DatosPersonales = new PersonalProfile
                {
                    Nombres = dto.Nombres,
                    ApellidoPaterno = dto.ApellidoPaterno,
                    ApellidoMaterno = dto.ApellidoMaterno,
                    CI = dto.CI,
                    Direccion = dto.DireccionEscrita,
                    Celular = dto.Celular,
                    Pais = "Bolivia",

                    // IMPORTANTE: Convertir a UTC para evitar error de Firestore
                    FechaNacimiento = dto.FechaNacimiento.ToUniversalTime()
                },

                // Permisos
                PermisosEmpleado = new SubAccountPermissions
                {
                    Modulos = permisosMapeados
                },

                // Módulos globales habilitados (Lectura rápida)
                ModulosHabilitados = permisosMapeados.Keys.ToList(),
                IdsPerfilesSociales = new List<string>()
            };

            // ---------------------------------------------------------
            // PASO 5: Guardar en Firestore
            // ---------------------------------------------------------
            await _firestoreDb.Collection(CollectionName).Document(createdUid).SetAsync(newEmployee);

            return createdUid; // ¡Éxito total!
        }
        catch (Exception ex)
        {
            // 🚨 ROLLBACK: Si algo falló (ej: error de fecha o base de datos caída),
            // borramos al usuario de Auth para no dejar "zombies".
            if (!string.IsNullOrEmpty(createdUid))
            {
                try
                {
                    await _firebaseAuth.DeleteUserAsync(createdUid);
                }
                catch
                {
                    // Logear este error crítico en producción
                }
            }

            // Avisamos al frontend del error original
            throw new Exception($"Error al crear empleado (se revirtieron los cambios): {ex.Message}");
        }
    }

    public async Task AddCompanyAreaAsync(string companyId, string newAreaName)
    {
        DocumentReference companyRef = _firestoreDb.Collection(CollectionName).Document(companyId);
        await companyRef.UpdateAsync("DatosEmpresa.AreasDefinidas", FieldValue.ArrayUnion(newAreaName));
    }
}