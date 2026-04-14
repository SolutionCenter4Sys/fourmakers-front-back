using DataTransferObject.Domain.Projeto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Projeto
{
    public interface IAtividadeProjetoRepository
    {
        Task<List<AtividadeProjetoDTO>> ListarAtividades(int orgId);
        Task<List<AtividadeProjetoDTO>> ListarAtividadesAssociadasComProjeto(string codProjeto, int orgId);
        Task AssociarAtividadesComProjeto(int orgId, List<AtividadeProjetoDTO> atividadesCadastradas, string codProjeto);
        Task<List<AtividadeProjetoDTO>> CadastroAtividades(int orgId, List<string> atividades, string codProjeto);
        void RemoverAtividadesAssociadasComProjeto(int orgId, List<string> atividadesCadastradas, string codProjeto);
        Task RemoverAtividadesAssociadasComProjetoAsync(int orgId, List<string> atividades, string codProjeto);
    }
}