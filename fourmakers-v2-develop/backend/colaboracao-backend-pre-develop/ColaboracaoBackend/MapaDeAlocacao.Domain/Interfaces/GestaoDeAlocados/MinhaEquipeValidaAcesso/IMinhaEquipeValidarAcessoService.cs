namespace MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.MinhaEquipeValidaAcesso
{
    public interface IMinhaEquipeValidarAcessoService
    {
        void ValidaAcessoMinhaEquipe(string cpfRequest, int orgId);
        void ValidaAcessoMinhaEquipeOrquestracao(string cpfRequest, int orgId);
    }
}

