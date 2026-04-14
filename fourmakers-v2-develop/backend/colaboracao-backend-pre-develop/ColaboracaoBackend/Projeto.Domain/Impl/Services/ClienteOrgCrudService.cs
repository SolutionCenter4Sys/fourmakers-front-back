using Colaboracao.Helper.Enum;
using Core.Domain.Usuario;
using Core.Domain.Usuario.Permissao;
using Core.DomainModel.Projeto;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Projeto.ClienteOrgCrud;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Util;
using Projeto.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Projeto.Domain.Impl.Services
{
    [LogDomainClass]
    public class ClienteOrgCrudService : IClienteOrgCrudService
    {
        private readonly IClienteOrgCrudRepository _clienteOrgCrudRepository;
        private readonly IClienteOrgCrudValidatorService _clienteOrgCrudValidatorService;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly IUsuarioColaboradorRepository _usuarioColaboradorRepository;

        public ClienteOrgCrudService(IClienteOrgCrudRepository clienteOrgCrudRepository,
                                IClienteOrgCrudValidatorService clienteOrgCrudValidatorService,
                                IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
                                IUsuarioColaboradorRepository usuarioColaboradorRepository)
        {
            _clienteOrgCrudRepository = clienteOrgCrudRepository;
            _clienteOrgCrudValidatorService = clienteOrgCrudValidatorService;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _usuarioColaboradorRepository = usuarioColaboradorRepository;
        }

        public async Task<ApiGenericResult<IEnumerable<ClienteOrgCrudResult>>> ListarClienteOrgCruds(string cpfRequest, int orgId)
        {
            ValidaAcessoClienteOrgCrud(cpfRequest, orgId);

            var apiGenericResult = new ApiGenericResult<IEnumerable<ClienteOrgCrudResult>>();

            var result = await _clienteOrgCrudRepository.ListarClienteOrgCrudsAsync(orgId);

            if (result == null)
            {
                apiGenericResult.Sucesso = false;
            }

            apiGenericResult.Retorno = result;

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<ClienteOrgCrudResult>> ObterClienteOrgCrudPorId(Guid id, string cpfRequest, int orgId)
        {
            ValidaAcessoClienteOrgCrud(cpfRequest, orgId);

            var apiGenericResult = new ApiGenericResult<ClienteOrgCrudResult>();

            var result = await _clienteOrgCrudRepository.ObterClienteOrgCrudPorIdAsync(id, orgId);

            if (result == null)
            {
                apiGenericResult.Sucesso = false;
                apiGenericResult.Mensagem = "Cliente não encontrado.";
            }

            apiGenericResult.Retorno = result;

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<ClienteOrgCrudResult>> InserirClienteOrgCrud(ClienteOrgCrudInput clienteOrgCrudInput, string cpfRequest, int orgId)
        {
            ValidaAcessoClienteOrgCrud(cpfRequest, orgId);

            var apiGenericResult = new ApiGenericResult<ClienteOrgCrudResult>();

            clienteOrgCrudInput.ConfigurarParaPersistencia(orgId, cpfRequest, Guid.NewGuid());

            await _clienteOrgCrudValidatorService.ValidaClienteOrgCrud(clienteOrgCrudInput, CRUDEnum.Create);

            var result = await _clienteOrgCrudRepository.InserirClienteOrgCrudAsync(clienteOrgCrudInput, TipoCadastroClienteEnum.CADASTRO_MANUAL_USUARIO);

            if (result == null)
            {
                apiGenericResult.Sucesso = false;
                apiGenericResult.Mensagem = "Cliente não foi inserido ou recuperado corretamente.";
            }

            apiGenericResult.Retorno = result;
            return apiGenericResult;
        }

        public async Task<ApiGenericResult<ClienteOrgCrudResult>> AtualizarClienteOrgCrud(ClienteOrgCrudInput clienteOrgCrudInput, Guid id, string cpfRequest, int orgId)
        {
            ValidaAcessoClienteOrgCrud(cpfRequest, orgId);

            var apiGenericResult = new ApiGenericResult<ClienteOrgCrudResult>();

            clienteOrgCrudInput.ConfigurarParaPersistencia(orgId, cpfRequest, id);

            await _clienteOrgCrudValidatorService.ValidaClienteOrgCrud(clienteOrgCrudInput, CRUDEnum.Update);

            var result = await _clienteOrgCrudRepository.AtualizarClienteOrgCrudAsync(clienteOrgCrudInput, TipoCadastroClienteEnum.CADASTRO_MANUAL_USUARIO);

            if (result == null)
            {
                apiGenericResult.Sucesso = false;
                apiGenericResult.Mensagem = "Cliente não foi atualizado ou recuperado corretamente.";
            }

            apiGenericResult.Retorno = result;
            return apiGenericResult;
        }

        public async Task<ApiGenericResult> DeletarClienteOrgCrud(Guid id, string cpfRequest, int orgId)
        {
            ValidaAcessoClienteOrgCrud(cpfRequest, orgId);

            var apiGenericResult = new ApiGenericResult();

            var clienteOrgCrudResult = (await ObterClienteOrgCrudPorId(id, cpfRequest, orgId)).Retorno;

            var clienteOrgInput = new ClienteOrgCrudInput();

            clienteOrgInput.AtualizarSafeComPropriedadesDe<ClienteOrgCrudBase>(clienteOrgCrudResult);
            clienteOrgInput.ConfigurarParaPersistencia(orgId, cpfRequest, id);

            await _clienteOrgCrudValidatorService.ValidaClienteOrgCrud(clienteOrgInput, CRUDEnum.Delete);

            var sucesso = await _clienteOrgCrudRepository.DeletarClienteOrgCrudAsync(id, cpfRequest, orgId);

            apiGenericResult.Sucesso = sucesso;
            apiGenericResult.Mensagem = sucesso ? "Cliente excluído com sucesso." : "Cliente não encontrado.";
            return apiGenericResult;
        }

        private void ValidaAcessoClienteOrgCrud(string cpfRequest, int orgId)
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