using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestaoAlocadosValidaAcesso;
using System;

namespace MapaDeAlocacao.Domain.Impl.GestaoDeAlocados.GestaoAlocadosValidaAcesso
{
    [LogDomainClass]
    public class GestaoAlocadosValidarAcessoService : IGestaoAlocadosValidarAcessoService
    {
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;

        public GestaoAlocadosValidarAcessoService(IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository)
        {
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
        }

        public void ValidaAcessoCadastroGestaoAlocados(string cpfRequest, int orgId)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpfRequest, orgId, FuncionalidadeSistemaEnum.CADASTRO_GESTAO_ALOCADOS);

            if (!isValid.Result)
            {
                throw new UnauthorizedAccessException("Acesso negado para Gestão Alocados."); //não pode adicionar, editar e remover
            }
        }
    }
}