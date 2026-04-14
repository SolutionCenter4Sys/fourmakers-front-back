using Colaboracao.Core.Interfaces;
using Core.Domain.GestaoPessoa.GestaoDesempenho.Gestor;
using Dapper;
using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Gestor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.GestaoPessoa.GestaoDesempenho.Gestor
{
    public class GestaoDesempenhoGestorRepository : IGestaoDesempenhoGestorRepository
    {
        private readonly IDBConnection _dapperConnection;

        public GestaoDesempenhoGestorRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<string> ObterCodigoExternoGestorAsync(string codigoInternoColaboradorGestor, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT cod_colaborador_externo
                FROM tb_colaborador_org
                WHERE codigo_interno_colaborador = @CodigoInternoColaboradorGestor
                    AND tb_org_id = @OrgId
                    AND ativo = 1
                LIMIT 1";

            var codExterno = await connection.QueryFirstOrDefaultAsync<string>(
                sql,
                new { CodigoInternoColaboradorGestor = codigoInternoColaboradorGestor, OrgId = orgId }
            );

            return codExterno;
        }

        public async Task<List<ColaboradorSubordinadoDTO>> ObterColaboradoresSubordinadosAsync(string codExternoGestor, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT
                    c.codigo_interno_colaborador AS CodigoInternoColaborador,
                    c.nome_completo AS NomeCompleto,
                    co.cod_colaborador_externo AS CodColaboradorExterno
                FROM tb_colaborador_hierarquia ch
                INNER JOIN tb_colaborador_org co
                    ON ch.cod_colaborador_externo = co.cod_colaborador_externo
                    AND ch.tb_org_id = co.tb_org_id
                INNER JOIN tb_colaborador c
                    ON co.codigo_interno_colaborador = c.codigo_interno_colaborador
                WHERE ch.cod_colaborador_superior = @CodExternoGestor
                    AND ch.tb_org_id = @OrgId
                    AND co.ativo = 1
                    AND c.ativo = 1";

            var colaboradores = await connection.QueryAsync<ColaboradorSubordinadoDTO>(
                sql,
                new { CodExternoGestor = codExternoGestor, OrgId = orgId }
            );

            return colaboradores.ToList();
        }

        public async Task<Dictionary<string, DateTime?>> ObterUltimasDataOneOnOneAsync(List<string> codigosInternosColaboradores)
        {
            var connection = _dapperConnection.GetConnection();

            if (codigosInternosColaboradores == null || !codigosInternosColaboradores.Any())
            {
                return new Dictionary<string, DateTime?>();
            }

            var sql = @"
                SELECT
                    codigo_interno_colaborador_avaliado AS CodigoInternoColaborador,
                    MAX(data_reuniao) AS DataReuniao
                FROM tb_gest_desemp_one_on_one
                WHERE codigo_interno_colaborador_avaliado IN @CodigosInternos
                GROUP BY codigo_interno_colaborador_avaliado";

            var resultado = await connection.QueryAsync<(string CodigoInternoColaborador, DateTime? DataReuniao)>(
                sql,
                new { CodigosInternos = codigosInternosColaboradores }
            );

            return resultado.ToDictionary(x => x.CodigoInternoColaborador, x => x.DataReuniao);
        }

        public async Task<Dictionary<string, DateTime?>> ObterUltimasDataFeedbackAsync(List<string> codigosInternosColaboradores)
        {
            var connection = _dapperConnection.GetConnection();

            if (codigosInternosColaboradores == null || !codigosInternosColaboradores.Any())
            {
                return new Dictionary<string, DateTime?>();
            }

            var sql = @"
                SELECT
                    codigo_interno_colaborador_avaliado AS CodigoInternoColaborador,
                    MAX(data_reuniao) AS DataReuniao
                FROM tb_gest_desemp_feedback
                WHERE codigo_interno_colaborador_avaliado IN @CodigosInternos
                GROUP BY codigo_interno_colaborador_avaliado";

            var resultado = await connection.QueryAsync<(string CodigoInternoColaborador, DateTime? DataReuniao)>(
                sql,
                new { CodigosInternos = codigosInternosColaboradores }
            );

            return resultado.ToDictionary(x => x.CodigoInternoColaborador, x => x.DataReuniao);
        }

        public async Task<List<OneOnOneRegistroCriticoDTO>> ObterRegistrosCriticosAsync(string codExternoGestor, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT
                    c.nome_completo AS NomeCompleto,
                    ooo.data_reuniao AS DataReuniao,
                    ooo.descricao_anotacoes AS DescricaoAnotacoes
                FROM tb_gest_desemp_one_on_one ooo
                INNER JOIN tb_colaborador c
                    ON ooo.codigo_interno_colaborador_avaliado = c.codigo_interno_colaborador
                INNER JOIN tb_colaborador_org co
                    ON c.codigo_interno_colaborador = co.codigo_interno_colaborador
                    AND co.tb_org_id = @OrgId
                    AND co.ativo = 1
                INNER JOIN tb_colaborador_hierarquia ch
                    ON co.cod_colaborador_externo = ch.cod_colaborador_externo
                    AND ch.tb_org_id = @OrgId
                WHERE ooo.registro_critico = 1
                    AND ch.cod_colaborador_superior = @CodExternoGestor
                ORDER BY ooo.data_reuniao DESC
                LIMIT 10";

            var registros = await connection.QueryAsync<OneOnOneRegistroCriticoDTO>(
                sql,
                new { CodExternoGestor = codExternoGestor, OrgId = orgId }
            );

            return registros.ToList();
        }

        public async Task<List<ColaboradorDetalhesDTO>> ObterColaboradoresSubordinadosComDetalhesAsync(string codExternoGestor, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT
                    c.codigo_interno_colaborador AS CodigoInternoColaborador,
                    co.cod_colaborador_externo AS CodColaboradorExterno,
                    c.nome_completo AS NomeCompleto,
                    co.cargo AS Cargo,
                    CASE WHEN co.ativo = 1 AND c.ativo = 1 THEN 'Ativo' ELSE 'Inativo' END AS Status
                FROM tb_colaborador_hierarquia ch
                INNER JOIN tb_colaborador_org co
                    ON ch.cod_colaborador_externo = co.cod_colaborador_externo
                    AND ch.tb_org_id = co.tb_org_id
                INNER JOIN tb_colaborador c
                    ON co.codigo_interno_colaborador = c.codigo_interno_colaborador
                WHERE ch.cod_colaborador_superior = @CodExternoGestor
                    AND ch.tb_org_id = @OrgId
                    AND co.ativo = 1
                    AND c.ativo = 1";

            var colaboradores = await connection.QueryAsync<ColaboradorDetalhesDTO>(
                sql,
                new { CodExternoGestor = codExternoGestor, OrgId = orgId }
            );

            return colaboradores.ToList();
        }

        public async Task InserirFeedbackAsync(string id, string codigoInternoColaboradorSuperior, InserirFeedbackRequestDTO request)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                INSERT INTO tb_gest_desemp_feedback
                (
                    id,
                    codigo_interno_colaborador_superior,
                    codigo_interno_colaborador_avaliado,
                    data_reuniao,
                    data_criacao,
                    data_atualizacao,
                    descricao_continuar,
                    descricao_comecar,
                    descricao_parar,
                    descricao_observacoes_gerais,
                    visualizado_pelo_colaborador,
                    data_visualizado_colaborador
                )
                VALUES
                (
                    @Id,
                    @CodigoInternoColaboradorSuperior,
                    @CodigoInternoColaboradorAvaliado,
                    @DataReuniao,
                    @DataCriacao,
                    @DataAtualizacao,
                    @DescricaoContinuar,
                    @DescricaoComecar,
                    @DescricaoParar,
                    @DescricaoObservacoesGerais,
                    @VisualizadoPeloColaborador,
                    @DataVisualizadoColaborador
                )";

            await connection.ExecuteAsync(sql, new
            {
                Id = id,
                CodigoInternoColaboradorSuperior = codigoInternoColaboradorSuperior,
                CodigoInternoColaboradorAvaliado = request.CodigoInternoColaboradorAvaliado,
                DataReuniao = request.DataReuniao,
                DataCriacao = DateTime.Now,
                DataAtualizacao = DateTime.Now,
                DescricaoContinuar = request.DescricaoContinuar,
                DescricaoComecar = request.DescricaoComecar,
                DescricaoParar = request.DescricaoParar,
                DescricaoObservacoesGerais = request.DescricaoObservacoesGerais,
                VisualizadoPeloColaborador = false,
                DataVisualizadoColaborador = (DateTime?)null
            });
        }

        public async Task<string> InserirOneOnOneAsync(string codigoInternoColaboradorSuperior, InserirOneOnOneRequestDTO request)
        {
            var connection = _dapperConnection.GetConnection();

            var oneOnOneId = Guid.NewGuid().ToString();

            var sql = @"
                INSERT INTO tb_gest_desemp_one_on_one
                (
                    id,
                    codigo_interno_colaborador_superior,
                    codigo_interno_colaborador_avaliado,
                    data_reuniao,
                    data_criacao,
                    descricao_anotacoes,
                    registro_critico,
                    visualizado_pelo_colaborador,
                    data_visualizado_colaborador
                )
                VALUES
                (
                    @Id,
                    @CodigoInternoColaboradorSuperior,
                    @CodigoInternoColaboradorAvaliado,
                    @DataReuniao,
                    @DataCriacao,
                    @DescricaoAnotacoes,
                    @RegistroCritico,
                    @VisualizadoPeloColaborador,
                    @DataVisualizadoColaborador
                )";

            await connection.ExecuteAsync(sql, new
            {
                Id = oneOnOneId,
                CodigoInternoColaboradorSuperior = codigoInternoColaboradorSuperior,
                CodigoInternoColaboradorAvaliado = request.CodigoInternoColaboradorAvaliado,
                DataReuniao = request.DataReuniao,
                DataCriacao = DateTime.Now,
                DescricaoAnotacoes = request.DescricaoAnotacoes,
                RegistroCritico = request.RegistroCritico,
                VisualizadoPeloColaborador = false,
                DataVisualizadoColaborador = (DateTime?)null
            });

            return oneOnOneId;
        }

        public async Task VincularPautasSugeridasAoOneOnOneAsync(string oneOnOneId, List<string> pautasSugeridasIds)
        {
            if (pautasSugeridasIds == null || !pautasSugeridasIds.Any())
            {
                return;
            }

            var connection = _dapperConnection.GetConnection();

            var sql = @"
                INSERT INTO tb_gest_desemp_one_on_one_pauta_sugerida
                (tb_gest_desemp_one_on_one_id, tb_gest_desemp_pauta_sugerida_id)
                VALUES (@OneOnOneId, @PautaSugeridaId)";

            foreach (var pautaId in pautasSugeridasIds)
            {
                await connection.ExecuteAsync(sql, new
                {
                    OneOnOneId = oneOnOneId,
                    PautaSugeridaId = pautaId
                });
            }
        }

        public async Task<DashboardColaboradorDTO> ObterDashboardColaboradorAsync(string codigoInternoColaboradorAvaliado, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT
                    c.codigo_interno_colaborador AS CodigoInternoColaboradorAvaliado,
                    co.cod_colaborador_externo AS CodigoColaboradorExterno,
                    c.nome_completo AS NomeCompletoColaboradorAvaliado,
                    CASE WHEN co.ativo = 1 AND c.ativo = 1 THEN 'Ativo' ELSE 'Inativo' END AS Status,
                    co.cargo AS Cargo,
                    u.email AS Email,
                    c.contato_principal AS Telefone,
                    c.data_nascimento AS DataNascimento,
                    co.data_admissao AS DataAdmissao,
                    IF(co.base_hora_mes IS NOT NULL, CONCAT(CAST(co.base_hora_mes AS CHAR), 'h/mês'), NULL) AS JornadaTrabalho,
                    co.modelo_trabalho AS TipoTrabalho,
                    tmco.descricao AS RegimeTrabalho,
                    co.modelo_contratacao AS ModalidadeContratacao
                FROM tb_colaborador c
                INNER JOIN tb_colaborador_org co
                    ON c.codigo_interno_colaborador = co.codigo_interno_colaborador
                    AND co.tb_org_id = @OrgId
                LEFT JOIN tb_usuario u
                    ON c.codigo_interno_colaborador = u.codigo_interno_colaborador
                    AND u.tb_org_id = @OrgId
                LEFT JOIN tb_modelo_contratacao_org tmco
                    ON tmco.codigo_modelo_contratacao = co.codigo_modelo_contratacao
                    AND tmco.tb_org_id = co.tb_org_id
                WHERE c.codigo_interno_colaborador = @CodigoInternoColaboradorAvaliado
                LIMIT 1";

            var dashboard = await connection.QueryFirstOrDefaultAsync<DashboardColaboradorDTO>(
                sql,
                new { CodigoInternoColaboradorAvaliado = codigoInternoColaboradorAvaliado, OrgId = orgId }
            );

            if (dashboard != null && dashboard.DataAdmissao.HasValue)
            {
                var tempoTotal = DateTime.Now - dashboard.DataAdmissao.Value;
                var anos = (int)(tempoTotal.TotalDays / 365.25);
                var meses = (int)((tempoTotal.TotalDays % 365.25) / 30.44);

                if (anos > 0 && meses > 0)
                {
                    dashboard.TempoCasa = $"{anos} ano{(anos > 1 ? "s" : "")} e {meses} m{(meses > 1 ? "eses" : "ês")}";
                }
                else if (anos > 0)
                {
                    dashboard.TempoCasa = $"{anos} ano{(anos > 1 ? "s" : "")}";
                }
                else if (meses > 0)
                {
                    dashboard.TempoCasa = $"{meses} m{(meses > 1 ? "eses" : "ês")}";
                }
                else
                {
                    var dias = (int)tempoTotal.TotalDays;
                    dashboard.TempoCasa = $"{dias} dia{(dias > 1 ? "s" : "")}";
                }
            }

            return dashboard;
        }

        public async Task<List<FeedbackDetalheDTO>> ObterFeedbacksColaboradorAsync(string codigoInternoColaboradorAvaliado)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT
                    f.id AS Id,
                    f.codigo_interno_colaborador_superior AS CodigoInternoColaboradorSuperior,
                    c.nome_completo AS NomeCompletoColaboradorSuperior,
                    f.data_reuniao AS DataReuniao,
                    f.descricao_continuar AS DescricaoContinuar,
                    f.descricao_comecar AS DescricaoComecar,
                    f.descricao_parar AS DescricaoParar,
                    f.descricao_observacoes_gerais AS DescricaoObservacoesGerais,
                    f.visualizado_pelo_colaborador AS VisualizadoPeloColaborador,
                    f.data_visualizado_colaborador AS DataVisualizadoColaborador
                FROM tb_gest_desemp_feedback f
                INNER JOIN tb_colaborador c
                    ON f.codigo_interno_colaborador_superior = c.codigo_interno_colaborador
                WHERE f.codigo_interno_colaborador_avaliado = @CodigoInternoColaboradorAvaliado
                ORDER BY f.data_reuniao DESC";

            var feedbacks = await connection.QueryAsync<FeedbackDetalheDTO>(
                sql,
                new { CodigoInternoColaboradorAvaliado = codigoInternoColaboradorAvaliado }
            );

            return feedbacks.ToList();
        }

        public async Task<List<OneOnOneDetalheDTO>> ObterOneOnOnesColaboradorAsync(string codigoInternoColaboradorAvaliado)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT
                    ooo.id AS Id,
                    ooo.codigo_interno_colaborador_superior AS CodigoInternoColaboradorSuperior,
                    c.nome_completo AS NomeCompletoColaboradorSuperior,
                    ooo.data_reuniao AS DataReuniao,
                    ooo.descricao_anotacoes AS DescricaoAnotacoes,
                    ooo.registro_critico AS RegistroCritico,
                    ooo.visualizado_pelo_colaborador AS VisualizadoPeloColaborador,
                    ooo.data_visualizado_colaborador AS DataVisualizadoColaborador
                FROM tb_gest_desemp_one_on_one ooo
                INNER JOIN tb_colaborador c
                    ON ooo.codigo_interno_colaborador_superior = c.codigo_interno_colaborador
                WHERE ooo.codigo_interno_colaborador_avaliado = @CodigoInternoColaboradorAvaliado
                ORDER BY ooo.data_reuniao DESC";

            var oneOnOnes = await connection.QueryAsync<OneOnOneDetalheDTO>(
                sql,
                new { CodigoInternoColaboradorAvaliado = codigoInternoColaboradorAvaliado }
            );

            return oneOnOnes.ToList();
        }

        public async Task<InserirPautaSugeridaGestorResponseDTO> UpsertPautaSugeridaGestorAsync(
    string codigoInternoColaboradorSuperior,
    InserirPautaSugeridaGestorRequestDTO request)
        {
            var connection = _dapperConnection.GetConnection();

            var pautaId = Guid.NewGuid().ToString();

            var sqlUpsert = @"
                                INSERT INTO tb_gest_desemp_pauta_sugerida
                                (
                                    id,
                                    codigo_interno_colaborador_criacao,
                                    codigo_interno_colaborador_superior,
                                    codigo_interno_colaborador_avaliado,
                                    data_criacao,
                                    data_atualizacao,
                                    descricao_pauta_sugerida,
                                    tipo_origem
                                )
                                VALUES
                                (
                                    @Id,
                                    @CodigoInternoColaboradorCriacao,
                                    @CodigoInternoColaboradorSuperior,
                                    @CodigoInternoColaboradorAvaliado,
                                    @DataCriacao,
                                    @DataAtualizacao,
                                    @DescricaoPautaSugerida,
                                    'GESTOR'
                                )
                                ON DUPLICATE KEY UPDATE
                                    descricao_pauta_sugerida = @DescricaoPautaSugerida,
                                    data_atualizacao = @DataAtualizacao;
                            ";
                        
            await connection.ExecuteAsync(sqlUpsert, new
            {
                Id = pautaId,
                CodigoInternoColaboradorCriacao = codigoInternoColaboradorSuperior,
                CodigoInternoColaboradorSuperior = codigoInternoColaboradorSuperior,
                CodigoInternoColaboradorAvaliado = request.CodigoInternoColaboradorAvaliado,
                DataCriacao = DateTime.Now,
                DataAtualizacao = DateTime.Now,
                DescricaoPautaSugerida = request.DescricaoPautaSugerida
            });

            var sqlGetId = @"
                                SELECT
                                    id
                                FROM
                                    tb_gest_desemp_pauta_sugerida
                                WHERE
                                    codigo_interno_colaborador_superior = @CodigoInternoColaboradorSuperior
                                    AND codigo_interno_colaborador_avaliado = @CodigoInternoColaboradorAvaliado
                                    AND tipo_origem = 'GESTOR';
                            ";

            var id = await connection.ExecuteScalarAsync<string>(sqlGetId, new
            {
                CodigoInternoColaboradorSuperior = codigoInternoColaboradorSuperior,
                CodigoInternoColaboradorAvaliado = request.CodigoInternoColaboradorAvaliado
            });

            return new InserirPautaSugeridaGestorResponseDTO
            {
                Id = id,
                CodigoInternoColaboradorSuperior = codigoInternoColaboradorSuperior,
                CodigoInternoColaboradorAvaliado = request.CodigoInternoColaboradorAvaliado,
                DescricaoPautaSugerida = request.DescricaoPautaSugerida
            };
        }

        public async Task<bool> AtualizarRegistroCriticoOneOnOneAsync(string oneOnOneId, bool registroCritico)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                UPDATE
                    tb_gest_desemp_one_on_one
                SET 
                    registro_critico = @RegistroCritico
                WHERE
                    id = @OneOnOneId";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                OneOnOneId = oneOnOneId,
                RegistroCritico = registroCritico ? 1 : 0
            });

            return rowsAffected > 0;
        }

    }
}
