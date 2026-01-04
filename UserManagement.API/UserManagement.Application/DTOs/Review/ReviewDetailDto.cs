using System;

namespace UserManagement.Application.DTOs.Review
{
    public class ReviewDetailDto
    {
        public string AutorNombre { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comentario { get; set; }
        public DateTime Fecha { get; set; }
    }
}
