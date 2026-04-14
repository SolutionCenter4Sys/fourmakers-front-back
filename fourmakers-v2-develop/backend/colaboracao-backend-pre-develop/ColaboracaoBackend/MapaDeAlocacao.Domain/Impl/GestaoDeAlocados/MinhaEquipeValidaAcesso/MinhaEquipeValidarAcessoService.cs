using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.MinhaEquipeValidaAcesso;
using System;

namespace MapaDeAlocacao.Domain.Impl.GestaoDeAlocados.MinhaEquipeValidaAcesso
{
    [LogDomainClass]
    public class MinhaEquipeValidarAcessoService : IMinhaEquipeValidarAcessoService
    {
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;

        public MinhaEquipeValidarAcessoService(IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository)
        {
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
        }

        public void ValidaAcessoMinhaEquipe(string cpfRequest, int orgId)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpfRequest, orgId, FuncionalidadeSistemaEnum.MINHA_EQUIPE);

            if (!isValid.Result)
            {
                throw new UnauthorizedAccessException("Acesso negado para Minha Equipe.");
            }
        }

        public void ValidaAcessoMinhaEquipeOrquestracao(string cpfRequest, int orgId)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpfRequest, orgId, FuncionalidadeSistemaEnum.MINHA_EQUIPE_ORQUESTRACAO);

            if (!isValid.Result)
            {
                throw new UnauthorizedAccessException("Acesso negado para Minha Equipe Orquestração.");
            }
        }
    }
}

