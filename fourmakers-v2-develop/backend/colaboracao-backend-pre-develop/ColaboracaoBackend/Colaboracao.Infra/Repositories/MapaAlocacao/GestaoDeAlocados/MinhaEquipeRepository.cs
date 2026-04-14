using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Interfaces;
using Competencia.Domain.Enums;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Dapper;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Match;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.MapaAlocacao.GestaoDeAlocados
{
    public class MinhaEquipeRepository : IMinhaEquipeRepository
    {
        private readonly IDBConnection _dapperConnection;
        private readonly IMinhaJornadaRepository _minhaJornadaRepository;
        private readonly IMatchClient _matchClient;

        public MinhaEquipeRepository(IDBConnection dapperConnection, IMinhaJornadaRepository minhaJornadaRepository, IMatchClient matchClient)
        {
            _dapperConnection = dapperConnection;
            _minhaJornadaRepository = minhaJornadaRepository;
            _matchClient = matchClient;
        }

        // ───────────────────────────────────────────────
        // Busca Liderados por Gestor com Perfil
        // ───────────────────────────────────────────────
        public async Task<List<MinhaEquipeDTO>> BuscarLideradosPorGestor(string codColaborador, string perfilId, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            try
            {
                const string sqlColaborador = @"
                                             SELECT 
                                                vgco.codigo_interno_colaborador_gestor as CodigoInternoColaboradorGestorAdm,
                                                vgco.nome_completo_gestor as NomeGestorAdm,
                                                tc.codigo_interno_colaborador as CodigoInternoColaborador, 
                                                tc.nome_completo as NomeColaborador, 
                                                tcg.descricao as Cargo,
                                                tcp.codigo_projeto as CodigoProjeto, 
                                                tcp.id as IdAlocacao
                                               FROM vw_gestores_colaboradores_org vgco
                                            INNER JOIN tb_colaborador_periodo_alocacao tcp 
                                                ON tcp.codigo_interno_colaborador = vgco.codigo_interno_colaborador_subordinado 
                                            INNER JOIN tb_colaborador tc 
                                                ON tc.codigo_interno_colaborador = tcp.codigo_interno_colaborador
                                            LEFT JOIN tb_colaborador_cargo tcc 
                                                ON tcc.codigo_interno_colaborador = tc.codigo_interno_colaborador
                                            LEFT JOIN tb_cargo tcg 
                                                ON tcg.id = tcc.cargo_id
                                            LEFT JOIN tb_perfil_alocacao tpa 
                                                ON tcp.id = tpa.tb_colaborador_periodo_alocacao_id
                                            WHERE vgco.codigo_interno_colaborador_gestor = @codColaborador
                                              AND vgco.tb_org_id = @OrgId 
                                              AND tcp.tb_org_id = @OrgId  
                                              AND tcp.ativo = 1
                                              AND tc.ativo = 1
                                              AND tcp.data_fim >= CURDATE();";

                var colaboradores = (await connection.QueryAsync<MinhaEquipeDTO>(
                    sqlColaborador,
                    new { codColaborador, OrgId = orgId }
                )).ToList();

                if (!colaboradores.Any())
                    return new List<MinhaEquipeDTO>();

                const string sqlClientes = @"
                                            SELECT  tcor.codigo_cliente AS CodigoCliente, 
	                                                tcor.nome_cliente AS NomeCliente,
                                                    perf.perfilid as PerfilId,
    	                                            perf.perfil AS Perfil,
                                                    perf.CodGestorCliente,
                                                    perf.NomeGestorCliente 
                                            FROM tb_projeto_org tpo 
                                            INNER JOIN tb_cliente_org tcor  ON tpo.cod_cliente = tcor.codigo_cliente 
							                   AND tcor.tb_org_id = @OrgId
                                            LEFT JOIN ( SELECT  CASE 
                                                                    WHEN vp.id IS NULL THEN ''
                                                                    WHEN LOCATE('|', vp.id) = 0 THEN vp.id  
                                                                    ELSE SUBSTRING_INDEX(vp.id, '|', -1)
                                                                END as perfilid,
                                                                vp.perfil AS perfil, 
		                                                        tpa.tb_colaborador_periodo_alocacao_id,
                                                                tge.cod_gestor_externo AS CodGestorCliente,
                                                                tge.nome AS NomeGestorCliente
    		                                            FROM tb_perfil_alocacao tpa  
    		                                            JOIN vw_perfis vp ON vp.tb_org_id = tpa.tb_org_id
			                                                      AND (vp.id = CONCAT('1|', tpa.tb_perfil_id) 
				                                                   OR vp.id = CONCAT('2|', tpa.tb_gestor_externo_perfil_id))
			                                            LEFT JOIN tb_gestor_externo_perfil tgep ON tpa.tb_gestor_externo_perfil_id = tgep.id AND tgep.ativo = 1
			                                            LEFT JOIN tb_gestor_externo tge ON tgep.cod_gestor_externo = tge.cod_gestor_externo AND tgep.tb_org_id = tge.tb_org_id                                      
			                                            ) AS perf  ON perf.tb_colaborador_periodo_alocacao_id = @IdAlocacao
                                            WHERE tpo.cod_projeto = @CodigoProjeto
                                              AND perf.perfilid = @PerfilId                                              
                                              AND tpo.tb_org_id = @OrgId;";

                const string sqlSkillsPorCliente = @"
                                            SELECT tcor.codigo_cliente AS CodigoCliente,
                                                   tip.id  as PerfilId,
                                                   tip.descricao AS TipoPerfil, 
                                                   vs.id as SkillId,
                                                   vs.descricao AS Habilidade,
                                                   tn.id as SenioridadeId,
                                                   tn.descricao AS Senioridade,
                                                   CASE WHEN tbi.codigo_interno_colaborador IS NULL THEN 0 
                                                        WHEN tbi.ativo = 0 THEN 0           
                                                        ELSE 1 END as Interesse
                                            FROM tb_colaborador_alocado_skill tcas
                                            INNER JOIN tb_cliente_org tcor 
                                                   ON tcor.codigo_cliente = @CodCliente AND tcor.tb_org_id = @OrgId
                                            LEFT JOIN vw_skills vs 
                                                   ON vs.id = tcas.skill_id AND vs.tipo_id = tcas.tb_item_perfil_id 
                                            LEFT JOIN tb_nivel tn 
                                                   ON tn.id = tcas.tb_nivel_id AND tn.ativo = 1
                                            LEFT JOIN tb_item_perfil tip 
                                                   ON tip.id = tcas.tb_item_perfil_id AND tip.ativo = 1
                                            LEFT JOIN tb_colaborador_interesse tbi 
                                                   ON tbi.codigo_interno_colaborador = @codColaborador
                                                    AND tbi.tipo_id = tip.id AND tbi.skill_id = vs.id
                                            WHERE tcas.tb_colaborador_periodo_alocacao_id = @IdAlocacao
                                            GROUP BY tcor.codigo_cliente, tip.id, tip.descricao, vs.id, vs.descricao, tn.id, tn.descricao, tbi.codigo_interno_colaborador;";

                foreach (var colaborador in colaboradores)
                {
                    var clientes = (await connection.QueryAsync<MEClientesAlocacaoDTO>(
                        sqlClientes,
                        new { CodigoProjeto = colaborador.CodigoProjeto, OrgId = orgId, IdAlocacao = colaborador.IdAlocacao, PerfilId = perfilId }
                    )).ToList();

                    foreach (var cliente in clientes)
                    {
                        var skillsPorCliente = (await connection.QueryAsync<(
                            string CodigoCliente,
                            int? PerfilId,
                            string TipoPerfil,
                            int? SkillId,
                            string Habilidade,
                            int? SenioridadeId,
                            string Senioridade,
                            int? Interesse
                        )>(
                            sqlSkillsPorCliente,
                            new
                            {
                                IdAlocacao = colaborador.IdAlocacao,
                                CodigoProjeto = colaborador.CodigoProjeto,
                                OrgId = orgId,
                                CodCliente = cliente.CodigoCliente,
                                codColaborador,
                                perfilId
                            }
                        )).ToList();

                        cliente.Habilidades = skillsPorCliente
                                              .Select(s => new MEHabilidadesDTO
                                              {
                                                  CodigoCliente = s.CodigoCliente,
                                                  TipoPerfilId = s.PerfilId,
                                                  TipoPerfil = s.TipoPerfil,
                                                  SkillId = s.SkillId,
                                                  Habilidade = s.Habilidade,
                                                  SenioridadeId = s.SenioridadeId,
                                                  Senioridade = s.Senioridade,
                                                  Interesse = s.Interesse
                                              }).ToList();
                    }

                    colaborador.Clientes = clientes;
                }
                // Filtra só colaboradores que realmente têm clientes
                colaboradores = colaboradores
                    .Where(c => c.Clientes != null && c.Clientes.Any())
                    .ToList();

                return colaboradores;
            }
            catch
            {
                throw;
            }
        }

        // ───────────────────────────────────────────────
        // Busca Indicadores por Gestor com Perfil
        // ───────────────────────────────────────────────
        public async Task<List<GestorColaboradoresSkill>> ListaIndicadoresDosLiderados(
            string codColaboradorGestor, string perfilId, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            try
            {
                const string sqlColaborador = @"
                                        SELECT DISTINCT
                                           vgco.codigo_interno_colaborador_gestor as CodigoInternoColaboradorGestorAdm,
                                           vgco.nome_completo_gestor as NomeGestorAdm,
                                           tc.codigo_interno_colaborador as CodigoInternoColaborador, 
                                           tc.nome_completo as NomeColaborador
                                        FROM vw_gestores_colaboradores_org vgco
                                        INNER JOIN tb_colaborador_periodo_alocacao tcp 
                                            ON tcp.codigo_interno_colaborador = vgco.codigo_interno_colaborador_subordinado 
                                        INNER JOIN tb_colaborador tc 
                                            ON tc.codigo_interno_colaborador = tcp.codigo_interno_colaborador
                                        WHERE vgco.codigo_interno_colaborador_gestor = @codColaboradorGestor
                                          AND vgco.tb_org_id = @OrgId 
                                          AND tcp.tb_org_id = @OrgId  
                                          AND tcp.ativo = 1
                                          AND tc.ativo = 1
                                          AND tcp.data_fim >= CURDATE();";

                var colaboradores = (await connection.QueryAsync<MinhaEquipeDTO>(
                    sqlColaborador,
                    new { codColaboradorGestor, OrgId = orgId }
                )).ToList();

                if (!colaboradores.Any())
                {
                    return new List<GestorColaboradoresSkill>
                                {
                                    new GestorColaboradoresSkill
                                    {
                                        CodigoGestorAdm = codColaboradorGestor,
                                        NomeGestorAdm = string.Empty,
                                        Colaboradores = new List<MinhaEquipeAderenciaDTO>()
                                    }
                                };
                }

                // Pega o gestor (vai ser o mesmo para todos os colaboradores)
                var codigoGestor = colaboradores.First().CodigoInternoColaboradorGestorAdm;
                var nomeGestor = colaboradores.First().NomeGestorAdm;

                var resultadoColaboradores = new List<MinhaEquipeAderenciaDTO>();

                foreach (var colaborador in colaboradores)
                {
                    // Busca habilidades do colaborador
                    var colabList = await _minhaJornadaRepository.BuscarSkillColaborador(colaborador.CodigoInternoColaborador);

                    // Busca Perfil da Vaga
                    var vagaList = await _minhaJornadaRepository.BuscarSkillColaboradorAlocado(colaborador.CodigoInternoColaborador, orgId);

                    var skillsColaborador = (colabList ?? new List<MinhaJornadaColaboradorDTO>())
                        .SelectMany(c => c.ColaboradorHabilidades)
                        .ToList();

                    var vagas = (vagaList ?? new List<MinhaJornadaDTO>())
                            .Where(v => string.IsNullOrEmpty(perfilId)
                                     || v.Clientes.Any(c => c.PerfilId == perfilId))
                            .ToList();

                    var equipeDto = new MinhaEquipeAderenciaDTO
                    {
                        CodigoInternoColaborador = colabList?.FirstOrDefault()?.CodigoInternoColaborador,
                        NomeColaborador = colabList?.FirstOrDefault()?.NomeCompleto,
                        CodigoProjeto = vagas.FirstOrDefault()?.CodigoProjeto,
                        IdAlocacao = vagas.FirstOrDefault()?.IdAlocacao ?? 0,
                        Clientes = new List<ClientesAlocacaoAderenciaDTO>()
                    };

                    foreach (var cliente in vagas.SelectMany(v => v.Clientes))
                    {
                        var resultados = new List<ResultadoIndicadoresDTO>();

                        foreach (var skillVaga in cliente.Habilidades)
                        {
                            var skillCol = skillsColaborador.FirstOrDefault(s => s.SkillId == skillVaga.SkillId && s.PerfilTipoId == skillVaga.PerfilTipoId);
                            var resultado = new ResultadoIndicadoresDTO();

                            int vagaNivelId = MapearSenioridade(skillVaga.Senioridade);
                            int colNivelId = MapearSenioridade(skillCol?.Senioridade);

                            resultado.Habilidade = skillCol?.Habilidade;
                            resultado.ColaboradorNivel = skillCol?.Senioridade ?? "não definido";
                            resultado.VagaNivel = skillVaga.Senioridade ?? "não definido";
                            resultado.Interesse = skillCol?.Interesse ?? skillVaga.Interesse;

                            if (skillCol == null)
                            {
                                resultado.PerfilTipoId = skillVaga.PerfilTipoId;
                                resultado.TipoPerfil = skillVaga.TipoPerfil;
                                resultado.Pendencia = true;
                                resultado.Status = "Colaborador não tem essa Habilidade";
                            }
                            else if (colNivelId < vagaNivelId)
                            {
                                resultado.PerfilTipoId = skillCol.PerfilTipoId;
                                resultado.TipoPerfil = skillCol.TipoPerfil;
                                resultado.Pendencia = true;
                                resultado.Status = "Colaborador não alcançou o nível da Habilidade desejado";
                            }
                            else
                            {
                                resultado.PerfilTipoId = skillCol.PerfilTipoId;
                                resultado.TipoPerfil = skillCol.TipoPerfil;
                                resultado.Pendencia = false;
                                resultado.Status = "Colaborador tem Habilidade Compatível";
                            }

                            resultados.Add(resultado);
                        }

                        var habilidadesValidas = resultados
                                    .Where(r => !string.IsNullOrWhiteSpace(r.Habilidade)
                                                && !string.IsNullOrWhiteSpace(r.Status))
                                    .ToList();

                        if (habilidadesValidas.Any())
                        {
                            equipeDto.Clientes.Add(new ClientesAlocacaoAderenciaDTO
                            {
                                CodigoCliente = cliente.CodigoCliente,
                                NomeCliente = cliente.NomeCliente,
                                PerfilId = cliente.PerfilId,
                                Perfil = cliente.Perfil,
                                CodGestorCliente = cliente.CodGestorCliente,
                                NomeGestorCliente = cliente.NomeGestorCliente,
                                ResultadoHabilidades = habilidadesValidas
                            });
                        }
                    }

                    resultadoColaboradores.Add(equipeDto);
                }

                // Filtra só colaboradores que realmente têm clientes
                resultadoColaboradores = resultadoColaboradores
                    .Where(c => c.Clientes != null && c.Clientes.Any())
                    .ToList();

                // Se não houver colaboradores com clientes, não retorna nada
                if (!resultadoColaboradores.Any())
                    return new List<GestorColaboradoresSkill>();

                // Retorna o gestor + lista de colaboradores válidos
                return new List<GestorColaboradoresSkill>
                            {
                                new GestorColaboradoresSkill
                                {
                                    CodigoGestorAdm = codigoGestor,
                                    NomeGestorAdm = nomeGestor,
                                    Colaboradores = resultadoColaboradores
                                }
                            };
            }
            catch
            {
                throw;
            }
        }

        // ────────────────────────────────────────────────────────
        // Lista Aderência dos Colaboradores por Gestor com Perfil
        // ────────────────────────────────────────────────────────
        public async Task<List<GestorCandidatosMatchResponse>> ListaAderenciaDosLideradosPorGestor(string codColaboradorGestor, string perfilId, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            try
            {
                const string sqlColaborador = @"
                                        SELECT DISTINCT
                                           vgco.codigo_interno_colaborador_gestor as CodigoInternoColaboradorGestorAdm,
                                           vgco.nome_completo_gestor as NomeGestorAdm,
                                           tc.codigo_interno_colaborador as CodigoInternoColaborador, 
                                           tc.nome_completo as NomeColaborador
                                        FROM vw_gestores_colaboradores_org vgco
                                        INNER JOIN tb_colaborador_periodo_alocacao tcp 
                                            ON tcp.codigo_interno_colaborador = vgco.codigo_interno_colaborador_subordinado 
                                        INNER JOIN tb_colaborador tc 
                                            ON tc.codigo_interno_colaborador = tcp.codigo_interno_colaborador
                                        WHERE vgco.codigo_interno_colaborador_gestor = @codColaboradorGestor
                                          AND vgco.tb_org_id = @OrgId 
                                          AND tcp.tb_org_id = @OrgId  
                                          AND tcp.ativo = 1
                                          AND tc.ativo = 1
                                          AND tcp.data_fim >= CURDATE();";

                var colaboradores = (await connection.QueryAsync<MinhaEquipeDTO>(
                    sqlColaborador,
                    new { codColaboradorGestor, OrgId = orgId }
                )).ToList();

                if (!colaboradores.Any())
                {
                    return new List<GestorCandidatosMatchResponse>
                                {
                                    new GestorCandidatosMatchResponse
                                    {
                                        CodigoGestor = codColaboradorGestor,
                                        NomeGestor = "",
                                        RetornoMatch = new List<CandidatosMatchResponse>()
                                    }
                                };
                }

                var resultadoMatch = new List<CandidatosMatchResponse>();

                // Dados do gestor
                var codigoGestor = colaboradores.First().CodigoInternoColaboradorGestorAdm;
                var nomeGestor = colaboradores.First().NomeGestorAdm;

                int totPendentes = 0;
                double totMatch = 0.0;

                foreach (var colaborador in colaboradores)
                {
                    // Buscar todas as alocações do colaborador
                    var alocacoesList = await _minhaJornadaRepository.BuscarSkillColaboradorAlocado(colaborador.CodigoInternoColaborador, orgId);
                    var alocacoes = alocacoesList ?? new List<MinhaJornadaDTO>();

                    // Buscar todas as skills do colaborador
                    var colabSkillsList = await _minhaJornadaRepository.BuscarSkillColaborador(colaborador.CodigoInternoColaborador);
                    var skillsColaborador = (colabSkillsList ?? new List<MinhaJornadaColaboradorDTO>())
                                            .SelectMany(c => c.ColaboradorHabilidades)
                                            .ToList();

                    // Listas agregadas de skills
                    var hardSkills = new List<SkillItem>();
                    var softSkills = new List<SkillItem>();
                    var metodologias = new List<SkillItem>();
                    var dominiosNegocio = new List<SkillItem>();
                    var idiomas = new List<SkillItem>();

                    int pendentesColaborador = 0;

                    foreach (var alocacao in alocacoes)
                    {
                        foreach (var cliente in alocacao.Clientes
                            .Where(c => string.IsNullOrEmpty(perfilId) || c.PerfilId == perfilId))
                        {
                            foreach (var skillVaga in cliente.Habilidades)
                            {
                                var skillCol = skillsColaborador.FirstOrDefault(s => s.SkillId == skillVaga.SkillId && s.PerfilTipoId == skillVaga.PerfilTipoId);
                                int vagaNivelId = MapearSenioridade(skillVaga.Senioridade);
                                int colNivelId = MapearSenioridade(skillCol?.Senioridade);

                                if (skillCol == null || colNivelId < vagaNivelId)
                                {
                                    pendentesColaborador++;
                                }

                                if (skillCol != null)
                                {
                                    var skillItem = new SkillItem
                                    {
                                        Nome = skillCol.Habilidade,
                                        Nivel = skillCol.Senioridade,
                                        Obrigatoriedade = (skillCol.Interesse ?? 0) == 1 ? "obrigatorio" : "desejavel"
                                    };

                                    AdicionarSkillNaListaCorreta(skillCol.TipoPerfil, skillItem,
                                        hardSkills, softSkills, metodologias, dominiosNegocio, idiomas);
                                }
                            }
                        }
                    }

                    // Cria o request final **uma vez por colaborador**
                    var baseRequest = new ScoreSingleCandidateRequest
                    {
                        HardSkills = hardSkills,
                        SoftSkills = softSkills,
                        Metodologias = metodologias,
                        DominiosNegocio = dominiosNegocio,
                        Idiomas = idiomas,
                        PesoHardSkills = 1,
                        PesoSoftSkills = 1,
                        PesoMetodologias = 1,
                        PesoDominiosNegocio = 1,
                        PesoIdiomas = 1,
                        PesoDisponibilidades = 1,
                        VisibleToOrgIds = new List<int> { orgId },
                        Disponibilidades = new List<DisponibilidadeItem>(),
                        NumeroDeCandidatos = 1,
                        CodigoInternoColaborador = colaborador.CodigoInternoColaborador
                    };

                    var scoreResult = await _matchClient.ScoreSingleCandidate(baseRequest);

                    if (scoreResult != null)
                    {
                        resultadoMatch.Add(scoreResult);
                        totMatch += scoreResult.Match;
                    }

                    totPendentes += pendentesColaborador;
                }

                double mediaMatch = colaboradores.Count > 0 ? Math.Round(totMatch / colaboradores.Count, 2) : 0.0;

                // Retorna resultado agregado
                return new List<GestorCandidatosMatchResponse>
                                {
                                    new GestorCandidatosMatchResponse
                                    {
                                        CodigoGestor = codigoGestor,
                                        NomeGestor = nomeGestor,
                                        RetornoMatch = resultadoMatch
                                    }
                                };
            }
            catch
            {
                throw;
            }
        }

        // ─────────────────────────────────────────────────────────
        // Lista Aderência dos Colaboradores por GestorAdm/Oper 
        // ─────────────────────────────────────────────────────────
        public async Task<List<GestorCandidatosMatchResponsePerfil>> ListaAderenciaDosLideradosPorGestorSemParamPerfil(string codGestorAdm, string codGestorOper, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            try
            {
                const string sqlColaborador = @"
                                            SELECT DISTINCT
                                                vgco.codigo_interno_colaborador_gestor AS CodigoInternoColaboradorGestorAdm,
                                                vgco.nome_completo_gestor AS NomeGestorAdm,
                                                tc.codigo_interno_colaborador AS CodigoInternoColaborador, 
                                                tc.nome_completo AS NomeColaborador,
                                                tg.codigo_interno_colaborador AS CodigoGestorOperacional, 
                                                tg.nome_completo AS NomeGestorOperacional
                                            FROM vw_gestores_colaboradores_org vgco
                                            INNER JOIN tb_colaborador_periodo_alocacao tcp 
                                                ON tcp.codigo_interno_colaborador = vgco.codigo_interno_colaborador_subordinado 
                                            INNER JOIN tb_colaborador tc 
                                                ON tc.codigo_interno_colaborador = tcp.codigo_interno_colaborador
                                            LEFT JOIN tb_projeto_org tpo 
                                                    ON tpo.cod_projeto = tcp.codigo_projeto 
                                                   AND tcp.tb_org_id = tpo.tb_org_id
                                            LEFT JOIN tb_projeto_gerente tpg 
                                                    ON tpo.cod_projeto = tpg.cod_projeto AND tpg.tb_org_id = tcp.tb_org_id
                                            LEFT JOIN tb_colaborador_org tco_g 
                                                ON tpg.cod_colaborador_gerente = tco_g.cod_colaborador_externo 
                                                AND tco_g.tb_org_id = tcp.tb_org_id
                                            LEFT JOIN tb_colaborador tg 
                                                ON tco_g.codigo_interno_colaborador = tg.codigo_interno_colaborador
                                            WHERE (vgco.codigo_interno_colaborador_gestor = @codGestorAdm OR @codGestorAdm IS NULL OR @codGestorAdm = '')
                                              AND (tg.codigo_interno_colaborador = @codGestorOper OR @codGestorOper IS NULL OR @codGestorOper = '')
                                              AND vgco.tb_org_id = @OrgId 
                                              AND tcp.tb_org_id = @OrgId  
                                              AND tcp.ativo = 1
                                              AND tc.ativo = 1
                                              AND tcp.data_fim >= CURDATE();";

                var colaboradores = (await connection.QueryAsync<MinhaEquipeDTO>(
                    sqlColaborador,
                    new { codGestorAdm, codGestorOper, OrgId = orgId }
                )).ToList();

                if (!colaboradores.Any())
                {
                    return new List<GestorCandidatosMatchResponsePerfil>
                                    {
                                        new GestorCandidatosMatchResponsePerfil
                                        {
                                            CodigoGestorAdm = codGestorAdm,
                                            NomeGestorAdm = "",
                                            CodigoGestorOper = codGestorOper,
                                            NomeGestorOper = "",
                                            RetornoMatch = new List<CandidatoComPerfilResponse>()
                                        }
                                    };
                }

                // Agrupa colaboradores por gestor
                var gestoresAgrupados = colaboradores
                    .GroupBy(c => new { c.CodigoInternoColaboradorGestorAdm, c.NomeGestorAdm, c.CodigoGestorOperacional, c.NomeGestorOperacional })
                    .ToList();

                var resultadoFinal = new List<GestorCandidatosMatchResponsePerfil>();

                // Para cada gestor
                foreach (var grupoGestor in gestoresAgrupados)
                {
                    var gestor = grupoGestor.Key;
                    var resultadoMatchWrapped = new List<CandidatoComPerfilResponse>();

                    // Para cada colaborador desse gestor
                    foreach (var colaborador in grupoGestor.DistinctBy(x => x.CodigoInternoColaborador))
                    {
                        // Buscar todas as alocações do colaborador
                        var alocacoesList = await _minhaJornadaRepository.BuscarSkillColaboradorAlocado(colaborador.CodigoInternoColaborador, orgId);
                        var alocacoes = alocacoesList ?? new List<MinhaJornadaDTO>();

                        // Buscar todas as skills do colaborador
                        var colabSkillsList = await _minhaJornadaRepository.BuscarSkillColaborador(colaborador.CodigoInternoColaborador);
                        var skillsColaborador = (colabSkillsList ?? new List<MinhaJornadaColaboradorDTO>())
                            .SelectMany(c => c.ColaboradorHabilidades)
                            .ToList();

                        var primeiroAlocado = alocacoes.FirstOrDefault();
                        var tipoId = primeiroAlocado?.Clientes?.FirstOrDefault()?.PerfilId ?? string.Empty;
                        var perfilNome = primeiroAlocado?.Clientes?.FirstOrDefault()?.Perfil ?? string.Empty;

                        // Listas agregadas de skills
                        var hardSkills = new List<SkillItem>();
                        var softSkills = new List<SkillItem>();
                        var metodologias = new List<SkillItem>();
                        var dominiosNegocio = new List<SkillItem>();
                        var idiomas = new List<SkillItem>();

                        foreach (var alocacao in alocacoes)
                        {
                            foreach (var cliente in alocacao.Clientes)
                            {
                                foreach (var skillVaga in cliente.Habilidades)
                                {
                                    var skillCol = skillsColaborador.FirstOrDefault(s => s.SkillId == skillVaga.SkillId && s.PerfilTipoId == skillVaga.PerfilTipoId);

                                    if (skillCol != null)
                                    {
                                        var skillItem = new SkillItem
                                        {
                                            Nome = skillCol.Habilidade,
                                            Nivel = skillCol.Senioridade,
                                            Obrigatoriedade = (skillCol.Interesse ?? 0) == 1 ? "obrigatorio" : "desejavel"
                                        };

                                        AdicionarSkillNaListaCorreta(skillCol.TipoPerfil, skillItem,
                                                                     hardSkills, softSkills, metodologias, dominiosNegocio, idiomas);
                                    }
                                }
                            }
                        }

                        // Cria o request por colaborador
                        var baseRequest = new ScoreSingleCandidateRequest
                        {
                            HardSkills = hardSkills,
                            SoftSkills = softSkills,
                            Metodologias = metodologias,
                            DominiosNegocio = dominiosNegocio,
                            Idiomas = idiomas,
                            PesoHardSkills = 1,
                            PesoSoftSkills = 1,
                            PesoMetodologias = 1,
                            PesoDominiosNegocio = 1,
                            PesoIdiomas = 1,
                            PesoDisponibilidades = 1,
                            VisibleToOrgIds = new List<int> { orgId },
                            Disponibilidades = new List<DisponibilidadeItem>(),
                            NumeroDeCandidatos = 1,
                            CodigoInternoColaborador = colaborador.CodigoInternoColaborador
                        };

                        // Chama o serviço de match
                        var scoreResult = await _matchClient.ScoreSingleCandidate(baseRequest);

                        if (scoreResult != null)
                        {
                            resultadoMatchWrapped.Add(new CandidatoComPerfilResponse
                            {
                                MatchResponse = scoreResult,
                                TipoId = tipoId,
                                Perfil = perfilNome
                            });
                        }
                    }

                    // Adiciona o resultado do gestor atual à lista final
                    resultadoFinal.Add(new GestorCandidatosMatchResponsePerfil
                    {
                        CodigoGestorAdm = gestor.CodigoInternoColaboradorGestorAdm,
                        NomeGestorAdm = gestor.NomeGestorAdm,
                        CodigoGestorOper =  gestor.CodigoGestorOperacional,
                        NomeGestorOper = gestor.NomeGestorOperacional,
                        RetornoMatch = resultadoMatchWrapped
                    });
                }

                return resultadoFinal;
            }
            catch (Exception ex)
            {
                // Retorna lista vazia (ou poderia logar o erro)
                return new List<GestorCandidatosMatchResponsePerfil>
                {
                    new GestorCandidatosMatchResponsePerfil
                    {
                        CodigoGestorAdm = codGestorAdm,
                        NomeGestorAdm = "Erro",
                        CodigoGestorOper = codGestorOper,
                        NomeGestorOper = "Erro",
                        RetornoMatch = new List<CandidatoComPerfilResponse>
                        {
                            new CandidatoComPerfilResponse
                            {
                                MatchResponse = null,
                                Perfil = "Erro ao calcular aderência",
                                TipoId = ex.Message
                            }
                        }
                    }
                };
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // Lista Aderência dos Colaboradores Individualmente com Perfil
        // ─────────────────────────────────────────────────────────────────
        public async Task<List<GestorCandidatosMatchResponse>> ListaAderenciaDoColaborador(string codColaborador, string perfilId, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            try
            {
                const string sqlColaborador = @"
                                    SELECT DISTINCT
                                       vgco.codigo_interno_colaborador_gestor as CodigoInternoColaboradorGestorAdm,
                                       vgco.nome_completo_gestor as NomeGestorAdm,
                                       tc.codigo_interno_colaborador as CodigoInternoColaborador, 
                                       tc.nome_completo as NomeColaborador
                                    FROM tb_colaborador tc
                                    INNER JOIN tb_colaborador_periodo_alocacao tcp 
                                        ON tcp.codigo_interno_colaborador = tc.codigo_interno_colaborador
                                    LEFT JOIN vw_gestores_colaboradores_org vgco
                                        ON tc.codigo_interno_colaborador = vgco.codigo_interno_colaborador_subordinado AND vgco.tb_org_id = @OrgId 
                                    WHERE tc.codigo_interno_colaborador = @codColaborador
                                      AND tcp.tb_org_id = @OrgId  
                                      AND tcp.ativo = 1
                                      AND tc.ativo = 1
                                      AND tcp.data_fim >= CURDATE();";

                var colaboradores = (await connection.QueryAsync<MinhaEquipeDTO>(
                    sqlColaborador,
                    new { codColaborador, OrgId = orgId }
                )).ToList();

                if (!colaboradores.Any())
                {
                    return new List<GestorCandidatosMatchResponse>
                            {
                                new GestorCandidatosMatchResponse
                                {
                                    CodigoGestor = codColaborador,
                                    NomeGestor = "",
                                    RetornoMatch = new List<CandidatosMatchResponse>()
                                }
                            };
                }

                var resultadoMatch = new List<CandidatosMatchResponse>();

                // Dados do gestor 
                var codigoGestor = colaboradores.First().CodigoInternoColaboradorGestorAdm;
                var nomeGestor = colaboradores.First().NomeGestorAdm;

                // Percorre cada colaborador do gestor
                foreach (var colaborador in colaboradores)
                {
                    // Busca as alocações desse colaborador
                    var alocacoesList = await _minhaJornadaRepository.BuscarSkillColaboradorAlocado(colaborador.CodigoInternoColaborador, orgId);
                    var alocacoes = alocacoesList ?? new List<MinhaJornadaDTO>();

                    // Busca habilidades do colaborador
                    var colabList = await _minhaJornadaRepository.BuscarSkillColaborador(colaborador.CodigoInternoColaborador);

                    // Perfil do Colaborador
                    var skillsColaborador = (colabList ?? new List<MinhaJornadaColaboradorDTO>())
                        .SelectMany(c => c.ColaboradorHabilidades)
                        .ToList();

                    // Listas agregadas de skills
                    var hardSkills = new List<SkillItem>();
                    var softSkills = new List<SkillItem>();
                    var metodologias = new List<SkillItem>();
                    var dominiosNegocio = new List<SkillItem>();
                    var idiomas = new List<SkillItem>();

                    foreach (var alocacao in alocacoes)
                    {
                        foreach (var cliente in alocacao.Clientes
                            .Where(c => string.IsNullOrEmpty(perfilId) || c.PerfilId == perfilId))
                        {
                            foreach (var skillVaga in cliente.Habilidades)
                            {
                                var skillCol = skillsColaborador.FirstOrDefault(s => s.SkillId == skillVaga.SkillId && s.PerfilTipoId == skillVaga.PerfilTipoId);

                                if (skillCol != null)
                                {
                                    var skillItem = new SkillItem
                                    {
                                        Nome = skillCol.Habilidade,
                                        Nivel = skillCol.Senioridade,
                                        Obrigatoriedade = (skillCol.Interesse ?? 0) == 1 ? "obrigatorio" : "desejavel"
                                    };

                                    AdicionarSkillNaListaCorreta(skillCol.TipoPerfil, skillItem,
                                                                 hardSkills, softSkills, metodologias, dominiosNegocio, idiomas);

                                }
                            }
                        }
                    }
                    // Criar baseRequest **uma vez por colaborador**
                    var baseRequest = new ScoreSingleCandidateRequest
                    {
                        HardSkills = hardSkills,
                        SoftSkills = softSkills,
                        Metodologias = metodologias,
                        DominiosNegocio = dominiosNegocio,
                        Idiomas = idiomas,
                        PesoHardSkills = 1,
                        PesoSoftSkills = 1,
                        PesoMetodologias = 1,
                        PesoDominiosNegocio = 1,
                        PesoIdiomas = 1,
                        PesoDisponibilidades = 1,
                        VisibleToOrgIds = new List<int> { orgId },
                        Disponibilidades = new List<DisponibilidadeItem>(),
                        NumeroDeCandidatos = 1,
                        CodigoInternoColaborador = colaborador.CodigoInternoColaborador
                    };

                    // Chamada única à API
                    var scoreResult = await _matchClient.ScoreSingleCandidate(baseRequest);

                    if (scoreResult != null)
                    {
                        resultadoMatch.Add(scoreResult);
                    }
                }

                return new List<GestorCandidatosMatchResponse>
                            {
                                new GestorCandidatosMatchResponse
                                {
                                    CodigoGestor = codigoGestor,
                                    NomeGestor = nomeGestor,
                                    RetornoMatch = resultadoMatch
                                }
                            };
            }
            catch
            {
                throw;
            }
        }
        // ─────────────────────────────────────────────────────────────
        // Lista Aderência dos Colaboradores Individualmente Adm/Oper
        // ─────────────────────────────────────────────────────────────
        public async Task<List<GestorCandidatosMatchResponse>> ListaAderenciaDoColaboradorAdmOper(string codColaborador, int orgId)
        { 
            var connection = _dapperConnection.GetConnection();

            try
            {
                const string sqlColaborador = @"
                                        SELECT DISTINCT
                                           vgco.codigo_interno_colaborador_gestor as CodigoInternoColaboradorGestorAdm,
                                           vgco.nome_completo_gestor as NomeGestorAdm,
                                           tc.codigo_interno_colaborador as CodigoInternoColaborador, 
                                           tc.nome_completo as NomeColaborador
                                        FROM tb_colaborador tc
                                        INNER JOIN tb_colaborador_periodo_alocacao tcp 
                                            ON tcp.codigo_interno_colaborador = tc.codigo_interno_colaborador
                                        LEFT JOIN vw_gestores_colaboradores_org vgco
                                            ON tc.codigo_interno_colaborador = vgco.codigo_interno_colaborador_subordinado AND vgco.tb_org_id = @OrgId 
                                        WHERE tc.codigo_interno_colaborador = @codColaborador
                                          AND tcp.tb_org_id = @OrgId  
                                          AND tcp.ativo = 1
                                          AND tc.ativo = 1
                                          AND tcp.data_fim >= CURDATE();";

                var colaboradores = (await connection.QueryAsync<MinhaEquipeDTO>(
                    sqlColaborador,
                    new { codColaborador, OrgId = orgId }
                )).ToList();

                if (!colaboradores.Any())
                {
                    return new List<GestorCandidatosMatchResponse>
                                {
                                    new GestorCandidatosMatchResponse
                                    {
                                        CodigoGestor = codColaborador,
                                        NomeGestor = "",
                                        RetornoMatch = new List<CandidatosMatchResponse>()
                                    }
                                };
                }

                var resultadoMatch = new List<CandidatosMatchResponse>();

                // Dados do gestor 
                var codigoGestor = colaboradores.First().CodigoInternoColaboradorGestorAdm;
                var nomeGestor = colaboradores.First().NomeGestorAdm;

                foreach (var colaborador in colaboradores)
                {
                    // Buscar todas as alocações do colaborador
                    var alocacoesList = await _minhaJornadaRepository.BuscarSkillColaboradorAlocado(colaborador.CodigoInternoColaborador, orgId);
                    var alocacoes = alocacoesList ?? new List<MinhaJornadaDTO>();

                    // Busca habilidades do colaborador
                    var colabList = await _minhaJornadaRepository.BuscarSkillColaborador(colaborador.CodigoInternoColaborador);

                    // Perfil do Colaborador
                    var skillsColaborador = (colabList ?? new List<MinhaJornadaColaboradorDTO>())
                        .SelectMany(c => c.ColaboradorHabilidades)
                        .ToList();

                    // Listas agregadas de skills
                    var hardSkills = new List<SkillItem>();
                    var softSkills = new List<SkillItem>();
                    var metodologias = new List<SkillItem>();
                    var dominiosNegocio = new List<SkillItem>();
                    var idiomas = new List<SkillItem>();

                    foreach (var alocacao in alocacoes)
                    {
                        foreach (var cliente in alocacao.Clientes)
                        {
                            foreach (var skillVaga in cliente.Habilidades)
                            {
                                var skillCol = skillsColaborador.FirstOrDefault(s => s.SkillId == skillVaga.SkillId && s.PerfilTipoId == skillVaga.PerfilTipoId);

                                if (skillCol != null)
                                {
                                    var skillItem = new SkillItem
                                    {
                                        Nome = skillCol.Habilidade,
                                        Nivel = skillCol.Senioridade,
                                        Obrigatoriedade = (skillCol.Interesse ?? 0) == 1 ? "obrigatorio" : "desejavel"
                                    };

                                    AdicionarSkillNaListaCorreta(skillCol.TipoPerfil, skillItem,
                                                                 hardSkills, softSkills, metodologias, dominiosNegocio, idiomas);
                                }
                            } 
                        }
                    }

                    // Criar baseRequest **uma vez por colaborador**
                    var baseRequest = new ScoreSingleCandidateRequest
                    {
                        HardSkills = hardSkills,
                        SoftSkills = softSkills,
                        Metodologias = metodologias,
                        DominiosNegocio = dominiosNegocio,
                        Idiomas = idiomas,
                        PesoHardSkills = 1,
                        PesoSoftSkills = 1,
                        PesoMetodologias = 1,
                        PesoDominiosNegocio = 1,
                        PesoIdiomas = 1,
                        PesoDisponibilidades = 1,
                        VisibleToOrgIds = new List<int> { orgId },
                        Disponibilidades = new List<DisponibilidadeItem>(),
                        NumeroDeCandidatos = 1,
                        CodigoInternoColaborador = colaborador.CodigoInternoColaborador
                    };

                    // Chamada única à API
                    var scoreResult = await _matchClient.ScoreSingleCandidate(baseRequest);

                    if (scoreResult != null)
                    {
                        resultadoMatch.Add(scoreResult);
                    }
                }

                return new List<GestorCandidatosMatchResponse>
                        {
                            new GestorCandidatosMatchResponse
                            {
                                CodigoGestor = codigoGestor,
                                NomeGestor = nomeGestor,
                                RetornoMatch = resultadoMatch
                            }
                        };
            }
            catch
            {
                throw;
            }
        }

        // ──────────────────────────────────────────────────────────────
        // Busca Indicadores por GestorAdm e GestorOperacional 
        // ──────────────────────────────────────────────────────────────
        public async Task<List<GestorColaboradoresSkillAdmOper>> ListaIndicadoresDosLideradosAdmOper(
                                int orgId, int limite, int cursor, string codGestorAdm, string codGestorOper, string codCliente)
        {
            var connection = _dapperConnection.GetConnection();

            try
            {
                const string sqlColaborador = @"
                            SELECT DISTINCT
                               vgco.codigo_interno_colaborador_gestor as CodigoInternoColaboradorGestorAdm,
                               vgco.nome_completo_gestor as NomeGestorAdm,
                               tc.codigo_interno_colaborador as CodigoInternoColaborador, 
                               tc.nome_completo as NomeColaborador,
                               tg.codigo_interno_colaborador AS CodigoGestorOperacional, 
                               tg.nome_completo AS NomeGestorOperacional
                            FROM vw_gestores_colaboradores_org vgco
                            INNER JOIN tb_colaborador_periodo_alocacao tcp 
                                ON tcp.codigo_interno_colaborador = vgco.codigo_interno_colaborador_subordinado 
                            INNER JOIN tb_colaborador tc 
                                ON tc.codigo_interno_colaborador = tcp.codigo_interno_colaborador
                            LEFT JOIN tb_projeto_org tpo 
                                    ON tpo.cod_projeto = tcp.codigo_projeto 
                                   AND tcp.tb_org_id = tpo.tb_org_id
                            LEFT JOIN tb_projeto_gerente tpg 
                                    ON tpo.cod_projeto = tpg.cod_projeto AND tpg.tb_org_id = tcp.tb_org_id
                            LEFT JOIN tb_colaborador_org tco_g 
                                ON tpg.cod_colaborador_gerente = tco_g.cod_colaborador_externo 
                                AND tco_g.tb_org_id = tcp.tb_org_id
                            LEFT JOIN tb_colaborador tg 
                                ON tco_g.codigo_interno_colaborador = tg.codigo_interno_colaborador
                            WHERE (vgco.codigo_interno_colaborador_gestor = @codGestorAdm OR @codGestorAdm IS NULL OR @codGestorAdm = '')
                              AND (tg.codigo_interno_colaborador = @codGestorOper OR @codGestorOper IS NULL OR @codGestorOper = '')
                              AND (tpo.cod_cliente = @codCliente OR @codCliente IS NULL OR @codCliente = '')
                              AND vgco.tb_org_id = @OrgId 
                              AND tcp.tb_org_id = @OrgId  
                              AND tcp.ativo = 1
                              AND tc.ativo = 1
                              AND tcp.data_fim >= CURDATE()
                              LIMIT @Limite OFFSET @Cursor;";

                var colaboradores = (await connection.QueryAsync<MinhaEquipeDTO>(
                    sqlColaborador,
                    new { OrgId = orgId, Limite = limite, Cursor = cursor, codGestorAdm, codGestorOper, codCliente }
                )).ToList();

                if (!colaboradores.Any())
                {
                    return new List<GestorColaboradoresSkillAdmOper>

                    {
                        new GestorColaboradoresSkillAdmOper
                        {
                            CodigoGestorAdm = codGestorAdm,
                            NomeGestorAdm = string.Empty,
                            Colaboradores = new List<MinhaEquipeAderenciaDTO>(),
                            CodigoGestorOperacional = codGestorOper,
                            NomeGestorOperacional = string.Empty,
                            GestoresOperacionais = new List<GestorOperacionalDTO>()
                        }
                    };
                }

                // Agrupa por Gestor Administrativo
                var gruposGestores = colaboradores
                    .GroupBy(c => new
                    {
                        c.CodigoInternoColaboradorGestorAdm,
                        c.NomeGestorAdm
                    })
                    .ToList();

                var resultado = new List<GestorColaboradoresSkillAdmOper>();

                foreach (var grupo in gruposGestores)
                {
                    var resultadoColaboradores = new List<MinhaEquipeAderenciaDTO>();

                    foreach (var colaborador in grupo.DistinctBy(c => c.CodigoInternoColaborador))
                    {
                        // Busca habilidades do colaborador
                        var colabList = await _minhaJornadaRepository.BuscarSkillColaborador(colaborador.CodigoInternoColaborador);

                        // Busca Perfil da Vaga
                        var vagaList = await _minhaJornadaRepository.BuscarSkillColaboradorAlocado(colaborador.CodigoInternoColaborador, orgId);

                        // Perfil do Colaborador
                        var skillsColaborador = (colabList ?? new List<MinhaJornadaColaboradorDTO>())
                            .SelectMany(c => c.ColaboradorHabilidades)
                            .ToList();

                        var vagas = (vagaList ?? new List<MinhaJornadaDTO>())
                            .ToList();

                        if (!vagas.Any() || vagas.FirstOrDefault()?.IdAlocacao == 0)
                            continue;

                        var equipeDto = new MinhaEquipeAderenciaDTO
                        {
                            CodigoInternoColaborador = colabList?.FirstOrDefault()?.CodigoInternoColaborador,
                            NomeColaborador = colabList?.FirstOrDefault()?.NomeCompleto,
                            CodigoProjeto = vagas.FirstOrDefault()?.CodigoProjeto,
                            IdAlocacao = vagas.FirstOrDefault()?.IdAlocacao ?? 0,
                            Clientes = new List<ClientesAlocacaoAderenciaDTO>()
                        };

                         foreach (var cliente in vagas.SelectMany(v => v.Clientes))
                        {
                            var resultados = new List<ResultadoIndicadoresDTO>();

                            foreach (var skillVaga in cliente.Habilidades)
                            {
                                var skillCol = skillsColaborador.FirstOrDefault(s => s.SkillId == skillVaga.SkillId && s.PerfilTipoId == skillVaga.PerfilTipoId);
                                var resultadoIndicador = new ResultadoIndicadoresDTO();

                                int vagaNivelId = MapearSenioridade(skillVaga.Senioridade);
                                int colNivelId = MapearSenioridade(skillCol?.Senioridade);

                                resultadoIndicador.Habilidade = skillVaga.Habilidade;
                                resultadoIndicador.ColaboradorNivel = skillCol?.Senioridade ?? "não definido";
                                resultadoIndicador.VagaNivel = skillVaga.Senioridade ?? "não definido";
                                resultadoIndicador.Interesse = skillCol?.Interesse ?? skillVaga.Interesse;

                                if (skillCol == null)
                                {
                                    resultadoIndicador.PerfilTipoId = skillVaga.PerfilTipoId;
                                    resultadoIndicador.TipoPerfil = skillVaga.TipoPerfil;
                                    resultadoIndicador.Pendencia = true;
                                    resultadoIndicador.Status = "Colaborador não tem essa Habilidade";
                                }
                                else if (colNivelId < vagaNivelId)
                                {
                                    resultadoIndicador.PerfilTipoId = skillCol.PerfilTipoId;
                                    resultadoIndicador.TipoPerfil = skillCol.TipoPerfil;
                                    resultadoIndicador.Pendencia = true;
                                    resultadoIndicador.Status = "Colaborador não alcançou o nível da Habilidade desejado";
                                }
                                else
                                {
                                    resultadoIndicador.PerfilTipoId = skillCol.PerfilTipoId;
                                    resultadoIndicador.TipoPerfil = skillCol.TipoPerfil;
                                    resultadoIndicador.Pendencia = false;
                                    resultadoIndicador.Status = "Colaborador tem Habilidade Compatível";
                                }

                                resultados.Add(resultadoIndicador);
                            }

                            var habilidadesValidas = resultados
                                            .Where(r => !string.IsNullOrWhiteSpace(r.Habilidade)
                                                        && !string.IsNullOrWhiteSpace(r.Status))
                                            .ToList();
                           
                            if (habilidadesValidas.Any())
                            {
                                equipeDto.Clientes.Add(new ClientesAlocacaoAderenciaDTO
                                {
                                    CodigoCliente = cliente.CodigoCliente,
                                    NomeCliente = cliente.NomeCliente,
                                    PerfilId = cliente.PerfilId,
                                    Perfil = cliente.Perfil,
                                    CodGestorCliente = cliente.CodGestorCliente,
                                    NomeGestorCliente = cliente.NomeGestorCliente,
                                    ResultadoHabilidades = habilidadesValidas
                                });
                            }
                        }

                        // Só adiciona se tiver clientes
                        //if (equipeDto.Clientes.Any())
                        //{
                        resultadoColaboradores.Add(equipeDto);
                        //}
                    }

                    // Só adiciona o grupo se tiver colaboradores com clientes
                    if (resultadoColaboradores.Any())
                    {
                        var gestoresOperacionais = grupo
                            .Where(g => !string.IsNullOrWhiteSpace(g.CodigoGestorOperacional)
                                     || !string.IsNullOrWhiteSpace(g.NomeGestorOperacional))
                            .GroupBy(g => new { g.CodigoGestorOperacional, g.NomeGestorOperacional })
                            .Select(g => new GestorOperacionalDTO
                            {
                                CodigoGestorOperacional = g.Key.CodigoGestorOperacional,
                                NomeGestorOperacional = g.Key.NomeGestorOperacional
                            })
                            .ToList();

                        var codigoGestorOperacional = string.Empty;
                        var nomeGestorOperacional = string.Empty;

                        if (gestoresOperacionais.Count == 1)
                        {
                            codigoGestorOperacional = gestoresOperacionais[0].CodigoGestorOperacional;
                            nomeGestorOperacional = gestoresOperacionais[0].NomeGestorOperacional;
                        }

                        resultado.Add(new GestorColaboradoresSkillAdmOper
                        {
                            CodigoGestorAdm = grupo.Key.CodigoInternoColaboradorGestorAdm,
                            NomeGestorAdm = grupo.Key.NomeGestorAdm,
                            CodigoGestorOperacional = codigoGestorOperacional,
                            NomeGestorOperacional = nomeGestorOperacional,
                            GestoresOperacionais = gestoresOperacionais,
                            Colaboradores = resultadoColaboradores
                        });
                    }
                }
                return resultado;
            }
            catch
            {
                throw;
            }
        }

        // ──────────────────────────────────────────────────────────────
        // Busca Indicadores por GestorAdm e GestorOperacional 
        // ──────────────────────────────────────────────────────────────
        public async Task<List<TotalizacaoIndicadoresPorGestorAdmOper>> TotalizacaoIndicadoresDosLideradosAdmOper(
            int orgId, string codGestorAdm, string codGestorOper)
        {
            var connection = _dapperConnection.GetConnection();

            try
            {
                const string sqlColaborador = @"
                                        SELECT DISTINCT
                                           vgco.codigo_interno_colaborador_gestor as CodigoInternoColaboradorGestorAdm,
                                           vgco.nome_completo_gestor as NomeGestorAdm,
                                           tc.codigo_interno_colaborador as CodigoInternoColaborador, 
                                           tc.nome_completo as NomeColaborador,
                                           tg.codigo_interno_colaborador AS CodigoGestorOperacional, 
                                           tg.nome_completo AS NomeGestorOperacional
                                        FROM vw_gestores_colaboradores_org vgco
                                        INNER JOIN tb_colaborador_periodo_alocacao tcp 
                                            ON tcp.codigo_interno_colaborador = vgco.codigo_interno_colaborador_subordinado 
                                        INNER JOIN tb_colaborador tc 
                                            ON tc.codigo_interno_colaborador = tcp.codigo_interno_colaborador
                                        LEFT JOIN tb_projeto_org tpo 
                                            ON tpo.cod_projeto = tcp.codigo_projeto 
                                           AND tcp.tb_org_id = tpo.tb_org_id
                                        LEFT JOIN tb_projeto_gerente tpg 
                                            ON tpo.cod_projeto = tpg.cod_projeto AND tpg.tb_org_id = tcp.tb_org_id
                                        LEFT JOIN tb_colaborador_org tco_g 
                                            ON tpg.cod_colaborador_gerente = tco_g.cod_colaborador_externo 
                                            AND tco_g.tb_org_id = tcp.tb_org_id
                                        LEFT JOIN tb_colaborador tg 
                                            ON tco_g.codigo_interno_colaborador = tg.codigo_interno_colaborador
                                        WHERE (vgco.codigo_interno_colaborador_gestor = @codGestorAdm OR @codGestorAdm IS NULL OR @codGestorAdm = '')
                                          AND (tg.codigo_interno_colaborador = @codGestorOper OR @codGestorOper IS NULL OR @codGestorOper = '')
                                          AND vgco.tb_org_id = @OrgId 
                                          AND tcp.tb_org_id = @OrgId  
                                          AND tcp.ativo = 1
                                          AND tc.ativo = 1
                                          AND tcp.data_fim >= CURDATE();";

                var colaboradores = (await connection.QueryAsync<MinhaEquipeDTO>(
                    sqlColaborador,
                    new { OrgId = orgId, codGestorAdm, codGestorOper }
                )).ToList();

                if (!colaboradores.Any())
                    return new List<TotalizacaoIndicadoresPorGestorAdmOper>();

                // Agrupa por Gestor Administrativo
                var gruposGestores = colaboradores
                    .GroupBy(c => new
                    {
                        c.CodigoInternoColaboradorGestorAdm,
                        c.NomeGestorAdm
                    })
                    .ToList();

                var resultadoFinal = new List<TotalizacaoIndicadoresPorGestorAdmOper>();

                double totMatch = 0;
                int totColaboradores = 0;
                int totPendentes = 0;
                int totGestores = 0;

                // Percorre cada grupo de gestor
                foreach (var grupo in gruposGestores)
                {                    
                    double somaMatch = 0;
                    int nColaboradores = 0;

                    foreach (var colaborador in grupo.DistinctBy(c => c.CodigoInternoColaborador))
                    {
                        var colabList = await _minhaJornadaRepository.BuscarSkillColaborador(colaborador.CodigoInternoColaborador);
                        var vagaList = await _minhaJornadaRepository.BuscarSkillColaboradorAlocado(colaborador.CodigoInternoColaborador, orgId);

                        var skillsColaborador = (colabList ?? new List<MinhaJornadaColaboradorDTO>())
                            .SelectMany(c => c.ColaboradorHabilidades)
                            .ToList();

                        var vagas = (vagaList ?? new List<MinhaJornadaDTO>()).ToList();
                        if (!vagas.Any() || vagas.FirstOrDefault()?.IdAlocacao == 0)
                            continue;

                        nColaboradores++;

                        var hardSkills = new List<SkillItem>();
                        var softSkills = new List<SkillItem>();
                        var metodologias = new List<SkillItem>();
                        var dominiosNegocio = new List<SkillItem>();
                        var idiomas = new List<SkillItem>();

                        int pendenciasColaborador = 0;

                        foreach (var cliente in vagas.SelectMany(v => v.Clientes))
                        {
                            foreach (var skillVaga in cliente.Habilidades)
                            {
                                var skillCol = skillsColaborador.FirstOrDefault(s => s.SkillId == skillVaga.SkillId && s.PerfilTipoId == skillVaga.PerfilTipoId);
                                int vagaNivelId = MapearSenioridade(skillVaga.Senioridade);
                                int colNivelId = MapearSenioridade(skillCol?.Senioridade);

                                if (skillCol == null || colNivelId < vagaNivelId)
                                    pendenciasColaborador++;

                                if (skillCol != null)
                                {
                                    var skillItem = new SkillItem
                                    {
                                        Nome = skillCol.Habilidade,
                                        Nivel = skillCol.Senioridade,
                                        Obrigatoriedade = (skillCol.Interesse ?? 0) == 1 ? "obrigatorio" : "desejavel"
                                    };

                                    AdicionarSkillNaListaCorreta(skillCol.TipoPerfil, skillItem,
                                                                 hardSkills, softSkills, metodologias, dominiosNegocio, idiomas);

                                }
                            }
                        }

                        // Chamada ao serviço de Match
                        var baseRequest = new ScoreSingleCandidateRequest
                        {
                            HardSkills = hardSkills,
                            SoftSkills = softSkills,
                            Metodologias = metodologias,
                            DominiosNegocio = dominiosNegocio,
                            Idiomas = idiomas,
                            PesoHardSkills = 1,
                            PesoSoftSkills = 1,
                            PesoMetodologias = 1,
                            PesoDominiosNegocio = 1,
                            PesoIdiomas = 1,
                            PesoDisponibilidades = 1,
                            VisibleToOrgIds = new List<int> { orgId },
                            Disponibilidades = new List<DisponibilidadeItem>(),
                            NumeroDeCandidatos = 1,
                            CodigoInternoColaborador = colaborador.CodigoInternoColaborador
                        };

                        //var scoreResult = await _matchClient.ScoreSingleCandidate(baseRequest);

                        totGestores++;
                        totPendentes += pendenciasColaborador;
                        //somaMatch += scoreResult?.Match ?? 0;
                    }

                    totColaboradores = totColaboradores + nColaboradores;
                    totMatch = totMatch + somaMatch;

                }
                // Calcula média de match do grupo (gestor)
                double mediaMatch = totColaboradores > 0
                    ? Math.Round(totMatch / totColaboradores, 2)
                    : 0;

                resultadoFinal.Add(new TotalizacaoIndicadoresPorGestorAdmOper
                {
                    TotColaboradores = totColaboradores,
                    TotPendentesSkills = totPendentes,
                    MediaMatch = mediaMatch
                });


                return resultadoFinal;
            }
            finally
            {
                connection.Dispose();
            }
        }

        private int MapearSenioridade(string senioridade)
        {
            if (string.IsNullOrWhiteSpace(senioridade))
                return 0;

            switch (senioridade.Trim().ToLower())
            {
                case "a definir":
                case "não definido":
                    return 0;
                case "básico":
                case "basico":
                case "iniciante":
                case "trainee":
                    return 1;
                case "intermediário":
                case "intermediario":
                case "junior":
                case "júnior":
                    return 2;
                case "avançado":
                case "avancado":
                case "pleno":
                    return 3;
                case "fluente":
                case "sênior":
                case "senior":
                    return 4;
                case "nativo":
                case "especialista":
                    return 5;
                default:
                    return 0;
            }
        }
        private void AdicionarSkillNaListaCorreta(
                                                string tipoPerfil,
                                                SkillItem skillItem,
                                                List<SkillItem> hardSkills,
                                                List<SkillItem> softSkills,
                                                List<SkillItem> metodologias,
                                                List<SkillItem> dominiosNegocio,
                                                List<SkillItem> idiomas)
        {
            if (string.IsNullOrEmpty(tipoPerfil))
                return;

            switch (tipoPerfil.Trim().ToUpper())
            {
                case "COMPETENCIA":
                case "COMPETÊNCIA":
                    hardSkills.Add(skillItem);
                    break;

                case "SOFTSKILL":
                    softSkills.Add(skillItem);
                    break;

                case "METODOLOGIA":
                    metodologias.Add(skillItem);
                    break;

                case "DOMINIONEGOCIO":
                case "DOMÍNIO":
                case "DOMINIO":
                    dominiosNegocio.Add(skillItem);
                    break;

                case "IDIOMA":
                    idiomas.Add(skillItem);
                    break;
            }
        }

    }
}
