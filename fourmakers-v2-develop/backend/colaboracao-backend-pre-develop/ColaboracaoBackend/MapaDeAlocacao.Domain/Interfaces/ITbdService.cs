using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.Tbd;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapaDeAlocacao.Domain.Interfaces
{
    public interface ITbdService
    {
        Task<IEnumerable<TbdAlocacaoDTO>> ListarTbd(string codGestor, int orgId);
        Task<TbdAlocacaoDTO> ObterTbdPorCodigo(int orgId, int codTbd);
        Task<TbdAlocacaoDTO> InserirTbd(TbdAlocacaoParam param, int OrgId);
        Task<TbdAlocacaoDTO> AtualizarTbd(TbdAlocacaoParam param, int orgId);
        Task<StatusResult> DeletarTbd(int codTbd, int orgId);
        bool ExisteAlocacaoNoTbd(int codTbd, int orgId);
    }
}