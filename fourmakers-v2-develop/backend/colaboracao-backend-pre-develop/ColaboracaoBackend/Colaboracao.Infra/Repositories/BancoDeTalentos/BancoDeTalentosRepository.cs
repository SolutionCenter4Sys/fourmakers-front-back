using Colaboracao.Core.Interfaces;
using Core.DomainModel.BancoDeTalentos;
using Dapper;
using DataTransferObject.Domain.BancoDeTalentos;
using DataTransferObject.Domain.Vaga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.BancoDeTalentos
{
    public class BancoDeTalentosRepository : IBancoDeTalentosRepository
    {
        private readonly IDBConnection _dapperConnection;

        public BancoDeTalentosRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<string> InserirBancoDeTalentos(string codInternoColaborador, int orgId, string tipoCadastro)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                var query = @"
                    INSERT INTO tb_banco_talentos
                    (codigo_interno_colaborador, tb_org_id, tipo_cadastro)
                    VALUES (@CodigoInternoColaborador, @OrgId, @TipoCadastro);
                ";
                var parametros = new
                {
                    CodigoInternoColaborador = codInternoColaborador,
                    OrgId = orgId,
                    TipoCadastro = tipoCadastro,
                };

                var result = await connection.ExecuteScalarAsync<string>(query, parametros);

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task RemoverBancoDeTalentos(string codInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                var query = @"
                    DELETE FROM
                        tb_banco_de_talentos tbdt
                    WHERE
                        tbdt.codigo_interno_colaborador = @CodigoInternoColaborador;
                ";
                var parametros = new
                {
                    CodigoInternoColaborador = codInternoColaborador,
                };

                await connection.ExecuteAsync(query, parametros);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BancoDeTalentoDTO> BuscarBancoDeTalentosPorColaborador(string codInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT
                    tbdt.codigo_interno_colaborador AS CodigoInternoColaborador,
                    tbdt.tipo_cadastro AS TipoCadastro
                FROM
                    tb_banco_talentos tbdt
                WHERE
                    tbdt.codigo_interno_colaborador = @CodigoInternoColaborador
            ";
            var parametros = new
            {
                CodigoInternoColaborador = codInternoColaborador,
            };

            var result = await connection.QueryAsync<BancoDeTalentoDTO>(query, parametros);
            return result.FirstOrDefault();
        }

        public async Task<string> InserirBancoDeTalentosIdExterno(string codInternoColaborador, int orgId, string tipoCadastro, string idExterno, string codigoInternoColaboradorCadastrante, DateTime utcNow, int formaCadastro)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                    INSERT INTO tb_banco_talentos
                    (codigo_interno_colaborador, tb_org_id, tipo_cadastro, id_externo, tb_colaborador_codigo_interno_colaborador_cadastrante, data_criacao, forma_cadastro)
                    VALUES (@CodigoInternoColaborador, @OrgId, @TipoCadastro, @IdExterno, @CodigoInternoColaboradorCadastrante, @DataCriacao, @FormaCadastro);
                ";
            var parametros = new
            {
                CodigoInternoColaborador = codInternoColaborador,
                OrgId = orgId,
                TipoCadastro = tipoCadastro,
                IdExterno = idExterno,
                CodigoInternoColaboradorCadastrante = codigoInternoColaboradorCadastrante,
                DataCriacao = utcNow,
                FormaCadastro = formaCadastro
            };

            var result = await connection.ExecuteScalarAsync<string>(query, parametros);

            return result;
        }

        public async Task<string> BuscarNomeCadastrante(string codInternoColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT
                    tc.nome_completo  AS QuemCadastrou
                FROM
                    tb_banco_talentos tbdt
                INNER JOIN tb_colaborador tc
	                ON tc.codigo_interno_colaborador = tbdt.tb_colaborador_codigo_interno_colaborador_cadastrante
                WHERE
                    tbdt.codigo_interno_colaborador = @CodigoInternoColaborador
                    AND tbdt.tb_org_id = @OrgId
            ";
            var parametros = new
            {
                CodigoInternoColaborador = codInternoColaborador,
                OrgId = orgId
            };

            return await connection.QueryFirstOrDefaultAsync<string>(query, parametros);
        }

        public async Task<IEnumerable<BuscarPessoasQueEuCadastrei>> BuscarPessoasQueEuCadastrei(string codigoInternoColaboradorCadastrante, int orgId, string busca, int cursor, int limite)
        {
            var connection = _dapperConnection.GetConnection();

            if (limite <= 0)
                return Enumerable.Empty<BuscarPessoasQueEuCadastrei>();

            var take = Math.Min(limite, 100); 
            var skip = Math.Max(cursor, 0);
            var hasBusca = !string.IsNullOrWhiteSpace(busca);

            static string EscapeLike(string s) =>
                s.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");

            var buscaLike = hasBusca ? $"%{EscapeLike(busca.Trim())}%" : null;

            var sb = new System.Text.StringBuilder("""

                                                           SELECT 
                                                               tc.nome_completo AS Nome,
                                                               tbt.codigo_interno_colaborador AS CodigoInternoColaborador,
                                                               cad.nome_completo AS NomeCadastrante,
                                                               tbt.data_criacao AS DataDoCadastro
                                                           FROM tb_banco_talentos tbt
                                                           INNER JOIN tb_colaborador tc 
                                                               ON tc.codigo_interno_colaborador = tbt.codigo_interno_colaborador
                                                           INNER JOIN tb_colaborador cad
                                                                ON cad.codigo_interno_colaborador = tbt.tb_colaborador_codigo_interno_colaborador_cadastrante                                           
                                                           WHERE 
                                                               tbt.tb_colaborador_codigo_interno_colaborador_cadastrante = @CodigoInternoColaboradorCadastrante
                                                               AND tbt.tb_org_id = @OrgId
                                                       
                                                   """);

            if (hasBusca)
            {
                sb.Append("""

                                      AND (
                                          tc.nome_completo LIKE @BuscaLike ESCAPE '\\'
                                          OR tbt.codigo_interno_colaborador LIKE @BuscaLike ESCAPE '\\'
                                          OR tc.url_linkedin LIKE @BuscaLike ESCAPE '\\'
                                      )
                                  
                          """);
            }

            sb.Append($"""

                               ORDER BY 
                                   tbt.data_criacao DESC,
                                   tbt.codigo_interno_colaborador ASC
                               LIMIT {take} OFFSET {skip};
                           
                       """);

            var sql = sb.ToString();

            var parametros = new DynamicParameters();
            parametros.Add("CodigoInternoColaboradorCadastrante", codigoInternoColaboradorCadastrante);
            parametros.Add("OrgId", orgId);
            if (hasBusca) parametros.Add("BuscaLike", buscaLike);

            var result = await connection.QueryAsync<BuscarPessoasQueEuCadastrei>(sql, parametros);
            return result;
        }

        public async Task<IEnumerable<RecrutadoresQuantidadeCadastroBancoTalentos>> BuscarRecrutadoresQuantidadeCadastrada()
        {
            var sql = @"
                                SELECT 
	                                tbt.tb_colaborador_codigo_interno_colaborador_cadastrante  as codigoInternoColaborador,
                                    tc.nome_completo  AS nomeColaborador,
                                    COUNT(*) AS totalTalentosCadastrados
                                FROM 
                                    tb_banco_talentos tbt
                                INNER JOIN 
                                    tb_colaborador tc 
                                    ON tc.codigo_interno_colaborador = tbt.tb_colaborador_codigo_interno_colaborador_cadastrante
                                GROUP BY 
	                                tbt.tb_colaborador_codigo_interno_colaborador_cadastrante,
                                    tc.nome_completo;
                            ";

            return await _dapperConnection.GetConnection().QueryAsync<RecrutadoresQuantidadeCadastroBancoTalentos>(sql);
        }

        public async Task<IEnumerable<BuscarPessoasQueEuCadastrei>> BuscarBancoTalentosPorOrg(int orgId, string busca, int cursor, int limite)
        {
            var connection = _dapperConnection.GetConnection();

            var take = Math.Min(limite, 100); 
            var skip = Math.Max(cursor, 0);
            var hasBusca = !string.IsNullOrWhiteSpace(busca);

            static string EscapeLike(string s) =>
                s.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");

            var buscaLike = hasBusca ? $"%{EscapeLike(busca.Trim())}%" : null;

            var sb = new System.Text.StringBuilder(@"
                SELECT
                    tc.nome_completo AS Nome,
                    tbdt.codigo_interno_colaborador AS CodigoInternoColaborador,
                    cad.nome_completo AS NomeCadastrante,
                    tbdt.data_criacao AS DataDoCadastro,
                    tbdt.tipo_cadastro AS TipoCadastro
                FROM
                    tb_banco_talentos tbdt
                INNER JOIN tb_colaborador tc 
                    ON tc.codigo_interno_colaborador = tbdt.codigo_interno_colaborador
                LEFT JOIN tb_colaborador cad
                    ON cad.codigo_interno_colaborador = tbdt.tb_colaborador_codigo_interno_colaborador_cadastrante
                WHERE
                    tbdt.tb_org_id = @orgId
            ");

            if (hasBusca)
            {
                sb.Append(@"
                    AND (
                        tc.nome_completo LIKE @BuscaLike ESCAPE '\\'
                        OR tbdt.codigo_interno_colaborador LIKE @BuscaLike ESCAPE '\\'
                        OR tc.url_linkedin LIKE @BuscaLike ESCAPE '\\'
                    )
                ");
            }

            sb.Append($@"
                ORDER BY 
                    tbdt.data_criacao DESC,
                    tbdt.codigo_interno_colaborador ASC
                LIMIT {take} OFFSET {skip};
            ");

            var sql = sb.ToString();

            var parametros = new DynamicParameters();
            parametros.Add("orgId", orgId);
            if (hasBusca) parametros.Add("BuscaLike", buscaLike);

            return await connection.QueryAsync<BuscarPessoasQueEuCadastrei>(sql, parametros);
        }

        public async Task<IEnumerable<BuscarPessoasCadastradasPorOrgResult>> BuscarPessoasCadastradasPorOrg(int orgId, string busca, List<int> statusVaga, List<int> statusCandidatura, string dataInicio, string dataFim, int cursor, int limite)
        {
            var connection = _dapperConnection.GetConnection();

            if (limite <= 0)
                return Enumerable.Empty<BuscarPessoasCadastradasPorOrgResult>();

            var take = limite;
            var skip = cursor;
            var hasBusca = !string.IsNullOrWhiteSpace(busca);
            var hasStatusVaga = statusVaga != null && statusVaga.Any();
            var hasStatusCandidatura = statusCandidatura != null && statusCandidatura.Any();
            var hasDataInicio = !string.IsNullOrWhiteSpace(dataInicio);
            var hasDataFim = !string.IsNullOrWhiteSpace(dataFim);

            static string EscapeLike(string s) =>
                s.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");

            var buscaLike = hasBusca ? $"%{EscapeLike(busca.Trim())}%" : null;

            var sb = new System.Text.StringBuilder(@"
                SELECT DISTINCT
                    tc.nome_completo AS Nome,
                    tc.codigo_interno_colaborador AS CodigoInternoColaborador,
                    cad.nome_completo AS NomeCadastrante,
                    tc.data_criacao AS DataDoCadastro,
                    tu.email AS EmailUsuario
                FROM
                    tb_candidato_vaga tcv
                INNER JOIN tb_vaga tv 
                    ON tv.id = tcv.tb_vaga_id
                INNER JOIN tb_colaborador tc 
                    ON tc.codigo_interno_colaborador = tcv.tb_colaborador_codigo_interno_colaborador
                LEFT JOIN tb_banco_talentos tbt 
                    ON tbt.codigo_interno_colaborador = tc.codigo_interno_colaborador
                LEFT JOIN tb_colaborador cad 
                    ON cad.codigo_interno_colaborador = tbt.tb_colaborador_codigo_interno_colaborador_cadastrante
                LEFT JOIN tb_usuario tu 
                    ON tu.codigo_interno_colaborador = tc.codigo_interno_colaborador
                    AND tu.tb_org_id = @orgId
                WHERE 1=1
                AND tcv.ativo = 1
                AND tcv.tb_org_id = @orgId
            ");

            var parametros = new DynamicParameters();
            parametros.Add("orgId", orgId);

            // Filtro de status da vaga
            if (hasStatusVaga)
            {
                sb.Append(@"
                    AND tv.tb_status_vaga_cod IN @StatusVaga
                ");
                parametros.Add("StatusVaga", statusVaga);
            }

            // Filtro de status da candidatura
            if (hasStatusCandidatura)
            {
                sb.Append(@"
                    AND tcv.tb_candidato_status_id IN @StatusCandidatura
                ");
                parametros.Add("StatusCandidatura", statusCandidatura);
            }

            // Filtro de data início
            if (hasDataInicio)
            {
                sb.Append(@"
                    AND tcv.data_criacao >= @DataInicio
                ");
                parametros.Add("DataInicio", dataInicio);
            }

            // Filtro de data fim
            if (hasDataFim)
            {
                sb.Append(@"
                    AND tcv.data_criacao <= @DataFim
                ");
                parametros.Add("DataFim", dataFim);
            }

            // Filtro de busca
            if (hasBusca)
            {
                sb.Append(@"
                    AND (
                        tc.nome_completo LIKE @BuscaLike ESCAPE '\\'
                        OR tbt.codigo_interno_colaborador LIKE @BuscaLike ESCAPE '\\'
                        OR tu.email LIKE @BuscaLike ESCAPE '\\'
                        OR tv.titulo LIKE @BuscaLike ESCAPE '\\'
                        OR tv.codigo LIKE @BuscaLike ESCAPE '\\'
                    )
                ");
                parametros.Add("BuscaLike", buscaLike);
            }

            sb.Append($@"
                ORDER BY 
                    tcv.data_criacao DESC,
                    tbt.codigo_interno_colaborador DESC
                LIMIT {take} OFFSET {skip};
            ");

            var sql = sb.ToString();

            var pessoas = (await connection.QueryAsync<BuscarPessoasCadastradasPorOrgResult>(sql, parametros)).ToList();

            return pessoas;
        }
    }
}