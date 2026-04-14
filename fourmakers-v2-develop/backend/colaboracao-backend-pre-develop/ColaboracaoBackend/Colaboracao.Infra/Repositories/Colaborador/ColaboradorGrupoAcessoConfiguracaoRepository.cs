using Colaboracao.Core.Interfaces;
using Dapper;
using DataTransferObject.Domain.Colaborador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Colaborador
{
    public class ColaboradorGrupoAcessoConfiguracaoRepository : IColaboradorGrupoAcessoConfiguracaoRepository
    {
        private IDBConnection _dapperConnection;

        public ColaboradorGrupoAcessoConfiguracaoRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<List<ColaboradorGrupoAcessoConfiguracaoDTO>> ObterConfiguracaoPorOrgAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            string sql = @"
                            SELECT
                                tabela,
                                coluna,
                                condicao,
                                chave,
                                acao AS Acao,
                                tb_grupo_acesso_id AS GrupoAcessoId,
                                tb_org_id AS OrgId
                            FROM
                                tb_colaborador_grupo_acesso_configuracao
                            WHERE
                                tb_org_id = @OrgId;
                        ";

            var result = await connection.QueryAsync<ColaboradorGrupoAcessoConfiguracaoDTO>(sql, new { OrgId = orgId });

            return result.ToList(); 
        }


        public async Task<string> ObterValorColunaEAcaoAsync(string tabela, string coluna, string condicao, int tbOrgId, string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();

            string[] camposCondicao = condicao.Split(',');
                var where = new List<string>();

                foreach (var campo in camposCondicao)
                {
                    if (campo.Equals("tb_org_id", StringComparison.OrdinalIgnoreCase))
                        where.Add($"{campo} = @OrgId");
                    else if (campo.Equals("codigo_interno_colaborador", StringComparison.OrdinalIgnoreCase))
                        where.Add($"{campo} = @CodigoInternoColaborador");
                }

                string sql = $@"
                                SELECT 
                                    {coluna} as Valor
                                FROM
                                    {tabela}
                                WHERE
                                    {string.Join(" AND ", where)}
                                LIMIT 1;
                            ";

                var valor = await connection.QueryFirstOrDefaultAsync<string>(sql, new
                {
                    OrgId = tbOrgId,
                    CodigoInternoColaborador = codigoInternoColaborador
                });

                return valor;

        }
    }
}
