using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Infrastructure.Persistence.Models
{
    [FirestoreData]
    public class UserDocument
    {
        [FirestoreProperty] public string Id { get; set; } = string.Empty;
        [FirestoreProperty] public string Email { get; set; } = string.Empty;
        [FirestoreProperty] public string? UserName { get; set; }
        [FirestoreProperty] public string TipoUsuario { get; set; } = string.Empty;
        [FirestoreProperty] public string Estado { get; set; } = string.Empty;
        [FirestoreProperty] public DateTime FechaRegistro { get; set; }
        [FirestoreProperty] public string? CreadoPorId { get; set; }

        [FirestoreProperty] public PersonalProfileDocument? DatosPersonales { get; set; }
        [FirestoreProperty] public CompanyProfileDocument? DatosEmpresa { get; set; }

        [FirestoreProperty] public SubAccountPermissionsDocument? PermisosEmpleado { get; set; }
        [FirestoreProperty] public SystemAdminPermissionsDocument? PermisosAdminSistema { get; set; }

        [FirestoreProperty] public List<string> ModulosHabilitados { get; set; } = new();
        [FirestoreProperty] public List<string> FuncionalidadesExtra { get; set; } = new();
        [FirestoreProperty] public List<string> IdsRecursosExternos { get; set; } = new();
    }

    [FirestoreData]
    public class PersonalProfileDocument
    {
        [FirestoreProperty] public string Nombres { get; set; } = string.Empty;
        [FirestoreProperty] public string ApellidoPaterno { get; set; } = string.Empty;
        [FirestoreProperty] public string ApellidoMaterno { get; set; } = string.Empty;
        [FirestoreProperty] public DateTime FechaNacimiento { get; set; }
        [FirestoreProperty] public string CI { get; set; } = string.Empty;
        [FirestoreProperty] public string FotoCiUrl { get; set; } = string.Empty;
        [FirestoreProperty] public string Pais { get; set; } = string.Empty;
        [FirestoreProperty] public string Departamento { get; set; } = string.Empty;
        [FirestoreProperty] public string Direccion { get; set; } = string.Empty;
        [FirestoreProperty] public string Celular { get; set; } = string.Empty;
        [FirestoreProperty] public string Profesion { get; set; } = string.Empty;
        [FirestoreProperty] public string? FotoTituloUrl { get; set; }
        [FirestoreProperty] public string? LinkedInUrl { get; set; }
        [FirestoreProperty] public string? Nit { get; set; }
        [FirestoreProperty] public bool VerificadoMarket { get; set; } = false;
        [FirestoreProperty] public Dictionary<string, string> DocumentosValidacion { get; set; } = new();
        [FirestoreProperty] public List<UploadedDocumentDocument> DocumentosSoporte { get; set; } = new();
    }

    [FirestoreData]
    public class CompanyProfileDocument
    {
        [FirestoreProperty] public string RazonSocial { get; set; } = string.Empty;
        [FirestoreProperty] public string TipoEmpresa { get; set; } = string.Empty;
        [FirestoreProperty] public string Nit { get; set; } = string.Empty;
        [FirestoreProperty] public LegalRepresentativeDocument Representante { get; set; } = new();
        [FirestoreProperty] public string TelefonoFijo { get; set; } = string.Empty;
        [FirestoreProperty] public string CelularCorporativo { get; set; } = string.Empty;
        [FirestoreProperty] public List<CompanyBranchDocument> Sucursales { get; set; } = new();
        [FirestoreProperty] public List<UploadedDocumentDocument> DocumentosLegales { get; set; } = new();
        [FirestoreProperty] public List<string> AreasDefinidas { get; set; } = new();
    }

    [FirestoreData]
    public class LegalRepresentativeDocument
    {
        [FirestoreProperty] public string Nombres { get; set; } = string.Empty;
        [FirestoreProperty] public string ApellidoPaterno { get; set; } = string.Empty;
        [FirestoreProperty] public string ApellidoMaterno { get; set; } = string.Empty;
        [FirestoreProperty] public string CI { get; set; } = string.Empty;
        [FirestoreProperty] public DateTime FechaNacimiento { get; set; }
        [FirestoreProperty] public string DireccionDomicilio { get; set; } = string.Empty;
        [FirestoreProperty] public string EmailPersonal { get; set; } = string.Empty;
        [FirestoreProperty] public List<string> NumerosContacto { get; set; } = new();
    }

    [FirestoreData]
    public class CompanyBranchDocument
    {
        [FirestoreProperty] public string NombreSucursal { get; set; } = string.Empty;
        [FirestoreProperty] public List<string> ModulosAsociados { get; set; } = new();
        [FirestoreProperty] public string DireccionEscrita { get; set; } = string.Empty;
        [FirestoreProperty] public string Departamento { get; set; } = string.Empty;
        [FirestoreProperty] public string Pais { get; set; } = string.Empty;
        [FirestoreProperty] public double Latitud { get; set; }
        [FirestoreProperty] public double Longitud { get; set; }
        [FirestoreProperty] public string? TelefonoSucursal { get; set; }
        [FirestoreProperty] public bool EsActiva { get; set; } = true;
    }

    [FirestoreData]
    public class UploadedDocumentDocument
    {
        [FirestoreProperty] public string TipoDocumento { get; set; } = string.Empty;
        [FirestoreProperty] public string UrlArchivo { get; set; } = string.Empty;
        [FirestoreProperty] public string ModuloObjetivo { get; set; } = string.Empty;
        [FirestoreProperty] public string EstadoValidacion { get; set; } = "Pendiente";
        [FirestoreProperty] public DateTime FechaSubida { get; set; }
        [FirestoreProperty] public string? MotivoRechazo { get; set; }
    }

    [FirestoreData]
    public class SubAccountPermissionsDocument
    {
        [FirestoreProperty] public string AreaTrabajo { get; set; } = string.Empty;
        [FirestoreProperty] public bool EsSuperAdminEmpresa { get; set; } = false;
        [FirestoreProperty] public Dictionary<string, ModuleAccessDocument> Modulos { get; set; } = new();
    }

    [FirestoreData]
    public class ModuleAccessDocument
    {
        [FirestoreProperty] public bool TieneAcceso { get; set; }
        [FirestoreProperty] public List<string> FuncionalidadesPermitidas { get; set; } = new();
        [FirestoreProperty] public List<string> RecursosEspecificosAllowed { get; set; } = new();
    }

    [FirestoreData]
    public class SystemAdminPermissionsDocument
    {
        [FirestoreProperty] public string RolSistema { get; set; } = string.Empty;
        [FirestoreProperty] public List<string> PermisosGlobales { get; set; } = new();
    }
}
