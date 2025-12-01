using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Domain.Entities.ValueObjects;

namespace UserManagement.Application.DTOs.Admin
{
    public class MarketValidationDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string CI { get; set; } = string.Empty;
        public string Profesion { get; set; } = string.Empty;
        public List<UploadedDocument> DocumentosEvidencia { get; set; } = new();
    }
}
