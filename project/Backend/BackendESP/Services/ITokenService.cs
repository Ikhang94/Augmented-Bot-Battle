using BackendESP.DTO;

namespace BackendESP.Services
{
    public interface ITokenService
    {
        Task<string> GenerateToken(PlayerDTO player);
    }
}
