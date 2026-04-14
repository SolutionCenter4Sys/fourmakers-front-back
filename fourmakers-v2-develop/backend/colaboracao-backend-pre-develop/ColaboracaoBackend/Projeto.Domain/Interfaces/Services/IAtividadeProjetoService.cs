using DataTransferObject.Domain.Projeto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Projeto.Domain.Interfaces.Services
{
    public interface IAtividadeProjetoService
    {
        Task<List<AtividadeProjetoDTO>> ListarAtividades(int orgId);
        Task AssociarAtividadesComProjeto(int orgId, List<AtividadeProjetoDTO> atividadesCadastradas, string projetoId);
        Task<List<AtividadeProjetoDTO>> CadastroAtividades(int orgId, List<string> atividades, string codProjeto);
        void RemoverAtividadesAssociadasComProjeto(int orgId, List<string> atividades, string codProjeto);
    }
}