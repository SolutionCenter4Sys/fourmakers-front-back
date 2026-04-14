using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using MapaDeAlocacao.Domain.Interfaces.Perfil.PerfilValidaAcesso;
using System;

namespace MapaDeAlocacao.Domain.Impl.Perfil.PerfilValidaAcesso
{
    [LogDomainClass]
    public class PerfilValidarAcessoService : IPerfilValidarAcessoService
    {
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;

        public PerfilValidarAcessoService(IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository)
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