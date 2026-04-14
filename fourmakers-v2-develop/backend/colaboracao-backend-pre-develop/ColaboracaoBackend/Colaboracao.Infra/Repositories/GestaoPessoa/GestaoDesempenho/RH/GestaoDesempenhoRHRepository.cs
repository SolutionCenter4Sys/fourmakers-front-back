using Colaboracao.Core.Interfaces;
using Core.Domain.GestaoPessoa.GestaoDesempenho.RH;
using Dapper;
using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.RH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.GestaoPessoa.GestaoDesempenho.RH
{
    public class GestaoDesempenhoRHRepository : IGestaoDesempenhoRHRepository
    {
        private readonly IDBConnection _dapperConnection;

        public GestaoDesempenhoRHRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<int> ObterQtdTotalColaboradoresAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT COUNT(DISTINCT c.codigo_interno_colaborador)
                FROM tb_colaborador c
                INNER JOIN tb_colaborador_org co
                    ON c.codigo_interno_colaborador = co.codigo_interno_colaborador
                    AND co.tb_org_id = @OrgId
                WHERE c.ativo = 1 AND co.ativo = 1 AND co.cod_diretoria <> 'BANCO TALENTOS'";

            var total = await connection.ExecuteScalarAsync<int>(sql, new { OrgId = orgId });

            return total;
        }

        public async Task<List<string>> ObterTodosCodigosInternosColaboradoresAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT DISTINCT c.codigo_interno_colaborador
                FROM tb_colaborador c
                INNER JOIN tb_colaborador_org co
                    ON c.codigo_interno_colaborador = co.codigo_interno_colaborador
                    AND co.tb_org_id = @OrgId
                WHERE c.ativo = 1 AND co.ativo = 1 AND co.cod_diretoria <> 'BANCO TALENTOS'";

            var codigos = await connection.QueryAsync<string>(sql, new { OrgId = orgId });

            return codigos.ToList();
        }

        public async Task<Dictionary<string, DateTime?>> ObterTodasUltimasDataOneOnOneAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT
                    ooo.codigo_interno_colaborador_avaliado AS CodigoInterno,
                    MAX(ooo.data_reuniao) AS UltimaData
                FROM tb_gest_desemp_one_on_one ooo
                INNER JOIN tb_colaborador c
                    ON ooo.codigo_interno_colaborador_avaliado = c.codigo_interno_colaborador
                INNER JOIN tb_colaborador_org co
                    ON c.codigo_interno_colaborador = co.codigo_interno_colaborador
                    AND co.tb_org_id = @OrgId
                WHERE c.ativo = 1 AND co.ativo = 1 AND co.cod_diretoria <> 'BANCO TALENTOS'
                GROUP BY ooo.codigo_interno_colaborador_avaliado";

            var result = await connection.QueryAsync<(string CodigoInterno, DateTime? UltimaData)>(
                sql,
                new { OrgId = orgId }
            );

            return result.ToDictionary(r => r.CodigoInterno, r => r.UltimaData);
        }

        public async Task<Dictionary<string, DateTime?>> ObterTodasUltimasDataFeedbackAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT
                    f.codigo_interno_colaborador_avaliado AS CodigoInterno,
                    MAX(f.data_reuniao) AS UltimaData
                FROM tb_gest_desemp_feedback f
                INNER JOIN tb_colaborador c
                    ON f.codigo_interno_colaborador_avaliado = c.codigo_interno_colaborador
                INNER JOIN tb_colaborador_org co
                    ON c.codigo_interno_colaborador = co.codigo_interno_colaborador
                    AND co.tb_org_id = @OrgId
                WHERE c.ativo = 1 AND co.ativo = 1 AND co.cod_diretoria <> 'BANCO TALENTOS'
                GROUP BY f.codigo_interno_colaborador_avaliado";

            var result = await connection.QueryAsync<(string CodigoInterno, DateTime? UltimaData)>(
                sql,
                new { OrgId = orgId }
            );

            return result.ToDictionary(r => r.CodigoInterno, r => r.UltimaData);
        }

        public async Task<List<ColaboradorRHDTO>> ObterTodosColaboradoresComDetalhesAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT
                    tc.codigo_interno_colaborador AS CodigoInternoColaborador,
                    tc.nome_completo AS NomeCompleto,
                    tco.cargo AS Cargo,
                    CASE WHEN tco.ativo = 1 AND tc.ativo = 1 THEN 'Ativo' ELSE 'Inativo' END AS Status,
                    GROUP_CONCAT(tsuperior.nome_completo,';') AS NomeCompletoColaboradorSuperior
                FROM
                	tb_colaborador tc
                JOIN
                	tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                LEFT JOIN
                	tb_colaborador_hierarquia th ON tco.cod_colaborador_externo = th.cod_colaborador_externo AND th.tb_org_id = tco.tb_org_id
                LEFT JOIN
                	tb_colaborador_org tco_superior ON th.cod_colaborador_superior = tco_superior.cod_colaborador_externo AND tco_superior.tb_org_id = tco.tb_org_id AND tco_superior.ativo = 1
                LEFT JOIN
                	tb_colaborador tsuperior ON tco_superior.codigo_interno_colaborador = tsuperior.codigo_interno_colaborador
                WHERE
                	tc.ativo = 1 AND tco.ativo = 1 AND tco.tb_org_id = @OrgId AND tco.cod_diretoria <> 'BANCO TALENTOS'
                GROUP BY
                	tc.codigo_interno_colaborador
                ORDER BY
                	tc.nome_completo";

            var resultados = await connection.QueryAsync<ColaboradorRHTempDTO>(
                sql,
                new { OrgId = orgId }
            );

            var colaboradores = resultados.Select(r => new ColaboradorRHDTO
            {
                CodigoInternoColaborador = r.CodigoInternoColaborador,
                NomeCompleto = r.NomeCompleto,
                Cargo = r.Cargo,
                Status = r.Status,
                NomesColaboradoresSuperiores = string.IsNullOrEmpty(r.NomeCompletoColaboradorSuperior)
                    ? new List<string>()
                    : r.NomeCompletoColaboradorSuperior.Split(';').Where(n => !string.IsNullOrWhiteSpace(n)).ToList()
            }).ToList();

            return colaboradores;
        }

        private class ColaboradorRHTempDTO
        {
            public string CodigoInternoColaborador { get; set; }
            public string NomeCompleto { get; set; }
            public string Cargo { get; set; }
            public string Status { get; set; }
            public string NomeCompletoColaboradorSuperior { get; set; }
        }
    }
}
