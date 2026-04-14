using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Util;
using Colaboracao.Helper.Util.Competencia;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Dapper;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.ClienteOrg;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.ColaboradoresAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao.Permanencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil.ModeloTrabalho;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil.ProfissionalLocalidade;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.RateCardsDosPerfis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.MapaAlocacao.GestaoDeAlocados
{
    public class GestaoAlocadosRepository : IGestaoAlocadosRepository
    {
        private readonly IDBConnection _dapperConnection;
        private IGestorExternoRepository _gestorExternoRepository;

        public GestaoAlocadosRepository(IDBConnection dapperConnection, IGestorExternoRepository gestorExternoRepository)
        {
            _dapperConnection = dapperConnection;
            _gestorExternoRepository = gestorExternoRepository;
        }

        public async Task<IEnumerable<ClienteOrgDaGestaoAlocadosResult>> ListarClienteOrgDaGestaoDeAlocados(
            string buscaCodigoOuNome,
            int limite,
            int cursor,
            bool semPerfil,
            int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"SELECT
                            tco.nome_cliente AS NomeCliente,
                            tco.id AS Id,
                            tco.codigo_cliente AS CodigoCliente,
                            IFNULL(QtdAlocados.QtdAlocados, 0) AS QtdAlocados,
                            IFNULL(QtdGestoresSemPerfil.QtdGestoresSemPerfil, 0) AS QtdGestoresSemPerfil,
                            IFNULL(QtdGestores.QtdGestores, 0) AS QtdGestores
                        FROM
                            tb_cliente_org tco
                        LEFT JOIN (
                            SELECT
                                tpo.cod_cliente,
                                tpo.tb_org_id,
                                COUNT(tcpa.codigo_colaborador) AS QtdAlocados
                            FROM
                                tb_colaborador_periodo_alocacao tcpa
                            JOIN
                                tb_projeto_org tpo ON tcpa.codigo_projeto = tpo.cod_projeto
                                                  AND tcpa.tb_org_id = tpo.tb_org_id
	                        JOIN tb_colaborador_org tco 
    	                        ON tcpa.codigo_interno_colaborador = tco.codigo_interno_colaborador 
   	 	                        AND tcpa.tb_org_id = tco.tb_org_id     
                            LEFT JOIN tb_projeto_gerente tpg 
    	                        ON tpo.cod_projeto = tpg.cod_projeto AND tpg.tb_org_id = tcpa.tb_org_id                            
    	                    LEFT JOIN tb_perfil_alocacao tpa 
                                ON tcpa.id = tpa.tb_colaborador_periodo_alocacao_id
                            LEFT JOIN tb_gestor_externo_perfil tgep 
                                ON tpa.tb_gestor_externo_perfil_id = tgep.id AND tgep.ativo = 1
                            WHERE
                                CURRENT_DATE() BETWEEN tcpa.data_inicio AND tcpa.data_fim
                                AND tcpa.ativo = 1 AND tcpa.tb_org_id = @OrgId
                                AND tcpa.codigo_colaborador IS NOT NULL -- somente não tbds
                                AND tco.ativo = 1
                                AND tcpa.ativo = 1   
                                AND tgep.id IS NOT NULL 
                            GROUP BY
                                tpo.cod_cliente,
                                tpo.tb_org_id
                        ) QtdAlocados ON QtdAlocados.cod_cliente = tco.codigo_cliente
                                    AND QtdAlocados.tb_org_id = tco.tb_org_id
                        LEFT JOIN (
                            SELECT
                                tge.codigo_cliente,
                                tge.tb_org_id,
                                COUNT(*) AS QtdGestoresSemPerfil
                            FROM
                                tb_gestor_externo tge
                            LEFT JOIN
                                tb_gestor_externo_perfil tgep ON tge.cod_gestor_externo = tgep.cod_gestor_externo
                                                            AND tge.tb_org_id = tgep.tb_org_id
                            WHERE
                                tgep.cod_gestor_externo IS NULL and tge.tb_org_id = @OrgId
                            GROUP BY
                                tge.codigo_cliente,
                                tge.tb_org_id
                        ) QtdGestoresSemPerfil ON QtdGestoresSemPerfil.codigo_cliente = tco.codigo_cliente
                                            AND QtdGestoresSemPerfil.tb_org_id = tco.tb_org_id
                        LEFT JOIN (
                            SELECT
                                tge.codigo_cliente,
                                tge.tb_org_id,
                                COUNT(*) AS QtdGestores
                            FROM
                                tb_gestor_externo tge
                            WHERE
                                tge.tb_org_id = @OrgId
                            GROUP BY
                                tge.codigo_cliente,
                                tge.tb_org_id
                        ) QtdGestores ON QtdGestores.codigo_cliente = tco.codigo_cliente
                                    AND QtdGestores.tb_org_id = tco.tb_org_id
                        WHERE
                            tco.ativo = 1
                            AND tco.tb_org_id = @OrgId
                            AND tco.deve_ocultar_na_gestao_de_alocados = 0
                            AND (
                                @SemPerfil = 0
                                OR (
                                    @SemPerfil = 1
                                    AND QtdGestoresSemPerfil.QtdGestoresSemPerfil > 0
                                )
                            )
                            AND (
                                @Busca IS NULL
                                OR (
                                    tco.nome_cliente LIKE @BuscaLike
                                    OR tco.codigo_cliente LIKE @BuscaLike
                                )
                            )
                        GROUP BY
                            tco.nome_cliente,
                            tco.id,
                            tco.codigo_cliente
                        ORDER BY
                            tco.nome_cliente
                        LIMIT
                            @Limite
                        OFFSET
                            @Cursor;
                        ";

            // Executando a consulta com os parâmetros para paginação e busca
            var result = await connection.QueryAsync<ClienteOrgDaGestaoAlocadosResult>(
                query,
                new
                {
                    OrgId = orgId,
                    Limite = limite,
                    Cursor = cursor,
                    Busca = buscaCodigoOuNome,
                    BuscaLike = $"%{buscaCodigoOuNome}%",
                    SemPerfil = semPerfil ? 1 : 0
                }
            );

            return result;
        }

        public async Task<List<GestoresEPerfisDaGestaoDeAlocadosResult>> ListarGestoresEPerfisDaGestaoDeAlocados(int limite, int cursor, string busca, string codigoCliente, bool semPerfil, bool semAreaAtuacao, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
            SELECT DISTINCT
                tge.nome AS NomeGestorExterno,
                tge.cod_gestor_externo AS CodGestorExterno,
                tgep.id AS GestorExternoPerfilId,
                tgep.nome_perfil AS GestorExternoPerfilNome,
                tge.codigo_cliente AS CodCliente,
                tge.codigo_interno_colaborador AS CodigoInternoColaborador,
                tc.url_linkedin AS LinkLinkedin,
                CASE WHEN TRIM(COALESCE(tc.url_linkedin, '')) = ''
                     THEN false ELSE true END AS PossuiLinkedin
            FROM tb_gestor_externo tge
            LEFT JOIN tb_gestor_externo_perfil tgep
              ON tge.cod_gestor_externo = tgep.cod_gestor_externo
             AND tge.tb_org_id = tgep.tb_org_id
             AND COALESCE(tgep.ativo, 1)
            LEFT JOIN tb_gestor_externo_area_atuacao tgeaa
              ON tge.cod_gestor_externo = tgeaa.cod_gestor_externo
             AND tge.tb_org_id = tgeaa.tb_org_id
            LEFT JOIN tb_area_atuacao tag
              ON tgeaa.tb_area_atuacao_id = tag.id
            LEFT JOIN tb_colaborador tc
              ON tc.codigo_interno_colaborador = tge.codigo_interno_colaborador
            LEFT JOIN tb_cliente_org tco
              ON tge.codigo_cliente = tco.codigo_cliente
             AND tge.tb_org_id = tco.tb_org_id
            WHERE tge.ativo = 1
              AND tge.tb_org_id = @OrgId
              AND tco.deve_ocultar_na_gestao_de_alocados = 0
              AND (@CodigoCliente IS NULL OR tge.codigo_cliente = @CodigoCliente)
              AND (
                  (@SemPerfil = 0 OR (@SemPerfil = 1 AND tgep.id IS NULL))
                  AND (@SemAreaAtuacao = 0 OR (@SemAreaAtuacao = 1 AND tgeaa.tb_area_atuacao_id IS NULL))
                  AND (
                      @Busca IS NULL
                      OR (
                          CAST(LOWER(tge.nome) AS CHAR CHARACTER SET utf8mb4)         LIKE CONCAT('%', @Busca, '%') OR
                          CAST(LOWER(tgep.nome_perfil) AS CHAR CHARACTER SET utf8mb4) LIKE CONCAT('%', @Busca, '%') OR
                          CAST(LOWER(tag.descricao) AS CHAR CHARACTER SET utf8mb4)    LIKE CONCAT('%', @Busca, '%')
                      )
                  )
              )
            ORDER BY tge.nome
            LIMIT @Limite
            OFFSET @Offset;";



            // Executa a consulta com os parâmetros fornecidos
            var gestores = (await connection.QueryAsync<GestoresEPerfisDaGestaoDeAlocadosResult>(
                query,
                new
                {
                    OrgId = orgId,
                    CodigoCliente = codigoCliente,
                    Busca = string.IsNullOrWhiteSpace(busca) ? null : busca.ToLower(),
                    Limite = limite,
                    Offset = cursor,
                    SemPerfil = semPerfil ? 1 : 0,
                    SemAreaAtuacao = semAreaAtuacao ? 1 : 0
                }
            )).ToList();

            // Mantém a lógica original de preenchimento das áreas de atuação
            foreach (var gestor in gestores)
            {
                gestor.AreasDeAtuacaoGestorExterno = await _gestorExternoRepository.ObterAreasDeAtuacaoPorGestorAsync(gestor.CodGestorExterno);
            }

            return gestores;
        }

        public async Task<List<RateCardsDosPerfisResult>> ListarRateCardsDosPerfisPorCliente(int limite, int cursor, string busca, string codigoCliente, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                             SELECT DISTINCT
                                 tge.nome AS NomeGestorExterno,
                                 tgep.nome_perfil AS GestorExternoPerfilNome,
                                 tge.cod_gestor_externo AS CodGestorExterno,
                                 tgep.id AS GestorExternoPerfilId,
                                 tgep.custo_perfil AS CustoPerfil,
                                 tgep.ratecard_perfil AS RatecardPerfil,
                                 tge.codigo_cliente AS CodCliente
                             FROM
                                 tb_gestor_externo tge
                             LEFT JOIN
                                 tb_gestor_externo_perfil tgep
                                 ON tge.cod_gestor_externo = tgep.cod_gestor_externo AND tge.tb_org_id = tgep.tb_org_id
                             WHERE
                                 tge.ativo = 1
                                 AND tgep.ativo = 1
                                 AND tge.tb_org_id = @OrgId
                                 AND tge.codigo_cliente = @CodigoCliente
                                 AND (
                                         (@Busca IS NULL OR (
                                             LOWER(tge.nome) LIKE CONCAT('%', @Busca, '%') OR
                                             LOWER(tgep.nome_perfil) LIKE CONCAT('%', @Busca, '%')
                                             ))
                                      )
                             ORDER BY
                                 tge.nome
                            LIMIT @Limite OFFSET @Offset;";

            // Executa a consulta com os parâmetros fornecidos
            var reusultado = (await connection.QueryAsync<RateCardsDosPerfisResult>(
                query,
                new
                {
                    OrgId = orgId,
                    CodigoCliente = codigoCliente,
                    Busca = string.IsNullOrWhiteSpace(busca) ? null : busca.ToLower(),
                    Limite = limite,
                    Offset = cursor
                }
            )).ToList();

            return reusultado;
        }

        public async Task<List<RateCardsDosPerfisHistoricoResult>> ListarRateCardsDosPerfisHistoricoPorPerfilId(int limite, int cursor, Guid gestorExternoPerfilId, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                            SELECT
                                nome_perfil AS GestorExternoPerfilNome,
                                data_alteracao AS DataAlteracao,
                                custo_perfil AS CustoPerfil,
                                ratecard_perfil AS RatecardPerfil,
                                descricao_modalidade_trabalho as DescricaoModalidadeTrabalho
                            FROM
                                (
                                SELECT
                                    tgepl.nome_perfil,
                                    tgepl.data_alteracao,
                                    tgepl.custo_perfil,
                                    tgepl.ratecard_perfil,
                                    tmt.descricao AS descricao_modalidade_trabalho,
                                    ROW_NUMBER() OVER (
                                        PARTITION BY
                                            tgepl.nome_perfil, tgepl.custo_perfil, tgepl.ratecard_perfil, tmt.descricao
                                        ORDER BY tgepl.data_alteracao DESC
                                    ) AS data_alteracao_prioridade
                                FROM
                                    tb_gestor_externo tge
                                LEFT JOIN
                                    tb_gestor_externo_perfil tgep
                                    ON tge.cod_gestor_externo = tgep.cod_gestor_externo AND tge.tb_org_id = tgep.tb_org_id
                                LEFT JOIN
                                    tb_gestor_externo_perfil_log tgepl
                                    ON tgep.id = tgepl.tb_gestor_externo_perfil_id
                                LEFT JOIN
                                    tb_modelo_trabalho tmt
                                    ON tgepl.tb_modelo_trabalho_id = tmt.id
                                WHERE
                                    tge.ativo = 1
                                    AND tgep.ativo = 1
                                    AND tge.tb_org_id = @OrgId
                                    AND tgep.id = @GestorExternoPerfilId
                            ) as RankedData
                            WHERE
                                data_alteracao_prioridade = 1 -- filtras apenas ultima data quando info iguais
                            ORDER BY
                                data_alteracao DESC
                            LIMIT @Limite OFFSET @Offset;
                        ";

            // Executa a consulta com os parâmetros fornecidos
            var reusultado = (await connection.QueryAsync<RateCardsDosPerfisHistoricoResult>(
                query,
                new
                {
                    OrgId = orgId,
                    GestorExternoPerfilId = gestorExternoPerfilId,
                    Limite = limite,
                    Offset = cursor
                }
            )).ToList();

            return reusultado;
        }

        public async Task<IEnumerable<PermanenciaResult>> ListarPermanenciasAsync()
        {
            var connection = _dapperConnection.GetConnection();
            // Consulta para listar as permanências em ordenação numérica
            var query = @"
                        SELECT 
                            id as Id,
                            descricao as Descricao,
                            ativo as Ativo, 
                            CASE 
                                WHEN descricao LIKE 'Até%' THEN 0
                                WHEN descricao LIKE 'Maior que%' THEN 999
                                ELSE CAST(REGEXP_REPLACE(descricao, '[^0-9]', '') AS UNSIGNED)
                            END AS Ordem_Numerica
                        FROM tb_permanencia
                        ORDER BY Ordem_Numerica;  ";

            var result = await connection.QueryAsync<PermanenciaResult>(query);

            return result;
        }

        public async Task<IEnumerable<ModeloTrabalhoResult>> ListarModelosTrabalhoAsync()
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT
                    id as Id,
                    descricao as Descricao,
                    ativo as Ativo,
                    codigo as Codigo
                FROM
                    tb_modelo_trabalho;
            ";

            var result = await connection.QueryAsync<ModeloTrabalhoResult>(query);

            return result;
        }

        public async Task<IEnumerable<ProfissionalLocalidadeResult>> ListarProfissionaisLocalidadesAsync()
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT
                    id as Id,
                    descricao as Descricao,
                    ativo as Ativo
                FROM
                    tb_profissional_localidade;
            ";

            var result = await connection.QueryAsync<ProfissionalLocalidadeResult>(query);

            return result;
        }

        public async Task<IEnumerable<string>> ListarPropostasPorCliente(string codCliente, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                  SELECT
                      tpp.cod_proposta
                  FROM
                      tb_projeto_proposta tpp
                  JOIN
                      tb_projeto_org tpo ON tpp.cod_projeto = tpo.cod_projeto
                  WHERE
                      cod_cliente = @CodCliente AND tpp.tb_org_id = @OrgId;";

            var result = await connection.QueryAsync<string>(query, new { OrgId = orgId, CodCliente = codCliente });

            var propostasFormatadas = result.Select(p => p.StartsWith("P") ? p : $"P{p}"); //adicionar "P" na frente de propostas que não começam com "P"

            return propostasFormatadas;
        }

        public async Task<IEnumerable<ColaboradoresAlocadosDaGestaoAlocadosResult>> ListarColaboradoresAlocadosAtualmentePorCliente(int limite, int cursor, string busca, string codCliente, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @$"
                        WITH Cadastro AS (
                            SELECT DISTINCT
                                tco.codigo_interno_colaborador,
                                tc.nome_completo,
                                tco2.tb_org_id,
                                tco2.codigo_interno_colaborador AS CodigoInternoGestorAdm
                            FROM tb_colaborador_org tco
                            JOIN tb_colaborador_hierarquia tch 
                                ON tch.cod_colaborador_externo = tco.cod_colaborador_externo
                            JOIN tb_colaborador_org tco2 
                                ON tco2.cod_colaborador_externo = tch.cod_colaborador_superior
                            JOIN tb_colaborador tc 
                                ON tc.codigo_interno_colaborador = tco2.codigo_interno_colaborador
                            WHERE tco2.tb_org_id = @OrgId
                        )
                        SELECT
                            tc.codigo_interno_colaborador AS CodigoInternoColaborador,
                            tc.nome_completo AS Colaborador,
                            tgep.id AS PerfilId,
                            tgep.nome_perfil AS Perfil,
                            tge.cod_gestor_externo AS CodGestorCliente,
                            tge.nome AS GestorCliente,
                            cadastro.CodigoInternoGestorAdm AS CodigoInternoGestorAdm,
                            cadastro.nome_completo AS GestorAdm,
                            tg.nome_completo AS NomeGerente
                        FROM tb_colaborador_periodo_alocacao tcpa
                        JOIN tb_colaborador tc 
                            ON tcpa.codigo_interno_colaborador = tc.codigo_interno_colaborador
                        JOIN tb_colaborador_org tco 
                            ON tcpa.codigo_interno_colaborador = tco.codigo_interno_colaborador 
                            AND tcpa.tb_org_id = tco.tb_org_id
                        JOIN tb_projeto_org tpo 
                            ON tpo.cod_projeto = tcpa.codigo_projeto 
                            AND tcpa.tb_org_id = tpo.tb_org_id
                        LEFT JOIN tb_perfil_alocacao tpa 
                            ON tcpa.id = tpa.tb_colaborador_periodo_alocacao_id
                        LEFT JOIN tb_gestor_externo_perfil tgep 
                            ON tpa.tb_gestor_externo_perfil_id = tgep.id AND tgep.ativo = 1
                        LEFT JOIN tb_gestor_externo tge 
                            ON tgep.cod_gestor_externo = tge.cod_gestor_externo AND tgep.tb_org_id = tge.tb_org_id
                        LEFT JOIN Cadastro cadastro 
                            ON cadastro.codigo_interno_colaborador = tc.codigo_interno_colaborador
                        LEFT JOIN tb_projeto_gerente tpg 
                            ON tpo.cod_projeto = tpg.cod_projeto AND tpg.tb_org_id = tcpa.tb_org_id
                        LEFT JOIN tb_colaborador_org tco_g 
                            ON tpg.cod_colaborador_gerente = tco_g.cod_colaborador_externo 
                            AND tco_g.tb_org_id = tcpa.tb_org_id
                        LEFT JOIN tb_colaborador tg 
                            ON tco_g.codigo_interno_colaborador = tg.codigo_interno_colaborador
                        WHERE
                            tcpa.tb_org_id = @OrgId
                            AND tco.ativo = 1
                            AND tcpa.ativo = 1
                            AND tcpa.codigo_colaborador IS NOT NULL
                            AND (@CodCliente IS NULL OR @CodCliente = '' OR tpo.cod_cliente = @CodCliente)
                            AND (@Busca IS NULL OR @Busca = '' OR (
                                tge.nome LIKE CONCAT('%', @Busca, '%')
                                OR tgep.nome_perfil LIKE CONCAT('%', @Busca, '%')
                                OR tc.nome_completo LIKE CONCAT('%', @Busca, '%')
                                OR cadastro.nome_completo LIKE CONCAT('%', @Busca, '%')
                                OR tg.nome_completo LIKE CONCAT('%', @Busca, '%')
                            ))
                            AND CURRENT_DATE() BETWEEN tcpa.data_inicio AND tcpa.data_fim
                            AND tgep.id IS NOT NULL 
                            ORDER BY
                            tge.nome,
                            tc.nome_completo
                        LIMIT @Limite
                        OFFSET @Cursor
                        ;";

            try
            {
                var result = await connection.QueryAsync<dynamic>(
                    query,
                    new
                    {
                        Busca = busca,
                        CodCliente = codCliente,
                        OrgId = orgId,
                        Limite = limite,
                        Cursor = cursor
                    }
                );

                var alocados = result.GroupBy(x => new
                {
                    x.CodigoInternoColaborador,
                    x.Colaborador,
                    x.PerfilId,
                    x.Perfil,
                    x.CodGestorCliente,
                    x.GestorCliente,
                    x.CodigoInternoGestorAdm,
                    x.GestorAdm,
                    x.NomeGerente
                }).Select(group => new ColaboradoresAlocadosDaGestaoAlocadosResult
                {
                    CodigoInternoColaborador = group.Key.CodigoInternoColaborador,
                    Colaborador = group.Key.Colaborador,
                    PerfilId = group.Key.PerfilId,
                    Perfil = group.Key.Perfil,
                    CodGestorCliente = group.Key.CodGestorCliente,
                    GestorCliente = group.Key.GestorCliente,
                    CodigoInternoGestorAdm = group.Key.CodigoInternoGestorAdm,
                    GestorAdm = group.Key.GestorAdm,
                    GestorProjeto = group.Key.NomeGerente
                }).ToList();

                return alocados;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<ColaboradoresAlocadosDaGestaoAlocadosResult>> ListarColaboradoresAlocadosPorClienteCompletoDetalhe(int limite, int cursor, string codCliente, int orgId, string codGestorAdm, string codGestorOper)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @$"
                        WITH Cadastro AS (
                            SELECT DISTINCT
                                tco.codigo_interno_colaborador,
                                tc.nome_completo,
                                tco2.tb_org_id,
                                tco2.codigo_interno_colaborador AS CodigoInternoGestorAdm
                            FROM tb_colaborador_org tco
                            JOIN tb_colaborador_hierarquia tch 
                                ON tch.cod_colaborador_externo = tco.cod_colaborador_externo
                            JOIN tb_colaborador_org tco2 
                                ON tco2.cod_colaborador_externo = tch.cod_colaborador_superior
                            JOIN tb_colaborador tc 
                                ON tc.codigo_interno_colaborador = tco2.codigo_interno_colaborador
                            WHERE tco2.tb_org_id = @OrgId
                        )
                        SELECT
                            tc.codigo_interno_colaborador AS CodigoInternoColaborador,
                            tc.nome_completo AS Colaborador,
                            tgep.id AS PerfilId,
                            tgep.nome_perfil AS Perfil,
                            tge.cod_gestor_externo AS CodGestorCliente,
                            tge.nome AS GestorCliente,
                            cadastro.CodigoInternoGestorAdm AS CodigoInternoGestorAdm,
                            cadastro.nome_completo AS GestorAdm,
                            tg.codigo_interno_colaborador AS CodGestorOperacional, 
                            tg.nome_completo AS NomeGerente
                        FROM tb_colaborador_periodo_alocacao tcpa
                        JOIN tb_colaborador tc 
                            ON tcpa.codigo_interno_colaborador = tc.codigo_interno_colaborador
                        JOIN tb_colaborador_org tco 
                            ON tcpa.codigo_interno_colaborador = tco.codigo_interno_colaborador 
                            AND tcpa.tb_org_id = tco.tb_org_id
                        JOIN tb_projeto_org tpo 
                            ON tpo.cod_projeto = tcpa.codigo_projeto 
                            AND tcpa.tb_org_id = tpo.tb_org_id
                        LEFT JOIN tb_perfil_alocacao tpa 
                            ON tcpa.id = tpa.tb_colaborador_periodo_alocacao_id
                        LEFT JOIN tb_gestor_externo_perfil tgep 
                            ON tpa.tb_gestor_externo_perfil_id = tgep.id AND tgep.ativo = 1
                        LEFT JOIN tb_gestor_externo tge 
                            ON tgep.cod_gestor_externo = tge.cod_gestor_externo AND tgep.tb_org_id = tge.tb_org_id
                        LEFT JOIN Cadastro cadastro 
                            ON cadastro.codigo_interno_colaborador = tc.codigo_interno_colaborador
                        LEFT JOIN tb_projeto_gerente tpg 
                            ON tpo.cod_projeto = tpg.cod_projeto AND tpg.tb_org_id = tcpa.tb_org_id
                        LEFT JOIN tb_colaborador_org tco_g 
                            ON tpg.cod_colaborador_gerente = tco_g.cod_colaborador_externo 
                            AND tco_g.tb_org_id = tcpa.tb_org_id
                        LEFT JOIN tb_colaborador tg 
                            ON tco_g.codigo_interno_colaborador = tg.codigo_interno_colaborador
                        WHERE
                            tcpa.tb_org_id = @OrgId
                            AND tco.ativo = 1
                            AND tcpa.ativo = 1
                            AND tcpa.codigo_colaborador IS NOT NULL
                            AND (@CodCliente IS NULL OR @CodCliente = '' OR tpo.cod_cliente = @CodCliente)
                            AND (@CodGestorAdm IS NULL OR @CodGestorAdm = '' OR cadastro.CodigoInternoGestorAdm = @CodGestorAdm)
                            AND (@CodGestorOper IS NULL OR @CodGestorOper = '' OR tg.codigo_interno_colaborador = @CodGestorOper)
                            AND CURRENT_DATE() BETWEEN tcpa.data_inicio AND tcpa.data_fim
                            AND tgep.id IS NOT NULL 
                            ORDER BY
                            tge.nome,
                            tc.nome_completo
                        LIMIT @Limite
                        OFFSET @Cursor
                        ;";

            try
            {
                var result = await connection.QueryAsync<dynamic>(
                    query,
                    new
                    {
                        CodCliente = codCliente,
                        OrgId = orgId,
                        Limite = limite,
                        Cursor = cursor,
                        CodGestorAdm = codGestorAdm,        
                        CodGestorOper = codGestorOper
                    }
                );

                var alocados = result.GroupBy(x => new
                {
                    x.CodigoInternoColaborador,
                    x.Colaborador,
                    x.PerfilId,
                    x.Perfil,
                    x.CodGestorCliente,
                    x.GestorCliente,
                    x.CodigoInternoGestorAdm,
                    x.GestorAdm,
                    x.CodGestorOperacional,
                    x.NomeGerente
                }).Select(group => new ColaboradoresAlocadosDaGestaoAlocadosResult
                {
                    CodigoInternoColaborador = group.Key.CodigoInternoColaborador,
                    Colaborador = group.Key.Colaborador,
                    PerfilId = group.Key.PerfilId,
                    Perfil = group.Key.Perfil,
                    CodGestorCliente = group.Key.CodGestorCliente,
                    GestorCliente = group.Key.GestorCliente,
                    CodigoInternoGestorAdm = group.Key.CodigoInternoGestorAdm,
                    GestorAdm = group.Key.GestorAdm,
                    CodGestorOperacional = group.Key.CodGestorOperacional,
                    GestorProjeto = group.Key.NomeGerente
                }).ToList();

                return alocados;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}