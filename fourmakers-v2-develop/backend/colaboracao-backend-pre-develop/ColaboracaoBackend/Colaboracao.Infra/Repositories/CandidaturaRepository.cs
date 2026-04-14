using Colaboracao.Core;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Infra.Context;
using Core.Domain.Social;
using Core.DomainModel;
using Dapper;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Candidato;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.Vaga.Enums;
using MySql.Data.MySqlClient;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories
{
    internal class QuantidadeCandidatosPorEstagioResult
    {
        public string VagaId { get; set; }
        public int IdStatus { get; set; }
        public string DescricaoStatus { get; set; }
        public int Quantidade { get; set; }
    }

    public class CandidaturaRepository : ICandidaturaRepository
    {
        private readonly IDBConnection _dapperConnection;
        private readonly IComentarioCandidaturaRepository _comentarioRepository;
        private readonly ILogCore _log;

        public CandidaturaRepository(IDBConnection dapperConnection, IComentarioCandidaturaRepository comentarioRepository, ILogCore log)
        {
            _dapperConnection = dapperConnection;
            _comentarioRepository = comentarioRepository;
            _log = log;
        }

        /// <summary>Normaliza string de códigos separados por vírgula para FIND_IN_SET; null/empty = sem filtro.</summary>
        private static string? NormalizarListaParaFindInSet(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;
            var list = valor.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
            return list.Count == 0 ? null : string.Join(",", list);
        }

        /// <summary>Converte CodigoCliente em lista (para uso em lógica que precise de lista).</summary>
        private static List<string>? ParseCodigosCliente(string? codigoCliente)
        {
            if (string.IsNullOrWhiteSpace(codigoCliente)) return null;
            var list = codigoCliente.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
            return list.Count == 0 ? null : list;
        }

        private async Task<CandidaturaArquivosDTO> BuscarArquivoPorId(string arquivoId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
        SELECT
            id                AS Id,
            tb_candidato_vaga_id AS IdCandidatura,
            tb_comentario_id  AS IdComentario,
            tb_colaborador_codigo_interno_colaborador_criador AS CodColaboradorCriador,
            data_archived     AS DataArquivo,
            link_arquivo      AS LinkArquivo
        FROM tb_candidato_vaga_documentos
        WHERE id = @ArquivoId;";
            return await connection.QueryFirstOrDefaultAsync<CandidaturaArquivosDTO>(query, new { ArquivoId = arquivoId });
        }


        public async Task<CandidaturaArquivosDTO> InserirArquivo(CandidaturaArquivosParams param, string url, string codigoInternoColaboradorLogado)
        {
            var IdArquivo = Guid.NewGuid();
            var connection = _dapperConnection.GetConnection();

            var query = @"
            INSERT INTO 
                tb_candidato_vaga_documentos (id, tb_candidato_vaga_id, tb_comentario_id, tb_colaborador_codigo_interno_colaborador_criador, data_archived, link_arquivo)
            VALUES
                (@Id, @TbCandidatoVagaId, @TbComentarioId, @TbColaboradorCodigoInternoCriador, @DataArquivado, @LinkArquivo);
            ";

            var parametros = new
            {
                Id = IdArquivo,
                TbCandidatoVagaId = param.IdCandidatura,
                TbComentarioId = param.IdComentario,
                TbColaboradorCodigoInternoCriador = codigoInternoColaboradorLogado,
                DataArquivado = DateTime.UtcNow,
                LinkArquivo = url
            };

            var insert = await connection.ExecuteAsync(query, parametros);

            if (insert > 0)
            {
                var result = await BuscarArquivoPorId(IdArquivo.ToString());

                return result;
            }

            return null;

        }

        public async Task<bool> DeletarArquivo(string arquivoPath)
        {
            var conn = _dapperConnection.GetConnection();
            var tx = await conn.BeginTransactionAsync();

            const string qDel = """
                                DELETE FROM tb_candidato_vaga_documentos
                                WHERE link_arquivo = @Path;
                                """;
            if (await conn.ExecuteAsync(qDel, new { Path = arquivoPath }, tx) == 0)
            {
                await tx.RollbackAsync();
                return true;
            }

            await tx.CommitAsync();
            return true;
        }

        public async Task AlterarStatusCandidatura(int vagaId, int candidatoId, int statusId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                UPDATE tb_vaga_candidato tvc
                SET tvc.status_id = @statusId
                WHERE
                    tvc.vaga_id = @vagaId
                    AND tvc.candidato_id = @candidatoId;
            ";

            var parametros = new
            {
                vagaId,
                candidatoId,
                statusId
            };

            await connection.QueryAsync(query, parametros);
            await InserirLogAlteracaoStatusCandidatura(vagaId, candidatoId, statusId);
        }

        public async Task<int?> ExisteStatusCandidatura(int vagaId, int candidatoId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT
                    tvc.vaga_id
                FROM
                    tb_vaga_candidato tvc
                WHERE
                    tvc.vaga_id = @vagaId
                    AND tvc.candidato_id = @candidatoId;
            ";

            var parametros = new
            {
                vagaId,
                candidatoId,
            };

            var result = await connection.QueryFirstOrDefaultAsync<int>(query, parametros);

            return result;
        }

        public async Task InserirLogAlteracaoStatusCandidatura(int vagaId, int candidatoId, int statusId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                INSERT INTO tb_vaga_candidato_status_log (vaga_id, candidato_id, status_id)
                VALUES (@vagaId, @candidatoId, @statusId)
            ";

            var parametros = new
            {
                vagaId,
                candidatoId,
                statusId
            };

            await connection.ExecuteAsync(query, parametros);
        }

        public async Task<List<CandidaturaStatusDTO>> ListarStatusCandidatura()
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT
                    tcs.id AS Id,
                    tcs.descricao AS Descricao,
                    tcs.origem AS Origem
                FROM
                    tb_candidato_status tcs;
            ";

            var result = await connection.QueryAsync<CandidaturaStatusDTO>(query);
            return result.ToList();
        }

        public async Task<CandidaturaStatusDTO> BuscarStatusCandidaturaPorId(int id)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT
                    tcs.id AS Id,
                    tcs.descricao AS Descricao,
                    tcs.origem AS Origem
                FROM
                    tb_candidato_status tcs
                WHERE
                    tcs.id = @statudId;
            ";

            var parametros = new
            {
                statudId = id
            };

            var result = await connection.QueryFirstOrDefaultAsync<CandidaturaStatusDTO>(query, parametros);
            return result;
        }

        public async Task<CandidaturaStatusDTO> InserirTipoStatusCandidatura(int statusId, string descricao, string origem)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                INSERT INTO tb_candidato_status (id, descricao, origem)
                VALUES (@statusId, @descricao, @origem);

                SELECT LAST_INSERT_ID();
            ";

            var parametros = new
            {
                origem,
                descricao,
                statusId
            };

            var id = await connection.ExecuteScalarAsync<int>(query, parametros);
            var result = await BuscarStatusCandidaturaPorId(id);
            return result;
        }

        public async Task<List<VagaCandidatoGestaoDTO>> ListarCandidatosDeVagas(string pesquisa, string candidato, List<int> status, string titulo, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT
                    tvc.vaga_id AS VagaId,
                    tvs.titulo AS Titulo,
                    tcs.descricao AS StatusCandidato,
                    tc.nome_completo AS Candidato,
                    tc.codigo_interno_colaborador AS CodigoInternoColaborador,
                    tvs.status_vaga AS StatusVaga
                FROM
                    tb_vaga_candidato tvc
                INNER JOIN
                    tb_vagas_srs tvs ON tvs.id_vaga = tvc.vaga_id
                INNER JOIN
                    tb_colaborador tc ON tc.codigo_interno_colaborador = tvc.codigo_interno_colaborador
                INNER JOIN
                    tb_candidato_status tcs ON tcs.id = tvc.status_id
                WHERE
                    tvs.status_vaga <> 'EM APROVAÇÃO'
                    AND tvc.tb_org_id = @orgId
                    AND (
                        (@pesquisa = '' OR tvs.titulo LIKE CONCAT('%', @pesquisa, '%') OR tc.nome_completo LIKE CONCAT('%', @pesquisa, '%'))
                    )
                    AND (@titulo = '' OR tvs.titulo LIKE CONCAT('%', @titulo, '%'))
                    AND (@candidato = '' OR tc.nome_completo LIKE CONCAT('%', @candidato, '%'))
                    AND (
                        (@status IS NULL OR @status = '' OR FIND_IN_SET(tvc.status_id, @status) > 0)
                    );
            ";

            var parametros = new
            {
                pesquisa,
                candidato,
                status = status != null && status.Count > 0 ? string.Join(",", status) : "",
                titulo,
                orgId
            };

            var result = await connection.QueryAsync<VagaCandidatoGestaoDTO>(query, parametros);

            return result.ToList();
        }

        public async Task CandidatarSeRecrutamento(string codigoInternoColaborador, string vagaId, int orgId, int status, List<string> opcoesContatoCodigos, decimal? pretencaoSalarial = null, string modeloTrabalhoId = null, string disponibilidadeEntrevistaId = null, int? quantidadeDiasPresencial = null)
        {
            var connection = _dapperConnection.GetConnection();
            var candidatura = await CadastrarCandidatura(codigoInternoColaborador, vagaId, orgId, status, pretencaoSalarial, modeloTrabalhoId, disponibilidadeEntrevistaId, quantidadeDiasPresencial, connection);

            foreach (var opcaoContato in opcoesContatoCodigos)
            {
                var queryOpcaoContato = @"INSERT INTO tb_candidato_vaga_opcao_contato (tb_candidato_vaga_id, tb_opcao_contato_id) VALUES (@idCandidatarSe, @opcaoContato);";

                var parametrosOpcaContato = new
                {
                    idCandidatarSe = candidatura.Id,
                    opcaoContato
                };

                await connection.ExecuteAsync(queryOpcaoContato, parametrosOpcaContato);
            }

            await CadastrarLogCandidatura(codigoInternoColaborador, status, connection, candidatura);
            await InserirLogPretensaoModeloSeNecessarioAsync(connection, null, candidatura, codigoInternoColaborador, "INSERT", OrigemAlteracaoPretensaoModeloLogEnum.Candidato);
        }

        public async Task<IEnumerable<ListarCandidatosInscritosResult>> ListarCandidatosInscritos(string vagaId, string busca, string dataInicio, string dataFim, int cursor, int limite, string codigoInternoColaboradorLogado, bool? qualificados, int? diasUltimaAlteracao, string? localizacaoCidade, string? localizacaoEstado)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);
            await connection.OpenAsync();

            try
            {
                var query = @"
                    SELECT 
                        tcv.id as IdCandidatura,
                        tb_colaborador_codigo_interno_colaborador as Codigo,
                        tcv.data_criacao as Candidatura,
                        tcv.data_ultima_alteracao AS UltimaAlteracao,
                        tc.nome_completo as Nome,
                        tu.email as EmailUsuario,
                        tc.email_alternativo as EmailAlternativo,
                        tcs.id as IdStatusCandidatura,
                        tcs.descricao as DescricaoStatusCandidatura,
                        COALESCE(tor_banco.id, tor_colaborg.id) as OrgId,
                        COALESCE(tor_banco.descricao, tor_colaborg.descricao) as OrgDescricao,
                        tco.ativo as AtivoNaOrg,
                        banco.codigo_interno_colaborador AS CodigoInternoColaboradorDeQuemCadastrou,
                        banco.tipo_cadastro AS TipoCadastroBancoDeTalentos,
                        colabCadastro.nome_completo AS NomeCompletoDeQuemCadastrou,
                        tcv.pretensao_salarial as PretencaoSalarial,
                        tcv.tb_modelo_trabalho_id as ModeloTrabalhoId,
                        tmt.descricao as ModeloTrabalhoDescricao,
                        tcv.tb_disponibilidade_entrevista_id as DisponibilidadeEntrevistaId,
                        tde.descricao as DisponibilidadeEntrevistaDescricao,
                        tcv.quantidade_dias_presencial as QuantidadeDiasPresencial,
                        tc.qualificado as Qualificado,
                        tcql.data_criacao as DataQualificacao,
                        qualificador.nome_completo as NomeDeQuemQualificou,
                        CASE 
                            WHEN tcv.tb_colaborador_codigo_interno_colaborador = (
                                SELECT tcvl.tb_colaborador_codigo_interno_colaborador 
                                FROM tb_candidato_vaga_log tcvl 
                                WHERE tcvl.tb_candidato_vaga_id = tcv.id 
                                ORDER BY tcvl.data_alteracao ASC 
                                LIMIT 1
                            )
                            THEN TRUE 
                            ELSE FALSE 
                        END AS Candidatouse,
                        CASE 
                            WHEN banco.tipo_cadastro = 'SRS_LINKEDIN' 
                            THEN TRUE 
                            ELSE FALSE 
                        END AS OrigemSrsLinkedin,
                        tco.cod_diretoria as CodDiretoria,
                        tcv.tb_colaborador_codigo_interno_colaborador_responsavel AS CodRecrutadorResponsavel,
                        tcResponsavel.nome_completo AS RecrutadorResponsavel,
                        CASE
                            WHEN tcv.tb_colaborador_codigo_interno_colaborador_responsavel IS NOT NULL
                                AND tcv.tb_colaborador_codigo_interno_colaborador_responsavel = @codigoInternoColaboradorLogado
                            THEN TRUE
                            ELSE FALSE
                        END AS UsuarioLogadoRecrutadorResponsavel
                    FROM 
	                    tb_candidato_vaga tcv
                    INNER JOIN 
                        tb_colaborador tc ON tcv.tb_colaborador_codigo_interno_colaborador = tc.codigo_interno_colaborador
                    LEFT JOIN 
                        tb_usuario tu ON tcv.tb_colaborador_codigo_interno_colaborador = tu.codigo_interno_colaborador AND tcv.tb_org_id = tu.tb_org_id
                    INNER JOIN 
                        tb_candidato_status tcs ON tcv.tb_candidato_status_id = tcs.id and tcs.origem = 'Fourmakers'
                    LEFT JOIN 
                        tb_colaborador_org tco ON tcv.tb_colaborador_codigo_interno_colaborador = tco.codigo_interno_colaborador AND tco.tb_org_id = tcv.tb_org_id 
                    LEFT JOIN 
                        tb_banco_talentos banco ON banco.codigo_interno_colaborador = tcv.tb_colaborador_codigo_interno_colaborador
                    LEFT JOIN 
                        tb_org tor_banco ON banco.tb_org_id = tor_banco.id
                    LEFT JOIN 
                        tb_org tor_colaborg ON tco.tb_org_id = tor_colaborg.id
                    LEFT JOIN 
                        tb_colaborador colabCadastro ON colabCadastro.codigo_interno_colaborador = banco.tb_colaborador_codigo_interno_colaborador_cadastrante
                    LEFT JOIN 
                        tb_modelo_trabalho tmt ON tcv.tb_modelo_trabalho_id = tmt.id
                    LEFT JOIN 
                        tb_disponibilidade_entrevista tde ON tcv.tb_disponibilidade_entrevista_id = tde.id
                    LEFT JOIN 
                        (SELECT tcql1.tb_colaborador_codigo_interno_colaborador_qualificado, tcql1.data_criacao, tcql1.tb_colaborador_codigo_interno_colaborador_qualificador
                         FROM tb_colaborador_qualificacao_log tcql1
                         INNER JOIN (SELECT tb_colaborador_codigo_interno_colaborador_qualificado, MAX(data_criacao) as max_data
                                     FROM tb_colaborador_qualificacao_log
                                     WHERE qualificado = 1
                                     GROUP BY tb_colaborador_codigo_interno_colaborador_qualificado) ult_qualificacao
                         ON tcql1.tb_colaborador_codigo_interno_colaborador_qualificado = ult_qualificacao.tb_colaborador_codigo_interno_colaborador_qualificado
                         AND tcql1.data_criacao = ult_qualificacao.max_data
                         WHERE tcql1.qualificado = 1
                        ) tcql ON tcql.tb_colaborador_codigo_interno_colaborador_qualificado = tcv.tb_colaborador_codigo_interno_colaborador
                    LEFT JOIN 
                        tb_colaborador qualificador ON qualificador.codigo_interno_colaborador = tcql.tb_colaborador_codigo_interno_colaborador_qualificador
                    LEFT JOIN 
	                    tb_endereco te ON te.id = tc.endereco_id
                    LEFT JOIN tb_colaborador tcResponsavel
                        ON tcResponsavel.codigo_interno_colaborador = tcv.tb_colaborador_codigo_interno_colaborador_responsavel
                    WHERE 
                        tb_vaga_id = @vagaId
                        AND (@Busca IS NULL OR @Busca = '' OR 
                            LOWER(tc.nome_completo) COLLATE utf8_general_ci LIKE CONCAT('%', LOWER(@Busca), '%') OR
                            LOWER(tcs.descricao) LIKE CONCAT('%', LOWER(@Busca), '%'))
	                    AND tcv.data_criacao between @DataInicio AND @DataFim
                        AND (@Qualificados IS NULL OR @Qualificados = 0 OR tc.qualificado = @Qualificados)
                        AND (
                                @diasUltimaAlteracao IS NULL 
                                OR @diasUltimaAlteracao = 0 
                                OR (
                                    tcv.data_ultima_alteracao >= DATE_SUB(CURDATE(), INTERVAL @diasUltimaAlteracao DAY)
                                    AND tcv.data_ultima_alteracao < DATE_ADD(CURDATE(), INTERVAL 1 DAY)
                                )
                            )
                        AND (@localizacaoCidade IS NULL OR @localizacaoCidade = '' OR te.cidade = @localizacaoCidade)
  	                    AND (@localizacaoEstado IS NULL OR @localizacaoEstado = '' OR te.estado = @localizacaoEstado)
                        AND tcv.ativo = 1
                    ORDER BY 
	                    tcv.data_ultima_alteracao DESC
                    LIMIT 
	                    @Limite
                    OFFSET
                        @Cursor;";

                var parameters = new { vagaId, busca, dataInicio, dataFim, limite, cursor, qualificados, diasUltimaAlteracao, localizacaoCidade, localizacaoEstado, codigoInternoColaboradorLogado };

                var candidaturas = await connection.QueryAsync<ListarCandidatosInscritosResult>(query, parameters);

                var semaphore = new SemaphoreSlim(5, 5);
                var tasks = new List<Task>();

                foreach (var candidatura in candidaturas)
                {
                    tasks.Add(ProcessarCandidaturaAsync(candidatura, semaphore, codigoInternoColaboradorLogado));
                }

                await Task.WhenAll(tasks);

                return candidaturas;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao buscar candidatos inscritos - CandidaturaRepositoy - {ex.Message}", DataTransferObject.Domain.Log.LevelsEnum.Error);
                return new List<ListarCandidatosInscritosResult>();
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        private async Task ProcessarCandidaturaAsync(ListarCandidatosInscritosResult candidatura, SemaphoreSlim semaphore, string codigoInternoColaboradorLogado)
        {
            await semaphore.WaitAsync();
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();
                var querydataUltimaModificacaoStatus = @"SELECT 
	                                                        data_alteracao 
                                                        FROM 
	                                                        tb_candidato_vaga_log 
                                                        WHERE 
	                                                        tb_candidato_vaga_id = @candidaturaId 
	                                                        AND tb_candidato_status_id = @candidaturaStatus 
                                                        ORDER BY 
	                                                        data_alteracao ASC 
                                                        LIMIT 1";
                var dataUltimaModificacaoStatus = (await connection.QueryAsync<DateTime>(querydataUltimaModificacaoStatus, new { candidaturaId = candidatura.IdCandidatura, candidaturaStatus = candidatura.IdStatusCandidatura })).FirstOrDefault();
                candidatura.SlaDecorridoDaEtapaAtual = DateTime.Now.Subtract(dataUltimaModificacaoStatus);

                candidatura.SlaDecorridoTotal = DateTime.Now.Subtract(candidatura.Candidatura);

                // Verificar se o usuário logado foi quem moveu a candidatura para o status 11 (Contratado)
                var queryExibirRemuneracao = @"
                    SELECT 
                        tb_colaborador_codigo_interno_colaborador 
                    FROM 
                        tb_candidato_vaga_log 
                    WHERE 
                        tb_candidato_vaga_id = @candidaturaId 
                        AND tb_candidato_status_id = 11
                    ORDER BY 
                        data_alteracao DESC 
                    LIMIT 1";

                var colaboradorQueContratou = await connection.QueryFirstOrDefaultAsync<string>(queryExibirRemuneracao, new { candidaturaId = candidatura.IdCandidatura });
                candidatura.ExibirRemuneracao = colaboradorQueContratou == codigoInternoColaboradorLogado;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao buscar data da alteracao do status da candidatura - CandidaturaRepositoy - {ex.Message}", DataTransferObject.Domain.Log.LevelsEnum.Error);
            }
            finally
            {
                await connection.CloseAsync();
                semaphore.Release();
            }
        }

        public async Task<IEnumerable<ListarCandidaturasPorCodCandidatoResult>> ListarCandidaturasPorCodCandidato(string codColaborador)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);
            await connection.OpenAsync();

            try
            {
                var query = @"
                        SELECT 
                             tcv.id AS CandidaturaId,
                             codigo AS Codigo,
                             titulo AS Titulo,    
                             tv.data_criacao AS DataCriacao,        
                             tcv.data_ultima_alteracao AS DataUltimaAlteracao,
                             tcv.tb_candidato_status_id AS StatusCandidaturaId,        
                             tcs.descricao AS StatusCandidaturaDescricao,        
                             tcv.pretensao_salarial AS PretensaoSalarial,        
                             tcv.tb_modelo_trabalho_id AS ModeloTrabalhoId,
                             tcv.tb_disponibilidade_entrevista_id AS DisponibilidadeEntrevistaId,
                             tcv.quantidade_dias_presencial AS QuantidadeDiasPresencial,
                             tco.nome_cliente AS NomeCliente,
	                         tco.codigo_cliente AS CodigoCliente,
	                         tge.cod_gestor_externo AS CodigoGestor,
	                         tge.nome AS NomeGestor,
	                         tv.id AS IdVaga,
	                         tv.tb_status_vaga_cod AS StatusVaga,
	                         tcv.data_criacao AS DataCandidatura
                         FROM 
                             tb_vaga tv
                         INNER JOIN 
                             tb_candidato_vaga tcv ON tcv.tb_vaga_id  = tv.id
                         INNER JOIN 
                            tb_candidato_status tcs ON tcs.id  = tcv.tb_candidato_status_id AND tcs.origem = 'Fourmakers'
                        LEFT JOIN 
                            tb_gestor_externo tge ON tv.tb_gestor_cod  = tge.cod_gestor_externo
                        LEFT JOIN 
                            tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente AND tco.tb_org_id = tv.tb_org_id
                         WHERE 
                             tcv.tb_colaborador_codigo_interno_colaborador  = @codColaborador
                         ORDER BY 
                             tv.data_criacao DESC;";

                var parameters = new { codColaborador };

                var retorno = await connection.QueryAsync<ListarCandidaturasPorCodCandidatoResult>(query, parameters);

                await connection.CloseAsync();

                return retorno;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<bool> EstaCandidaturaExiste(string codigoColaborador, string idVaga)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                        SELECT EXISTS (
                                       SELECT 
					                         1
					                     FROM 
					                         tb_candidato_vaga tcv
					                     WHERE 
					                         tcv.tb_colaborador_codigo_interno_colaborador  = @codigoColaborador
                                             AND tcv.tb_vaga_id  = @idVaga
                                   );";

            var parameters = new { codigoColaborador, idVaga };

            return await connection.ExecuteScalarAsync<bool>(query, parameters);
        }

        public async Task<bool> EstaCandidaturaExiste(string idCandidatura)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                        SELECT EXISTS (
                                       SELECT 
					                         1
					                     FROM 
					                         tb_candidato_vaga tcv
					                     WHERE 
					                         tcv.id = @idCandidatura
                                   );";

            var parameters = new { idCandidatura };

            return await connection.ExecuteScalarAsync<bool>(query, parameters);
        }

        public async Task<CandidaturaRecrutamentoDTO> ObterCandidaturaPorId(string idCandidatura)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                    SELECT 
                         tcv.id AS Id,
	                     tcv.tb_colaborador_codigo_interno_colaborador AS CodColaborador,
	                     tcv.tb_vaga_id AS IdVaga,
	                     tcv.tb_org_id AS OrgId,
	                     tcv.ativo AS Ativa,
                         tv.titulo AS TituloVaga,    
                         tcv.data_criacao AS Candidatura,        
                         tcv.data_ultima_alteracao AS UltimaAlteracao,
                         tcv.tb_candidato_status_id AS StatusId,        
                         tcs.descricao AS StatusDescricao,
                         tcv.pretensao_salarial AS PretencaoSalarial,
                         tcv.tb_modelo_trabalho_id AS ModeloTrabalhoId,
                         tcv.tb_disponibilidade_entrevista_id AS DisponibilidadeEntrevistaId,
                         tcv.quantidade_dias_presencial AS QuantidadeDiasPresencial,
                         tcv.tb_colaborador_codigo_interno_colaborador_responsavel AS ColaboradorResponsavel
                     FROM 
                         tb_vaga tv
                     INNER JOIN 
                         tb_candidato_vaga tcv ON tcv.tb_vaga_id  = tv.id
                     INNER JOIN 
                        tb_candidato_status tcs ON tcs.id  = tcv.tb_candidato_status_id AND tcs.origem = 'Fourmakers'
                     WHERE 
                        tcv.id  = @idCandidatura
                     ORDER BY 
                         tv.data_criacao DESC;";

            var parameters = new { idCandidatura };

            return await connection.QueryFirstOrDefaultAsync<CandidaturaRecrutamentoDTO>(query, parameters);
        }

        public async Task<IEnumerable<int>> ListarIdsStatusCandidaturaRecrutamento()
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                    SELECT 
                         id AS Id  
                     FROM 
                         tb_candidato_status
                     WHERE 
                         origem = 'Fourmakers';";

            return await connection.QueryAsync<int>(query);
        }

        public async Task<string> MudarStatusCandidatura(string idCandidatura, int codigoStatus, string codColaborador, string comentario = null)
        {
            var connection = _dapperConnection.GetConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    if (codigoStatus == StatusCandidaturaRecrutamento.EntrevistaInicial.ToInt())
                    {
                        var queryResponsavel = @"
                                    UPDATE tb_candidato_vaga 
                                    SET
                                        tb_colaborador_codigo_interno_colaborador_responsavel = @codColaborador
                                    WHERE 
                                        id = @idCandidatura;
                                ";

                        await connection.ExecuteAsync(
                            queryResponsavel,
                            new
                            {
                                idCandidatura,
                                codColaborador
                            }
                        );
                    }

                    var query = @"
                            UPDATE tb_candidato_vaga SET
                                tb_candidato_status_id = @codigoStatus
                            WHERE 
                                id = @idCandidatura;

                            SELECT 
                                tcv.id AS Id,
	                            tcv.tb_colaborador_codigo_interno_colaborador AS CodColaborador,
	                            tcv.tb_vaga_id AS IdVaga,
	                            tcv.tb_org_id AS OrgId,
	                            tcv.ativo AS Ativa,   
                                tcv.data_criacao AS Candidatura,        
                                tcv.data_ultima_alteracao AS UltimaAlteracao,
                                tcv.tb_candidato_status_id AS StatusId,
                                tb_colaborador_codigo_interno_colaborador_responsavel AS ColaboradorResponsavel
                            FROM 
                                tb_candidato_vaga tcv
                            WHERE
                                id = @idCandidatura;";

                    var candidatura = await connection.QueryFirstOrDefaultAsync<CandidaturaRecrutamentoDTO>(query, new { idCandidatura, codigoStatus });

                    var idLog = Guid.NewGuid().ToString();
                    var candidaturaId = candidatura.Id;
                    string comentarioId = null;

                    // Se houver comentário, criar usando o repositório de comentários
                    if (!string.IsNullOrEmpty(comentario))
                    {
                        var comentarioObjeto = new ComentarioCandidaturaDTO
                        {
                            Id = Guid.NewGuid().ToString(),
                            Texto = comentario,
                            CodigoInternoColaborador = codColaborador,
                            CandidaturaId = candidaturaId,
                            DataCriacao = DateTime.UtcNow
                        };

                        await _comentarioRepository.AddAsync(comentarioObjeto, transacaoAberta: true);
                        comentarioId = comentarioObjeto.Id;
                    }

                    // Criar o log com referência ao comentário
                    var queryLog = @"
                        INSERT INTO tb_candidato_vaga_log (
                            id,
                            tb_candidato_vaga_id,
                            tb_candidato_status_id,
                            tb_colaborador_codigo_interno_colaborador,
                            data_alteracao,
                            objeto,
                            tb_comentario_id
                        ) VALUES (
                            @idLog,
                            @candidaturaId,
                            @codigoStatus,
                            @codColaborador,
                            NOW(),
                            @objeto,
                            @comentarioId
                        )";

                    var parametrosLog = new
                    {
                        idLog,
                        candidaturaId,
                        codigoStatus,
                        codColaborador,
                        objeto = JsonSerializer.Serialize(candidatura),
                        comentarioId
                    };

                    await connection.ExecuteAsync(queryLog, parametrosLog, transaction);

                    transaction.Commit();
                    return comentarioId;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<IEnumerable<StatusCandidaturaRecrutamentoDTO>> ListarStatusCandidaturaRecrutamento()
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"SELECT 
                            id as Id, 
                            descricao as Descricao 
                        FROM 
                            tb_candidato_status
                        WHERE 
                            origem = 'Fourmakers';";

            return (await connection.QueryAsync<StatusCandidaturaRecrutamentoDTO>(query)).ToList();
        }

        public async Task<IEnumerable<MotivoDescandidatarDTO>> ListarMotivosDescandidatura()
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                            SELECT 
                                id AS Id,
                                descricao AS Descricao
                            FROM 
                                tb_motivo_descandidatar
                            ORDER BY 
                                descricao";

            return await connection.QueryAsync<MotivoDescandidatarDTO>(query);
        }

        public async Task DescandidatarSe(string idCandidatura, string idMotivoDescandidatura)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                            UPDATE tb_candidato_vaga
                                SET ativo = 0
                            WHERE
                                id = @idCandidatura";

            await connection.ExecuteAsync(query, new { idCandidatura });

            var queryMotivoDescandidatura = @"
                            INSERT INTO 
                                tb_descandidaturas (tb_candidato_vaga_id, tb_motivo_descandidatar_id)
                            VALUES 
                                (@idCandidatura, @idMotivoDescandidatura);";

            await connection.ExecuteAsync(queryMotivoDescandidatura, new { idCandidatura, idMotivoDescandidatura });
        }

        public async Task CandidatarOutraPessoa(string codigoColaborador, string vagaId, int orgId, int status, string codUsuarioLogado, decimal? pretencaoSalarial = null, string modeloTrabalhoId = null, string disponibilidadeEntrevistaId = null, int? quantidadeDiasPresencial = null)
        {
            var connection = _dapperConnection.GetConnection();
            var candidatura = await CadastrarCandidatura(codigoColaborador, vagaId, orgId, status, pretencaoSalarial, modeloTrabalhoId, disponibilidadeEntrevistaId, quantidadeDiasPresencial, connection);

            await CadastrarLogCandidatura(codUsuarioLogado, status, connection, candidatura);
            await InserirLogPretensaoModeloSeNecessarioAsync(connection, null, candidatura, codUsuarioLogado, "INSERT", OrigemAlteracaoPretensaoModeloLogEnum.Candidato);
        }
        private static async Task CadastrarLogCandidatura(string codigoInternoColaborador, int status, MySql.Data.MySqlClient.MySqlConnection connection, CandidaturaRecrutamentoDTO candidatura)
        {
            var queryLog = @"
                INSERT INTO tb_candidato_vaga_log (
                    id,
                    tb_candidato_vaga_id,
                    tb_candidato_status_id,
                    tb_colaborador_codigo_interno_colaborador,
                    data_alteracao,
                    objeto
                ) VALUES (
                    @idLog,
                    @candidaturaId,
                    @status,
                    @codigoInternoColaborador,
                    NOW(),
                    @objeto
                );";

            var idLog = Guid.NewGuid().ToString();
            var candidaturaId = candidatura.Id;

            var parametrosLog = new
            {
                idLog,
                candidaturaId,
                status,
                codigoInternoColaborador,
                objeto = JsonSerializer.Serialize(candidatura)
            };

            await connection.ExecuteAsync(queryLog, parametrosLog);
        }

        private static async Task<CandidaturaRecrutamentoDTO> CadastrarCandidatura(string codigoInternoColaborador, string vagaId, int orgId, int status, decimal? pretencaoSalarial, string modeloTrabalhoId, string disponibilidadeEntrevistaId, int? quantidadeDiasPresencial, MySql.Data.MySqlClient.MySqlConnection connection)
        {
            var idCandidatarSe = Guid.NewGuid().ToString();
            var query = @"
                INSERT INTO tb_candidato_vaga (
                    id,
                    tb_colaborador_codigo_interno_colaborador,
                    tb_vaga_id,
                    tb_org_id,
                    tb_candidato_status_id,
                    data_criacao,
                    ativo,
                    pretensao_salarial,
                    tb_modelo_trabalho_id,
                    tb_disponibilidade_entrevista_id,
                    quantidade_dias_presencial
                ) VALUES (
                    @idCandidatarSe,
                    @codigoInternoColaborador,
                    @vagaId,
                    @orgId,
                    @status,
                    Now(),
                    1,
                    @pretencaoSalarial,
                    @modeloTrabalhoId,
                    @disponibilidadeEntrevistaId,
                    @quantidadeDiasPresencial
                );

                SELECT 
                    tcv.id AS Id,
	                tcv.tb_colaborador_codigo_interno_colaborador AS CodColaborador,
	                tcv.tb_vaga_id AS IdVaga,
	                tcv.tb_org_id AS OrgId,
	                tcv.ativo AS Ativa,   
                    tcv.data_criacao AS Candidatura,        
                    tcv.data_ultima_alteracao AS UltimaAlteracao,
                    tcv.tb_candidato_status_id AS StatusId,
                    tcv.pretensao_salarial AS PretencaoSalarial,
                    tcv.tb_modelo_trabalho_id AS ModeloTrabalhoId,
                    tcv.tb_disponibilidade_entrevista_id AS DisponibilidadeEntrevistaId,
                    tcv.quantidade_dias_presencial AS QuantidadeDiasPresencial
                FROM 
                    tb_candidato_vaga tcv
                WHERE
                    id = @idCandidatarSe;
            ";

            var parametros = new
            {
                idCandidatarSe,
                codigoInternoColaborador,
                vagaId,
                orgId,
                status,
                pretencaoSalarial,
                modeloTrabalhoId,
                disponibilidadeEntrevistaId,
                quantidadeDiasPresencial
            };

            var candidatura = await connection.QueryFirstOrDefaultAsync<CandidaturaRecrutamentoDTO>(query, parametros);
            return candidatura;
        }

        public async Task<IEnumerable<CandidaturaLogDTO>> ListarLogsCandidaturaPorColaborador(string codColaborador)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT 
                    log.tb_candidato_vaga_id AS IdCandidatura,
                    log.tb_candidato_status_id AS IdStatus,
                    status.descricao AS StatusDescricao,
                    colab.nome_completo AS Recrutador,
                    candidato.nome_completo AS NomeCandidato,
                    vaga.codigo AS CodVaga,
                    vaga.titulo AS NomeVaga,
                    gestor.nome AS NomeGestor,
                    log.data_alteracao AS DataAlteracao,
                    log.objeto
                FROM 
                    tb_candidato_vaga_log log
                INNER JOIN 
                    tb_colaborador colab 
                        ON colab.codigo_interno_colaborador = log.tb_colaborador_codigo_interno_colaborador
                INNER JOIN 
                    tb_candidato_status status 
                        ON status.id = log.tb_candidato_status_id AND status.origem = 'FOURMAKERS'
                INNER JOIN 
                    tb_candidato_vaga candidatura 
                        ON candidatura.id = log.tb_candidato_vaga_id
                INNER JOIN 
                    tb_colaborador candidato 
                        ON candidato.codigo_interno_colaborador = candidatura.tb_colaborador_codigo_interno_colaborador
                INNER JOIN 
                    tb_vaga vaga 
                        ON vaga.id = candidatura.tb_vaga_id
                LEFT JOIN 
                    tb_gestor_externo gestor 
                        ON gestor.cod_gestor_externo = vaga.tb_gestor_cod AND gestor.tb_org_id = vaga.tb_org_id
                WHERE 
                    candidatura.tb_colaborador_codigo_interno_colaborador = @codColaborador
                ORDER BY 
                    log.data_alteracao ASC;
            ";

            var parametros = new
            {
                codColaborador
            };

            var logs = await connection.QueryAsync<CandidaturaLogDTO>(query, parametros);

            var cacheColaboradores = new Dictionary<string, string>();

            foreach (var log in logs)
            {
                if (string.IsNullOrWhiteSpace(log.Objeto))
                    continue;

                try
                {
                    var objeto = JsonSerializer.Deserialize<CandidatoVagaLogObjetoDTO>(
                        log.Objeto,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (string.IsNullOrWhiteSpace(objeto?.ColaboradorResponsavel))
                        continue;

                    if (!cacheColaboradores.TryGetValue(objeto.ColaboradorResponsavel, out var nome))
                    {
                        nome = await BuscarColaboradorLog(objeto.ColaboradorResponsavel);
                        cacheColaboradores[objeto.ColaboradorResponsavel] = nome;
                    }

                    log.RecrutadorResponsavel = nome;
                }
                catch (JsonException)
                {
                    // opcional: logar JSON inválido
                }
            }

            return logs;
        }

        public async Task<IEnumerable<CandidaturaAgrupadaDTO>> ListarLogsCandidaturaAgrupadosPorColaborador(string codColaborador, string busca)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT 
                    log.tb_candidato_vaga_id AS IdCandidatura,
                    log.tb_candidato_status_id AS IdStatus,
                    status.descricao AS StatusDescricao,
                    colab.nome_completo AS Recrutador,
                    candidato.nome_completo AS NomeCandidato,
                    vaga.codigo AS CodVaga,
                    vaga.titulo AS NomeVaga,
                    gestor.nome AS NomeGestor,
                    tco.nome_cliente AS NomeCliente,
                    log.data_alteracao AS DataAlteracao,
                    log.objeto,
                    c.texto AS ComentarioTexto,
                    log.tb_comentario_id AS IdComentario,
                    candidatura.pretensao_salarial AS PretencaoSalarial,
                    candidatura.tb_modelo_trabalho_id AS ModeloTrabalhoId,
                    candidatura.tb_disponibilidade_entrevista_id AS DisponibilidadeEntrevistaId,
                    candidatura.quantidade_dias_presencial AS QuantidadeDiasPresencial,
                    tmt.descricao AS ModeloTrabalhoDescricao,
                    tde.descricao AS DisponibilidadeEntrevistaDescricao,
                    candidato.qualificado  AS Qualificado,
                    tmr.descricao AS MotivoReprovacao,
                    tmd.descricao AS MotivoDeclinio,
                    tsv.descricao AS StatusDaVaga
                FROM 
                    tb_candidato_vaga_log log
                INNER JOIN 
                    tb_colaborador colab 
                        ON colab.codigo_interno_colaborador = log.tb_colaborador_codigo_interno_colaborador
                INNER JOIN 
                    tb_candidato_status status 
                        ON status.id = log.tb_candidato_status_id AND status.origem = 'FOURMAKERS'
                INNER JOIN 
                    tb_candidato_vaga candidatura 
                        ON candidatura.id = log.tb_candidato_vaga_id
                INNER JOIN 
                    tb_colaborador candidato 
                        ON candidato.codigo_interno_colaborador = candidatura.tb_colaborador_codigo_interno_colaborador
                INNER JOIN 
                    tb_vaga vaga 
                        ON vaga.id = candidatura.tb_vaga_id
                LEFT JOIN 
                    tb_status_vaga tsv 
                        ON tsv.codigo = vaga.tb_status_vaga_cod
                LEFT JOIN 
                    tb_gestor_externo gestor 
                        ON gestor.cod_gestor_externo = vaga.tb_gestor_cod AND gestor.tb_org_id = vaga.tb_org_id
                LEFT JOIN 
                    tb_cliente_org tco 
                        ON tco.codigo_cliente = gestor.codigo_cliente AND tco.tb_org_id = gestor.tb_org_id
                LEFT JOIN 
                    tb_comentario c 
                        ON c.id = log.tb_comentario_id
                LEFT JOIN 
                    tb_modelo_trabalho tmt 
                        ON candidatura.tb_modelo_trabalho_id = tmt.id
                LEFT JOIN 
                    tb_disponibilidade_entrevista tde 
                        ON candidatura.tb_disponibilidade_entrevista_id = tde.id
                LEFT JOIN 
	                tb_motivo_reprovacao tmr 
		                ON candidatura.tb_motivo_reprovacao_id = tmr.id
                LEFT JOIN 
	                tb_motivo_declinio tmd 
		                ON candidatura.tb_motivo_declinio_id  = tmd.id
                WHERE 
                    candidatura.tb_colaborador_codigo_interno_colaborador = @codColaborador
                    AND (@busca IS NULL OR @busca = '' OR 
                        LOWER(gestor.nome) LIKE CONCAT('%', LOWER(@busca), '%') OR
                        LOWER(vaga.codigo) LIKE CONCAT('%', LOWER(@busca), '%') OR
                        LOWER(vaga.titulo) LIKE CONCAT('%', LOWER(@busca), '%'))
                ORDER BY 
                    log.tb_candidato_vaga_id, log.data_alteracao ASC;
            ";

            var parametros = new
            {
                codColaborador,
                busca
            };

            var logs = await connection.QueryAsync<dynamic>(query, parametros);

            // Agrupa os logs por candidatura e adiciona os comentários

            var candidaturasAgrupadas = new List<CandidaturaAgrupadaDTO>();
            foreach (var grupo in logs.GroupBy(log => new { log.IdCandidatura, log.NomeCandidato, log.CodVaga, log.NomeVaga, log.NomeGestor, log.NomeCliente, log.PretencaoSalarial, log.ModeloTrabalhoId, log.ModeloTrabalhoDescricao, log.DisponibilidadeEntrevistaId, log.DisponibilidadeEntrevistaDescricao, log.QuantidadeDiasPresencial, log.Qualificado, log.StatusDaVaga }))
            {
                var candidaturaId = grupo.Key.IdCandidatura;
                var comentarios = await _comentarioRepository.GetByCandidaturaIdAsync(candidaturaId);

                var alteracoesComArquivos = new List<CandidaturaAgrupadaAlteracaoDTO>();
                foreach (var log in grupo)
                {
                    CandidatoVagaLogObjetoDTO objetoLog = null;
                    var nomeRecrutadorResponsavel = string.Empty;

                    if (!string.IsNullOrEmpty(log.objeto))
                    {
                        try
                        {
                            objetoLog = JsonSerializer.Deserialize<CandidatoVagaLogObjetoDTO>(
                                log.objeto,
                                new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });
                        }
                        catch
                        {
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(objetoLog?.ColaboradorResponsavel))
                    {
                        nomeRecrutadorResponsavel =
                            await BuscarColaboradorLog(objetoLog.ColaboradorResponsavel);
                    }

                    var alteracao = new CandidaturaAgrupadaAlteracaoDTO
                    {
                        IdStatus = log.IdStatus,
                        StatusDescricao = log.StatusDescricao,
                        Recrutador = log.Recrutador,
                        DataAlteracao = log.DataAlteracao,
                        Comentario = log.ComentarioTexto,
                        IdComentario = log.IdComentario,
                        MotivoReprovacao = log.IdStatus == StatusCandidaturaRecrutamento.Reprovado.ToInt() ? log.MotivoReprovacao : String.Empty,
                        MotivoDeclinio = log.IdStatus == StatusCandidaturaRecrutamento.Declinou.ToInt() ? log.MotivoDeclinio : String.Empty,
                        RecrutadorResponsavel = nomeRecrutadorResponsavel
                    };

                    if (!string.IsNullOrEmpty(log.IdComentario))
                        alteracao.Arquivos = await _comentarioRepository.GetArquivosByComentarioIdAsync(log.IdComentario);

                    alteracoesComArquivos.Add(alteracao);
                }

                CandidaturaAgrupadaAlteracaoDTO ultimaAlteracao = null;

                if (alteracoesComArquivos != null && alteracoesComArquivos.Any())
                {
                    ultimaAlteracao = alteracoesComArquivos
                        .OrderByDescending(a => a.DataAlteracao)
                        .FirstOrDefault();
                }

                candidaturasAgrupadas.Add(new CandidaturaAgrupadaDTO
                {
                    IdCandidatura = grupo.Key.IdCandidatura,
                    NomeCandidato = grupo.Key.NomeCandidato,
                    CodVaga = grupo.Key.CodVaga,
                    NomeVaga = grupo.Key.NomeVaga,
                    NomeGestor = grupo.Key.NomeGestor,
                    NomeCliente = grupo.Key.NomeCliente,
                    PretencaoSalarial = grupo.Key.PretencaoSalarial,
                    ModeloTrabalhoId = grupo.Key.ModeloTrabalhoId,
                    ModeloTrabalhoDescricao = grupo.Key.ModeloTrabalhoDescricao,
                    DisponibilidadeEntrevistaId = grupo.Key.DisponibilidadeEntrevistaId,
                    DisponibilidadeEntrevistaDescricao = grupo.Key.DisponibilidadeEntrevistaDescricao,
                    QuantidadeDiasPresencial = grupo.Key.QuantidadeDiasPresencial,
                    Alteracoes = alteracoesComArquivos,
                    Comentarios = comentarios,
                    DataUltimaAlteracao = ultimaAlteracao?.DataAlteracao,
                    DescricaoUltimaAlteracao = ultimaAlteracao?.StatusDescricao,
                    Qualificado = grupo.Key.Qualificado,
                    StatusDaVaga = grupo.Key.StatusDaVaga
                });
            }

            return candidaturasAgrupadas;
        }

        public async Task<IEnumerable<DisponibilidadeEntrevistaDTO>> ListarDisponibilidadesEntrevista()
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT 
                    id AS Id,
                    descricao AS Descricao
                FROM 
                    tb_disponibilidade_entrevista
                ORDER BY 
                    descricao;";

            var disponibilidades = await connection.QueryAsync<DisponibilidadeEntrevistaDTO>(query);
            return disponibilidades;
        }

        private async Task<string> BuscarColaboradorLog(string codColaborador)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT tc.nome_completo  
                FROM tb_colaborador tc 
                WHERE tc.codigo_interno_colaborador = @codColaborador;";

            var nomeColaborador = await connection.QuerySingleOrDefaultAsync<string>(query, new { codColaborador });
            return nomeColaborador;
        }

        public async Task<bool> EstaModeloTrabalhoExiste(string id)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT COUNT(1)
                FROM tb_modelo_trabalho
                WHERE id = @id;";

            var count = await connection.ExecuteScalarAsync<int>(query, new { id });
            return count > 0;
        }

        public async Task<bool> EstaDisponibilidadeEntrevistaExiste(string id)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT COUNT(1)
                FROM tb_disponibilidade_entrevista
                WHERE id = @id;";

            var count = await connection.ExecuteScalarAsync<int>(query, new { id });
            return count > 0;
        }

        public async Task<IEnumerable<QuantidadeCandidatosPorEstagioDTO>> ObterQuantidadeCandidatosPorEstagio(string vagaId)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT 
                    tcv.tb_candidato_status_id AS IdStatus,
                    tcs.descricao AS DescricaoStatus,
                    COUNT(*) AS Quantidade
                FROM 
                    tb_candidato_vaga tcv
                INNER JOIN 
                    tb_candidato_status tcs ON tcv.tb_candidato_status_id = tcs.id
                WHERE 
                    tcv.tb_vaga_id = @vagaId
                    AND tcv.ativo = 1
                    AND tcs.origem = 'Fourmakers'
                GROUP BY 
                    tcv.tb_candidato_status_id, tcs.descricao
                ORDER BY 
                    tcv.tb_candidato_status_id;";

            var parameters = new { vagaId };
            await connection.CloseAsync();

            return await connection.QueryAsync<QuantidadeCandidatosPorEstagioDTO>(query, parameters);
        }

        public async Task<Dictionary<string, List<QuantidadeCandidatosPorEstagioDTO>>> ObterQuantidadeCandidatosPorEstagioMultiplasVagas(IEnumerable<string> vagaIds)
        {
            var vagaIdsList = vagaIds.ToList();
            if (!vagaIdsList.Any())
                return new Dictionary<string, List<QuantidadeCandidatosPorEstagioDTO>>();

            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT 
                    tcv.tb_vaga_id AS VagaId,
                    tcv.tb_candidato_status_id AS IdStatus,
                    tcs.descricao AS DescricaoStatus,
                    COUNT(*) AS Quantidade
                FROM 
                    tb_candidato_vaga tcv
                INNER JOIN 
                    tb_candidato_status tcs ON tcv.tb_candidato_status_id = tcs.id
                WHERE 
                    tcv.tb_vaga_id IN @vagaIds
                    AND tcv.ativo = 1
                    AND tcs.origem = 'Fourmakers'
                GROUP BY 
                    tcv.tb_vaga_id, tcv.tb_candidato_status_id, tcs.descricao
                ORDER BY 
                    tcv.tb_vaga_id, tcv.tb_candidato_status_id;";

            var parameters = new { vagaIds = vagaIdsList };
            var results = await connection.QueryAsync<QuantidadeCandidatosPorEstagioResult>(query, parameters);
            await connection.CloseAsync();

            // Agrupar por VagaId e converter para o DTO
            var resultadoPorVaga = results
                .GroupBy(r => r.VagaId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(r => new QuantidadeCandidatosPorEstagioDTO
                    {
                        IdStatus = r.IdStatus,
                        DescricaoStatus = r.DescricaoStatus,
                        Quantidade = r.Quantidade
                    }).ToList()
                );

            return resultadoPorVaga;
        }

        public async Task AtualizarCandidatura(string idCandidatura, decimal? pretencaoSalarial, string modeloTrabalhoId, string disponibilidadeEntrevistaId, int? quantidadeDiasPresencial, string codigoInternoColaborador, OrigemAlteracaoPretensaoModeloLogEnum origemAlteracao = OrigemAlteracaoPretensaoModeloLogEnum.MovimentacaoKanban)
        {
            var connection = _dapperConnection.GetConnection();
            var origemTexto = origemAlteracao.ParaTextoPersistencia();

            using var transaction = connection.BeginTransaction();

            try
            {
                var antes = await ObterCandidaturaPorId(idCandidatura);
                if (antes == null)
                    throw new ApplicationException("Candidatura não encontrada.");

                var query = @"
                    UPDATE tb_candidato_vaga 
                    SET 
                        pretensao_salarial = @pretencaoSalarial,
                        tb_modelo_trabalho_id = @modeloTrabalhoId,
                        tb_disponibilidade_entrevista_id = @disponibilidadeEntrevistaId,
                        quantidade_dias_presencial = @quantidadeDiasPresencial,
                        data_ultima_alteracao = NOW()
                    WHERE 
                        id = @idCandidatura;";

                var parametros = new
                {
                    idCandidatura,
                    pretencaoSalarial,
                    modeloTrabalhoId,
                    disponibilidadeEntrevistaId,
                    quantidadeDiasPresencial
                };

                await connection.ExecuteAsync(query, parametros, transaction);

                var pretAlterou = PretensaoSalarialDiferente(antes.PretencaoSalarial, pretencaoSalarial);
                var modeloAlterou = ModeloTrabalhoDiferente(antes.ModeloTrabalhoId, modeloTrabalhoId);
                if (pretAlterou || modeloAlterou)
                {
                    await InserirLogPretensaoModeloAsync(connection, transaction, idCandidatura, antes.CodColaborador,
                        "UPDATE",
                        pretAlterou ? antes.PretencaoSalarial : null,
                        pretAlterou ? pretencaoSalarial : null,
                        modeloAlterou ? NormalizarModeloTrabalhoId(antes.ModeloTrabalhoId) : null,
                        modeloAlterou ? NormalizarModeloTrabalhoId(modeloTrabalhoId) : null,
                        codigoInternoColaborador, origemTexto);
                }

                const string queryCandidaturaAposUpdate = @"
                    SELECT 
                         tcv.id AS Id,
	                     tcv.tb_colaborador_codigo_interno_colaborador AS CodColaborador,
	                     tcv.tb_vaga_id AS IdVaga,
	                     tcv.tb_org_id AS OrgId,
	                     tcv.ativo AS Ativa,
                         tv.titulo AS TituloVaga,    
                         tcv.data_criacao AS Candidatura,        
                         tcv.data_ultima_alteracao AS UltimaAlteracao,
                         tcv.tb_candidato_status_id AS StatusId,        
                         tcs.descricao AS StatusDescricao,
                         tcv.pretensao_salarial AS PretencaoSalarial,
                         tcv.tb_modelo_trabalho_id AS ModeloTrabalhoId,
                         tcv.tb_disponibilidade_entrevista_id AS DisponibilidadeEntrevistaId,
                         tcv.quantidade_dias_presencial AS QuantidadeDiasPresencial,
                         tcv.tb_colaborador_codigo_interno_colaborador_responsavel AS ColaboradorResponsavel
                     FROM tb_vaga tv
                     INNER JOIN tb_candidato_vaga tcv ON tcv.tb_vaga_id = tv.id
                     INNER JOIN tb_candidato_status tcs ON tcs.id = tcv.tb_candidato_status_id AND tcs.origem = 'Fourmakers'
                     WHERE tcv.id = @idCandidatura
                     ORDER BY tv.data_criacao DESC
                     LIMIT 1;";

                var candidatura = await connection.QueryFirstOrDefaultAsync<CandidaturaRecrutamentoDTO>(queryCandidaturaAposUpdate, new { idCandidatura }, transaction);
                if (candidatura == null)
                    throw new ApplicationException("Candidatura não encontrada após atualização.");

                await GravarLogCandidatura(idCandidatura, candidatura.StatusId.ToInt(), codigoInternoColaborador, candidatura, transaction);

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<List<LogPretensaoModeloCandidaturaDTO>> ListarLogPretensaoModeloPorCandidatura(Guid idCandidatura, int limit, int cursor)
        {
            var connection = _dapperConnection.GetConnection();
            var idCandidaturaTexto = idCandidatura.ToString();

            const string query = @"
                SELECT 
                    log.id AS Id,
                    log.tb_candidato_vaga_id AS IdCandidatura,
                    tcv.tb_vaga_id AS IdVaga,
                    tv.titulo AS TituloVaga,
                    log.tb_colaborador_codigo_interno_colaborador_candidato AS CodigoInternoCandidato,
                    cand.nome_completo AS NomeCandidato,
                    log.tipo_operacao AS TipoOperacao,
                    log.pretensao_salarial_valor_anterior AS PretensaoSalarialValorAnterior,
                    log.pretensao_salarial_valor_novo AS PretensaoSalarialValorNovo,
                    log.tb_modelo_trabalho_id_anterior AS ModeloTrabalhoIdAnterior,
                    log.tb_modelo_trabalho_id_novo AS ModeloTrabalhoIdNovo,
                    mta.descricao AS ModeloTrabalhoDescricaoAnterior,
                    mtn.descricao AS ModeloTrabalhoDescricaoNovo,
                    log.tb_colaborador_codigo_interno_colaborador_executor AS CodigoInternoExecutor,
                    ex.nome_completo AS NomeExecutor,
                    log.data_alteracao AS DataAlteracao,
                    log.origem_alteracao AS OrigemAlteracao
                FROM tb_candidato_vaga_log_pretensao_modelo log
                INNER JOIN tb_candidato_vaga tcv ON tcv.id = log.tb_candidato_vaga_id
                INNER JOIN tb_vaga tv ON tv.id = tcv.tb_vaga_id
                INNER JOIN tb_colaborador cand ON cand.codigo_interno_colaborador = log.tb_colaborador_codigo_interno_colaborador_candidato
                INNER JOIN tb_colaborador ex ON ex.codigo_interno_colaborador = log.tb_colaborador_codigo_interno_colaborador_executor
                LEFT JOIN tb_modelo_trabalho mta ON mta.id = log.tb_modelo_trabalho_id_anterior
                LEFT JOIN tb_modelo_trabalho mtn ON mtn.id = log.tb_modelo_trabalho_id_novo
                WHERE log.tb_candidato_vaga_id = @idCandidatura
                ORDER BY log.data_alteracao DESC, log.id DESC
                LIMIT @limit OFFSET @cursor;";

            var rows = await connection.QueryAsync<LogPretensaoModeloCandidaturaDTO>(query, new { idCandidatura = idCandidaturaTexto, limit, cursor });
            return rows.ToList();
        }

        public async Task<List<LogPretensaoModeloCandidaturaDTO>> ListarLogPretensaoModeloPorOrganizacao(int orgId, int limit, int cursor, string codigoInternoCandidato)
        {
            var connection = _dapperConnection.GetConnection();

            const string query = @"
                SELECT 
                    log.id AS Id,
                    log.tb_candidato_vaga_id AS IdCandidatura,
                    tcv.tb_vaga_id AS IdVaga,
                    tv.titulo AS TituloVaga,
                    log.tb_colaborador_codigo_interno_colaborador_candidato AS CodigoInternoCandidato,
                    cand.nome_completo AS NomeCandidato,
                    log.tipo_operacao AS TipoOperacao,
                    log.pretensao_salarial_valor_anterior AS PretensaoSalarialValorAnterior,
                    log.pretensao_salarial_valor_novo AS PretensaoSalarialValorNovo,
                    log.tb_modelo_trabalho_id_anterior AS ModeloTrabalhoIdAnterior,
                    log.tb_modelo_trabalho_id_novo AS ModeloTrabalhoIdNovo,
                    mta.descricao AS ModeloTrabalhoDescricaoAnterior,
                    mtn.descricao AS ModeloTrabalhoDescricaoNovo,
                    log.tb_colaborador_codigo_interno_colaborador_executor AS CodigoInternoExecutor,
                    ex.nome_completo AS NomeExecutor,
                    log.data_alteracao AS DataAlteracao,
                    log.origem_alteracao AS OrigemAlteracao
                FROM tb_candidato_vaga_log_pretensao_modelo log
                INNER JOIN tb_candidato_vaga tcv ON tcv.id = log.tb_candidato_vaga_id
                INNER JOIN tb_vaga tv ON tv.id = tcv.tb_vaga_id
                INNER JOIN tb_colaborador cand ON cand.codigo_interno_colaborador = log.tb_colaborador_codigo_interno_colaborador_candidato
                INNER JOIN tb_colaborador ex ON ex.codigo_interno_colaborador = log.tb_colaborador_codigo_interno_colaborador_executor
                LEFT JOIN tb_modelo_trabalho mta ON mta.id = log.tb_modelo_trabalho_id_anterior
                LEFT JOIN tb_modelo_trabalho mtn ON mtn.id = log.tb_modelo_trabalho_id_novo
                WHERE tcv.tb_org_id = @orgId
                      AND log.tb_colaborador_codigo_interno_colaborador_candidato = @codigoInternoCandidato
                      ORDER BY log.data_alteracao DESC, log.id DESC
                LIMIT @limit OFFSET @cursor;";

            var result = await connection.QueryAsync<LogPretensaoModeloCandidaturaDTO>(query, new { orgId, limit, cursor, codigoInternoCandidato });
            return result.ToList();
        }

        public async Task<UltimaPretensaoModeloColaboradorSnapshotDTO> ObterUltimaPretensaoModeloInformadaColaboradorAsync(string codigoInternoColaborador)
        {
            if (string.IsNullOrWhiteSpace(codigoInternoColaborador))
                return new UltimaPretensaoModeloColaboradorSnapshotDTO();

            try
            {
                var connection = _dapperConnection.GetConnection();
                // Pretensão e modelo: cada um usa a última alteração não nula no log; se não houver, o valor da candidatura mais recentemente alterada.
                const string sql = @"
                                    SELECT
                                        p.UltimaPretensaoSalarial,
                                        mi.UltimoModeloTrabalhoId,
                                        m.descricao AS UltimoModeloTrabalhoDescricao
                                    FROM (
                                        SELECT COALESCE(
                                            (SELECT l.pretensao_salarial_valor_novo FROM tb_candidato_vaga_log_pretensao_modelo l
                                             WHERE l.tb_colaborador_codigo_interno_colaborador_candidato = @cpf AND l.pretensao_salarial_valor_novo IS NOT NULL
                                             ORDER BY l.data_alteracao DESC LIMIT 1),
                                            (SELECT tcv.pretensao_salarial FROM tb_candidato_vaga tcv
                                             WHERE tcv.tb_colaborador_codigo_interno_colaborador = @cpf AND tcv.pretensao_salarial IS NOT NULL
                                             ORDER BY tcv.data_ultima_alteracao DESC LIMIT 1)
                                        ) AS UltimaPretensaoSalarial
                                    ) p
                                    CROSS JOIN (
                                        SELECT COALESCE(
                                            (SELECT l.tb_modelo_trabalho_id_novo FROM tb_candidato_vaga_log_pretensao_modelo l
                                             WHERE l.tb_colaborador_codigo_interno_colaborador_candidato = @cpf AND l.tb_modelo_trabalho_id_novo IS NOT NULL
                                             ORDER BY l.data_alteracao DESC LIMIT 1),
                                            (SELECT tcv.tb_modelo_trabalho_id FROM tb_candidato_vaga tcv
                                             WHERE tcv.tb_colaborador_codigo_interno_colaborador = @cpf AND tcv.tb_modelo_trabalho_id IS NOT NULL
                                             ORDER BY tcv.data_ultima_alteracao DESC LIMIT 1)
                                        ) AS UltimoModeloTrabalhoId
                                    ) mi
                                    LEFT JOIN tb_modelo_trabalho m ON m.id = mi.UltimoModeloTrabalhoId;";

                var row = await connection.QueryFirstOrDefaultAsync<UltimaPretensaoModeloColaboradorSnapshotDTO>(sql, new { cpf = codigoInternoColaborador });
                return row ?? new UltimaPretensaoModeloColaboradorSnapshotDTO();
            }
            catch (MySqlException ex) when (ex.Number == 1146)
            {
                return new UltimaPretensaoModeloColaboradorSnapshotDTO();
            }
        }

        private static string NormalizarModeloTrabalhoId(string id) =>
            string.IsNullOrWhiteSpace(id) ? null : id.Trim();

        private static bool PretensaoSalarialDiferente(decimal? anterior, decimal? novo)
        {
            if (!anterior.HasValue && !novo.HasValue) return false;
            if (!anterior.HasValue || !novo.HasValue) return true;
            return anterior.Value != novo.Value;
        }

        private static bool ModeloTrabalhoDiferente(string anterior, string novo) =>
            NormalizarModeloTrabalhoId(anterior) != NormalizarModeloTrabalhoId(novo);

        private static async Task InserirLogPretensaoModeloSeNecessarioAsync(IDbConnection connection, IDbTransaction transaction,
            CandidaturaRecrutamentoDTO candidatura, string codigoExecutor, string tipoOperacao, OrigemAlteracaoPretensaoModeloLogEnum origem)
        {
            var pretNovo = candidatura.PretencaoSalarial;
            var modeloNovo = NormalizarModeloTrabalhoId(candidatura.ModeloTrabalhoId);
            if (!pretNovo.HasValue && modeloNovo == null)
                return;

            await InserirLogPretensaoModeloAsync(connection, transaction, candidatura.Id, candidatura.CodColaborador,
                tipoOperacao, null, pretNovo, null, modeloNovo, codigoExecutor, origem.ParaTextoPersistencia());
        }

        private static async Task InserirLogPretensaoModeloAsync(IDbConnection connection, IDbTransaction transaction,
            string idCandidatura, string codigoCandidato, string tipoOperacao,
            decimal? pretAnt, decimal? pretNovo, string modeloAnt, string modeloNovo,
            string codigoExecutor, string origem)
        {
            var sql = @"
                INSERT INTO tb_candidato_vaga_log_pretensao_modelo (
                    id,
                    tb_candidato_vaga_id,
                    tb_colaborador_codigo_interno_colaborador_candidato,
                    tipo_operacao,
                    pretensao_salarial_valor_anterior,
                    pretensao_salarial_valor_novo,
                    tb_modelo_trabalho_id_anterior,
                    tb_modelo_trabalho_id_novo,
                    tb_colaborador_codigo_interno_colaborador_executor,
                    data_alteracao,
                    origem_alteracao
                ) VALUES (
                    @id,
                    @idCv,
                    @codCand,
                    @tipo,
                    @pretA,
                    @pretN,
                    @modA,
                    @modN,
                    @exec,
                    NOW(),
                    @origem
                );";

            await connection.ExecuteAsync(sql, new
            {
                id = Guid.NewGuid().ToString(),
                idCv = idCandidatura,
                codCand = codigoCandidato,
                tipo = tipoOperacao,
                pretA = pretAnt,
                pretN = pretNovo,
                modA = modeloAnt,
                modN = modeloNovo,
                exec = codigoExecutor,
                origem
            }, transaction);
        }

        public async Task AtualizarRecrutadorResponsavel(string idCandidatura, string? codigoRecrutadorResponsavel, string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();

            // Iniciar transação
            using var transaction = connection.BeginTransaction();

            try
            {
                var query = @"
                    UPDATE tb_candidato_vaga 
                    SET 
                        tb_colaborador_codigo_interno_colaborador_responsavel = @codigoRecrutadorResponsavel
                    WHERE 
                        id = @idCandidatura;";

                var parametros = new
                {
                    idCandidatura,
                    codigoRecrutadorResponsavel
                };

                await connection.ExecuteAsync(query, parametros, transaction);

                // Obter os dados da candidatura para o log
                var candidatura = await ObterCandidaturaPorId(idCandidatura);

                // Gravar o log da alteração
                await GravarLogCandidatura(idCandidatura, candidatura.StatusId.ToInt(), codigoInternoColaborador, candidatura, transaction);

                // Commit da transação
                transaction.Commit();
            }
            catch (Exception)
            {
                // Rollback em caso de erro
                transaction.Rollback();
                throw;
            }
        }

        public async Task GravarLogCandidatura(string idCandidatura, int statusCandidatura, string codigoInternoColaborador, object objeto, IDbTransaction transaction = null)
        {
            var connection = _dapperConnection.GetConnection();

            var queryLog = @"
                INSERT INTO tb_candidato_vaga_log (
                    id,
                    tb_candidato_vaga_id,
                    tb_candidato_status_id,
                    tb_colaborador_codigo_interno_colaborador,
                    data_alteracao,
                    objeto
                ) VALUES (
                    @idLog,
                    @candidaturaId,
                    @status,
                    @codigoInternoColaborador,
                    NOW(),
                    @objeto
                );";

            var idLog = Guid.NewGuid().ToString();

            var parametrosLog = new
            {
                idLog,
                candidaturaId = idCandidatura,
                status = statusCandidatura,
                codigoInternoColaborador,
                objeto = JsonSerializer.Serialize(objeto)
            };

            await connection.ExecuteAsync(queryLog, parametrosLog, transaction);
        }

        public async Task<DataTransferObject.Domain.Vaga.CountCandidatosInscritosResult> CountCandidatosInscritos(string vagaId)
        {
            var connection = _dapperConnection.GetConnection();

            // Primeiro, buscar todos os status possíveis
            var queryTodosStatus = @"
                SELECT 
                    id as Id
                FROM 
                    tb_candidato_status
                WHERE 
                    origem = 'Fourmakers'
                ORDER BY 
                    id";

            var todosStatus = await connection.QueryAsync<int>(queryTodosStatus);

            // Inicializar o dicionário com todos os status com contagem zero
            var result = new DataTransferObject.Domain.Vaga.CountCandidatosInscritosResult();
            foreach (var statusId in todosStatus)
            {
                result.TotalCandidatosInscritos[statusId.ToString()] = 0;
            }

            // Agora buscar as contagens reais
            var query = @"
                SELECT 
                    tcv.tb_candidato_status_id as StatusId,
                    COUNT(DISTINCT tcv.tb_colaborador_codigo_interno_colaborador) as TotalCandidatos
                FROM 
	                tb_candidato_vaga tcv
                WHERE 
                    tb_vaga_id = @vagaId
                    AND ativo = 1
                GROUP BY 
                    tcv.tb_candidato_status_id";

            var resultados = await connection.QueryAsync(query, new { vagaId });

            // Atualizar as contagens reais
            foreach (var item in resultados)
            {
                result.TotalCandidatosInscritos[((int)item.StatusId).ToString()] = (int)item.TotalCandidatos;
            }

            return result;
        }

        public async Task<int> CountInscritoVagas(int orgId, string codColaborador)
        {
            //var connection = _dapperConnection.GetConnection();
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);
            await connection.OpenAsync();

            var query = @"
                        SELECT count(*) FROM tb_candidato_vaga va
                        LEFT JOIN tb_vaga vg ON vg.id = va.tb_vaga_id
                        WHERE va.tb_colaborador_codigo_interno_colaborador = @codColab    
                        AND vg.tb_status_vaga_cod IN (3,4,5,6,7,10)
                        AND   va.ativo = 1
                        AND   va.tb_org_id = @OrgId;
                    ";

            int countVagas = await connection.ExecuteScalarAsync<int>(
                query,
                new
                {
                    codColab = codColaborador,
                    OrgId = orgId
                }
            );

            await connection.CloseAsync();

            return countVagas > 0 ? countVagas - 1 : 0;
        }

        public async Task<PdfTemplateDTO> GetPdfTemplateHeaderPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga)
        {
            var conn = _dapperConnection.GetConnection();

            const string sql = """
                SELECT 
                t.id            AS Id,
                v.codigo        AS NumeroVaga,
                v.titulo        AS TituloVaga,
                co.nome_cliente AS Cliente
            FROM tb_template_pdf_admissao t
            JOIN tb_vaga v
              ON v.id = t.tb_vaga_id
            JOIN tb_candidato_vaga cv
              ON cv.id = t.tb_candidato_vaga_id
             AND cv.tb_vaga_id = v.id
            JOIN tb_gestor_externo ge
              ON ge.cod_gestor_externo = v.tb_gestor_cod
            JOIN tb_cliente_org co
              ON co.codigo_cliente = ge.codigo_cliente
            WHERE t.tb_candidato_vaga_id = @idCandidatura
              AND t.tb_vaga_id = @idVaga
              AND cv.tb_colaborador_codigo_interno_colaborador = @codInternoCandidato
            LIMIT 1;
            """;


            var result = await conn.QueryFirstOrDefaultAsync<PdfTemplateDTO>(sql, new { idCandidatura, idVaga, codInternoCandidato });
            return result ?? new PdfTemplateDTO();
        }

        public async Task<ProfissionalDTO> GetProfissionalPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga)
        {
            var conn = _dapperConnection.GetConnection();

            const string sql = """
                SELECT 
                    p.nome,
                    p.rg,
                    p.cpf,
                    p.data_nascimento AS DataNascimento,
                    c.contato_principal AS telefone,
                    p.email_pessoal  AS EmailPessoal,
                    p.tamanho_camiseta AS TamanhoCamiseta,
                    p.superior_imediato AS SuperiorImediato,
                    e.cep,
                    e.endereco,
                    e.numero,
                    e.complemento,
                    e.bairro,
                    e.cidade,
                    e.estado,
                    e.com_quem_mora,
                    e.internacional_linha_um,
                    e.internacional_linha_dois
                FROM tb_profissional_template_pdf p
                JOIN tb_template_pdf_admissao t
                  ON t.id = p.tb_template_pdf_admissao_id
                JOIN tb_candidato_vaga cv
                  ON cv.id = t.tb_candidato_vaga_id
                JOIN tb_colaborador c
                  ON c.codigo_interno_colaborador = cv.tb_colaborador_codigo_interno_colaborador
                LEFT JOIN tb_endereco e
                  ON e.id = c.endereco_id
                WHERE t.tb_candidato_vaga_id = @idCandidatura
                  AND t.tb_vaga_id = @idVaga
                  AND cv.tb_colaborador_codigo_interno_colaborador = @codInternoCandidato
                LIMIT 1;
            """;

            var result = await conn.QueryFirstOrDefaultAsync(sql, new { idCandidatura, idVaga, codInternoCandidato });

            if (result == null)
                return new ProfissionalDTO();

            var profissional = new ProfissionalDTO
            {
                Nome = result.nome,
                RG = result.rg,
                CPF = result.cpf,
                DataNascimento = result.DataNascimento,
                Telefone = result.telefone,
                EmailPessoal = result.EmailPessoal,
                TamanhoCamiseta = result.TamanhoCamiseta,
                SuperiorImediato = result.SuperiorImediato
            };

            // Constrói o EnderecoDTO se houver dados de endereço
            if (!string.IsNullOrWhiteSpace(result.endereco))
            {
                profissional.Endereco = new DataTransferObject.Domain.Endereco.EnderecoDTO
                {
                    Cep = result.cep,
                    Endereco = result.endereco,
                    Numero = result.numero,
                    Complemento = result.complemento,
                    Bairro = result.bairro,
                    Cidade = result.cidade,
                    Estado = result.estado,
                    ComQuemMora = result.com_quem_mora,
                    InternacionalLinhaUm = result.internacional_linha_um,
                    InternacionalLinhaDois = result.internacional_linha_dois
                };
            }

            return profissional;
        }

        public async Task<VagaAdmissaoDTO> GetVagaAdmissaoPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga)
        {
            var conn = _dapperConnection.GetConnection();

            const string sql = """
                SELECT  
                    vtp.id_vaga            AS IdVaga,
                    vtp.cliente            AS Cliente,
                    vtp.cargo              AS Cargo,
                    tbvmt.descricao        AS ModalidadeTrabalho,
                    vtp.tipo_vaga          AS TipoVaga,
                    vtp.endereco           AS Endereco,
                    vtp.cep                AS CEP,
                    vtp.data_inicio        AS DataInicio,
                    vtp.diretoria          AS Diretoria,
                    vtp.solicitante        AS Solicitante,
                    vtp.gestor_responsavel AS GestorResponsavel,
                    vtp.analista_responsavel AS AnalistaResponsavel,
                    vtp.unidade_trabalho   AS UnidadeTrabalho,
                    tbpl.descricao         AS LocalAlocacao,
                    vtp.area_staff         AS AreaStaff,
                    vtp.horario_trabalho   AS HorarioTrabalho,
                    vtp.horas_fechadas     AS HorasFechadas
                FROM tb_vaga_admissao_template_pdf vtp
                LEFT JOIN tb_template_pdf_admissao t
                  ON t.id = vtp.tb_template_pdf_admissao_id
                LEFT JOIN tb_candidato_vaga cv
                  ON cv.id = t.tb_candidato_vaga_id
                LEFT JOIN tb_modelo_trabalho tbvmt
                  ON tbvmt.id = cv.tb_modelo_trabalho_id
                LEFT JOIN tb_profissional_localidade tbpl
                  ON vtp.local_alocacao  = tbpl.id
                LEFT JOIN tb_tipo_vaga tbtv
                  on vtp.tipo_vaga = tbtv.id
                WHERE t.tb_candidato_vaga_id = @idCandidatura
                  AND t.tb_vaga_id = @idVaga
                  AND cv.tb_colaborador_codigo_interno_colaborador = @codInternoCandidato
                LIMIT 1;
            """;

            var result = await conn.QueryFirstOrDefaultAsync<VagaAdmissaoDTO>(sql, new { idCandidatura, idVaga, codInternoCandidato });

            return result ?? new VagaAdmissaoDTO();
        }

        public async Task<BeneficiosDTO> GetBeneficiosPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga)
        {
            var conn = _dapperConnection.GetConnection();

            const string sql = """
                SELECT  
                    b.salario,
                    b.vale_refeicao          AS ValeRefeicao,
                    b.vale_alimentacao       AS ValeAlimentacao,
                    b.assistencia_medica     AS AssistenciaMedica,
                    b.ajuda_de_custo         AS AjudaDeCusto,
                    b.mobilidade,
                    b.assistencia_educacional AS AssistenciaEducacional,
                    b.remuneracao_total      AS RemuneracaoTotal
                FROM tb_beneficios_template_pdf b
                JOIN tb_template_pdf_admissao t
                  ON t.id = b.tb_template_pdf_admissao_id
                JOIN tb_candidato_vaga cv
                  ON cv.id = t.tb_candidato_vaga_id
                WHERE t.tb_candidato_vaga_id = @idCandidatura
                  AND t.tb_vaga_id = @idVaga
                  AND cv.tb_colaborador_codigo_interno_colaborador = @codInternoCandidato
                LIMIT 1;
            """;

            var result = await conn.QueryFirstOrDefaultAsync<BeneficiosDTO>(sql, new { idCandidatura, idVaga, codInternoCandidato });

            return result ?? new BeneficiosDTO();
        }

        public async Task<ChecklistInstalacaoDTO> GetChecklistInstalacaoPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga)
        {
            var conn = _dapperConnection.GetConnection();

            const string sql = """
                SELECT  
                    c.nome_profissional AS NomeProfissional,
                    c.descricao_maquina AS DescricaoMaquina,
                    c.hardware,
                    c.outros_softwares  AS OutrosSoftwares
                FROM tb_checklist_instalacao_template_pdf c
                JOIN tb_template_pdf_admissao t
                  ON t.id = c.tb_template_pdf_admissao_id
                JOIN tb_candidato_vaga cv
                  ON cv.id = t.tb_candidato_vaga_id
                WHERE t.tb_candidato_vaga_id = @idCandidatura
                  AND t.tb_vaga_id = @idVaga
                  AND cv.tb_colaborador_codigo_interno_colaborador = @codInternoCandidato
                LIMIT 1;
            """;

            var result = await conn.QueryFirstOrDefaultAsync<ChecklistInstalacaoDTO>(sql, new { idCandidatura, idVaga, codInternoCandidato });

            return result ?? new ChecklistInstalacaoDTO();
        }
        public async Task<AcessosUsuarioDTO> GetAcessosUsuarioPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga)
        {
            var conn = _dapperConnection.GetConnection();

            // Cabeçalho dos acessos (valida os 3 parâmetros na mesma consulta)
            var head = await conn.QueryFirstOrDefaultAsync(
                """
                SELECT a.id, a.email_foursys, a.tipo_email, a.login_rede, a.observacao_acesso_usuario, tv.maquina 
                FROM tb_acessos_usuario_template_pdf a
                JOIN tb_profissional_template_pdf p
                  ON p.id = a.tb_profissional_template_pdf_id
                JOIN tb_template_pdf_admissao t
                  ON t.id = a.tb_template_pdf_admissao_id
                 AND t.id = p.tb_template_pdf_admissao_id
                JOIN tb_candidato_vaga cv
                  ON cv.id = t.tb_candidato_vaga_id
                JOIN tb_vaga tv
                  ON tv.id  = t.tb_vaga_id
                WHERE t.tb_candidato_vaga_id = @idCandidatura
                  AND t.tb_vaga_id = @idVaga
                  AND cv.tb_colaborador_codigo_interno_colaborador = @codInternoCandidato
                LIMIT 1;
                """, new { idCandidatura, idVaga, codInternoCandidato });

            if (head == null)
                return new AcessosUsuarioDTO();

            var dto = new AcessosUsuarioDTO
            {
                EmailFoursys = (string)head.email_foursys,
                TipoEmail = (string)head.tipo_email,
                LoginRede = (string)head.login_rede,
                ObservacaoAprovador = (string)head.observacao_acesso_usuario,
                Maquina = (string)head.maquina
            };

            // Grupos de e-mail
            dto.GruposEmail = await conn.QueryFirstOrDefaultAsync<AcessoGrupoEmailDTO>(
                """
                SELECT 
                    (todos_foursys     = 1) AS TodosFoursys,
                    (foursys_alphavile = 1) AS FoursysAlphaville,
                    (foursys_paulista  = 1) AS FoursysPaulista,
                    (foursys_curitiba  = 1) AS FoursysCuritiba,
                    grupo_email_contrato      AS GrupoEmailContrato,
                    descricao_outros_grupos   AS DescricaoOutrosGrupos,
                    observacao_aprovador      AS ObservacaoAprovador
                FROM tb_acessos_usuario_grupos_template_pdf
                WHERE tb_acessos_usuario_template_pdf_id = @acessosId
                LIMIT 1;
                """, new { acessosId = (string)head.id });

            // Sistemas liberados (flag na RELAÇÃO)
            var sistemas = await conn.QueryAsync<AcessoSistemaDTO>(
                """
                SELECT 
                       rel.tb_sistemas_liberados_template_pdf_id AS Id,
                       cat.nome_sistema                           AS NomeSistema,
                       (rel.liberado = 1)                         AS Liberado
                   FROM tb_sistemas_liberados_acessos_usuario_template_pdf rel
                   JOIN tb_sistemas_liberados_template_pdf cat
                     ON cat.id = rel.tb_sistemas_liberados_template_pdf_id
                   WHERE rel.tb_acessos_usuario_template_pdf_id = @acessosId;
                """, new { acessosId = (string)head.id });
            dto.SistemasLiberados = sistemas.ToList();

            // Diretórios de rede (leitura/escrita vêm do “catálogo”)
            var dirs = await conn.QueryAsync<AcessoDiretorioRedeDTO>(
                """
                SELECT 
                    cat.id                   AS Id,
                    cat.diretorio            AS Diretorio,
                    (rel.leitura = 1)        AS Leitura,
                    (rel.escrita = 1)        AS Escrita
                FROM tb_acessos_pasta_rede_template_pdf_rel rel
                JOIN tb_acessos_pasta_rede_template_pdf cat
                  ON cat.id = rel.tb_acessos_pasta_rede_template_pdf_id
                WHERE rel.tb_acessos_usuario_template_pdf_id = @acessosId;
                """, new { acessosId = (string)head.id });
            dto.DiretoriosRede = dirs.ToList();

            return dto;
        }

        public async Task<PdfTemplateDTO> GetPdfTemplateDeAdmissaoPdf(string idCandidatura, string codInternoCandidato, string idVaga)
        {
            var header = await GetPdfTemplateHeaderPdfAdmissao(idCandidatura, codInternoCandidato, idVaga);
            if (header == null) return null;

            header.Profissional = await GetProfissionalPdfAdmissao(idCandidatura, codInternoCandidato, idVaga);
            header.VagaAdmissao = await GetVagaAdmissaoPdfAdmissao(idCandidatura, codInternoCandidato, idVaga);
            header.EequipamentosFoursys = await GetEquipamentoFourSysAdmissaoPdf(idCandidatura, codInternoCandidato, idVaga);
            header.Beneficios = await GetBeneficiosPdfAdmissao(idCandidatura, codInternoCandidato, idVaga);
            header.ChecklistInstalacao = await GetChecklistInstalacaoPdfAdmissao(idCandidatura, codInternoCandidato, idVaga);
            header.AcessosUsuario = await GetAcessosUsuarioPdfAdmissao(idCandidatura, codInternoCandidato, idVaga);

            return header;
        }

        public async Task<EquipamentosFoursysDTO> GetEquipamentoFourSysAdmissaoPdf(
            string idCandidatura,
            string codInternoCandidato,
            string idVaga)
        {
            var conn = _dapperConnection.GetConnection();

            const string sql = """
                                   SELECT
                                       (tef.celular = 1)                             AS Celular,
                                       (tef.plano_dados = 1)                         AS PlanoDados,
                                       CAST(tef.quantidade_minutos_plano_dados AS SIGNED) AS QuantidadeMinutosPlanoDados,
                                       (tef.cartao_visitas = 1)                      AS CartaoVisitas,
                                       CAST(tef.quantidade_cartao_visitas AS SIGNED) AS QuantidadeCartaoVisitas,
                                       tef.outros                                    AS Outros
                                   FROM tb_equipamentos_foursys_template_pdf tef
                                   JOIN tb_template_pdf_admissao t
                                     ON t.id = tef.tb_template_pdf_admissao_id
                                   JOIN tb_candidato_vaga cv
                                     ON cv.id = t.tb_candidato_vaga_id
                                   WHERE t.tb_candidato_vaga_id = @idCandidatura
                                     AND t.tb_vaga_id            = @idVaga
                                     AND cv.tb_colaborador_codigo_interno_colaborador = @codInternoCandidato
                                   LIMIT 1;
                               """;

            var result = await conn.QueryFirstOrDefaultAsync<EquipamentosFoursysDTO>(
                sql,
                new { idCandidatura, idVaga, codInternoCandidato });

            return result ?? new EquipamentosFoursysDTO();
        }

        public async Task<PdfTemplateDTO> CreatePdfTemplateDeAdmissao(CreatePdfTemplateParameters param)
        {

            PdfTemplateDTO retorno = new PdfTemplateDTO();

            if (param == null) throw new ArgumentNullException(nameof(param));
            if (string.IsNullOrWhiteSpace(param.IdCandidatura)) throw new ArgumentException("IdCandidatura é obrigatório.");
            if (string.IsNullOrWhiteSpace(param.CodInternoCandidato)) throw new ArgumentException("CodInternoCandidato é obrigatório.");
            if (string.IsNullOrWhiteSpace(param.IdVaga)) throw new ArgumentException("IdVaga é obrigatório.");

            var conn = _dapperConnection.GetConnection();
            using var tx = conn.BeginTransaction();

            var templateIdsExistentes = await conn.QueryAsync<string>(
                """
                SELECT t.id
                  FROM tb_template_pdf_admissao t
                  JOIN tb_candidato_vaga cv
                    ON cv.id = t.tb_candidato_vaga_id
                 WHERE t.tb_candidato_vaga_id = @idCandidatura
                   AND t.tb_vaga_id            = @idVaga
                   AND cv.tb_colaborador_codigo_interno_colaborador = @codInternoCandidato
                 FOR UPDATE;
                """,
                new { idCandidatura = param.IdCandidatura, idVaga = param.IdVaga, codInternoCandidato = param.CodInternoCandidato },
                tx
            );

            if (templateIdsExistentes.Any())
            {
                await conn.ExecuteAsync(
                    "DELETE FROM tb_template_pdf_admissao WHERE id IN @ids;",
                    new { ids = templateIdsExistentes.ToArray() },
                    tx
                );
            }

            // Valida se o email e login ja existem
            var emailJaCadastrado = await conn.QueryFirstOrDefaultAsync<bool>(
                """
                    SELECT EXISTS(
                        SELECT 1
                        FROM tb_usuario tu
                        WHERE tu.email = @EmailFoursys
                    ) AS ExisteEmail;
                    """,
                new { param.AcessosUsuario.EmailFoursys }, tx);

            // Se já existir, interrompe a execução e retorna erro
            if (emailJaCadastrado)
                throw new InvalidOperationException("Email já cadastrado na base de dados.");

            var vagaInfo = await conn.QueryFirstOrDefaultAsync(
                """
                  SELECT  
                   tv.id AS IdVaga,
                   tv.codigo AS NumeroVaga,
                   tv.titulo AS TituloVaga,
                   tv.tb_gestor_cod AS CodGestor,
                   tge.codigo_cliente AS CodigoCliente,
                   tco.nome_cliente AS NomeCliente,
                   tv.cargo AS Cargo,
                   tbtv.descricao AS TipoVaga,
                   tv.estado,
                   tv.cidade,
                   tbgep.cep  AS Cep,
                   tco2.diretoria,
                   tc.nome_completo AS Solicitante,
                   tcr.nome_completo AS AnalistaResponsavel,
                   tge.nome AS Gestor,
                   tbpl.id as Localizacao,
                   tbu.descricao AS UnidadeTrabalho,    
                   tbvmt.descricao AS ModalidadeTrabalho
                 FROM tb_vaga tv 
                 LEFT JOIN tb_candidato_vaga tcv 
                  ON tcv.tb_vaga_id = tv.id 
                 LEFT JOIN tb_gestor_externo tge 
                  ON tge.cod_gestor_externo = tv.tb_gestor_cod  
                 LEFT JOIN tb_cliente_org tco 
                  ON tco.codigo_cliente  = tge.codigo_cliente
                 LEFT JOIN tb_colaborador_org tco2
                  ON tco2.tb_org_id  = tv.tb_org_id AND tco2.codigo_interno_colaborador = @codInternoColaboradorSolicitante
                 LEFT JOIN tb_tipo_vaga tbtv
                  ON tv.tb_tipo_vaga_id = tbtv.id
                 LEFT JOIN tb_colaborador tc 
                  ON tc.codigo_interno_colaborador = tv.tb_usuario_criador_cpf
                 LEFT JOIN tb_colaborador tcr
                  ON tcr.codigo_interno_colaborador = tv.tb_usuario_aprovador_cpf    
                 LEFT JOIN tb_profissional_localidade tbpl
                  ON tv.localizacao  = tbpl.id
                 LEFT JOIN tb_modelo_trabalho tbvmt
                  ON tbvmt.id = tcv.tb_modelo_trabalho_id
                 LEFT JOIN tb_unidade tbu
                  ON tv.tb_unidade_id = tbu.id
                 LEFT JOIN tb_gestor_externo_perfil tbgep
                  ON tbgep.id = tv.tb_gestor_externo_perfil_id
                 WHERE tv.id = @idVaga
                  AND tcv.id = @idCandidatura
                 """,
                new { idVaga = param.IdVaga, idCandidatura = param.IdCandidatura, codInternoColaboradorSolicitante = param.CodInternoColaboradorSolicitante }, tx);



            if (vagaInfo == null)
                throw new InvalidOperationException("Vaga não encontrada para o Id informado.");


            var templateId = Guid.NewGuid().ToString();
            await conn.ExecuteAsync(
                """
                 INSERT INTO tb_template_pdf_admissao
                     (id, tb_candidato_vaga_id, tb_vaga_id, tb_cliente_org_codigo_cliente)
                 VALUES
                     (@id, @candidaturaId, @vagaId, @codigoCliente);
                 """,
                new
                {
                    id = templateId,
                    candidaturaId = param.IdCandidatura,
                    vagaId = param.IdVaga,
                    codigoCliente = (string)vagaInfo.CodigoCliente
                }, tx);


            string equipamentosId = null;
            if (param.EquipamentosFoursys != null)
            {
                equipamentosId = Guid.NewGuid().ToString();
                await conn.ExecuteAsync(
                    """
                     INSERT INTO tb_equipamentos_foursys_template_pdf
                         (id, tb_template_pdf_admissao_id, celular, plano_dados, quantidade_minutos_plano_dados, 
                          cartao_visitas, quantidade_cartao_visitas, outros)
                     VALUES
                         (@id, @templateId, @celular, @plano, @minutos, @cartao, @qtdCartao, @outros);
                     """,
                    new
                    {
                        id = equipamentosId,
                        templateId,
                        celular = param.EquipamentosFoursys.Celular ? 1 : 0,
                        plano = param.EquipamentosFoursys.PlanoDados ? 1 : 0,
                        minutos = param.EquipamentosFoursys.QuantidadeMinutosPlanoDados,
                        cartao = param.EquipamentosFoursys.CartaoVisitas ? 1 : 0,
                        qtdCartao = param.EquipamentosFoursys.QuantidadeCartaoVisitas,
                        outros = param.EquipamentosFoursys.Outros
                    }, tx);
            }

            if (param.Profissional == null)
                throw new ArgumentException("Profissional é obrigatório.");

            var profissionalId = Guid.NewGuid().ToString();
            await conn.ExecuteAsync(
                """
                 INSERT INTO tb_profissional_template_pdf
                     (id, tb_template_pdf_admissao_id, tb_equipamentos_foursys_template_pdf_id,
                      nome, rg, cpf, data_nascimento, telefone, email_pessoal, tamanho_camiseta, superior_imediato, endereco)
                 VALUES
                     (@id, @templateId, @equipId, @nome, @rg, @cpf, @nasc, @tel, @email, @camiseta, @sup, @endereco);
                 """,
                new
                {
                    id = profissionalId,
                    templateId,
                    equipId = equipamentosId,
                    nome = param.Profissional.Nome,
                    rg = param.Profissional.RG,
                    cpf = param.Profissional.CPF,
                    nasc = param.Profissional.DataNascimento,
                    tel = param.Profissional.Telefone,
                    email = param.Profissional.EmailPessoal,
                    camiseta = param.Profissional.TamanhoCamiseta,
                    sup = param.Profissional.SuperiorImediato,
                    endereco = param.Profissional.Endereco?.Endereco
                }, tx);

            // Atualiza o telefone e endereço do colaborador na tabela tb_colaborador
            long? enderecoId = null;

            // Insere o endereço na tabela tb_endereco se fornecido
            if (param.Profissional.Endereco != null)
            {
                enderecoId = await conn.QueryFirstOrDefaultAsync<long>(
                    """
                     INSERT INTO tb_endereco 
                         (cep, endereco, numero, complemento, bairro, cidade, estado, 
                          com_quem_mora, internacional_linha_um, internacional_linha_dois,
                          ativo, data_criacao, data_alteracao)
                     VALUES 
                         (@cep, @endereco, @numero, @complemento, @bairro, @cidade, @estado,
                          @comQuemMora, @internacionalLinhaUm, @internacionalLinhaDois,
                          1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);
                     SELECT LAST_INSERT_ID();
                     """,
                    new
                    {
                        cep = param.Profissional.Endereco.Cep,
                        endereco = param.Profissional.Endereco.Endereco,
                        numero = param.Profissional.Endereco.Numero,
                        complemento = param.Profissional.Endereco.Complemento,
                        bairro = param.Profissional.Endereco.Bairro,
                        cidade = param.Profissional.Endereco.Cidade,
                        estado = param.Profissional.Endereco.Estado,
                        comQuemMora = param.Profissional.Endereco.ComQuemMora,
                        internacionalLinhaUm = param.Profissional.Endereco.InternacionalLinhaUm,
                        internacionalLinhaDois = param.Profissional.Endereco.InternacionalLinhaDois
                    }, tx);
            }

            // Atualiza o colaborador com telefone e endereço
            var updateFields = new List<string>();
            var parameters = new Dictionary<string, object> { { "codInternoCandidato", param.CodInternoCandidato } };

            if (!string.IsNullOrWhiteSpace(param.Profissional.Telefone))
            {
                updateFields.Add("contato_principal = @telefone");
                parameters.Add("telefone", param.Profissional.Telefone);
            }

            if (enderecoId.HasValue)
            {
                updateFields.Add("endereco_id = @enderecoId");
                parameters.Add("enderecoId", enderecoId.Value);
            }

            if (updateFields.Any())
            {
                updateFields.Add("data_alteracao = CURRENT_TIMESTAMP");

                var updateSql = $"""
                    UPDATE tb_colaborador 
                    SET {string.Join(", ", updateFields)}
                    WHERE codigo_interno_colaborador = @codInternoCandidato;
                    """;

                await conn.ExecuteAsync(updateSql, parameters, tx);
            }


            if (param.Beneficios != null)
            {
                var beneficiosId = Guid.NewGuid().ToString();
                await conn.ExecuteAsync(
                    """
                     INSERT INTO tb_beneficios_template_pdf
                         (id, tb_template_pdf_admissao_id, salario, vale_refeicao, vale_alimentacao,
                          assistencia_medica, ajuda_de_custo, mobilidade, assistencia_educacional, remuneracao_total, custo_hora)
                     VALUES
                         (@id, @templateId, @salario, @vr, @va, @medica, @ajuda, @mobilidade, @educ, @total, @CustoHora);
                     """,
                    new
                    {
                        id = beneficiosId,
                        templateId,
                        salario = param.Beneficios.Salario,
                        vr = param.Beneficios.ValeRefeicao,
                        va = param.Beneficios.ValeAlimentacao,
                        medica = param.Beneficios.AssistenciaMedica,
                        ajuda = param.Beneficios.AjudaDeCusto,
                        mobilidade = param.Beneficios.Mobilidade,
                        educ = param.Beneficios.AssistenciaEducacional,
                        total = param.Beneficios.RemuneracaoTotal,
                        param.Beneficios.CustoHora
                    }, tx);
            }
            if (param.ChecklistInstalacao != null)
            {
                var checklistId = Guid.NewGuid().ToString();
                await conn.ExecuteAsync(
                    """
                     INSERT INTO tb_checklist_instalacao_template_pdf
                         (id, tb_template_pdf_admissao_id, nome_profissional, descricao_maquina, hardware, outros_softwares)
                     VALUES
                         (@id, @templateId, @nome, @desc, @hw, @soft);
                     """,
                    new
                    {
                        id = checklistId,
                        templateId,
                        nome = param.ChecklistInstalacao.NomeProfissional,
                        desc = param.ChecklistInstalacao.DescricaoMaquina,
                        hw = param.ChecklistInstalacao.Hardware,
                        soft = param.ChecklistInstalacao.OutrosSoftwares
                    }, tx);
            }


            string acessosUsuarioId = null;
            if (param.AcessosUsuario != null)
            {
                acessosUsuarioId = Guid.NewGuid().ToString();

                await conn.ExecuteAsync(
                    """
                     INSERT INTO tb_acessos_usuario_template_pdf
                         (id, tb_template_pdf_admissao_id, tb_profissional_template_pdf_id,
                          email_foursys, tipo_email, login_rede, observacao_acesso_usuario, maquina)
                     VALUES
                         (@id, @templateId, @profId, @email, @tipo, @login, @obs, @Maquina);
                     """,
                    new
                    {
                        id = acessosUsuarioId,
                        templateId,
                        profId = profissionalId,
                        email = param.AcessosUsuario.EmailFoursys,
                        tipo = param.AcessosUsuario.TipoEmail,
                        login = param.AcessosUsuario.LoginRede,
                        obs = param.AcessosUsuario.ObservacaoAprovador,
                        param.AcessosUsuario.Maquina
                    }, tx);

                await conn.ExecuteAsync(
                    """
                         UPDATE tb_vaga
                         SET 
                             maquina = @Maquina
                         WHERE 
                             id = @IdVaga;
                     """,
                    new
                    {
                        param.AcessosUsuario.Maquina,
                        param.IdVaga
                    }, tx);

                if (param.AcessosUsuario.GruposEmail != null)
                {
                    await conn.ExecuteAsync(
                        """
                         INSERT INTO tb_acessos_usuario_grupos_template_pdf
                             (id, tb_acessos_usuario_template_pdf_id, tb_template_pdf_admissao_id,
                              todos_foursys, foursys_alphavile, foursys_paulista, foursys_curitiba,
                              grupo_email_contrato, descricao_outros_grupos, observacao_aprovador, maquina)
                         VALUES
                             (@id, @acessosId, @templateId, @todos, @alpha, @paul, @cwb, @contrato, @outros, @obs, @Maquina);
                         """,
                        new
                        {
                            id = Guid.NewGuid().ToString(),
                            acessosId = acessosUsuarioId,
                            templateId,
                            todos = param.AcessosUsuario.GruposEmail.TodosFoursys ? 1 : 0,
                            alpha = param.AcessosUsuario.GruposEmail.FoursysAlphaville ? 1 : 0,
                            paul = param.AcessosUsuario.GruposEmail.FoursysPaulista ? 1 : 0,
                            cwb = param.AcessosUsuario.GruposEmail.FoursysCuritiba ? 1 : 0,
                            contrato = param.AcessosUsuario.GruposEmail.GrupoEmailContrato,
                            outros = param.AcessosUsuario.GruposEmail.DescricaoOutrosGrupos,
                            obs = param.AcessosUsuario.GruposEmail.ObservacaoAprovador,
                            param.AcessosUsuario.Maquina
                        }, tx);
                }

                if (param.AcessosUsuario.SistemasLiberados?.Count > 0)
                {
                    foreach (var sys in param.AcessosUsuario.SistemasLiberados)
                    {
                        // localizar no catálogo; se não existir, cria
                        var catId = await conn.QueryFirstOrDefaultAsync<string>(
                            "SELECT id FROM tb_sistemas_liberados_template_pdf WHERE nome_sistema = @nome LIMIT 1;",
                            new { nome = sys.NomeSistema }, tx);

                        if (string.IsNullOrEmpty(catId))
                        {
                            catId = Guid.NewGuid().ToString();
                            await conn.ExecuteAsync(
                                "INSERT INTO tb_sistemas_liberados_template_pdf (id, nome_sistema) VALUES (@id, @nome);",
                                new { id = catId, nome = sys.NomeSistema }, tx);
                        }

                        await conn.ExecuteAsync(
                            """
                            INSERT INTO tb_sistemas_liberados_acessos_usuario_template_pdf
                                (id, tb_acessos_usuario_template_pdf_id, tb_sistemas_liberados_template_pdf_id, liberado)
                            VALUES
                                (@id, @acessosId, @sysId, @lib)
                            ON DUPLICATE KEY UPDATE liberado = VALUES(liberado);
                            """,
                            new
                            {
                                id = Guid.NewGuid().ToString(),
                                acessosId = acessosUsuarioId,
                                sysId = catId,
                                lib = sys.Liberado ? 1 : 0
                            }, tx);
                    }
                }

                if (param.AcessosUsuario.DiretoriosRede?.Count > 0)
                {
                    foreach (var dir in param.AcessosUsuario.DiretoriosRede)
                    {
                        var dirCatId = await conn.QueryFirstOrDefaultAsync<string>(
                            "SELECT id FROM tb_acessos_pasta_rede_template_pdf WHERE diretorio = @d LIMIT 1;",
                            new { d = dir.Diretorio }, tx);

                        if (string.IsNullOrEmpty(dirCatId))
                        {
                            dirCatId = Guid.NewGuid().ToString();
                            await conn.ExecuteAsync(
                                "INSERT INTO tb_acessos_pasta_rede_template_pdf (id, diretorio) VALUES (@id, @d);",
                                new { id = dirCatId, d = dir.Diretorio }, tx);
                        }
                        await conn.ExecuteAsync(
                            """
                            INSERT INTO tb_acessos_pasta_rede_template_pdf_rel
                                (id, tb_acessos_usuario_template_pdf_id, tb_acessos_pasta_rede_template_pdf_id, leitura, escrita)
                            VALUES
                                (@id, @acessosId, @dirId, @l, @e)
                            ON DUPLICATE KEY UPDATE leitura = VALUES(leitura), escrita = VALUES(escrita);
                            """,
                            new
                            {
                                id = Guid.NewGuid().ToString(),
                                acessosId = acessosUsuarioId,
                                dirId = dirCatId,
                                l = dir.Leitura ? 1 : 0,
                                e = dir.Escrita ? 1 : 0
                            }, tx);
                    }
                }

                await conn.ExecuteAsync(
                                            """
                                            INSERT INTO tb_vaga_admissao_template_pdf
                                            (
                                                id,
                                                tb_template_pdf_admissao_id,
                                                id_vaga,
                                                cliente,
                                                cargo,
                                                stack_principal,
                                                nivel_cargo,
                                                tipo_vaga,
                                                endereco,
                                                cep,
                                                data_inicio,
                                                diretoria,
                                                solicitante,
                                                gestor_responsavel,
                                                analista_responsavel,
                                                unidade_trabalho,
                                                local_alocacao,
                                                area_staff,
                                                horario_trabalho,
                                                horas_fechadas
                                            )
                                            VALUES
                                            (
                                                @id,
                                                @templateId,
                                                @idVaga,
                                                @cliente,
                                                @cargo,
                                                NULL,             
                                                NULL,      
                                                @tipo,
                                                @endereco,
                                                @cep,         
                                                @dataInicio,
                                                @diretoria,
                                                @solicitante,
                                                @gestor,
                                                @analistaResponsavel,
                                                @unidadeTrabalho,
                                                @local,
                                                @areaStaff,
                                                @horario,
                                                @horasFechadas
                                            );
                                            """,
                                            new
                                            {
                                                id = Guid.NewGuid().ToString(),
                                                templateId,
                                                idVaga = param.IdVaga,
                                                cliente = vagaInfo.CodigoCliente,
                                                cargo = vagaInfo.Cargo,
                                                tipo = vagaInfo.TipoVaga,
                                                endereco = $"{vagaInfo.cidade} {vagaInfo.estado}",
                                                cep = vagaInfo.Cep,
                                                dataInicio = param.DataInicio,
                                                diretoria = vagaInfo.diretoria,
                                                solicitante = vagaInfo.Solicitante,
                                                gestor = vagaInfo.Gestor,
                                                analistaResponsavel = vagaInfo.AnalistaResponsavel,
                                                unidadeTrabalho = vagaInfo.UnidadeTrabalho,
                                                local = vagaInfo.Localizacao,
                                                areaStaff = param.AreaStaff,
                                                horario = param.HorarioDeTrabalho,
                                                horasFechadas = param.HorasFechadas
                                            },
                                            tx);

            }

            tx.Commit();

            var pdf = new PdfTemplateDTO
            {
                Id = templateId,
                NumeroVaga = (long)vagaInfo.NumeroVaga,
                TituloVaga = (string)vagaInfo.TituloVaga,
                Cliente = (string)vagaInfo.NomeCliente
            };

            pdf.VagaAdmissao = await conn.QueryFirstOrDefaultAsync<VagaAdmissaoDTO>(
                 """
                     SELECT  tvatp.id_vaga AS IdVaga,
                     		tvatp.cliente AS Cliente,
                     		tvatp.cargo AS Cargo,
                     		tvatp.tipo_vaga AS TipoVaga,
                     		tvatp.endereco AS Endereco,
                     		tvatp.cep AS CEP,
                     		tvatp.data_inicio AS DataInicio,
                     		tvatp.diretoria AS Diretoria,
                     		tvatp.solicitante AS Solicitante,
                     		tvatp.gestor_responsavel AS GestorResponsavel,
                     		tvatp.analista_responsavel AS AnalistaResponsavel,
                     		tvatp.unidade_trabalho AS UnidadeTrabalho,
                     		tbpl.descricao  AS LocalAlocacao,
                     		tvatp.area_staff AS AreaStaff,
                     		tvatp.horario_trabalho AS HorarioTrabalho,
                     		tvatp.horas_fechadas AS HorasFechadas
                     FROM tb_vaga_admissao_template_pdf tvatp
                     LEFT JOIN tb_profissional_localidade tbpl
                      ON tvatp.local_alocacao  = tbpl.id
                     WHERE tb_template_pdf_admissao_id = @templateId;
                     """, new { templateId });

            pdf.VagaAdmissao.ModalidadeTrabalho = vagaInfo.ModalidadeTrabalho;

            var profissionalResult = await conn.QueryFirstOrDefaultAsync(
                     """
                     SELECT  
                         p.nome,
                         p.rg,
                         p.cpf,
                         p.data_nascimento AS DataNascimento,
                         c.contato_principal AS telefone,
                         p.email_pessoal AS EmailPessoal,
                         p.tamanho_camiseta AS TamanhoCamiseta,
                         p.superior_imediato AS SuperiorImediato,
                         e.cep,
                         e.endereco,
                         e.numero,
                         e.complemento,
                         e.bairro,
                         e.cidade,
                         e.estado,
                         e.com_quem_mora,
                         e.internacional_linha_um,
                         e.internacional_linha_dois
                     FROM tb_profissional_template_pdf p
                     JOIN tb_template_pdf_admissao t
                       ON t.id = p.tb_template_pdf_admissao_id
                     JOIN tb_candidato_vaga cv
                       ON cv.id = t.tb_candidato_vaga_id
                     JOIN tb_colaborador c
                       ON c.codigo_interno_colaborador = cv.tb_colaborador_codigo_interno_colaborador
                     LEFT JOIN tb_endereco e
                       ON e.id = c.endereco_id
                     WHERE p.tb_template_pdf_admissao_id = @templateId
                     LIMIT 1;
                     """, new { templateId });

            if (profissionalResult != null)
            {
                pdf.Profissional = new ProfissionalDTO
                {
                    Nome = profissionalResult.nome,
                    RG = profissionalResult.rg,
                    CPF = profissionalResult.cpf,
                    DataNascimento = profissionalResult.DataNascimento,
                    Telefone = profissionalResult.telefone,
                    EmailPessoal = profissionalResult.EmailPessoal,
                    TamanhoCamiseta = profissionalResult.TamanhoCamiseta,
                    SuperiorImediato = profissionalResult.SuperiorImediato
                };

                // Constrói o EnderecoDTO se houver dados de endereço
                if (!string.IsNullOrWhiteSpace(profissionalResult.endereco))
                {
                    pdf.Profissional.Endereco = new DataTransferObject.Domain.Endereco.EnderecoDTO
                    {
                        Cep = profissionalResult.cep,
                        Endereco = profissionalResult.endereco,
                        Numero = profissionalResult.numero,
                        Complemento = profissionalResult.complemento,
                        Bairro = profissionalResult.bairro,
                        Cidade = profissionalResult.cidade,
                        Estado = profissionalResult.estado,
                        ComQuemMora = profissionalResult.com_quem_mora,
                        InternacionalLinhaUm = profissionalResult.internacional_linha_um,
                        InternacionalLinhaDois = profissionalResult.internacional_linha_dois
                    };
                }
            }

            pdf.EequipamentosFoursys = await conn.QueryFirstOrDefaultAsync<EquipamentosFoursysDTO>(
                """
                 
                            SELECT
                                tb_template_pdf_admissao_id,
                                (tef.celular = 1)                             AS Celular,
                            (tef.plano_dados = 1)                         AS PlanoDados,
                                CAST(tef.quantidade_minutos_plano_dados AS SIGNED) AS QuantidadeMinutosPlanoDados,
                            (tef.cartao_visitas = 1)                      AS CartaoVisitas,
                                CAST(tef.quantidade_cartao_visitas AS SIGNED) AS QuantidadeCartaoVisitas,
                                tef.outros                                    AS Outros
                            FROM tb_equipamentos_foursys_template_pdf tef
                            WHERE tb_template_pdf_admissao_id = @templateId
                            LIMIT 1;            
                """, new { templateId });

            pdf.Beneficios = await conn.QueryFirstOrDefaultAsync<BeneficiosDTO>(
                """
                     SELECT  
                         salario,
                         vale_refeicao AS ValeRefeicao,
                         vale_alimentacao AS ValeAlimentacao,
                         assistencia_medica AS AssistenciaMedica,
                         ajuda_de_custo AS AjudaDeCusto,
                         mobilidade,
                         assistencia_educacional AS AssistenciaEducacional,
                         remuneracao_total AS RemuneracaoTotal,
                         custo_hora AS CustoHora
                     FROM tb_beneficios_template_pdf
                     WHERE tb_template_pdf_admissao_id = @templateId
                     LIMIT 1;
                     """, new { templateId });

            pdf.ChecklistInstalacao = await conn.QueryFirstOrDefaultAsync<ChecklistInstalacaoDTO>(
                """
                     SELECT  
                         nome_profissional AS NomeProfissional,
                         descricao_maquina AS DescricaoMaquina,
                         hardware,
                         outros_softwares AS OutrosSoftwares
                     FROM tb_checklist_instalacao_template_pdf
                     WHERE tb_template_pdf_admissao_id = @templateId
                     LIMIT 1;
                     """, new { templateId });

            var acessosHead = await conn.QueryFirstOrDefaultAsync(
                """
                     SELECT tautp.id, tautp.email_foursys, tautp.tipo_email, tautp.login_rede, tautp.observacao_acesso_usuario, tv.maquina
                     FROM tb_acessos_usuario_template_pdf tautp
                     INNER JOIN tb_template_pdf_admissao ttpa
                     	ON ttpa.id = tautp.tb_template_pdf_admissao_id 
                     INNER JOIN tb_vaga tv
                     	ON tv.id = ttpa.tb_vaga_id
                     WHERE tb_template_pdf_admissao_id = @templateId
                     LIMIT 1;
                     """, new { templateId });


            if (acessosHead != null)
            {
                var acessosDto = new AcessosUsuarioDTO
                {
                    NomeProfissional = pdf.Profissional?.Nome,
                    EmailFoursys = (string)acessosHead.email_foursys,
                    TipoEmail = (string)acessosHead.tipo_email,
                    LoginRede = (string)acessosHead.login_rede,
                    ObservacaoAprovador = (string)acessosHead.observacao_acesso_usuario,
                    Maquina = (string)acessosHead.maquina
                };

                acessosDto.GruposEmail = await conn.QueryFirstOrDefaultAsync<AcessoGrupoEmailDTO>(
                    """
                                SELECT 
                                    todos_foursys AS TodosFoursys,
                                    foursys_alphavile AS FoursysAlphaville,
                                    foursys_paulista AS FoursysPaulista,
                                    foursys_curitiba AS FoursysCuritiba,
                                    grupo_email_contrato AS GrupoEmailContrato,
                                    descricao_outros_grupos AS DescricaoOutrosGrupos,
                                    observacao_aprovador AS ObservacaoAprovador,
                                    maquina AS Maquina
                                FROM tb_acessos_usuario_grupos_template_pdf
                                WHERE tb_acessos_usuario_template_pdf_id = @acessosId
                                LIMIT 1;
                                """,
                    new { acessosId = (string)acessosHead.id });

                var sistemas = await conn.QueryAsync<AcessoSistemaDTO>(
                    """
                                SELECT 
                                    rel.tb_sistemas_liberados_template_pdf_id AS Id,
                                    cat.nome_sistema AS NomeSistema,
                                    rel.liberado AS Liberado
                                FROM tb_sistemas_liberados_acessos_usuario_template_pdf rel
                                INNER JOIN tb_sistemas_liberados_template_pdf cat
                                    ON cat.id = rel.tb_sistemas_liberados_template_pdf_id
                                WHERE rel.tb_acessos_usuario_template_pdf_id = @acessosId;
                                """, new { acessosId = (string)acessosHead.id });
                acessosDto.SistemasLiberados = sistemas.ToList();

                var dirs = await conn.QueryAsync<AcessoDiretorioRedeDTO>(
                    """
                                SELECT 
                                    cat.id AS Id,
                                    cat.diretorio AS Diretorio,
                                    rel.leitura AS Leitura,
                                    rel.escrita AS Escrita
                                FROM tb_acessos_pasta_rede_template_pdf_rel rel
                                INNER JOIN tb_acessos_pasta_rede_template_pdf cat
                                    ON cat.id = rel.tb_acessos_pasta_rede_template_pdf_id
                                WHERE rel.tb_acessos_usuario_template_pdf_id = @acessosId;
                                """, new { acessosId = (string)acessosHead.id });
                acessosDto.DiretoriosRede = dirs.ToList();

                pdf.AcessosUsuario = acessosDto;
            }

            retorno = pdf;


            return retorno;
        }
        public async Task<List<AcessoSistemaDTO>> GetSistemasLiberadosTemplatePdf()
        {
            var conn = _dapperConnection.GetConnection();

            var sistemas = await conn.QueryAsync<AcessoSistemaDTO>(
                """
                SELECT 
                    id AS Id,
                    nome_sistema AS NomeSistema
                FROM tb_sistemas_liberados_template_pdf
                ORDER BY nome_sistema;
                """);

            return sistemas.ToList();
        }

        public async Task<List<AcessoDiretorioRedeDTO>> GetAcessosPastaRedeTemplatePdf()
        {
            var conn = _dapperConnection.GetConnection();

            var dirs = await conn.QueryAsync<AcessoDiretorioRedeDTO>(
                """
                SELECT 
                    id AS Id,
                    diretorio AS Diretorio
                FROM tb_acessos_pasta_rede_template_pdf
                ORDER BY diretorio;
                """);

            return dirs.ToList();
        }

        public async Task<List<MotivoDeclinioDTO>> ListarMotivosDeclinio()
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT
                    id AS Id,
                    descricao AS Descricao,
                    ativo AS Ativo,
                    data_criacao AS DataCriacao,
                    data_alteracao AS DataAlteracao
                FROM
                    tb_motivo_declinio
                WHERE
                    ativo = 1
                ORDER BY
                    descricao;
            ";

            var result = await connection.QueryAsync<MotivoDeclinioDTO>(query);
            return result.ToList();
        }

        public async Task<List<MotivoReprovacaoDTO>> ListarMotivosReprovacao()
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT
                    id AS Id,
                    descricao AS Descricao,
                    ativo AS Ativo,
                    data_criacao AS DataCriacao,
                    data_alteracao AS DataAlteracao
                FROM
                    tb_motivo_reprovacao
                WHERE
                    ativo = 1
                ORDER BY
                    descricao;
            ";

            var result = await connection.QueryAsync<MotivoReprovacaoDTO>(query);
            return result.ToList();
        }

        public async Task<MotivoDeclinioDTO> ObterMotivoDeclinioPorId(string id)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);
            await connection.OpenAsync();

            try
            {
                var query = @"
                    SELECT
                        id AS Id,
                        descricao AS Descricao,
                        ativo AS Ativo,
                        data_criacao AS DataCriacao,
                        data_alteracao AS DataAlteracao
                    FROM
                        tb_motivo_declinio
                    WHERE
                        id = @id
                        AND ativo = 1;
                ";

                var parametros = new { id };

                var result = await connection.QueryFirstOrDefaultAsync<MotivoDeclinioDTO>(query, parametros);
                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ObterMotivoDeclinioPorId - CandidaturaRepositoy - {ex.Message}", DataTransferObject.Domain.Log.LevelsEnum.Error);
                return null;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task<MotivoReprovacaoDTO> ObterMotivoReprovacaoPorId(string id)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);
            await connection.OpenAsync();

            try
            {
                var query = @"
                    SELECT
                        id AS Id,
                        descricao AS Descricao,
                        ativo AS Ativo,
                        data_criacao AS DataCriacao,
                        data_alteracao AS DataAlteracao
                    FROM
                        tb_motivo_reprovacao
                    WHERE
                        id = @id
                        AND ativo = 1;
                ";

                var parametros = new { id };

                var result = await connection.QueryFirstOrDefaultAsync<MotivoReprovacaoDTO>(query, parametros);
                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ObterMotivoReprovacaoPorId - CandidaturaRepositoy - {ex.Message}", DataTransferObject.Domain.Log.LevelsEnum.Error);
                return null;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task DeclinarCandidato(string candidaturaId, string idMotivoDeclinio)
        {
            var connection = _dapperConnection.GetConnection();

            var updateQuery = @"
                UPDATE tb_candidato_vaga 
                SET
                    tb_motivo_declinio_id = @idMotivoDeclinio,
                    data_ultima_alteracao = NOW()
                WHERE id = @candidaturaId;
            ";

            await connection.ExecuteAsync(updateQuery, new { candidaturaId, idMotivoDeclinio });
        }

        public async Task ReprovarCandidato(string candidaturaId, string idMotivoReprovacao)
        {
            var connection = _dapperConnection.GetConnection();

            var updateQuery = @"
                UPDATE tb_candidato_vaga 
                SET
                    tb_motivo_reprovacao_id = @idMotivoReprovacao,
                    data_ultima_alteracao = NOW()
                WHERE id = @candidaturaId;
            ";

            await connection.ExecuteAsync(updateQuery, new { candidaturaId, idMotivoReprovacao });
        }

        public async Task<IEnumerable<TemplateDestinatarioEmail>> BuscaDestinatariosTemplateCandidatos(int orgId, bool anexo)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"SELECT 
                            email AS Email, 
                            assunto AS Assunto, 
                            area AS Area 
			       FROM  tb_destinatarios_email_candidatos 
			       WHERE ativo = 1 
                     AND anexo = @Anexo
                     AND tb_org_id = @OrgId;";

            var parameters = new { Anexo = anexo ? 1 : 0, OrgId = orgId };

            return await connection.QueryAsync<TemplateDestinatarioEmail>(query, parameters);
        }

        public async Task ExcluirCandidatura(string codColaboradorContratado, string idVaga, string codColaboradorLogado)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);
            await connection.OpenAsync();

            // Iniciar transação
            using var transaction = connection.BeginTransaction();

            try
            {
                // Obter os dados da candidatura para o log ANTES de atualizar
                var queryCandidatura = @"
                    SELECT 
                         tcv.id AS Id,
                         tcv.tb_colaborador_codigo_interno_colaborador AS CodColaborador,
                         tcv.tb_vaga_id AS IdVaga,
                         tcv.tb_org_id AS OrgId,
                         tcv.ativo AS Ativa,
                         tv.titulo AS TituloVaga,    
                         tcv.data_criacao AS Candidatura,        
                         tcv.data_ultima_alteracao AS UltimaAlteracao,
                         tcv.tb_candidato_status_id AS StatusId,        
                         tcs.descricao AS StatusDescricao,
                         tcv.pretensao_salarial AS PretencaoSalarial,
                         tcv.tb_modelo_trabalho_id AS ModeloTrabalhoId,
                         tcv.tb_disponibilidade_entrevista_id AS DisponibilidadeEntrevistaId,
                         tcv.quantidade_dias_presencial AS QuantidadeDiasPresencial
                     FROM 
                         tb_vaga tv
                     INNER JOIN 
                         tb_candidato_vaga tcv ON tcv.tb_vaga_id = tv.id
                     INNER JOIN 
                        tb_candidato_status tcs ON tcs.id = tcv.tb_candidato_status_id AND tcs.origem = 'Fourmakers'
                     WHERE 
                        tcv.tb_vaga_id = @idVaga
                        AND tcv.tb_colaborador_codigo_interno_colaborador = @codColaboradorContratado
                     ORDER BY 
                         tv.data_criacao DESC
                     LIMIT 1;";

                var candidatura = await connection.QueryFirstOrDefaultAsync<CandidaturaRecrutamentoDTO>(queryCandidatura, new { codColaboradorContratado, idVaga }, transaction);

                if (candidatura == null)
                {
                    throw new Exception("Candidatura não encontrada.");
                }

                var query = @"
                    UPDATE tb_candidato_vaga
                    SET ativo = 0
                    WHERE
                        tb_colaborador_codigo_interno_colaborador = @codColaboradorContratado
                        AND tb_vaga_id = @idVaga;
                ";

                var parametros = new { codColaboradorContratado, idVaga };

                await connection.ExecuteAsync(query, parametros, transaction);

                // Gravar o log na tb_candidato_vaga_log
                var queryLog = @"
                    INSERT INTO tb_candidato_vaga_log (
                        id,
                        tb_candidato_vaga_id,
                        tb_candidato_status_id,
                        tb_colaborador_codigo_interno_colaborador,
                        data_alteracao,
                        objeto
                    ) VALUES (
                        @idLog,
                        @candidaturaId,
                        @status,
                        @codigoInternoColaborador,
                        NOW(),
                        @objeto
                    );";

                var idLog = Guid.NewGuid().ToString();

                var parametrosLog = new
                {
                    idLog,
                    candidaturaId = candidatura.Id,
                    status = candidatura.StatusId.ToInt(),
                    codigoInternoColaborador = codColaboradorLogado,
                    objeto = JsonSerializer.Serialize(candidatura)
                };

                await connection.ExecuteAsync(queryLog, parametrosLog, transaction);

                // Commit da transação
                transaction.Commit();
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ExcluirCandidatura - CandidaturaRepositoy - {ex.Message}", DataTransferObject.Domain.Log.LevelsEnum.Error);
                transaction.Rollback();
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task<CandidaturaRecrutamentoDTO> ObterCandidaturaPorIdVagaColaborador(string codColaboradorContratado, string idVaga)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                    SELECT 
                         tcv.id AS Id,
	                     tcv.tb_colaborador_codigo_interno_colaborador AS CodColaborador,
	                     tcv.tb_vaga_id AS IdVaga,
	                     tcv.tb_org_id AS OrgId,
	                     tcv.ativo AS Ativa,
                         tv.titulo AS TituloVaga,    
                         tcv.data_criacao AS Candidatura,        
                         tcv.data_ultima_alteracao AS UltimaAlteracao,
                         tcv.tb_candidato_status_id AS StatusId,        
                         tcs.descricao AS StatusDescricao,
                         tcv.pretensao_salarial AS PretencaoSalarial,
                         tcv.tb_modelo_trabalho_id AS ModeloTrabalhoId,
                         tcv.tb_disponibilidade_entrevista_id AS DisponibilidadeEntrevistaId,
                         tcv.quantidade_dias_presencial AS QuantidadeDiasPresencial
                     FROM 
                         tb_vaga tv
                     INNER JOIN 
                         tb_candidato_vaga tcv ON tcv.tb_vaga_id  = tv.id
                     INNER JOIN 
                        tb_candidato_status tcs ON tcs.id  = tcv.tb_candidato_status_id AND tcs.origem = 'Fourmakers'
                     WHERE 
                        tcv.tb_vaga_id  = @idVaga
                        AND tcv.tb_colaborador_codigo_interno_colaborador = @codColaboradorContratado
                     ORDER BY 
                         tv.data_criacao DESC;";

            var parameters = new { codColaboradorContratado, idVaga };

            return await connection.QueryFirstOrDefaultAsync<CandidaturaRecrutamentoDTO>(query, parameters);
        }

        public async Task<List<MeusTalentos>> ListarMeusTalentos(string codigoUsuarioLogado)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT 	tc.nome_completo AS NomeColaborador,
		                tv.codigo CodigoVaga,
		                tv.titulo NomeVaga,
		                tco.nome_cliente AS NomeCliente,
		                tcGestorExterno.nome_completo AS NomeGestor,
		                tsv.descricao AS StatusVaga,
		                tcs.descricao AS StatusMovimentacao,
		                tcvl.data_alteracao AS UltimaAlteracao,
		                tcvl.tb_colaborador_codigo_interno_colaborador,
		                tcColaboradorLog.nome_completo AS RecrutadorUltimaMovimentacao,
		                tc.codigo_interno_colaborador AS CodColaborador,
		                tcCriador.nome_completo AS CriadoPor
                FROM tb_candidato_vaga tcv 
                INNER JOIN tb_vaga tv 
	                ON tv.id = tcv.tb_vaga_id
                INNER JOIN tb_candidato_status tcs 
	                ON tcs.id = tcv.tb_candidato_status_id AND tcs.origem = ""Fourmakers""
                INNER JOIN tb_colaborador tc 
	                ON tc.codigo_interno_colaborador = tcv.tb_colaborador_codigo_interno_colaborador 
                INNER JOIN tb_gestor_externo tge
	                ON tge.cod_gestor_externo = tv.tb_gestor_cod  
                INNER JOIN tb_colaborador tcGestorExterno
	                ON tge.codigo_interno_colaborador = tcGestorExterno.codigo_interno_colaborador 
                LEFT JOIN tb_cliente_org tco 
	                ON tge.codigo_cliente = tco.codigo_cliente AND tco.tb_org_id = tge.tb_org_id
                INNER JOIN tb_status_vaga tsv 
	                ON tv.tb_status_vaga_cod = tsv.codigo
                LEFT JOIN tb_colaborador tcCriador
	                ON tcCriador.codigo_interno_colaborador = tv.tb_usuario_criador_cpf
                LEFT JOIN (
                    SELECT *
                    FROM (
                        SELECT
                            tcvl.*,
                            ROW_NUMBER() OVER (
                                PARTITION BY tcvl.tb_candidato_vaga_id
                                ORDER BY tcvl.data_alteracao DESC
                            ) AS rn
                        FROM tb_candidato_vaga_log tcvl
                    ) x
                    WHERE x.rn = 1
                ) tcvl
                    ON tcvl.tb_candidato_vaga_id = tcv.id
                LEFT JOIN tb_colaborador tcColaboradorLog
	                ON tcvl.tb_colaborador_codigo_interno_colaborador = tcColaboradorLog.codigo_interno_colaborador
                WHERE tcv.tb_colaborador_codigo_interno_colaborador_responsavel = @codigoUsuarioLogado
                ORDER BY tv.codigo;
            ";

            var result = await connection.QueryAsync<MeusTalentos>(query, new { codigoUsuarioLogado });
            return result.ToList();
        }

        public async Task<UsuarioColaboradorDTO> BuscarDadosColaboradorEmail(string codInterno, int orgId)
        {
            var conn = _dapperConnection.GetConnection();

            var usuario = await conn.QuerySingleOrDefaultAsync<UsuarioColaboradorDTO>(
                $@"SELECT   tc.email_alternativo AS Email, 
                            tc.nome_completo AS NomeColaborador
			        FROM   tb_colaborador tc
                    WHERE tc.codigo_interno_colaborador = @CodInterno;"
                    , new { CodInterno = codInterno });

            return usuario;

        }

        public async Task<UsuarioColaboradorDTO> BuscarDescricaoOrg(int orgId)
        {
            var conn = _dapperConnection.GetConnection();

            var org = await conn.QuerySingleOrDefaultAsync<UsuarioColaboradorDTO>(
                $@"SELECT   descricao AS DescricaoOrg
			        FROM   tb_org
                    WHERE  id = @OrgId;"
                    , new { OrgId = orgId });

            return org;

        }
        public async Task<TemplateOrgEmailResponseDTO> BuscarDadosTemplateEmail(int orgId)
        {
            var conn = _dapperConnection.GetConnection();

            var corpoEmail = await conn.QuerySingleOrDefaultAsync<TemplateOrgEmailResponseDTO>(
                $@"SELECT  emails_copia AS EmailsCC, 
                           titulo AS Titulo,
                           descricao AS Descricao
			        FROM   tb_candidato_template_email
                    WHERE  tb_org_id = @OrgId  ;"
                    , new { OrgId = orgId });

            return corpoEmail;

        }

        //--- Candidato Template Email

        public async Task<CandidatoTemplateEmailResponseDTO> CandidatoTemplateEmailInserir(CandidatoTemplateEmailParamDTO param)
        {
            var conn = _dapperConnection.GetConnection();


            int qtdExiste = await conn.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM tb_candidato_template_email WHERE tb_org_id = @OrgId;",
                    new { OrgId = param.OrgId });

            if (qtdExiste > 0)
                throw new InvalidOperationException($"Já existe Template com este OrgId: {param.OrgId}");

            var query = @"
                INSERT INTO tb_candidato_template_email (tb_org_id, emails_copia, titulo, descricao)
                VALUES (@OrgId, @EmailsCopia, @Titulo, @Descricao);
            ";

            var parametros = new
            {
                OrgId = param.OrgId,
                EmailsCopia = param.EmailsCC,
                Titulo = param.Titulo,
                Descricao = param.Descricao
            };

            var id = await conn.ExecuteAsync(query, parametros);

            return await CandidatoTemplateEmailListarPorId(param.OrgId);

        }

        public async Task<CandidatoTemplateEmailResponseDTO> CandidatoTemplateEmailAtualizar(CandidatoTemplateEmailParamDTO param)
        {
            var conn = _dapperConnection.GetConnection();

            try
            {
                const string qUpdate = @"
                                UPDATE  tb_candidato_template_email SET
                                        emails_copia = @EmailsCC,
                                        titulo       = @Titulo,
                                        descricao    = @Descricao
                                WHERE   tb_org_id    = @OrgId;
                               ";

                var linhasAfetadas = await conn.ExecuteAsync(qUpdate, new
                {
                    OrgId = param.OrgId,
                    EmailsCC = param.EmailsCC,
                    Titulo = param.Titulo,
                    Descricao = param.Descricao
                });

                if (linhasAfetadas <= 0)
                    throw new InvalidOperationException($"Não existe Template cadastrado com este OrgId: {param.OrgId}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao atualizar Template - " + ex.Message);
            }

            return await CandidatoTemplateEmailListarPorId(param.OrgId);
        }

        public async Task<bool> CandidatoTemplateEmailDeletar(int orgId)
        {
            var conn = _dapperConnection.GetConnection();

            return await conn.ExecuteAsync("DELETE FROM tb_candidato_template_email WHERE tb_org_id = @OrgId;", new { OrgId = orgId }) > 0;
        }


        public async Task<CandidatoTemplateEmailResponseDTO> CandidatoTemplateEmailListarPorId(int orgId)
        {
            var conn = _dapperConnection.GetConnection();

            var listaTemplate = await conn.QuerySingleOrDefaultAsync<CandidatoTemplateEmailResponseDTO>(
                @"SELECT    id AS Id, 
                            tb_org_id AS OrgId,
                            titulo AS Titulo,
                            emails_copia AS EmailsCC,
                            descricao AS Descricao,
                            data_criacao AS DataCriacao,
                            data_alteracao AS DataAlteracao
                FROM tb_candidato_template_email 
                WHERE tb_org_id = @Id;"
                , new { Id = orgId });

            return listaTemplate;
        }

        //--- Metricas Vagas
        //--- Serviço: SRS
        //--- Dashboard - BigNumbers
        public async Task<DashboardBigNumbers> dashboardMetricasRecrutamento(DashboardBigNumbersParam param, int orgId)
        {
            string baseFilterJoins = @"
                    INNER JOIN tb_vaga v ON v.id = l.tb_vaga_id
                    LEFT JOIN tb_gestor_externo tge ON v.tb_gestor_cod = tge.cod_gestor_externo AND tge.tb_org_id = v.tb_org_id
                    LEFT JOIN tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente AND tco.tb_org_id = v.tb_org_id";

            string baseFilterWhere = @"
                    AND (@codigoGestor IS NULL OR @codigoGestor = '' OR FIND_IN_SET(v.tb_gestor_cod, @codigoGestor) > 0)
                    AND (@codigoCliente IS NULL OR @codigoCliente = '' OR FIND_IN_SET(tco.codigo_cliente, @codigoCliente) > 0)
                    AND (@recrutadorCpf IS NULL OR v.tb_colaborador_codigo_interno_colaborador_recrutador = @recrutadorCpf)
                    AND v.tb_org_id = @Id ";

            var codigoGestor = NormalizarListaParaFindInSet(param.CodigoGestor);
            var codigoCliente = NormalizarListaParaFindInSet(param.CodigoCliente);

            // CTEs para organizar dados do histórico (log) e do estado atual das vagas em uma única varredura
            string sql = $@"
                    WITH log_base AS (
                        SELECT l.tb_vaga_id, l.tb_status_vaga_cod, l.data_alteracao
                        FROM tb_vaga_fourmakers_log l
                        {baseFilterJoins}
                        WHERE l.data_alteracao BETWEEN @inicio AND @fim
                        {baseFilterWhere}
                    ),
                    vagas_atuais AS (
                        SELECT 
                            v.id,
                            v.tb_status_vaga_cod
                        FROM tb_vaga v
                        LEFT JOIN tb_gestor_externo tge ON v.tb_gestor_cod = tge.cod_gestor_externo AND tge.tb_org_id = v.tb_org_id
                        LEFT JOIN tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente AND tco.tb_org_id = v.tb_org_id
                        WHERE v.data_ultima_alteracao BETWEEN @inicio AND @fim
                          AND TIMESTAMPDIFF(HOUR, v.data_ultima_alteracao, NOW()) > 24
                          AND (@codigoGestor IS NULL OR @codigoGestor = '' OR FIND_IN_SET(v.tb_gestor_cod, @codigoGestor) > 0)
                          AND (@codigoCliente IS NULL OR @codigoCliente = '' OR FIND_IN_SET(tco.codigo_cliente, @codigoCliente) > 0)
                          AND (@recrutadorCpf IS NULL OR v.tb_colaborador_codigo_interno_colaborador_recrutador = @recrutadorCpf)
                          AND v.tb_org_id = @Id
                    ),
                    metricas_vagas AS (
                        SELECT
                            COUNT(DISTINCT CASE WHEN tb_status_vaga_cod = 2 THEN id END) AS EmFoco,
                            COUNT(DISTINCT CASE WHEN tb_status_vaga_cod = 6 THEN id END) AS EntrevistaCliente,
                            COUNT(DISTINCT CASE WHEN tb_status_vaga_cod = 8 THEN id END) AS Contratacoes,
                            COUNT(DISTINCT CASE WHEN tb_status_vaga_cod = 9 THEN id END) AS VagasCanceladas,
                            COUNT(DISTINCT CASE WHEN tb_status_vaga_cod = 11 THEN id END) AS VagasPerdidas
                        FROM vagas_atuais
                    ),
                    metricas_log AS (
                        SELECT
                            SUM(CASE WHEN tb_status_vaga_cod IN (3,4,5,6,7,10) THEN 1 ELSE 0 END) AS EmAndamento,
                            SUM(CASE WHEN tb_status_vaga_cod = 6 THEN 1 ELSE 0 END) AS CountEntrevista6,
                            SUM(CASE WHEN tb_status_vaga_cod IN (2,3,4,5,6,7,10) THEN 1 ELSE 0 END) AS CountTotalAndamento
                        FROM log_base
                    ),
                    tempo_medio AS (
                        SELECT ROUND(AVG(TIMESTAMPDIFF(SECOND, s.data_inicio, e.data_fim)) / 86400, 2) AS TempoMedio
                        FROM (
                            SELECT tb_vaga_id, MIN(data_alteracao) AS data_inicio
                            FROM log_base WHERE tb_status_vaga_cod = 2 GROUP BY tb_vaga_id
                        ) s
                        INNER JOIN (
                            SELECT tb_vaga_id, MAX(data_alteracao) AS data_fim
                            FROM log_base WHERE tb_status_vaga_cod = 10 GROUP BY tb_vaga_id
                        ) e ON e.tb_vaga_id = s.tb_vaga_id AND e.data_fim > s.data_inicio
                    )
                    SELECT
                        COALESCE(mv.EmFoco, 0) AS EmFoco,
                        COALESCE(ml.EmAndamento, 0) + COALESCE(mv.EmFoco, 0) AS EmAndamento,
                        COALESCE(mv.Contratacoes, 0) AS Contratacoes,
                        COALESCE(mv.EntrevistaCliente, 0) AS EntrevistaCliente,
                        COALESCE(mv.VagasCanceladas, 0) AS VagasCanceladas,
                        COALESCE(mv.VagasPerdidas, 0) AS VagasPerdidas,
                        COALESCE(tm.TempoMedio, 0) AS TempoMedioDiasRecrutamento,
                        COALESCE((ml.CountEntrevista6 * 100.0 / NULLIF(ml.CountTotalAndamento, 0)), 0) AS PercentagemEntrevistaEmAndamento
                    FROM metricas_vagas mv
                    CROSS JOIN metricas_log ml
                    LEFT JOIN tempo_medio tm ON 1=1; ";

            var conn = _dapperConnection.GetConnection();
            {
                return await conn.QueryFirstAsync<DashboardBigNumbers>(sql, new
                {
                    inicio = param.DataInicio?.Date,
                    fim = param.DataFim?.Date.AddDays(1).AddSeconds(-1),
                    codigoGestor,
                    codigoCliente,
                    recrutadorCpf = param.CodigoRecrutador,
                    Id = orgId
                });
            }
        }

        //--- Vagas em Foco +24hs
        public async Task<List<DashboardVagasEmFocoResponse>> dashboardMetricasVagasEmFoco(DashboardBigNumbersParam param, int orgId)
        {
            string baseSelect = @"SELECT tco.nome_cliente AS Cliente, 
                                vag.codigo AS CodVaga, vag.titulo AS Vaga, 
                                svg.descricao AS Status, tcol.nome_completo AS Responsavel,
                                vag.data_ultima_alteracao  AS UltimaMovimentacao, 
                                DATEDIFF(NOW(), vag.data_ultima_alteracao) AS TempoNaEtapa,
                                CASE WHEN DATEDIFF(NOW(), vag.data_ultima_alteracao) > 4 THEN 'Atrasado'
	                                 WHEN DATEDIFF(NOW(), vag.data_ultima_alteracao) > 2 THEN 'No Prazo'
	                                 ELSE 'Atencao' END AS SLA
                                FROM tb_vaga vag ";

            string baseFilterJoins = @"
                                INNER JOIN tb_status_vaga svg on svg.codigo = vag.tb_status_vaga_cod 
                                LEFT  JOIN tb_gestor_externo tge ON vag.tb_gestor_cod = tge.cod_gestor_externo AND tge.tb_org_id = vag.tb_org_id
                                LEFT  JOIN tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente AND tco.tb_org_id = vag.tb_org_id
                                LEFT  JOIN tb_colaborador tcol on tcol.codigo_interno_colaborador = vag.tb_colaborador_codigo_interno_colaborador_recrutador ";

            string baseFilterWhere = @"
                AND (@codigoGestor IS NULL OR @codigoGestor = '' OR FIND_IN_SET(vag.tb_gestor_cod, @codigoGestor) > 0)
                AND (@codigoCliente IS NULL OR @codigoCliente = '' OR FIND_IN_SET(tco.codigo_cliente, @codigoCliente) > 0)
                AND (@recrutadorCpf IS NULL OR vag.tb_colaborador_codigo_interno_colaborador_recrutador = @recrutadorCpf)
                AND vag.tb_org_id = @Id ";

            var codigoGestor = NormalizarListaParaFindInSet(param.CodigoGestor);
            var codigoCliente = NormalizarListaParaFindInSet(param.CodigoCliente);

            string sql = $@"
                     {baseSelect} {baseFilterJoins}
                     WHERE vag.tb_status_vaga_cod = 2 AND vag.data_ultima_alteracao BETWEEN @inicio AND @fim 
                     AND TIMESTAMPDIFF(HOUR, vag.data_ultima_alteracao, NOW()) > 24
                     {baseFilterWhere}
                     ;";

            var conn = _dapperConnection.GetConnection();

            var result = await conn.QueryAsync<DashboardVagasEmFocoResponse>(sql, new
            {
                inicio = param.DataInicio?.Date,
                fim = param.DataFim?.Date.AddDays(1).AddSeconds(-1),
                codigoGestor,
                codigoCliente,
                recrutadorCpf = param.CodigoRecrutador,
                Id = orgId
            });

            return result.ToList();
        }

        //-- Funil de Vagas
        public async Task<DashboardFunilVagasResponse> dashboardMetricasFunilDeVagas(DashboardBigNumbersParam param, int orgId)
        {
            string baseFilterJoins = @"
                INNER JOIN tb_vaga vag ON vag.id = log.tb_vaga_id
                LEFT JOIN tb_gestor_externo tge ON vag.tb_gestor_cod = tge.cod_gestor_externo AND tge.tb_org_id = vag.tb_org_id
                LEFT JOIN tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente AND tco.tb_org_id = vag.tb_org_id";

            string baseFilterWhere = @"
                AND (@codigoGestor IS NULL OR @codigoGestor = '' OR FIND_IN_SET(vag.tb_gestor_cod, @codigoGestor) > 0)
                AND (@codigoCliente IS NULL OR @codigoCliente = '' OR FIND_IN_SET(tco.codigo_cliente, @codigoCliente) > 0)
                AND (@recrutadorCpf IS NULL OR vag.tb_colaborador_codigo_interno_colaborador_recrutador = @recrutadorCpf)
                AND vag.tb_org_id = @Id ";

            var codigoGestor = NormalizarListaParaFindInSet(param.CodigoGestor);
            var codigoCliente = NormalizarListaParaFindInSet(param.CodigoCliente);

            // Uma única varredura (CTE) + agregação condicional para todas as métricas do funil
            // EmFoco é calculado baseado na situação atual das vagas (tb_vaga), não no log histórico
            string sql = $@"
                WITH base AS (
                    SELECT log.tb_status_vaga_cod, log.data_alteracao
                    FROM tb_vaga_fourmakers_log log
                    {baseFilterJoins}
                    WHERE log.data_alteracao BETWEEN @inicio AND @fim
                    {baseFilterWhere}
                )
                SELECT
                    COALESCE((
                        SELECT COUNT(DISTINCT vag.id)
                        FROM tb_vaga vag
                        LEFT JOIN tb_gestor_externo tge ON vag.tb_gestor_cod = tge.cod_gestor_externo AND tge.tb_org_id = vag.tb_org_id
                        LEFT JOIN tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente AND tco.tb_org_id = vag.tb_org_id
                        WHERE vag.tb_status_vaga_cod = 2 
                          AND vag.data_ultima_alteracao BETWEEN @inicio AND @fim
                          AND TIMESTAMPDIFF(HOUR, vag.data_ultima_alteracao, NOW()) > 24
                          AND (@codigoGestor IS NULL OR @codigoGestor = '' OR FIND_IN_SET(vag.tb_gestor_cod, @codigoGestor) > 0)
                          AND (@codigoCliente IS NULL OR @codigoCliente = '' OR FIND_IN_SET(tco.codigo_cliente, @codigoCliente) > 0)
                          AND (@recrutadorCpf IS NULL OR vag.tb_colaborador_codigo_interno_colaborador_recrutador = @recrutadorCpf)
                          AND vag.tb_org_id = @Id
                    ), 0) AS EmFoco,
                    COALESCE(SUM(CASE WHEN tb_status_vaga_cod = 3 THEN 1 ELSE 0 END), 0) AS EntrevistaInicial,
                    COALESCE(SUM(CASE WHEN tb_status_vaga_cod = 4 THEN 1 ELSE 0 END), 0) AS AplicacaoTestes,
                    COALESCE(SUM(CASE WHEN tb_status_vaga_cod = 5 THEN 1 ELSE 0 END), 0) AS EntrevistaTecnica,
                    COALESCE(SUM(CASE WHEN tb_status_vaga_cod = 6 THEN 1 ELSE 0 END), 0) AS EntrevistaComCliente,
                    COALESCE(SUM(CASE WHEN tb_status_vaga_cod = 7 THEN 1 ELSE 0 END), 0) AS CartaOferta,
                    COALESCE(SUM(CASE WHEN tb_status_vaga_cod = 10 THEN 1 ELSE 0 END), 0) AS ProcurandoCandidatos
                FROM base;
                ";

            var conn = _dapperConnection.GetConnection();
            {
                return await conn.QueryFirstAsync<DashboardFunilVagasResponse>(sql, new
                {
                    inicio = param.DataInicio?.Date,
                    fim = param.DataFim?.Date.AddDays(1).AddSeconds(-1),
                    codigoGestor,
                    codigoCliente,
                    recrutadorCpf = param.CodigoRecrutador,
                    Id = orgId
                });
            }
        }

        //--- Vagas Perdidas por Motivo
        public async Task<List<DashboardPerdidasMotivoResponse>> dashboardMetricasVagasPerdidasMotivo(DashboardBigNumbersParam param, int orgId)
        {
            string baseSelect = @"SELECT COUNT(*) AS Total, CAST(tpd.id AS CHAR) AS Id, COALESCE(tpd.descricao, 'Outros') AS Motivo FROM  tb_vaga vag ";

            string baseFilterJoins = @"
                LEFT  JOIN tb_gestor_externo tge ON vag.tb_gestor_cod = tge.cod_gestor_externo AND tge.tb_org_id = vag.tb_org_id
                LEFT  JOIN tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente AND tco.tb_org_id = vag.tb_org_id
                LEFT  JOIN tb_vaga_motivos_perda tpd on tpd.id = vag.tb_vaga_motivos_perda_id ";

            string baseFilterWhere = @"
                AND (@codigoGestor IS NULL OR @codigoGestor = '' OR FIND_IN_SET(vag.tb_gestor_cod, @codigoGestor) > 0)
                AND (@codigoCliente IS NULL OR @codigoCliente = '' OR FIND_IN_SET(tco.codigo_cliente, @codigoCliente) > 0)
                AND (@recrutadorCpf IS NULL OR vag.tb_colaborador_codigo_interno_colaborador_recrutador = @recrutadorCpf)
                AND vag.tb_org_id = @Id ";

            var codigoGestor = NormalizarListaParaFindInSet(param.CodigoGestor);
            var codigoCliente = NormalizarListaParaFindInSet(param.CodigoCliente);

            string sql = $@"
                     {baseSelect} {baseFilterJoins}
                     WHERE vag.tb_status_vaga_cod = 11 AND vag.data_ultima_alteracao BETWEEN @inicio AND @fim
                     {baseFilterWhere}
                     GROUP BY tpd.id, tpd.descricao ;";

            var conn = _dapperConnection.GetConnection();

            var resultado = await conn.QueryAsync<DashboardPerdidasMotivoResponse>(sql, new
            {
                inicio = param.DataInicio?.Date,
                fim = param.DataFim?.Date.AddDays(1).AddSeconds(-1),
                codigoGestor,
                codigoCliente,
                recrutadorCpf = param.CodigoRecrutador,
                Id = orgId
            });

            return resultado.ToList();

        }

        //--- Aquisição Candidatos
        public async Task<List<DashboardAquisicaoCandidatos>> dashboardMetricasAquisicaoCandidatos(DashboardBigNumbersParam param, int orgId)
        {
            string baseSelect = @"SELECT count(*) AS Total, vag.tb_org_id AS OrgId, log.tb_usuario_cpf AS CodigoInternoColaborador FROM  tb_vaga_fourmakers_log log ";

            string baseFilterJoins = @"
                INNER JOIN tb_vaga vag ON vag.id = log.tb_vaga_id
                LEFT  JOIN tb_gestor_externo tge ON vag.tb_gestor_cod = tge.cod_gestor_externo AND tge.tb_org_id = vag.tb_org_id
                LEFT  JOIN tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente AND tco.tb_org_id = vag.tb_org_id
                ";

            string baseFilterWhere = @"
                AND (@codigoGestor  IS NULL OR @codigoGestor  = '' OR FIND_IN_SET(vag.tb_gestor_cod, @codigoGestor)   > 0)
                AND (@codigoCliente IS NULL OR @codigoCliente = '' OR FIND_IN_SET(tco.codigo_cliente, @codigoCliente) > 0)
                AND (@recrutadorCpf IS NULL OR vag.tb_colaborador_codigo_interno_colaborador_recrutador = @recrutadorCpf)
                AND vag.tb_org_id = @Id ";

            var codigoGestor = NormalizarListaParaFindInSet(param.CodigoGestor);
            var codigoCliente = NormalizarListaParaFindInSet(param.CodigoCliente);

            string sql = $@"
                     {baseSelect} {baseFilterJoins}
                     WHERE log.tb_status_vaga_cod = 1 AND log.data_alteracao BETWEEN @inicio AND @fim
                     {baseFilterWhere} 
                     ;";

            var conn = _dapperConnection.GetConnection();

            var resultado = await conn.QueryAsync<DashboardAquisicaoCandidatos>(sql, new
            {
                inicio = param.DataInicio?.Date,
                fim = param.DataFim?.Date.AddDays(1).AddSeconds(-1),
                codigoGestor,
                codigoCliente,
                recrutadorCpf = param.CodigoRecrutador,
                Id = orgId
            });

            return resultado.ToList();

        }

        //--- Novos candidatos (banco de talentos) agrupados por origem
        public async Task<List<DashboardNovosCandidatosPorOrigemItem>> dashboardNovosCandidatosPorOrigem(DashboardNovosCandidatosParam param, int orgId)
        {
            const string baseSelect = @"
                SELECT 1 AS Total, bt.tb_org_id AS OrgId, bt.tipo_cadastro AS TipoCadastro, bt.forma_cadastro AS FormaCadastro
                FROM tb_banco_talentos bt
                WHERE (
                    (bt.data_criacao IS NOT NULL AND bt.data_criacao BETWEEN @inicio AND @fim)
                    OR (bt.data_criacao IS NULL AND (bt.tipo_cadastro = 'SRS_LINKEDIN' OR bt.forma_cadastro = 2))
                )
                AND bt.tb_org_id = @Id ";

            var conn = _dapperConnection.GetConnection();
            var resultado = await conn.QueryAsync<DashboardNovosCandidatosPorOrigemItem>(baseSelect, new
            {
                inicio = param.DataInicio?.Date,
                fim = param.DataFim?.Date.AddDays(1).AddSeconds(-1),
                Id = orgId
            });
            return resultado.ToList();
        }

        public async Task<List<RecrutadorListagemResponse>> RecrutadorListagem(string nomeRecrutador, int cursor, int limite, int orgId)
        {
            const string baseSelect = @"
                            SELECT tc.nome_completo AS NomeRecrutador,
                            tv.tb_colaborador_codigo_interno_colaborador_recrutador AS CodigoRecrutador
                            FROM tb_vaga tv
                            INNER JOIN tb_colaborador tc on tc.codigo_interno_colaborador = tv.tb_colaborador_codigo_interno_colaborador_recrutador
                            WHERE tv.tb_colaborador_codigo_interno_colaborador_recrutador IS NOT NULL
                            AND tv.tb_org_id = @orgId
                            AND (@nomeRecrutador IS NULL OR @nomeRecrutador = '' OR LOWER(tc.nome_completo) LIKE CONCAT('%', LOWER(@nomeRecrutador), '%'))
                            GROUP BY tc.nome_completo, tv.tb_colaborador_codigo_interno_colaborador_recrutador
                            ORDER BY tc.nome_completo, tv.tb_colaborador_codigo_interno_colaborador_recrutador 
                            LIMIT @limite OFFSET @cursor;";

            var conn = _dapperConnection.GetConnection();
            var resultado = await conn.QueryAsync<RecrutadorListagemResponse>(baseSelect, new
            {
                cursor,
                limite,
                nomeRecrutador,
                orgId
            });
            return resultado.ToList();
        }



    }
}