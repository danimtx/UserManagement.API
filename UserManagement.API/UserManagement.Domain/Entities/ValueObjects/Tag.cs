using System;
using System.Collections.Generic;
using UserManagement.Domain.Enums;

namespace UserManagement.Domain.Entities.ValueObjects
{
    public class Tag
    {
        public string Nombre { get; set; } = string.Empty;
        public TagStatus Estado { get; set; }
        public bool EsEmpirico { get; set; }
        public List<UploadedDocument> Evidencias { get; set; } = new List<UploadedDocument>();
        public double CalificacionPromedio { get; set; }
        public int TotalResenas { get; set; }
    }
}
