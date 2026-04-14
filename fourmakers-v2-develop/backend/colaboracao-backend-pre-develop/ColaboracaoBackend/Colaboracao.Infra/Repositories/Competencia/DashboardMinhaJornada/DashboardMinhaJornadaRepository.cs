using Colaboracao.Core.Interfaces;
using Core.Domain.Competencia.DashboardMinhaJornada;
using Dapper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Competencia.DashboardMinhaJornada;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Competencia.DashboardMinhaJornada
{
    public class DashboardMinhaJornadaRepository : IDashboardMinhaJornadaRepository
    {
        private readonly IDBConnection _dapperConnection;

        public DashboardMinhaJornadaRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<BigNumbersDashboardMinhaJornada> BigNumbers(int orgIdUsuarioLogado)
        {
            try
            {
                var connection = _dapperConnection.GetConnection();

                var query = @"
                    SELECT
                        SUM(CASE WHEN tsl.tb_skills_movimentacao_id IN (1, 5) THEN 1 ELSE 0 END) AS SkillsAdicionadas,
                        SUM(CASE WHEN tsl.tb_skills_movimentacao_id = 2 THEN 1 ELSE 0 END) AS SkillsRejeitadas,
                        SUM(CASE WHEN tsl.tb_skills_movimentacao_id = 3 THEN 1 ELSE 0 END) AS AdicionadasPDI,
                        SUM(CASE WHEN tsl.tb_skills_movimentacao_id = 4 THEN 1 ELSE 0 END) AS SkillsSugeridas
                    FROM tb_skills_log tsl
                    INNER JOIN tb_gestor_externo_perfil tgep 
	                    ON tgep.id = tsl.tb_gestor_externo_perfil_id
                    WHERE tgep.tb_org_id = @orgIdUsuarioLogado;
                ";

                var result = await connection
                    .QueryFirstOrDefaultAsync<BigNumbersDashboardMinhaJornada>(query, new { orgIdUsuarioLogado });

                return result ?? new BigNumbersDashboardMinhaJornada();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao retornar BigNumbers do dashboard de minha jornada. ERRO: {ex.Message}");
            }
        }

        public async Task<TopDezDashboardMinhaJornada> TopDez(int orgIdUsuarioLogado)
        {
            try
            {
                var connection = _dapperConnection.GetConnection();

                var query = @"  WITH skills_agrupadas AS (
                                SELECT
                                    tsl.skill_id AS SkillId,
                                    tsl.tb_skills_movimentacao_id AS Movimentacao,
                                    COALESCE(
                                        tc.descricao,
                                        tm.descricao,
                                        td.descricao,
                                        ts.descricao,
                                        ti2.descricao,
                                        'DESCONHECIDO'
                                    ) AS NomeSkill,
                                    COUNT(*) AS Total
                                FROM tb_skills_log tsl
                                INNER JOIN tb_gestor_externo_perfil tgep
        							ON tgep.id = tsl.tb_gestor_externo_perfil_id
                                LEFT JOIN tb_competencia tc
                                    ON tc.id = tsl.skill_id AND tsl.tb_item_perfil_id = 1
                                LEFT JOIN tb_metodologia tm
                                    ON tm.id  = tsl.skill_id AND tsl.tb_item_perfil_id = 3
                                LEFT JOIN tb_dominionegocio td
                                    ON td.id  = tsl.skill_id AND tsl.tb_item_perfil_id = 4
                                LEFT JOIN tb_softskill ts
                                    ON ts.id  = tsl.skill_id AND tsl.tb_item_perfil_id = 8
                                LEFT JOIN tb_idioma ti2 
                                    ON ti2.id  = tsl.skill_id AND tsl.tb_item_perfil_id = 9
                                WHERE   tgep.tb_org_id = @orgIdUsuarioLogado
                                GROUP BY
                                    tsl.skill_id,
                                    tsl.tb_skills_movimentacao_id,
                                    NomeSkill
                                ),
                                ranking AS (
                                    SELECT
                                        *,
                                        ROW_NUMBER() OVER (
                                            PARTITION BY Movimentacao
                                            ORDER BY Total DESC
                                        ) AS rn
                                    FROM skills_agrupadas
                                )
                                SELECT
                                    SkillId,
                                    Movimentacao,
                                    NomeSkill,
                                    Total
                                FROM ranking
                                WHERE rn <= 10;
                                ";

                var result = await connection.QueryAsync<SkillTopDezDTO>(query, new { orgIdUsuarioLogado });

                var dashboard = new TopDezDashboardMinhaJornada
                {
                    TopDezPorMovimentacao = result
                        .GroupBy(x => x.Movimentacao)
                        .ToDictionary(
                            g => g.Key,
                            g => g.ToList()
                        )
                };

                return dashboard;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao retornar TopDez do dashboard de minha jornada. ERRO: {ex.Message}");
            }
        }

        public async Task<List<LogDetalhadoDashboardMinhaJornada>> ListarSkillsLog(int limit, int cursor, int orgIdUsuarioLogado)
        {
            try
            {
                var connection = _dapperConnection.GetConnection();

                var query = @"
                    SELECT
                        tsl.id                 AS Id,
                        MAX(tsl.data_criacao)   AS DataCriacao,
                        MAX(tc2.nome_completo)  AS NomeColaborador,
                        MAX(tcor.nome_cliente)  AS NomeCliente,
                        MAX(tgep.nome_perfil)   AS NomePerfil,
                        COALESCE(
                            MAX(tc.descricao),
                            MAX(tm.descricao),
                            MAX(td.descricao),
                            MAX(ts.descricao),
                            MAX(ti2.descricao),
                            'DESCONHECIDO'
                        ) AS NomeSkill,
                        MAX(tip.descricao) AS TipoSkill,
                        MAX(tn.descricao)  AS Senioridade,
                        MAX(CASE tsm.movimentacao
                            WHEN 'ADICIONADO_PERFIL' THEN 'Adicionado Perfil'
                            WHEN 'NAO_INTERESSADO'   THEN 'Rejeitado'
                            WHEN 'ADICIONADO_PDI'    THEN 'Adicionado PDI'
                            WHEN 'SUGERIDO'          THEN 'Sugerido'
                            WHEN 'ATUALIZADO'        THEN 'Atualizado'
                            WHEN 'INTERESSADO'       THEN 'Interessado'
                            ELSE tsm.movimentacao
                        END) AS Evento
                    FROM tb_skills_log tsl
                    LEFT JOIN tb_competencia tc
                        ON tc.id = tsl.skill_id AND tsl.tb_item_perfil_id = 1
                    LEFT JOIN tb_metodologia tm
                        ON tm.id  = tsl.skill_id AND tsl.tb_item_perfil_id = 3
                    LEFT JOIN tb_dominionegocio td
                        ON td.id  = tsl.skill_id AND tsl.tb_item_perfil_id = 4
                    LEFT JOIN tb_softskill ts
                        ON ts.id  = tsl.skill_id AND tsl.tb_item_perfil_id = 8
                    LEFT JOIN tb_idioma ti2 
                        ON ti2.id  = tsl.skill_id AND tsl.tb_item_perfil_id = 9
                    INNER JOIN tb_colaborador tc2
                        ON tc2.codigo_interno_colaborador = tsl.tb_colaborador_codigo_interno_colaborador
                    INNER JOIN tb_gestor_externo_perfil tgep
                        ON tgep.id = tsl.tb_gestor_externo_perfil_id
                    INNER JOIN tb_skills_movimentacao tsm
                        ON tsm.id = tsl.tb_skills_movimentacao_id
                    INNER JOIN tb_item_perfil tip
                        ON tip.id = tsl.tb_item_perfil_id
                    INNER JOIN tb_nivel tn
                        ON tn.id = tsl.tb_nivel_id
                    INNER JOIN tb_colaborador_periodo_alocacao tcpa
	                    ON tcpa.codigo_interno_colaborador = tsl.tb_colaborador_codigo_interno_colaborador 
	                    AND tcpa.data_inicio <= CURDATE()
	                    AND tcpa.data_fim >= CURDATE()
	                    AND tcpa.ativo = 1
                    INNER JOIN tb_perfil_alocacao tpa
	                    ON tpa.tb_colaborador_periodo_alocacao_id = tcpa.id 
	                    AND tpa.tb_gestor_externo_perfil_id = tsl.tb_gestor_externo_perfil_id 
                    INNER JOIN tb_projeto_org tpo
	                    ON tpo.cod_projeto = tcpa.codigo_projeto
	                    AND tpo.tb_org_id = tcpa.tb_org_id 
                    INNER JOIN tb_cliente_org tcor
	                    ON tcor.codigo_cliente = tpo.cod_cliente
	                    AND tcor.tb_org_id = tcpa.tb_org_id 
                    WHERE tgep.tb_org_id = @orgIdUsuarioLogado
                    GROUP BY tsl.id
                    ORDER BY MAX(tsl.data_criacao) DESC
                    LIMIT @limit OFFSET @cursor;
                ";

                var itens = await connection.QueryAsync<LogDetalhadoDashboardMinhaJornada>(
                    query,
                    new { limit, cursor, orgIdUsuarioLogado }
                );

                return itens.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Erro ao retornar skills detalhadas do dashboard de minha jornada. ERRO: {ex.Message}",
                    ex);
            }
        }

        public async Task<List<dynamic>> BuscaRelatorioLogDetalhado(DateTime dataInicio, DateTime dataFim, int orgIdUsuarioLogado)
        {
            try
            {
                var connection = _dapperConnection.GetConnection();

                var query = @"
                    SELECT
                        MAX(tc2.nome_completo)  AS NomeColaborador,
                        MAX(tcor.nome_cliente)  AS NomeCliente,
                        MAX(tgep.nome_perfil)   AS NomePerfil,
                        COALESCE(
                            MAX(tc.descricao),
                            MAX(tm.descricao),
                            MAX(td.descricao),
                            MAX(ts.descricao),
                            MAX(ti2.descricao),
                            'DESCONHECIDO'
                        ) AS NomeSkill,
                        MAX(tip.descricao) AS TipoSkill,
                        MAX(tn.descricao)  AS Senioridade,
                        MAX(CASE tsm.movimentacao
                            WHEN 'ADICIONADO_PERFIL' THEN 'Adicionado Perfil'
                            WHEN 'NAO_INTERESSADO'   THEN 'Rejeitado'
                            WHEN 'ADICIONADO_PDI'    THEN 'Adicionado PDI'
                            WHEN 'SUGERIDO'          THEN 'Sugerido'
                            WHEN 'ATUALIZADO'        THEN 'Atualizado'
                            WHEN 'INTERESSADO'       THEN 'Interessado'
                            ELSE tsm.movimentacao
                        END) AS Evento
                    FROM tb_skills_log tsl
                    LEFT JOIN tb_competencia tc
                        ON tc.id = tsl.skill_id AND tsl.tb_item_perfil_id = 1
                    LEFT JOIN tb_metodologia tm
                        ON tm.id  = tsl.skill_id AND tsl.tb_item_perfil_id = 3
                    LEFT JOIN tb_dominionegocio td
                        ON td.id  = tsl.skill_id AND tsl.tb_item_perfil_id = 4
                    LEFT JOIN tb_softskill ts
                        ON ts.id  = tsl.skill_id AND tsl.tb_item_perfil_id = 8
                    LEFT JOIN tb_idioma ti2 
                        ON ti2.id  = tsl.skill_id AND tsl.tb_item_perfil_id = 9
                    INNER JOIN tb_colaborador tc2
                        ON tc2.codigo_interno_colaborador = tsl.tb_colaborador_codigo_interno_colaborador
                    INNER JOIN tb_gestor_externo_perfil tgep
                        ON tgep.id = tsl.tb_gestor_externo_perfil_id
                    INNER JOIN tb_skills_movimentacao tsm
                        ON tsm.id = tsl.tb_skills_movimentacao_id
                    INNER JOIN tb_item_perfil tip
                        ON tip.id = tsl.tb_item_perfil_id
                    INNER JOIN tb_nivel tn
                        ON tn.id = tsl.tb_nivel_id
                    INNER JOIN tb_colaborador_periodo_alocacao tcpa
	                    ON tcpa.codigo_interno_colaborador = tsl.tb_colaborador_codigo_interno_colaborador 
	                    AND tcpa.data_inicio <= CURDATE()
	                    AND tcpa.data_fim >= CURDATE()
	                    AND tcpa.ativo = 1
                    INNER JOIN tb_perfil_alocacao tpa
	                    ON tpa.tb_colaborador_periodo_alocacao_id = tcpa.id 
	                    AND tpa.tb_gestor_externo_perfil_id = tsl.tb_gestor_externo_perfil_id 
                    INNER JOIN tb_projeto_org tpo
	                    ON tpo.cod_projeto = tcpa.codigo_projeto
	                    AND tpo.tb_org_id = tcpa.tb_org_id 
                    INNER JOIN tb_cliente_org tcor
	                    ON tcor.codigo_cliente = tpo.cod_cliente
	                    AND tcor.tb_org_id = tcpa.tb_org_id 
                    WHERE tgep.tb_org_id = @orgIdUsuarioLogado
	                      AND tsl.data_criacao >= @dataInicio
	                      AND tsl.data_criacao < DATE_ADD(@dataFim, INTERVAL 1 DAY)
                    GROUP BY tsl.id
                    ORDER BY MAX(tsl.data_criacao) DESC;
                ";

                var itens = await connection.QueryAsync<dynamic>(
                    query,
                    new { dataInicio, dataFim, orgIdUsuarioLogado }
                );

                return itens.ToList();
            } 
            catch (Exception ex)
            {
                throw new Exception(
                    $"Erro ao retornar skills detalhadas do dashboard de minha jornada. ERRO: {ex.Message}",
                    ex);
            }
        }
    }
}
