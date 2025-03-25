using FootScout_Vue.WebAPI.Models.DTOs;

namespace FootScout_Vue.WebAPI.Services.Interfaces
{
    // Interfejs deklarujący operacje związane z zarządzaniem kontem użytkownika 
    public interface IAccountService
    {
        Task Register(RegisterDTO registerDTO);
        Task<string> Login(LoginDTO loginDTO);
        Task<IEnumerable<string>> GetRoles();
        Task MakeAnAdmin(string userId);
        Task MakeAnUser(string userId);
    }
}