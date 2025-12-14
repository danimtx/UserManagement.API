namespace UserManagement.Application.DTOs.Admin
{
    public class PersonalModuleDecisionDto
    {
        public string PersonalUserId { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
        public bool Approve { get; set; }
        public string? RejectionReason { get; set; }
    }
}
