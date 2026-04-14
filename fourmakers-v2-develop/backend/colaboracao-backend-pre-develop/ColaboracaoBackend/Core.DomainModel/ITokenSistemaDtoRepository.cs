namespace Core.DomainModel
{
    public interface ITokenSistemaDtoRepository
    {
        bool ValidaTokenSistema(string token, int orgId);
    }
}
