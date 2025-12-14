using System.Threading.Tasks;
using UserManagement.Application.DTOs.Public;

namespace UserManagement.Application.Interfaces.Services
{
    public interface ISearchService
    {
        Task<SearchResultDto> SearchAsync(string query, string? ciudad);
    }
}
