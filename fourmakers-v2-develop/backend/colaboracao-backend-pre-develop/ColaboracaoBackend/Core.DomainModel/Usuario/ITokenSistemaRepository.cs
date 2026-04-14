namespace Core.Domain.Usuario
{
    public interface ITokenSistemaRepository
    {
        bool ValidaTokenSistema(string token, int orgId);
    }
}