using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Usuario.Permissao;
using Logs.Infra.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Usuario.Domain.Interfaces.Services.Permissao;

namespace Usuario.Domain.Impl.Services.Permissao
{
    [LogDomainClass]
    public class FuncionalidadeSistemaService : IFuncionalidadeSistemaService
    {
        private IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;

        public FuncionalidadeSistemaService(
            IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository)
        {
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
        }

        public async Task<IEnumerable<FuncionalidadeSistemaDTO>> ListarFuncionalidadesSistema(string cpfRequest, int orgId)
        {
            ValidaAcessoPermissaoGrupoAcesso(cpfRequest, orgId);
            return await _funcionalidadeSistemaRepository.ListarFuncionalidadesSistema();
        }

        public async Task<IEnumerable<PessoaFuncionalidadeSistemaDTO>> ListarPessoasPorFuncionalidadeSistema(string cpfRequest, int orgId, int? funcionalidadeId = null)
        {
            ValidaAcessoPermissaoGrupoAcesso(cpfRequest, orgId);
            return await _funcionalidadeSistemaRepository.ListarPessoasPorFuncionalidadeSistema(orgId, funcionalidadeId);
        }

        private void ValidaAcessoPermissaoGrupoAcesso(string cpf, int orgId)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpf, orgId, FuncionalidadeSistemaEnum.CONTROLE_FUNCIONALIDADE_SISTEMA_GRUPO_ACESSO);

            if (!isValid.Result)
            {
                throw new UnauthorizedAccessException("Acesso negado para acessar Grupo Acesso no Controle de Funcionalidade do Sistema.");
            }
        }
    }
}