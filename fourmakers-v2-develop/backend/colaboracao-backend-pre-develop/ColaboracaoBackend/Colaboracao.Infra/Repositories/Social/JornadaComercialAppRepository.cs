using Colaboracao.Core.Interfaces;
using Core.Domain.Social;
using Dapper;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Social;
using DataTransferObject.Domain.Usuario;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Social
{
    public class JornadaComercialAppRepository : IJornadaComercialAppRepository
    {
        private readonly IDBConnection _dapperConnection;

        // Agendas Realizadas
        private const string QCabecalhoAgendasRealizadas = """
                                                WITH ResumoAgendas AS (
                                                    SELECT 
                                                        COUNT(CASE WHEN (DATE(tae.data_agendada) >= @dataInicio AND DATE(tae.data_agendada) <= @dataFim AND DATE(tae.data_agendada) < CURDATE()) THEN 1 END) AS TotalAtrasados,
                                                        SUM(CASE 
                                                            WHEN DATE(tae.data_agendada) >= @dataInicio AND DATE(tae.data_agendada) < @dataFim
                                                             AND DATE(tae.data_agendada) BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL 48 HOUR)
                                                            THEN 1 ELSE 0 
                                                        END) AS TotalExpirando
                                                    FROM tb_agendas_comerciais tae
                                                    INNER JOIN tb_cliente_org tco ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente AND tco.tb_org_id = @orgIdUsuarioLogado
                                                )
                                                SELECT 
                                                    TotalAtrasados,
                                                    TotalExpirando,
                                                    (SELECT count(*) FROM tb_agendas_comerciais tae
                                                     INNER JOIN tb_cliente_org tco ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente AND tco.tb_org_id = @orgIdUsuarioLogado
                                                     WHERE DATE(data_agendada) >= @dataInicio AND DATE(data_agendada) <= @dataFim  
                                                     AND DATE(tae.data_agendada) < CURDATE()
                                                    ) AS Total 
                                                FROM ResumoAgendas
                                                """;

        private const string QCabecalhoReunioesAgendadas = """
                                              SELECT count(*) AS Total,
                                              0 AS TotalAtrasados,
                                              SUM(CASE WHEN DATE(tae.data_agendada) >= CURDATE() AND DATE(tae.data_agendada) <= @dataFim
                                                            AND DATE(tae.data_agendada) BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL 48 HOUR)
                                                       THEN 1 ELSE 0 
                                                 END) AS TotalExpirando
                                             FROM tb_agendas_comerciais tae
                                              INNER JOIN tb_cliente_org tco
                                               ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente
                                              AND tco.tb_org_id = @orgIdUsuarioLogado      
                                              INNER JOIN tb_colaborador tc
                                                 ON tc.codigo_interno_colaborador = tae.tb_colaborador_codigo_interno_colaborador
                                             """;
       
        private const string QDetalhesReunioesAgendadas = """
                                             SELECT  tae.id AS AgendaId,
                                                     tae.titulo,
                                                     tae.descricao,
                                                     tc.nome_completo AS Responsavel,
                                                     tae.data_agendada AS Prazo,
                                                     tae.status,
                                                     tae.tb_cliente_org_codigo_cliente AS ClienteId,
                                                     tco.nome_cliente AS Cliente,
                                                     tae.localizacao                                             
                                             FROM tb_agendas_comerciais tae
                                             INNER JOIN tb_colaborador tc
                                                 ON tc.codigo_interno_colaborador = tae.tb_colaborador_codigo_interno_colaborador
                                             INNER JOIN tb_cliente_org tco
                                                 ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente
                                                AND tco.tb_org_id = @orgIdUsuarioLogado
                                             """;

        private const string QBaseAgendasRealizadasDetalhe = """                                             
                                             WITH InteracoesUnicas AS (
                                                 SELECT 
                                                     tb_agendas_comerciais_id, 
                                                     COUNT(*) as total_interacoes
                                                 FROM tb_interacoes 
                                                 GROUP BY tb_agendas_comerciais_id
                                             )
                                             SELECT 
                                                 tae.id AS AgendaId,
                                                 tae.titulo,
                                                 tae.descricao,
                                                 tc.nome_completo AS Responsavel,
                                                 tae.data_agendada AS Prazo,
                                                 tae.status,
                                                 tae.tb_cliente_org_codigo_cliente AS ClienteId,
                                                 tco.nome_cliente AS Cliente,    
                                                 tae.localizacao,
                                                 tae.id AS Id,
                                                 CASE 
                                                     WHEN DATE(tae.data_agendada) >= @dataInicio AND DATE(tae.data_agendada) < @dataFim
                                                      AND DATE(tae.data_agendada) < CURDATE()
                                                     AND iu.total_interacoes IS NULL 
                                                     THEN 1 ELSE 0 
                                                 END AS Atrasado,
                                                 CASE 
                                                     WHEN DATE(tae.data_agendada) >= @dataInicio AND DATE(tae.data_agendada) < @dataFim 
                                                          AND DATE(tae.data_agendada) BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL 48 HOUR)
                                                          AND iu.total_interacoes IS NULL 
                                                     THEN 1 ELSE 0 
                                                 END AS Vencendo
                                             FROM tb_agendas_comerciais tae
                                             INNER JOIN tb_colaborador tc 
                                                 ON tc.codigo_interno_colaborador = tae.tb_colaborador_codigo_interno_colaborador
                                             INNER JOIN tb_cliente_org tco 
                                                 ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente 
                                                 AND tco.tb_org_id = @orgIdUsuarioLogado
                                             LEFT JOIN InteracoesUnicas iu 
                                                 ON iu.tb_agendas_comerciais_id = tae.id
                                             """;

        private const string QCabecalhoAcoes = """
                                                 SELECT COUNT(tae.id) AS Total,      	    
                                                 COUNT(CASE WHEN (@dataInicio IS NULL OR DATE(tae.data_limite) >= @dataInicio) 
                                                                 AND (@dataFim IS NULL OR DATE(tae.data_limite) < @dataFim)
                                                                 AND DATE(tae.data_limite) < CURDATE()
                                                            THEN 1 END) AS TotalAtrasados,
                                                 SUM(CASE 
                                                     WHEN (@dataInicio IS NULL OR DATE(tae.data_limite) >= @dataInicio) 
                                                          AND (@dataFim IS NULL OR DATE(tae.data_limite) < @dataFim)
                                                          AND DATE(tae.data_limite) BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL 48 HOUR)
                                                         THEN 1 ELSE 0 
                                                     END) AS TotalExpirando
                                                 FROM tb_interacao_acoes tae
                                                 INNER JOIN tb_interacao_ai tii ON tii.id = tae.tb_interacao_ai_id
                                                 INNER JOIN tb_interacoes ti ON ti.id = tii.tb_interacoes_id
                                                 INNER JOIN tb_agendas_comerciais tmm ON tmm.id = ti.tb_agendas_comerciais_id
                                                 INNER JOIN tb_cliente_org tco ON tco.codigo_cliente = tmm.tb_cliente_org_codigo_cliente AND tco.tb_org_id = @orgIdUsuarioLogado
                                                 INNER JOIN tb_colaborador tc
                                                    ON tc.codigo_interno_colaborador = tae.tb_colaborador_codigo_interno_colaborador
                                             """;

        private const string QAcoesBase = """
                                        SELECT 
                                            tia.id AS Id,
                                            tia.tb_interacao_ai_id AS InteracaoAiId,
                                            tia2.tb_agendas_comerciais_id AS AgendaId,
                                            tii.tb_interacoes_id AS InteracaoId,
                                            tia.texto AS Texto,
                                            tc.nome_completo AS NomeResponsavel,
                                            tia.tb_colaborador_codigo_interno_colaborador AS CodInternoColaborador,
                                            tia.data_limite AS DataLimite,
                                            tia.tb_status_acoes_id AS StatusAcoes,
                                            CASE WHEN (@dataInicio IS NULL OR DATE(tia.data_limite) >= @dataInicio) 
                                                      AND (@dataFim IS NULL OR DATE(tia.data_limite) < @dataFim)
                                                      AND DATE(tia.data_limite) < CURDATE()
                                                 THEN 1 ELSE 0 END AS Atrasado,
                                            CASE WHEN (@dataInicio IS NULL OR DATE(tia.data_limite) >= @dataInicio) 
                                                      AND (@dataFim IS NULL OR DATE(tia.data_limite) < @dataFim)
                                                      AND DATE(tia.data_limite) BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL 48 HOUR)
                                                 THEN 1 ELSE 0 END AS Vencendo				 
                                        FROM tb_interacao_acoes tia
                                        INNER JOIN tb_interacao_ai tii ON tii.id = tia.tb_interacao_ai_id   
                                        INNER JOIN tb_interacoes tia2 ON tia2.id = tii.tb_interacoes_id 
                                        INNER JOIN tb_agendas_comerciais tmm ON tmm.id = tia2.tb_agendas_comerciais_id
                                        INNER JOIN tb_cliente_org tco ON tco.codigo_cliente = tmm.tb_cliente_org_codigo_cliente AND tco.tb_org_id = @orgIdUsuarioLogado
                                        INNER JOIN tb_colaborador tc
                                           ON tc.codigo_interno_colaborador = tia.tb_colaborador_codigo_interno_colaborador
                                        """;

        public static string QAcoesBase1 => QAcoesBase;

        public JornadaComercialAppRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<KanbanEncontrosAcoesComerciais> BuscarKanbanEncontrosAcoesComerciais(string busca, DateTime? dataInicio, DateTime? dataFim, int orgIdUsuarioLogado)
        {
            var conn = _dapperConnection.GetConnection();

            // Se as datas não forem informadas, assumir valores padrão para trazer tudo
            if (!dataInicio.HasValue)
                dataInicio = new DateTime(2000, 1, 1);
            
            if (!dataFim.HasValue)
                dataFim = DateTime.Now.AddDays(1000);
            
            KanbanEncontrosAcoesComerciais retorno = new();

            #region Reunioes Agendadas + Realizadas 

            var filtroData = "";
            if (dataInicio.HasValue)
                filtroData += " AND DATE(tae.data_agendada) >= @dataInicio";

            if (dataFim.HasValue)
                filtroData += " AND DATE(tae.data_agendada) <= @dataFim";

            var filtroBusca = "";
            var filtroBuscaAcoes = "";

            if (!string.IsNullOrEmpty(busca))
            {
                busca = busca.Replace("+", "%").Replace("'", "%").Replace("/", "%");
                filtroBusca = " AND tco.nome_cliente LIKE CONCAT('%', @busca, '%') ";
                filtroBuscaAcoes = " AND tco.nome_cliente LIKE CONCAT('%', @busca, '%') ";
            }
            else
            {
                filtroBusca = " AND (@busca IS NULL OR @busca = '') ";
            }
                //var filtroBusca = " AND (@busca IS NULL OR @busca = '' OR LOWER(tco.nome_cliente) LIKE CONCAT('%', LOWER(@busca), '%')) ";

                var sqlReunioes = $@"
                                {QCabecalhoReunioesAgendadas}
                                WHERE 1 = 1
                                AND DATE(tae.data_agendada) >= CURDATE() AND DATE(tae.data_agendada) <= @dataFim 
                                {filtroBusca};
                                
                                {QDetalhesReunioesAgendadas}
                                WHERE 1 = 1
                                AND DATE(tae.data_agendada) >= CURDATE() AND DATE(tae.data_agendada) <= @dataFim 
                                {filtroBusca};

                                WITH ResumoAgendas AS (
                                    SELECT 
                                        COUNT(CASE WHEN (DATE(tae.data_agendada) >= @dataInicio AND DATE(tae.data_agendada) <= @dataFim AND DATE(tae.data_agendada) < CURDATE()) THEN 1 END) AS TotalAtrasados,
                                        SUM(CASE 
                                            WHEN DATE(tae.data_agendada) >= @dataInicio AND DATE(tae.data_agendada) < @dataFim
                                             AND DATE(tae.data_agendada) BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL 48 HOUR)
                                            THEN 1 ELSE 0 
                                        END) AS TotalExpirando
                                    FROM tb_agendas_comerciais tae
                                    INNER JOIN tb_cliente_org tco ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente AND tco.tb_org_id = @orgIdUsuarioLogado
                                    INNER JOIN tb_colaborador tc 
                                        ON tc.codigo_interno_colaborador = tae.tb_colaborador_codigo_interno_colaborador
                                    WHERE 1 = 1
                                    {filtroBusca}                              
                                )
                                SELECT 
                                    TotalAtrasados,
                                    TotalExpirando,
                                    (SELECT count(*) FROM tb_agendas_comerciais tae
                                     INNER JOIN tb_cliente_org tco ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente AND tco.tb_org_id = @orgIdUsuarioLogado
                                     INNER JOIN tb_colaborador tc 
                                        ON tc.codigo_interno_colaborador = tae.tb_colaborador_codigo_interno_colaborador
                                     WHERE DATE(data_agendada) >= @dataInicio AND DATE(data_agendada) <= @dataFim  
                                                    AND DATE(tae.data_agendada) < CURDATE()
                                     {filtroBusca}  
                                    ) AS Total 
                                FROM ResumoAgendas
                                ;

                                {QBaseAgendasRealizadasDetalhe}
                                WHERE 1 = 1
                                {filtroData}
                                AND DATE(tae.data_agendada) < CURDATE()
                                {filtroBusca};";

            using (var multiReunioes = await conn.QueryMultipleAsync(sqlReunioes, new { dataInicio, dataFim, busca, orgIdUsuarioLogado }))
            {
                retorno.ReunioesAgendadas.Cabecalho = await multiReunioes.ReadFirstOrDefaultAsync<CabecalhoReunioesAcoes>();
                retorno.ReunioesAgendadas.Reunioes = (await multiReunioes.ReadAsync<AgendaComercial>()).ToList();
                retorno.ReunioesRealizadas.Cabecalho = await multiReunioes.ReadFirstOrDefaultAsync<CabecalhoReunioesAcoes>();
                retorno.ReunioesRealizadas.Reunioes = (await multiReunioes.ReadAsync<AgendaComercialId>()).ToList();
            }

            //if (!retorno.ReunioesAgendadas.Reunioes.Any())
            //    return retorno;

            foreach (var agenda in retorno.ReunioesAgendadas.Reunioes)
            {
                agenda.GestoresCliente = await BuscarGestoresCliente(agenda.ClienteId, agenda.AgendaId);
                agenda.ParticipantesAgenda = await BuscarParticipantesAgenda(agenda.AgendaId);
            }

            #endregion Reunioes Agendadas

            #region Reunioes Realizadas

            foreach (var acao in retorno.ReunioesRealizadas.Reunioes)
            {
                acao.InteracaoCategoria = await BuscarCategoriasPorInteracao(acao.Id);
            }


            foreach (var agenda in retorno.ReunioesRealizadas.Reunioes)
            {
                agenda.GestoresCliente = await BuscarGestoresCliente(agenda.ClienteId, agenda.AgendaId);
                agenda.ParticipantesAgenda = await BuscarParticipantesAgenda(agenda.AgendaId);
            }

            #endregion Reunioes Realizadas

            #region Ações 
            
            var filtroDataAcoes = "";
            if (dataInicio.HasValue)
                filtroDataAcoes += " AND DATE(tmm.data_agendada) >= @dataInicio";
            if (dataFim.HasValue)
                filtroDataAcoes += " AND DATE(tmm.data_agendada) <= @dataFim";

            // 1-Em Andamento / 2-Pendente / 3-Concluída
            var sqlAcoes = $@"
                            {QCabecalhoAcoes} 
                            WHERE tae.tb_status_acoes_id = 1
                            AND tae.data_limite IS NOT NULL
                            {filtroBuscaAcoes}
                            {filtroDataAcoes};

                            {QAcoesBase}
                            WHERE tia.tb_status_acoes_id = 1 
                            AND tia.data_limite IS NOT NULL
                            {filtroBuscaAcoes}
                            {filtroDataAcoes}
                            ;

                            {QCabecalhoAcoes} 
                            WHERE tae.tb_status_acoes_id = 2
                            AND tae.data_limite IS NOT NULL
                            {filtroBuscaAcoes}
                            {filtroDataAcoes};

                            {QAcoesBase}
                            WHERE tia.tb_status_acoes_id = 2
                            AND tia.data_limite IS NOT NULL
                            {filtroBuscaAcoes}
                            {filtroDataAcoes}
                            ;

                            {QCabecalhoAcoes} 
                            WHERE tae.tb_status_acoes_id = 3
                            AND tae.data_limite IS NOT NULL
                            {filtroBuscaAcoes}
                            {filtroDataAcoes};

                            {QAcoesBase}
                            WHERE tia.tb_status_acoes_id = 3 
                            AND tia.data_limite IS NOT NULL
                            {filtroBuscaAcoes}
                            {filtroDataAcoes}
                            ;";

            using (var multiAcoes = await conn.QueryMultipleAsync(sqlAcoes, new { dataInicio, dataFim, busca, orgIdUsuarioLogado }))
            {
                retorno.AcoesAndamento.Cabecalho = await multiAcoes.ReadFirstOrDefaultAsync<CabecalhoReunioesAcoes>();
                retorno.AcoesAndamento.Acoes = (await multiAcoes.ReadAsync<AcaoKanbanBody>()).ToList();
                retorno.AcoesPendentes.Cabecalho = await multiAcoes.ReadFirstOrDefaultAsync<CabecalhoReunioesAcoes>();
                retorno.AcoesPendentes.Acoes = (await multiAcoes.ReadAsync<AcaoKanbanBody>()).ToList();
                retorno.AcoesRealizadas.Cabecalho = await multiAcoes.ReadFirstOrDefaultAsync<CabecalhoReunioesAcoes>();
                retorno.AcoesRealizadas.Acoes = (await multiAcoes.ReadAsync<AcaoKanbanBody>()).ToList();
            }

            foreach (var acao in retorno.AcoesAndamento.Acoes)
                acao.InteracaoCategoria = await BuscarCategoriasPorInteracao(acao.InteracaoId);
            
            foreach (var acao in retorno.AcoesPendentes.Acoes)
                acao.InteracaoCategoria = await BuscarCategoriasPorInteracao(acao.InteracaoId);
            
            foreach (var acao in retorno.AcoesRealizadas.Acoes)
                acao.InteracaoCategoria = await BuscarCategoriasPorInteracao(acao.InteracaoId);

            #endregion Ações

            return retorno;
        }

        private async Task<List<InteracaoComCategoriaSubDTO>> BuscarCategoriasPorInteracao(int interacaoId)
        {
            var conn = _dapperConnection.GetConnection();

            var sql = @"
                        SELECT 
                            tcs.id AS Id,
                            tcs.tb_interacoes_id AS InteracaoId,
                            tcs.tb_categoria_assunto_id AS CategoriaId,
                            tca.descricao AS CategoriaDescricao,
                            tcs.tb_subcategoria_assunto_id AS SubcategoriaId,
                            tsa.descricao AS SubcategoriaDescricao,
                            tcs.ativo AS Ativo
                        FROM tb_interacoes_categoria_sub tcs
                        INNER JOIN tb_categoria_assunto tca 
                            ON tca.id = tcs.tb_categoria_assunto_id
                        INNER JOIN tb_subcategoria_assunto tsa 
                            ON tsa.tb_categoria_assunto_id = tcs.tb_categoria_assunto_id 
                            AND tsa.id = tcs.tb_subcategoria_assunto_id
                        WHERE tcs.tb_interacoes_id = @interacaoId
                          AND tcs.ativo = 1;
                    ";

            return (await conn.QueryAsync<InteracaoComCategoriaSubDTO>(sql, new { interacaoId })).ToList();
        }
        private async Task<List<GestorCliente>> BuscarGestoresCliente(string clienteId, int agendaId)
        {
            var conn = _dapperConnection.GetConnection();

            return (await conn.QueryAsync<GestorCliente>(@$"
                                                            SELECT  tge.cod_gestor_externo AS CodigoGestorExterno,
                                                                    tge.nome AS Nome
                                                            FROM tb_gestor_externo tge 
                                                            INNER JOIN tb_agenda_convidados tep 
	                                                            ON tep.codigo_colaborador_interno_externo  = tge.cod_gestor_externo 
	                                                            AND tep.tb_agendas_comerciais_id = @agendaId
                                                            WHERE tge.codigo_cliente = @clienteId AND tep.tipo_codigo = 2 ", 
                                                            new { clienteId, agendaId })).ToList();
        }

        private async Task<ParticipantesAgenda> BuscarParticipantesAgenda(int agendaId)
        {
            var conn = _dapperConnection.GetConnection();

            string sql = @"
                        /* 1. Gestores externos */
                        SELECT g.cod_gestor_externo AS CodigoGestorExterno, g.nome AS Nome
                        FROM tb_agenda_convidados p
                        INNER JOIN tb_gestor_externo g ON g.cod_gestor_externo = p.codigo_colaborador_interno_externo
                        WHERE p.tb_agendas_comerciais_id = @agendaId AND p.tipo_codigo = 2;

                        /* 2. Colaboradores */
                        SELECT DISTINCT tc.codigo_interno_colaborador AS CodInternoColaborador, tc.nome_completo AS Nome, tc.email_alternativo AS Email
                        FROM tb_agenda_convidados p
                        INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = p.codigo_colaborador_interno_externo
                        WHERE p.tb_agendas_comerciais_id = @agendaId AND p.tipo_codigo = 1;

                        /* 3. Participantes Externos */
                        SELECT DISTINCT tpe.nome, tpe.email 
                        FROM tb_participantes_externo tpe
                        WHERE tpe.tb_agendas_comerciais_id = @agendaId;";

            using (var multi = await conn.QueryMultipleAsync(sql, new { agendaId }))
            {
                return new ParticipantesAgenda
                {
                    GestoresExterno = (await multi.ReadAsync<GestorCliente>()).ToList(),
                    Colaboradores = (await multi.ReadAsync<ColaboradorAgenda>()).ToList(),
                    ParticipantesExterno = (await multi.ReadAsync<ParticipanteExterno>()).ToList()
                };
            }
        }


        public async Task<EncontroAiPassos> AtualizarStatusAcoes(AtualizarIteracoesAcoesParam p)
        {
            if (p.Id <= 0 || p.StatusAcoesId <= 0)
                throw new ArgumentException("Parâmetro enviado inválido.");

            var conn = _dapperConnection.GetConnection();

            // existe?
            var okIteracaoAcao = await conn.ExecuteScalarAsync<bool>(
                    "SELECT EXISTS(SELECT 1 FROM tb_interacao_acoes WHERE id = @Id AND tb_interacao_ai_id = @InteracaoAiIid)",
                     new { Id = p.Id, p.InteracaoAiIid });

            if (!okIteracaoAcao)
                throw new InvalidOperationException("Interação Ação não encontrada.");

            var okStatusAcao = await conn.ExecuteScalarAsync<bool>(
                "SELECT EXISTS(SELECT 1 FROM tb_status_acoes WHERE id = @Id)",
                 new { Id = p.StatusAcoesId });

            if (!okStatusAcao)
                throw new InvalidOperationException("Status de Interação Ação não encontrada.");

            const string sQuery = @"
                                UPDATE tb_interacao_acoes SET
                                    tb_status_acoes_id   = @Tb_status_acoes_id                                  
                                WHERE id = @Id AND tb_interacao_ai_id = @InteracaoAiIid;
                                ";
            await conn.ExecuteAsync(sQuery, new
            {
                Id = p.Id,
                InteracaoAiIid = p.InteracaoAiIid,
                Tb_status_acoes_id = p.StatusAcoesId
            });

            return await CarregarIteracaoAcao(p.Id);
        }

        public async Task<EncontroAiPassos> CarregarIteracaoAcao(int iteracaoAcaoId)
        {
            var conn = _dapperConnection.GetConnection();

            var sQuery = @"
                    SELECT  teap.id AS Id, 
		                    teap.texto AS Texto, 
                            teap.tb_colaborador_codigo_interno_colaborador AS CodigoColaborador,
                            tc.nome_completo AS NomeColaborador,
                            teap.data_limite AS DataLimite,
                            teap.tb_status_acoes_id AS StatusAcoesId, 
                            teap.comentario_acao AS ComentariosAcoes
                    FROM tb_interacao_acoes teap
                    INNER JOIN tb_colaborador tc 
	                    ON tc.codigo_interno_colaborador = teap.tb_colaborador_codigo_interno_colaborador 
                    WHERE teap.id = @Id;";

            var parametros = new
            {
                Id = iteracaoAcaoId
            };

            var result = await conn.QueryFirstOrDefaultAsync<EncontroAiPassos>(sQuery, parametros);

            return result;
        }

        public async Task<InteracaoAcoesResponseDTO> ListarInteracaoAcao(int interacaoAcaoId)
        {
            var conn = _dapperConnection.GetConnection();

            var sQuery = @"
                     SELECT teap.id AS Id, 
                            teap.tb_interacao_ai_id AS InteracaoAiId,
                            teap.tb_colaborador_codigo_interno_colaborador AS CodigoColaborador,
                            tc.nome_completo AS NomeColaborador,
		                    teap.texto AS Texto, 
                            teap.data_limite AS DataLimite,
                            teap.tb_status_acoes_id AS StatusAcoesId,
                            tsa.descricao  AS DescricaoStatusAcao
                    FROM tb_interacao_acoes teap
                    INNER JOIN tb_colaborador tc 
	                    ON tc.codigo_interno_colaborador = teap.tb_colaborador_codigo_interno_colaborador 
	                INNER JOIN tb_status_acoes tsa ON tsa.id = teap.tb_status_acoes_id 
                    WHERE teap.tb_interacao_ai_id = @Id;";

            var parametros = new
            {
                Id = interacaoAcaoId
            };

            var result = await conn.QueryFirstOrDefaultAsync<InteracaoAcoesResponseDTO>(sQuery, parametros);

            return result;
        }
        public async Task<InteracaiAIResponse> InserirInteracaoIA(EncontroAiParamInclusao p)
        {
            var conn = _dapperConnection.GetConnection();

            // existe?
            var okIteracaoAcao = await conn.ExecuteScalarAsync<bool>(
                "SELECT EXISTS(SELECT 1 FROM tb_interacoes WHERE id = @Id )",
                 new { Id = p.InteracaoId });

            if (!okIteracaoAcao)
                throw new InvalidOperationException("Interação não encontrada.");

            const string sQuery = @"
                                INSERT INTO tb_interacao_ai (tb_interacoes_id, resumo)
                                VALUES (@Id, @Resumo);
                                SELECT LAST_INSERT_ID();
                                ";

            var interacaoID = await conn.QuerySingleAsync<int>(sQuery, new
            {
                Id = p.InteracaoId,
                Resumo = p.Resumo
            });

            return await BuscarInteracaoAiPorIdAsync(interacaoID);
        }

        public async Task<InteracaiAIResponse> AtualizarInteracaoIA(EncontroAiParamAtualizacao p)
        {
            var conn = _dapperConnection.GetConnection();

            const string qUpd = @"
                                UPDATE tb_interacao_ai SET
                                    resumo = @Resumo
                                WHERE id = @Id;
                                ";

            await conn.ExecuteAsync(qUpd, new
            {
                Id = p.Id,
                Resumo = p.Resumo
            });

            return await BuscarInteracaoAiPorIdAsync(p.Id);
        }

        public async Task<bool> DeletarInteracaoIA(int interacaoIA)
        {
            var conn = _dapperConnection.GetConnection();
            var tx = await conn.BeginTransactionAsync();

            const string qDel = @"
                                DELETE FROM tb_interacao_ai
                                WHERE id = @Id;
                                ";

            int linhas = await conn.ExecuteAsync(qDel, new { Id = interacaoIA }, tx);

            if (linhas == 0)
            {
                await tx.RollbackAsync();
                return false;
            }

            await tx.CommitAsync();
            return true;
        }
        private async Task<InteracaiAIResponse?> BuscarInteracaoAiPorIdAsync(long id)
        {
            var conn = _dapperConnection.GetConnection();

            const string sQuery = @"
                    SELECT 
                        id, 
                        tb_interacoes_id AS InteracaoId, 
                        resumo, 
                        data_gerada AS DataGerada 
                    FROM tb_interacao_ai 
                    WHERE id = @Id;";

            return await conn.QueryFirstOrDefaultAsync<InteracaiAIResponse>(sQuery, new { Id = id });
        }

        public async Task<ComentariosAcoesResponseDTO> InsercaoComentariosAcoes(ComentariosAcoesParamDTO param, UsuarioLogadoDTO user)
        {
            if (param is null || param.InteracaoAcoesId <= 0)
                throw new ArgumentException("Parâmetro Comentário inválido.");

            var conn = _dapperConnection.GetConnection();

            // existe?
            var okIteracaoAcao = await conn.ExecuteScalarAsync<bool>(
                "SELECT EXISTS(SELECT 1 FROM tb_interacao_acoes WHERE id = @Id )",
                 new { Id = param.InteracaoAcoesId });

            if (!okIteracaoAcao)
                throw new InvalidOperationException("Id Interação Ação não encontrada.");

            const string insertComentario = @"
                    INSERT INTO tb_comentarios_interacao_acoes
                           (tb_interacao_acoes_id, tb_colaborador_codigo_interno_colaborador, comentario, data)
                    VALUES (@InteracaoAcoesId, @CodInternoColaborador, @Comentario, @Data);
                    SELECT LAST_INSERT_ID();";

            var idComentario = await conn.QuerySingleAsync<long>(insertComentario, new
            {
                InteracaoAcoesId = param.InteracaoAcoesId,
                CodInternoColaborador = user.Cpf,
                Comentario = param.Comentario,
                Data = param.Data
            });

            return await ListaComentarioAcao(idComentario);

        }

        public async Task<ComentariosAcoesResponseDTO> ListaComentarioAcao(long comentarioAcaoId)
        {
            var conn = _dapperConnection.GetConnection();

            var sQuery = @"
                     SELECT teap.id AS Id, 
                            teap.tb_interacao_acoes_id AS InteracaoAcoesId,
                            teap.tb_colaborador_codigo_interno_colaborador AS CodInternoColaborador,
                            tc.nome_completo AS NomeColaborador,
                            teap.data AS Data,
                            teap.comentario AS Comentario
                    FROM tb_comentarios_interacao_acoes teap
                    INNER JOIN tb_colaborador tc 
	                    ON tc.codigo_interno_colaborador = teap.tb_colaborador_codigo_interno_colaborador 
                    WHERE teap.id = @Id;";

            return await conn.QueryFirstOrDefaultAsync<ComentariosAcoesResponseDTO>(
                sQuery,
                new { Id = comentarioAcaoId }
            );
        }
        public async Task<ComentariosAcoesResponseDTO> AtualizarComentario(ComentariosAcoesResponseDTO param, UsuarioLogadoDTO user)
        {
            var query = @"UPDATE tb_comentarios_interacao_acoes 
                  SET comentario = @Descricao,
                      data = NOW(),
                      tb_colaborador_codigo_interno_colaborador = @Usuario
                  WHERE id = @Id;

                  SELECT * FROM tb_comentarios_interacao_acoes WHERE id = @Id;";

            var conn = _dapperConnection.GetConnection();
            return await conn.QueryFirstOrDefaultAsync<ComentariosAcoesResponseDTO>(query, new
            {
                param.Id,
                param.Comentario,
                Usuario = user.Cpf
            });
        }

        public async Task<bool> DeletarComentario(long comentarioId)
        {
            var query = @"DELETE FROM tb_comentarios_interacao_acoes WHERE id = @Id;";

            var conn = _dapperConnection.GetConnection();
            var rows = await conn.ExecuteAsync(query, new { Id = comentarioId });

            return rows > 0;
        }
        public async Task<List<ComentariosAcoesResponseDTO>> ListarComentariosPorInteracao(int interacaoAcoesId)
        {
            var conn = _dapperConnection.GetConnection();

            const string sQuery = @"
                                SELECT teap.id AS Id, 
                                       teap.tb_interacao_acoes_id AS InteracaoAcoesId,
                                       teap.tb_colaborador_codigo_interno_colaborador AS CodInternoColaborador,
                                       tc.nome_completo AS NomeColaborador,
                                       teap.data AS Data,
                                       teap.comentario AS Comentario
                                FROM tb_comentarios_interacao_acoes teap
                                INNER JOIN tb_colaborador tc
                                        ON tc.codigo_interno_colaborador = teap.tb_colaborador_codigo_interno_colaborador
                                WHERE teap.tb_interacao_acoes_id = @InteracaoAcoesId
                                ORDER BY teap.data DESC;
                            ";

            return (await conn.QueryAsync<ComentariosAcoesResponseDTO>(sQuery, new
            {
                InteracaoAcoesId = interacaoAcoesId
            })).ToList();
        }
        public async Task<IEnumerable<CategoriaAssuntoDTO>> ListarCategorias()
        {
            var conn = _dapperConnection.GetConnection();
            var query = "SELECT id AS Id, descricao AS Descricao FROM tb_categoria_assunto;";
            return await conn.QueryAsync<CategoriaAssuntoDTO>(query);
        }

        public async Task<IEnumerable<SubcategoriaAssuntoDTO>> ListarSubcategoriasPorCategoria(int categoriaId)
        {
            var conn = _dapperConnection.GetConnection();
            var query = @"
                        SELECT id AS Id,
                               descricao AS Descricao,
                               tb_categoria_assunto_id AS CategoriaAssuntoId,
                               data_criacao AS DataCriacao,
                               ativo AS Ativo
                          FROM tb_subcategoria_assunto
                         WHERE tb_categoria_assunto_id = @CategoriaId";

            return await conn.QueryAsync<SubcategoriaAssuntoDTO>(query, new { CategoriaId = categoriaId });
        }
        public async Task<IEnumerable<CategoriaAssuntoComSubDTO>> ListarCategoriasComSub()
        {
            var conn = _dapperConnection.GetConnection();

            var sql = @"
                        SELECT 
                            c.id AS Id,
                            c.descricao AS Descricao,
                            s.id AS Id,
                            s.descricao AS Descricao,
                            s.tb_categoria_assunto_id AS CategoriaAssuntoId,
                            s.data_criacao AS DataCriacao,
                            s.ativo AS Ativo
                        FROM tb_categoria_assunto c
                        LEFT JOIN tb_subcategoria_assunto s 
                               ON s.tb_categoria_assunto_id = c.id
                        ORDER BY c.id, s.id;
                    ";

            var categoriaDict = new Dictionary<int, CategoriaAssuntoComSubDTO>();

            await conn.QueryAsync<CategoriaAssuntoComSubDTO, SubcategoriaAssuntoDTO, CategoriaAssuntoComSubDTO>(
                sql,
                (categoria, sub) =>
                {
                    if (!categoriaDict.TryGetValue(categoria.Id, out var categoriaAtual))
                    {
                        categoriaAtual = categoria;
                        categoriaAtual.Subcategorias = new List<SubcategoriaAssuntoDTO>();
                        categoriaDict.Add(categoriaAtual.Id, categoriaAtual);
                    }

                    if (sub != null && sub.Id > 0)
                    {
                        if (!categoriaAtual.Subcategorias.Any(x => x.Id == sub.Id))
                            categoriaAtual.Subcategorias.Add(sub);
                    }

                    return categoriaAtual;
                },
                splitOn: "Id"
            );

            return categoriaDict.Values;
        }

        public async Task<InteracaoCategoriaSubResponseDTO> InsercaoInteracaoCategoria(CategoriaInsercaoParamDTO param)
        {
            if (param is null || param.InteracaoId <= 0)
                throw new ArgumentException("Parâmetro Comentário inválido.");

            var conn = _dapperConnection.GetConnection();

            var okIteracaoAcao = await conn.ExecuteScalarAsync<bool>(
                "SELECT EXISTS(SELECT 1 FROM tb_interacoes WHERE id = @Id )",
                 new { Id = param.InteracaoId });

            if (!okIteracaoAcao)
                throw new InvalidOperationException("Id Interação Ação não encontrada.");

            var okCateg = await conn.ExecuteScalarAsync<bool>(
                "SELECT EXISTS(SELECT 1 FROM tb_subcategoria_assunto WHERE id = @Id AND tb_categoria_assunto_id = @CategoriaId )",
                 new { Id = param.SubCategoriaAssuntoId, CategoriaId = param.CategoriaAssuntoId });

            if (!okCateg)
                throw new InvalidOperationException("Categoria/Sub não encontrada.");

            //Inserção nas Categorias
            const string qCateg = @"
                                INSERT INTO tb_interacoes_categoria_sub
                                        (tb_interacoes_id,
                                         tb_categoria_assunto_id,
                                         tb_subcategoria_assunto_id)
                                VALUES
                                        (@InteracaoId, 
                                         @CategoriaAssuntoId, 
                                         @SubCategoriaAssuntoId);
                                SELECT LAST_INSERT_ID();
                                ";

            long retId = await conn.QuerySingleAsync<long>(qCateg, new
            {
                InteracaoId = param.InteracaoId,
                CategoriaAssuntoId = param.CategoriaAssuntoId,
                SubCategoriaAssuntoId = param.SubCategoriaAssuntoId
            });

            return await ListarInteracaoCategoriaSub(retId);
        }
        public async Task<InteracaoCategoriaSubResponseDTO> AtualizarInteracaoCategoria(CategoriaAtualizacaoParamDTO param)
        {
            if (param is null || param.InteracaoCategoriaId is null || param.InteracaoCategoriaId <= 0)
                throw new ArgumentException("Parâmetro de atualização inválido.");

            var interacaoCategoriaId = param.InteracaoCategoriaId.Value;
            var conn = _dapperConnection.GetConnection();

            var okIntSubCateg = await conn.ExecuteScalarAsync<bool>(
                "SELECT EXISTS(SELECT 1 FROM tb_interacoes_categoria_sub WHERE id = @Id )",
                 new { Id = interacaoCategoriaId });

            if (!okIntSubCateg)
                throw new InvalidOperationException("ID Interação Categoria/Sub não encontrada.");

            var okSubCateg = await conn.ExecuteScalarAsync<bool>(
                "SELECT EXISTS(SELECT 1 FROM tb_subcategoria_assunto WHERE id = @Id AND tb_categoria_assunto_id = @CategoriaId )",
                 new { Id = param.SubCategoriaAssuntoId, CategoriaId = param.CategoriaAssuntoId });

            if (!okSubCateg)
                throw new InvalidOperationException("Categoria/Sub não encontrada.");

            const string qCateg = @"UPDATE tb_interacoes_categoria_sub SET
                                        tb_categoria_assunto_id    = @CategoriaAssuntoId,
                                        tb_subcategoria_assunto_id = @SubCategoriaAssuntoId
                           WHERE id = @Id;";

            var rows = await conn.ExecuteAsync(qCateg, new
            {
                Id = interacaoCategoriaId,
                CategoriaAssuntoId = param.CategoriaAssuntoId,
                SubCategoriaAssuntoId = param.SubCategoriaAssuntoId
            });

            if (rows == 0)
                throw new InvalidOperationException("Não foi possível atualizar o registro de categoria/sub.");

            return await ListarInteracaoCategoriaSub(interacaoCategoriaId);
        }

        public async Task<bool> DeletarInteracaoCategoria(long interacaoCategoriaId)
        {
            var conn = _dapperConnection.GetConnection();

            var okIntSubCateg = await conn.ExecuteScalarAsync<bool>(
                "SELECT EXISTS(SELECT 1 FROM tb_interacoes_categoria_sub WHERE id = @Id )",
                 new { Id = interacaoCategoriaId });

            if (!okIntSubCateg)
                throw new InvalidOperationException("Id Interação Categoria/Sub não encontrada.");

            var query = @"DELETE FROM tb_interacoes_categoria_sub WHERE id = @Id;";

            var rows = await conn.ExecuteAsync(query, new { Id = interacaoCategoriaId });

            return rows > 0;
        }

        public async Task<InteracaoCategoriaSubResponseDTO> ListarInteracaoCategoriaSub(long intercaoCategoriaId)
        {
            var conn = _dapperConnection.GetConnection();
            var query = @"
                        SELECT id AS InteracaoCategoriaId,
                               tb_interacoes_id AS InteracaoId,
                               tb_categoria_assunto_id AS CategoriaAssuntoId,
                               tb_subcategoria_assunto_id AS SubCategoriaAssuntoId,
                               data_criacao AS DataCriacao,
                               ativo AS Ativo
                          FROM tb_interacoes_categoria_sub
                         WHERE id = @Id";

            return await conn.QueryFirstOrDefaultAsync<InteracaoCategoriaSubResponseDTO>(query, new { Id = intercaoCategoriaId });
        }

        public async Task<(List<AgendaHierarquicaDTO> Data, int TotalRegistros)> FiltroAgendasInteracoesCategoriaSub(
                                                            string nomeCliente = null,
                                                            DateTime? dataAgendadaInicio = null,
                                                            DateTime? dataAgendadaFim = null,
                                                            int? categoriaId = null,
                                                            int? subCategoriaId = null,
                                                            int pagina = 1,
                                                            int limite = 10)
        {
            int offset = (pagina - 1) * limite;

            var sqlCount = @"
                            SELECT COUNT(DISTINCT tag.id)
                            FROM tb_agendas_comerciais tag
                            LEFT JOIN tb_interacoes tbi ON tbi.tb_agendas_comerciais_id = tag.id
                            LEFT JOIN tb_interacoes_categoria_sub tis ON tis.tb_interacoes_id = tbi.id
                            LEFT JOIN tb_categoria_assunto tca ON tca.id = tis.tb_categoria_assunto_id
                            LEFT JOIN tb_subcategoria_assunto tcs 
                                ON tcs.id = tis.tb_subcategoria_assunto_id
                                AND tcs.tb_categoria_assunto_id = tis.tb_categoria_assunto_id
                            LEFT JOIN tb_cliente_org cli on cli.codigo_cliente = tag.tb_cliente_org_codigo_cliente
                            WHERE 1 = 1
                        ";

            var sql = @"
                        SELECT 
                            tag.id AS AgendaId,
                            tag.titulo AS Titulo,
                            tag.tb_cliente_org_codigo_cliente AS CodigoCliente,
                            cli.nome_cliente AS NomeCliente,
                            tag.quantidade_participantes AS QuantidadeParticipante,
                            tag.data_agendada AS DataAgendado,
                            tbi.id AS InteracaoId,
                            tca.id AS CategoriaId,
                            tca.descricao AS CategoriaNome,
                            tcs.id AS SubCategoriaId,
                            tcs.descricao AS SubCategoriaNome
                        FROM tb_agendas_comerciais tag
                        LEFT JOIN tb_interacoes tbi 
                            ON tbi.tb_agendas_comerciais_id = tag.id
                        LEFT JOIN tb_interacoes_categoria_sub tis 
                            ON tis.tb_interacoes_id = tbi.id
                        LEFT JOIN tb_categoria_assunto tca 
                            ON tca.id = tis.tb_categoria_assunto_id 
                        LEFT JOIN tb_subcategoria_assunto tcs 
                            ON tcs.id = tis.tb_subcategoria_assunto_id
                            AND tcs.tb_categoria_assunto_id = tis.tb_categoria_assunto_id
                        LEFT JOIN tb_cliente_org cli on cli.codigo_cliente = tag.tb_cliente_org_codigo_cliente
                        WHERE 1 = 1
                    ";

            var param = new DynamicParameters();

            // FILTROS
            if (!string.IsNullOrEmpty(nomeCliente))
            {
                sql += " AND cli.nome_cliente LIKE CONCAT('%', @NomeCliente, '%')";
                sqlCount += " AND cli.nome_cliente LIKE CONCAT('%', @NomeCliente, '%')";
                param.Add("NomeCliente", nomeCliente);
            }

            if (dataAgendadaInicio.HasValue)
            {
                sql += " AND tag.data_agendada >= @DataAgendadaInicio";
                sqlCount += " AND tag.data_agendada >= @DataAgendadaInicio";
                param.Add("DataAgendadaInicio", dataAgendadaInicio);
            }

            if (dataAgendadaFim.HasValue)
            {
                //var dataFim = dataAgendadaFim.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                var dataFim = dataAgendadaFim.Value.Date.AddDays(1).AddTicks(-1);

                sql += " AND tag.data_agendada <= @DataAgendadaFim";
                sqlCount += " AND tag.data_agendada <= @DataAgendadaFim";
                param.Add("DataAgendadaFim", dataFim);
            }

            if (categoriaId.HasValue)
            {
                sql += " AND tca.id = @CategoriaId";
                sqlCount += " AND tca.id = @CategoriaId";
                param.Add("CategoriaId", categoriaId);
            }

            if (subCategoriaId.HasValue)
            {
                sql += " AND tcs.id = @SubCategoriaId";
                sqlCount += " AND tcs.id = @SubCategoriaId";
                param.Add("SubCategoriaId", subCategoriaId);
            }

            sql += @"
                    ORDER BY tag.id DESC
                    LIMIT @Limite OFFSET @Offset;
                ";

            param.Add("Limite", limite);
            param.Add("Offset", offset);

            var conn = _dapperConnection.GetConnection();

            // TOTAL PARA PAGINAÇÃO
            int total = await conn.ExecuteScalarAsync<int>(sqlCount, param);

            // BUSCA DINÂMICA
            var rows = await conn.QueryAsync(sql, param);

            // AGRUPAMENTO HIERÁRQUICO
            var agendas = new Dictionary<int, AgendaHierarquicaDTO>();

            foreach (var row in rows)
            {
                int agendaId = (int)row.AgendaId;

                // Agenda
                if (!agendas.TryGetValue(agendaId, out var agenda))
                {
                    agenda = new AgendaHierarquicaDTO
                    {
                        AgendaId = agendaId,
                        Titulo = row.Titulo,
                        CodigoCliente = row.CodigoCliente,
                        NomeCliente = row.NomeCliente,
                        QuantidadeParticipante = row.QuantidadeParticipante,
                        DataAgendado = row.DataAgendado,
                        Interacoes = new List<InteracaoDTO>()
                    };

                    agendas.Add(agendaId, agenda);
                }

                // Interação
                if (row.InteracaoId != null)
                {
                    int interacaoId = (int)row.InteracaoId;
                    string titulo = (string)row.Titulo;

                    var interacao = agenda.Interacoes
                        .FirstOrDefault(i => i.InteracaoId == interacaoId);

                    if (interacao == null)
                    {
                        interacao = new InteracaoDTO
                        {
                            InteracaoId = interacaoId,
                            TituloInteracao = titulo,
                            Categorias = new List<CategoriaDTO>()
                        };

                        agenda.Interacoes.Add(interacao);
                    }

                    // Categoria
                    if (row.CategoriaId != null)
                    {
                        int categoria = (int)row.CategoriaId;

                        var cat = interacao.Categorias
                            .FirstOrDefault(c => c.CategoriaId == categoria);

                        if (cat == null)
                        {
                            cat = new CategoriaDTO
                            {
                                CategoriaId = categoria,
                                CategoriaNome = row.CategoriaNome,
                                SubCategorias = new List<SubCategoriaDTO>()
                            };

                            interacao.Categorias.Add(cat);
                        }

                        // Subcategoria
                        if (row.SubCategoriaId != null)
                        {
                            int subId = (int)row.SubCategoriaId;

                            if (!cat.SubCategorias.Any(s => s.SubCategoriaId == subId))
                            {
                                cat.SubCategorias.Add(new SubCategoriaDTO
                                {
                                    SubCategoriaId = subId,
                                    SubCategoriaNome = row.SubCategoriaNome
                                });
                            }
                        }
                    }
                }
            }

            return (agendas.Values.ToList(), total);
        }

        public async Task<IntegracaoMoxeResponseDTO> BuscarProximosPassosIntegracaoMoxe(IntegracaoMoxeRequestDTO request)
        {
            try
            {
                // Construir o prompt
                var titulo = request.Agenda?.Titulo ?? "Informação não fornecida";
                var tipo = request.Agenda?.TipoInteracao ?? "Informação não fornecida";
                var data = request.Agenda?.Data ?? "Informação não fornecida";
                var horario = request.Agenda?.Horario ?? "Informação não fornecida";
                var local = request.Agenda?.Local ?? "Informação não fornecida";
                var descricao = request.Agenda?.Descricao ?? "Informação não fornecida";
                var vagas = request.Agenda?.Vagas ?? 0;

                var ata = request.Interacao?.Ata ?? "Informação não fornecida";
                var principaisPontosAudio = request.Interacao?.PrincipaisPontosAudio ?? "Informação não fornecida";
                var categoria = request.Interacao?.Categoria ?? "Informação não fornecida";
                var subcategoria = request.Interacao?.SubCategoria ?? "Informação não fornecida";
                
                var nomeCliente = request.Cliente?.Empresa ?? "Informação não fornecida";
                var codigoCliente = request.Cliente?.Codigo ?? "Informação não fornecida";
                var qtdAlocados = request.Cliente?.QtdAlocados ?? 0;

                // Formatar lista de gestores
                var gestoresInfo = "";
                if (request.Cliente?.Gestores != null && request.Cliente.Gestores.Any())
                {
                    foreach (var gestor in request.Cliente.Gestores)
                    {
                        gestoresInfo += $"  - {gestor.Nome} ({gestor.Email})\n";
                    }
                }
                else
                {
                    gestoresInfo = "  - Informação não fornecida\n";
                }

                // Formatar lista de equipe
                var equipeInfo = "";
                if (request.EquipeFourtalentsParticipante != null && request.EquipeFourtalentsParticipante.Any())
                {
                    foreach (var participante in request.EquipeFourtalentsParticipante)
                    {
                        equipeInfo += $"  - {participante}\n";
                    }
                }
                else
                {
                    equipeInfo = "  - Informação não fornecida\n";
                }

                var horarioLine = !string.IsNullOrEmpty(horario) && horario != "Informação não fornecida" 
                    ? $"- Horário: {horario}" 
                    : "";

                var transcriptsInfo = !string.IsNullOrEmpty(principaisPontosAudio) && principaisPontosAudio != "Informação não fornecida"
                    ? principaisPontosAudio
                    : "  - Informação não fornecida";

                var prompt = $@"
                    Você é um assistente de produtividade. Leia os **Dados de contexto** e retorne **somente** o JSON no formato abaixo:

                    ```json
                    {{
                      ""resumo"": ""até 4 linhas descrevendo quem participou, o que foi discutido e decidido"",
                      ""passos"": [
                        ""Próximo passo 1 (imperativo e breve)"",
                        ""Próximo passo 2"",
                        ""Próximo passo 3""
                      ]
                    }}
                    ```

                    Regras importantes:
                    - Preencha o campo ""resumo"" com até 6 linhas em português, com informações da agenda, citando participantes, tópicos discutidos na interação, os principais pontos capturados em áudio, se existentes, devem ser enfatizados  e decisões quando disponíveis.
                    - Sempre forneça exatamente 3 itens no array ""passos"", iniciados com verbos no imperativo; quando faltar contexto, proponha ações coerentes e sinalize lacunas com ""Informação não fornecida"".
                    - Use a expressão ""Informação não fornecida"" para representar dados ausentes, em vez de declarar que não é possível gerar o resumo.
                    - Nunca responda que não é possível gerar o resumo; sempre utilize as informações fornecidas.
                    - O JSON deve ser válido e não pode conter texto adicional fora dele.

                    **Dados de contexto**

                    **Agenda**
                    - Título: {titulo}
                    - Tipo de interação: {tipo}
                    - Data: {data}
                    {horarioLine}
                    - Local: {local}
                    - Descrição: {descricao}
                    - Número de vagas: {vagas}

                    **Interação**
                    - Ata da interação: {ata}.
                    - Principais pontos capturados em áudio: {transcriptsInfo}.
                    - Categoria: {categoria}.
                    - Subcategoria: {subcategoria}.


                    **Cliente**
                    - Empresa: {nomeCliente}
                    - Código: {codigoCliente}
                    - Quantidade de alocados: {qtdAlocados}
                    - Gestores do cliente: {gestoresInfo}

                    **Equipe Fourtalents participante**
                    {equipeInfo}";

                // Fazer requisição para API Moxe
                using var httpClient = new System.Net.Http.HttpClient();
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

                var jsonContent = System.Text.Json.JsonSerializer.Serialize(moxeRequest);
                var content = new System.Net.Http.StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("https://api-moxe-109278280777.southamerica-east1.run.app/api/v1/inference/", content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new IntegracaoMoxeResponseDTO
                    {
                        Sucesso = false,
                        Mensagem = $"Erro na chamada à API Moxe: {response.StatusCode} - {errorContent}",
                        Dados = null
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                
                // Parse da resposta bruta da API
                var rawResponse = System.Text.Json.JsonSerializer.Deserialize<MoxeApiRawResponseDTO>(responseContent, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (rawResponse?.Result == null)
                {
                    return new IntegracaoMoxeResponseDTO
                    {
                        Sucesso = false,
                        Mensagem = "Resposta da API Moxe não contém o campo 'result'.",
                        Dados = null
                    };
                }

                // Extrair o JSON do campo result (remove os backticks do markdown)
                var jsonResult = rawResponse.Result.Trim();
                
                // Remove os delimitadores de markdown ```json e ```
                if (jsonResult.StartsWith("```json"))
                {
                    jsonResult = jsonResult.Substring(7); // Remove ```json
                }
                if (jsonResult.StartsWith("```"))
                {
                    jsonResult = jsonResult.Substring(3); // Remove ```
                }
                if (jsonResult.EndsWith("```"))
                {
                    jsonResult = jsonResult.Substring(0, jsonResult.Length - 3); // Remove ```
                }
                
                jsonResult = jsonResult.Trim();

                // Parse do JSON extraído
                var moxeResponse = System.Text.Json.JsonSerializer.Deserialize<MoxeAiResponseDTO>(jsonResult, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return new IntegracaoMoxeResponseDTO
                {
                    Sucesso = true,
                    Mensagem = "Informações processadas com sucesso.",
                    Dados = moxeResponse
                };
            }
            catch (Exception ex)
            {
                return new IntegracaoMoxeResponseDTO
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao processar informações: {ex.Message}",
                    Dados = null
                };
            }
        }

        private const string SqlSelectAgendaSolicitanteComJoin = """
            SELECT
                s.id AS Id,
                s.tb_agendas_comerciais_id AS TbAgendasComerciaisId,
                CASE s.tipo_codigo
                    WHEN 1 THEN tc.nome_completo
                    WHEN 2 THEN g.nome
                END AS Nome,
                CASE s.tipo_codigo
                    WHEN 1 THEN COALESCE(NULLIF(TRIM(tc.email_alternativo), ''), NULLIF(TRIM(tu.email), ''), '')
                    WHEN 2 THEN COALESCE(NULLIF(TRIM(g.email), ''), '')
                END AS Email,
                s.codigo_colaborador_interno_externo AS CodigoColaboradorInternoExterno,
                s.tb_colaborador_codigo_interno_colaborador_criador AS CodColaboradorInternoCriador,
                s.tipo_codigo AS TipoCodigo,
                s.tb_status_app_id AS TbStatusAppId,
                s.data_solicitacao AS DataSolicitacao,
                s.data_resposta AS DataResposta
            FROM tb_agenda_solicitante s
            LEFT JOIN tb_colaborador tc ON s.tipo_codigo = 1 AND tc.codigo_interno_colaborador = s.codigo_colaborador_interno_externo
            LEFT JOIN (
                SELECT u1.*
                FROM tb_usuario u1
                INNER JOIN (
                    SELECT codigo_interno_colaborador, MAX(id) AS max_id
                    FROM tb_usuario
                    GROUP BY codigo_interno_colaborador
                ) umax ON u1.codigo_interno_colaborador = umax.codigo_interno_colaborador AND u1.id = umax.max_id
            ) tu ON s.tipo_codigo = 1 AND tu.codigo_interno_colaborador = tc.codigo_interno_colaborador
            LEFT JOIN tb_gestor_externo g ON s.tipo_codigo = 2
                AND g.cod_gestor_externo = s.codigo_colaborador_interno_externo
                AND g.tb_org_id = 2
            """;

        private const string SqlSelectAgendaConvidadoComJoin = """
            SELECT
                c.id AS Id,
                c.tb_agendas_comerciais_id AS TbAgendasComerciaisId,
                CASE c.tipo_codigo
                    WHEN 1 THEN tc.nome_completo
                    WHEN 2 THEN g.nome
                END AS Nome,
                CASE c.tipo_codigo
                    WHEN 1 THEN COALESCE(NULLIF(TRIM(tc.email_alternativo), ''), NULLIF(TRIM(tu.email), ''), '')
                    WHEN 2 THEN COALESCE(NULLIF(TRIM(g.email), ''), '')
                END AS Email,
                c.codigo_colaborador_interno_externo AS CodigoColaboradorInternoExterno,
                c.tipo_codigo AS TipoCodigo,
                c.tb_status_app_id AS TbStatusAppId,
                c.data_convite AS DataConvite,
                c.data_resposta AS DataResposta
            FROM tb_agenda_convidados c
            LEFT JOIN tb_colaborador tc ON c.tipo_codigo = 1 AND tc.codigo_interno_colaborador = c.codigo_colaborador_interno_externo
            LEFT JOIN (
                SELECT u1.*
                FROM tb_usuario u1
                INNER JOIN (
                    SELECT codigo_interno_colaborador, MAX(id) AS max_id
                    FROM tb_usuario
                    GROUP BY codigo_interno_colaborador
                ) umax ON u1.codigo_interno_colaborador = umax.codigo_interno_colaborador AND u1.id = umax.max_id
            ) tu ON c.tipo_codigo = 1 AND tu.codigo_interno_colaborador = tc.codigo_interno_colaborador
            LEFT JOIN tb_gestor_externo g ON c.tipo_codigo = 2
                AND g.cod_gestor_externo = c.codigo_colaborador_interno_externo
                AND g.tb_org_id = 2
            """;

        public async Task<AgendaSolicitanteDTO> SolicitarParticiparAgenda(int tbAgendasComerciaisId, string codigoColaboradorExterno, string codColaboradorInternoCriador)
        {
            var codigo = codigoColaboradorExterno.Trim();
            var codigoCriador = string.IsNullOrWhiteSpace(codColaboradorInternoCriador) ? null : codColaboradorInternoCriador.Trim();
            var conn = _dapperConnection.GetConnection();

            var agendaExiste = await conn.ExecuteScalarAsync<bool>(
                "SELECT EXISTS(SELECT 1 FROM tb_agendas_comerciais WHERE id = @Id)",
                new { Id = tbAgendasComerciaisId });

            if (!agendaExiste)
                throw new InvalidOperationException("Agenda não encontrada.");

            var solicitacaoExistente = await conn.QueryFirstOrDefaultAsync<dynamic>(
                """
                SELECT
                    id,
                    codigo_colaborador_interno_externo AS Codigo,
                    tipo_codigo AS TipoCodigo,
                    tb_status_app_id AS Status
                FROM tb_agenda_solicitante
                WHERE tb_agendas_comerciais_id = @TbAgendasComerciaisId
                  AND TRIM(codigo_colaborador_interno_externo) = @Codigo
                LIMIT 1;
                """,
                new { TbAgendasComerciaisId = tbAgendasComerciaisId, Codigo = codigo });

            if (solicitacaoExistente != null)
                throw new InvalidOperationException(
                    $"Você já solicitou participação nessa agenda (codigo='{solicitacaoExistente.Codigo}', tipoCodigo={solicitacaoExistente.TipoCodigo}, status={solicitacaoExistente.Status}).");

            var tipoCodigo = InferirTipoCodigoPorComprimentoDoCodigo(codigo);
            await ValidarCodigoParticipanteExisteAsync(conn, tipoCodigo, codigo);

            const string sqlInsert = @"
                        INSERT INTO tb_agenda_solicitante (
                            tb_agendas_comerciais_id,
                            codigo_colaborador_interno_externo,
                            tipo_codigo,
                            tb_colaborador_codigo_interno_colaborador_criador,
                            data_solicitacao,
                            data_resposta
                        ) VALUES (
                            @TbAgendasComerciaisId,
                            @CodigoColaboradorExterno,
                            @TipoCodigo,
                            @TbColaboradorCodigoInternoColaboradorCriador,
                            NOW(),
                            NULL
                        );
                        SELECT LAST_INSERT_ID();";

            var novoId = await conn.ExecuteScalarAsync<int>(sqlInsert, new
            {
                TbAgendasComerciaisId = tbAgendasComerciaisId,
                CodigoColaboradorExterno = codigo,
                TipoCodigo = (int)tipoCodigo,
                TbColaboradorCodigoInternoColaboradorCriador = codigoCriador
            });

            return await ObterAgendaSolicitantePorId(novoId);
        }

        public async Task<AgendaSolicitanteDTO> ObterAgendaSolicitantePorId(int id)
        {
            var conn = _dapperConnection.GetConnection();
            var sql = $"{SqlSelectAgendaSolicitanteComJoin}\nWHERE s.id = @Id;";

            var row = await conn.QueryFirstOrDefaultAsync<AgendaSolicitanteDTO>(sql, new { Id = id });
            if (row == null)
                throw new InvalidOperationException("Solicitação de participação não encontrada.");
            return row;
        }

        public async Task<IReadOnlyList<AgendaSolicitanteDTO>> ListarAgendaSolicitantesPorAgendaComercial(int tbAgendasComerciaisId)
        {
            if (tbAgendasComerciaisId <= 0)
                throw new ArgumentException("Agenda Id inválido.", nameof(tbAgendasComerciaisId));

            var conn = _dapperConnection.GetConnection();
            var sql = $"{SqlSelectAgendaSolicitanteComJoin}\nWHERE s.tb_agendas_comerciais_id = @TbAgendasComerciaisId\nORDER BY s.data_solicitacao DESC, s.id DESC;";

            var list = (await conn.QueryAsync<AgendaSolicitanteDTO>(sql, new { TbAgendasComerciaisId = tbAgendasComerciaisId })).ToList();
            return list;
        }

        public async Task<AgendaSolicitanteDTO> AceitarRecusarSolicitanteNaAgenda(AgendaSolicitanteAtualizacaoDTO param)
        {
            var codigo = param.CodigoColaboradorExterno.Trim();
            var conn = _dapperConnection.GetConnection();

            const string sql = @"
                            UPDATE tb_agenda_solicitante SET
                                tb_status_app_id = @TbStatusAppId,
                                data_resposta =  NOW()
                            WHERE tb_agendas_comerciais_id = @TbAgendasComerciaisId
                              AND codigo_colaborador_interno_externo = @CodigoColaboradorExterno;";

            var linhas = await conn.ExecuteAsync(sql, new
            {
                param.TbAgendasComerciaisId,
                CodigoColaboradorExterno = codigo,
                param.TbStatusAppId
            });

            if (linhas == 0)
                throw new InvalidOperationException("Solicitação de participação não encontrada.");

            return await ObterAgendaSolicitantePorAgendaECodigoAsync(param.TbAgendasComerciaisId, codigo);
        }

        public async Task<bool> ExcluirAgendaSolicitante(int tbAgendasComerciaisId, string codigoColaboradorExterno)
        {
            var codigo = codigoColaboradorExterno.Trim();
            var conn = _dapperConnection.GetConnection();
            var linhas = await conn.ExecuteAsync(
                """
                DELETE FROM tb_agenda_solicitante
                WHERE tb_agendas_comerciais_id = @TbAgendasComerciaisId
                  AND codigo_colaborador_interno_externo = @CodigoColaboradorExterno
                """,
                new { TbAgendasComerciaisId = tbAgendasComerciaisId, CodigoColaboradorExterno = codigo });
            return linhas > 0;
        }

        public async Task<IReadOnlyList<AgendaConvidadoDTO>> ConvidarParaAgenda(int tbAgendasComerciaisId, IReadOnlyList<string> codigosColaboradorExterno)
        {
            if (tbAgendasComerciaisId <= 0)
                throw new ArgumentException("Agenda Id inválido.", nameof(tbAgendasComerciaisId));

            if (codigosColaboradorExterno == null || codigosColaboradorExterno.Count == 0)
                throw new ArgumentException("Informe ao menos um código de colaborador ou gestor.", nameof(codigosColaboradorExterno));

            var codigos = RecebeStringConverteLista(codigosColaboradorExterno.Select(c => c?.Trim()).Where(c => !string.IsNullOrEmpty(c)));
            
            if (codigos.Count == 0)
                throw new ArgumentException("Nenhum código válido foi informado.", nameof(codigosColaboradorExterno));

            var conn = _dapperConnection.GetConnection();

            var agendaExiste = await conn.ExecuteScalarAsync<bool>(
                "SELECT EXISTS(SELECT 1 FROM tb_agendas_comerciais WHERE id = @Id)",
                new { Id = tbAgendasComerciaisId });

            if (!agendaExiste)
                throw new InvalidOperationException("Agenda não encontrada.");

            using var tran = conn.BeginTransaction();
            var resultados = new List<AgendaConvidadoDTO>(codigos.Count);

            try
            {
                foreach (var codigo in codigos)
                {
                    var convidadoExistenteId = await conn.QueryFirstOrDefaultAsync<int?>(
                        """
                        SELECT id FROM tb_agenda_convidados
                        WHERE tb_agendas_comerciais_id = @TbAgendasComerciaisId
                          AND codigo_colaborador_interno_externo = @Codigo
                        ORDER BY id DESC
                        LIMIT 1
                        """,
                        new { TbAgendasComerciaisId = tbAgendasComerciaisId, Codigo = codigo },
                        tran);

                    if (convidadoExistenteId is > 0)
                        throw new InvalidOperationException($"Colaborador com código: {codigo} já foi convidado para essa agenda.");

                    var tipoCodigo = InferirTipoCodigoPorComprimentoDoCodigo(codigo);
                    await ValidarCodigoParticipanteExisteAsync(conn, tipoCodigo, codigo);

                    const string sqlInsert = @"
                                    INSERT INTO tb_agenda_convidados (
                                        tb_agendas_comerciais_id,
                                        codigo_colaborador_interno_externo,
                                        tipo_codigo,
                                        data_convite,
                                        data_resposta
                                    ) VALUES (
                                        @TbAgendasComerciaisId,
                                        @CodigoColaboradorExterno,
                                        @TipoCodigo,
                                        NOW(),
                                        NULL
                                    );
                                    SELECT LAST_INSERT_ID();";

                    var novoId = await conn.ExecuteScalarAsync<int>(sqlInsert, new
                    {
                        TbAgendasComerciaisId = tbAgendasComerciaisId,
                        CodigoColaboradorExterno = codigo,
                        TipoCodigo = (int)tipoCodigo
                    }, tran);

                    resultados.Add(await ObterAgendaConvidadoPorIdCore(conn, novoId));
                }

                tran.Commit();
                return resultados;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<AgendaConvidadoDTO> ObterAgendaConvidadoPorId(int id)
        {
            var conn = _dapperConnection.GetConnection();
            return await ObterAgendaConvidadoPorIdCore(conn, id);
        }

        private static async Task<AgendaConvidadoDTO> ObterAgendaConvidadoPorIdCore(IDbConnection conn, int id)
        {
            var sql = $"{SqlSelectAgendaConvidadoComJoin}\nWHERE c.id = @Id;";

            var row = await conn.QueryFirstOrDefaultAsync<AgendaConvidadoDTO>(sql, new { Id = id });

            if (row == null)
                throw new InvalidOperationException("Convite de agenda não encontrado.");

            return row;
        }

        private static List<string> RecebeStringConverteLista(IEnumerable<string> source)
        {
            var vistos = new HashSet<string>(StringComparer.Ordinal);
            var lista = new List<string>();
            foreach (var s in source)
            {
                if (vistos.Add(s))
                    lista.Add(s);
            }
            return lista;
        }

        public async Task<IReadOnlyList<AgendaConvidadoDTO>> ListarAgendaConvidadosPorAgendaComercial(int tbAgendasComerciaisId)
        {
            if (tbAgendasComerciaisId <= 0)
                throw new ArgumentException("Agenda Id inválido.", nameof(tbAgendasComerciaisId));

            var conn = _dapperConnection.GetConnection();
            var sql = $"{SqlSelectAgendaConvidadoComJoin}\nWHERE c.tb_agendas_comerciais_id = @TbAgendasComerciaisId\nORDER BY c.data_convite DESC, c.id DESC;";

            var list = (await conn.QueryAsync<AgendaConvidadoDTO>(sql, new { TbAgendasComerciaisId = tbAgendasComerciaisId })).ToList();
            return list;
        }

        public async Task<AgendaConvidadoDTO> AceitarRecusarConvidadoNaAgenda(AgendaConvidadoAtualizacaoDTO param)
        {
            var codigo = param.CodigoColaboradorExterno.Trim();
            var conn = _dapperConnection.GetConnection();

            const string sql = @"
                                UPDATE tb_agenda_convidados SET
                                    tb_status_app_id = @TbStatusAppId,
                                    data_resposta = NOW() 
                                WHERE tb_agendas_comerciais_id = @TbAgendasComerciaisId
                                  AND codigo_colaborador_interno_externo = @CodigoColaboradorExterno;";
            var linhas = await conn.ExecuteAsync(sql, new
            {
                param.TbAgendasComerciaisId,
                CodigoColaboradorExterno = codigo,
                param.TbStatusAppId
            });

            if (linhas == 0)
                throw new InvalidOperationException("Convite de agenda não encontrado.");

            return await ObterAgendaConvidadoPorAgendaECodigoAsync(param.TbAgendasComerciaisId, codigo);
        }

        public async Task<bool> ExcluirAgendaConvidado(int tbAgendasComerciaisId, string codigoColaboradorExterno)
        {
            var codigo = codigoColaboradorExterno.Trim();
            var conn = _dapperConnection.GetConnection();
            var linhas = await conn.ExecuteAsync(
                """
                DELETE FROM tb_agenda_convidados
                WHERE tb_agendas_comerciais_id = @TbAgendasComerciaisId
                  AND codigo_colaborador_interno_externo = @CodigoColaboradorExterno
                """,
                new { TbAgendasComerciaisId = tbAgendasComerciaisId, CodigoColaboradorExterno = codigo });
            return linhas > 0;
        }

        private async Task<AgendaSolicitanteDTO> ObterAgendaSolicitantePorAgendaECodigoAsync(int tbAgendasComerciaisId, string codigoColaboradorExterno)
        {
            var conn = _dapperConnection.GetConnection();
            var sql = $"{SqlSelectAgendaSolicitanteComJoin}\nWHERE s.tb_agendas_comerciais_id = @TbAgendasComerciaisId AND s.codigo_colaborador_interno_externo = @Codigo\nORDER BY s.id DESC\nLIMIT 1;";
            var row = await conn.QueryFirstOrDefaultAsync<AgendaSolicitanteDTO>(sql, new { TbAgendasComerciaisId = tbAgendasComerciaisId, Codigo = codigoColaboradorExterno });
            
            if (row == null)
                throw new InvalidOperationException("Solicitação de participação não encontrada.");
            return row;
        }

        private async Task<AgendaConvidadoDTO> ObterAgendaConvidadoPorAgendaECodigoAsync(int tbAgendasComerciaisId, string codigoColaboradorExterno)
        {
            var conn = _dapperConnection.GetConnection();
            var sql = $"{SqlSelectAgendaConvidadoComJoin}\nWHERE c.tb_agendas_comerciais_id = @TbAgendasComerciaisId AND c.codigo_colaborador_interno_externo = @Codigo\nORDER BY c.id DESC\nLIMIT 1;";
            var row = await conn.QueryFirstOrDefaultAsync<AgendaConvidadoDTO>(sql, new { TbAgendasComerciaisId = tbAgendasComerciaisId, Codigo = codigoColaboradorExterno });
            if (row == null)
                throw new InvalidOperationException("Convite de agenda não encontrado.");
            return row;
        }

        // Comprimento do código < 36 → gestor externo (2); senão → colaborador interno (1).
        private static TipoCodigoParticipanteAgenda InferirTipoCodigoPorComprimentoDoCodigo(string codigo)
        {
            return codigo.Length > 35
                ? TipoCodigoParticipanteAgenda.ColaboradorInterno
                : TipoCodigoParticipanteAgenda.GestorExterno;
        }

        private static async System.Threading.Tasks.Task ValidarCodigoParticipanteExisteAsync(System.Data.IDbConnection conn, TipoCodigoParticipanteAgenda tipoCodigo, string codigo)
        {
            switch (tipoCodigo)
            {
                case TipoCodigoParticipanteAgenda.ColaboradorInterno:
                    {
                        var existe = await conn.ExecuteScalarAsync<bool>(
                            "SELECT EXISTS(SELECT 1 FROM tb_colaborador WHERE codigo_interno_colaborador = @Codigo LIMIT 1)",
                            new { Codigo = codigo });

                        if (!existe)
                            throw new InvalidOperationException("Colaborador não encontrado para o código informado.");
                        return;
                    }
                case TipoCodigoParticipanteAgenda.GestorExterno:
                    {
                        var existe = await conn.ExecuteScalarAsync<bool>(
                            "SELECT EXISTS(SELECT 1 FROM tb_gestor_externo WHERE cod_gestor_externo = @Codigo LIMIT 1)",
                            new { Codigo = codigo });

                        if (!existe)
                            throw new InvalidOperationException("Gestor externo não encontrado para o código informado.");
                        return;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(tipoCodigo), "tipo_codigo deve ser 1 (colaborador interno) ou 2 (gestor externo).");
            }
        }

    }
}