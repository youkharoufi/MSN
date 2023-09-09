using MSN.Models;

namespace MSN.Token
{
    public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user);
    }
}
