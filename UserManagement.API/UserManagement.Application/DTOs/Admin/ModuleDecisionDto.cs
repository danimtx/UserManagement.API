namespace UserManagement.Application.DTOs.Admin
{
    public class ModuleDecisionDto
    {
        public string CompanyUserId { get; set; } = string.Empty;
        public string CommercialProfileId { get; set; } = string.Empty;
        public bool Approve { get; set; }
        public string? RejectionReason { get; set; }
    }
}
