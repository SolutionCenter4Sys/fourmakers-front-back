using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.RH;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.GestaoPessoa.GestaoDesempenho.RH
{
    public interface IGestaoDesempenhoRHRepository
    {
        Task<int> ObterQtdTotalColaboradoresAsync(int orgId);
        Task<Dictionary<string, DateTime?>> ObterTodasUltimasDataOneOnOneAsync(int orgId);
        Task<Dictionary<string, DateTime?>> ObterTodasUltimasDataFeedbackAsync(int orgId);
        Task<List<string>> ObterTodosCodigosInternosColaboradoresAsync(int orgId);
        Task<List<ColaboradorRHDTO>> ObterTodosColaboradoresComDetalhesAsync(int orgId);
    }
}
