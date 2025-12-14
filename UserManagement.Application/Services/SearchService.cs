using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Public;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Application.Interfaces.Services;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Entities; // Needed for UserType

namespace UserManagement.Application.Services
{
    public class SearchService : ISearchService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService; 

        public SearchService(IUserRepository userRepository, IUserService userService)
        {
            _userRepository = userRepository;
            _userService = userService;
        }

        public async Task<SearchResultDto> SearchAsync(string query, string? ciudad)
        {
            var users = await _userRepository.SearchUsersAsync(query, ciudad);

            var result = new SearchResultDto();

            foreach (var user in users.Where(u => u.TipoUsuario == UserType.Personal.ToString()))
            {
                try
                {
                    var publicProfile = await _userService.GetPublicProfileAsync(user.Id);
                    result.Personas.Add(publicProfile);
                }
                catch (KeyNotFoundException)
                {
                    // If GetPublicProfileAsync throws, it means the profile is not available or deleted, skip it for public search
                }
            }

            foreach (var user in users.Where(u => u.TipoUsuario == UserType.Empresa.ToString() && u.DatosEmpresa != null))
            {
                var matchingProfiles = user.DatosEmpresa.PerfilesComerciales
                    .Where(p => p.Estado == CommercialProfileStatus.Activo &&
                                (p.NombreComercial.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                (p.ModuloAsociado?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)))
                    .Select(p => new PublicCommercialProfileDto
                    {
                        Id = p.Id,
                        Nombre = p.NombreComercial,
                        Tipo = p.Tipo.ToString(),
                        LogoUrl = p.LogoUrl,
                        RubroModulo = p.ModuloAsociado,
                        Estrellas = p.CalificacionPromedio,
                        TotalResenas = p.TotalResenas
                    }).ToList();
                
                result.Empresas.AddRange(matchingProfiles);
            }

            return result;
        }
    }
}
