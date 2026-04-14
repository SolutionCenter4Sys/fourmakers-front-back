using Colaborador.Domain.Interfaces.Services.BancoDeTalentos;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using System;

namespace Colaborador.Domain.Impl.Services.BancoDeTalentos
{
    [LogDomainClass]
    public class BancoDeTalentosValidatorService : IBancoDeTalentosValidatorService
    {
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;

        public BancoDeTalentosValidatorService(IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository)
        {
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
        }

        public void ValidaAcessoBuscarPessoasCadastradasPorOrg(string cpf, int orgId)
        {
            var temAcesso = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpf, 
                orgId, 
                FuncionalidadeSistemaEnum.CADASTRO_PESSOAS
            ).Result;

            if (!temAcesso)
            {
                throw new UnauthorizedAccessException("Acesso negado. Este endpoint requer acesso à funcionalidade CADASTRO_PESSOAS.");
            }
        }
    }
}

