using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Application.DTOs
{
    public class RegisterUserDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string? ApellidoMaterno { get; set; }
        public string Pais { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Profesion { get; set; } = string.Empty;
    }
}
