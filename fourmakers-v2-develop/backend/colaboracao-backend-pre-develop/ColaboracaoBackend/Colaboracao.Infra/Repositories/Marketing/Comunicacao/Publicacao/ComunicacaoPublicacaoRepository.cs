using Colaboracao.Core.Interfaces;
using Core.Domain.Marketing.Comunicacao.Publicacao;
using Dapper;
using DataTransferObject.Domain.Marketing.Comunicacao;
using DataTransferObject.Domain.Marketing.Comunicacao.Publicacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Helper;
using ApiClient.Domain;

namespace Colaboracao.Infra.Repositories.Marketing.Comunicacao.Publicacao
{
    public class ComunicacaoPublicacaoRepository : IComunicacaoPublicacaoRepository
    {
        private readonly IDBConnection _dapperConnection;

        public ComunicacaoPublicacaoRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        #region Row Classes

        private class PublicacaoRow
        {
            public Guid Id { get; set; }
            public string Tipo { get; set; }
            public string Titulo { get; set; }
            public string Subtitulo { get; set; }
            public string Conteudo { get; set; }
            public bool RequerConfirmacaoLeitura { get; set; }
            public bool PermiteComentarios { get; set; }
            public bool PermiteCurtidas { get; set; }
            public bool Fixada { get; set; }
            public bool PermiteDownload { get; set; }
            public bool OcultarNoFeed { get; set; }
            public string Pasta { get; set; }
            public DateTime? DataAgendamentoPublicacao { get; set; }
            public DateTime? DataPublicacao { get; set; }
            public DateTime? DataValidade { get; set; }
            public string PublicacaoStatus { get; set; }
            public string AprovacaoStatus { get; set; }
            public string CodigoInternoColaboradorCriacao { get; set; }
            public string NomeAutor { get; set; }
            public string EmailAutor { get; set; }
            public string DepartamentoAutor { get; set; }
            public string CodDepartamentoAutor { get; set; }
            public string CodigoInternoColaboradorAprovador { get; set; }
            public string NomeAprovador { get; set; }
            public string MotivoRejeicao { get; set; }
            public string AutorImagemPath { get; set; }
            public string AutoriaTipo { get; set; }
            public string NomeAutorAlternativo { get; set; }
            public string UrlFotoAlternativa { get; set; }
        }

        private class AnexoRow
        {
            public Guid Id { get; set; }
            public Guid PublicacaoId { get; set; }
            public string NomeArquivo { get; set; }
            public string UrlArquivo { get; set; }
            public long? TamanhoBytes { get; set; }
            public string Tipo { get; set; }
        }

        private class InteracaoRow
        {
            public Guid PublicacaoId { get; set; }
            public string CodigoInterno { get; set; }
            public DateTime? DataPrimeiraEntrega { get; set; }
            public bool? Visualizado { get; set; }
            public DateTime? DataVisualizado { get; set; }
            public bool? ConfirmouLeitura { get; set; }
            public DateTime? DataConfirmouLeitura { get; set; }
            public string CurtidaEmoji { get; set; }
        }

        private class InteracaoComentarioRow
        {
            public Guid ComentarioId { get; set; }
            public string Emoji { get; set; }
            public int Quantidade { get; set; }
        }

        private class InteracaoComentarioUsuarioRow
        {
            public Guid ComentarioId { get; set; }
            public string CodigoInterno { get; set; }
            public DateTime? DataInteracao { get; set; }
            public string CurtidaEmoji { get; set; }
        }

        private class InteracaoPublicacaoEmojiRow
        {
            public Guid PublicacaoId { get; set; }
            public string Emoji { get; set; }
            public int Quantidade { get; set; }
        }

        private class ComentarioRow
        {
            public Guid ComentarioId { get; set; }
            public Guid PublicacaoId { get; set; }
            public string Conteudo { get; set; }
            public DateTime? DataCriacao { get; set; }
            public Guid? ComentarioPaiId { get; set; }
            public string CodigoInternoColaborador { get; set; }
            public string NomeAutor { get; set; }
            public string EmailAutor { get; set; }
            public string AutorImagemPath { get; set; }
        }

        private class LabelRow
        {
            public Guid PublicacaoId { get; set; }
            public Guid LabelId { get; set; }
            public string LabelNome { get; set; }
            public string LabelTipo { get; set; }
        }

        private class TagRow
        {
            public Guid PublicacaoId { get; set; }
            public Guid TagId { get; set; }
            public string TagNome { get; set; }
        }

        private class GrupoRow
        {
            public Guid PublicacaoId { get; set; }
            public string GrupoNome { get; set; }
        }

        private class ComunidadeRow
        {
            public Guid PublicacaoId { get; set; }
            public Guid? ComunidadeId { get; set; }
            public string ComunidadeNome { get; set; }
        }

        private class AnaliticoRow
        {
            public Guid PublicacaoId { get; set; }
            public int QuantidadeVisualizacao { get; set; }
            public int QuantidadeCurtida { get; set; }
            public int QuantidadeComentarios { get; set; }
            public int QuantidadeConfirmacoesLeitura { get; set; }
        }

        #endregion

        #region Feed (Publicacao)

        /// <summary>
        /// Predicado sobre alias com: vê conteúdo se comunidade pública e está em cup, ou privada com (grupo OU cup) e sem opt-out em nao_participando quando permite_sair.
        /// </summary>
        private const string SqlComunidadeAcessivelParaColaborador = @"
((com.tipo = 'publica')
 OR EXISTS (
    SELECT 1 FROM tb_mkt_comunidade_grupo cg
    INNER JOIN tb_mkt_grupo_usuario gu ON gu.tb_mkt_grupo_id = cg.tb_mkt_grupo_id AND gu.codigo_interno_colaborador = @CodigoInternoColaborador
    WHERE cg.tb_mkt_comunidade_id = com.id)
 OR (com.tipo = 'privada' AND EXISTS (
    SELECT 1 FROM tb_mkt_comunidade_usuario_participando cup0
    WHERE cup0.tb_mkt_comunidade_id = com.id AND cup0.codigo_interno_colaborador = @CodigoInternoColaborador))
)
AND (
    (com.tipo = 'publica' AND EXISTS (SELECT 1 FROM tb_mkt_comunidade_usuario_participando cup WHERE cup.tb_mkt_comunidade_id = com.id AND cup.codigo_interno_colaborador = @CodigoInternoColaborador))
    OR (com.tipo = 'privada'
        AND (EXISTS (
                SELECT 1 FROM tb_mkt_comunidade_grupo cg2
                INNER JOIN tb_mkt_grupo_usuario gu2 ON gu2.tb_mkt_grupo_id = cg2.tb_mkt_grupo_id AND gu2.codigo_interno_colaborador = @CodigoInternoColaborador
                WHERE cg2.tb_mkt_comunidade_id = com.id)
            OR EXISTS (SELECT 1 FROM tb_mkt_comunidade_usuario_participando cup2 WHERE cup2.tb_mkt_comunidade_id = com.id AND cup2.codigo_interno_colaborador = @CodigoInternoColaborador))
        AND (com.permite_sair = 0 OR NOT EXISTS (SELECT 1 FROM tb_mkt_comunidade_privada_usuario_nao_participando np WHERE np.tb_mkt_comunidade_id = com.id AND np.codigo_interno_colaborador = @CodigoInternoColaborador)))
)";

        /// <summary>Excluídas não entram no feed nem na leitura por id do colaborador, mesmo se o cliente enviar filtro de status.</summary>
        private const string SqlNuncaIncluirPublicacaoExcluida = " AND p.publicacao_status <> 'excluida'";

        public async Task CriarOuAtualizarInteracoesParaFeedGeralAsync(string codigoInternoColaborador, int orgId, bool somenteLeituraObrigatoria, string tipo, List<string> labels, List<string> tags, List<string> status, List<string> statusAprovacao, string comunidadeId, bool somenteNaoOcultoNoFeed = false, bool somentePublicacaoOficial = false)
        {
            if (string.IsNullOrWhiteSpace(codigoInternoColaborador))
                throw new ArgumentException("Código interno do colaborador é obrigatório para o feed de publicações.");
            var connection = _dapperConnection.GetConnection();
            var publicacaoIds = await ObterIdsPublicacoesFeedGeralAsync(connection, orgId, codigoInternoColaborador, somenteLeituraObrigatoria, tipo, labels, tags, status, statusAprovacao, comunidadeId, somenteNaoOcultoNoFeed, somentePublicacaoOficial);
            await CriarOuAtualizarInteracoesAsync(connection, publicacaoIds, codigoInternoColaborador);
        }

        private async Task<List<Guid>> ObterIdsPublicacoesFeedGeralAsync(System.Data.IDbConnection connection, int orgId, string codigoInternoColaborador, bool somenteLeituraObrigatoria, string tipo, List<string> labels, List<string> tags, List<string> status, List<string> statusAprovacao, string comunidadeId, bool somenteNaoOcultoNoFeed = false, bool somentePublicacaoOficial = false)
        {
            var sqlBase = @"
                SELECT DISTINCT p.id AS Id
                FROM tb_mkt_publicacao p
                LEFT JOIN tb_colaborador autor
                    ON p.codigo_interno_colaborador_criacao = autor.codigo_interno_colaborador";

            if (labels != null && labels.Count > 0)
                sqlBase += @"
                INNER JOIN tb_mkt_publicacao_label pl ON pl.tb_mkt_publicacao_id = p.id
                INNER JOIN tb_mkt_label l ON l.id = pl.tb_mkt_label_id AND l.tb_org_id = @OrgId AND l.nome IN @Labels";

            if (tags != null && tags.Count > 0)
                sqlBase += @"
                INNER JOIN tb_mkt_publicacao_tag pt ON pt.tb_mkt_publicacao_id = p.id
                INNER JOIN tb_mkt_tag t ON t.id = pt.tb_mkt_tag_id AND t.tb_org_id = @OrgId AND t.nome IN @Tags";

            if (!somentePublicacaoOficial && !string.IsNullOrWhiteSpace(comunidadeId))
                sqlBase += @"
                INNER JOIN tb_mkt_comunidade_publicacao cp ON cp.tb_mkt_publicacao_id = p.id AND cp.tb_mkt_comunidade_id = @ComunidadeId
                INNER JOIN tb_mkt_comunidade com ON com.id = cp.tb_mkt_comunidade_id AND com.tb_org_id = @OrgId AND com.ativo = 1";

            sqlBase += @"
                WHERE p.tb_org_id = @OrgId" + SqlNuncaIncluirPublicacaoExcluida;
            if (somenteNaoOcultoNoFeed)
                sqlBase += " AND p.ocultar_no_feed = 0";

            if (status != null && status.Count > 0)
                sqlBase += " AND p.publicacao_status IN @Status";
            if (statusAprovacao != null && statusAprovacao.Count > 0)
                sqlBase += " AND p.aprovacao_status IN @StatusAprovacao";

            sqlBase += @"
                    AND (
                        NOT EXISTS (SELECT 1 FROM tb_mkt_publicaco_grupo pg2 WHERE pg2.tb_mkt_publicacao_id = p.id)
                        OR EXISTS (SELECT 1 FROM tb_mkt_publicaco_grupo pg2 INNER JOIN tb_mkt_grupo_usuario gu ON gu.tb_mkt_grupo_id = pg2.tb_mkt_grupo_id AND gu.codigo_interno_colaborador = @CodigoInternoColaborador WHERE pg2.tb_mkt_publicacao_id = p.id)
                    )";

            if (somenteLeituraObrigatoria)
                sqlBase += " AND p.requer_confirmacao_leitura = 1";

            if (!string.IsNullOrWhiteSpace(tipo))
                sqlBase += " AND p.tipo = @Tipo";

            if (!somentePublicacaoOficial && !string.IsNullOrWhiteSpace(comunidadeId))
                sqlBase += " AND " + SqlComunidadeAcessivelParaColaborador;

            if (somentePublicacaoOficial)
                sqlBase += " AND p.publicacao_oficial = 1";

            sqlBase += " ORDER BY p.publicacao_fixada_pelo_responsavel DESC, p.data_publicacao DESC";

            var param = new { OrgId = orgId, CodigoInternoColaborador = codigoInternoColaborador, Tipo = tipo, Labels = labels ?? new List<string>(), Tags = tags ?? new List<string>(), Status = status ?? new List<string>(), StatusAprovacao = statusAprovacao ?? new List<string>(), ComunidadeId = comunidadeId };
            var rows = await connection.QueryAsync<Guid>(sqlBase, param);
            return rows.ToList();
        }

        public async Task<PublicacaoFeedResponseDTO> ObterListaPublicacaoGeralAsync(string codigoInternoColaborador, int orgId, bool somenteLeituraObrigatoria, string tipo, List<string> labels, List<string> tags, List<string> status, List<string> statusAprovacao, string comunidadeId, bool somenteNaoOcultoNoFeed = false, bool somentePublicacaoOficial = false)
        {
            if (string.IsNullOrWhiteSpace(codigoInternoColaborador))
                throw new ArgumentException("Código interno do colaborador é obrigatório para listar publicações.");
            var connection = _dapperConnection.GetConnection();

            var sqlBase = @"
                SELECT
                    p.id AS Id,
                    p.tipo AS Tipo,
                    p.titulo AS Titulo,
                    p.subtitulo AS Subtitulo,
                    p.conteudo AS Conteudo,
                    p.requer_confirmacao_leitura AS RequerConfirmacaoLeitura,
                    p.permite_comentarios AS PermiteComentarios,
                    p.permite_curtidas AS PermiteCurtidas,
                    p.publicacao_fixada_pelo_responsavel AS Fixada,
                    p.permite_download AS PermiteDownload,
                    p.ocultar_no_feed AS OcultarNoFeed,
                    p.pasta AS Pasta,
                    p.data_agendamento_publicacao AS DataAgendamentoPublicacao,
                    p.data_publicacao AS DataPublicacao,
                    p.data_validade AS DataValidade,
                    p.publicacao_status AS PublicacaoStatus,
                    p.aprovacao_status AS AprovacaoStatus,
                    p.codigo_interno_colaborador_criacao AS CodigoInternoColaboradorCriacao,
                    p.autoria_tipo AS AutoriaTipo,
                    oc.nome_autor_alternativo AS NomeAutorAlternativo,
                    oc.url_foto_alternativa AS UrlFotoAlternativa,
                    autor.nome_completo AS NomeAutor,
                    uAutor.email AS EmailAutor,
                    dAutor.departamento AS DepartamentoAutor,
                    coAutor.cod_departamento AS CodDepartamentoAutor,
                    p.codigo_interno_colaborador_aprovador AS CodigoInternoColaboradorAprovador,
                    aprov.nome_completo AS NomeAprovador,
                    p.motivo_rejeicao AS MotivoRejeicao,
                    tiAutor.path AS AutorImagemPath
                FROM tb_mkt_publicacao p
                LEFT JOIN tb_mkt_org_config oc ON oc.tb_org_id = p.tb_org_id
                LEFT JOIN tb_colaborador autor
                    ON p.codigo_interno_colaborador_criacao = autor.codigo_interno_colaborador
                LEFT JOIN tb_imagem tiAutor ON autor.imagem_id = tiAutor.id";

            if (labels != null && labels.Count > 0)
                sqlBase += @"
                INNER JOIN tb_mkt_publicacao_label pl ON pl.tb_mkt_publicacao_id = p.id
                INNER JOIN tb_mkt_label l ON l.id = pl.tb_mkt_label_id AND l.tb_org_id = @OrgId AND l.nome IN @Labels";

            if (tags != null && tags.Count > 0)
                sqlBase += @"
                INNER JOIN tb_mkt_publicacao_tag pt ON pt.tb_mkt_publicacao_id = p.id
                INNER JOIN tb_mkt_tag t ON t.id = pt.tb_mkt_tag_id AND t.tb_org_id = @OrgId AND t.nome IN @Tags";

            if (!somentePublicacaoOficial && !string.IsNullOrWhiteSpace(comunidadeId))
                sqlBase += @"
                INNER JOIN tb_mkt_comunidade_publicacao cp ON cp.tb_mkt_publicacao_id = p.id AND cp.tb_mkt_comunidade_id = @ComunidadeId
                INNER JOIN tb_mkt_comunidade com ON com.id = cp.tb_mkt_comunidade_id AND com.tb_org_id = @OrgId AND com.ativo = 1";

            sqlBase += @"
                LEFT JOIN tb_colaborador_org coAutor
                    ON autor.codigo_interno_colaborador = coAutor.codigo_interno_colaborador
                    AND coAutor.tb_org_id = @OrgId AND coAutor.ativo = 1 AND coAutor.cod_diretoria <> 'BANCO TALENTOS'
                LEFT JOIN tb_usuario uAutor
                    ON autor.codigo_interno_colaborador = uAutor.codigo_interno_colaborador
                    AND uAutor.tb_org_id = @OrgId
                LEFT JOIN tb_departamento_org dAutor
                    ON coAutor.cod_departamento = dAutor.cod_departamento
                    AND dAutor.tb_org_id = @OrgId
                LEFT JOIN tb_colaborador aprov
                    ON p.codigo_interno_colaborador_aprovador = aprov.codigo_interno_colaborador
                LEFT JOIN tb_colaborador_org coAprov
                    ON aprov.codigo_interno_colaborador = coAprov.codigo_interno_colaborador
                    AND coAprov.tb_org_id = @OrgId AND coAprov.ativo = 1 AND coAprov.cod_diretoria <> 'BANCO TALENTOS'
                LEFT JOIN tb_usuario uAprov
                    ON aprov.codigo_interno_colaborador = uAprov.codigo_interno_colaborador
                    AND uAprov.tb_org_id = @OrgId
                WHERE p.tb_org_id = @OrgId" + SqlNuncaIncluirPublicacaoExcluida;
            if (somenteNaoOcultoNoFeed)
                sqlBase += " AND p.ocultar_no_feed = 0";

            if (status != null && status.Count > 0)
                sqlBase += " AND p.publicacao_status IN @Status";
            if (statusAprovacao != null && statusAprovacao.Count > 0)
                sqlBase += " AND p.aprovacao_status IN @StatusAprovacao";

            sqlBase += @"
                    AND (
                        NOT EXISTS (SELECT 1 FROM tb_mkt_publicaco_grupo pg2 WHERE pg2.tb_mkt_publicacao_id = p.id)
                        OR EXISTS (SELECT 1 FROM tb_mkt_publicaco_grupo pg2 INNER JOIN tb_mkt_grupo_usuario gu ON gu.tb_mkt_grupo_id = pg2.tb_mkt_grupo_id AND gu.codigo_interno_colaborador = @CodigoInternoColaborador WHERE pg2.tb_mkt_publicacao_id = p.id)
                    )";

            if (somenteLeituraObrigatoria)
                sqlBase += " AND p.requer_confirmacao_leitura = 1";

            if (!string.IsNullOrWhiteSpace(tipo))
                sqlBase += " AND p.tipo = @Tipo";

            if (!somentePublicacaoOficial && !string.IsNullOrWhiteSpace(comunidadeId))
                sqlBase += " AND " + SqlComunidadeAcessivelParaColaborador;

            if (somentePublicacaoOficial)
                sqlBase += " AND p.publicacao_oficial = 1";

            sqlBase += " ORDER BY p.publicacao_fixada_pelo_responsavel DESC, p.data_publicacao DESC";

            var param = new { OrgId = orgId, CodigoInternoColaborador = codigoInternoColaborador, Tipo = tipo, Labels = labels ?? new List<string>(), Tags = tags ?? new List<string>(), Status = status ?? new List<string>(), StatusAprovacao = statusAprovacao ?? new List<string>(), ComunidadeId = comunidadeId };
            var publicacaoRows = (await connection.QueryAsync<PublicacaoRow>(sqlBase, param)).ToList();
            if ((labels != null && labels.Count > 0) || (tags != null && tags.Count > 0))
                publicacaoRows = publicacaoRows.GroupBy(r => r.Id).Select(g => g.First()).ToList();

            if (!publicacaoRows.Any())
            {
                return new PublicacaoFeedResponseDTO
                {
                    QuantidadeTotal = 0,
                    QuantidadePendentesLeitura = 0,
                    QuantidadeLidosAceitos = 0,
                    Labels = new List<PublicacaoLabelResumoDTO>(),
                    Tags = new List<PublicacaoTagResumoDTO>(),
                    Pastas = new List<PublicacaoPastaResumoDTO>(),
                    Filtros = new List<string>(),
                    Publicacoes = new List<PublicacaoDetalheDTO>()
                };
            }

            var publicacaoIds = publicacaoRows.Select(p => p.Id).ToList();
            var labelRows = await ObterLabelsPorPublicacoesAsync(connection, publicacaoIds, orgId);
            var tagRows = await ObterTagsPorPublicacoesAsync(connection, publicacaoIds, orgId);
            var publicacoes = await MontarPublicacoesDetalhadasAsync(connection, publicacaoRows, publicacaoIds, codigoInternoColaborador, orgId, labelRows, tagRows);

            // Contagens sobre a lista retornada: Pendentes + Lidos = total de publicações que exigem confirmação de leitura
            var comRequerLeitura = publicacoes.Where(p => p.RequerConfirmacaoLeitura).ToList();
            var quantidadeLidosAceitos = comRequerLeitura.Count(p => p.Interacao?.ConfirmouLeitura == true);
            var quantidadePendentesLeitura = comRequerLeitura.Count - quantidadeLidosAceitos;

            var idsLabelsNoRetorno = publicacoes.SelectMany(p => p.Labels).Select(l => l.Id).Distinct().ToHashSet();
            var resumoLabels = MontarResumoLabelsFromLabelRows(labelRows.Where(l => idsLabelsNoRetorno.Contains(l.LabelId)).ToList());
            var idsTagsNoRetorno = publicacoes.SelectMany(p => p.Tags).Select(t => t.Id).Distinct().ToHashSet();
            var resumoTags = MontarResumoTagsFromTagRows(tagRows.Where(t => idsTagsNoRetorno.Contains(t.TagId)).ToList());
            var resumoPastas = MontarResumoPastasFromPublicacoes(publicacoes);
            var filtros = MontarFiltrosFromPublicacoes(publicacoes);

            return new PublicacaoFeedResponseDTO
            {
                QuantidadeTotal = publicacoes.Count,
                QuantidadePendentesLeitura = quantidadePendentesLeitura,
                QuantidadeLidosAceitos = quantidadeLidosAceitos,
                Labels = resumoLabels,
                Tags = resumoTags,
                Pastas = resumoPastas,
                Filtros = filtros,
                Publicacoes = publicacoes
            };
        }

        public async Task<bool> ConfirmarLeituraObrigatoriaAsync(Guid publicacaoId, string codigoInternoColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                INSERT INTO tb_mkt_publicacao_colaborador_interacao
                (id, tb_mkt_publicacao_id, codigo_interno_colaborador, confirmou_leitura, data_confirmou_leitura, visualizado, data_visualizado, data_primeira_entrega)
                VALUES (@Id, @PublicacaoId, @CodigoInternoColaborador, 1, @DataConfirmacao, 1, @DataConfirmacao, @DataConfirmacao)
                ON DUPLICATE KEY UPDATE
                    confirmou_leitura = 1,
                    data_confirmou_leitura = @DataConfirmacao";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                Id = Guid.NewGuid(),
                PublicacaoId = publicacaoId,
                CodigoInternoColaborador = codigoInternoColaborador,
                DataConfirmacao = DateTime.Now
            });

            return rowsAffected > 0;
        }

        private async Task CriarOuAtualizarInteracoesAsync(System.Data.IDbConnection connection, List<Guid> publicacaoIds, string codigoInternoColaborador)
        {
            if (publicacaoIds == null || !publicacaoIds.Any())
                return;

            var dataVisualizado = DateTime.Now;
            var sql = @"
                INSERT INTO tb_mkt_publicacao_colaborador_interacao
                (id, tb_mkt_publicacao_id, codigo_interno_colaborador, visualizado, data_visualizado, data_primeira_entrega)
                VALUES (@Id, @PublicacaoId, @CodigoInternoColaborador, 1, @DataVisualizado, @DataVisualizado)
                ON DUPLICATE KEY UPDATE
                    visualizado = 1,
                    data_visualizado = @DataVisualizado";

            foreach (var publicacaoId in publicacaoIds)
            {
                await connection.ExecuteAsync(sql, new
                {
                    Id = Guid.NewGuid(),
                    PublicacaoId = publicacaoId,
                    CodigoInternoColaborador = codigoInternoColaborador,
                    DataVisualizado = dataVisualizado
                });
            }
        }

        public async Task<Guid> InserirPublicacaoAsync(string codigoInternoColaboradorCriacao, int orgId, InserirPublicacaoRequestDTO request, string publicacaoStatus, string aprovacaoStatus, DateTime? dataPublicacao)
        {
            var connection = _dapperConnection.GetConnection();
            var publicacaoId = Guid.NewGuid();

            var publicacaoOficial = string.IsNullOrWhiteSpace(request.ComunidadeId);
            var sqlPublicacao = @"
                INSERT INTO tb_mkt_publicacao
                (
                    id,
                    tipo,
                    publicacao_status,
                    aprovacao_status,
                    titulo,
                    subtitulo,
                    conteudo,
                    requer_confirmacao_leitura,
                    permite_comentarios,
                    permite_curtidas,
                    publicacao_fixada_pelo_responsavel,
                    permite_download,
                    ocultar_no_feed,
                    pasta,
                    publicacao_oficial,
                    processado_envio_notificacao_email,
                    data_agendamento_publicacao,
                    data_publicacao,
                    data_validade,
                    tb_org_id,
                    data_criacao,
                    data_alteracao,
                    codigo_interno_colaborador_criacao,
                    codigo_interno_colaborador_alteracao,
                    autoria_tipo
                )
                VALUES
                (
                    @PublicacaoId,
                    @Tipo,
                    @PublicacaoStatus,
                    @AprovacaoStatus,
                    @Titulo,
                    @Subtitulo,
                    @Conteudo,
                    @RequerConfirmacaoLeitura,
                    @PermiteComentarios,
                    @PermiteCurtidas,
                    @Fixada,
                    @PermiteDownload,
                    @OcultarNoFeed,
                    @Pasta,
                    @PublicacaoOficial,
                    @ProcessadoEnvioNotificacaoEmail,
                    @DataAgendamentoPublicacao,
                    @DataPublicacao,
                    @DataValidade,
                    @OrgId,
                    @DataCriacao,
                    @DataAlteracao,
                    @CodigoInternoCriacao,
                    @CodigoInternoAlteracao,
                    @AutoriaTipo
                )";

            await connection.ExecuteAsync(sqlPublicacao, new
            {
                PublicacaoId = publicacaoId,
                request.Tipo,
                PublicacaoStatus = publicacaoStatus,
                AprovacaoStatus = aprovacaoStatus,
                request.Titulo,
                Subtitulo = request.Subtitulo,
                request.Conteudo,
                RequerConfirmacaoLeitura = request.ConfiguracaoInteracao?.RequerConfirmacaoLeitura ?? false,
                PermiteComentarios = request.ConfiguracaoInteracao?.PermiteComentarios ?? false,
                PermiteCurtidas = request.ConfiguracaoInteracao?.PermiteCurtidas ?? false,
                Fixada = request.ConfiguracaoInteracao?.Fixada ?? false,
                PermiteDownload = string.Equals(request.Tipo, "documento", StringComparison.OrdinalIgnoreCase) ? (request.ConfiguracaoInteracao?.PermiteDownload ?? false) : false,
                OcultarNoFeed = request.ConfiguracaoInteracao?.OcultarNoFeed ?? false,
                Pasta = string.IsNullOrWhiteSpace(request.Pasta) ? null : request.Pasta.Trim(),
                PublicacaoOficial = publicacaoOficial,
                ProcessadoEnvioNotificacaoEmail = false,
                request.DataAgendamentoPublicacao,
                DataPublicacao = dataPublicacao,
                DataValidade = request.DataValidadePublicacao,
                OrgId = orgId,
                DataCriacao = DateTime.Now,
                DataAlteracao = DateTime.Now,
                CodigoInternoCriacao = codigoInternoColaboradorCriacao,
                CodigoInternoAlteracao = codigoInternoColaboradorCriacao,
                AutoriaTipo = string.Equals(request.AutoriaTipo, "alternativo", StringComparison.OrdinalIgnoreCase) ? "alternativo" : "pessoal"
            });

            await InserirRelacionamentosPublicacaoAsync(connection, publicacaoId, orgId, request.Tipo, request.Labels, request.Tags, request.Grupos, request.ComunidadeId, request.Anexos);

            await SincronizarSugestoesPastasPorPublicacoesAtivasAsync(orgId);

            return publicacaoId;
        }

        public async Task SincronizarSugestoesPastasPorPublicacoesAtivasAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var agora = DateTime.UtcNow;
            await connection.ExecuteAsync(
                "DELETE FROM tb_mkt_publicacao_pasta_sugestao WHERE tb_org_id = @OrgId",
                new { OrgId = orgId });

            const string sqlInsert = @"
                INSERT INTO tb_mkt_publicacao_pasta_sugestao (id, tb_org_id, nome, data_atualizacao)
                SELECT UUID(), @OrgId, x.nome_pasta, @Agora
                FROM (
                    SELECT DISTINCT TRIM(p.pasta) AS nome_pasta
                    FROM tb_mkt_publicacao p
                    WHERE p.tb_org_id = @OrgId
                      AND p.publicacao_status = 'ativa'
                      AND p.pasta IS NOT NULL
                      AND CHAR_LENGTH(TRIM(p.pasta)) > 0
                ) x";

            await connection.ExecuteAsync(sqlInsert, new { OrgId = orgId, Agora = agora });
        }

        public async Task<List<string>> ObterNomesPastasSugestaoAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            const string sql = @"
                SELECT nome AS Nome
                FROM tb_mkt_publicacao_pasta_sugestao
                WHERE tb_org_id = @OrgId
                ORDER BY nome";
            var rows = await connection.QueryAsync<string>(sql, new { OrgId = orgId });
            return rows.ToList();
        }

        public async Task<PublicacaoDetalheDTO> ObterPublicacaoPorIdAsync(Guid publicacaoId, string codigoInternoColaborador, int orgId)
        {
            if (string.IsNullOrWhiteSpace(codigoInternoColaborador))
                throw new ArgumentException("Código interno do colaborador é obrigatório para obter publicação.");
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT
                    p.id AS Id,
                    p.tipo AS Tipo,
                    p.titulo AS Titulo,
                    p.subtitulo AS Subtitulo,
                    p.conteudo AS Conteudo,
                    p.requer_confirmacao_leitura AS RequerConfirmacaoLeitura,
                    p.permite_comentarios AS PermiteComentarios,
                    p.permite_curtidas AS PermiteCurtidas,
                    p.publicacao_fixada_pelo_responsavel AS Fixada,
                    p.permite_download AS PermiteDownload,
                    p.ocultar_no_feed AS OcultarNoFeed,
                    p.pasta AS Pasta,
                    p.data_agendamento_publicacao AS DataAgendamentoPublicacao,
                    p.data_publicacao AS DataPublicacao,
                    p.data_validade AS DataValidade,
                    p.publicacao_status AS PublicacaoStatus,
                    p.aprovacao_status AS AprovacaoStatus,
                    p.codigo_interno_colaborador_criacao AS CodigoInternoColaboradorCriacao,
                    p.autoria_tipo AS AutoriaTipo,
                    oc.nome_autor_alternativo AS NomeAutorAlternativo,
                    oc.url_foto_alternativa AS UrlFotoAlternativa,
                    autor.nome_completo AS NomeAutor,
                    uAutor.email AS EmailAutor,
                    dAutor.departamento AS DepartamentoAutor,
                    coAutor.cod_departamento AS CodDepartamentoAutor,
                    p.codigo_interno_colaborador_aprovador AS CodigoInternoColaboradorAprovador,
                    aprov.nome_completo AS NomeAprovador,
                    p.motivo_rejeicao AS MotivoRejeicao,
                    tiAutor.path AS AutorImagemPath
                FROM tb_mkt_publicacao p
                LEFT JOIN tb_mkt_org_config oc ON oc.tb_org_id = p.tb_org_id
                INNER JOIN tb_colaborador autor
                    ON p.codigo_interno_colaborador_criacao = autor.codigo_interno_colaborador
                LEFT JOIN tb_imagem tiAutor ON autor.imagem_id = tiAutor.id
                LEFT JOIN tb_colaborador_org coAutor
                    ON autor.codigo_interno_colaborador = coAutor.codigo_interno_colaborador
                    AND coAutor.tb_org_id = @OrgId AND coAutor.ativo = 1 AND coAutor.cod_diretoria <> 'BANCO TALENTOS'
                LEFT JOIN tb_usuario uAutor
                    ON autor.codigo_interno_colaborador = uAutor.codigo_interno_colaborador
                    AND uAutor.tb_org_id = @OrgId
                LEFT JOIN tb_departamento_org dAutor
                    ON coAutor.cod_departamento = dAutor.cod_departamento
                    AND dAutor.tb_org_id = @OrgId
                LEFT JOIN tb_colaborador aprov
                    ON p.codigo_interno_colaborador_aprovador = aprov.codigo_interno_colaborador
                LEFT JOIN tb_colaborador_org coAprov
                    ON aprov.codigo_interno_colaborador = coAprov.codigo_interno_colaborador
                    AND coAprov.tb_org_id = @OrgId AND coAprov.ativo = 1 AND coAprov.cod_diretoria <> 'BANCO TALENTOS'
                LEFT JOIN tb_usuario uAprov
                    ON aprov.codigo_interno_colaborador = uAprov.codigo_interno_colaborador
                    AND uAprov.tb_org_id = @OrgId
                WHERE p.id = @PublicacaoId
                    AND p.tb_org_id = @OrgId" + SqlNuncaIncluirPublicacaoExcluida + @"
                    AND (
                        NOT EXISTS (SELECT 1 FROM tb_mkt_publicaco_grupo pg2 WHERE pg2.tb_mkt_publicacao_id = p.id)
                        OR EXISTS (SELECT 1 FROM tb_mkt_publicaco_grupo pg2 INNER JOIN tb_mkt_grupo_usuario gu ON gu.tb_mkt_grupo_id = pg2.tb_mkt_grupo_id AND gu.codigo_interno_colaborador = @CodigoInternoColaborador WHERE pg2.tb_mkt_publicacao_id = p.id)
                    )";

            var row = (await connection.QueryAsync<PublicacaoRow>(sql, new
            {
                PublicacaoId = publicacaoId,
                OrgId = orgId,
                CodigoInternoColaborador = codigoInternoColaborador
            })).FirstOrDefault();

            if (row == null) return null;

            var publicacoes = await MontarPublicacoesDetalhadasAsync(connection, new List<PublicacaoRow> { row }, new List<Guid> { publicacaoId }, codigoInternoColaborador, orgId);
            return publicacoes.FirstOrDefault();
        }

        public async Task<List<string>> ObterCodigosColaboradoresQueConfirmaramLeituraAsync(Guid publicacaoId, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT i.codigo_interno_colaborador
                FROM tb_mkt_publicacao_colaborador_interacao i
                INNER JOIN tb_mkt_publicacao p ON p.id = i.tb_mkt_publicacao_id AND p.tb_org_id = @OrgId
                WHERE i.tb_mkt_publicacao_id = @PublicacaoId
                    AND i.confirmou_leitura = 1
                ORDER BY i.data_confirmou_leitura ASC";
            return (await connection.QueryAsync<string>(sql, new { PublicacaoId = publicacaoId, OrgId = orgId })).ToList();
        }

        public async Task<bool> AtualizarPublicacaoAsync(string codigoInternoColaboradorAlteracao, int orgId, AtualizarPublicacaoRequestDTO request)
        {
            var connection = _dapperConnection.GetConnection();
            var publicacaoOficial = string.IsNullOrWhiteSpace(request.ComunidadeId);

            var sql = @"
                UPDATE tb_mkt_publicacao
                SET titulo = @Titulo,
                    subtitulo = @Subtitulo,
                    conteudo = @Conteudo,
                    tipo = @Tipo,
                    requer_confirmacao_leitura = @RequerConfirmacaoLeitura,
                    permite_comentarios = @PermiteComentarios,
                    permite_curtidas = @PermiteCurtidas,
                    publicacao_fixada_pelo_responsavel = @Fixada,
                    permite_download = @PermiteDownload,
                    ocultar_no_feed = @OcultarNoFeed,
                    pasta = @Pasta,
                    publicacao_oficial = @PublicacaoOficial,
                    data_agendamento_publicacao = @DataAgendamentoPublicacao,
                    data_validade = @DataValidade,
                    codigo_interno_colaborador_alteracao = @CodigoInternoAlteracao,
                    data_alteracao = @DataAlteracao,
                    autoria_tipo = @AutoriaTipo
                WHERE id = @PublicacaoId
                    AND tb_org_id = @OrgId";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                request.Titulo,
                Subtitulo = request.Subtitulo,
                request.Conteudo,
                request.Tipo,
                RequerConfirmacaoLeitura = request.ConfiguracaoInteracao?.RequerConfirmacaoLeitura ?? false,
                PermiteComentarios = request.ConfiguracaoInteracao?.PermiteComentarios ?? false,
                PermiteCurtidas = request.ConfiguracaoInteracao?.PermiteCurtidas ?? false,
                Fixada = request.ConfiguracaoInteracao?.Fixada ?? false,
                PermiteDownload = string.Equals(request.Tipo, "documento", StringComparison.OrdinalIgnoreCase) ? (request.ConfiguracaoInteracao?.PermiteDownload ?? false) : false,
                OcultarNoFeed = request.ConfiguracaoInteracao?.OcultarNoFeed ?? false,
                Pasta = string.IsNullOrWhiteSpace(request.Pasta) ? null : request.Pasta.Trim(),
                PublicacaoOficial = publicacaoOficial,
                request.DataAgendamentoPublicacao,
                DataValidade = request.DataValidadePublicacao,
                CodigoInternoAlteracao = codigoInternoColaboradorAlteracao,
                DataAlteracao = DateTime.Now,
                PublicacaoId = request.PublicacaoId,
                OrgId = orgId,
                AutoriaTipo = string.Equals(request.AutoriaTipo, "alternativo", StringComparison.OrdinalIgnoreCase) ? "alternativo" : "pessoal"
            });

            if (rowsAffected == 0) return false;

            await RemoverRelacionamentosPublicacaoLabelsGruposAsync(connection, request.PublicacaoId);
            await InserirRelacionamentosPublicacaoAsync(connection, request.PublicacaoId, orgId, request.Tipo, request.Labels, request.Tags, request.Grupos, request.ComunidadeId, null);

            await SincronizarSugestoesPastasPorPublicacoesAtivasAsync(orgId);

            return true;
        }

        public async Task<bool> DeletarPublicacaoAsync(string publicacaoId, string codigoInternoColaboradorAlteracao, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                UPDATE tb_mkt_publicacao
                SET publicacao_status = 'excluida',
                    codigo_interno_colaborador_alteracao = @CodigoInternoAlteracao,
                    data_alteracao = @DataAlteracao
                WHERE id = @PublicacaoId
                    AND tb_org_id = @OrgId";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                CodigoInternoAlteracao = codigoInternoColaboradorAlteracao,
                DataAlteracao = DateTime.Now,
                PublicacaoId = publicacaoId,
                OrgId = orgId
            });

            if (rowsAffected > 0)
                await SincronizarSugestoesPastasPorPublicacoesAtivasAsync(orgId);

            return rowsAffected > 0;
        }

        public async Task<bool> ArquivarPublicacaoAsync(string publicacaoId, string codigoInternoColaboradorAlteracao, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                UPDATE tb_mkt_publicacao
                SET publicacao_status = 'arquivada',
                    codigo_interno_colaborador_alteracao = @CodigoInternoAlteracao,
                    data_alteracao = @DataAlteracao
                WHERE id = @PublicacaoId
                    AND tb_org_id = @OrgId";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                CodigoInternoAlteracao = codigoInternoColaboradorAlteracao,
                DataAlteracao = DateTime.Now,
                PublicacaoId = publicacaoId,
                OrgId = orgId
            });

            if (rowsAffected > 0)
                await SincronizarSugestoesPastasPorPublicacoesAtivasAsync(orgId);

            return rowsAffected > 0;
        }

        public async Task<bool> AtualizarOcultarNoFeedAsync(Guid publicacaoId, string codigoInternoColaboradorAlteracao, int orgId, bool ocultarNoFeed)
        {
            var connection = _dapperConnection.GetConnection();
            var sql = @"
                UPDATE tb_mkt_publicacao
                SET ocultar_no_feed = @OcultarNoFeed,
                    codigo_interno_colaborador_alteracao = @CodigoInternoAlteracao,
                    data_alteracao = @DataAlteracao
                WHERE id = @PublicacaoId
                    AND tb_org_id = @OrgId";
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                OcultarNoFeed = ocultarNoFeed,
                CodigoInternoAlteracao = codigoInternoColaboradorAlteracao,
                DataAlteracao = DateTime.Now,
                PublicacaoId = publicacaoId,
                OrgId = orgId
            });
            return rowsAffected > 0;
        }

        #endregion

        #region Gerencial

        //public async Task<PublicacaoGerencialResponseDTO> ObterListaPublicacaoAgendadoEAprovacaoAsync(int orgId, string codigoInternoColaborador)
        //{
        //    var connection = _dapperConnection.GetConnection();

        //    var sql = @"
        //        SELECT
        //            p.id AS Id,
        //            p.tipo AS Tipo,
        //            p.titulo AS Titulo,
        //            p.conteudo AS Conteudo,
        //            p.requer_confirmacao_leitura AS RequerConfirmacaoLeitura,
        //            p.permite_comentarios AS PermiteComentarios,
        //            p.permite_curtidas AS PermiteCurtidas,
        //            p.publicacao_fixada_pelo_responsavel AS Fixada,
        //            p.permite_download AS PermiteDownload,
        //            p.data_agendamento_publicacao AS DataAgendamentoPublicacao,
        //            p.data_publicacao AS DataPublicacao,
        //            p.data_validade AS DataValidade,
        //            p.publicacao_status AS PublicacaoStatus,
        //            p.aprovacao_status AS AprovacaoStatus,
        //            p.codigo_interno_colaborador_criacao AS CodigoInternoColaboradorCriacao,
        //            autor.nome_completo AS NomeAutor,
        //            uAutor.email AS EmailAutor,
        //            dAutor.departamento AS DepartamentoAutor,
        //            coAutor.cod_departamento AS CodDepartamentoAutor,
        //            p.codigo_interno_colaborador_aprovador AS CodigoInternoColaboradorAprovador,
        //            aprov.nome_completo AS NomeAprovador,
        //            p.motivo_rejeicao AS MotivoRejeicao
        //        FROM tb_mkt_publicacao p
        //        INNER JOIN tb_colaborador autor
        //            ON p.codigo_interno_colaborador_criacao = autor.codigo_interno_colaborador
        //        LEFT JOIN tb_colaborador_org coAutor
        //            ON autor.codigo_interno_colaborador = coAutor.codigo_interno_colaborador
        //            AND coAutor.tb_org_id = @OrgId
        //        LEFT JOIN tb_usuario uAutor
        //            ON autor.codigo_interno_colaborador = uAutor.codigo_interno_colaborador
        //            AND uAutor.tb_org_id = @OrgId
        //        LEFT JOIN tb_departamento_org dAutor
        //            ON coAutor.cod_departamento = dAutor.cod_departamento
        //            AND dAutor.tb_org_id = @OrgId
        //        LEFT JOIN tb_colaborador aprov
        //            ON p.codigo_interno_colaborador_aprovador = aprov.codigo_interno_colaborador
        //        LEFT JOIN tb_colaborador_org coAprov
        //            ON aprov.codigo_interno_colaborador = coAprov.codigo_interno_colaborador
        //            AND coAprov.tb_org_id = @OrgId
        //        LEFT JOIN tb_usuario uAprov
        //            ON aprov.codigo_interno_colaborador = uAprov.codigo_interno_colaborador
        //            AND uAprov.tb_org_id = @OrgId
        //        WHERE p.tb_org_id = @OrgId
        //            AND (p.publicacao_status = 'agendada' OR p.aprovacao_status = 'pendente')
        //        ORDER BY p.data_criacao DESC";

        //    var publicacaoRows = (await connection.QueryAsync<PublicacaoRow>(sql, new { OrgId = orgId })).ToList();

        //    if (!publicacaoRows.Any())
        //    {
        //        return new PublicacaoGerencialResponseDTO();
        //    }

        //    var publicacaoIds = publicacaoRows.Select(p => p.Id).ToList();
        //    var publicacoes = await MontarPublicacoesDetalhadasAsync(connection, publicacaoRows, publicacaoIds, codigoInternoColaborador, orgId);

        //    return new PublicacaoGerencialResponseDTO
        //    {
        //        QuantidadeAgendados = publicacoes.Count(p => p.PublicacaoStatus == "agendada"),
        //        QuantidadePendentesAprovacao = publicacoes.Count(p => p.AprovacaoStatus == "pendente"),
        //        Publicacoes = publicacoes
        //    };
        //}

        public async Task<bool> PublicarAgoraAsync(string publicacaoId, string codigoInternoColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                UPDATE tb_mkt_publicacao
                SET publicacao_status = 'ativa',
                    data_publicacao = @DataPublicacao,
                    data_agendamento_publicacao = NULL,
                    codigo_interno_colaborador_alteracao = @CodigoInternoAlteracao,
                    data_alteracao = @DataAlteracao
                WHERE id = @PublicacaoId
                    AND tb_org_id = @OrgId
                    AND publicacao_status IN ('agendada', 'rascunho')";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                DataPublicacao = DateTime.Now,
                CodigoInternoAlteracao = codigoInternoColaborador,
                DataAlteracao = DateTime.Now,
                PublicacaoId = publicacaoId,
                OrgId = orgId
            });

            if (rowsAffected > 0)
                await SincronizarSugestoesPastasPorPublicacoesAtivasAsync(orgId);

            return rowsAffected > 0;
        }

        public async Task<bool> AprovarComunicacaoAsync(string publicacaoId, string codigoInternoColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                UPDATE tb_mkt_publicacao
                SET aprovacao_status = 'aprovado',
                    publicacao_status = CASE
                        WHEN data_agendamento_publicacao IS NOT NULL THEN 'agendada'
                        ELSE 'ativa'
                    END,
                    data_publicacao = CASE
                        WHEN data_agendamento_publicacao IS NULL THEN @DataPublicacao
                        ELSE data_publicacao
                    END,
                    codigo_interno_colaborador_aprovador = @CodigoInternoAprovador,
                    data_criacao_status_aprovacao = @DataAprovacao,
                    codigo_interno_colaborador_alteracao = @CodigoInternoAlteracao,
                    data_alteracao = @DataAlteracao
                WHERE id = @PublicacaoId
                    AND tb_org_id = @OrgId
                    AND aprovacao_status = 'pendente'";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                DataPublicacao = DateTime.Now,
                CodigoInternoAprovador = codigoInternoColaborador,
                DataAprovacao = DateTime.Now,
                CodigoInternoAlteracao = codigoInternoColaborador,
                DataAlteracao = DateTime.Now,
                PublicacaoId = publicacaoId,
                OrgId = orgId
            });

            if (rowsAffected > 0)
                await SincronizarSugestoesPastasPorPublicacoesAtivasAsync(orgId);

            return rowsAffected > 0;
        }

        public async Task<bool> RejeitarComunicacaoAsync(string publicacaoId, string codigoInternoColaborador, int orgId, string motivoRejeicao)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                UPDATE tb_mkt_publicacao
                SET aprovacao_status = 'rejeitado',
                    motivo_rejeicao = @MotivoRejeicao,
                    codigo_interno_colaborador_aprovador = @CodigoInternoAprovador,
                    data_criacao_status_aprovacao = @DataRejeicao,
                    codigo_interno_colaborador_alteracao = @CodigoInternoAlteracao,
                    data_alteracao = @DataAlteracao
                WHERE id = @PublicacaoId
                    AND tb_org_id = @OrgId
                    AND aprovacao_status = 'pendente'";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                MotivoRejeicao = motivoRejeicao,
                CodigoInternoAprovador = codigoInternoColaborador,
                DataRejeicao = DateTime.Now,
                CodigoInternoAlteracao = codigoInternoColaborador,
                DataAlteracao = DateTime.Now,
                PublicacaoId = publicacaoId,
                OrgId = orgId
            });

            return rowsAffected > 0;
        }

        public async Task<bool> InserirAnexosPublicacaoAsync(Guid publicacaoId, List<PublicacaoAnexoInputDTO> anexos)
        {
            var connection = _dapperConnection.GetConnection();

            if (anexos == null || anexos.Count == 0)
                return true;

            foreach (var anexo in anexos)
            {
                var anexoId = anexo.AnexoId ?? Guid.NewGuid();
                var sqlAnexo = @"
                        INSERT INTO tb_mkt_publicacao_anexo (id, tb_mkt_publicacao_id, nome_arquivo, url_arquivo, tamanho_bytes, tipo)
                        VALUES (@Id, @PublicacaoId, @NomeArquivo, @UrlArquivo, @TamanhoBytes, @Tipo)";

                var rows = await connection.ExecuteAsync(sqlAnexo, new
                {
                    Id = anexoId,
                    PublicacaoId = publicacaoId,
                    anexo.NomeArquivo,
                    anexo.UrlArquivo,
                    anexo.TamanhoBytes,
                    anexo.Tipo
                });

                if (rows <= 0)
                    return false;
            }

            return true;
        }

        public async Task<PublicacaoAnexoDTO> ObterAnexoPublicacaoAsync(Guid publicacaoId, Guid anexoId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT
                    a.id AS AnexoId,
                    a.nome_arquivo AS NomeArquivo,
                    a.url_arquivo AS UrlArquivo,
                    a.tamanho_bytes AS TamanhoBytes,
                    a.tipo AS Tipo
                FROM tb_mkt_publicacao_anexo a
                WHERE a.id = @AnexoId
                    AND a.tb_mkt_publicacao_id = @PublicacaoId";

            return (await connection.QueryAsync<PublicacaoAnexoDTO>(sql, new
            {
                AnexoId = anexoId,
                PublicacaoId = publicacaoId
            })).FirstOrDefault();
        }

        public async Task<bool> RemoverAnexoPublicacaoAsync(Guid publicacaoId, Guid anexoId)
        {
            var connection = _dapperConnection.GetConnection();

            var rows = await connection.ExecuteAsync(@"
                DELETE FROM tb_mkt_publicacao_anexo
                WHERE id = @AnexoId
                    AND tb_mkt_publicacao_id = @PublicacaoId", new
            {
                AnexoId = anexoId,
                PublicacaoId = publicacaoId
            });

            return rows > 0;
        }

        public async Task<Guid?> InserirComentarioAsync(Guid publicacaoId, string codigoInternoColaborador, string conteudo, Guid? comentarioPaiId)
        {
            var connection = _dapperConnection.GetConnection();
            var id = Guid.NewGuid();
            var sql = @"
                INSERT INTO tb_mkt_publicacao_comentario (id, tb_mkt_publicacao_id, conteudo, comentario_pai_id, codigo_interno_colaborador, data_criacao)
                VALUES (@Id, @PublicacaoId, @Conteudo, @ComentarioPaiId, @CodigoInternoColaborador, @DataCriacao)";
            var dataCriacao = DateTime.UtcNow;
            var rows = await connection.ExecuteAsync(sql, new
            {
                Id = id,
                PublicacaoId = publicacaoId,
                Conteudo = conteudo ?? "",
                ComentarioPaiId = comentarioPaiId.HasValue ? (Guid?)comentarioPaiId.Value : null,
                CodigoInternoColaborador = codigoInternoColaborador,
                DataCriacao = dataCriacao
            });
            return rows > 0 ? (Guid?)id : null;
        }

        public async Task<(string CodigoAutor, DateTime DataCriacao)?> ObterComentarioParaValidacaoAsync(Guid comentarioId)
        {
            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT codigo_interno_colaborador AS CodigoAutor, data_criacao AS DataCriacao
                FROM tb_mkt_publicacao_comentario
                WHERE id = @ComentarioId";
            var row = await connection.QueryFirstOrDefaultAsync<(string CodigoAutor, DateTime DataCriacao)>(sql, new { ComentarioId = comentarioId });
            return row.CodigoAutor != null ? row : ((string, DateTime)?)null;
        }

        public async Task<bool> AtualizarComentarioAsync(Guid comentarioId, string codigoInternoColaborador, string conteudo)
        {
            var connection = _dapperConnection.GetConnection();
            var sql = @"
                UPDATE tb_mkt_publicacao_comentario
                SET conteudo = @Conteudo
                WHERE id = @ComentarioId
                    AND codigo_interno_colaborador = @CodigoInternoColaborador";
            var rows = await connection.ExecuteAsync(sql, new
            {
                ComentarioId = comentarioId,
                CodigoInternoColaborador = codigoInternoColaborador,
                Conteudo = conteudo ?? ""
            });
            return rows > 0;
        }

        /// <summary>
        /// Remove o comentário (apenas se for do autor). Em cascata: remove todas as respostas e subcomentários, e suas interações (curtidas).
        /// </summary>
        public async Task<bool> RemoverComentarioAsync(Guid comentarioId, string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();
            var comentario = await ObterComentarioParaValidacaoAsync(comentarioId);
            if (comentario == null || !string.Equals(comentario.Value.CodigoAutor, codigoInternoColaborador, StringComparison.OrdinalIgnoreCase))
                return false;
            await RemoverComentarioCascataAsync(connection, comentarioId);
            return true;
        }

        /// <summary>
        /// Remove o comentário e, recursivamente, todas as respostas e subcomentários, além das curtidas de cada um.
        /// </summary>
        private async Task RemoverComentarioCascataAsync(System.Data.IDbConnection connection, Guid comentarioId)
        {
            var filhos = (await connection.QueryAsync<Guid>(@"
                SELECT id FROM tb_mkt_publicacao_comentario WHERE comentario_pai_id = @ComentarioId",
                new { ComentarioId = comentarioId })).ToList();
            foreach (var filhoId in filhos)
                await RemoverComentarioCascataAsync(connection, filhoId);
            await connection.ExecuteAsync(@"
                DELETE FROM tb_mkt_publicacao_comentario_interacao WHERE tb_mkt_publicacao_comentario_id = @ComentarioId",
                new { ComentarioId = comentarioId });
            await connection.ExecuteAsync(@"
                DELETE FROM tb_mkt_publicacao_comentario WHERE id = @ComentarioId",
                new { ComentarioId = comentarioId });
        }

        public async Task<bool> AdicionarInteracaoComentarioAsync(Guid comentarioId, string codigoInternoColaborador, string emoji)
        {
            var connection = _dapperConnection.GetConnection();
            var existe = await connection.ExecuteScalarAsync<int>(@"
                SELECT 1 FROM tb_mkt_publicacao_comentario WHERE id = @ComentarioId LIMIT 1",
                new { ComentarioId = comentarioId });
            if (existe == 0)
                return false;

            var id = Guid.NewGuid().ToString();
            var emojiVal = string.IsNullOrEmpty(emoji) ? null : (emoji.Length > 50 ? emoji.Substring(0, 50) : emoji);
            var rows = await connection.ExecuteAsync(@"
                INSERT INTO tb_mkt_publicacao_comentario_interacao (id, tb_mkt_publicacao_comentario_id, codigo_interno_colaborador, curtida_emoji, data_interacao)
                VALUES (@Id, @ComentarioId, @CodigoInternoColaborador, @CurtidaEmoji, CURRENT_TIMESTAMP)
                ON DUPLICATE KEY UPDATE curtida_emoji = @CurtidaEmoji, data_interacao = CURRENT_TIMESTAMP",
                new
                {
                    Id = id,
                    ComentarioId = comentarioId,
                    CodigoInternoColaborador = codigoInternoColaborador,
                    CurtidaEmoji = emojiVal
                });
            return rows > 0;
        }

        public async Task<bool> RemoverInteracaoComentarioAsync(Guid comentarioId, string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();
            var rows = await connection.ExecuteAsync(@"
                DELETE FROM tb_mkt_publicacao_comentario_interacao
                WHERE tb_mkt_publicacao_comentario_id = @ComentarioId
                    AND codigo_interno_colaborador = @CodigoInternoColaborador",
                new { ComentarioId = comentarioId, CodigoInternoColaborador = codigoInternoColaborador });
            return rows > 0;
        }

        public async Task<bool> AdicionarInteracaoPublicacaoAsync(Guid publicacaoId, string codigoInternoColaborador, string emoji)
        {
            var connection = _dapperConnection.GetConnection();
            var existe = await connection.ExecuteScalarAsync<int>(@"
                SELECT 1 FROM tb_mkt_publicacao WHERE id = @PublicacaoId LIMIT 1",
                new { PublicacaoId = publicacaoId });
            if (existe == 0)
                return false;

            var emojiVal = string.IsNullOrEmpty(emoji) ? null : (emoji.Length > 50 ? emoji.Substring(0, 50) : emoji);
            var sql = @"
                INSERT INTO tb_mkt_publicacao_colaborador_interacao
                (id, tb_mkt_publicacao_id, codigo_interno_colaborador, curtida_emoji)
                VALUES (@Id, @PublicacaoId, @CodigoInternoColaborador, @CurtidaEmoji)
                ON DUPLICATE KEY UPDATE curtida_emoji = @CurtidaEmoji";
            var rows = await connection.ExecuteAsync(sql, new
            {
                Id = Guid.NewGuid(),
                PublicacaoId = publicacaoId,
                CodigoInternoColaborador = codigoInternoColaborador,
                CurtidaEmoji = emojiVal
            });
            return rows > 0;
        }

        public async Task<bool> RemoverInteracaoPublicacaoAsync(Guid publicacaoId, string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();
            var rows = await connection.ExecuteAsync(@"
                UPDATE tb_mkt_publicacao_colaborador_interacao
                SET curtida_emoji = NULL
                WHERE tb_mkt_publicacao_id = @PublicacaoId
                    AND codigo_interno_colaborador = @CodigoInternoColaborador",
                new { PublicacaoId = publicacaoId, CodigoInternoColaborador = codigoInternoColaborador });
            return rows > 0;
        }

        public async Task<List<string>> ObterGruposExistentesAsync(List<string> grupoIds, int orgId)
        {
            if (grupoIds == null || grupoIds.Count == 0)
                return new List<string>();

            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT CAST(g.id AS CHAR(36)) AS Id
                FROM tb_mkt_grupo g
                WHERE g.tb_org_id = @OrgId
                    AND g.id IN @GrupoIds";

            return (await connection.QueryAsync<string>(sql, new
            {
                OrgId = orgId,
                GrupoIds = grupoIds
            })).ToList();
        }

        public async Task<bool> UsuarioEstaEmAlgumGrupoQuePermitePublicarSemAprovacaoAsync(string codigoInternoColaborador, int orgId)
        {
            if (string.IsNullOrWhiteSpace(codigoInternoColaborador))
                return false;

            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT 1
                FROM tb_mkt_grupo_usuario gu
                INNER JOIN tb_mkt_grupo g ON g.id = gu.tb_mkt_grupo_id AND g.tb_org_id = @OrgId
                WHERE gu.codigo_interno_colaborador = @CodigoInternoColaborador
                    AND g.publicacao_oficial_requer_aprovacao = 0
                LIMIT 1";
            var existe = await connection.QueryFirstOrDefaultAsync<int>(sql, new { CodigoInternoColaborador = codigoInternoColaborador, OrgId = orgId });
            return existe == 1;
        }

        #endregion

        #region Helpers

        private async Task<List<PublicacaoDetalheDTO>> MontarPublicacoesDetalhadasAsync(
            System.Data.IDbConnection connection,
            List<PublicacaoRow> publicacaoRows,
            List<Guid> publicacaoIds,
            string codigoInternoColaborador,
            int orgId,
            List<LabelRow> labelRows = null,
            List<TagRow> tagRows = null)
        {
            var labels = labelRows ?? await ObterLabelsPorPublicacoesAsync(connection, publicacaoIds, orgId);
            var tags = tagRows ?? await ObterTagsPorPublicacoesAsync(connection, publicacaoIds, orgId);
            var grupos = await ObterGruposPorPublicacoesAsync(connection, publicacaoIds, orgId);
            var comunidades = await ObterComunidadeIdPorPublicacoesAsync(connection, publicacaoIds);
            var anexos = await ObterAnexosPorPublicacoesAsync(connection, publicacaoIds);
            var interacoes = await ObterInteracoesPorPublicacoesAsync(connection, publicacaoIds, codigoInternoColaborador);
            var interacoesEmojiPorPublicacao = await ObterInteracoesEmojiPorPublicacoesAsync(connection, publicacaoIds);
            var comentarios = await ObterComentariosPorPublicacoesAsync(connection, publicacaoIds, orgId);
            var comentarioIds = comentarios.Select(c => c.ComentarioId).Distinct().ToList();
            var interacoesPorComentario = comentarioIds.Any()
                ? await ObterInteracoesPorComentariosAsync(connection, comentarioIds)
                : new Dictionary<Guid, List<InteracaoComentarioEmojiCountDTO>>();
            var interacaoUsuarioPorComentario = comentarioIds.Any()
                ? await ObterInteracaoComentarioPorUsuarioAsync(connection, comentarioIds, codigoInternoColaborador)
                : new Dictionary<Guid, ComentarioInteracaoDTO>();
            var analiticos = await ObterAnaliticosPorPublicacoesAsync(connection, publicacaoIds);

            var publicacoes = new List<PublicacaoDetalheDTO>();

            foreach (var row in publicacaoRows)
            {
                var pub = new PublicacaoDetalheDTO
                {
                    PublicacaoId = row.Id,
                    Tipo = row.Tipo,
                    Titulo = row.Titulo,
                    Subtitulo = row.Subtitulo,
                    Conteudo = row.Conteudo,
                    RequerConfirmacaoLeitura = row.RequerConfirmacaoLeitura,
                    PermiteComentarios = row.PermiteComentarios,
                    PermiteCurtidas = row.PermiteCurtidas,
                    Fixada = row.Fixada,
                    PermiteDownload = row.PermiteDownload,
                    OcultarNoFeed = row.OcultarNoFeed,
                    Pasta = row.Pasta,
                    DataAgendamentoPublicacao = row.DataAgendamentoPublicacao,
                    DataPublicacao = row.DataPublicacao,
                    DataValidadePublicacao = row.DataValidade,
                    PublicacaoStatus = row.PublicacaoStatus,
                    AprovacaoStatus = row.AprovacaoStatus,
                    MotivoRejeicao = row.MotivoRejeicao,
                    Gerencial = new PublicacaoGerencialDTO
                    {
                        PublicacaoStatus = row.PublicacaoStatus,
                        AprovacaoStatus = row.AprovacaoStatus
                    },
                    Autor = new ColaboradorResumoDTO
                    {
                        CodigoColaboradorInterno = row.CodigoInternoColaboradorCriacao,
                        NomeCompleto = row.NomeAutor,
                        Email = row.EmailAutor,
                        NomeAutorAlternativo = string.Equals(row.AutoriaTipo, "alternativo", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(row.NomeAutorAlternativo)
                            ? row.NomeAutorAlternativo
                            : null,
                        UrlFotoAlternativa = string.Equals(row.AutoriaTipo, "alternativo", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(row.UrlFotoAlternativa)
                            ? row.UrlFotoAlternativa
                            : null,
                        CodDepartamento = row.CodDepartamentoAutor,
                        Departamento = row.DepartamentoAutor,
                        UrlFoto = !string.IsNullOrWhiteSpace(row.AutorImagemPath)
                            ? VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + row.AutorImagemPath
                            : null
                    },
                    Labels = labels.Where(l => l.PublicacaoId == row.Id)
                        .Select(l => new PublicacaoLabelItemDTO { Id = l.LabelId, Nome = l.LabelNome, Tipo = l.LabelTipo })
                        .ToList(),
                    Tags = string.Equals(row.Tipo, "documento", StringComparison.OrdinalIgnoreCase)
                        ? tags.Where(t => t.PublicacaoId == row.Id).Select(t => new PublicacaoTagItemDTO { Id = t.TagId, Nome = t.TagNome }).ToList()
                        : new List<PublicacaoTagItemDTO>(),
                    Grupos = grupos.Where(g => g.PublicacaoId == row.Id).Select(g => g.GrupoNome).ToList(),
                    ComunidadeId = comunidades.FirstOrDefault(c => c.PublicacaoId == row.Id)?.ComunidadeId?.ToString(),
                    ComunidadeNome = comunidades.FirstOrDefault(c => c.PublicacaoId == row.Id)?.ComunidadeNome,
                    Anexos = anexos.Where(a => a.PublicacaoId == row.Id).Select(a => new PublicacaoAnexoDTO
                    {
                        AnexoId = a.Id,
                        NomeArquivo = a.NomeArquivo,
                        UrlArquivo = a.UrlArquivo,
                        TamanhoBytes = a.TamanhoBytes,
                        Tipo = a.Tipo
                    }).ToList()
                };

                if (!string.IsNullOrWhiteSpace(row.CodigoInternoColaboradorAprovador))
                {
                    pub.Aprovador = new ColaboradorResumoDTO
                    {
                        CodigoColaboradorInterno = row.CodigoInternoColaboradorAprovador,
                        NomeCompleto = row.NomeAprovador
                    };
                }

                var interacao = interacoes.FirstOrDefault(i => i.PublicacaoId == row.Id);
                if (interacao != null)
                {
                    pub.Interacao = new PublicacaoInteracaoDTO
                    {
                        CodigoInterno = interacao.CodigoInterno,
                        DataPrimeiraEntrega = interacao.DataPrimeiraEntrega,
                        Visualizado = interacao.Visualizado,
                        DataVisualizado = interacao.DataVisualizado,
                        ConfirmouLeitura = interacao.ConfirmouLeitura,
                        DataConfirmouLeitura = interacao.DataConfirmouLeitura,
                        CurtidaEmoji = interacao.CurtidaEmoji
                    };
                }

                pub.InteracoesPorEmoji = interacoesEmojiPorPublicacao.TryGetValue(row.Id, out var emojisPub) ? emojisPub : new List<InteracaoComentarioEmojiCountDTO>();

                var analitico = analiticos.FirstOrDefault(a => a.PublicacaoId == row.Id);
                if (analitico != null)
                {
                    pub.Analitico = new PublicacaoAnaliticoDTO
                    {
                        QuantidadeVisualizacao = analitico.QuantidadeVisualizacao,
                        QuantidadeCurtida = analitico.QuantidadeCurtida,
                        QuantidadeComentarios = analitico.QuantidadeComentarios,
                        QuantidadeConfirmacoesLeitura = analitico.QuantidadeConfirmacoesLeitura
                    };
                }

                var comentariosPub = comentarios
                    .Where(c => c.PublicacaoId == row.Id)
                    .GroupBy(c => c.ComentarioId)
                    .Select(g => g.First())
                    .ToList();
                pub.Comentarios = MontarComentariosHierarquia(comentariosPub, interacoesPorComentario, interacaoUsuarioPorComentario);

                publicacoes.Add(pub);
            }

            return publicacoes;
        }

        private List<PublicacaoComentarioDTO> MontarComentariosHierarquia(
            List<ComentarioRow> comentarioRows,
            Dictionary<Guid, List<InteracaoComentarioEmojiCountDTO>> interacoesPorComentario,
            Dictionary<Guid, ComentarioInteracaoDTO> interacaoUsuarioPorComentario)
        {
            var raizes = comentarioRows.Where(c => !c.ComentarioPaiId.HasValue).ToList();
            var respostas = comentarioRows.Where(c => c.ComentarioPaiId.HasValue).ToList();

            return raizes.Select(r => new PublicacaoComentarioDTO
            {
                ComentarioId = r.ComentarioId,
                Conteudo = r.Conteudo,
                DataCriacao = r.DataCriacao,
                Autor = new ColaboradorResumoDTO
                {
                    CodigoColaboradorInterno = r.CodigoInternoColaborador,
                    NomeCompleto = r.NomeAutor,
                    Email = r.EmailAutor,
                    UrlFoto = !string.IsNullOrWhiteSpace(r.AutorImagemPath)
                        ? VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + r.AutorImagemPath
                        : null
                },
                Interacao = interacaoUsuarioPorComentario.TryGetValue(r.ComentarioId, out var minhaInteracaoRaiz) ? minhaInteracaoRaiz : null,
                InteracoesPorEmoji = interacoesPorComentario.TryGetValue(r.ComentarioId, out var interacoesRaiz) ? interacoesRaiz : new List<InteracaoComentarioEmojiCountDTO>(),
                RespostaComentarios = respostas.Where(resp => resp.ComentarioPaiId == r.ComentarioId)
                    .Select(resp => new PublicacaoComentarioRespostaDTO
                    {
                        ComentarioId = resp.ComentarioId,
                        Conteudo = resp.Conteudo,
                        DataCriacao = resp.DataCriacao,
                        Autor = new ColaboradorResumoDTO
                        {
                            CodigoColaboradorInterno = resp.CodigoInternoColaborador,
                            NomeCompleto = resp.NomeAutor,
                            Email = resp.EmailAutor,
                            UrlFoto = !string.IsNullOrWhiteSpace(resp.AutorImagemPath)
                                ? VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + resp.AutorImagemPath
                                : null
                        },
                        Interacao = interacaoUsuarioPorComentario.TryGetValue(resp.ComentarioId, out var minhaInteracaoResp) ? minhaInteracaoResp : null,
                        InteracoesPorEmoji = interacoesPorComentario.TryGetValue(resp.ComentarioId, out var interacoesResp) ? interacoesResp : new List<InteracaoComentarioEmojiCountDTO>()
                    }).ToList()
            }).ToList();
        }

        private async Task<Dictionary<Guid, List<InteracaoComentarioEmojiCountDTO>>> ObterInteracoesEmojiPorPublicacoesAsync(System.Data.IDbConnection connection, List<Guid> publicacaoIds)
        {
            if (publicacaoIds == null || !publicacaoIds.Any())
                return new Dictionary<Guid, List<InteracaoComentarioEmojiCountDTO>>();

            var sql = @"
                SELECT
                    i.tb_mkt_publicacao_id AS PublicacaoId,
                    i.curtida_emoji AS Emoji,
                    COUNT(*) AS Quantidade
                FROM tb_mkt_publicacao_colaborador_interacao i
                WHERE i.tb_mkt_publicacao_id IN @PublicacaoIds
                    AND i.curtida_emoji IS NOT NULL AND TRIM(i.curtida_emoji) != ''
                GROUP BY i.tb_mkt_publicacao_id, i.curtida_emoji";
            var rows = (await connection.QueryAsync<InteracaoPublicacaoEmojiRow>(sql, new { PublicacaoIds = publicacaoIds })).ToList();
            var dict = new Dictionary<Guid, List<InteracaoComentarioEmojiCountDTO>>();
            foreach (var g in rows.GroupBy(x => x.PublicacaoId))
            {
                dict[g.Key] = g.Where(x => !string.IsNullOrWhiteSpace(x.Emoji)).Select(x => new InteracaoComentarioEmojiCountDTO { Emoji = x.Emoji, Count = x.Quantidade }).ToList();
            }
            return dict;
        }

        private async Task<Dictionary<Guid, List<InteracaoComentarioEmojiCountDTO>>> ObterInteracoesPorComentariosAsync(System.Data.IDbConnection connection, List<Guid> comentarioIds)
        {
            if (comentarioIds == null || !comentarioIds.Any())
                return new Dictionary<Guid, List<InteracaoComentarioEmojiCountDTO>>();

            var sql = @"
                SELECT
                    tb_mkt_publicacao_comentario_id AS ComentarioId,
                    curtida_emoji AS Emoji,
                    COUNT(*) AS Quantidade
                FROM tb_mkt_publicacao_comentario_interacao
                WHERE tb_mkt_publicacao_comentario_id IN @ComentarioIds
                    AND curtida_emoji IS NOT NULL AND TRIM(curtida_emoji) != ''
                GROUP BY tb_mkt_publicacao_comentario_id, curtida_emoji";
            var rows = (await connection.QueryAsync<InteracaoComentarioRow>(sql, new { ComentarioIds = comentarioIds })).ToList();
            var dict = new Dictionary<Guid, List<InteracaoComentarioEmojiCountDTO>>();
            foreach (var g in rows.GroupBy(x => x.ComentarioId))
            {
                dict[g.Key] = g.Where(x => !string.IsNullOrWhiteSpace(x.Emoji)).Select(x => new InteracaoComentarioEmojiCountDTO { Emoji = x.Emoji, Count = x.Quantidade }).ToList();
            }
            return dict;
        }

        private async Task<Dictionary<Guid, ComentarioInteracaoDTO>> ObterInteracaoComentarioPorUsuarioAsync(System.Data.IDbConnection connection, List<Guid> comentarioIds, string codigoInternoColaborador)
        {
            if (comentarioIds == null || !comentarioIds.Any() || string.IsNullOrWhiteSpace(codigoInternoColaborador))
                return new Dictionary<Guid, ComentarioInteracaoDTO>();

            var sql = @"
                SELECT
                    tb_mkt_publicacao_comentario_id AS ComentarioId,
                    codigo_interno_colaborador AS CodigoInterno,
                    data_interacao AS DataInteracao,
                    curtida_emoji AS CurtidaEmoji
                FROM tb_mkt_publicacao_comentario_interacao
                WHERE tb_mkt_publicacao_comentario_id IN @ComentarioIds
                    AND codigo_interno_colaborador = @CodigoInternoColaborador";
            var rows = (await connection.QueryAsync<InteracaoComentarioUsuarioRow>(sql, new { ComentarioIds = comentarioIds, CodigoInternoColaborador = codigoInternoColaborador })).ToList();
            return rows.Where(x => !string.IsNullOrWhiteSpace(x.CurtidaEmoji)).ToDictionary(x => x.ComentarioId, x => new ComentarioInteracaoDTO
            {
                CodigoInterno = x.CodigoInterno,
                DataCriacao = x.DataInteracao,
                Emoji = x.CurtidaEmoji
            });
        }

        private async Task<List<LabelRow>> ObterLabelsPorPublicacoesAsync(System.Data.IDbConnection connection, List<Guid> publicacaoIds, int orgId)
        {
            var sql = @"
                SELECT
                    pl.tb_mkt_publicacao_id AS PublicacaoId,
                    l.id AS LabelId,
                    l.nome AS LabelNome,
                    l.tipo AS LabelTipo
                FROM tb_mkt_publicacao_label pl
                INNER JOIN tb_mkt_label l ON pl.tb_mkt_label_id = l.id AND l.tb_org_id = @OrgId
                WHERE pl.tb_mkt_publicacao_id IN @PublicacaoIds";

            return (await connection.QueryAsync<LabelRow>(sql, new { PublicacaoIds = publicacaoIds, OrgId = orgId })).ToList();
        }

        private async Task<List<TagRow>> ObterTagsPorPublicacoesAsync(System.Data.IDbConnection connection, List<Guid> publicacaoIds, int orgId)
        {
            var sql = @"
                SELECT
                    pt.tb_mkt_publicacao_id AS PublicacaoId,
                    t.id AS TagId,
                    t.nome AS TagNome
                FROM tb_mkt_publicacao_tag pt
                INNER JOIN tb_mkt_tag t ON pt.tb_mkt_tag_id = t.id AND t.tb_org_id = @OrgId
                WHERE pt.tb_mkt_publicacao_id IN @PublicacaoIds";

            return (await connection.QueryAsync<TagRow>(sql, new { PublicacaoIds = publicacaoIds, OrgId = orgId })).ToList();
        }

        private static List<PublicacaoLabelResumoDTO> MontarResumoLabelsFromLabelRows(List<LabelRow> labelRows)
        {
            if (labelRows == null || !labelRows.Any())
                return new List<PublicacaoLabelResumoDTO>();

            return labelRows
                .GroupBy(l => l.LabelId)
                .Select(g =>
                {
                    var first = g.First();
                    return new PublicacaoLabelResumoDTO
                    {
                        Id = g.Key,
                        Nome = first.LabelNome,
                        Tipo = first.LabelTipo,
                        QuantidadeDocumentos = g.Select(x => x.PublicacaoId).Distinct().Count()
                    };
                })
                .OrderBy(l => l.Nome)
                .ToList();
        }

        private static List<PublicacaoTagResumoDTO> MontarResumoTagsFromTagRows(List<TagRow> tagRows)
        {
            if (tagRows == null || !tagRows.Any())
                return new List<PublicacaoTagResumoDTO>();

            return tagRows
                .GroupBy(t => t.TagId)
                .Select(g =>
                {
                    var first = g.First();
                    return new PublicacaoTagResumoDTO
                    {
                        Id = g.Key,
                        Nome = first.TagNome,
                        QuantidadeDocumentos = g.Select(x => x.PublicacaoId).Distinct().Count()
                    };
                })
                .OrderBy(t => t.Nome)
                .ToList();
        }

        private static List<PublicacaoPastaResumoDTO> MontarResumoPastasFromPublicacoes(List<PublicacaoDetalheDTO> publicacoes)
        {
            if (publicacoes == null || !publicacoes.Any())
                return new List<PublicacaoPastaResumoDTO>();

            return publicacoes
                .Where(p => !string.IsNullOrWhiteSpace(p.Pasta))
                .GroupBy(p => p.Pasta.Trim())
                .Select(g => new PublicacaoPastaResumoDTO
                {
                    Nome = g.Key,
                    QuantidadeDocumentos = g.Count()
                })
                .OrderBy(x => x.Nome)
                .ToList();
        }

        private static List<string> MontarFiltrosFromPublicacoes(List<PublicacaoDetalheDTO> publicacoes)
        {
            if (publicacoes == null || !publicacoes.Any())
                return new List<string>();

            var set = new HashSet<string>();
            foreach (var p in publicacoes)
            {
                if (string.IsNullOrWhiteSpace(p.Pasta))
                    continue;
                var pasta = p.Pasta.Trim();
                if (p.Labels != null)
                {
                    foreach (var l in p.Labels)
                    {
                        if (string.IsNullOrWhiteSpace(l?.Nome))
                            continue;
                        set.Add($"{pasta}/{l.Nome}");
                    }
                }
                if (string.Equals(p.Tipo, "documento", StringComparison.OrdinalIgnoreCase) && p.Tags != null)
                {
                    foreach (var t in p.Tags)
                    {
                        if (string.IsNullOrWhiteSpace(t?.Nome))
                            continue;
                        set.Add($"{pasta}/{t.Nome}");
                    }
                }
            }

            return set.OrderBy(x => x).ToList();
        }

        private async Task<List<GrupoRow>> ObterGruposPorPublicacoesAsync(System.Data.IDbConnection connection, List<Guid> publicacaoIds, int orgId)
        {
            var sql = @"
                SELECT
                    pg.tb_mkt_publicacao_id AS PublicacaoId,
                    g.nome AS GrupoNome
                FROM tb_mkt_publicaco_grupo pg
                INNER JOIN tb_mkt_grupo g ON pg.tb_mkt_grupo_id = g.id AND g.tb_org_id = @OrgId
                WHERE pg.tb_mkt_publicacao_id IN @PublicacaoIds";

            return (await connection.QueryAsync<GrupoRow>(sql, new { PublicacaoIds = publicacaoIds, OrgId = orgId })).ToList();
        }

        private async Task<List<ComunidadeRow>> ObterComunidadeIdPorPublicacoesAsync(System.Data.IDbConnection connection, List<Guid> publicacaoIds)
        {
            if (publicacaoIds == null || !publicacaoIds.Any())
                return new List<ComunidadeRow>();
            var sql = @"
                SELECT cp.tb_mkt_publicacao_id AS PublicacaoId, cp.tb_mkt_comunidade_id AS ComunidadeId, c.nome AS ComunidadeNome
                FROM tb_mkt_comunidade_publicacao cp
                INNER JOIN tb_mkt_comunidade c ON c.id = cp.tb_mkt_comunidade_id
                WHERE cp.tb_mkt_publicacao_id IN @PublicacaoIds";
            return (await connection.QueryAsync<ComunidadeRow>(sql, new { PublicacaoIds = publicacaoIds })).ToList();
        }

        private async Task<List<AnexoRow>> ObterAnexosPorPublicacoesAsync(System.Data.IDbConnection connection, List<Guid> publicacaoIds)
        {
            var sql = @"
                SELECT
                    a.id AS Id,
                    a.tb_mkt_publicacao_id AS PublicacaoId,
                    a.nome_arquivo AS NomeArquivo,
                    a.url_arquivo AS UrlArquivo,
                    a.tamanho_bytes AS TamanhoBytes,
                    a.tipo AS Tipo
                FROM tb_mkt_publicacao_anexo a
                WHERE a.tb_mkt_publicacao_id IN @PublicacaoIds";

            return (await connection.QueryAsync<AnexoRow>(sql, new { PublicacaoIds = publicacaoIds })).ToList();
        }

        private async Task<List<InteracaoRow>> ObterInteracoesPorPublicacoesAsync(System.Data.IDbConnection connection, List<Guid> publicacaoIds, string codigoInternoColaborador)
        {
            var sql = @"
                SELECT
                    i.tb_mkt_publicacao_id AS PublicacaoId,
                    i.codigo_interno_colaborador AS CodigoInterno,
                    i.data_primeira_entrega AS DataPrimeiraEntrega,
                    i.visualizado AS Visualizado,
                    i.data_visualizado AS DataVisualizado,
                    i.confirmou_leitura AS ConfirmouLeitura,
                    i.data_confirmou_leitura AS DataConfirmouLeitura,
                    i.curtida_emoji AS CurtidaEmoji
                FROM tb_mkt_publicacao_colaborador_interacao i
                WHERE i.tb_mkt_publicacao_id IN @PublicacaoIds
                    AND i.codigo_interno_colaborador = @CodigoInternoColaborador";

            return (await connection.QueryAsync<InteracaoRow>(sql, new
            {
                PublicacaoIds = publicacaoIds,
                CodigoInternoColaborador = codigoInternoColaborador
            })).ToList();
        }

        private async Task<List<ComentarioRow>> ObterComentariosPorPublicacoesAsync(System.Data.IDbConnection connection, List<Guid> publicacaoIds, int orgId)
        {
            var sql = @"
                SELECT
                    c.id AS ComentarioId,
                    c.tb_mkt_publicacao_id AS PublicacaoId,
                    c.conteudo AS Conteudo,
                    c.data_criacao AS DataCriacao,
                    NULLIF(c.comentario_pai_id, '') AS ComentarioPaiId,
                    c.codigo_interno_colaborador AS CodigoInternoColaborador,
                    col.nome_completo AS NomeAutor,
                    u.email AS EmailAutor,
                    ti.path AS AutorImagemPath
                FROM tb_mkt_publicacao_comentario c
                INNER JOIN tb_colaborador col
                    ON c.codigo_interno_colaborador = col.codigo_interno_colaborador
                LEFT JOIN tb_imagem ti ON col.imagem_id = ti.id
                LEFT JOIN tb_usuario u
                    ON col.codigo_interno_colaborador = u.codigo_interno_colaborador AND u.tb_org_id = @OrgId
                WHERE c.tb_mkt_publicacao_id IN @PublicacaoIds
                ORDER BY c.data_criacao ASC";

            return (await connection.QueryAsync<ComentarioRow>(sql, new { PublicacaoIds = publicacaoIds, OrgId = orgId })).ToList();
        }

        private async Task<List<AnaliticoRow>> ObterAnaliticosPorPublicacoesAsync(System.Data.IDbConnection connection, List<Guid> publicacaoIds)
        {
            var sql = @"
                SELECT
                    i.tb_mkt_publicacao_id AS PublicacaoId,
                    SUM(CASE WHEN i.visualizado = 1 THEN 1 ELSE 0 END) AS QuantidadeVisualizacao,
                    SUM(CASE WHEN i.curtida_emoji IS NOT NULL AND i.curtida_emoji != '' THEN 1 ELSE 0 END) AS QuantidadeCurtida,
                    SUM(CASE WHEN i.confirmou_leitura = 1 THEN 1 ELSE 0 END) AS QuantidadeConfirmacoesLeitura
                FROM tb_mkt_publicacao_colaborador_interacao i
                WHERE i.tb_mkt_publicacao_id IN @PublicacaoIds
                GROUP BY i.tb_mkt_publicacao_id";

            var analiticos = (await connection.QueryAsync<AnaliticoRow>(sql, new { PublicacaoIds = publicacaoIds })).ToList();

            var sqlComentarios = @"
                SELECT
                    c.tb_mkt_publicacao_id AS PublicacaoId,
                    COUNT(*) AS QuantidadeComentarios
                FROM tb_mkt_publicacao_comentario c
                WHERE c.tb_mkt_publicacao_id IN @PublicacaoIds
                GROUP BY c.tb_mkt_publicacao_id";

            var comentariosCounts = (await connection.QueryAsync<AnaliticoRow>(sqlComentarios, new { PublicacaoIds = publicacaoIds })).ToList();

            foreach (var a in analiticos)
            {
                var cc = comentariosCounts.FirstOrDefault(c => c.PublicacaoId == a.PublicacaoId);
                if (cc != null) a.QuantidadeComentarios = cc.QuantidadeComentarios;
            }

            foreach (var cc in comentariosCounts.Where(c => !analiticos.Any(a => a.PublicacaoId == c.PublicacaoId)))
            {
                analiticos.Add(new AnaliticoRow
                {
                    PublicacaoId = cc.PublicacaoId,
                    QuantidadeComentarios = cc.QuantidadeComentarios
                });
            }

            return analiticos;
        }

        private async Task InserirRelacionamentosPublicacaoAsync(
            System.Data.IDbConnection connection,
            Guid publicacaoId,
            int orgId,
            string tipoPublicacao,
            List<string> labels,
            List<string> tags,
            List<string> grupos,
            string comunidadeId,
            List<PublicacaoAnexoInputDTO> anexos)
        {
            var documento = string.Equals(tipoPublicacao, "documento", StringComparison.OrdinalIgnoreCase);

            if (labels?.Count > 0)
            {
                var labelTipo = documento ? "documento" : "informativo";
                foreach (var labelNome in labels)
                {
                    if (string.IsNullOrWhiteSpace(labelNome)) continue;
                    var labelId = await ObterOuCriarLabelIdAsync(connection, labelNome, orgId, labelTipo);

                    var sqlPubLabel = @"
                        INSERT INTO tb_mkt_publicacao_label (id, tb_mkt_label_id, tb_mkt_publicacao_id)
                        VALUES (@Id, @LabelId, @PublicacaoId)";

                    await connection.ExecuteAsync(sqlPubLabel, new
                    {
                        Id = Guid.NewGuid(),
                        LabelId = labelId,
                        PublicacaoId = publicacaoId
                    });
                }
            }

            if (documento && tags?.Count > 0)
            {
                foreach (var tagNome in tags)
                {
                    if (string.IsNullOrWhiteSpace(tagNome)) continue;
                    var tagId = await ObterOuCriarTagIdAsync(connection, tagNome, orgId);

                    var sqlPubTag = @"
                        INSERT INTO tb_mkt_publicacao_tag (id, tb_mkt_tag_id, tb_mkt_publicacao_id)
                        VALUES (@Id, @TagId, @PublicacaoId)";

                    await connection.ExecuteAsync(sqlPubTag, new
                    {
                        Id = Guid.NewGuid(),
                        TagId = tagId,
                        PublicacaoId = publicacaoId
                    });
                }
            }

            if (grupos?.Count > 0)
            {
                foreach (var grupoId in grupos)
                {
                    var sqlPubGrupo = @"
                        INSERT INTO tb_mkt_publicaco_grupo (id, tb_mkt_publicacao_id, tb_mkt_grupo_id)
                        VALUES (@Id, @PublicacaoId, @GrupoId)";

                    await connection.ExecuteAsync(sqlPubGrupo, new
                    {
                        Id = Guid.NewGuid(),
                        PublicacaoId = publicacaoId,
                        GrupoId = grupoId
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(comunidadeId))
            {
                var sqlComunidade = @"
                    INSERT INTO tb_mkt_comunidade_publicacao (tb_mkt_comunidade_id, tb_mkt_publicacao_id)
                    VALUES (@ComunidadeId, @PublicacaoId)";
                await connection.ExecuteAsync(sqlComunidade, new { ComunidadeId = comunidadeId, PublicacaoId = publicacaoId });
            }

            if (anexos?.Count > 0)
            {
                foreach (var anexo in anexos)
                {
                    var anexoId = anexo.AnexoId ?? Guid.NewGuid();
                    var sqlAnexo = @"
                        INSERT INTO tb_mkt_publicacao_anexo (id, tb_mkt_publicacao_id, nome_arquivo, url_arquivo, tamanho_bytes, tipo)
                        VALUES (@Id, @PublicacaoId, @NomeArquivo, @UrlArquivo, @TamanhoBytes, @Tipo)";

                    await connection.ExecuteAsync(sqlAnexo, new
                    {
                        Id = anexoId,
                        PublicacaoId = publicacaoId,
                        anexo.NomeArquivo,
                        anexo.UrlArquivo,
                        anexo.TamanhoBytes,
                        anexo.Tipo
                    });
                }
            }
        }

        private async Task RemoverRelacionamentosPublicacaoAsync(System.Data.IDbConnection connection, Guid publicacaoId)
        {
            await connection.ExecuteAsync("DELETE FROM tb_mkt_publicacao_label WHERE tb_mkt_publicacao_id = @PublicacaoId", new { PublicacaoId = publicacaoId });
            await connection.ExecuteAsync("DELETE FROM tb_mkt_publicacao_tag WHERE tb_mkt_publicacao_id = @PublicacaoId", new { PublicacaoId = publicacaoId });
            await connection.ExecuteAsync("DELETE FROM tb_mkt_publicaco_grupo WHERE tb_mkt_publicacao_id = @PublicacaoId", new { PublicacaoId = publicacaoId });
            await connection.ExecuteAsync("DELETE FROM tb_mkt_publicacao_anexo WHERE tb_mkt_publicacao_id = @PublicacaoId", new { PublicacaoId = publicacaoId });
        }

        private async Task RemoverRelacionamentosPublicacaoLabelsGruposAsync(System.Data.IDbConnection connection, Guid publicacaoId)
        {
            await connection.ExecuteAsync("DELETE FROM tb_mkt_publicacao_label WHERE tb_mkt_publicacao_id = @PublicacaoId", new { PublicacaoId = publicacaoId });
            await connection.ExecuteAsync("DELETE FROM tb_mkt_publicacao_tag WHERE tb_mkt_publicacao_id = @PublicacaoId", new { PublicacaoId = publicacaoId });
            await connection.ExecuteAsync("DELETE FROM tb_mkt_publicaco_grupo WHERE tb_mkt_publicacao_id = @PublicacaoId", new { PublicacaoId = publicacaoId });
            await connection.ExecuteAsync("DELETE FROM tb_mkt_comunidade_publicacao WHERE tb_mkt_publicacao_id = @PublicacaoId", new { PublicacaoId = publicacaoId });
        }

        private async Task<Guid> ObterOuCriarLabelIdAsync(System.Data.IDbConnection connection, string labelNome, int orgId, string labelTipo = "informativo")
        {
            var tipo = string.Equals(labelTipo, "documento", StringComparison.OrdinalIgnoreCase) ? "documento" : "informativo";
            var sqlSelect = @"
                SELECT id FROM tb_mkt_label
                WHERE nome = @Nome AND tipo = @Tipo AND tb_org_id = @OrgId
                LIMIT 1";

            var existingId = await connection.ExecuteScalarAsync<Guid?>(sqlSelect, new
            {
                Nome = labelNome,
                Tipo = tipo,
                OrgId = orgId
            });

            if (existingId.HasValue)
                return existingId.Value;

            var newId = Guid.NewGuid();

            var sqlInsert = @"
                INSERT INTO tb_mkt_label (id, nome, tipo, tb_org_id, data_criacao)
                VALUES (@Id, @Nome, @Tipo, @OrgId, @DataCriacao)";

            await connection.ExecuteAsync(sqlInsert, new
            {
                Id = newId,
                Nome = labelNome,
                Tipo = tipo,
                OrgId = orgId,
                DataCriacao = DateTime.Now
            });

            return newId;
        }

        private async Task<Guid> ObterOuCriarTagIdAsync(System.Data.IDbConnection connection, string tagNome, int orgId)
        {
            var sqlSelect = @"
                SELECT id FROM tb_mkt_tag
                WHERE nome = @Nome AND tb_org_id = @OrgId
                LIMIT 1";

            var existingId = await connection.ExecuteScalarAsync<Guid?>(sqlSelect, new
            {
                Nome = tagNome,
                OrgId = orgId
            });

            if (existingId.HasValue)
                return existingId.Value;

            var newId = Guid.NewGuid();

            var sqlInsert = @"
                INSERT INTO tb_mkt_tag (id, nome, tb_org_id, data_criacao)
                VALUES (@Id, @Nome, @OrgId, @DataCriacao)";

            await connection.ExecuteAsync(sqlInsert, new
            {
                Id = newId,
                Nome = tagNome,
                OrgId = orgId,
                DataCriacao = DateTime.Now
            });

            return newId;
        }

        #endregion

        #region Rotina Publicação Agendada

        public async Task<List<Guid>> ListarIdsAgendadasParaAtivarAsync(DateTime dataReferencia)
        {
            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT id
                FROM tb_mkt_publicacao
                WHERE publicacao_status = 'agendada'
                    AND aprovacao_status IN ('aprovado', 'nao_requer')
                    AND data_agendamento_publicacao IS NOT NULL
                    AND data_agendamento_publicacao <= @DataReferencia";
            var rows = await connection.QueryAsync<Guid>(sql, new { DataReferencia = dataReferencia });
            return rows.ToList();
        }

        public async Task<List<Guid>> ListarIdsAtivasParaExpirarAsync(DateTime dataReferencia)
        {
            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT id
                FROM tb_mkt_publicacao
                WHERE publicacao_status = 'ativa'
                    AND data_validade IS NOT NULL
                    AND data_validade < @DataReferencia";
            var rows = await connection.QueryAsync<Guid>(sql, new { DataReferencia = dataReferencia });
            return rows.ToList();
        }

        public async Task AtualizarStatusPublicacaoEmLoteAsync(IEnumerable<Guid> ids, string publicacaoStatus, DateTime? dataPublicacao = null)
        {
            var idList = ids?.ToList() ?? new List<Guid>();
            if (idList.Count == 0)
                return;

            var connection = _dapperConnection.GetConnection();
            if (dataPublicacao.HasValue)
            {
                var sql = @"
                    UPDATE tb_mkt_publicacao
                    SET publicacao_status = @PublicacaoStatus,
                        data_publicacao = @DataPublicacao,
                        data_agendamento_publicacao = NULL,
                        data_alteracao = @DataAlteracao
                    WHERE id IN @Ids";
                await connection.ExecuteAsync(sql, new
                {
                    PublicacaoStatus = publicacaoStatus,
                    DataPublicacao = dataPublicacao.Value,
                    DataAlteracao = DateTime.UtcNow,
                    Ids = idList
                });
            }
            else
            {
                var sql = @"
                    UPDATE tb_mkt_publicacao
                    SET publicacao_status = @PublicacaoStatus,
                        data_alteracao = @DataAlteracao
                    WHERE id IN @Ids";
                await connection.ExecuteAsync(sql, new
                {
                    PublicacaoStatus = publicacaoStatus,
                    DataAlteracao = DateTime.UtcNow,
                    Ids = idList
                });
            }

            var orgIdsAfetadas = (await connection.QueryAsync<int>(
                "SELECT DISTINCT tb_org_id AS OrgId FROM tb_mkt_publicacao WHERE id IN @Ids",
                new { Ids = idList })).Distinct().ToList();
            foreach (var orgAfeta in orgIdsAfetadas)
                await SincronizarSugestoesPastasPorPublicacoesAtivasAsync(orgAfeta);
        }

        public async Task<List<PublicacaoOficialEnvioDTO>> ListarPublicacoesAtivasNaoProcessadasEnvioAsync()
        {
            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT id AS Id,
                    publicacao_oficial AS PublicacaoOficial,
                    titulo AS Titulo,
                    subtitulo AS Subtitulo,
                    tb_org_id AS OrgId
                FROM tb_mkt_publicacao
                WHERE publicacao_status = 'ativa'
                    AND aprovacao_status IN ('aprovado', 'nao_requer')
                    AND (processado_envio_notificacao_email = 0)";
            
            var pubs = (await connection.QueryAsync<PublicacaoOficialEnvioDTO>(sql)).ToList();
            if (pubs.Count == 0)
                return pubs;

            var ids = pubs.Select(p => p.Id).ToList();
            
            var relSql = @"
                SELECT tb_mkt_publicacao_id AS PublicacaoId, tb_mkt_grupo_id AS GrupoId
                FROM tb_mkt_publicaco_grupo
                WHERE tb_mkt_publicacao_id IN @Ids";
            
            var rels = await connection.QueryAsync<(Guid PublicacaoId, string GrupoId)>(relSql, new { Ids = ids });
            
            var porPub = rels
                .GroupBy(r => r.PublicacaoId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.GrupoId).Distinct().ToList());

            foreach (var p in pubs)
            {
                if (porPub.TryGetValue(p.Id, out var gids))
                    p.GrupoIds = gids;
            }

            var comSql = @"
                SELECT cp.tb_mkt_publicacao_id AS PublicacaoId,
                    cp.tb_mkt_comunidade_id AS ComunidadeId,
                    com.nome AS ComunidadeNome
                FROM tb_mkt_comunidade_publicacao cp
                INNER JOIN tb_mkt_comunidade com ON com.id = cp.tb_mkt_comunidade_id
                WHERE cp.tb_mkt_publicacao_id IN @Ids";
            var comRels = await connection.QueryAsync<(Guid PublicacaoId, Guid ComunidadeId, string ComunidadeNome)>(comSql, new { Ids = ids });
            foreach (var row in comRels)
            {
                if (string.IsNullOrWhiteSpace(row.ComunidadeId.ToStringOuVazio()))
                    continue;

                var p = pubs.Find(x => x.Id == row.PublicacaoId);
                if (p == null || !string.IsNullOrEmpty(p.ComunidadeId))
                    continue;

                p.ComunidadeId = row.ComunidadeId.ToStringOuVazio();
                p.ComunidadeNome = row.ComunidadeNome;
            }

            return pubs;
        }

        public async Task AtualizarProcessadoEnvioNotificacaoEmailAsync(IEnumerable<Guid> publicacaoIds)
        {
            var idList = publicacaoIds?.ToList() ?? new List<Guid>();
            if (idList.Count == 0)
                return;

            var connection = _dapperConnection.GetConnection();
            var sql = @"
                UPDATE tb_mkt_publicacao
                SET processado_envio_notificacao_email = 1,
                    data_alteracao = @DataAlteracao
                WHERE id IN @Ids";
            await connection.ExecuteAsync(sql, new { DataAlteracao = DateTime.UtcNow, Ids = idList });
        }

        public async Task InserirLogEnvioPublicacaoOficialAsync(Guid publicacaoId, string tipo, string codigoInternoColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var sql = @"
                INSERT INTO tb_mkt_publicacao_envio_log (id, tb_mkt_publicacao_id, tipo, codigo_interno_colaborador, tb_org_id)
                VALUES (@Id, @PublicacaoId, @Tipo, @CodigoInternoColaborador, @OrgId)";
            await connection.ExecuteAsync(sql, new
            {
                Id = Guid.NewGuid(),
                PublicacaoId = publicacaoId,
                Tipo = tipo,
                CodigoInternoColaborador = codigoInternoColaborador,
                OrgId = orgId
            });
        }

        #endregion
    }
}
