using System;

namespace UserManagement.Application.DTOs.Admin
{
    public class PendingIdentityDto
    {
        public string UserId { get; set; } = string.Empty;
        public string RazonSocial { get; set; } = string.Empty;
        public string Nit { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
    }
}
