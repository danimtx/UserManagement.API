namespace UserManagement.Application.DTOs.Admin
{
    public class PersonalTagDecisionDto
    {
        public string PersonalUserId { get; set; } = string.Empty;
        public string TagName { get; set; } = string.Empty;
        public bool Approve { get; set; }
        public string? RejectionReason { get; set; }
    }
}
