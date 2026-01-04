using System.Collections.Generic;

namespace UserManagement.Application.DTOs.Public
{
    public class SearchResultDto
    {
        public List<PublicUserProfileDto> Personas { get; set; } = new();
        public List<PublicCommercialProfileDto> Empresas { get; set; } = new();
    }
}
