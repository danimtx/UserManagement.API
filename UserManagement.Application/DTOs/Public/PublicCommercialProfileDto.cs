namespace UserManagement.Application.DTOs.Public
{
    public class PublicCommercialProfileDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? RubroModulo { get; set; }
        public double Estrellas { get; set; }
        public int TotalResenas { get; set; }
    }
}
