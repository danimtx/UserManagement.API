using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Application.DTOs
{
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RubroSeleccionado { get; set; } = string.Empty;
    }
}
