namespace Colaborador.Domain.Interfaces.Services.BancoDeTalentos
{
    public interface IBancoDeTalentosValidatorService
    {
        void ValidaAcessoBuscarPessoasCadastradasPorOrg(string cpf, int orgId);
    }
}

