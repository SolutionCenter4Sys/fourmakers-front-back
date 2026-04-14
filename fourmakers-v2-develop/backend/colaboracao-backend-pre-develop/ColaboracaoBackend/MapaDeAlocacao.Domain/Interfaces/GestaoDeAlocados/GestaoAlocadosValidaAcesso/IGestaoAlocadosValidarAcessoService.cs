namespace MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestaoAlocadosValidaAcesso
{
    public interface IGestaoAlocadosValidarAcessoService
    {
        void ValidaAcessoCadastroGestaoAlocados(string cpfRequest, int orgId);
    }
}