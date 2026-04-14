using Colaboracao.Core.Interfaces;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Dapper;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Usuario;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.MapaAlocacao.GestaoDeAlocados
{
    public class MinhaJornadaRepository : IMinhaJornadaRepository
    {
        private readonly IDBConnection _dapperConnection;
        private IConnectionStringCore _connectionString;
        private readonly UsuarioLogadoDTO _usuarioLogado;
        public MinhaJornadaRepository(IDBConnection dapperConnection, IConnectionStringCore connectionString, IAspNetUser aspNetUser)
        {
            _dapperConnection = dapperConnection;
            _connectionString = connectionString;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        // ───────────────────────────────────────────────
        // Busca Colaborador e suas Skills
        // ───────────────────────────────────────────────
        public async Task<List<MinhaJornadaColaboradorDTO>> BuscarSkillColaborador(string codColaborador)
        {
            var connection = _dapperConnection.GetConnection();

            const string query = @"
                                SELECT 
                                    tc.codigo_interno_colaborador AS CodigoInternoColaborador,
                                    tc.nome_completo AS NomeCompleto,
                                    hab.tipo_id AS PerfilTipoId,
                                    hab.tipo AS TipoPerfil, 
                                    hab.id AS SkillId,    
                                    hab.descricao AS Habilidade,
                                    hab.nivel_id AS SenioridadeId,
                                    hab.nivel AS Senioridade,
                                    CASE 
                                        WHEN tbi.codigo_interno_colaborador IS NULL THEN 1
                                        WHEN tbi.ativo = 0 THEN 0
                                        ELSE 1 
                                    END AS Interesse
                                FROM 
                                    tb_colaborador tc
                                LEFT JOIN 
                                (
                                    SELECT
                                        tc.id AS id,
                                        c.codigo_interno_colaborador AS codigo_interno_colaborador,
                                        n.id AS nivel_id,
                                        n.descricao AS nivel,
                                        1 AS tipo_id,
                                        'Competencia' AS tipo,
                                        tc.descricao AS descricao
                                    FROM tb_colaborador_competencia c
                                    JOIN tb_nivel n ON c.tb_nivel_id = n.id
                                    JOIN tb_competencia tc ON c.competencia_id = tc.id
                                    WHERE c.ativo = 1
                                    AND c.codigo_interno_colaborador =  @codColaborador

                                    UNION ALL

                                    SELECT
                                        tm.id AS id,
                                        m.codigo_interno_colaborador AS codigo_interno_colaborador,
                                        n.id AS nivel_id,
                                        n.descricao AS nivel,
                                        3 AS tipo_id,
                                        'Metodologia' AS tipo,
                                        tm.descricao AS descricao
                                    FROM tb_colaborador_metodologia m
                                    JOIN tb_nivel n ON m.tb_nivel_id = n.id
                                    JOIN tb_metodologia tm ON m.metodologia_id = tm.id
                                    WHERE m.ativo = 1
                                    AND m.codigo_interno_colaborador =  @codColaborador

                                    UNION ALL

                                    SELECT
                                        ti.id AS id,
                                        tci.codigo_interno_colaborador AS codigo_interno_colaborador,
                                        n.id AS nivel_id,
                                        n.descricao AS nivel,
                                        9 AS tipo_id,
                                        'Idioma' AS tipo,
                                        ti.descricao AS descricao
                                    FROM tb_colaborador_idioma tci
                                    JOIN tb_nivel n ON tci.tb_nivel_id = n.id
                                    JOIN tb_idioma ti ON tci.idioma_id = ti.id
                                    WHERE tci.ativo = 1
                                    AND tci.codigo_interno_colaborador =  @codColaborador

                                    UNION ALL

                                    SELECT
                                        ts.id AS id,
                                        tcs.codigo_interno_colaborador AS codigo_interno_colaborador,
                                        n.id AS nivel_id,
                                        n.descricao AS nivel,
                                        8 AS tipo_id,
                                        'Softskill' AS tipo,
                                        ts.descricao AS descricao
                                    FROM tb_colaborador_softskill tcs
                                    JOIN tb_nivel n ON tcs.tb_nivel_id = n.id
                                    JOIN tb_softskill ts ON tcs.softskill_id = ts.id
                                    WHERE tcs.ativo = 1
                                     AND tcs.codigo_interno_colaborador =  @codColaborador
                                    UNION ALL

                                    SELECT
                                        tf.id AS id,
                                        tcf.codigo_interno_colaborador AS codigo_interno_colaborador,
                                        n.id AS nivel_id,
                                        n.descricao AS nivel,
                                        2 AS tipo_id,
                                        'Formacao' AS tipo,
                                        tf.descricao AS descricao
                                    FROM tb_colaborador_formacao tcf
                                    JOIN tb_nivel n ON tcf.tb_nivel_id = n.id
                                    JOIN tb_formacao tf ON tcf.formacao_id = tf.id
                                    WHERE tcf.ativo = 1
                                    AND tcf.codigo_interno_colaborador =  @codColaborador

                                    UNION ALL

                                    SELECT
                                        td.id AS id,
                                        tcd.codigo_interno_colaborador AS codigo_interno_colaborador,
                                        n.id AS nivel_id,
                                        n.descricao AS nivel,
                                        4 AS tipo_id,
                                        'Domínio' AS tipo,
                                        td.descricao AS descricao
                                    FROM tb_colaborador_dominionegocio tcd
                                    JOIN tb_nivel n ON tcd.tb_nivel_id = n.id
                                    JOIN tb_dominionegocio td ON tcd.dominionegocio_id = td.id
                                    WHERE tcd.ativo = 1
                                    AND tcd.codigo_interno_colaborador = @codColaborador
                                ) hab 
                                    ON tc.codigo_interno_colaborador = hab.codigo_interno_colaborador
                                LEFT JOIN 
                                    tb_colaborador_interesse tbi 
                                        ON tbi.codigo_interno_colaborador = hab.codigo_interno_colaborador 
                                       AND tbi.skill_id = hab.id
                                       AND tbi.tipo_id = hab.tipo_id
                                WHERE 
                                    tc.codigo_interno_colaborador = @codColaborador    
                                    AND tc.ativo = 1; ";

            var colaboradorDictionary = new Dictionary<string, MinhaJornadaColaboradorDTO>();

            // Processa as linhas e preenche o dicionário.
            await connection.QueryAsync<MinhaJornadaColaboradorDTO, HabilidadesColaboradorDTO, MinhaJornadaColaboradorDTO>(
                sql: query,
                map: (colaborador, habilidade) =>
                {
                    if (!colaboradorDictionary.TryGetValue(colaborador.CodigoInternoColaborador, out var colaboradorAtual))
                    {
                        colaboradorAtual = colaborador;
                        colaboradorAtual.ColaboradorHabilidades = new List<HabilidadesColaboradorDTO>();
                        colaboradorDictionary.Add(colaboradorAtual.CodigoInternoColaborador, colaboradorAtual);
                    }

                    if (habilidade != null && habilidade.SkillId != null)
                    {
                        // Preenche manualmente o código do colaborador no objeto de habilidade.
                        habilidade.CodigoInternoColaborador = colaborador.CodigoInternoColaborador;

                        colaboradorAtual.ColaboradorHabilidades.Add(habilidade);
                    }

                    return colaboradorAtual;
                },
                param: new { codColaborador },
                splitOn: "PerfilTipoId"
            );

            // Como a busca é por um único 'codColaborador', esta lista conterá 0 ou 1 item.
            return colaboradorDictionary.Values.ToList();
        }

        // ───────────────────────────────────────────────
        // Busca Skills dos Colaboradores Alocados
        // ───────────────────────────────────────────────

        public async Task<List<MinhaJornadaDTO>> BuscarSkillColaboradorAlocado(string codColaborador, int orgId)
        {
            //var connection = _dapperConnection.GetConnection();
            var connection = _dapperConnection.GetConnection();

            const string query = @"
                                WITH 
                                CTE_Gestores AS (
                                    SELECT
                                        tco.cod_colaborador_externo AS cod_colaborador_externo_subordinado,
                                        tc.codigo_interno_colaborador AS codigo_interno_colaborador_subordinado,
                                        tc.nome_completo AS nome_completo_subordinado,
                                        tch.cod_colaborador_superior AS cod_colaborador_externo_gestor,
                                        tc_gerente.codigo_interno_colaborador AS codigo_interno_colaborador_gestor,
                                        tc_gerente.nome_completo AS nome_completo_gestor,
                                        tch.tb_org_id AS tb_org_id
                                    FROM tb_colaborador_hierarquia tch
                                    INNER JOIN tb_colaborador_org tco 
                                        ON tch.cod_colaborador_externo = tco.cod_colaborador_externo 
                                       AND tch.tb_org_id = tco.tb_org_id
                                    INNER JOIN tb_colaborador tc 
                                        ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador
                                    INNER JOIN tb_colaborador_org tco_gerente 
                                        ON tco_gerente.cod_colaborador_externo = tch.cod_colaborador_superior 
                                       AND tch.tb_org_id = tco_gerente.tb_org_id
                                    INNER JOIN tb_colaborador tc_gerente 
                                        ON tc_gerente.codigo_interno_colaborador = tco_gerente.codigo_interno_colaborador
                                    WHERE tco.codigo_interno_colaborador = @codColaborador
                                      AND tch.tb_org_id = @orgId
                                ),

                                CTE_Perfis AS (
                                    SELECT
                                        tp.nome_perfil AS perfil,
                                        tp.tb_org_id AS tb_org_id,
                                        CONCAT('1|', tp.id) AS id,
                                        tpo.cod_cliente AS codigo_cliente,
                                        1 AS ativo
                                    FROM tb_perfil tp
                                    INNER JOIN tb_projeto_org tpo 
                                        ON tpo.tb_org_id = tp.tb_org_id 
                                       AND tpo.cod_projeto = tp.codigo_projeto
                                    WHERE tp.tb_org_id = @orgId

                                    UNION ALL

                                    SELECT
                                        CONCAT(tgep.nome_perfil, ' / ', tge.nome) AS perfil,
                                        tgep.tb_org_id AS tb_org_id,
                                        CONCAT('2|', tgep.id) AS id,
                                        tge.codigo_cliente AS codigo_cliente,
                                        tgep.ativo AS ativo
                                    FROM tb_gestor_externo_perfil tgep
                                    INNER JOIN tb_gestor_externo tge 
                                        ON tge.tb_org_id = tgep.tb_org_id 
                                       AND tge.cod_gestor_externo = tgep.cod_gestor_externo
                                    WHERE tgep.tb_org_id = @orgId
                                    AND tgep.ativo = 1
                                    AND tge.ativo = 1
                                ),

                                CTE_Alocacoes AS (
                                    SELECT 
                                        tcp.id,
                                        tcp.codigo_interno_colaborador,
                                        tcp.codigo_projeto,
                                        tcp.tb_org_id
                                    FROM tb_colaborador_periodo_alocacao tcp
                                    WHERE tcp.codigo_interno_colaborador = @codColaborador
                                      AND tcp.tb_org_id = @orgId
                                      AND tcp.ativo = 1
                                      AND tcp.data_fim >= CURDATE()
                                      AND tcp.id > 0
                                ),

                                CTE_Perfis_Alocacao AS (
                                    SELECT 
                                        CASE 
                                            WHEN LOCATE('|', vp.id) = 0 THEN vp.id 
                                            ELSE SUBSTRING_INDEX(vp.id, '|', -1) 
                                        END AS perfilid,
                                        vp.perfil, 
                                        tpa.tb_colaborador_periodo_alocacao_id, 
                                        tge.cod_gestor_externo AS CodGestorCliente, 
                                        tge.nome AS NomeGestorCliente
                                    FROM tb_perfil_alocacao tpa  
                                    INNER JOIN CTE_Alocacoes ca 
                                        ON ca.id = tpa.tb_colaborador_periodo_alocacao_id
                                    INNER JOIN CTE_Perfis vp 
                                        ON vp.tb_org_id = tpa.tb_org_id 
                                       AND (vp.id = CONCAT('1|', tpa.tb_perfil_id) OR vp.id = CONCAT('2|', tpa.tb_gestor_externo_perfil_id))
                                    LEFT JOIN tb_gestor_externo_perfil tgep 
                                        ON tpa.tb_gestor_externo_perfil_id = tgep.id 
                                       AND tgep.ativo = 1
                                    LEFT JOIN tb_gestor_externo tge 
                                        ON tgep.cod_gestor_externo = tge.cod_gestor_externo 
                                       AND tgep.tb_org_id = tge.tb_org_id
                                    WHERE tpa.tb_org_id = @orgId
                                ),

                                CTE_Skills_Alocacao AS (
                                    SELECT 
                                        tcas.tb_colaborador_periodo_alocacao_id, 
                                        tip.id AS PerfilTipoId, 
                                        tip.descricao AS TipoPerfil, 
                                        tbs.skill_id AS SkillId, 
                                        COALESCE(
											    tcco.descricao,
											    tccs.descricao,
											    tccm.descricao,
											    tccn.descricao,
											    tcci.descricao,
                                                tccd.descricao
											) AS Habilidade, 
                                        tn.id AS SenioridadeId, 
                                        tn.descricao AS Senioridade,
                                        CASE 
                                            WHEN tbi.codigo_interno_colaborador IS NULL THEN 1 
                                            WHEN tbi.ativo = 0 THEN 0 
                                            ELSE 1 
                                        END AS Interesse
                                    FROM  tb_perfil_alocacao tcas
                                    INNER JOIN tb_gestor_externo_perfil_skill tbs on tbs.tb_gestor_externo_perfil_id = tcas.tb_gestor_externo_perfil_id
                                    INNER JOIN CTE_Alocacoes ca            ON ca.id = tcas.tb_colaborador_periodo_alocacao_id
                                    LEFT JOIN  tb_competencia tcco         ON tcco.id = tbs.skill_id AND tbs.tb_item_perfil_id = 1  AND tcco.ativo = 1
                                    LEFT JOIN  tb_softskill tccs           ON tccs.id = tbs.skill_id AND tbs.tb_item_perfil_id = 8  AND tccs.ativo = 1
                                    LEFT JOIN  tb_metodologia tccm         ON tccm.id = tbs.skill_id AND tbs.tb_item_perfil_id = 3  AND tccm.ativo = 1
                                    LEFT JOIN  tb_dominionegocio tccn      ON tccn.id = tbs.skill_id AND tbs.tb_item_perfil_id = 4  AND tccn.ativo = 1
                                    LEFT JOIN  tb_idioma tcci              ON tcci.id = tbs.skill_id AND tbs.tb_item_perfil_id = 9  AND tcci.ativo = 1
  									LEFT JOIN  tb_skill_desconhecida tccd  ON tccd.id = tbs.skill_id AND tbs.tb_item_perfil_id = 14 AND tccd.ativo = 1
                                    LEFT JOIN tb_nivel tn                  ON tn.id   = tbs.tb_nivel_id AND tn.ativo = 1
                                    LEFT JOIN tb_item_perfil tip           ON tip.id  = tbs.tb_item_perfil_id AND tip.ativo = 1
                                    LEFT JOIN tb_colaborador_interesse tbi ON tbi.codigo_interno_colaborador = @codColaborador
                                                                           AND tbi.tipo_id  = tip.id 
                                                                           AND tbi.skill_id = tbs.skill_id
                                )

                                SELECT 
                                    vgco.codigo_interno_colaborador_gestor AS CodigoInternoGestorAdm,
                                    vgco.nome_completo_gestor AS NomeGestorAdm,
                                    tc.codigo_interno_colaborador AS CodigoInternoColaborador, 
                                    tc.nome_completo AS NomeCompleto, 
                                    ca.codigo_projeto AS CodigoProjeto, 
                                    ca.id AS IdAlocacao, 
                                    tcor.codigo_cliente AS CodigoCliente,
                                    tcor.nome_cliente AS NomeCliente, 
                                    perf.perfilid AS PerfilId, 
                                    perf.perfil AS Perfil,
                                    perf.CodGestorCliente, 
                                    perf.NomeGestorCliente, 
                                    skill.PerfilTipoId, 
                                    skill.TipoPerfil,
                                    skill.SkillId, 
                                    skill.Habilidade, 
                                    skill.SenioridadeId, 
                                    skill.Senioridade, 
                                    skill.Interesse
                                FROM CTE_Alocacoes ca
                                INNER JOIN tb_colaborador tc 
                                    ON tc.codigo_interno_colaborador = ca.codigo_interno_colaborador
                                    AND tc.ativo = 1
                                LEFT JOIN CTE_Gestores vgco 
                                    ON ca.codigo_interno_colaborador = vgco.codigo_interno_colaborador_subordinado                                     
                                LEFT JOIN tb_colaborador_cargo tcc 
                                    ON tcc.codigo_interno_colaborador = tc.codigo_interno_colaborador 
                                   AND tcc.ativo = 1
                                LEFT JOIN tb_projeto_org tpo 
                                    ON tpo.cod_projeto = ca.codigo_projeto 
                                   AND tpo.tb_org_id = ca.tb_org_id
                                LEFT JOIN tb_cliente_org tcor 
                                    ON tpo.cod_cliente = tcor.codigo_cliente 
                                   AND tcor.tb_org_id = ca.tb_org_id 
                                   AND tcor.ativo = 1
                                LEFT JOIN CTE_Perfis_Alocacao perf 
                                    ON perf.tb_colaborador_periodo_alocacao_id = ca.id
                                LEFT JOIN CTE_Skills_Alocacao skill 
                                    ON skill.tb_colaborador_periodo_alocacao_id = ca.id
                                WHERE perf.perfilid IS NOT NULL 
                                ORDER BY ca.id, tcor.codigo_cliente;   ";

            // Constrói um dicionário onde cada entrada é uma alocação (MinhaJornadaDTO) totalmente preenchida.
            var jornadaDictionary = new Dictionary<int, MinhaJornadaDTO>();
            var clienteDictionary = new Dictionary<string, ClientesAlocacaoDTO>();

            await connection.QueryAsync<MinhaJornadaDTO, ClientesAlocacaoDTO, HabilidadesDTO, MinhaJornadaDTO>(
                query,
                (jornada, cliente, habilidade) =>
                {
                    if (!jornadaDictionary.TryGetValue(jornada.IdAlocacao, out var jornadaAtual))
                    {
                        jornadaAtual = jornada;
                        jornadaAtual.Clientes = new List<ClientesAlocacaoDTO>();
                        jornadaDictionary.Add(jornadaAtual.IdAlocacao, jornadaAtual);
                    }
                    if (cliente != null)
                    {
                        var chaveCliente = $"{jornada.IdAlocacao}-{cliente.CodigoCliente}";
                        if (!clienteDictionary.TryGetValue(chaveCliente, out var clienteAtual))
                        {
                            clienteAtual = cliente;
                            clienteAtual.Habilidades = new List<HabilidadesDTO>();
                            clienteDictionary.Add(chaveCliente, clienteAtual);
                            jornadaAtual.Clientes.Add(clienteAtual);
                        }
                        if (habilidade != null && habilidade.SkillId != null && !clienteAtual.Habilidades.Any(h => h.SkillId == habilidade.SkillId))
                        {
                            habilidade.CodigoCliente = clienteAtual.CodigoCliente;
                            clienteAtual.Habilidades.Add(habilidade);
                        }
                    }
                    return jornadaAtual;
                },
                new { codColaborador, orgId },
                splitOn: "CodigoCliente,PerfilTipoId"
            );

            // 'jornadaDictionary.Values' contém a coleção de objetos 'MinhaJornadaDTO',
            // um para cada alocação, com todos os campos (IdAlocacao, CodigoProjeto, Clientes) preenchidos.
            return jornadaDictionary.Values.ToList();
        }

        // ───────────────────────────────────────────────
        // ──────── Sugestão ─────────────────────────────
        // ───────────────────────────────────────────────
        public async Task<SugestaoSkilleHistoricoOrdemResponseDTO> InserirSugestao(SugestaoParamDTO param)
        {
            var connection = _dapperConnection.GetConnection();

            var skillExiste = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM vw_skills WHERE tipo_id = @TipoId AND id = @Skillid",
                new { TipoId = param.Tipo_Id, Skillid = param.Skill_Id });

            if (skillExiste == 0)
                throw new Exception($"Skill {param.Skill_Id}, Tipo {param.Tipo_Id} não encontrado !");

            var senioridadeExiste = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM tb_nivel WHERE id = @SenioridadeId",
                new { SenioridadeId = param.Senioridade_Id });

            if (senioridadeExiste == 0)
                throw new Exception($"Senioridade {param.Senioridade_Id} não encontrado !");

            var clienteExiste = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM tb_cliente_org WHERE codigo_cliente = @CodigoCliente",
                new { CodigoCliente = param.CodigoCliente });

            if (clienteExiste == 0)
                throw new Exception($"Cliente {param.CodigoCliente} não encontrado !");

            var sugestaoExiste = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM tb_colaborador_sugestao WHERE tipo_id = @TipoId AND skill_id = @Skillid AND codigo_interno_colaborador = @CodigoInternoColaborador",
                new { TipoId = param.Tipo_Id, Skillid = param.Skill_Id, CodigoInternoColaborador = param.CodigoInternoColaborador });

            if (sugestaoExiste > 0)
                throw new Exception($"Sugestão já Cadastrado !");

            var sugestaoId = Guid.NewGuid().ToString();
            var historicoId = Guid.NewGuid().ToString();
            var usuarioLogado = _usuarioLogado.Cpf;

            var insertSql = @"
                INSERT INTO 
                    tb_colaborador_sugestao (id, codigo_interno_colaborador, codigo_interno_colaborador_sugeriu, codigo_gestor_adm, codigo_cliente, tipo_id, skill_id, senioridade_id, perfil_id, data)
                VALUES 
                    (@Id, @CodigoInternoColaborador, @CodigoInternoColaboradorSugeriu, @CodigoGestorAdm, @CodigoCliente, @Tipo_Id, @Skill_Id, @Senioridade_Id, @Perfil_Id, NOW());
                ";

            try
            {
                var parametros = new
                {
                    Id = sugestaoId.ToString(),
                    CodigoInternoColaborador = param.CodigoInternoColaborador,
                    CodigoInternoColaboradorSugeriu = usuarioLogado,
                    CodigoGestorAdm = param.CodigoGestorAdm,
                    //CodigoGestorOper = "", // param.CodigoGestorOper,
                    CodigoCliente = param.CodigoCliente,
                    Tipo_Id = param.Tipo_Id,
                    Skill_Id = param.Skill_Id,
                    Senioridade_Id = param.Senioridade_Id,
                    Perfil_Id = param.Perfil_Id
                };

                await connection.ExecuteAsync(insertSql, parametros);

                //Inclui um item no Historico

                var query = @"
                            INSERT INTO 
                                tb_historico_sugestao (id, codigo_interno_colaborador_avaliador, tb_colaborador_sugestao_id, aprovado, tb_status_sugestao_id, codigo_interno_colaborador, perfil_id, observacao, data)
                            VALUES 
                                (@Id, @CodigoInternoColaboradorAvaliador, @SugestaoId, @Aprovado, @TbStatusSugestaoId, @CodigoInternoColaborador, @Perfil_Id, @Observacao, NOW());
                            ";

                var parametrosHistorico = new
                {
                    Id = historicoId.ToString(),
                    CodigoInternoColaboradorAvaliador = usuarioLogado,
                    SugestaoId = sugestaoId,
                    Aprovado = 0,
                    TbStatusSugestaoId = 2,
                    CodigoInternoColaborador = param.CodigoInternoColaborador,
                    Perfil_Id = param.Perfil_Id,
                    Observacao = "",
                    Data = param.Data
                };

                await connection.ExecuteAsync(query, parametrosHistorico);

            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao inserir sugestão: {ex.Message}", ex);
            }

            //return await BuscarSugestaoPorId(sugestaoId); 
            return await BuscarSugestaoHistoricoPorId(sugestaoId);

        }

        public async Task<SugestaoSkillResponseDTO> AtualizarSugestao(SugestaoAtualizacaoParamDTO param)
        {
            var connection = _dapperConnection.GetConnection();

            var skillId = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM vw_skills WHERE tipo_id = @TipoId AND id = @Skillid",
                new { TipoId = param.Tipo_Id, Skillid = param.Skill_Id });

            if (skillId == 0)
                throw new Exception($"Skill {param.Tipo_Id}, Tipo {param.Tipo_Id} não encontrado !");

            var seniorId = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM tb_nivel WHERE id = @SenioridadeId",
                new { SenioridadeId = param.Senioridade_Id });

            if (seniorId == 0)
                throw new Exception($"Senioridade {param.Senioridade_Id} não encontrado !");

            var clienteExiste = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM tb_cliente_org WHERE codigo_cliente = @CodigoCliente",
                new { CodigoCliente = param.CodigoCliente });

            if (clienteExiste == 0)
                throw new Exception($"Cliente {param.CodigoCliente} não encontrado !");

            // ─── Atualiza Sugestão ───
            if (param.Id != null)
            {
                await connection.ExecuteAsync(@"
                                            UPDATE tb_colaborador_sugestao SET
                                                codigo_gestor_adm = @CodigoGestorAdm,
                                                codigo_cliente = @CodigoCliente,
                                                tipo_id = @Tipo_Id,
                                                skill_id = @Skill_Id,
                                                perfil_id = @Perfil_Id,
                                                senioridade_id = @Senioridade_Id
                                            WHERE id = @Id;",
                    new
                    {
                        param.Id,
                        param.CodigoGestorAdm,
                        //param.CodigoGestorOper,
                        param.CodigoCliente,
                        param.Tipo_Id,
                        param.Skill_Id,
                        param.Perfil_Id,
                        param.Senioridade_Id
                    });

            }
            return await BuscarSugestaoPorId(param.Id.ToString());
        }

        public async Task<List<SugestaoHistoricoDTO>> AprovarRejeitarSugestao(SugestaoHistoricoParamDTO param)
        {
            var connection = _dapperConnection.GetConnection();
            var usuarioLogado = _usuarioLogado.Cpf;
            var Id = Guid.NewGuid().ToString();

            // ─── Atualiza Sugestão ───
            if (param.SugestaoId != null)
            {
                var query = @"
                INSERT INTO 
                    tb_historico_sugestao (id, codigo_interno_colaborador_avaliador, tb_colaborador_sugestao_id, aprovado, tb_status_sugestao_id, codigo_interno_colaborador, perfil_id, observacao, data)
                VALUES 
                    (@Id, @CodigoInternoColaboradorAvaliador, @SugestaoId, @Aprovado, @TbStatusSugestaoId, @CodigoInternoColaborador, @Perfil_Id, @Observacao, NOW());
                ";

                var parametros = new
                {
                    Id = Id.ToString(),
                    CodigoInternoColaboradorAvaliador = usuarioLogado,
                    param.SugestaoId,
                    param.Aprovado,
                    param.TbStatusSugestaoId,
                    param.CodigoInternoColaborador,
                    param.Perfil_Id,
                    param.Observacao,
                    param.Data
                };

                await connection.ExecuteAsync(query, parametros);
            }

            return await BuscarHistoricoPorId(Id.ToString());
        }

        public async Task<bool> DeletarSugestao(string sugestaoId)
        {
            var connection = _dapperConnection.GetConnection();
            const string query = @"
                    DELETE FROM tb_colaborador_sugestao 
                           WHERE id = @ID;";

            var ok = await connection.ExecuteAsync(query, new { ID = sugestaoId });
            return ok > 0;
        }
        public async Task<SugestaoSkillResponseDTO> BuscarSugestaoPorId(string sugestaoId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                    SELECT 
                        tbs.id as ID,
                        tbs.codigo_interno_colaborador as CodigoInternoColaborador,
                        tbs.codigo_interno_colaborador_sugeriu as CodigoInternoColaboradorSugeriu,  
                        tbs.codigo_gestor_adm as CodigoGestorAdm,
                        tbs.codigo_cliente as CodigoCliente,
                        tbs.tipo_id as Tipo_Id,
                        CASE WHEN tbs.tipo_id = 1 THEN 'Competencia'
	                         WHEN tbs.tipo_id = 2 THEN 'Formacao'
	                         WHEN tbs.tipo_id = 3 THEN 'Metodologia'
	                         WHEN tbs.tipo_id = 4 THEN 'DominioNegocio'
	                         WHEN tbs.tipo_id = 8 THEN 'SoftSkill'
	                         WHEN tbs.tipo_id = 9 THEN 'Idioma'
							 ELSE 'DESCONHECIDO'
						END AS DescricaoTipo,
                        tbs.skill_id as Skill_Id, 
                        vws.descricao AS DescricaoSkill,
                        tbs.senioridade_id as Senioridade_Id,
                        tbn.descricao AS Senioridade,
                        tbs.perfil_id as Perfil_Id,
                        tbs.data as Data,
                        tbs.ativo as Ativo
                    FROM tb_colaborador_sugestao tbs
                    LEFT JOIN vw_skills vws on vws.id = tbs.skill_id and vws.tipo_id = tbs.tipo_id
                    LEFT JOIN tb_nivel tbn ON tbs.senioridade_id = tbn.id AND tbn.ativo = 1 
                    WHERE tbs.id = @Id; ";

            var parametros = new
            {
                Id = sugestaoId
            };

            var result = await connection.QueryFirstOrDefaultAsync<SugestaoSkillResponseDTO>(query, parametros);
            return result;
        }

        public async Task<List<SugestaoSkilleHistoricoResponseDTO>> BuscarSugestaoPorCodColaboradorOuAdm(string codInternoColaborador, string codInternoGestor, string perfilId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                       SELECT 
                        tbs.id as ID,
                        tbs.codigo_interno_colaborador as CodigoInternoColaborador,
                        tbs.codigo_interno_colaborador_sugeriu as CodigoInternoColaboradorSugeriu,  
                        tbs.codigo_gestor_adm as CodigoGestorAdm,
                        tbs.codigo_cliente as CodigoCliente,
                        tbs.tipo_id as Tipo_Id,
                        CASE WHEN tbs.tipo_id = 1 THEN 'Competencia'
	                         WHEN tbs.tipo_id = 2 THEN 'Formacao'
	                         WHEN tbs.tipo_id = 3 THEN 'Metodologia'
	                         WHEN tbs.tipo_id = 4 THEN 'DominioNegocio'
	                         WHEN tbs.tipo_id = 8 THEN 'SoftSkill'
	                         WHEN tbs.tipo_id = 9 THEN 'Idioma'
							 ELSE 'DESCONHECIDO'
						END AS DescricaoTipo,
                        tbs.skill_id as Skill_Id, 
                        vws.descricao AS DescricaoSkill,
                        tbs.senioridade_id as Senioridade_Id,
                        tbn.descricao AS Senioridade,
                        tbs.perfil_id as Perfil_Id,
                        tbs.data as Data,
                        tbs.ativo as Ativo
                    FROM tb_colaborador_sugestao tbs
                    LEFT JOIN vw_skills vws on vws.id = tbs.skill_id and vws.tipo_id = tbs.tipo_id
                    LEFT JOIN tb_nivel tbn ON tbs.senioridade_id = tbn.id AND tbn.ativo = 1
                    WHERE 1 = 1
                    AND (tbs.codigo_interno_colaborador = @CodInternoColaborador OR @CodInternoColaborador IS NULL OR @CodInternoColaborador = '') 
                    AND (tbs.codigo_gestor_adm = @CodInternoGestor OR @CodInternoGestor IS NULL OR @CodInternoGestor = '')
                    AND (tbs.perfil_id = @PerfilId OR @PerfilId IS NULL OR @PerfilId = ''); ";

            var parametros = new
            {
                CodInternoColaborador = codInternoColaborador,
                CodInternoGestor = codInternoGestor,
                PerfilId = perfilId
            };

            var sugestoes = (await connection.QueryAsync<SugestaoSkilleHistoricoResponseDTO>(query, parametros)).ToList();

            if (sugestoes == null || sugestoes.Count == 0)
                return sugestoes;

            // Query de histórico
            const string queryHistorico = @"
                                    SELECT
                                        id AS Id,
                                        codigo_interno_colaborador_avaliador AS CodigoInternoColaboradorAvaliador,
                                        aprovado AS Aprovado,
                                        tb_status_sugestao_id AS TbStatusSugestaoId,
                                        codigo_interno_colaborador AS CodigoInternoColaborador,
                                        tb_colaborador_sugestao_id AS ColaboradorSugestaoId,
                                        perfil_id AS Perfil_Id,
                                        observacao AS Observacao,
                                        data AS Data
                                    FROM tb_historico_sugestao
                                    WHERE tb_colaborador_sugestao_id = @SugestaoId
                                    ORDER BY data DESC;";

            // Carregar o histórico para cada item da lista
            foreach (var sug in sugestoes)
            {
                var historicos = await connection.QueryAsync<SugestaoHistoricoDTO>(
                    queryHistorico,
                    new { SugestaoId = sug.Id });

                sug.HistoricoSugestao = historicos.ToList();
            }

            return sugestoes;
        }

        public async Task<List<SugestaoHistoricoDTO>> BuscarHistoricoPorId(string sugestaoId)
        {
            var connection = _dapperConnection.GetConnection();

            const string queryHistorico = @"
                                    SELECT
                                        id AS Id,
                                        codigo_interno_colaborador_avaliador AS CodigoInternoColaboradorAvaliador,
                                        codigo_interno_colaborador AS CodigoInternoColaborador,
                                        aprovado AS Aprovado,
                                        tb_status_sugestao_id AS TbStatusSugestaoId,
                                        perfil_id AS Perfil_Id,
                                        observacao AS Observacao,
                                        data AS Data
                                    FROM 
                                        tb_historico_sugestao
                                    WHERE 
                                        id = @SugestaoId
                                    ORDER BY data DESC;";

            var historicos = await connection.QueryAsync<SugestaoHistoricoDTO>(
                queryHistorico, new { SugestaoId = sugestaoId });

            return historicos.ToList();
        }

        public async Task<SugestaoSkilleHistoricoOrdemResponseDTO> BuscarSugestaoHistoricoPorId(string sugestaoId)
        {
            var connection = _dapperConnection.GetConnection();

            // Consulta da tabela pai: tb_colaborador_sugestao
            const string querySugestao = @"
                                           SELECT 
                                            tbs.id as ID,
                                            tbs.codigo_interno_colaborador as CodigoInternoColaborador,
                                            tbs.codigo_interno_colaborador_sugeriu as CodigoInternoColaboradorSugeriu,  
                                            tbs.codigo_gestor_adm as CodigoGestorAdm,
                                            tbs.codigo_cliente as CodigoCliente,
                                            tbs.tipo_id as Tipo_Id,
                                            CASE WHEN tbs.tipo_id = 1 THEN 'Competencia'
	                                             WHEN tbs.tipo_id = 2 THEN 'Formacao'
	                                             WHEN tbs.tipo_id = 3 THEN 'Metodologia'
	                                             WHEN tbs.tipo_id = 4 THEN 'DominioNegocio'
	                                             WHEN tbs.tipo_id = 8 THEN 'SoftSkill'
	                                             WHEN tbs.tipo_id = 9 THEN 'Idioma'
							                     ELSE 'DESCONHECIDO'
						                    END AS DescricaoTipo,
                                            tbs.skill_id as Skill_Id, 
                                            vws.descricao AS DescricaoSkill,
                                            tbs.senioridade_id as Senioridade_Id,
                                            tbn.descricao AS Senioridade,
                                            tbs.perfil_id as Perfil_Id,
                                            tbs.data as Data,
                                            tbs.ativo as Ativo
                                        FROM tb_colaborador_sugestao tbs
                                        LEFT JOIN vw_skills vws on vws.id = tbs.skill_id and vws.tipo_id = tbs.tipo_id
                                        LEFT JOIN tb_nivel tbn ON tbs.senioridade_id = tbn.id AND tbn.ativo = 1
                                        WHERE tbs.id = @SugestaoId;";

            var sugestao = await connection.QueryFirstOrDefaultAsync<SugestaoSkilleHistoricoOrdemResponseDTO>(
                querySugestao, new { SugestaoId = sugestaoId });

            if (sugestao == null)
                return null;

            // Consulta da tabela filho: tb_historico_sugestao
            const string queryHistorico = @"
                                SELECT
                                    id AS Id,
                                    codigo_interno_colaborador_avaliador AS CodigoInternoColaboradorAvaliador,
                                    aprovado AS Aprovado,
                                    tb_status_sugestao_id AS TbStatusSugestaoId,
                                    codigo_interno_colaborador AS CodigoInternoColaborador,
                                    tb_colaborador_sugestao_id AS ColaboradorSugestaoId,
                                    perfil_id AS Perfil_Id,
                                    observacao AS Observacao,
                                    data AS Data
                                FROM 
                                    tb_historico_sugestao
                                WHERE 
                                    tb_colaborador_sugestao_id = @SugestaoId
                                ORDER BY data DESC;";

            var historicos = await connection.QueryAsync<SugestaoHistoricoDTO>(
                queryHistorico, new { SugestaoId = sugestaoId });

            sugestao.HistoricoSugestao = historicos.AsList();

            return sugestao;
        }

    }
}
