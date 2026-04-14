using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Util.Competencia;
using Competencia.Domain.Enums;
using Core.Domain.BI;
using Dapper;
using DataTransferObject.Domain.Competencia;

namespace Colaboracao.Infra.Repositories.BI;

public class BICompentenciaRepository(IDBConnection dapperCoonnection) : IBICompetenciaRepository
{
	private readonly IDbConnection _connection = dapperCoonnection.GetConnection();
	
    public async Task<List<CompetenciaGroupDTO>> ObterCompetenciasPorTipoEListaDeIds(int ItemPerfilTipoID, List<long> ids)
    {
        if (ids == null || ids.Count == 0)
            return new List<CompetenciaGroupDTO>();

        var tipoBancoHard = CompetenciaUtils.ConverterPerfilItemParaTipoCompetenciaSRS((ItemPerfilEnum)ItemPerfilTipoID); // 1 no banco            

        var query = @"SELECT
	                        v.id AS Id,
	                        v.descricao AS Descricao,
	                        @tipoBancoHard AS TipoIdSRS,
	                        v.confirmada AS Confirmada
                        FROM vw_skills v
                        WHERE
	                        v.tipo_id = @itemPerfilSRS
	                        AND v.id IN @Ids";
        var parametros = new { itemPerfilSRS = ItemPerfilTipoID, tipoBancoHard = (int)tipoBancoHard, Ids = ids };
        var result = await _connection.QueryAsync<CompetenciaGroupDTO>(query, parametros);
        return result.ToList();
    }
}