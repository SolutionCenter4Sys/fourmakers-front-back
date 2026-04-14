using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Extension;
using Core.Domain.Colaborador;
using Dapper;
using DataTransferObject.Domain.Diretoria;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Colaborador
{
    public class ColaboradorDepartamentoRepository : IColaboradorDepartamentoRepository
    {
        private readonly IDBConnection _dapperConnection;

        public ColaboradorDepartamentoRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public List<DepartamentoColaboradorDTO> ListarDepartamentosDosColaboradores(int orgId, string codigoDiretoria, List<string>? restricaoDiretorias = null)
        {
            var connection = _dapperConnection.GetConnection();
            var inClauseDiretorias = restricaoDiretorias.BuildInClauseOrNull();

            string sql = $@"
                            SELECT
                                c.cod_departamento AS Cod,
                                c.departamento AS Departamento
                            FROM
                                tb_colaborador_org c
                            WHERE
                                c.tb_org_id = @OrgId
                                AND c.cod_departamento IS NOT NULL
                                AND c.departamento IS NOT NULL
                                AND TRIM(c.cod_departamento) != ''
                                AND TRIM(c.departamento) != ''
                                AND (@CodigoDiretoria IS NULL OR c.cod_diretoria = @CodigoDiretoria)
                                {(inClauseDiretorias == null ? "" : $"AND c.cod_diretoria IN {inClauseDiretorias}")}

                            UNION

                            SELECT
                                DISTINCT
                                tdo.cod_departamento AS Cod,
                                tdo.departamento AS Departamento
                            FROM
                                tb_departamento_org tdo
                            WHERE
                                tdo.tb_org_id = @OrgId
                                AND tdo.cod_departamento IS NOT NULL
                                AND tdo.departamento IS NOT NULL;
                            ";

            var listaDepartamentoOrdenada = connection.Query<DepartamentoColaboradorDTO>(sql, new
            {
                OrgId = orgId,
                CodigoDiretoria = codigoDiretoria
            })
                                                                                                   .OrderBy(c => c.Departamento)
                                                                                                   .ToList();

            return listaDepartamentoOrdenada;
        }
    }
}