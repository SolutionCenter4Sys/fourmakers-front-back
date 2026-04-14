using ApiClient.Domain;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Core.Domain.Social;
using Dapper;
using DataTransferObject.Domain.Social;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Social
{
    public class Feedback360Repository : IFeedback360Repository
    {
        private readonly IDBConnection _dapperConnection;

        private const string SqlSelectFeedback360 = @"
                SELECT 
                    f.id AS Id,
                    f.codigo_interno_colaborador_remetente AS CodigoInternoColaboradorRemetente,
                    tr.nome_completo AS NomeRemetente,
                    img_tr.path AS RemetenteImagemPathRelativa,
                    f.tb_org_id AS OrgId,
                    f.situacao AS Situacao,
                    f.tarefa AS Tarefa,
                    f.acao AS Acao,
                    f.resultado AS Resultado,
                    f.previa AS Previa,
                    f.data_interacao AS DataInteracao,
                    f.tb_feedback360_relacionamento_id AS Feedback360RelacionamentoId,
                    r.descricao AS RelacionamentoDescricao,
                    f.relacionamento_outro AS RelacionamentoOutroEspecificacao,
                    f.tb_feedback360_avaliacao_id AS Feedback360AvaliacaoId,
                    a.descricao AS AvaliacaoDescricao,
                    f.data_criacao AS DataCriacao,
                    f.data_alteracao AS DataAlteracao,
                    f.editado AS Editado,
                    (UTC_TIMESTAMP() <= DATE_ADD(f.data_criacao, INTERVAL 1 HOUR)) AS Editavel
                FROM tb_feedback360 f
                INNER JOIN tb_feedback360_relacionamento r ON r.id = f.tb_feedback360_relacionamento_id
                LEFT JOIN tb_feedback360_avaliacoes a ON a.id = f.tb_feedback360_avaliacao_id
                LEFT JOIN tb_colaborador tr ON tr.codigo_interno_colaborador = f.codigo_interno_colaborador_remetente
                LEFT JOIN tb_imagem img_tr ON img_tr.id = tr.imagem_id AND img_tr.ativo = 1";

        private const string SqlJoinRecebidosDestinatario = @"
                INNER JOIN tb_feedback360_destinatario dest
                    ON dest.tb_feedback360_id = f.id AND dest.codigo_interno_colaborador = @CodigoInternoColaborador ";

        private const string SqlFiltrosComuns = @"
                  AND f.data_criacao >= @DataInicio
                  AND f.data_criacao <= @DataFim
                  AND (@SentimentoId IS NULL OR f.tb_feedback360_avaliacao_id = @SentimentoId)
                  AND (@RelacionamentoId IS NULL OR f.tb_feedback360_relacionamento_id = @RelacionamentoId)";

        public Feedback360Repository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<Feedback360DTO> GetByIdAsync(Guid id)
        {
            const string sql = SqlSelectFeedback360 + @"
                WHERE f.id = @Id";

            var connection = _dapperConnection.GetConnection();
            var row = await connection.QueryFirstOrDefaultAsync<Feedback360DTO>(sql, new { Id = id });
            if (row != null)
            {
                AplicarUrlsRemetenteFeedback360(new[] { row });
                await CarregarDestinatariosAsync(connection, new[] { row });
            }
            return row;
        }

        public async Task<IEnumerable<Feedback360DTO>> ListarEnviadosPorColaboradorAsync(Guid codigoInternoColaborador, FiltroFeedback360DTO filtro)
        {
            var sql = SqlSelectFeedback360 + @"
                WHERE f.codigo_interno_colaborador_remetente = @CodigoInternoColaborador"
                + SqlFiltrosComuns + @"
                ORDER BY f.data_criacao DESC
                LIMIT @Limit OFFSET @Cursor";

            var connection = _dapperConnection.GetConnection();
            var list = (await connection.QueryAsync<Feedback360DTO>(sql, MontarParametros(codigoInternoColaborador, filtro))).ToList();
            AplicarUrlsRemetenteFeedback360(list);
            await CarregarDestinatariosAsync(connection, list);
            return list;
        }

        public async Task<IEnumerable<Feedback360DTO>> ListarRecebidosPorColaboradorAsync(Guid codigoInternoColaborador, FiltroFeedback360DTO filtro)
        {
            var sql = SqlSelectFeedback360.Replace(
                    "FROM tb_feedback360 f",
                    "FROM tb_feedback360 f" + SqlJoinRecebidosDestinatario)
                + SqlFiltrosComuns + @"
                ORDER BY f.data_criacao DESC
                LIMIT @Limit OFFSET @Cursor";

            var connection = _dapperConnection.GetConnection();
            var list = (await connection.QueryAsync<Feedback360DTO>(sql, MontarParametros(codigoInternoColaborador, filtro))).ToList();
            AplicarUrlsRemetenteFeedback360(list);
            await CarregarDestinatariosAsync(connection, list);
            return list;
        }

        private static object MontarParametros(Guid codigoInternoColaborador, FiltroFeedback360DTO filtro)
        {
            return new
            {
                CodigoInternoColaborador = codigoInternoColaborador,
                DataInicio = filtro?.DataInicio?.Date,
                DataFim = filtro?.DataFim?.Date.AddDays(1).AddSeconds(-1),
                SentimentoId = filtro?.SentimentoId,
                RelacionamentoId = filtro?.RelacionamentoId,
                Limit = filtro?.Limit ?? 10,
                Cursor = filtro?.Cursor ?? 0
            };
        }

        public async Task<IEnumerable<Feedback360DTO>> ListarRecebidosPorColaboradorFiltradoAsync(
            Guid codigoInternoColaborador,
            DateTime dataInicio,
            DateTime dataFim,
            int? avaliacaoId)
        {
            var sql = new StringBuilder(SqlSelectFeedback360.Replace(
                "FROM tb_feedback360 f",
                "FROM tb_feedback360 f" + SqlJoinRecebidosDestinatario));
            sql.Append(@"
                WHERE f.data_criacao >= @DataInicio
                  AND f.data_criacao <= @DataFim");

            if (avaliacaoId.HasValue)
                sql.Append(@"
                  AND f.tb_feedback360_avaliacao_id = @AvaliacaoId");

            sql.Append(@"
                ORDER BY f.data_criacao DESC");

            var connection = _dapperConnection.GetConnection();
            var list = (await connection.QueryAsync<Feedback360DTO>(sql.ToString(), new
            {
                CodigoInternoColaborador = codigoInternoColaborador,
                DataInicio = dataInicio.Date,
                DataFim = dataFim.Date.AddDays(1).AddSeconds(-1),
                AvaliacaoId = avaliacaoId
            })).ToList();
            AplicarUrlsRemetenteFeedback360(list);
            await CarregarDestinatariosAsync(connection, list);
            return list;
        }

        public async Task<IEnumerable<Feedback360RelacionamentoDTO>> ListarRelacionamentosAsync()
        {
            const string sql = @"
                SELECT id AS Id, descricao AS Descricao
                FROM tb_feedback360_relacionamento
                WHERE ativo = 1
                ORDER BY id";

            var connection = _dapperConnection.GetConnection();
            return await connection.QueryAsync<Feedback360RelacionamentoDTO>(sql);
        }

        public async Task<IEnumerable<Feedback360AvaliacaoDTO>> ListarAvaliacoesAsync()
        {
            const string sql = @"
                SELECT id AS Id, descricao AS Descricao
                FROM tb_feedback360_avaliacoes
                WHERE ativo = 1
                ORDER BY id";

            var connection = _dapperConnection.GetConnection();
            return await connection.QueryAsync<Feedback360AvaliacaoDTO>(sql);
        }

        public async Task AddAsync(Feedback360DTO feedback, IReadOnlyList<Guid> codigosInternosDestinatarios)
        {
            if (codigosInternosDestinatarios == null || codigosInternosDestinatarios.Count == 0)
                throw new ArgumentException("É necessário ao menos um destinatário.", nameof(codigosInternosDestinatarios));

            const string sqlInsertFeedback = @"
                INSERT INTO tb_feedback360 (
                    id,
                    codigo_interno_colaborador_remetente,
                    tb_org_id,
                    situacao,
                    tarefa,
                    acao,
                    resultado,
                    previa,
                    data_interacao,
                    tb_feedback360_relacionamento_id,
                    relacionamento_outro,
                    tb_feedback360_avaliacao_id,
                    data_criacao,
                    editado
                )
                VALUES (
                    @Id,
                    @CodigoInternoColaboradorRemetente,
                    @OrgId,
                    @Situacao,
                    @Tarefa,
                    @Acao,
                    @Resultado,
                    @Previa,
                    @DataInteracao,
                    @Feedback360RelacionamentoId,
                    @RelacionamentoOutroEspecificacao,
                    @Feedback360AvaliacaoId,
                    @DataCriacao,
                    0
                )";

            const string sqlInsertDest = @"
                INSERT INTO tb_feedback360_destinatario (id, tb_feedback360_id, codigo_interno_colaborador)
                VALUES (@RowId, @FeedbackId, @CodigoInternoColaborador)";

            var connection = _dapperConnection.GetConnection();
            using var transaction = connection.BeginTransaction();
            try
            {
                await connection.ExecuteAsync(sqlInsertFeedback, feedback, transaction);
                foreach (var cod in codigosInternosDestinatarios)
                {
                    await connection.ExecuteAsync(sqlInsertDest, new
                    {
                        RowId = Guid.NewGuid(),
                        FeedbackId = feedback.Id,
                        CodigoInternoColaborador = cod
                    }, transaction);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task UpdateAsync(Feedback360DTO feedback)
        {
            const string sql = @"
                UPDATE tb_feedback360
                SET situacao = @Situacao,
                    tarefa = @Tarefa,
                    acao = @Acao,
                    resultado = @Resultado,
                    previa = @Previa,
                    data_interacao = @DataInteracao,
                    tb_feedback360_relacionamento_id = @Feedback360RelacionamentoId,
                    relacionamento_outro = @RelacionamentoOutroEspecificacao,
                    tb_feedback360_avaliacao_id = @Feedback360AvaliacaoId,
                    data_alteracao = @DataAlteracao,
                    editado = 1
                WHERE id = @Id";

            var connection = _dapperConnection.GetConnection();
            await connection.ExecuteAsync(sql, feedback);
        }

        public async Task InserirLogEdicaoAsync(Guid id, Guid feedback360Id, Guid codigoColaboradorAlterador, string acao, string objetoJson, string alteracaoJson)
        {
            const string sql = @"
                INSERT INTO tb_feedback360_log (
                    id,
                    tb_feedback360_id,
                    tb_colaborador_codigo_interno_colaborador_alterador,
                    acao,
                    objeto,
                    alteracao
                )
                VALUES (
                    @Id,
                    @Feedback360Id,
                    @CodigoColaboradorAlterador,
                    @Acao,
                    @Objeto,
                    @Alteracao
                )";

            var connection = _dapperConnection.GetConnection();
            await connection.ExecuteAsync(sql, new
            {
                Id = id,
                Feedback360Id = feedback360Id,
                CodigoColaboradorAlterador = codigoColaboradorAlterador,
                Acao = acao,
                Objeto = objetoJson,
                Alteracao = alteracaoJson
            });
        }

        public async Task<bool> ColaboradorAtivoNaOrgAsync(Guid codigoInternoColaborador, int orgId)
        {
            const string sql = @"
                SELECT 1
                FROM tb_colaborador_org
                WHERE codigo_interno_colaborador = @CodigoInternoColaborador
                  AND tb_org_id = @OrgId
                  AND ativo = 1
                LIMIT 1";
            var connection = _dapperConnection.GetConnection();
            var result = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { CodigoInternoColaborador = codigoInternoColaborador, OrgId = orgId });
            return result.HasValue;
        }

        public async Task<IEnumerable<Feedback360DTO>> ListarMuralReconhecimentoPorOrgAsync(int orgId, FiltroFeedback360DTO filtro)
        {
            var busca = string.IsNullOrWhiteSpace(filtro?.Busca) ? null : $"%{filtro.Busca.Trim()}%";
            var sqlFiltroBusca = busca == null ? "" : @"
                  AND (
                        f.previa LIKE @Busca
                        OR EXISTS (
                            SELECT 1 FROM tb_feedback360_destinatario dx
                            INNER JOIN tb_colaborador tdx ON tdx.codigo_interno_colaborador = dx.codigo_interno_colaborador
                            WHERE dx.tb_feedback360_id = f.id AND tdx.nome_completo LIKE @Busca)
                      )";

            var sql = SqlSelectFeedback360 + @"
                WHERE f.tb_org_id = @OrgId"
                + SqlFiltrosComuns
                + sqlFiltroBusca + @"
                ORDER BY f.data_criacao DESC
                LIMIT @Limit OFFSET @Cursor";

            var connection = _dapperConnection.GetConnection();
            var list = (await connection.QueryAsync<Feedback360DTO>(sql, new
            {
                OrgId = orgId,
                DataInicio = filtro?.DataInicio?.Date,
                DataFim = filtro?.DataFim?.Date.AddDays(1).AddSeconds(-1),
                SentimentoId = filtro?.SentimentoId,
                RelacionamentoId = filtro?.RelacionamentoId,
                Busca = busca,
                Limit = filtro?.Limit ?? 10,
                Cursor = filtro?.Cursor ?? 0
            })).ToList();
            AplicarUrlsRemetenteFeedback360(list);
            await CarregarDestinatariosAsync(connection, list);
            return list;
        }

        public async Task<IEnumerable<Feedback360MuralReconhecimentoDTO>> BuscarMuralReconhecimentoAsync(int orgId, FiltroFeedback360DTO filtro, Guid codigoInternoColaboradorLogado)
        {
            var busca = string.IsNullOrWhiteSpace(filtro?.Busca) ? null : $"%{filtro.Busca.Trim()}%";
            var sqlFiltroBusca = busca == null ? "" : @"
                  AND (tr.nome_completo LIKE @Busca OR f.situacao LIKE @Busca OR f.tarefa LIKE @Busca OR f.acao LIKE @Busca OR f.resultado LIKE @Busca OR f.previa LIKE @Busca
                    OR EXISTS (
                        SELECT 1 FROM tb_feedback360_destinatario dx
                        INNER JOIN tb_colaborador tdx ON tdx.codigo_interno_colaborador = dx.codigo_interno_colaborador
                        WHERE dx.tb_feedback360_id = f.id AND tdx.nome_completo LIKE @Busca))";

            string sqlMural = @"
                SELECT
                    f.id AS Id,
                    f.codigo_interno_colaborador_remetente AS CodigoInternoColaboradorRemetente,
                    tr.nome_completo AS NomeRemetente,
                    f.data_criacao AS DataCriacao,
                    f.data_interacao AS DataInteracao,
                    f.situacao AS Situacao,
                    f.tarefa AS Tarefa,
                    f.acao AS Acao,
                    f.resultado AS Resultado,
                    f.previa AS Previa,
                    TRIM(CONCAT(
                        r.descricao,
                        IF(TRIM(IFNULL(f.relacionamento_outro, '')) = '', '', CONCAT(' — ', TRIM(f.relacionamento_outro)))
                    )) AS Relacionamento
                FROM tb_feedback360 f
                INNER JOIN tb_feedback360_relacionamento r ON r.id = f.tb_feedback360_relacionamento_id
                LEFT JOIN tb_colaborador tr ON tr.codigo_interno_colaborador = f.codigo_interno_colaborador_remetente
                WHERE f.tb_org_id = @OrgId
                  AND f.data_criacao >= @DataInicio
                  AND f.data_criacao <= @DataFim
                  AND (@SentimentoId IS NULL OR f.tb_feedback360_avaliacao_id = @SentimentoId)
                  AND (@RelacionamentoId IS NULL OR f.tb_feedback360_relacionamento_id = @RelacionamentoId)"
                + sqlFiltroBusca + @"
                ORDER BY f.data_criacao DESC
                LIMIT @Limit OFFSET @Cursor";

            var connection = _dapperConnection.GetConnection();
            var cards = (await connection.QueryAsync<Feedback360MuralCardRowDTO>(sqlMural, new
            {
                OrgId = orgId,
                DataInicio = filtro?.DataInicio?.Date,
                DataFim = filtro?.DataFim?.Date.AddDays(1).AddSeconds(-1),
                SentimentoId = filtro?.SentimentoId,
                RelacionamentoId = filtro?.RelacionamentoId,
                Busca = busca,
                Limit = filtro?.Limit ?? 10,
                Cursor = filtro?.Cursor ?? 0
            })).ToList();

            if (cards.Count == 0)
                return Enumerable.Empty<Feedback360MuralReconhecimentoDTO>();

            var ids = cards.Select(c => c.Id).Distinct().ToList();

            const string sqlDestMural = @"
                SELECT
                    d.tb_feedback360_id AS Feedback360Id,
                    d.codigo_interno_colaborador AS CodigoInternoColaborador,
                    c.nome_completo AS NomeCompleto,
                    img.path AS ImagemPathRelativa
                FROM tb_feedback360_destinatario d
                INNER JOIN tb_colaborador c ON c.codigo_interno_colaborador = d.codigo_interno_colaborador
                LEFT JOIN tb_imagem img ON img.id = c.imagem_id AND img.ativo = 1
                WHERE d.tb_feedback360_id IN @Ids
                ORDER BY d.tb_feedback360_id, c.nome_completo";
            var destRows = (await connection.QueryAsync<Feedback360MuralDestinatarioRowDTO>(sqlDestMural, new { Ids = ids })).ToList();
            var destPorFeedback = destRows
                .GroupBy(r => r.Feedback360Id)
                .ToDictionary(g => g.Key, g => g.ToList());
            var reacoesAtivas = (await ListarReacoesMuralAsync()).ToList();

            const string sqlContagem = @"
                SELECT i.tb_feedback360_id AS Feedback360Id, i.tb_feedback360_mural_reacao_id AS ReacaoId, r.emoji AS Emoji, COUNT(*) AS Quantidade
                FROM tb_feedback360_mural_reacoes i
                INNER JOIN tb_feedback360_mural_emojis_reacao r ON r.id = i.tb_feedback360_mural_reacao_id
                WHERE i.tb_feedback360_id IN @Ids
                GROUP BY i.tb_feedback360_id, i.tb_feedback360_mural_reacao_id, r.emoji";
            var contagens = (await connection.QueryAsync<Feedback360MuralReacaoContagemRowDTO>(sqlContagem, new { Ids = ids })).ToList();

            const string sqlUsuarioReagiu = @"
                SELECT tb_feedback360_id AS Feedback360Id, tb_feedback360_mural_reacao_id AS ReacaoId
                FROM tb_feedback360_mural_reacoes
                WHERE codigo_interno_colaborador = @CodigoLogado AND tb_feedback360_id IN @Ids";
            var usuarioReagiuSet = new HashSet<(Guid FbId, int ReacaoId)>(
                (await connection.QueryAsync<Feedback360MuralReacaoUsuarioRowDTO>(sqlUsuarioReagiu, new { CodigoLogado = codigoInternoColaboradorLogado, Ids = ids }))
                .Select(x => (x.Feedback360Id, x.ReacaoId)));

            var contagemDict = contagens.GroupBy(x => x.Feedback360Id).ToDictionary(g => g.Key, g => g.ToDictionary(y => y.ReacaoId, y => (y.Emoji, y.Quantidade)));

            var resultado = new List<Feedback360MuralReconhecimentoDTO>();
            foreach (var c in cards)
            {
                var cardId = c.Id;
                var reacoesDoCard = new List<Feedback360MuralReacaoCardDTO>();
                foreach (var reacao in reacoesAtivas)
                {
                    var (emoji, qtd) = contagemDict.TryGetValue(cardId, out var d) && d.TryGetValue(reacao.Id, out var t) ? t : (reacao.Emoji, 0);
                    reacoesDoCard.Add(new Feedback360MuralReacaoCardDTO
                    {
                        ReacaoId = reacao.Id,
                        Slug = reacao.Slug,
                        Emoji = emoji,
                        Quantidade = qtd,
                        UsuarioReagiu = usuarioReagiuSet.Contains((cardId, reacao.Id))
                    });
                }

                var colaboradoresDest = destPorFeedback.TryGetValue(cardId, out var drs)
                    ? drs.Select(r => MontarColaboradorCardComFoto(r.CodigoInternoColaborador, r.NomeCompleto, r.ImagemPathRelativa)).ToList()
                    : new List<Feedback360ColaboradorCardDTO>();

                resultado.Add(new Feedback360MuralReconhecimentoDTO
                {
                    Id = c.Id,
                    ColaboradorRemetente = new Feedback360ColaboradorCardDTO { Id = c.CodigoInternoColaboradorRemetente, Nome = c.NomeRemetente },
                    ColaboradoresDestinatarios = colaboradoresDest,
                    DataCriacao = c.DataCriacao,
                    DataInteracao = c.DataInteracao,
                    Situacao = c.Situacao,
                    Tarefa = c.Tarefa,
                    Acao = c.Acao,
                    Resultado = c.Resultado,
                    Previa = c.Previa,
                    Relacionamento = c.Relacionamento,
                    Reacoes = reacoesDoCard
                });
            }
            return resultado;
        }

        public async Task<IEnumerable<Feedback360MuralReacaoDTO>> ListarReacoesMuralAsync()
        {
            const string sql = @"
                SELECT id AS Id, slug AS Slug, emoji AS Emoji, ativo AS Ativo
                FROM tb_feedback360_mural_emojis_reacao
                WHERE ativo = 1
                ORDER BY id";
            var connection = _dapperConnection.GetConnection();
            return await connection.QueryAsync<Feedback360MuralReacaoDTO>(sql);
        }

        public async Task<int> DefinirReacaoMuralAsync(Guid feedback360Id, int reacaoId, Guid codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();
            using var transaction = connection.BeginTransaction();
            try
            {
                const string sqlDelete = @"
                    DELETE FROM tb_feedback360_mural_reacoes
                    WHERE tb_feedback360_id = @Feedback360Id AND codigo_interno_colaborador = @CodigoInternoColaborador";
                await connection.ExecuteAsync(sqlDelete, new { Feedback360Id = feedback360Id, CodigoInternoColaborador = codigoInternoColaborador }, transaction);

                if (reacaoId > 0)
                {
                    const string sqlInsert = @"
                        INSERT INTO tb_feedback360_mural_reacoes (tb_feedback360_id, tb_feedback360_mural_reacao_id, codigo_interno_colaborador)
                        VALUES (@Feedback360Id, @ReacaoId, @CodigoInternoColaborador)";
                    await connection.ExecuteAsync(sqlInsert, new { Feedback360Id = feedback360Id, ReacaoId = reacaoId, CodigoInternoColaborador = codigoInternoColaborador }, transaction);
                }

                transaction.Commit();
                return reacaoId > 0 ? reacaoId : 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<ColaboradorGestorFeedbackDTO>> ListarSubordinadosGestorAsync(Guid codigoInternoGestor, int orgId)
        {
            const string sqlCodExterno = @"
                SELECT cod_colaborador_externo
                FROM tb_colaborador_org
                WHERE codigo_interno_colaborador = @CodigoInternoGestor
                  AND tb_org_id = @OrgId
                  AND ativo = 1
                LIMIT 1";

            const string sqlSubordinados = @"
                SELECT
                    c.codigo_interno_colaborador AS CodigoInternoColaborador,
                    c.nome_completo              AS NomeCompleto,
                    co.cod_colaborador_externo   AS CodColaboradorExterno
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

            var connection = _dapperConnection.GetConnection();

            var codExterno = await connection.QueryFirstOrDefaultAsync<string>(
                sqlCodExterno,
                new { CodigoInternoGestor = codigoInternoGestor, OrgId = orgId });

            if (string.IsNullOrEmpty(codExterno))
                return Enumerable.Empty<ColaboradorGestorFeedbackDTO>();

            var resultado = new List<ColaboradorGestorFeedbackDTO>();
            var visitados = new HashSet<string>();

            await BuscarSubordinadosRecursivo(connection, sqlSubordinados, orgId, codExterno, resultado, visitados);

            return resultado.OrderBy(c => c.NomeCompleto);
        }

        private async Task BuscarSubordinadosRecursivo(System.Data.IDbConnection connection, string sql, int orgId, string codExternoGestor, List<ColaboradorGestorFeedbackDTO> resultado, HashSet<string> visitados)
        {
            var subordinados = (await connection.QueryAsync<ColaboradorGestorFeedbackInternalDTO>(
                sql,
                new { CodExternoGestor = codExternoGestor, OrgId = orgId })).ToList();

            foreach (var s in subordinados)
            {
                if (!visitados.Add(s.CodColaboradorExterno))
                    continue;

                resultado.Add(new ColaboradorGestorFeedbackDTO
                {
                    CodigoInternoColaborador = Guid.Parse(s.CodigoInternoColaborador),
                    NomeCompleto = s.NomeCompleto
                });

                await BuscarSubordinadosRecursivo(connection, sql, orgId, s.CodColaboradorExterno, resultado, visitados);
            }
        }

        private sealed class Feedback360DestinatarioDbRow
        {
            public Guid Feedback360Id { get; set; }
            public Guid CodigoInternoColaborador { get; set; }
            public string NomeCompleto { get; set; }
            public string ImagemPathRelativa { get; set; }
        }

        private static async Task CarregarDestinatariosAsync(System.Data.IDbConnection connection, IReadOnlyList<Feedback360DTO> feedbacks)
        {
            if (feedbacks == null || feedbacks.Count == 0)
                return;

            var ids = feedbacks.Select(f => f.Id).Distinct().ToArray();
            const string sql = @"
                SELECT d.tb_feedback360_id AS Feedback360Id,
                       d.codigo_interno_colaborador AS CodigoInternoColaborador,
                       c.nome_completo AS NomeCompleto,
                       img.path AS ImagemPathRelativa
                FROM tb_feedback360_destinatario d
                INNER JOIN tb_colaborador c ON c.codigo_interno_colaborador = d.codigo_interno_colaborador
                LEFT JOIN tb_imagem img ON img.id = c.imagem_id AND img.ativo = 1
                WHERE d.tb_feedback360_id IN @Ids
                ORDER BY d.tb_feedback360_id, c.nome_completo";

            var rows = (await connection.QueryAsync<Feedback360DestinatarioDbRow>(sql, new { Ids = ids })).ToList();
            var byFeedback = rows
                .GroupBy(r => r.Feedback360Id)
                .ToDictionary(g => g.Key, g => g.OrderBy(x => x.NomeCompleto).ToList());

            foreach (var f in feedbacks)
            {
                if (!byFeedback.TryGetValue(f.Id, out var listaRaw))
                {
                    f.Destinatarios = new List<Feedback360DestinatarioItemDTO>();
                    continue;
                }

                f.Destinatarios = listaRaw.Select(MapearDestinatarioItemComFoto).ToList();
            }
        }

        private static Feedback360DestinatarioItemDTO MapearDestinatarioItemComFoto(Feedback360DestinatarioDbRow r)
        {
            var dto = new Feedback360DestinatarioItemDTO
            {
                CodigoInternoColaborador = r.CodigoInternoColaborador,
                NomeCompleto = r.NomeCompleto
            };
            if (string.IsNullOrWhiteSpace(r.ImagemPathRelativa))
                return dto;
            try
            {
                var baseMidia = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA);
                var p = r.ImagemPathRelativa;
                dto.UrlFoto = baseMidia + p;
                dto.UrlFotoThumb = baseMidia + p.Replace(".png", "_thumb.png");
                dto.UrlFotoThumbMini = baseMidia + p.Replace(".png", "_thumb50.png");
                dto.UrlFotoThumbVeryMini = baseMidia + p.Replace(".png", "_thumb25.png");
            }
            catch
            {
                // Igual GetColaborador
            }
            return dto;
        }

        private static void AplicarUrlsRemetenteFeedback360(IReadOnlyList<Feedback360DTO> feedbacks)
        {
            if (feedbacks == null)
                return;
            foreach (var f in feedbacks)
                AplicarUrlsRemetenteUm(f);
        }

        private static void AplicarUrlsRemetenteUm(Feedback360DTO f)
        {
            if (f == null)
                return;
            var path = f.RemetenteImagemPathRelativa;
            f.RemetenteImagemPathRelativa = null;
            if (string.IsNullOrWhiteSpace(path))
                return;
            try
            {
                var baseMidia = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA);
                var p = path;
                f.RemetenteUrlFoto = baseMidia + p;
                f.RemetenteUrlFotoThumb = baseMidia + p.Replace(".png", "_thumb.png");
                f.RemetenteUrlFotoThumbMini = baseMidia + p.Replace(".png", "_thumb50.png");
                f.RemetenteUrlFotoThumbVeryMini = baseMidia + p.Replace(".png", "_thumb25.png");
            }
            catch
            {
                // Igual GetColaborador
            }
        }

        /// <summary>
        /// Monta URLs de perfil como em BuscaColaboradorRepository.GetColaborador / ShowMe (SERVICE_MIDIA + path e thumbs .png).
        /// </summary>
        private static Feedback360ColaboradorCardDTO MontarColaboradorCardComFoto(Guid id, string nome, string imagemPathRelativa)
        {
            var card = new Feedback360ColaboradorCardDTO { Id = id, Nome = nome };
            if (string.IsNullOrWhiteSpace(imagemPathRelativa))
                return card;

            try
            {
                var baseMidia = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA);
                var p = imagemPathRelativa;
                card.UrlFoto = baseMidia + p;
                card.UrlFotoThumb = baseMidia + p.Replace(".png", "_thumb.png");
                card.UrlFotoThumbMini = baseMidia + p.Replace(".png", "_thumb50.png");
                card.UrlFotoThumbVeryMini = baseMidia + p.Replace(".png", "_thumb25.png");
            }
            catch
            {
                // Mesmo comportamento de GetColaborador: em falha, urls permanecem nulas.
            }

            return card;
        }

        public async Task<GerarModeloStarMoxeResultadoDTO> GerarModeloStarMoxeAsync(string situacao, string dataInteracao, string contexto)
        {
            var prompt = $@"Você é um assistente que organiza feedbacks no método STAR (Situação, Tarefa, Ação, Resultado).

                **Dados enviados pelo usuário:**

                - **Situação** (onde ocorreu): {situacao}
                - **Data da interação**: {dataInteracao}
                - **Contexto** (o que ocorreu, impacto, ação da pessoa): {contexto}

                **Instruções:**
                Separe e melhore esses dados em:
                1. **situacao**: Onde ocorreu (frase clara, com a data quando fizer sentido).
                2. **tarefa**: Objetivo ou tarefa principal (verbos no infinitivo, ex: alinhar informações e responder dúvidas).
                3. **acao**: O que a pessoa fez – a ação realizada (texto melhorado, sem repetir a situação).
                4. **resultado**: Resultado obtido (ex: facilitou a tomada de decisão, ajudou o time a avançar).
                5. **previa**: Um único parágrafo fluido para exibição como prévia do feedback, resumindo situação, ação e resultado.

                Retorne SOMENTE um JSON válido, sem markdown e sem texto antes ou depois, com as chaves em minúsculo:
                ""situacao"", ""tarefa"", ""acao"", ""resultado"", ""previa"".
            ";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("x-api-key", "fourlabs-main-key");

            var moxeRequest = new MoxeAiRequestDTO
            {
                Engine = "azure",
                Model = "gpt-5-chat",
                Prompt = prompt,
                Image_base64 = null,
                System_prompt = "",
                Temperature = 0.1,
                Max_tokens = 1000
            };

            var jsonContent = JsonSerializer.Serialize(moxeRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://api-moxe-109278280777.southamerica-east1.run.app/api/v1/inference/", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"Erro na chamada à API Moxe: {response.StatusCode} - {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            var rawResponse = JsonSerializer.Deserialize<MoxeApiRawResponseDTO>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (rawResponse?.Result == null)
                throw new ApplicationException("Resposta da API Moxe não contém o campo 'result'.");

            var jsonResult = rawResponse.Result.Trim();

            if (jsonResult.StartsWith("```json"))
                jsonResult = jsonResult.Substring(7);
            if (jsonResult.StartsWith("```"))
                jsonResult = jsonResult.Substring(3);
            if (jsonResult.EndsWith("```"))
                jsonResult = jsonResult.Substring(0, jsonResult.Length - 3);
            jsonResult = jsonResult.Trim();

            var resultado = JsonSerializer.Deserialize<GerarModeloStarMoxeResultadoDTO>(jsonResult, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (resultado == null)
                throw new ApplicationException("Não foi possível interpretar o JSON retornado pela API Moxe.");

            return resultado;
        }
    }
}
