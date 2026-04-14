namespace MapaDeAlocacao.Domain.Interfaces.Perfil.PerfilValidaAcesso
{
    public interface IPerfilValidarAcessoService
    {
        void ValidaAcessoCadastroGestaoAlocados(string cpfRequest, int orgId);
    }
}