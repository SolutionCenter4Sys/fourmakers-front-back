using Colaboracao.Core.Interfaces;
using Core.Domain.GestaoPessoa.GestaoDesempenho.Colaborador;
using Dapper;
using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Colaborador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.GestaoPessoa.GestaoDesempenho.Colaborador
{
    public class GestaoDesempenhoColaboradorRepository : IGestaoDesempenhoColaboradorRepository
    {
        private readonly IDBConnection _dapperConnection;

        public GestaoDesempenhoColaboradorRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<List<ColaboradorFeedbackDetalheDTO>> ObterMeusFeedbacksAsync(string codigoInternoColaborador)
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
                WHERE f.codigo_interno_colaborador_avaliado = @CodigoInternoColaborador
                ORDER BY f.data_reuniao DESC";

            var feedbacks = await connection.QueryAsync<ColaboradorFeedbackDetalheDTO>(
                sql,
                new { CodigoInternoColaborador = codigoInternoColaborador }
            );

            return feedbacks.ToList();
        }

        public async Task<List<ColaboradorOneOnOneDetalheDTO>> ObterMeusOneOnOnesAsync(string codigoInternoColaborador)
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
                WHERE ooo.codigo_interno_colaborador_avaliado = @CodigoInternoColaborador
                ORDER BY ooo.data_reuniao DESC";

            var oneOnOnes = await connection.QueryAsync<ColaboradorOneOnOneDetalheDTO>(
                sql,
                new { CodigoInternoColaborador = codigoInternoColaborador }
            );

            return oneOnOnes.ToList();
        }

        public async Task<List<ColaboradorPautaSugeridaDTO>> ObterPautasSugeridasPorColaboradorAvalidadoAsync(string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT
                    ps.id AS Id,
                    ps.descricao_pauta_sugerida AS DescricaoPautaSugerida,
                    ps.data_criacao AS DataCriacao,
                    ps.codigo_interno_colaborador_criacao AS CodigoInternoColaboradorCriacao,
                    c.nome_completo AS NomeCompletoColaboradorCriacao,
                    ps.tipo_origem as TipoOrigem
                FROM
                    tb_gest_desemp_pauta_sugerida ps
                JOIN
                    tb_colaborador c ON ps.codigo_interno_colaborador_criacao = c.codigo_interno_colaborador
                WHERE
                    ps.codigo_interno_colaborador_avaliado = @CodigoInternoColaborador
                ORDER BY
                    ps.data_criacao DESC";

            var pautas = await connection.QueryAsync<ColaboradorPautaSugeridaDTO>(
                sql,
                new { CodigoInternoColaborador = codigoInternoColaborador }
            );

            return pautas.ToList();
        }

        public async Task<List<RegistroCriticoDTO>> ObterMeusRegistrosCriticosAsync(string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT
                    ooo.data_reuniao AS DataReuniao,
                    ooo.descricao_anotacoes AS DescricaoAnotacoes
                FROM tb_gest_desemp_one_on_one ooo
                WHERE ooo.codigo_interno_colaborador_avaliado = @CodigoInternoColaborador
                    AND ooo.registro_critico = 1
                ORDER BY ooo.data_reuniao DESC";

            var registros = await connection.QueryAsync<RegistroCriticoDTO>(
                sql,
                new { CodigoInternoColaborador = codigoInternoColaborador }
            );

            return registros.ToList();
        }

        public async Task<bool> InserirVisualizacaoFeedbackAsync(string feedbackId, string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                UPDATE tb_gest_desemp_feedback
                SET visualizado_pelo_colaborador = 1,
                    data_visualizado_colaborador = @DataVisualizacao
                WHERE id = @FeedbackId
                    AND codigo_interno_colaborador_avaliado = @CodigoInternoColaborador";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                FeedbackId = feedbackId,
                CodigoInternoColaborador = codigoInternoColaborador,
                DataVisualizacao = DateTime.Now
            });

            return rowsAffected > 0;
        }

        public async Task<bool> InserirVisualizacaoOneOnOneAsync(string oneOnOneId, string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                UPDATE tb_gest_desemp_one_on_one
                SET visualizado_pelo_colaborador = 1,
                    data_visualizado_colaborador = @DataVisualizacao
                WHERE id = @OneOnOneId
                    AND codigo_interno_colaborador_avaliado = @CodigoInternoColaborador";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                OneOnOneId = oneOnOneId,
                CodigoInternoColaborador = codigoInternoColaborador,
                DataVisualizacao = DateTime.Now
            });

            return rowsAffected > 0;
        }

        public async Task<InserirPautaSugeridaColaboradorResponseDTO> UpsertPautaSugeridaColaboradorAsync(
    string codigoInternoColaboradorAvaliado,
    InserirPautaSugeridaColaboradorRequestDTO request)
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
            'SUBORDINADO'
        )
        ON DUPLICATE KEY UPDATE
            descricao_pauta_sugerida = @DescricaoPautaSugerida,
            data_atualizacao = @DataAtualizacao;
    ";

            await connection.ExecuteAsync(sqlUpsert, new
            {
                Id = pautaId,
                CodigoInternoColaboradorCriacao = codigoInternoColaboradorAvaliado,
                CodigoInternoColaboradorSuperior = request.CodigoInternoColaboradorSuperior,
                CodigoInternoColaboradorAvaliado = codigoInternoColaboradorAvaliado,
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
                                AND tipo_origem = 'SUBORDINADO';
                        ";

            var id = await connection.ExecuteScalarAsync<string>(sqlGetId, new
            {
                CodigoInternoColaboradorSuperior = request.CodigoInternoColaboradorSuperior,
                CodigoInternoColaboradorAvaliado = codigoInternoColaboradorAvaliado
            });

            return new InserirPautaSugeridaColaboradorResponseDTO
            {
                Id = id,
                CodigoInternoColaboradorSuperior = request.CodigoInternoColaboradorSuperior,
                CodigoInternoColaboradorAvaliado = codigoInternoColaboradorAvaliado,
                DescricaoPautaSugerida = request.DescricaoPautaSugerida
            };
        }

    }
}
