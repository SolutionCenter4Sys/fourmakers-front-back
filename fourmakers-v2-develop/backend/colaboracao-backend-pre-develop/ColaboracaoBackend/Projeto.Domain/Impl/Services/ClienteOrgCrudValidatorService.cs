using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.DomainModel.Projeto;
using DataTransferObject.Domain.Projeto.ClienteOrgCrud;
using Projeto.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logs.Infra.Attributes;

namespace Projeto.Domain.Impl.Services
{
    [LogDomainClass]
    public class ClienteOrgCrudValidatorService : IClienteOrgCrudValidatorService
    {
        private IClienteOrgCrudRepository _clienteOrgCrudRepository;

        public ClienteOrgCrudValidatorService(IClienteOrgCrudRepository clienteOrgCrudRepository)
        {
            _clienteOrgCrudRepository = clienteOrgCrudRepository;
        }

        public async Task ValidaClienteOrgCrud(ClienteOrgCrudInput input, CRUDEnum crudOperation)
        {
            if (crudOperation == CRUDEnum.Create || crudOperation == CRUDEnum.Update)
            {
                await ValidaCamposObrigatorios(input, crudOperation);

                if (crudOperation == CRUDEnum.Create)
                {
                    await ValidaSeJaExisteClienteOrgCrud(input.CodigoCliente, input.NomeCliente, input.OrgId);
                }

                if (crudOperation == CRUDEnum.Update)
                {
                    var clienteOrgCrudAtual = await _clienteOrgCrudRepository.ObterClienteOrgCrudPorIdAsync(input.Id, input.OrgId);

                    if (input.CodigoCliente != clienteOrgCrudAtual?.CodigoCliente)
                    {
                        await ValidaSeJaExisteClienteOrgCrud(input.CodigoCliente, input.NomeCliente, input.OrgId);
                        await ValidaSeExisteClienteOrgComProjetoUtilizando(input.CodigoCliente, crudOperation, input.OrgId);
                    }
                }
            }

            if (crudOperation == CRUDEnum.Update || crudOperation == CRUDEnum.Delete)
            {
                await ValidaSeExisteClienteOrgCrud(input.Id, input.OrgId);

                if (crudOperation == CRUDEnum.Delete) //somente no delete pq já validou o update no if de cima
                {
                    await ValidaSeExisteClienteOrgComProjetoUtilizando(input.CodigoCliente, crudOperation, input.OrgId);
                }
            }
        }

        private async Task ValidaSeExisteClienteOrgCrud(Guid id, int orgId)
        {
            var clienteOrgCrud = await _clienteOrgCrudRepository.ObterClienteOrgCrudPorIdAsync(id, orgId);

            if (clienteOrgCrud.IsNull())
            {
                throw new ApplicationException($"Cliente com id: '{id.ToString()}' não encontrado.");
            }
        }

        private async Task ValidaSeExisteClienteOrgComProjetoUtilizando(string codigoClienteOrgCrud, CRUDEnum cRUDEnum, int orgId)
        {
            string acao = "";

            if (cRUDEnum == CRUDEnum.Delete) acao = "deletado";
            if (cRUDEnum == CRUDEnum.Update) acao = "editado";

            if ((await _clienteOrgCrudRepository.ListarCodigosDosProjetosAssociadosAoClienteAsync(codigoClienteOrgCrud, orgId)).Any())
            {
                throw new ApplicationException($"O Código Cliente '{codigoClienteOrgCrud}' não pode ser {acao} devido à sua associação atual com projetos em andamento.");
            }
        }

        private async Task ValidaSeJaExisteClienteOrgCrud(string codigoCliente, string nomeCliente, int orgId)
        {
            var clienteOrg = await _clienteOrgCrudRepository.ObterClienteOrgCrudPorCodigoAsync(codigoCliente, orgId);

            if (clienteOrg.IsNotNull())
            {
                throw new ApplicationException($@"Cliente com codigo: {codigoCliente} já existente."
                                                + Environment.NewLine + $"Cliente encontrado: {clienteOrg.CodigoCliente} + {clienteOrg.NomeCliente}");
            }

            clienteOrg = await _clienteOrgCrudRepository.ObterClienteOrgCrudPorNomeAsync(nomeCliente, orgId);

            if (clienteOrg.IsNotNull())
            {
                throw new ApplicationException($"Cliente com nome: {nomeCliente} já existente."
                                                + Environment.NewLine + $"Cliente encontrado: {clienteOrg.CodigoCliente} + {clienteOrg.NomeCliente}");
            }
        }

        private async Task ValidaCamposObrigatorios(ClienteOrgCrudInput input, CRUDEnum cRUDEnum)
        {
            var errorMessages = new List<string>();

            if (cRUDEnum == CRUDEnum.Update)
            {
                if (string.IsNullOrEmpty(input.Id.ToString()))
                {
                    errorMessages.Add("'Id'");
                }
            }

            if (string.IsNullOrEmpty(input.NomeCliente))
            {
                errorMessages.Add("'NomeCliente'");
            }

            if (string.IsNullOrEmpty(input.CodigoCliente))
            {
                errorMessages.Add("'CodigoCliente'");
            }

            if (errorMessages.Any())
            {
                var plural = (errorMessages.Count > 1 ? "s" : "");
                throw new ApplicationException($"Campo{plural} {string.Join("; ", errorMessages)} obrigatório{plural}");
            }
        }
    }
}