namespace UserManagement.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
