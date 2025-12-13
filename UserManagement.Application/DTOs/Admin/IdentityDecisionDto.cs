namespace UserManagement.Application.DTOs.Admin
{
    public class IdentityDecisionDto
    {
        public string UserId { get; set; } = string.Empty;
        public bool Approve { get; set; }
        public string? RejectionReason { get; set; }
    }
}
