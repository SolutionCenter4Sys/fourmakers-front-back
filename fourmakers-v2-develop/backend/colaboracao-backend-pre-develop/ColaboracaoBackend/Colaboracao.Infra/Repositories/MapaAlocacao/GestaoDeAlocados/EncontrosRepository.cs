using Colaboracao.Core.Interfaces;
using Dapper;
using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.ClienteOrg;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.SolicitacaoParticipacao;
using DataTransferObject.Domain.Projeto.GestorExterno;
using DataTransferObject.Domain.Usuario;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using MapaDeAlocacao.Domain.Impl.GestaoDeAlocados;
using MySql.Data.MySqlClient;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.AgendaEncontroResult;

namespace Colaboracao.Infra.Repositories.MapaAlocacao.GestaoDeAlocados
{
    public class EncontrosRepository : IEncontrosRepository
    {
        private readonly IDBConnection dapperConnection;
        private IConnectionStringCore _connectionString;

        public EncontrosRepository(IDBConnection _dapperConnection, IConnectionStringCore connectionString)
        {
            dapperConnection = _dapperConnection;
            _connectionString = connectionString;
        }

        private const string QEncontroBase = """
                                             SELECT
                                                 te.id AS Id,
                                                 te.tb_agendas_comerciais_id AS AgendaId,
                                                 te.tb_colaborador_codigo_interno_colaborador AS CodigoInternoColaborador,
                                                 tc.nome_completo AS NomeCompletoColaboradorCriador,
                                                 te.data_requisicao AS DataRequisicao,
                                                 te.titulo_interacao AS TituloInteracao,
                                                 te.resumo_interacao AS ResumoInteracao,
                                                 te.descricao_interacao AS DescricaoInteracao
                                             FROM tb_interacoes te
                                             INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = te.tb_colaborador_codigo_interno_colaborador
                                             """;

        private const string QAgendaBase = """
                                           SELECT
                                               tae.id AS Id,
                                               tae.graph_event_id AS GraphEventId,
                                               tae.tb_colaborador_codigo_interno_colaborador AS CodColaboradorCriador,
                                               tc.nome_completo AS NomeCompletoColaboradorCriador,
                                               tae.tb_cliente_org_codigo_cliente AS CodigoCliente,
                                               tae.tb_tipo_agenda_id AS TipoInteracao,
                                               tae.data_criacao AS DataCriacao,
                                               tae.data_atualizacao AS DataAtualizacao,
                                               tae.data_agendada AS DataAgendada,
                                               tae.data_inicio AS DataInicio,
                                               tae.data_fim AS DataFim,
                                               tae.status AS Status,
                                               tae.localizacao AS Localizacao,
                                               tae.link_reuniao AS LinkReuniao,
                                               tae.quantidade_participantes AS QuantidadeParticipantes,
                                               tae.titulo AS Titulo,
                                               tae.descricao AS Descricao,
                                               tae.agenda_pai_id AS AgendaPaiId,
                                               tao.id AS AgendaObjetivoId
                                           FROM tb_agendas_comerciais tae
                                           INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = tae.tb_colaborador_codigo_interno_colaborador
                                           LEFT JOIN tb_agenda_objetivo tao on tao.id = tae.tb_agenda_objetivo_id AND tao.ativo = 1
                                           """;

        public async Task<ArquivoEncontroDto> InserirArquivo(ArquivoEncontroParam param, string url)
        {
            var connection = dapperConnection.GetConnection();
            var isAudio = param.TipoArquivo?.StartsWith("audio", StringComparison.OrdinalIgnoreCase) == true;

            const string qInsert = """
                                   INSERT INTO tb_interacoes_archives
                                   (tb_interacoes_id, link_audio_interacao, link_imagem_interacao, transcricao)
                                   VALUES (@EncontroId, @LinkAudio, @LinkImagem, @Transcricao);
                                   SELECT LAST_INSERT_ID();
                                   """;

            var id = await connection.QuerySingleAsync<int>(qInsert, new
            {
                EncontroId = param.EncontroId,
                LinkAudio = isAudio ? url : null,
                LinkImagem = isAudio ? null : url,
                param.Transcricao
            });

            const string qSelect = """
                                   SELECT
                                       id                    AS Id,
                                       data_archived         AS DataArquivo,
                                       link_audio_interacao  AS LinkAudioInteracao,
                                       link_imagem_interacao AS LinkImagemInteracao
                                   FROM tb_interacoes_archives
                                   WHERE id = @Id;
                                   """;
            return await connection.QueryFirstAsync<ArquivoEncontroDto>(qSelect, new { Id = id });
        }

        public async Task<ArquivoEncontroDto> BuscarArquivoPorId(int id)
        {
            var conn = dapperConnection.GetConnection();

            const string qSelect = """
                                   SELECT
                                       id                    AS Id,
                                       data_archived         AS DataArquivo,
                                       link_audio_interacao  AS LinkAudioInteracao,
                                       link_imagem_interacao AS LinkImagemInteracao,
                                       transcricao           AS Transcricao
                                   FROM tb_interacoes_archives
                                   WHERE id = @Id;
                                   """;
            return await conn.QueryFirstOrDefaultAsync<ArquivoEncontroDto>(qSelect, new { Id = id });
        }

        public async Task<bool> DeletarArquivo(int id)
        {
            var conn = dapperConnection.GetConnection();

            const string qDel = """
                                DELETE FROM tb_interacoes_archives
                                WHERE id = @Id;
                                """;
            var linhasAfetadas = await conn.ExecuteAsync(qDel, new { Id = id });
            
            // Retorna true se deletou (1 ou mais linhas afetadas), false se não encontrou
            return linhasAfetadas > 0;
        }

        public async Task<AgendaEncontroResult> carregarAgenda(int id, string cpfUsuarioLogado)
        {
            try
            {
                var conn = dapperConnection.GetConnection();

                // 1) Montagem das queries
                const string sql = $@"
                    -- 1. Agenda
                    {QAgendaBase} WHERE tae.id = @Id;

                    -- 2. Gestores Externos
                    SELECT  g.cod_gestor_externo AS CodGestorExterno, g.nome, g.email, p.tb_agendas_comerciais_id AS AgendaEncontroId,
                            p.data_convite AS DataCriacao, NULL AS DataAlteracao, g.telefone, g.codigo_cliente AS CodigoCliente, g.perfil_linkedin AS PerfilLinkedin
                    FROM tb_agenda_convidados p
                    INNER JOIN tb_gestor_externo g ON g.cod_gestor_externo = p.codigo_colaborador_interno_externo AND g.tb_org_id = 2
                    WHERE p.tb_agendas_comerciais_id = @Id
                    AND   p.tipo_codigo = 2;

                    -- 3. Colaboradores
                    SELECT DISTINCT p.codigo_colaborador_interno_externo AS CodInternoColaborador, tc.nome_completo AS Nome, tu.email AS Email
                    FROM tb_agenda_convidados p
                    INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = p.codigo_colaborador_interno_externo
                    LEFT JOIN tb_usuario tu ON tu.codigo_interno_colaborador = p.codigo_colaborador_interno_externo
                    WHERE p.tb_agendas_comerciais_id = @Id
                    AND   p.tipo_codigo = 1;

                    -- 4. Participantes Externos
                    SELECT DISTINCT nome, email FROM tb_participantes_externo WHERE tb_agendas_comerciais_id = @Id;

                    -- 5. Todos os vínculos (Participantes - Gestores e Colaboradores) 
                    SELECT  p.id,
                            COALESCE(g.nome, tc.nome_completo) AS Nome,
                            CASE WHEN p.tipo_codigo = 2 THEN p.codigo_colaborador_interno_externo END AS CodigoGestorExterno,
                            CASE WHEN p.tipo_codigo = 1 THEN p.codigo_colaborador_interno_externo END AS CodigoColaborador,
                            p.tb_status_app_id AS Status,
                            p.data_resposta AS DataResposta,
                            p.data_convite AS DataConvite
                    FROM tb_agenda_convidados p 
                    LEFT JOIN tb_gestor_externo g ON g.cod_gestor_externo = p.codigo_colaborador_interno_externo AND g.tb_org_id = 2
                    LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = p.codigo_colaborador_interno_externo
                    WHERE p.tb_agendas_comerciais_id = @Id;

                    -- 6. Cliente (Subquery baseada no ID da agenda para evitar round-trip)
                    SELECT id, codigo_cliente AS CodigoCliente, nome_cliente AS NomeCliente 
                    FROM tb_cliente_org 
                    WHERE tb_org_id = 2 AND codigo_cliente = (SELECT tb_cliente_org_codigo_cliente FROM tb_agendas_comerciais WHERE id = @Id);
        
                    -- 7. Status Solicitação (Integrado)
                    SELECT tasp.tb_status_app_id AS status 
                    FROM tb_agenda_solicitante tasp
                    WHERE tasp.tb_agendas_comerciais_id = @Id 
                    AND tasp.codigo_colaborador_interno_externo = @Cpf;

                    -- 8. Participação Usuário Logado (Integrado)
                    SELECT  tb_agendas_comerciais_id AS AgendaId, 
                            tb_status_app_id AS Status, 
                            data_resposta AS DataResposta
                    FROM    tb_agenda_convidados
                    WHERE   codigo_colaborador_interno_externo = @Cpf AND tb_agendas_comerciais_id = @Id
                    LIMIT 1;
                ";

                var multi = await conn.QueryMultipleAsync(sql, new { Id = id, Cpf = cpfUsuarioLogado });

                var agendaModel = await multi.ReadFirstOrDefaultAsync<AgendaEncontroResult>();

                if (agendaModel == null)
                    return null;

                // A ordem de leitura DEVE ser a mesma ordem do SQL acima
                agendaModel.GestoresExternos = (await multi.ReadAsync<GestorExternoResult>()).ToList();
                agendaModel.Colaboradores = (await multi.ReadAsync<ColaboradorAgenda>()).ToList();
                agendaModel.ParticipantesExterno = (await multi.ReadAsync<ParticipanteExterno>()).ToList();
                agendaModel.Participantes = (await multi.ReadAsync<ParticipanteConvidadoDto>()).ToList();
                agendaModel.Cliente = await multi.ReadFirstOrDefaultAsync<ClienteOrgDaGestaoAlocadosResult>();
                agendaModel.StatusSolicitacaoParticipante = await multi.ReadFirstOrDefaultAsync<StatusSolicitacaoParticipante>();
                agendaModel.ParticipacaoUsuarioLogado = await multi.ReadFirstOrDefaultAsync<ParticipacaoUsuarioConvidado>();
                agendaModel.Encontros = await buscarEncontrosPorAgendaId(id.ToString());

                return agendaModel;
            }
            catch (Exception)
            {

                throw;
            }
        }


        private async Task<AgendaEncontroResult> carregarAgendaSemInteracao(int id, string cpfUsuarioLogado)
        {
             var conn = dapperConnection.GetConnection();

            try
            {
                // 1) Montagem das queries
                const string sql = $@"
                    -- 1. Agenda
                    {QAgendaBase} WHERE tae.id = @Id;

                    -- 2. Gestores Externos
                    SELECT  g.cod_gestor_externo AS CodGestorExterno, g.nome, g.email, p.tb_agendas_comerciais_id AS AgendaEncontroId,
                            p.data_convite AS DataCriacao, NULL AS DataAlteracao, g.telefone, g.codigo_cliente AS CodigoCliente, g.perfil_linkedin AS PerfilLinkedin
                    FROM tb_agenda_convidados p
                    INNER JOIN tb_gestor_externo g ON g.cod_gestor_externo = p.codigo_colaborador_interno_externo AND g.tb_org_id = 2
                    WHERE p.tb_agendas_comerciais_id = @Id
                    AND   p.tipo_codigo = 2;

                    -- 3. Colaboradores
                    SELECT DISTINCT p.codigo_colaborador_interno_externo AS CodInternoColaborador, tc.nome_completo AS Nome, tu.email AS Email
                    FROM tb_agenda_convidados p
                    INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = p.codigo_colaborador_interno_externo
                    LEFT JOIN tb_usuario tu ON tu.codigo_interno_colaborador = p.codigo_colaborador_interno_externo
                    WHERE p.tb_agendas_comerciais_id = @Id
                    AND   p.tipo_codigo = 1;

                    -- 4. Participantes Externos
                    SELECT DISTINCT nome, email FROM tb_participantes_externo WHERE tb_agendas_comerciais_id = @Id;

                    -- 5. Todos os vínculos (Participantes - Gestores e Colaboradores)
                    SELECT  p.id, COALESCE(g.nome, tc.nome_completo) AS Nome, 
                            CASE WHEN p.tipo_codigo = 2 THEN p.codigo_colaborador_interno_externo END AS CodigoGestorExterno,
                            CASE WHEN p.tipo_codigo = 1 THEN p.codigo_colaborador_interno_externo END AS CodigoColaborador,
                            p.tb_status_app_id AS Status, 
                            p.data_resposta AS DataResposta, p.data_convite AS DataConvite                            
                    FROM tb_agenda_convidados p 
                    LEFT JOIN tb_gestor_externo g ON g.cod_gestor_externo = p.codigo_colaborador_interno_externo AND g.tb_org_id = 2
                    LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = p.codigo_colaborador_interno_externo
                    WHERE p.tb_agendas_comerciais_id = @Id;

                    -- 6. Cliente (Subquery baseada no ID da agenda para evitar round-trip)
                    SELECT id, codigo_cliente AS CodigoCliente, nome_cliente AS NomeCliente 
                    FROM tb_cliente_org 
                    WHERE tb_org_id = 2 AND codigo_cliente = (SELECT tb_cliente_org_codigo_cliente FROM tb_agendas_comerciais WHERE id = @Id);
        
                    -- 7. Status Solicitação (Integrado)
                    SELECT tasp.tb_status_app_id AS status 
                    FROM tb_agenda_solicitante tasp
                    WHERE tasp.tb_agendas_comerciais_id = @Id 
                    AND tasp.codigo_colaborador_interno_externo = @Cpf;

                    -- 8. Participação Usuário Logado (Integrado)
                    SELECT  tb_agendas_comerciais_id AS AgendaId, 
                            tb_status_app_id AS Status, 
                            data_resposta AS DataResposta
                    FROM    tb_agenda_convidados
                    WHERE   codigo_colaborador_interno_externo = @Cpf AND tb_agendas_comerciais_id = @Id
                    LIMIT 1;
                ";

                var multi = await conn.QueryMultipleAsync(sql, new { Id = id, Cpf = cpfUsuarioLogado });

                var agendaModel = await multi.ReadFirstOrDefaultAsync<AgendaEncontroResult>();

                if (agendaModel == null)
                    return null;

                // A ordem de leitura DEVE ser a mesma ordem do SQL acima
                agendaModel.GestoresExternos = (await multi.ReadAsync<GestorExternoResult>()).ToList();
                agendaModel.Colaboradores = (await multi.ReadAsync<ColaboradorAgenda>()).ToList();
                agendaModel.ParticipantesExterno = (await multi.ReadAsync<ParticipanteExterno>()).ToList();
                agendaModel.Participantes = (await multi.ReadAsync<ParticipanteConvidadoDto>())
                    .Select(p => new ParticipanteConvidadoDto
                    {
                        Id = p.Id,
                        Nome = p.Nome,
                        CodigoGestorExterno = p.CodigoGestorExterno,
                        CodigoColaborador = p.CodigoColaborador,
                        Status = p.Status,
                        DataResposta = p.DataResposta,
                        DataConvite = p.DataConvite
                    })
                    .ToList();
                agendaModel.Cliente = await multi.ReadFirstOrDefaultAsync<ClienteOrgDaGestaoAlocadosResult>();
                agendaModel.StatusSolicitacaoParticipante = await multi.ReadFirstOrDefaultAsync<StatusSolicitacaoParticipante>();
                agendaModel.ParticipacaoUsuarioLogado = await multi.ReadFirstOrDefaultAsync<ParticipacaoUsuarioConvidado>();
                agendaModel.Encontros = null;

                return agendaModel;

            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task<AgendaEncontroResult> criarAgenda(AgendaEncontroParam p, UsuarioLogadoDTO user)
        {
            var conn = dapperConnection.GetConnection();
            var tx = await conn.BeginTransactionAsync();

            var dataCriacao = System.DateTime.Now;

            // 1. Inserir Agenda e recuperar ID em um único comando
            const string qIns = @"
                                INSERT INTO tb_agendas_comerciais
                                (graph_event_id, tb_colaborador_codigo_interno_colaborador,
                                    tb_cliente_org_codigo_cliente,
                                    tb_tipo_agenda_id,
                                    data_agendada, data_inicio, data_fim, status,
                                    localizacao, link_reuniao, quantidade_participantes,
                                    titulo, descricao, agenda_pai_id, data_criacao, data_atualizacao, tb_agenda_objetivo_id)
                                VALUES
                                (@GraphEventId, @Colab, @CodigoCliente, @TipoInteracaoId, @Agendada, @Inicio, @Fim, @Status,
                                 @Loc, @Link, @Qtd, @Titulo, @Desc, @AgendaPaiId, @dataCriacao, @dataCriacao, @ObjetivoId);
                                SELECT LAST_INSERT_ID();
                                ";

            var id = await conn.QuerySingleAsync<int>(qIns, new
            {
                GraphEventId = p.GraphEventId,
                Colab = user.Cpf,
                p.CodigoCliente,
                TipoInteracaoId = p.TipoInteracao,
                Agendada = p.DataAgendada,
                Inicio = p.DataInicio,
                Fim = p.DataFim,
                Status = p.Status ?? "Pendente",
                Loc = p.Localizacao,
                Link = p.LinkReuniao,
                Qtd = p.QuantidadeParticipantes,
                p.Titulo,
                Desc = p.Descricao,
                p.AgendaPaiId,
                dataCriacao = dataCriacao,
                ObjetivoId = p.AgendaObjetivoId is null or 0 ? 1 : p.AgendaObjetivoId.Value
            }, tx);

            // 2. Participantes – Gestores Externos
            if (p.CodigosGestoresExternos?.Any() == true)
            {
                const string qPartG = @"
                            INSERT INTO tb_agenda_convidados
                            (tb_agendas_comerciais_id, codigo_colaborador_interno_externo, tipo_codigo, tb_status_app_id, data_convite)
                            VALUES (@AgendaId, @Gestor, 2, 0, NOW());
                            ";

                // Tira duplicidade
                await conn.ExecuteAsync(qPartG,
                    p.CodigosGestoresExternos.Distinct().Select(g => new { AgendaId = id, Gestor = g }), tx);
            }

            // 3. Participantes – colaboradores
            if (p.CodigosColaboradores?.Any() == true)
            {
                // Cria lista sem duplicidade
                var colaboradoresIdsUnicos = p.CodigosColaboradores.Distinct().ToList();

                const string qCheck = """
                            SELECT codigo_interno_colaborador
                            FROM tb_colaborador
                            WHERE codigo_interno_colaborador IN @Ids;
                        """;

                var existentes = (await conn.QueryAsync<string>(qCheck,
                    new { Ids = colaboradoresIdsUnicos }, tx)).AsList();

                // A lógica de "faltando" agora compara contra a lista já distinta
                var faltando = colaboradoresIdsUnicos.Except(existentes).ToList();

                if (faltando.Count > 0)
                {
                    await tx.RollbackAsync();
                    throw new InvalidOperationException(
                        $"Colaborador(es) não encontrado(s): {string.Join(", ", faltando)}");
                }

                const string qPartC = """
                            INSERT INTO tb_agenda_convidados
                            (tb_agendas_comerciais_id, codigo_colaborador_interno_externo, tipo_codigo, tb_status_app_id, data_convite)
                            VALUES (@AgendaId, @Colab, 1, 0, NOW());
                        """;

                await conn.ExecuteAsync(qPartC,
                    existentes.Select(c => new { AgendaId = id, Colab = c }), tx);
            }

            // 4. Participantes Externos
            if (p.ParticipantesExternos?.Any() == true)
            {
                const string qPartG = """
                            INSERT INTO tb_participantes_externo (tb_agendas_comerciais_id, nome, email)
                            VALUES (@AgendaId, @Nome, @Email);
                        """;

                // Remove objetos com o mesmo e-mail
                var participantesUnicos = p.ParticipantesExternos
                                           .DistinctBy(pe => pe.Email?.ToLower().Trim())
                                           .ToList();

                await conn.ExecuteAsync(qPartG,
                    participantesUnicos.Select(pe => new
                    {
                        AgendaId = id,
                        pe.Nome,
                        Email = pe.Email?.ToLower().Trim()
                    }), tx);
            }

            await tx.CommitAsync();
            return await carregarAgendaSemInteracao(id, user.Cpf);
        }

        public async Task<UsuarioColaboradorDTO> BuscarDadosColaboradorEmail(string cpf)
        {
            var conn = dapperConnection.GetConnection();

            var usuario = await conn.QuerySingleOrDefaultAsync<UsuarioColaboradorDTO>(
                $@" SELECT  tu.email, 
                            tc.nome_completo AS NomeColaborador
                    FROM tb_usuario tu
                    INNER JOIN tb_colaborador tc 
	                    ON tc.codigo_interno_colaborador =  tu.codigo_interno_colaborador 
                    WHERE tu.tb_org_id = 2 AND
                          tu.codigo_interno_colaborador = @cpf;", new { cpf });

            return usuario;
        }

        public async Task<GestorExternoResult> BuscarDadosGestorExternoEmail(string codigoGestor)
        {
            var conn = dapperConnection.GetConnection();

            var usuario = await conn.QuerySingleOrDefaultAsync<GestorExternoResult>(
                $@" SELECT  tge.nome,
		                    tge.email
                    FROM tb_gestor_externo tge
                    WHERE tge.cod_gestor_externo  = @codigoGestor
                    LIMIT 1;", new { codigoGestor });

            return usuario;
        }

        public async Task<string> BuscarNomeClienteEmailAgenda(string codCliente)
        {
            var conn = dapperConnection.GetConnection();

            var nomeCliente = await conn.QuerySingleOrDefaultAsync<string>(
                $@" SELECT nome_cliente
                    FROM tb_cliente_org tco
                    WHERE tco.tb_org_id = 2
					AND tco.codigo_cliente  = @codCliente", new { codCliente });

            return nomeCliente;
        }

        public async Task<AgendaEncontroResult> atualizarAgenda(int agendaId, AgendaEncontroParam p, UsuarioLogadoDTO user)
        {
            if (p is null || agendaId <= 0)
                throw new ArgumentException("Parâmetro agenda inválido ou sem Id.");

            var conn = dapperConnection.GetConnection();

            // 1) Validações Prévias
            var clienteExiste = await conn.ExecuteScalarAsync<bool>(
                    "SELECT EXISTS(SELECT 1 FROM tb_cliente_org WHERE codigo_cliente = @CodClie)",
                    new { CodClie = p.CodigoCliente });

            if (!clienteExiste)
                throw new Exception($"Código Cliente: {p.CodigoCliente} não encontrado!");

            // Monta as listas
            var gestores = (p.CodigosGestoresExternos ?? []).Where(g => !string.IsNullOrWhiteSpace(g)).Distinct().ToList();
            var colabs = (p.CodigosColaboradores ?? []).Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().ToList();
            var externos = (p.ParticipantesExternos ?? []).Where(e => !string.IsNullOrWhiteSpace(e.Email))
                            .DistinctBy(e => e.Email.Trim().ToLower()).ToList();

            var dataCriacao = System.DateTime.Now;

            var tx = await conn.BeginTransactionAsync();
            try
            {
                // 2) Atualiza os dados principais e limpeza vínculos
                const string qUpd = """
                                        UPDATE tb_agendas_comerciais SET
                                            data_agendada = @Agendada, data_inicio = @Inicio, data_fim = @Fim,
                                            status = @Status, localizacao = @Loc, link_reuniao = @Link,
                                            titulo = @Titulo, descricao = @Desc, quantidade_participantes = @QuantidadeParticipantes,
                                            graph_event_id = @GraphEventId, tb_cliente_org_codigo_cliente = @CodCli,
                                            data_atualizacao = @DataAtual, tb_agenda_objetivo_id = @ObjetivoAgenda
                                        WHERE id = @Id;

                                        DELETE FROM tb_agenda_convidados WHERE tb_agendas_comerciais_id = @Id;
                                        DELETE FROM tb_participantes_externo WHERE tb_agendas_comerciais_id = @Id;
                                    """;

                var rowsAffected = await conn.ExecuteAsync(qUpd, new
                {
                    Id = agendaId,
                    p.GraphEventId,
                    Agendada = p.DataAgendada,
                    Inicio = p.DataInicio,
                    Fim = p.DataFim,
                    Status = p.Status ?? "Pendente",
                    Loc = p.Localizacao,
                    Link = p.LinkReuniao,
                    p.Titulo,
                    Desc = p.Descricao,
                    p.QuantidadeParticipantes,
                    CodCli = p.CodigoCliente,
                    DataAtual = dataCriacao,
                    ObjetivoAgenda = p.AgendaObjetivoId is null or 0 ? 1 : p.AgendaObjetivoId.Value
                }, tx);

                if (rowsAffected == 0)
                    throw new Exception($"Agenda: {agendaId} não encontrada!");

                // --- 3) Inserções 
                const string sqlInsertConvidados = """
                    INSERT INTO tb_agenda_convidados
                        (tb_agendas_comerciais_id, codigo_colaborador_interno_externo, tipo_codigo, data_convite, data_resposta)
                    VALUES
                        (@AgendaId, @Codigo, @TipoCodigo, NOW(), NULL);
                    """;

                var gestoresExternosParaInserir = gestores
                    .Where(g => !string.IsNullOrWhiteSpace(g))
                    .Select(g => new { AgendaId = agendaId, Codigo = g.Trim(), TipoCodigo = 2 })
                    .ToList();

                if (gestoresExternosParaInserir.Any())
                    await conn.ExecuteAsync(sqlInsertConvidados, gestoresExternosParaInserir, tx);

                var colaboradoresParaInserir = colabs
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Select(c => new { AgendaId = agendaId, Codigo = c.Trim(), TipoCodigo = 1 })
                    .ToList();

                if (colaboradoresParaInserir.Any())
                    await conn.ExecuteAsync(sqlInsertConvidados, colaboradoresParaInserir, tx);

                if (externos.Any())
                {
                    await conn.ExecuteAsync(
                        "INSERT INTO tb_participantes_externo (tb_agendas_comerciais_id, nome, email) VALUES (@AgendaId, @Nome, @Email)",
                        externos.Select(e => new { AgendaId = agendaId, e.Nome, Email = e.Email.Trim().ToLower() }), tx);
                }

                await tx.CommitAsync();

                return await carregarAgendaSemInteracao(agendaId, user.Cpf);
            }
            catch (Exception)
            {
                await tx.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> deletarAgenda(string agendaId)
        {
            var conn = dapperConnection.GetConnection();

            // opcional — impede exclusão se ainda houver encontros
            var qtdEncontros = await conn.ExecuteScalarAsync<bool>(
                    "SELECT EXISTS(SELECT 1 FROM tb_interacoes WHERE tb_agendas_comerciais_id = @Id)",
                     new { Id = agendaId });

            if (qtdEncontros)
                throw new InvalidOperationException("Agenda possui encontros vinculados — exclua-os primeiro.");

            var agendasFilhos = await BuscarIdAgendasFilhos(agendaId);

            if (agendasFilhos?.Any() == true)
                throw new InvalidOperationException(
                    "Agenda possui agendas filhos vinculados — exclua-os primeiro."
                );

            return await conn.ExecuteAsync("DELETE FROM tb_agendas_comerciais WHERE id = @Id;",
                       new { Id = agendaId }) > 0;
        }

        public async Task<InteracoesDTO> criarEncontro(EncontrosDTO p, UsuarioLogadoDTO user)
        {
            var conn = dapperConnection.GetConnection();

            const string qIns = @"
                                INSERT INTO tb_interacoes
                                    (tb_agendas_comerciais_id,
                                     tb_colaborador_codigo_interno_colaborador,
                                     titulo_interacao,
                                     descricao_interacao,
                                     resumo_interacao)
                                VALUES
                                    (@AgendaId, @Colab, @Titulo, @Desc, @Resumo);
                                SELECT LAST_INSERT_ID();
                                ";

            var encontroId = await conn.QuerySingleAsync<int>(qIns, new
            {
                AgendaId = p.AgendaId,
                Colab = user.Cpf,
                Titulo = p.TituloInteracao,
                Desc = p.DescricaoInteracao,
                Resumo = p.ResumoInteracao
            });

            return await BuscarInteracaoPorId(encontroId.ToString());
        }

        public async Task<InteracoesDTO> atualizarEncontro(string encontroId, EncontrosParam p)
        {
            var conn = dapperConnection.GetConnection();

            try
            {
                const string qUpd = @"
                                UPDATE  tb_interacoes SET
                                        tb_agendas_comerciais_id = @AgendaId,
                                        titulo_interacao         = @Titulo,
                                        descricao_interacao      = @Desc,
                                        resumo_interacao         = @Resumo
                                WHERE id = @Id;
                               ";

                await conn.ExecuteAsync(qUpd, new
                {
                    Id = encontroId,
                    AgendaId = p.AgendaId,
                    Titulo = p.TituloInteracao,
                    Desc = p.DescricaoInteracao,
                    Resumo = p.ResumoInteracao
                });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao atualizar Interação - " + ex.Message);
            }

            return await BuscarInteracaoPorId(encontroId);
        }

        public async Task<EncontrosResponse> buscarEncontroPorId(string id)
        {
            var conn = dapperConnection.GetConnection();

            var sql = @"
                    -- Encontro AI
                    SELECT id AS Id, resumo, data_gerada 
                    FROM tb_interacao_ai 
                    WHERE tb_interacoes_id = @Id;

                    -- Passos (Ações)
                    SELECT  
                        teap.id AS Id, 
                        teap.tb_interacao_ai_id AS EncontroAiId,
                        teap.texto AS Texto, 
                        teap.tb_colaborador_codigo_interno_colaborador AS CodigoColaborador,
                        tc.nome_completo AS NomeColaborador,
                        teap.data_limite AS DataLimite
                    FROM tb_interacao_acoes teap
                    INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = teap.tb_colaborador_codigo_interno_colaborador 
                    WHERE teap.tb_interacao_ai_id IN (SELECT id FROM tb_interacao_ai WHERE tb_interacoes_id = @Id);

                    -- Comentários
                    SELECT 
                        tcic.id AS Id,
                        tcic.tb_interacao_acoes_id AS InteracaoAcoesId,
                        tcic.tb_colaborador_codigo_interno_colaborador AS CodInternoColaborador,
                        tc.nome_completo AS NomeColaborador,
                        tcic.data AS Data,
                        tcic.comentario AS Comentario
                    FROM tb_comentarios_interacao_acoes tcic
                    LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = tcic.tb_colaborador_codigo_interno_colaborador
                    WHERE tcic.tb_interacao_acoes_id IN (
                        SELECT id FROM tb_interacao_acoes WHERE tb_interacao_ai_id IN (
                            SELECT id FROM tb_interacao_ai WHERE tb_interacoes_id = @Id
                        )
                    );

                    -- Categorias
                    SELECT 
                        tcs.id AS Id,
                        tcs.tb_interacoes_id AS InteracaoId,
                        tcs.tb_categoria_assunto_id AS CategoriaId,
                        tca.descricao AS CategoriaDescricao,
                        tcs.tb_subcategoria_assunto_id AS SubcategoriaId,
                        tsa.descricao AS SubcategoriaDescricao,
                        tcs.data_criacao AS DataCriacao,
                        tcs.ativo AS Ativo
                    FROM tb_interacoes_categoria_sub tcs
                    INNER JOIN tb_categoria_assunto tca ON tca.id = tcs.tb_categoria_assunto_id
                    INNER JOIN tb_subcategoria_assunto tsa ON tsa.tb_categoria_assunto_id = tcs.tb_categoria_assunto_id 
                        AND tsa.id = tcs.tb_subcategoria_assunto_id
                    WHERE tcs.tb_interacoes_id = @Id AND tcs.ativo = 1;";

            try
            {
                var multi = await conn.QueryMultipleAsync(sql, new { Id = id });

                var encontroAi = await multi.ReadFirstOrDefaultAsync<EncontroAi>();
                var passos = (await multi.ReadAsync<EncontroAiPassos>()).ToList();
                var comentarios = (await multi.ReadAsync<ComentariosAcoesResponseDTO>()).ToList();
                var categorias = (await multi.ReadAsync<InteracaoCategoriaSubResponseDTO>()).ToList();

                // Mapeamento
                if (passos.Any())
                {
                    var lookup = comentarios.ToLookup(c => c.InteracaoAcoesId);
                    foreach (var p in passos)
                        p.ComentariosAcoes = lookup[p.Id].ToList();

                    if (encontroAi != null)
                        encontroAi.Passos = passos;
                }

                var encontro = await conn.QueryFirstOrDefaultAsync<EncontrosResponse>(
                    $"{QEncontroBase} WHERE id = @Id;", new { Id = id });

                if (encontro != null)
                {
                    encontro.EncontroAi = encontroAi;
                    encontro.Categorias = categorias;
                }

                return encontro;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<InteracoesDTO> BuscarInteracaoPorId(string id)
        {
            var conn = dapperConnection.GetConnection();

            var sql = @"
                        SELECT 
                        id AS Id, 
                        tb_agendas_comerciais_id AS AgendaId,
                        tb_colaborador_codigo_interno_colaborador AS CodigoInternoColaborador,
                        titulo_interacao AS TituloInteracao,
                        descricao_interacao AS DescricaoInteracao,
                        resumo_interacao AS ResumoInteracao
                        FROM tb_interacoes 
                        WHERE id = @Id;
                    ";

            return await conn.QueryFirstOrDefaultAsync<InteracoesDTO>(sql, new { Id = id });
        }

        public async Task<EncontrosResponseDetalhado> BuscarEncontroPorIdComComentarios(string id)
        {
            var conn = dapperConnection.GetConnection();

            // Query consolidada
            const string multiQuery = @"
                        -- 1. Dados principais do encontro (interação)
                        SELECT 
                            tbi.id AS Id,
                            tbi.tb_colaborador_codigo_interno_colaborador AS CodigoInternoColaborador,
                            tc.nome_completo AS NomeCompletoColaboradorCriador,
                            tbi.data_requisicao AS DataRequisicao,
                            tbi.titulo_interacao AS TituloInteracao,
                            tbi.descricao_interacao AS DescricaoInteracao,
                            tbi.resumo_interacao AS ResumoInteracao,
                            tbi.tb_agendas_comerciais_id AS AgendaId
                        FROM tb_interacoes tbi
                        INNER JOIN tb_colaborador tc 
	                            ON tc.codigo_interno_colaborador = tbi.tb_colaborador_codigo_interno_colaborador 
                        WHERE id = @Id;

                        -- 2. Arquivos
                        SELECT 
                            id AS Id,
                            data_archived AS DataArquivo,
                            link_audio_interacao AS LinkAudioInteracao,
                            link_imagem_interacao AS LinkImagemInteracao,
                            transcricao AS Transcricao
                        FROM tb_interacoes_archives
                        WHERE tb_interacoes_id = @Id;

                        -- 3. IA do encontro
                        SELECT 
                            id AS Id,
                            tb_interacoes_id AS EncontroId,
                            resumo AS Resumo,
                            data_gerada AS DataGerada
                        FROM tb_interacao_ai
                        WHERE tb_interacoes_id = @Id;

                        -- 4. Passos/Ações da IA
                        SELECT 
                            teap.id AS Id,
                            teap.tb_interacao_ai_id AS EncontroAiId,
                            teap.texto AS Texto,
                            teap.tb_colaborador_codigo_interno_colaborador AS CodigoColaborador,
                            tc.nome_completo AS NomeColaborador,
                            teap.data_limite AS DataLimite,
                            teap.tb_status_acoes_id AS StatusAcoesId
                        FROM tb_interacao_acoes teap
                        LEFT JOIN tb_colaborador tc 
                            ON tc.codigo_interno_colaborador = teap.tb_colaborador_codigo_interno_colaborador
                        WHERE teap.tb_interacao_ai_id IN (
                            SELECT id FROM tb_interacao_ai WHERE tb_interacoes_id = @Id
                        );

                        -- 5. Comentários das ações
                        SELECT 
                            tcic.id AS Id,
                            tcic.tb_interacao_acoes_id AS InteracaoAcoesId,
                            tcic.tb_colaborador_codigo_interno_colaborador AS CodInternoColaborador,
                            tc.nome_completo AS NomeColaborador,
                            tcic.data AS Data,
                            tcic.comentario AS Comentario
                        FROM tb_comentarios_interacao_acoes tcic
                        LEFT JOIN tb_colaborador tc 
                            ON tc.codigo_interno_colaborador = tcic.tb_colaborador_codigo_interno_colaborador
                        WHERE tcic.tb_interacao_acoes_id IN (
                            SELECT teap.id 
                            FROM tb_interacao_acoes teap
                            WHERE teap.tb_interacao_ai_id IN (
                                SELECT id FROM tb_interacao_ai WHERE tb_interacoes_id = @Id 
                            )
                        );                        

                        -- 6. Categorias e Subcategorias
                        SELECT 
                            tcs.id AS Id,
                            tcs.tb_interacoes_id AS InteracaoId,
                            tcs.tb_categoria_assunto_id AS CategoriaId,
                            tca.descricao AS CategoriaDescricao,
                            tcs.tb_subcategoria_assunto_id AS SubcategoriaId,
                            tsa.descricao AS SubcategoriaDescricao,
                            tcs.data_criacao AS DataCriacao,
                            tcs.ativo AS Ativo
                        FROM tb_interacoes_categoria_sub tcs
                        INNER JOIN tb_categoria_assunto tca 
                            ON tca.id = tcs.tb_categoria_assunto_id
                        INNER JOIN tb_subcategoria_assunto tsa 
                            ON tsa.tb_categoria_assunto_id = tcs.tb_categoria_assunto_id 
                            AND tsa.id = tcs.tb_subcategoria_assunto_id
                        WHERE tcs.tb_interacoes_id = @Id 
                            AND tcs.ativo = 1;
                            ";

            // Executa a query
            var multi = await conn.QueryMultipleAsync(multiQuery, new { Id = id });

            // 1. Lê encontro principal
            var encontroBase = await multi.ReadFirstOrDefaultAsync<EncontrosResponse>();
            if (encontroBase == null) return null;

            var encontro = new EncontrosResponseDetalhado
            {
                Id = encontroBase.Id,
                CodigoInternoColaborador = encontroBase.CodigoInternoColaborador,
                NomeCompletoColaboradorCriador = encontroBase.NomeCompletoColaboradorCriador,
                DataRequisicao = encontroBase.DataRequisicao,
                AgendaId = encontroBase.AgendaId,
                TituloInteracao = encontroBase.TituloInteracao,
                DescricaoInteracao = encontroBase.DescricaoInteracao,
                ResumoInteracao = encontroBase.ResumoInteracao
            };

            // 2. Lê arquivos 
            encontro.Arquivos = (await multi.ReadAsync<ArquivoEncontroDto>()).ToList();

            // 3. Lê IA do encontro
            var encontroAiBase = await multi.ReadFirstOrDefaultAsync<EncontroAi>();

            if (encontroAiBase != null)
            {
                // Cria o DTO detalhado da IA
                encontro.EncontroAi = new EncontroAiDetalhado
                {
                    Id = encontroAiBase.Id,
                    EncontroId = encontroAiBase.EncontroId,
                    Resumo = encontroAiBase.Resumo,
                    DataGerada = encontroAiBase.DataGerada
                };

                // 4. Lê passos/ações
                var passos = (await multi.ReadAsync<EncontroAiPassosDetalhado>()).ToList();

                // 5. Lê comentários 
                var comentarios = (await multi.ReadAsync<ComentariosAcoesResponseDTO>()).ToList();

                // Agrupa
                var comentariosPorAcao = comentarios
                    .ToLookup(c => c.InteracaoAcoesId);

                // Atribui comentários a cada passo
                foreach (var passo in passos)
                {
                    passo.Comentarios = comentariosPorAcao[passo.Id].ToList();
                }

                encontro.EncontroAi.Passos = passos;
            }
            else
            {
                // Consome os resultsets restantes mesmo se EncontroAi for null
                await multi.ReadAsync<EncontroAiPassosDetalhado>();
                await multi.ReadAsync<ComentariosAcoesResponseDTO>();
            }

            // 6. Lê categorias e subcategorias
            encontro.CategoriasSubcategorias = (await multi.ReadAsync<CategoriaSubcategoriaDTO>()).ToList();

            return encontro;
        }
        public async Task<IEnumerable<InteracoesCategoriaSubResponseDTO>> ListarCategoriasSubPorInteracaoId(int interacaoId)
        {
            var conn = dapperConnection.GetConnection();

            var encontro = await conn.QueryAsync<InteracoesCategoriaSubResponseDTO>(
                @"SELECT    tic.id AS Id, 
                    tic.tb_interacoes_id AS InteracaoId,
                    tic.tb_categoria_assunto_id AS CategoriaAssuntoId,
                    tca.descricao AS DescricaoCategoria,
                    tic.tb_subcategoria_assunto_id AS SubCategoriaAssuntoId,
                    tcs.descricao AS DescricaoSubCategoria
        FROM tb_interacoes_categoria_sub tic
        LEFT JOIN tb_categoria_assunto tca on tca.id = tic.tb_categoria_assunto_id
        LEFT JOIN tb_subcategoria_assunto tcs on tcs.id = tic.tb_subcategoria_assunto_id 
                AND tcs.tb_categoria_assunto_id  = tic.tb_categoria_assunto_id AND tcs.ativo = 1
        WHERE tb_interacoes_id = @InteracaoId
        AND tic.ativo = 1;"
                , new { InteracaoId = interacaoId });

            if (encontro == null)
                return null;

            return encontro;
        }
        public async Task<bool> deletarEncontro(string id)
        {
            var conn = dapperConnection.GetConnection();
            const string sql = @"
                DELETE FROM tb_interacoes_categoria_sub WHERE tb_interacoes_id = @Id;
                DELETE FROM tb_interacoes WHERE id = @Id;
            ";

            var totalLinhasAfetadas = await conn.ExecuteAsync(sql, new { Id = id });

            return totalLinhasAfetadas > 0;
        }

        public async Task<EncontrosResponse> buscarEncontroPorColaboradorId(string colabId)
        {
            var conn = dapperConnection.GetConnection();
            var id = await conn.ExecuteScalarAsync<int?>(
                "SELECT id FROM tb_interacoes WHERE tb_colaborador_codigo_interno_colaborador = @Id ORDER BY id DESC LIMIT 1;",
                new { Id = colabId });

            return id.HasValue ? await buscarEncontroPorId(id.Value.ToString()) : null;
        }

        public async Task<List<EncontrosResponse>> buscarTodosEncontros()
        {
            var conn = dapperConnection.GetConnection();

            // 1. Busca IDs do último mês
            var ids = (await conn.QueryAsync<int>(
                "SELECT DISTINCT id FROM tb_interacoes WHERE data_requisicao > (NOW() - INTERVAL 1 MONTH)"
            )).ToList();

            if (!ids.Any())
                return new List<EncontrosResponse>();

            // 2. SQL usando o parâmetro @ids 
            // tb_interacoes
            const string sql = @$"
                {QEncontroBase} WHERE id IN @ids;

                SELECT id AS Id, tb_interacoes_id AS InteracaoId, resumo, data_gerada 
                FROM tb_interacao_ai 
                WHERE tb_interacoes_id IN @ids;

                SELECT teap.id AS Id, teap.tb_interacao_ai_id AS EncontroAiId, teap.texto AS Texto, 
                       tc.codigo_interno_colaborador AS CodigoColaborador, tc.nome_completo AS NomeColaborador, teap.data_limite AS DataLimite
                FROM tb_interacao_acoes teap
                INNER JOIN tb_interacao_ai tai ON tai.id = teap.tb_interacao_ai_id
                INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = teap.tb_colaborador_codigo_interno_colaborador
                WHERE tai.tb_interacoes_id IN @ids;

                SELECT tcic.id AS Id, tcic.tb_interacao_acoes_id AS InteracaoAcoesId, tcic.comentario, 
                       tc.nome_completo AS NomeColaborador, tcic.data AS Data
                FROM tb_comentarios_interacao_acoes tcic
                LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = tcic.tb_colaborador_codigo_interno_colaborador
                WHERE tcic.tb_interacao_acoes_id IN (
                    SELECT id FROM tb_interacao_acoes WHERE tb_interacao_ai_id IN (
                        SELECT id FROM tb_interacao_ai WHERE tb_interacoes_id IN @ids
                    )
                );

                SELECT tcs.id AS Id, tcs.tb_interacoes_id AS InteracaoId, tcs.id AS InteracaoCategoriaId,
                       tcs.tb_categoria_assunto_id AS CategoriaAssuntoId,
                       tcs.tb_subcategoria_assunto_id AS SubCategoriaAssuntoId, tcs.ativo AS Ativo, tsa.data_criacao AS DataCriacao
                FROM tb_interacoes_categoria_sub tcs
                INNER JOIN tb_categoria_assunto tca ON tca.id = tcs.tb_categoria_assunto_id
                INNER JOIN tb_subcategoria_assunto tsa ON tsa.id = tcs.tb_subcategoria_assunto_id 
                                                        AND tsa.tb_categoria_assunto_id = tcs.tb_categoria_assunto_id
                WHERE tcs.tb_interacoes_id IN @ids AND tcs.ativo = 1;

                SELECT  id AS Id, tb_interacoes_id AS InteracaoId, 
                        link_audio_interacao AS LinkImagemInteracao, 
                        link_imagem_interacao AS LinkAudioInteracao,
                        transcricao AS Transcricao,
                        data_archived AS DataArquivo
                FROM tb_interacoes_archives 
                WHERE tb_interacoes_id IN @ids;
            ";

            // 3. Lista de IDs como parâmetro
            var multi = await conn.QueryMultipleAsync(sql, new { ids });

            var todosEncontros = (await multi.ReadAsync<EncontrosResponse>()).ToList();
            var todosAis = (await multi.ReadAsync<EncontroAiRelacional>()).ToLookup(x => x.InteracaoId);
            var todosPassos = (await multi.ReadAsync<EncontroAiPassosRelacional>()).ToLookup(x => x.EncontroAiId);
            var todosComentarios = (await multi.ReadAsync<ComentariosAcoesResponseDTO>()).ToLookup(x => x.InteracaoAcoesId);
            var todasCategorias = (await multi.ReadAsync<InteracaoCategoriaSubResponseDTO>()).ToLookup(x => x.InteracaoId);
            var todosArquivos = (await multi.ReadAsync<ArquivoRelacional>()).ToLookup(x => x.InteracaoId);

            // 4. Montagem dos objetos
            foreach (var enc in todosEncontros)
            {
                enc.Arquivos = todosArquivos[enc.Id].Cast<ArquivoEncontroDto>().ToList();
                enc.Categorias = todasCategorias[enc.Id].ToList();

                var ai = todosAis[enc.Id].FirstOrDefault();
                if (ai != null)
                {
                    var passos = todosPassos[ai.Id].ToList();
                    foreach (var p in passos)
                    {
                        p.ComentariosAcoes = todosComentarios[p.Id].ToList();
                    }
                    ai.Passos = passos.Cast<EncontroAiPassos>().ToList();
                    enc.EncontroAi = ai;
                }
            }

            return todosEncontros;
        }

        // ---------------------------------------------------------------------
        // Agendas – consultas
        // ---------------------------------------------------------------------
        public async Task<List<AgendaEncontroResult>> buscarAgendaEncontros(string cpfUsuarioLogado, string dataInicio, string dataFim)
        {
            var conn = dapperConnection.GetConnection();

            // 1. Filtro de Datas
            string where = (!string.IsNullOrWhiteSpace(dataInicio) && !string.IsNullOrWhiteSpace(dataFim))
                ? " WHERE tae.data_agendada BETWEEN @DataInicio AND @DataFim "
                : " WHERE tae.data_agendada BETWEEN DATE_SUB(NOW(), INTERVAL 10 DAY) AND DATE_ADD(NOW(), INTERVAL 20 DAY) ";

            // 2) Montagem do SQL 
            string sql = $@"
                        -- 1. Agendas
                        {QAgendaBase} {where};

                        -- 2. Clientes
                        SELECT DISTINCT tco.codigo_cliente AS CodigoCliente,
                               tco.nome_cliente AS NomeCliente,
                               tae.id AS AgendaId
                        FROM tb_agendas_comerciais tae
                        INNER JOIN tb_cliente_org tco ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente
                        {where};

                        -- 3. Gestores Externos
                        SELECT  g.cod_gestor_externo AS CodGestorExterno, g.nome AS Nome, g.email AS Email, p.tb_agendas_comerciais_id AS AgendaId,
                                p.data_convite AS DataCriacao, NULL AS DataAlteracao, g.telefone AS Telefone, g.codigo_cliente AS CodigoCliente, g.perfil_linkedin AS PerfilLinkedin
                        FROM tb_agenda_convidados p
                        INNER JOIN tb_gestor_externo g ON g.cod_gestor_externo = p.codigo_colaborador_interno_externo
                        WHERE p.tb_agendas_comerciais_id IN (SELECT id FROM tb_agendas_comerciais tae {where})
                        AND   p.tipo_codigo = 2;

                        -- 4. Colaboradores
                        SELECT DISTINCT p.codigo_colaborador_interno_externo AS CodInternoColaborador, 
                               tc.nome_completo AS Nome, 
                               tu.email AS Email, p.tb_agendas_comerciais_id AS AgendaId
                        FROM tb_agenda_convidados p
                        INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = p.codigo_colaborador_interno_externo
                        LEFT JOIN tb_usuario tu ON tu.codigo_interno_colaborador = p.codigo_colaborador_interno_externo
                        WHERE p.tb_agendas_comerciais_id IN (SELECT id FROM tb_agendas_comerciais tae {where})
                        AND   p.tipo_codigo = 1;

                        -- 5. Participantes Externos
                        SELECT nome AS Nome, email AS Email, tb_agendas_comerciais_id AS AgendaId 
                        FROM tb_participantes_externo 
                        WHERE tb_agendas_comerciais_id IN (SELECT id FROM tb_agendas_comerciais tae {where});

                        -- 6. Participantes
                        SELECT id, COALESCE(g.nome, tc.nome_completo) AS Nome,
                               CASE WHEN p.tipo_codigo = 2 THEN p.codigo_colaborador_interno_externo END AS CodigoGestorExterno,
                               CASE WHEN p.tipo_codigo = 1 THEN p.codigo_colaborador_interno_externo END AS CodigoColaborador,
                               p.tb_status_app_id AS Status, p.data_resposta AS DataResposta, 
                               p.data_convite AS DataConvite,
                               p.tb_agendas_comerciais_id AS AgendaId
                        FROM tb_agenda_convidados p
                        LEFT JOIN tb_gestor_externo g ON g.cod_gestor_externo = p.codigo_colaborador_interno_externo AND g.tb_org_id = 2
                        LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = p.codigo_colaborador_interno_externo
                        WHERE p.tb_agendas_comerciais_id IN (SELECT id FROM tb_agendas_comerciais tae {where});

                        -- 7. Status Solicitação 
                        SELECT tasp.tb_status_app_id AS status 
                        FROM tb_agenda_solicitante tasp
                        WHERE tasp.codigo_colaborador_interno_externo = @Cpf
                        AND   tasp.tb_agendas_comerciais_id IN (SELECT id FROM tb_agendas_comerciais tae {where});

                        -- 8. Participação Usuário Logado
                        SELECT tb_status_app_id AS Status, data_resposta AS DataResposta, tb_agendas_comerciais_id AS AgendaId
                        FROM tb_agenda_convidados
                        WHERE codigo_colaborador_interno_externo = @Cpf
                          AND tb_agendas_comerciais_id IN (SELECT id FROM tb_agendas_comerciais tae {where})
                        LIMIT 1;
                    ";

            var paramsData = new { DataInicio = dataInicio, DataFim = dataFim, Cpf = cpfUsuarioLogado };

            var multi = await conn.QueryMultipleAsync(sql, paramsData);

            // 3) Leitura e Mapeamento em Memória 
            var agendas = (await multi.ReadAsync<AgendaEncontroResult>()).ToList();
            if (!agendas.Any())
                return agendas;

            var clientesLookup = (await multi.ReadAsync<dynamic>()).ToLookup(x => (int)x.AgendaId);
            var gestoresLookup = (await multi.ReadAsync<dynamic>()).ToLookup(x => (int)x.AgendaId);
            var colaboradoresLookup = (await multi.ReadAsync<dynamic>()).ToLookup(x => (int)x.AgendaId);
            var externosLookup = (await multi.ReadAsync<dynamic>()).ToLookup(x => (int)x.AgendaId);
            var participantesLookup = (await multi.ReadAsync<dynamic>()).ToLookup(x => (int)x.AgendaId);
            var statusLookup = (await multi.ReadAsync<StatusReadDTO>()).ToLookup(x => x.AgendaId);
            var participacaoLogadoLookup = (await multi.ReadAsync<ParticipacaoUsuarioConvidado>()).ToLookup(x => x.AgendaId);

            // 4) Monta os objetos 
            foreach (var a in agendas)
            {
                var clienteRow = clientesLookup[a.Id].FirstOrDefault();
                if (clienteRow != null)
                {
                    a.Cliente = new ClienteOrgDaGestaoAlocadosResult
                    {
                        CodigoCliente = clienteRow.CodigoCliente,
                        NomeCliente = clienteRow.NomeCliente
                    };
                }

                a.GestoresExternos = gestoresLookup[a.Id]
                    .Select(x => new GestorExternoResult
                    {
                        CodGestorExterno = x.CodGestorExterno,
                        Nome = x.Nome,
                        Email = x.Email,
                        Telefone = x.Telefone,
                        CodigoCliente = x.CodigoCliente,
                        PerfilLinkedin = x.PerfilLinkedin
                    })
                    .ToList();
                a.Colaboradores = colaboradoresLookup[a.Id].Select(x => new ColaboradorAgenda { CodInternoColaborador = x.CodInternoColaborador, Nome = x.Nome, Email = x.Email }).ToList();
                a.ParticipantesExterno = externosLookup[a.Id].Select(x => new ParticipanteExterno { Nome = x.Nome, Email = x.Email }).ToList();
                a.Participantes = participantesLookup[a.Id]
                    .Select(x => new ParticipanteConvidadoDto
                    {
                        Id = x.id ?? 0,
                        Nome = x.Nome,
                        CodigoGestorExterno = x.CodigoGestorExterno,
                        CodigoColaborador = x.CodigoColaborador,
                        Status = x.Status ?? 0,
                        DataResposta = x.DataResposta,
                        DataConvite = x.DataConvite
                    })
                    .ToList();

                a.Encontros = await buscarEncontrosPorAgendaId(a.Id.ToString());
                //a.StatusSolicitacaoParticipante = await BuscarStatusSolicitacaoParticipante(conn, a.Id, cpfUsuarioLogado);
                //a.ParticipacaoUsuarioLogado = await BuscarParticipacaoUsuarioLogado(conn, a.Id, cpfUsuarioLogado);

                var row = statusLookup[a.Id].FirstOrDefault();
                a.StatusSolicitacaoParticipante = row?.Status != null
                    ? (StatusSolicitacaoParticipante)row.Status.Value
                    : StatusSolicitacaoParticipante.Pendente;

                a.ParticipacaoUsuarioLogado = participacaoLogadoLookup[a.Id].FirstOrDefault();

            }

            return agendas;
        }


        private async Task<StatusSolicitacaoParticipante?> BuscarStatusSolicitacaoParticipante(MySqlConnection conn, int agendaId, string cpfUsuarioLogado)
        {

            return await conn.QueryFirstOrDefaultAsync<StatusSolicitacaoParticipante?>("""
                                                                        SELECT tasp.tb_status_app_id AS status
                                                                        FROM tb_agenda_solicitante tasp
                                                                        WHERE tasp.codigo_colaborador_interno_externo = @cpfUsuarioLogado
                                                                        AND tasp.tb_agendas_comerciais_id = @AgendaId
                                                                        """, param: new { AgendaId = agendaId, cpfUsuarioLogado });
        }

        private async Task<ParticipacaoUsuarioConvidado?> BuscarParticipacaoUsuarioLogado(MySqlConnection conn, int agendaId, string cpfUsuarioLogado)
        {

            return await conn.QueryFirstOrDefaultAsync<ParticipacaoUsuarioConvidado?>("""
                                                                           SELECT  tb_status_app_id AS Status,
                                                                       		       data_resposta AS DataResposta
                                                                           FROM tb_agenda_convidados
                                                                           WHERE codigo_colaborador_interno_externo = @cpfUsuarioLogado
                                                                       	      AND tb_agendas_comerciais_id = @agendaId
                                                                       """, param: new { cpfUsuarioLogado, agendaId });
        }

        public async Task<List<AgendaEncontroResult>> buscarAgendaEncontrosPorColaboradorId(string colabId)
        {
            var conn = dapperConnection.GetConnection();
            var q = $"{QAgendaBase} WHERE tb_colaborador_codigo_interno_colaborador = @Id;";
            return (await conn.QueryAsync<AgendaEncontroResult>(q, new { Id = colabId })).AsList();
        }

        public async Task<List<AgendaEncontroResult>> buscarAgendaEncontrosPorData(string dia)
        {
            var conn = dapperConnection.GetConnection();
            var q = $"{QAgendaBase} WHERE DATE(data_agendada) = @Dia;";
            return (await conn.QueryAsync<AgendaEncontroResult>(q, new { Dia = dia })).AsList();
        }

        public async Task<List<EncontrosResponse>> buscarEncontrosPorAgendaId(string agendaId)
        {
            var conn = dapperConnection.GetConnection();

            try
            {

                // 1) Busca TODOS os encontros daquela agenda de uma vez
                var sql = $@"
                        -- 1. Dados Base de todos os encontros
                        {QEncontroBase} WHERE tb_agendas_comerciais_id = @AgendaId;

                        -- 2. Todos os encontros AI dessa agenda
                        SELECT id AS Id, 
                               tb_interacoes_id AS InteracaoId, 
                               tb_interacoes_id AS EncontroId,
                               resumo AS Resumo, 
                               data_gerada AS DataGerada
                        FROM tb_interacao_ai 
                        WHERE tb_interacoes_id IN (SELECT id FROM tb_interacoes WHERE tb_agendas_comerciais_id = @AgendaId);

                        -- 3. Todas as Ações (Passos)
                        SELECT teap.id AS Id, teap.tb_interacao_ai_id AS EncontroAiId, teap.texto AS Texto, 
                               teap.tb_colaborador_codigo_interno_colaborador AS CodigoColaborador, 
                               tc.nome_completo AS NomeColaborador, 
                               teap.data_limite AS DataLimite,
                               teap.tb_status_acoes_id AS StatusAcoesId
                        FROM tb_interacao_acoes teap
                        INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = teap.tb_colaborador_codigo_interno_colaborador
                        WHERE teap.tb_interacao_ai_id IN (SELECT id FROM tb_interacao_ai WHERE tb_interacoes_id IN (SELECT id FROM tb_interacoes WHERE tb_agendas_comerciais_id = @AgendaId));

                        -- 4. Todos os Comentários
                        SELECT tcic.id AS Id, tcic.tb_interacao_acoes_id AS InteracaoAcoesId, tcic.tb_colaborador_codigo_interno_colaborador AS CodInternoColaborador,
                               tc.nome_completo AS NomeColaborador, tcic.data AS Data, tcic.comentario AS Comentario
                        FROM tb_comentarios_interacao_acoes tcic
                        LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = tcic.tb_colaborador_codigo_interno_colaborador
                        WHERE tcic.tb_interacao_acoes_id IN (
                            SELECT id FROM tb_interacao_acoes WHERE tb_interacao_ai_id IN (
                                SELECT id FROM tb_interacao_ai WHERE tb_interacoes_id IN (SELECT id FROM tb_interacoes WHERE tb_agendas_comerciais_id = @AgendaId)
                            )
                        );

                        -- 5. Todas as Categorias
                        SELECT tcs.id AS InteracaoCategoriaId, tcs.tb_interacoes_id AS InteracaoId, tcs.tb_categoria_assunto_id AS CategoriaAssuntoId, tca.descricao AS CategoriaDescricao,
                               tcs.tb_subcategoria_assunto_id AS SubCategoriaAssuntoId, tsa.descricao AS SubcategoriaDescricao, tcs.data_criacao AS DataCriacao, tcs.ativo AS Ativo
                        FROM tb_interacoes_categoria_sub tcs
                        INNER JOIN tb_categoria_assunto tca ON tca.id = tcs.tb_categoria_assunto_id
                        INNER JOIN tb_subcategoria_assunto tsa ON tsa.tb_categoria_assunto_id = tcs.tb_categoria_assunto_id AND tsa.id = tcs.tb_subcategoria_assunto_id
                        WHERE tcs.tb_interacoes_id IN (SELECT id FROM tb_interacoes WHERE tb_agendas_comerciais_id = @AgendaId) AND tcs.ativo = 1;

                        -- 6. Todos os Arquivos
                        SELECT id AS Id, tb_interacoes_id AS InteracaoId, data_archived AS DataArquivo, link_audio_interacao AS LinkAudioInteracao,
                               link_imagem_interacao AS LinkImagemInteracao, transcricao AS Transcricao
                        FROM tb_interacoes_archives 
                        WHERE tb_interacoes_id IN (SELECT id FROM tb_interacoes WHERE tb_agendas_comerciais_id = @AgendaId);
                    ";

                var multi = await conn.QueryMultipleAsync(sql, new { AgendaId = agendaId });

                // 2) Leitura dos dados
                var encontros = (await multi.ReadAsync<EncontrosResponse>()).ToList();
                var todosAi = (await multi.ReadAsync<EncontroAiRelacional>()).ToList();
                var todosPassos = (await multi.ReadAsync<EncontroAiPassos>()).ToList();
                var todosComentarios = (await multi.ReadAsync<ComentariosAcoesResponseDTO>()).ToList();
                var todasCategorias = (await multi.ReadAsync<InteracaoCategoriaSubResponseDTO>()).ToList();
                var todosArquivos = (await multi.ReadAsync<ArquivoEncontroDtoRelacional>()).ToList();

                // 3) Criar dicionários para mapeamento rápido
                // IMPORTANTE: Pode haver múltiplos AI por Interação, então usamos GroupBy
                var dictAiPorInteracao = todosAi.GroupBy(a => a.InteracaoId).ToDictionary(g => g.Key, g => g.ToList());
                var dictPassosPorAiId = todosPassos.GroupBy(p => p.EncontroAiId).ToDictionary(g => g.Key, g => g.ToList());
                var dictComentarios = todosComentarios.GroupBy(c => c.InteracaoAcoesId).ToDictionary(g => g.Key, g => g.ToList());
                var dictCategorias = todasCategorias.GroupBy(c => c.InteracaoId).ToDictionary(g => g.Key, g => g.ToList());
                var dictArquivos = todosArquivos.GroupBy(a => a.InteracaoId).ToDictionary(g => g.Key, g => g.ToList());

                // 4) Montar a estrutura completa
                foreach (var enc in encontros)
                {
                    // Vincula TODOS os AI dessa Interação
                    if (dictAiPorInteracao.TryGetValue(enc.Id, out var listaAi) && listaAi.Any())
                    {
                        // Pega o AI mais recente (último da lista)
                        var ai = listaAi.OrderByDescending(a => a.Id).First();
                        
                        // Vincula Passos ao AI
                        if (dictPassosPorAiId.TryGetValue(ai.Id, out var passos))
                        {
                            // Vincula Comentários aos Passos
                            foreach (var passo in passos)
                            {
                                passo.ComentariosAcoes = dictComentarios.TryGetValue(passo.Id, out var comentarios) 
                                    ? comentarios 
                                    : new List<ComentariosAcoesResponseDTO>();
                            }
                            ai.Passos = passos;
                        }
                        else
                        {
                            ai.Passos = new List<EncontroAiPassos>();
                        }
                        
                        enc.EncontroAi = ai;
                    }

                    // Vincula Categorias e Arquivos
                    enc.Categorias = dictCategorias.TryGetValue(enc.Id, out var cats) ? cats : new List<InteracaoCategoriaSubResponseDTO>();
                    enc.Arquivos = dictArquivos.TryGetValue(enc.Id, out var arqs) ? arqs.Cast<ArquivoEncontroDto>().ToList() : new List<ArquivoEncontroDto>();
                }

                return encontros;

            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<List<string>> buscarDeviceTokensAppEmLote(List<string> codigosInternoColaborador)
        {
            var conn = dapperConnection.GetConnection();

            var sql = @"
                        SELECT tu.fcm_token
                        FROM tb_usuario tu
                        WHERE tu.codigo_interno_colaborador IN @Codigos;";

            return (await conn.QueryAsync<string>(sql, new { Codigos = codigosInternoColaborador })).AsList();
        }

        public async Task<string> buscarDeviceTokenApp(string codigoInternoColaborador)
        {
            var conn = dapperConnection.GetConnection();

            var sql = @"
                        SELECT tu.fcm_token
                        FROM tb_usuario tu
                        WHERE tu.codigo_interno_colaborador = @Codigo;";

            return await conn.QueryFirstOrDefaultAsync<string>(sql, new { Codigo = codigoInternoColaborador });
        }

        public async Task<EncontroAi> CriarEncontroAi(EncontroAi ai)
        {
            var conn = dapperConnection.GetConnection();

            const string insertAi = """
                                    INSERT INTO tb_interacao_ai (tb_interacoes_id, resumo)
                                    VALUES (@EncontroId, @Resumo);
                                    SELECT LAST_INSERT_ID();
                                    """;

            ai.Id = await conn.QuerySingleAsync<int>(insertAi, new { ai.EncontroId, ai.Resumo });

            if (ai.Passos?.Count > 0)
            {
                const string insertPasso = """
                                            INSERT INTO tb_interacao_acoes
                                            (tb_interacao_ai_id, texto, tb_colaborador_codigo_interno_colaborador, data_limite)
                                            VALUES (@AiId, @Texto, @Colab, @Limite);
                                           """;

                await conn.ExecuteAsync(insertPasso,
                    ai.Passos.Select(p => new
                    {
                        AiId = ai.Id,
                        Texto = p.Texto,
                        Colab = p.CodigoColaborador,
                        Limite = p.DataLimite
                    }));
            }

            return ai;
        }

        public async Task<EncontroAi> AtualizarEncontroAi(EncontroAi ai)
        {
            var conn = dapperConnection.GetConnection();
            var tx = await conn.BeginTransactionAsync();

            const string updateAi = "UPDATE tb_interacao_ai SET resumo = @Resumo WHERE id = @Id;";
            await conn.ExecuteAsync(updateAi, new { ai.Resumo, ai.Id }, tx);

            const string deletePassos = "DELETE FROM tb_interacao_acoes WHERE tb_interacao_ai_id IN (SELECT id FROM tb_interacao_ai WHERE tb_interacoes_id = @AiId); ";
            await conn.ExecuteAsync(deletePassos, new { AiId = ai.EncontroId }, tx);

            if (ai.Passos?.Count > 0)
            {
                const string insertPasso = @"
                    INSERT INTO tb_interacao_acoes
                           (tb_interacao_ai_id, texto, tb_colaborador_codigo_interno_colaborador, data_limite)
                    VALUES (@AiId, @Texto, @Colab, @Limite);";

                await conn.ExecuteAsync(insertPasso,
                    ai.Passos.Select(p => new
                    {
                        AiId = ai.Id,
                        Texto = p.Texto,
                        Colab = p.CodigoColaborador,
                        Limite = p.DataLimite
                    }), tx);
            }

            await tx.CommitAsync();
            return await BuscarEncontroAiPorEncontroId(ai.EncontroId);
        }

        public async Task<bool> DeletarEncontroAi(int id)
        {
            var conn = dapperConnection.GetConnection();
            return await conn.ExecuteAsync("DELETE FROM tb_interacao_ai WHERE id = @Id;", new { Id = id }) > 0;
        }

        public async Task<EncontroAi> BuscarEncontroAiPorEncontroId(int encontroId)
        {
            var conn = dapperConnection.GetConnection();
            var ai = await conn.QueryFirstOrDefaultAsync<EncontroAi>(@"
                SELECT id AS Id, tb_interacoes_id AS EncontroId, resumo AS Resumo, data_gerada AS DataGerada
                FROM tb_interacao_ai WHERE tb_interacoes_id = @Id;", new { Id = encontroId });

            if (ai != null)
            {
                ai.Passos = (await conn.QueryAsync<EncontroAiPassos>(@"
                    SELECT  teap.id AS Id, 
		                    teap.texto AS Texto, 
                            teap.tb_colaborador_codigo_interno_colaborador AS CodigoColaborador,
                            tc.nome_completo AS NomeColaborador,
                            teap.data_limite AS DataLimite
                    FROM tb_interacao_acoes teap
                    INNER JOIN tb_colaborador tc 
	                    ON tc.codigo_interno_colaborador = teap.tb_colaborador_codigo_interno_colaborador 
                    WHERE teap.tb_interacao_ai_id = @Id;", new { Id = ai.Id })).ToList();
            }

            return ai;
        }

        public async Task<string> BuscarNomeColaborador(string codigoColaborador)
        {
            var conn = dapperConnection.GetConnection();

            var nomeCompleto = await conn.QueryFirstOrDefaultAsync<string>(@"   SELECT tc.nome_completo
                                                                                FROM tb_colaborador tc 
                                                                                WHERE tc.codigo_interno_colaborador = @codigoColaborador
                                                                                ", new { codigoColaborador });

            return nomeCompleto;
        }

        public async Task<List<SolicitacaoResponse>> BuscarSolicitacoesAgendas(string cpfUsuarioLogado, int cursor, int limite)
        {
            var conn = dapperConnection.GetConnection();

            var solicitacoes = (await conn.QueryAsync<SolicitacaoResponse>(@"   SELECT  tasp.tb_agendas_comerciais_id AS AgendaId,
		                                                                                tasp.tb_colaborador_codigo_interno_colaborador_criador AS CodigoInternoColaboradorCriador,
		                                                                                tasp.codigo_colaborador_interno_externo AS CodigoInternoColaboradorSolicitante,
                                                                                        tcs.nome_completo AS NomeColaboradorSolicitante,
                                                                                        tcc.nome_completo AS NomeColaboradorCriador,
		                                                                                tasp.tb_status_app_id,
		                                                                                tasp.data_solicitacao AS DataSolicitacao,
		                                                                                tasp.data_resposta AS DataDecisao
                                                                                FROM tb_agenda_solicitante tasp
                                                                                INNER JOIN tb_colaborador tcs
	                                                                                ON tcs.codigo_interno_colaborador = tasp.codigo_colaborador_interno_externo
                                                                                INNER JOIN tb_colaborador tcc
	                                                                                ON tcc.codigo_interno_colaborador = tasp.tb_colaborador_codigo_interno_colaborador_criador 
                                                                                WHERE tasp.codigo_colaborador_interno_externo = @Id
                                                                                ORDER BY AgendaId
                                                                                LIMIT 
                                                                                    @Limite
                                                                                OFFSET
                                                                                    @Cursor;
                                                                                ", new { Id = cpfUsuarioLogado, Limite = limite, Cursor = cursor })).ToList();
            
            return solicitacoes;
        }

        public async Task<List<SolicitacaoResponse>> BuscarSolicitacoesMinhasAgendas(string cpfUsuarioLogado, int cursor, int limite)
        {
            var conn = dapperConnection.GetConnection();

            var solicitacoes = (await conn.QueryAsync<SolicitacaoResponse>(@"   SELECT  tasp.tb_agendas_comerciais_id AS AgendaId,
		                                                                                tasp.tb_colaborador_codigo_interno_colaborador_criador AS CodigoInternoColaboradorCriador,
		                                                                                tasp.codigo_colaborador_interno_externo AS CodigoInternoColaboradorSolicitante,
		                                                                                tcs.nome_completo AS NomeColaboradorSolicitante,
                                                                                        tcc.nome_completo AS NomeColaboradorCriador,
                                                                                        tasp.tb_status_app_id,
		                                                                                tasp.data_solicitacao AS DataSolicitacao,
		                                                                                tasp.data_resposta AS DataDecisao
                                                                                FROM tb_agenda_solicitante tasp
                                                                                INNER JOIN tb_colaborador tcs
	                                                                                ON tcs.codigo_interno_colaborador = tasp.codigo_colaborador_interno_externo
                                                                                INNER JOIN tb_colaborador tcc
	                                                                                ON tcc.codigo_interno_colaborador = tasp.tb_colaborador_codigo_interno_colaborador_criador 
                                                                                WHERE tasp.tb_colaborador_codigo_interno_colaborador_criador = @Id
                                                                                ORDER BY AgendaId
                                                                                LIMIT 
                                                                                    @Limite
                                                                                OFFSET
                                                                                    @Cursor;
                                                                                ", new { Id = cpfUsuarioLogado, Limite = limite, Cursor = cursor })).ToList();

            return solicitacoes;
        }

        public async Task<List<SolicitacaoResponse>> BuscarSolicitacoesPorAgenda(int agendaId, int cursor, int limite)
        {
            var conn = dapperConnection.GetConnection();

            var solicitacoes = (await conn.QueryAsync<SolicitacaoResponse>(@"   SELECT  tasp.tb_agendas_comerciais_id AS AgendaId,
		                                                                                tasp.tb_colaborador_codigo_interno_colaborador_criador AS CodigoInternoColaboradorCriador,
		                                                                                tasp.codigo_colaborador_interno_externo AS CodigoInternoColaboradorSolicitante,
		                                                                                tcs.nome_completo AS NomeColaboradorSolicitante,
                                                                                        tcc.nome_completo AS NomeColaboradorCriador,
                                                                                        tasp.tb_status_app_id AS status,
		                                                                                tasp.data_solicitacao AS DataSolicitacao,
		                                                                                tasp.data_resposta AS DataDecisao
                                                                                FROM tb_agenda_solicitante tasp
                                                                                INNER JOIN tb_colaborador tcs
                                                                                    ON tcs.codigo_interno_colaborador = tasp.codigo_colaborador_interno_externo
                                                                                INNER JOIN tb_colaborador tcc
                                                                                    ON tcc.codigo_interno_colaborador = tasp.tb_colaborador_codigo_interno_colaborador_criador 
                                                                                WHERE tasp.tb_agendas_comerciais_id = @Id
                                                                                ORDER BY AgendaId
                                                                                LIMIT 
                                                                                    @Limite
                                                                                OFFSET
                                                                                    @Cursor;
                                                                                ", new { Id = agendaId, Limite = limite, Cursor = cursor })).ToList();

            return solicitacoes;
        }

        // Antigo
        public async Task<bool> AprovarReprovarSolicitacaoAgenda(StatusSolicitacaoParticipante decisaoStatus, int agendaId, string codigoColaboradorSolicitante, string cpfUsuarioLogado)
        {
            var conn = dapperConnection.GetConnection();

            try
            {
                var linhasAfetadas = await conn.ExecuteAsync(@"
                                                                UPDATE tb_agenda_solicitante 
                                                                SET tb_status_app_id = @decisaoStatus,
                                                                    data_resposta = NOW()
                                                                WHERE tb_agendas_comerciais_id = @agendaId
                                                                  AND tb_colaborador_codigo_interno_colaborador_criador = @colaboradorCriador
                                                                  AND codigo_colaborador_interno_externo = @colaboradorSolicitante;
                                                            ", new
                {
                    decisaoStatus,
                    agendaId,
                    colaboradorCriador = cpfUsuarioLogado,
                    colaboradorSolicitante = codigoColaboradorSolicitante
                }
                );

                return linhasAfetadas > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AceitarRecusarConviteAgenda(StatusSolicitacaoParticipante decisaoStatus, int agendaId, string codigoColaborador)
        {
            var conn = dapperConnection.GetConnection();

            try
            {
                var linhasAfetadas = await conn.ExecuteAsync(@"
                                                                UPDATE tb_agenda_convidados 
                                                                SET tb_status_app_id = @decisaoStatus,
                                                                    data_resposta = NOW()
                                                                WHERE tb_agendas_comerciais_id = @agendaId
                                                                  AND codigo_colaborador_interno_externo = @codigoColaborador;
                                                            ", new
                {
                    decisaoStatus,
                    agendaId,
                    codigoColaborador
                }
                );

                return linhasAfetadas > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        
        //Antigo
        public async Task<bool> SolicitarParticipacaoAgenda(int agendaId, string codigoColaboradorSolicitante, string codigoColaboradorCriador)
        {
            var conn = dapperConnection.GetConnection();

            try
            {
                // Verifica se já existe a solicitação
                var existe = await conn.QueryFirstOrDefaultAsync<int>(@"
                                                                    SELECT COUNT(1)
                                                                    FROM tb_agenda_solicitante
                                                                    WHERE tb_agendas_comerciais_id = @agendaId
                                                                        AND codigo_colaborador_interno_externo = @codigoColaboradorSolicitante;
                                                                ", new { agendaId, codigoColaboradorSolicitante });

                if (existe > 0)
                    throw new InvalidOperationException("O usuário já solicitou participação nesta agenda.");

                var codigo = codigoColaboradorSolicitante.Trim();
                var tipoCodigo = InferirTipoCodigoPorComprimentoDoCodigo(codigo);

                var linhasAfetadas = await conn.ExecuteAsync(@"
                                                                INSERT INTO tb_agenda_solicitante (
                                                                    id,
                                                                    tb_agendas_comerciais_id,
                                                                    tb_colaborador_codigo_interno_colaborador_criador,
                                                                    codigo_colaborador_interno_externo,
                                                                    tb_status_app_id,
                                                                    tipo_codigo,
                                                                    data_solicitacao,
                                                                    data_resposta
                                                                ) VALUES (
                                                                    @GUID,
                                                                    @agendaId,
                                                                    @codigoColaboradorCriador,
                                                                    @codigoColaboradorSolicitante,
                                                                    0,
                                                                    @TipoCodigo,
                                                                    NOW(),
                                                                    NULL
                                                                );
                                                            ", new
                {
                    GUID = Guid.NewGuid().ToString(),
                    agendaId,
                    codigoColaboradorCriador,
                    codigoColaboradorSolicitante,
                    TipoCodigo = (int)tipoCodigo
                }
                );

                return linhasAfetadas > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<AgendaEncontroResult> BuscarAgendaPorId(int agendaId)
        {
            var conn = _connectionString.CreateMySqlConnection();

            try
            {
                await conn.OpenAsync();

                string sql = $@"
                -- 1. Dados da Agenda
                {QAgendaBase} WHERE id = @agendaId;

                -- 2. Cliente (org 2 conforme regra de negócio para e-mail/convite)
                SELECT codigo_cliente AS CodigoCliente, nome_cliente AS NomeCliente 
                FROM tb_cliente_org 
                WHERE tb_org_id = 2 AND codigo_cliente = (SELECT tb_cliente_org_codigo_cliente FROM tb_agendas_comerciais WHERE id = @agendaId);

                -- 3. Todos os Participantes (DTO Base)
                SELECT id, 
                       CASE WHEN tipo_codigo = 2 THEN codigo_colaborador_interno_externo END AS CodigoGestorExterno,
                       CASE WHEN tipo_codigo = 1 THEN codigo_colaborador_interno_externo END AS CodigoColaborador,
                       tb_status_app_id AS Confirmado, data_resposta AS DataConfirmacao, 
                       NULL AS Interessado, data_convite AS DataInteresse
                FROM tb_agenda_convidados 
                WHERE tb_agendas_comerciais_id = @agendaId;

                -- 4. Gestores Externos (Join)
                SELECT  g.cod_gestor_externo AS CodigoGestorExterno, g.nome AS Nome, g.email AS Email, 
                        p.tb_agendas_comerciais_id AS AgendaEncontroId
                FROM tb_agenda_convidados p
                INNER JOIN tb_gestor_externo g ON g.cod_gestor_externo = p.codigo_colaborador_interno_externo AND g.tb_org_id = 2
                WHERE p.tb_agendas_comerciais_id = @agendaId
                AND   p.tipo_codigo = 2;

                -- 5. Colaboradores (Join)
                SELECT DISTINCT p.codigo_colaborador_interno_externo AS CodInternoColaborador, 
                                tc.nome_completo AS Nome, tu.email AS Email
                FROM tb_agenda_convidados p
                INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = p.codigo_colaborador_interno_externo
                INNER JOIN tb_usuario tu ON tu.codigo_interno_colaborador = tc.codigo_interno_colaborador
                WHERE p.tb_agendas_comerciais_id = @agendaId
                AND   p.tipo_codigo = 1;

                -- 6. Participantes Externos
                SELECT DISTINCT nome AS Nome, email AS Email FROM tb_participantes_externo 
                        WHERE tb_agendas_comerciais_id = @agendaId;
                ";

                var multi = await conn.QueryMultipleAsync(sql, new { agendaId });

                var agenda = await multi.ReadFirstOrDefaultAsync<AgendaEncontroResult>();

                if (agenda == null) return null;

                agenda.Cliente = await multi.ReadFirstOrDefaultAsync<ClienteOrgDaGestaoAlocadosResult>();
                agenda.Participantes = (await multi.ReadAsync<ParticipanteEncontroDto>())
                    .Select(p => new ParticipanteConvidadoDto
                    {
                        Id = p.Id,
                        Nome = p.Nome,
                        CodigoGestorExterno = p.CodigoGestorExterno,
                        CodigoColaborador = p.CodigoColaborador,
                        Status = p.Confirmado == true ? 1 : 0,
                        DataResposta = p.DataConfirmacao,
                        DataConvite = null
                    })
                    .ToList();
                agenda.GestoresExternos = (await multi.ReadAsync<GestorExternoResult>()).ToList();
                agenda.Colaboradores = (await multi.ReadAsync<ColaboradorAgenda>()).ToList();
                agenda.ParticipantesExterno = (await multi.ReadAsync<ParticipanteExterno>()).ToList();
                agenda.Encontros = await buscarEncontrosPorAgendaId(agendaId.ToString());

                return agenda;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ConvidarGestorExternoAgenda(string codigoGestorExterno, int agendaId)
        {
            var conn = dapperConnection.GetConnection();

            try
            {
                var linhasAfetadas = await conn.ExecuteAsync(@"
                                                                INSERT INTO tb_agenda_convidados (
                                                                    tb_agendas_comerciais_id,
                                                                    codigo_colaborador_interno_externo,
                                                                    tipo_codigo,
                                                                    tb_status_app_id,
                                                                    data_convite,
                                                                    data_resposta
                                                                ) VALUES (
                                                                    @agendaId,
                                                                    @codigoGestorExterno,
                                                                    2,
                                                                    0,
                                                                    NOW(),
                                                                    NULL
                                                                );
                                                            ", new
                {
                    agendaId,
                    codigoGestorExterno
                }
                );

                return linhasAfetadas > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> ConvidarColaboradorInternoAgenda(string codigoColaboradorInterno, int agendaId)
        {
            var conn = dapperConnection.GetConnection();

            try
            {
                var linhasAfetadas = await conn.ExecuteAsync(@"
                                                                INSERT INTO tb_agenda_convidados (
                                                                    tb_agendas_comerciais_id,
                                                                    codigo_colaborador_interno_externo,
                                                                    tipo_codigo,
                                                                    tb_status_app_id,
                                                                    data_convite,
                                                                    data_resposta
                                                                ) VALUES (
                                                                    @agendaId,
                                                                    @codigoColaboradorInterno,
                                                                    1,
                                                                    0,
                                                                    NOW(),
                                                                    NULL
                                                                );
                                                            ", new
                {
                    agendaId,
                    codigoColaboradorInterno
                }
                );

                return linhasAfetadas > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<AgendaEncontroResult>> BuscarAgendasFilhosCompletas(string agendaId, string cpfUsuarioLogado)
        {
            var conn = dapperConnection.GetConnection();

            // SQL Consolidado para buscar todas as agendas filhas e seus detalhes de uma vez
            string sql = $@"
                        -- 1. Agendas Filhas
                        {QAgendaBase} WHERE tae.agenda_pai_id = @AgendaIdPai;

                        -- 2. Clientes (Join com as agendas filhas)
                        SELECT tco.codigo_cliente AS CodigoCliente, tco.nome_cliente AS NomeCliente
                        FROM tb_cliente_org tco
                        WHERE tco.codigo_cliente IN (SELECT codigo_cliente FROM tb_agendas_comerciais WHERE agenda_pai_id = @AgendaIdPai);

                        -- 3. Participantes/Vínculos
                        SELECT id, 
                               CASE WHEN tipo_codigo = 2 THEN codigo_colaborador_interno_externo END AS CodigoGestorExterno,
                               CASE WHEN tipo_codigo = 1 THEN codigo_colaborador_interno_externo END AS CodigoColaborador,
                               tb_status_app_id AS Confirmado, data_resposta AS DataConfirmacao, 
                               NULL AS Interessado, data_convite AS DataInteresse,
                               tb_agendas_comerciais_id AS AgendaId
                        FROM tb_agenda_convidados 
                        WHERE tb_agendas_comerciais_id IN (SELECT id FROM tb_agendas_comerciais WHERE agenda_pai_id = @AgendaIdPai);

                        -- 4. Gestores Externos Detalhes
                        SELECT  g.cod_gestor_externo AS CodGestorExterno, g.nome AS Nome, g.email AS Email, 
                                p.tb_agendas_comerciais_id AS AgendaId
                        FROM tb_agenda_convidados p
                        INNER JOIN tb_gestor_externo g ON g.cod_gestor_externo = p.codigo_colaborador_interno_externo
                        WHERE p.tb_agendas_comerciais_id IN (SELECT id FROM tb_agendas_comerciais WHERE agenda_pai_id = @AgendaIdPai)
                          AND p.tb_status_app_id <> 2
                          AND p.tipo_codigo = 2;

                        -- 5. Colaboradores Detalhes
                        SELECT DISTINCT p.codigo_colaborador_interno_externo AS CodInternoColaborador, 
                                        tc.nome_completo AS Nome, 
                                        tu.email AS Email, p.tb_agendas_comerciais_id AS AgendaId
                        FROM tb_agenda_convidados p
                        INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = p.codigo_colaborador_interno_externo
                        LEFT JOIN tb_usuario tu ON tu.codigo_interno_colaborador = p.codigo_colaborador_interno_externo
                        WHERE p.tb_agendas_comerciais_id IN (SELECT id FROM tb_agendas_comerciais WHERE agenda_pai_id = @AgendaIdPai)
                          AND p.tb_status_app_id <> 2
                          AND p.tipo_codigo = 1;

                        -- 6. Participantes Externos
                        SELECT DISTINCT nome AS Nome, email AS Email, tb_agendas_comerciais_id AS AgendaId 
                        FROM tb_participantes_externo 
                        WHERE tb_agendas_comerciais_id IN (SELECT id FROM tb_agendas_comerciais WHERE agenda_pai_id = @AgendaIdPai);

                        -- 7. Status Solicitação 
                        SELECT tasp.tb_status_app_id AS status
                        FROM  tb_agenda_solicitante tasp
                        WHERE tasp.codigo_colaborador_interno_externo = @Cpf
                          AND tasp.tb_agendas_comerciais_id IN (SELECT id FROM tb_agendas_comerciais WHERE agenda_pai_id = @AgendaIdPai);

                        -- 8. Participação Usuário Logado
                        SELECT tb_status_app_id AS Status, data_resposta AS DataResposta, tb_agendas_comerciais_id AS AgendaId
                        FROM tb_agenda_convidados
                        WHERE codigo_colaborador_interno_externo = @Cpf
                          AND tb_agendas_comerciais_id IN (SELECT id FROM tb_agendas_comerciais WHERE agenda_pai_id = @AgendaIdPai)
                        LIMIT 1;

                        -- 9. Encontros
                        SELECT 
                            e.id,
                            e.data_requisicao AS DataRequisicao,
                            e.titulo_interacao AS TituloInteracao,
                            e.descricao_interacao AS DescricaoInteracao,
                            e.tb_agendas_comerciais_id AS AgendaId
                        FROM tb_interacoes e
                        WHERE e.tb_agendas_comerciais_id IN (
                            SELECT id FROM tb_agendas_comerciais WHERE agenda_pai_id = @AgendaIdPai
                        );
                    ";

            var multi = await conn.QueryMultipleAsync(sql,
                new { AgendaIdPai = agendaId, Cpf = cpfUsuarioLogado });

            // 1) Leitura dos dados
            var agendas = (await multi.ReadAsync<AgendaEncontroResult>()).ToList();

            if (!agendas.Any())
                return agendas;

            // 2. Clientes
            var clientesLookup = (await multi.ReadAsync<ClienteOrgDaGestaoAlocadosResult>())
                .DistinctBy(x => x.CodigoCliente)
                .ToDictionary(x => x.CodigoCliente);

            // 3. Participantes
            var participantesLookup = (await multi.ReadAsync<dynamic>())
                .ToLookup(x => (int)x.AgendaId);

            // 4. Gestores
            var gestoresLookup = (await multi.ReadAsync<dynamic>())
                .ToLookup(x => (int)x.AgendaId);

            // 5. Colaboradores
            var colaboradoresLookup = (await multi.ReadAsync<dynamic>())
                .ToLookup(x => (int)x.AgendaId);

            // 6. Externos
            var externosLookup = (await multi.ReadAsync<dynamic>())
                .ToLookup(x => (int)x.AgendaId);

            // 7. Status solicitação
            var statusLookup = (await multi.ReadAsync<StatusSolicitacaoParticipanteRel>())
                .ToLookup(x => x.AgendaId);

            // 8. Participação usuário logado
            var participacaoLookup = (await multi.ReadAsync<ParticipacaoUsuarioConvidado>())
                .ToLookup(x => x.AgendaId);

            // 9. Encontros
            var encontrosLookup = (await multi.ReadAsync<EncontrosDTO>())
                .ToLookup(x => x.AgendaId);

            foreach (var a in agendas)
            {
                // Cliente
                if (!string.IsNullOrEmpty(a.CodigoCliente) &&
                    clientesLookup.TryGetValue(a.CodigoCliente, out var cliente))
                {
                    a.Cliente = cliente;
                }

                // Encontros
                a.Encontros = encontrosLookup[a.Id]
                       .Select(e => new EncontrosResponse
                       {
                           Id = e.Id,
                           DataRequisicao = e.DataRequisicao
                       })
                        .ToList();


                // Participantes
                a.Participantes = participantesLookup[a.Id]
                    .Select(x => new ParticipanteConvidadoDto
                    {
                        Id = x.id,
                        Nome = x.Nome,
                        CodigoGestorExterno = x.CodigoGestorExterno,
                        CodigoColaborador = x.CodigoColaborador,
                        Status = x.confirmado == true ? 1 : 0,
                        DataResposta = x.data_confirmacao,
                        DataConvite = null
                    }).ToList();

                // Listas tipadas
                a.GestoresExternos = gestoresLookup[a.Id]
                    .Select(x => new GestorExternoResult
                    {
                        CodGestorExterno = x.CodGestorExterno,
                        Nome = x.Nome,
                        Email = x.Email
                    }).ToList();

                a.Colaboradores = colaboradoresLookup[a.Id]
                    .Select(x => new ColaboradorAgenda
                    {
                        CodInternoColaborador = x.CodInternoColaborador,
                        Nome = x.Nome,
                        Email = x.Email
                    }).ToList();

                a.ParticipantesExterno = externosLookup[a.Id]
                    .Select(x => new ParticipanteExterno
                    {
                        Nome = x.Nome,
                        Email = x.Email
                    }).ToList();

                // Status / Participação
                a.StatusSolicitacaoParticipante =
                        (StatusSolicitacaoParticipante)(statusLookup[a.Id].FirstOrDefault()?.Status ?? 0);

                a.ParticipacaoUsuarioLogado =
                    participacaoLookup[a.Id].FirstOrDefault();
            }

            return agendas;
        }

        public async Task<List<int>> BuscarIdAgendasFilhos(string agendaId)
        {
            var conn = dapperConnection.GetConnection();
            var idAgendas = (await conn.QueryAsync<int>($@"SELECT id 
                                                           FROM tb_agendas_comerciais
                                                           WHERE agenda_pai_id  = @agendaId;", new { agendaId })).ToList();

            return idAgendas;
        }

        public async Task<List<AgendaObjetivoDTO>> ListarTodosObjetivos()
        {
            var conn = dapperConnection.GetConnection();

            var objetivos = await conn.QueryAsync<AgendaObjetivoDTO>(
                @"SELECT id AS Id,
                         titulo AS Titulo,
                         descricao AS Descricao,
                         data_criacao AS DataCriacao,
                         ativo AS Ativo
                  FROM tb_agenda_objetivo
                  ORDER BY titulo;");

            return objetivos.AsList();
        }

        // Comprimento do código < 36 → gestor externo (2); senão → colaborador interno (1).
        private static TipoCodigoParticipanteAgenda InferirTipoCodigoPorComprimentoDoCodigo(string codigo)
        {
            return codigo.Length > 35
                ? TipoCodigoParticipanteAgenda.ColaboradorInterno
                : TipoCodigoParticipanteAgenda.GestorExterno;
        }
        
    }
}