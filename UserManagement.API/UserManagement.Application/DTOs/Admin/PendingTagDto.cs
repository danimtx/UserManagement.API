using System;

namespace UserManagement.Application.DTOs.Admin
{
    public class PendingTagDto
    {
        public string RequestType { get; set; } = string.Empty; // "Personal" or "Empresa"
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string TagOrProfileId { get; set; } = string.Empty;
        public string TagOrProfileName { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
    }
}
