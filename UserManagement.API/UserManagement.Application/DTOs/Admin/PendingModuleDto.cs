using System;

namespace UserManagement.Application.DTOs.Admin
{
    public class PendingModuleDto
    {
        public string CompanyUserId { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string CommercialProfileId { get; set; } = string.Empty;
        public string ProfileName { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
    }
}
