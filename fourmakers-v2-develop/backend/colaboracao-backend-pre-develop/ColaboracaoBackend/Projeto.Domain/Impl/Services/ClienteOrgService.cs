using Colaboracao.Helper;
using Core.DomainModel.Projeto;
using DataTransferObject.Domain.Projeto;
using Logs.Infra.Attributes;
using Projeto.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Projeto.Domain.Impl.Services
{
    [LogDomainClass]
    public class ClienteOrgService : IClienteOrgService
    {
        private readonly IClienteOrgRepository _clienteOrgRepository;

        public ClienteOrgService(IClienteOrgRepository clienteOrgRepository)
        {
            _clienteOrgRepository = clienteOrgRepository;
        }

        public async Task<List<ClienteOrgDTO>> ListarClientesOrg(int orgId, string codigoClienteFiltro, string codigoGerenteProjeto)
        {
            try
            {
                return await _clienteOrgRepository.ListarClientesOrg(orgId, codigoClienteFiltro, codigoGerenteProjeto.ToNullSeTextoNullOuZero());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ClienteOrgDTO ObterClientePorCodigo(string codCliente, int orgId)
        {
            try
            {
                return _clienteOrgRepository.ObterClientePorCodigo(codCliente, orgId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task<ClienteOrgDTO> ObterClientePorCodigoAsync(string codCliente, int orgId)
        {
            try
            {
                return await _clienteOrgRepository.ObterClientePorCodigoAsync(codCliente, orgId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task InativarClienteCasoNaoEstejaAssociadoANenhumProjetoAsync(string codCliente, int orgId)
        {
            try
            {
                var qtdProjetosAssociadosAoCliente = (await _clienteOrgRepository.ListarCodigosDosProjetosAssociadosAoAsync(codCliente, orgId)).Count;
                if (qtdProjetosAssociadosAoCliente == 0)
                {
                   await _clienteOrgRepository.InativarClienteAsync(codCliente, orgId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task AtivarClienteCasoEstejaAssociadoAAlgumProjetoAsync(string codCliente, int orgId)
        {
            try
            {
                var qtdProjetosAssociadosAoCliente =
                    (await _clienteOrgRepository.ListarCodigosDosProjetosAssociadosAoAsync(codCliente, orgId)).Count;
                if (qtdProjetosAssociadosAoCliente > 0)
                {
                    await _clienteOrgRepository.AtivarClienteAsync(codCliente, orgId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}