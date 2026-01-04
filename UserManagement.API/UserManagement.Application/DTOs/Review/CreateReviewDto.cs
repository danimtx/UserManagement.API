namespace UserManagement.Application.DTOs.Review
{
    public class CreateReviewDto
    {
        public string RecipientId { get; set; } = string.Empty;
        public string ContextoId { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
