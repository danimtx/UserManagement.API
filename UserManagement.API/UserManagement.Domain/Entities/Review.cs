using System;

namespace UserManagement.Domain.Entities
{
    public class Review
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        // Quién escribe la reseña
        public string AuthorId { get; set; } = string.Empty; 
        public string AuthorDisplayName { get; set; } = string.Empty;
        
        // A quién se le escribe la reseña
        public string RecipientId { get; set; } = string.Empty;

        // Sobre qué se está calificando (ID del Perfil Comercial o Nombre del Tag)
        public string ContextoId { get; set; } = string.Empty;
        
        public int Rating { get; set; } // e.g., 1-5 stars
        public string? Comment { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
