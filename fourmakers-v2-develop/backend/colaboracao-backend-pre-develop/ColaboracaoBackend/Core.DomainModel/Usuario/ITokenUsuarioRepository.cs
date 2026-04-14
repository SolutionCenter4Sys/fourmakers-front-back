namespace Core.Domain.Usuario
{
    public interface ITokenUsuarioRepository
    {
        long GetUserIdByToken(string token, bool isSSO);
    }
}