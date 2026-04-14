using Core.Domain.Projeto;
using DataTransferObject.Domain.Projeto;
using Projeto.Domain.Interfaces.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Projeto.Domain.Impl.Services
{
    [LogDomainClass]
    public class AtividadeProjetoService : IAtividadeProjetoService
    {
        private readonly IAtividadeProjetoRepository _atividadeProjetoRepository;
        public AtividadeProjetoService(IAtividadeProjetoRepository atividadeProjetoRepository)
        {
            _atividadeProjetoRepository = atividadeProjetoRepository;
        }
        Task<List<AtividadeProjetoDTO>> IAtividadeProjetoService.CadastroAtividades(int orgId, List<string> atividades, string codProjeto)
        {
            return _atividadeProjetoRepository.CadastroAtividades(orgId, atividades, codProjeto);
        }

        public Task<List<AtividadeProjetoDTO>> ListarAtividades(int orgId)
        {
            return _atividadeProjetoRepository.ListarAtividades(orgId);
        }

        public Task AssociarAtividadesComProjeto(int orgId, List<AtividadeProjetoDTO> atividadesCadastradas, string codProjeto)
        {
            return _atividadeProjetoRepository.AssociarAtividadesComProjeto(orgId, atividadesCadastradas, codProjeto);
        }

        public void RemoverAtividadesAssociadasComProjeto(int orgId, List<string> atividadesCadastradas, string codProjeto)
        {
            _atividadeProjetoRepository.RemoverAtividadesAssociadasComProjeto(orgId, atividadesCadastradas, codProjeto);
        }
    }
}