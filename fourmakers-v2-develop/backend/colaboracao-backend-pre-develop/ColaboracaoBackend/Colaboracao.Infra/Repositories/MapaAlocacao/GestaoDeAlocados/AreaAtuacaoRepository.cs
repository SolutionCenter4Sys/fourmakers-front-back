using Colaboracao.Core.Interfaces;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Dapper;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao.AreaAtuacao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.MapaAlocacao.GestaoDeAlocados
{
    public class AreaAtuacaoRepository : IAreaAtuacaoRepository
    {
        private readonly IDBConnection _dapperConnection;

        public AreaAtuacaoRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<IEnumerable<AreaAtuacaoBase>> ListarAreasAtuacaoAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT
                   id as Id,
                   descricao as Descricao,
                   tb_org_id as OrgId,
                   ativo as Ativo
                FROM
                    tb_area_atuacao
                WHERE
                    tb_org_id = @OrgId;
                ";

            // Executando a consulta com os parâmetros para paginação e busca
            var result = await connection.QueryAsync<AreaAtuacaoBase>(
                query,
                new
                {
                    OrgId = orgId
                }
            );

            return result;
        }

        public async Task<AreaAtuacaoResult> InserirAreaAtuacaoAsync(AreaAtuacaoInput input)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"  INSERT INTO tb_area_atuacao (id, descricao, tb_org_id, ativo)
                            VALUES (UUID(), @Descricao, @OrgId, 1);";

            var linhasInseridas = await connection.ExecuteAsync(
                query,
                new
                {
                    input.Descricao,
                    input.OrgId
                }
            );

            var result = await GetAreaAtuacaoPorDescricaoAsync(input.Descricao, input.OrgId);

            return result;
        }

        public async Task<AreaAtuacaoResult> GetAreaAtuacaoPorDescricaoAsync(string descricao, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                            SELECT
                                id AS Id,
                                descricao AS Descricao,
                                tb_org_id AS OrgId,
                                ativo AS Ativo
                            FROM
                                tb_area_atuacao
                            WHERE
                                tb_org_id = @OrgId
                                AND descricao LIKE @Descricao
                            LIMIT 1;
                        ";

            // Executando a consulta e buscando o resultado
            var result = await connection.QueryFirstOrDefaultAsync<AreaAtuacaoResult>(
                query,
                new
                {
                    OrgId = orgId,
                    Descricao = descricao
                }
            );

            return result;
        }
    }
}