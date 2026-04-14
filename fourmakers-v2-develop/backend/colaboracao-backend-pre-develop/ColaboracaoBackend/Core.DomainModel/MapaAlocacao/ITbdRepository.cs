using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.Tbd;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.MapaAlocacao
{
    public interface ITbdRepository
    {
        Task<IEnumerable<TbdAlocacaoDTO>> ListarTbd(string codGestor, int orgId);
        Task<TbdAlocacaoDTO> ObterTbdPorCodigo(int orgId, int codTbd);
        Task<TbdAlocacaoDTO> InserirTbd(TbdAlocacaoParam param, int orgId);
        Task<TbdAlocacaoDTO> AtualizarTbd(TbdAlocacaoParam param, int orgId);
        Task<StatusResult> DeletarTbd(int codTbd, int orgId);
        Task<bool> ExisteColaboradorAtivo(string codGestor, int orgId);
        bool ExisteAlocacaoNoTbd(int codTbd, int orgId);
    }
}