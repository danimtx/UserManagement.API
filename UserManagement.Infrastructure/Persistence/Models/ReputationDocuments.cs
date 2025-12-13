using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace UserManagement.Infrastructure.Persistence.Models
{
    [FirestoreData]
    public class GeoLocationDocument
    {
        [FirestoreProperty] public double Lat { get; set; }
        [FirestoreProperty] public double Lng { get; set; }
    }

    [FirestoreData]
    public class TagDocument
    {
        [FirestoreProperty] public string Nombre { get; set; } = string.Empty;
        [FirestoreProperty] public string Estado { get; set; } = string.Empty;
        [FirestoreProperty] public bool EsEmpirico { get; set; }
        [FirestoreProperty] public List<UploadedDocumentDocument> Evidencias { get; set; } = new List<UploadedDocumentDocument>();
        [FirestoreProperty] public double CalificacionPromedio { get; set; }
        [FirestoreProperty] public int TotalResenas { get; set; }
    }

    [FirestoreData]
    public class PerfilComercialDocument
    {
        [FirestoreProperty] public string Id { get; set; } = string.Empty;
        [FirestoreProperty] public string NombreComercial { get; set; } = string.Empty;
        [FirestoreProperty] public string Tipo { get; set; } = string.Empty;
        [FirestoreProperty] public string? ModuloAsociado { get; set; }
        [FirestoreProperty] public string? LogoUrl { get; set; }
        [FirestoreProperty] public string Estado { get; set; } = string.Empty;
        [FirestoreProperty] public List<UploadedDocumentDocument> DocumentosEspecificos { get; set; } = new List<UploadedDocumentDocument>();
        [FirestoreProperty] public double CalificacionPromedio { get; set; }
        [FirestoreProperty] public int TotalResenas { get; set; }
    }

    [FirestoreData]
    public class ReviewDocument
    {
        [FirestoreProperty] public string Id { get; set; } = string.Empty;
        [FirestoreProperty] public string AuthorId { get; set; } = string.Empty;
        [FirestoreProperty] public string RecipientId { get; set; } = string.Empty;
        [FirestoreProperty] public string ContextoId { get; set; } = string.Empty;
        [FirestoreProperty] public int Rating { get; set; }
        [FirestoreProperty] public string? Comment { get; set; }
        [FirestoreProperty] public DateTime Timestamp { get; set; }
    }
}
