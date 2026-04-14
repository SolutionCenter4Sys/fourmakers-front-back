using DataTransferObject.Domain.MapaDeAlocacao;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.MapaAlocacao
{
    public interface IMapaDeAlocacaoExternoRepository
    {
        Task<List<CadastroMapaAlocacaoDTO>> BuscarAlocacoesPorMesAno(int mes, int ano, int orgId, string codigoColaborador = null, string codigoProjeto = null);
        Task<DateTime[]> GetFeriadosPorOrgId(int orgId);
    }
}
