using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Extension;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Dapper;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DotNetEnv;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Colaboracao.Helper.Extension;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.MapaDemografico;

namespace Colaboracao.Infra.Repositories.MapaAlocacao.GestaoDeAlocados
{
    public class MapaDemograficoRepository : IMapaDemograficoRepository
    {
        //Teste para subida p Main
        private readonly IDBConnection _dapperConnection;
        private IConnectionStringCore _connectionString;

        public MapaDemograficoRepository(IDBConnection dapperConnection, IConnectionStringCore connectionString)
        {
            _dapperConnection = dapperConnection;
            _connectionString = connectionString;
        }

        // ───────────────────────────────────────────────
        // Busca Banco de Talentos
        // ───────────────────────────────────────────────
        public async Task<List<MapaDemograficoDTO>> BuscarBancoTalentosOrgId( List<string>? diretorias, int? orgID, string talento)
        {
            if (string.IsNullOrWhiteSpace(talento))
            {
                return new List<MapaDemograficoDTO>();
            }

            //Abre conexao
            var connection = _dapperConnection.GetConnection();
                
            var inClauseUnidades = diretorias.BuildInClauseOrNull();
            
            try
            {
                string query = @$"
                        SELECT tc.nome_completo as 'NomeCompleto',
                               tc.codigo_interno_colaborador as 'CodigoInternoColaborador',
                               tc.data_nascimento as 'DataNascimento',
                               tco.data_admissao as 'DataAdmissao',
                               DATEDIFF(CURRENT_DATE(), IFNULL(tco.data_admissao, CURRENT_DATE())) as 'TempoCasa',          
                               tc.genero as 'Genero',
                               tc.etnia as 'Etnia',
                               tc.orientacao_sexual as 'OrientacaoSexual',
                               tc.escolaridade as 'Escolaridade',
                               tp.descricao as 'LocalVisto',
                               tcv.validade as 'ValidadeVisto',
                               tc.email_alternativo as 'Email',        
                               tbe.cidade as 'Cidade',
                               tbe.estado as 'Estado',
                               tco.modelo_trabalho as 'ModeloTrabalho',
                               tco.diretoria as 'Unidade',
                               {orgID} as 'OrgId',
                               tor.descricao as 'Organizacao',
                               '{talento.ToUpper()}' as Talento,
                               CASE WHEN tge.codigo_interno_colaborador IS NULL THEN 'N' ELSE 'S' END as Gestor,
                               tco.ativo as Ativo";

                if (orgID.IsNotNull() && talento.ToUpperInvariant() == "N")
                {
                    query = query + $@"  
                           FROM tb_colaborador_org tco
                           LEFT JOIN tb_colaborador tc ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador 
                           LEFT JOIN tb_org tor ON tor.id = tco.tb_org_id
                           LEFT JOIN tb_colaborador_visto tcv ON tcv.codigo_interno_colaborador = tc.codigo_interno_colaborador    
                           LEFT JOIN tb_pais tp ON tp.id = tcv.tb_pais_id
                           LEFT JOIN tb_endereco tbe ON tbe.id = tc.endereco_id and tbe.ativo = 1
                           LEFT JOIN vw_gestores_estatisticas_org AS tge ON tge.codigo_interno_colaborador = tc.codigo_interno_colaborador
                           LEFT JOIN tb_banco_talentos tbt ON tco.codigo_interno_colaborador = tbt.codigo_interno_colaborador                            
                           WHERE (@OrgID IS NULL OR tco.tb_org_id = @OrgID) 
                           {(inClauseUnidades == null ? "" : $"AND tco.cod_diretoria IN {inClauseUnidades}")}
                           AND  tbt.codigo_interno_colaborador IS NULL 
                           ORDER BY tco.codigo_interno_colaborador;  ";
                }
                else if (orgID.IsNotNull() && talento.ToUpperInvariant() == "S")
                {
                    query = query + @"  
                           FROM tb_banco_talentos tbt
                           LEFT JOIN tb_colaborador tc ON tbt.codigo_interno_colaborador = tc.codigo_interno_colaborador 
                           LEFT JOIN tb_colaborador_org tco on tbt.codigo_interno_colaborador = tco.codigo_interno_colaborador and (@OrgId is null OR  tbt.tb_org_id = @OrgId)
                           LEFT JOIN tb_org tor ON tor.id = tco.tb_org_id
                           LEFT JOIN tb_colaborador_visto tcv ON tcv.codigo_interno_colaborador = tc.codigo_interno_colaborador    
                           LEFT JOIN tb_pais tp ON tp.id = tcv.tb_pais_id
                           LEFT JOIN tb_endereco tbe ON tbe.id = tc.endereco_id and tbe.ativo = 1
                           LEFT JOIN vw_gestores_estatisticas_org AS tge ON tge.codigo_interno_colaborador = tc.codigo_interno_colaborador
                           WHERE (@OrgID IS NULL OR tbt.tb_org_id = @OrgID) 
                           ORDER BY tbt.codigo_interno_colaborador;  ";
                }
                else
                {
                    query = @$"
                        SELECT tc.nome_completo as 'NomeCompleto',
                               tc.codigo_interno_colaborador as 'CodigoInternoColaborador',
                               tc.data_nascimento as 'DataNascimento',
                               tco.data_admissao as 'DataAdmissao',
                               DATEDIFF(CURRENT_DATE(), IFNULL(tco.data_admissao, CURRENT_DATE())) as 'TempoCasa',          
                               tc.genero as 'Genero',
                               tc.etnia as 'Etnia',
                               tc.orientacao_sexual as 'OrientacaoSexual',
                               tc.escolaridade as 'Escolaridade',
                               tp.descricao as 'LocalVisto',
                               tcv.validade as 'ValidadeVisto',
                               tc.email_alternativo as 'Email',        
                               tbe.cidade as 'Cidade',
                               tbe.estado as 'Estado',
                               tco.modelo_trabalho as 'ModeloTrabalho',
                               tco.diretoria as 'Unidade',
                               CASE WHEN tco.tb_org_id IS NULL and tbt.data_criacao IS NOT NULL THEN tbt.tb_org_id ELSE tco.tb_org_id END as 'OrgId',  
                               tor.descricao as 'Organizacao',
                               CASE WHEN tbt.data_criacao IS NULL THEN 'N' ELSE 'S' END as Talento,
                               CASE WHEN tge.codigo_interno_colaborador IS NULL THEN 'N' ELSE 'S' END as Gestor,
                               tco.ativo as Ativo
                        FROM tb_colaborador tc
                        LEFT JOIN tb_colaborador_org tco ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador
                        LEFT JOIN tb_banco_talentos  tbt on tbt.codigo_interno_colaborador = tc.codigo_interno_colaborador
                        LEFT JOIN tb_org tor ON tor.id = tco.tb_org_id
                        LEFT JOIN tb_colaborador_visto tcv ON tcv.codigo_interno_colaborador = tc.codigo_interno_colaborador
                        LEFT JOIN tb_pais tp ON tp.id = tcv.tb_pais_id
                        LEFT JOIN tb_endereco tbe ON tbe.id = tc.endereco_id AND tbe.ativo = 1
                        LEFT JOIN vw_gestores_estatisticas_org AS tge ON tge.codigo_interno_colaborador = tc.codigo_interno_colaborador
                        WHERE (CASE WHEN tbt.data_criacao IS NULL THEN 'N' ELSE 'S' END) = @Talento 
                        {(inClauseUnidades == null ? "" : $"AND tco.cod_diretoria IN {inClauseUnidades}")}
                        ORDER BY tc.codigo_interno_colaborador;";
                }

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                //var parametros = new { OrgID = orgID, Talento = string.IsNullOrWhiteSpace(talento) ? null : talento };
                var parametros = new { OrgID = orgID, Talento = talento };
                var colaboradores = (await connection.QueryAsync<MapaDemograficoDTO>(query, parametros)).ToList();

                if (colaboradores.Any())
                {
                    await LoadAuxiliaryData(colaboradores);
                }
                return colaboradores;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<MapaDemograficoDTO>> BuscarBancoTalentosCodColaborador(string codColaborador)
        {
            //Abre conexao
            var connection = _dapperConnection.GetConnection();

            try
            {
                const string query = @"
                        SELECT tc.nome_completo as 'NomeCompleto',
                               tc.codigo_interno_colaborador as 'CodigoInternoColaborador',
                               tc.data_nascimento as 'DataNascimento',
                               tco.data_admissao as 'DataAdmissao',
                               DATEDIFF(CURRENT_DATE(), IFNULL(tco.data_admissao, CURRENT_DATE())) as 'TempoCasa',          
                               tc.genero as 'Genero',
                               tc.etnia as 'Etnia',
                               tc.orientacao_sexual as 'OrientacaoSexual',
                               tc.escolaridade as 'Escolaridade',
                               tp.descricao as 'LocalVisto',
                               tcv.validade as 'ValidadeVisto',
                               tc.email_alternativo as 'Email',        
                               tbe.cidade as 'Cidade',
                               tbe.estado as 'Estado',
                               tco.modelo_trabalho as 'ModeloTrabalho',
                               tco.diretoria as 'Unidade',
                               CASE WHEN tco.tb_org_id IS NULL and tbt.data_criacao IS NOT NULL THEN tbt.tb_org_id ELSE tco.tb_org_id END as 'OrgId',
                               tor.descricao as 'Organizacao',
                               CASE WHEN tbt.data_criacao IS NULL THEN 'N' ELSE 'S' END as Talento,
                               CASE WHEN tge.codigo_interno_colaborador IS NULL THEN 'N' ELSE 'S' END as Gestor,
                               tco.ativo as 'Ativo'
                           FROM tb_colaborador tc
                           LEFT JOIN tb_colaborador_org tco ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador
                           LEFT JOIN tb_banco_talentos  tbt on tbt.codigo_interno_colaborador = tc.codigo_interno_colaborador
                           LEFT JOIN tb_org tor ON tor.id = tco.tb_org_id
                           LEFT JOIN tb_colaborador_visto tcv ON tcv.codigo_interno_colaborador = tc.codigo_interno_colaborador    
                           LEFT JOIN tb_pais tp ON tp.id = tcv.tb_pais_id
                           LEFT JOIN tb_endereco tbe ON tbe.id = tc.endereco_id and tbe.ativo = 1
                           LEFT JOIN vw_gestores_estatisticas_org AS tge ON tge.codigo_interno_colaborador = tc.codigo_interno_colaborador
                           WHERE tc.codigo_interno_colaborador  = @codColaborador;";

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                var parametros = new { codColaborador = codColaborador};
                var colaboradores = (await connection.QueryAsync<MapaDemograficoDTO>(query, parametros)).ToList();

                if (colaboradores.Any())
                {
                    await LoadAuxiliaryData(colaboradores);
                }
                return colaboradores;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task LoadAuxiliaryData(List<MapaDemograficoDTO> colaboradores)
        {
            var codigos = colaboradores.Select(c => c.CodigoInternoColaborador).Distinct().ToList();
            if (!codigos.Any()) return;

            // Coleções acumuladoras
            var allHardSkills = new List<HardSkillsDTO>();
            var allSoftSkills = new List<SoftSkillsDTO>();
            var allFormacoes = new List<FormacoesDTO>();
            var allLinguas = new List<LinguasDTO>();
            var allCargos = new List<CargosDTO>();
            var allMetodologias = new List<MetodologiasDTO>();
            var allNacionalidades = new List<NacionalidadesDTO>();
            var allClientes = new List<ClientesDTO>();
            var allGestores = new List<GestoresDTO>();

            // Query única e consolidada — todos os SELECTs em sequência
            const string multiQuery = @"
                            SELECT  tcpp.id AS Id,
                                    tcp.codigo_interno_colaborador AS CodigoInternoColaborador, 
                                    tcpp.descricao AS DescricaoHardSkill, 
                                    tbn.descricao AS Senioridade
                            FROM tb_colaborador_competencia tcp
                            LEFT JOIN tb_competencia tcpp ON tcpp.id = tcp.competencia_id AND tcpp.ativo = 1
                            LEFT JOIN tb_nivel tbn ON tbn.id = tcp.tb_nivel_id AND tbn.ativo = 1
                            WHERE tcp.codigo_interno_colaborador IN @Codigos AND tcpp.id IS NOT NULL;

                            SELECT  tskk.id AS Id, 
                                    tsk.codigo_interno_colaborador AS CodigoInternoColaborador, 
                                    tskk.descricao AS DescricaoSoftSkill, 
                                    tbn.descricao AS Senioridade
                            FROM tb_colaborador_softskill tsk
                            LEFT JOIN tb_softskill tskk ON tskk.id = tsk.softskill_id AND tskk.ativo = 1
                            LEFT JOIN tb_nivel tbn ON tbn.id = tsk.tb_nivel_id AND tbn.ativo = 1
                            WHERE tsk.codigo_interno_colaborador IN @Codigos AND tskk.id IS NOT NULL;

                            SELECT  tfmm.id AS Id, 
                                    tfm.codigo_interno_colaborador AS CodigoInternoColaborador, 
                                    tfmm.descricao AS DescricaoFormacao, 
                                    tbn.descricao AS Senioridade
                            FROM tb_colaborador_formacao tfm
                            LEFT JOIN tb_formacao tfmm ON tfmm.id = tfm.formacao_id AND tfmm.ativo = 1
                            LEFT JOIN tb_nivel tbn ON tbn.id = tfm.tb_nivel_id AND tbn.ativo = 1
                            WHERE tfm.codigo_interno_colaborador IN @Codigos AND tfmm.id IS NOT NULL;

                            SELECT  tcii.id AS Id, 
                                    tci.codigo_interno_colaborador AS CodigoInternoColaborador, 
                                    tcii.descricao AS DescricaoLingua, 
                                    tbn.descricao AS Senioridade
                            FROM tb_colaborador_idioma tci
                            LEFT JOIN tb_idioma tcii ON tcii.id = tci.idioma_id AND tcii.ativo = 1
                            LEFT JOIN tb_nivel tbn ON tbn.id = tci.tb_nivel_id AND tbn.ativo = 1
                            WHERE tci.codigo_interno_colaborador IN @Codigos AND tcii.id IS NOT NULL;

                            SELECT  tcaa.id AS Id, 
                                    tca.codigo_interno_colaborador AS CodigoInternoColaborador, 
                                    tcaa.descricao AS DescricaoCargo
                            FROM tb_colaborador_cargo tca
                            LEFT JOIN tb_cargo tcaa ON tcaa.id = tca.cargo_id AND tcaa.ativo = 1
                            WHERE tca.codigo_interno_colaborador IN @Codigos AND tcaa.id IS NOT NULL;

                            SELECT  tmee.id AS Id, 
                                    tme.codigo_interno_colaborador AS CodigoInternoColaborador, 
                                    tmee.descricao AS DescricaoMetodologia, 
                                    tbn.descricao AS Senioridade
                            FROM tb_colaborador_metodologia tme
                            LEFT JOIN tb_metodologia tmee ON tmee.id = tme.metodologia_id AND tmee.ativo = 1
                            LEFT JOIN tb_nivel tbn ON tbn.id = tme.tb_nivel_id AND tbn.ativo = 1
                            WHERE tme.codigo_interno_colaborador IN @Codigos AND tmee.id IS NOT NULL;

                            SELECT  tccc.id AS Id, 
                                    tcc.codigo_interno_colaborador AS CodigoInternoColaborador, 
                                    tccc.descricao AS DescricaoNacionalidade
                            FROM tb_cidadania_colaborador tcc
                            LEFT JOIN tb_cidadania tccc ON tccc.id = tcc.tb_cidadania_id AND tcc.ativo = 1
                            WHERE tcc.codigo_interno_colaborador IN @Codigos AND tccc.id IS NOT NULL;

                            SELECT  tpo.cod_cliente AS Id, 
                                    tcpa.codigo_interno_colaborador AS CodigoInternoColaborador, 
                                    tclo.nome_cliente AS NomeCliente
                            FROM tb_colaborador_periodo_alocacao tcpa
                            JOIN tb_projeto_org tpo ON tpo.cod_projeto = tcpa.codigo_projeto AND tcpa.tb_org_id = tpo.tb_org_id
                            LEFT JOIN tb_cliente_org tclo ON tpo.cod_cliente = tclo.codigo_cliente AND tpo.tb_org_id = tclo.tb_org_id
                            JOIN tb_colaborador_org tco ON tco.codigo_interno_colaborador = tcpa.codigo_interno_colaborador AND tcpa.tb_org_id = tco.tb_org_id
                            WHERE tcpa.codigo_interno_colaborador IN @Codigos
                              AND tclo.nome_cliente IS NOT NULL 
                              AND tclo.nome_cliente <> '' 
                              AND tcpa.ativo = 1 
                              AND tco.ativo = 1 
                              AND tcpa.data_fim >= CURDATE()
                            GROUP BY tcpa.codigo_interno_colaborador, tpo.cod_cliente, tclo.nome_cliente;

                            SELECT tco.codigo_interno_colaborador AS CodigoInternoColaborador,
                                   tcc.codigo_interno_colaborador AS CodigoInternoGestor,
                                   tcc.nome_completo AS NomeGestor
                            FROM   tb_colaborador_org AS tco 
                            INNER JOIN tb_colaborador_hierarquia AS ch
                                ON ch.cod_colaborador_externo = tco.cod_colaborador_externo
                               AND ch.tb_org_id = tco.tb_org_id
                            LEFT JOIN tb_colaborador_org AS tcoo
                                ON tcoo.cod_colaborador_externo = ch.cod_colaborador_superior
                               AND tcoo.tb_org_id = tco.tb_org_id
                            LEFT JOIN tb_colaborador AS tcc
                                ON tcc.codigo_interno_colaborador = tcoo.codigo_interno_colaborador
                            WHERE  tco.codigo_interno_colaborador IN @Codigos; 
                        ";

            // Ajuste do chunk size — evita listas grandes no IN
            int chunkSize = codigos.Count > 1000 ? 500 : codigos.Count;
            var chunks = codigos.Chunk(chunkSize);

            await using var conn = _connectionString.CreateMySqlConnection();
            await conn.OpenAsync();

            foreach (var chunk in chunks)
            {
                await using var multi = await conn.QueryMultipleAsync(multiQuery, new { Codigos = chunk.ToList() });

                allHardSkills.AddRange(await multi.ReadAsync<HardSkillsDTO>());
                allSoftSkills.AddRange(await multi.ReadAsync<SoftSkillsDTO>());
                allFormacoes.AddRange(await multi.ReadAsync<FormacoesDTO>());
                allLinguas.AddRange(await multi.ReadAsync<LinguasDTO>());
                allCargos.AddRange(await multi.ReadAsync<CargosDTO>());
                allMetodologias.AddRange(await multi.ReadAsync<MetodologiasDTO>());
                allNacionalidades.AddRange(await multi.ReadAsync<NacionalidadesDTO>());
                allClientes.AddRange(await multi.ReadAsync<ClientesDTO>());
                allGestores.AddRange(await multi.ReadAsync<GestoresDTO>());
            }

            // Agrupa resultados em dicionários (lookup rápido)
            var dictHard = allHardSkills.GroupBy(x => x.CodigoInternoColaborador).ToDictionary(g => g.Key, g => g.ToList());
            var dictSoft = allSoftSkills.GroupBy(x => x.CodigoInternoColaborador).ToDictionary(g => g.Key, g => g.ToList());
            var dictForm = allFormacoes.GroupBy(x => x.CodigoInternoColaborador).ToDictionary(g => g.Key, g => g.ToList());
            var dictLing = allLinguas.GroupBy(x => x.CodigoInternoColaborador).ToDictionary(g => g.Key, g => g.ToList());
            var dictCarg = allCargos.GroupBy(x => x.CodigoInternoColaborador).ToDictionary(g => g.Key, g => g.ToList());
            var dictMet = allMetodologias.GroupBy(x => x.CodigoInternoColaborador).ToDictionary(g => g.Key, g => g.ToList());
            var dictNac = allNacionalidades.GroupBy(x => x.CodigoInternoColaborador).ToDictionary(g => g.Key, g => g.ToList());
            var dictCli = allClientes.GroupBy(x => x.CodigoInternoColaborador).ToDictionary(g => g.Key, g => g.ToList());
            var dictGest = allGestores.GroupBy(x => x.CodigoInternoColaborador).ToDictionary(g => g.Key, g => g.ToList());

            // Monta objetos finais
            foreach (var col in colaboradores)
            {
                var cod = col.CodigoInternoColaborador;
                col.HardSkills = dictHard.GetValueOrDefault(cod) ?? new List<HardSkillsDTO>();
                col.SoftSkills = dictSoft.GetValueOrDefault(cod) ?? new List<SoftSkillsDTO>();
                col.Formacoes = dictForm.GetValueOrDefault(cod) ?? new List<FormacoesDTO>();
                col.Linguas = dictLing.GetValueOrDefault(cod) ?? new List<LinguasDTO>();
                col.Cargos = dictCarg.GetValueOrDefault(cod) ?? new List<CargosDTO>();
                col.Metodologias = dictMet.GetValueOrDefault(cod) ?? new List<MetodologiasDTO>();
                col.Nacionalidades = dictNac.GetValueOrDefault(cod) ?? new List<NacionalidadesDTO>();
                col.Clientes = dictCli.GetValueOrDefault(cod) ?? new List<ClientesDTO>();
                col.Gestores = dictGest.GetValueOrDefault(cod) ?? new List<GestoresDTO>();
            }
        }

        private async Task LoadAuxiliaryData_OLD(List<MapaDemograficoDTO> colaboradores)
        {
            var codigos = colaboradores.Select(c => c.CodigoInternoColaborador).Distinct().ToList();
            if (!codigos.Any()) return;

            var allHardSkills = new ConcurrentBag<HardSkillsDTO>();
            var allSoftSkills = new ConcurrentBag<SoftSkillsDTO>();
            var allFormacoes = new ConcurrentBag<FormacoesDTO>();
            var allLinguas = new ConcurrentBag<LinguasDTO>();
            var allCargos = new ConcurrentBag<CargosDTO>();
            var allMetodologias = new ConcurrentBag<MetodologiasDTO>();
            var allNacionalidades = new ConcurrentBag<NacionalidadesDTO>();
            var allClientes = new ConcurrentBag<ClientesDTO>();
            var allGestores = new ConcurrentBag<GestoresDTO>();

            // Multi Select's
            // HardSkills / SoftSkills / Formações / Linguas / Cargos / Metodologias / Nacionalidades / Clientes
            const string multiQuery = @"
                SELECT  tcpp.id AS Id,
                        tcp.codigo_interno_colaborador AS CodigoInternoColaborador, 
                        tcpp.descricao AS DescricaoHardSkill, 
                        tbn.descricao AS Senioridade
                FROM tb_colaborador_competencia tcp
                LEFT JOIN tb_competencia tcpp ON tcpp.id = tcp.competencia_id AND tcpp.ativo = 1
                LEFT JOIN tb_nivel tbn ON tbn.id = tcp.tb_nivel_id AND tbn.ativo = 1
                WHERE tcp.codigo_interno_colaborador IN @Codigos AND tcpp.id IS NOT NULL;

                SELECT  tskk.id AS Id, 
                        tsk.codigo_interno_colaborador AS CodigoInternoColaborador, 
                        tskk.descricao AS DescricaoSoftSkill, 
                        tbn.descricao AS Senioridade
                FROM tb_colaborador_softskill tsk
                LEFT JOIN tb_softskill tskk ON tskk.id = tsk.softskill_id AND tskk.ativo = 1
                LEFT JOIN tb_nivel tbn ON tbn.id = tsk.tb_nivel_id AND tbn.ativo = 1
                WHERE tsk.codigo_interno_colaborador IN @Codigos AND tskk.id IS NOT NULL;

                SELECT  tfmm.id AS Id, 
                        tfm.codigo_interno_colaborador AS CodigoInternoColaborador, 
                        tfmm.descricao AS DescricaoFormacao, 
                        tbn.descricao AS Senioridade
                FROM tb_colaborador_formacao tfm
                LEFT JOIN tb_formacao tfmm ON tfmm.id = tfm.formacao_id AND tfmm.ativo = 1
                LEFT JOIN tb_nivel tbn ON tbn.id = tfm.tb_nivel_id AND tbn.ativo = 1
                WHERE tfm.codigo_interno_colaborador IN @Codigos AND tfmm.id IS NOT NULL;

                SELECT  tcii.id AS Id, 
                        tci.codigo_interno_colaborador AS CodigoInternoColaborador, 
                        tcii.descricao AS DescricaoLingua, 
                        tbn.descricao AS Senioridade
                FROM tb_colaborador_idioma tci
                LEFT JOIN tb_idioma tcii ON tcii.id = tci.idioma_id AND tcii.ativo = 1
                LEFT JOIN tb_nivel tbn ON tbn.id = tci.tb_nivel_id AND tbn.ativo = 1
                WHERE tci.codigo_interno_colaborador IN @Codigos AND tcii.id IS NOT NULL;

                SELECT  tcaa.id AS Id, 
                        tca.codigo_interno_colaborador AS CodigoInternoColaborador, 
                        tcaa.descricao AS DescricaoCargo
                FROM tb_colaborador_cargo tca
                LEFT JOIN tb_cargo tcaa ON tcaa.id = tca.cargo_id AND tcaa.ativo = 1
                WHERE tca.codigo_interno_colaborador IN @Codigos AND tcaa.id IS NOT NULL;

                SELECT  tmee.id AS Id, 
                        tme.codigo_interno_colaborador AS CodigoInternoColaborador, 
                        tmee.descricao AS DescricaoMetodologia, 
                        tbn.descricao AS Senioridade
                FROM tb_colaborador_metodologia tme
                LEFT JOIN tb_metodologia tmee ON tmee.id = tme.metodologia_id AND tmee.ativo = 1
                LEFT JOIN tb_nivel tbn ON tbn.id = tme.tb_nivel_id AND tbn.ativo = 1
                WHERE tme.codigo_interno_colaborador IN @Codigos AND tmee.id IS NOT NULL;

                SELECT  tccc.id AS Id, 
                        tcc.codigo_interno_colaborador AS CodigoInternoColaborador, 
                        tccc.descricao AS DescricaoNacionalidade
                FROM tb_cidadania_colaborador tcc
                LEFT JOIN tb_cidadania tccc ON tccc.id = tcc.tb_cidadania_id AND tcc.ativo = 1
                WHERE tcc.codigo_interno_colaborador IN @Codigos AND tccc.id IS NOT NULL;

                SELECT  tpo.cod_cliente AS Id, 
                        tcpa.codigo_interno_colaborador AS CodigoInternoColaborador, 
                        tclo.nome_cliente AS NomeCliente
                FROM tb_colaborador_periodo_alocacao tcpa
                JOIN tb_projeto_org tpo ON tpo.cod_projeto = tcpa.codigo_projeto AND tcpa.tb_org_id = tpo.tb_org_id
                LEFT JOIN tb_cliente_org tclo ON tpo.cod_cliente = tclo.codigo_cliente AND tpo.tb_org_id = tclo.tb_org_id
                JOIN tb_colaborador_org tco ON tco.codigo_interno_colaborador = tcpa.codigo_interno_colaborador AND tcpa.tb_org_id = tco.tb_org_id
                WHERE tcpa.codigo_interno_colaborador IN @Codigos
                  AND tclo.nome_cliente IS NOT NULL 
                  AND tclo.nome_cliente <> '' 
                  AND tcpa.ativo = 1 
                  AND tco.ativo = 1 
                  AND tcpa.data_fim >= CURDATE()
                GROUP BY tcpa.codigo_interno_colaborador, tpo.cod_cliente, tclo.nome_cliente;

                SELECT tco.codigo_interno_colaborador AS CodigoInternoColaborador,
                       tcc.codigo_interno_colaborador AS CodigoInternoGestor,
                       tcc.nome_completo AS NomeGestor
                FROM   tb_colaborador_org AS tco 
                INNER JOIN tb_colaborador_hierarquia AS ch
                    ON ch.cod_colaborador_externo = tco.cod_colaborador_externo
                   AND ch.tb_org_id = tco.tb_org_id
                LEFT JOIN tb_colaborador_org AS tcoo
                    ON tcoo.cod_colaborador_externo = ch.cod_colaborador_superior
                   AND tcoo.tb_org_id = tco.tb_org_id
                LEFT JOIN tb_colaborador AS tcc
                    ON tcc.codigo_interno_colaborador = tcoo.codigo_interno_colaborador
                WHERE  tco.codigo_interno_colaborador IN @Codigos; 
            ";

            int chunkSize = 250;    //quantidade de busca
            var chunks = codigos.Chunk(chunkSize).ToList();
            var semaphore = new SemaphoreSlim(8); // quantidade de conexões concorrentes
            var tasks = chunks.Select(async chunk =>
            {
                await semaphore.WaitAsync();
                try
                {
                    await using var conn = _connectionString.CreateMySqlConnection();
                    await conn.OpenAsync();
                    await using var multi = await conn.QueryMultipleAsync(multiQuery, new { Codigos = chunk.ToList() });

                    foreach (var hs in await multi.ReadAsync<HardSkillsDTO>()) allHardSkills.Add(hs);
                    foreach (var ss in await multi.ReadAsync<SoftSkillsDTO>()) allSoftSkills.Add(ss);
                    foreach (var f in await multi.ReadAsync<FormacoesDTO>()) allFormacoes.Add(f);
                    foreach (var l in await multi.ReadAsync<LinguasDTO>()) allLinguas.Add(l);
                    foreach (var c in await multi.ReadAsync<CargosDTO>()) allCargos.Add(c);
                    foreach (var m in await multi.ReadAsync<MetodologiasDTO>()) allMetodologias.Add(m);
                    foreach (var n in await multi.ReadAsync<NacionalidadesDTO>()) allNacionalidades.Add(n);
                    foreach (var cli in await multi.ReadAsync<ClientesDTO>()) allClientes.Add(cli);
                    foreach (var g in await multi.ReadAsync<GestoresDTO>()) allGestores.Add(g);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);

            // Distribui os dados para cada colaborador
            var dictHard = allHardSkills.GroupBy(x => x.CodigoInternoColaborador).ToDictionary(g => g.Key, g => g.ToList());
            var dictSoft = allSoftSkills.GroupBy(x => x.CodigoInternoColaborador).ToDictionary(g => g.Key, g => g.ToList());
            var dictForm = allFormacoes.GroupBy(x => x.CodigoInternoColaborador).ToDictionary(g => g.Key, g => g.ToList());
            var dictLing = allLinguas.GroupBy(x => x.CodigoInternoColaborador).ToDictionary(g => g.Key, g => g.ToList());
            var dictCarg = allCargos.GroupBy(x => x.CodigoInternoColaborador).ToDictionary(g => g.Key, g => g.ToList());
            var dictMet = allMetodologias.GroupBy(x => x.CodigoInternoColaborador).ToDictionary(g => g.Key, g => g.ToList());
            var dictNac = allNacionalidades.GroupBy(x => x.CodigoInternoColaborador).ToDictionary(g => g.Key, g => g.ToList());
            var dictCli = allClientes.GroupBy(x => x.CodigoInternoColaborador).ToDictionary(g => g.Key, g => g.ToList());
            var dictGest = allGestores.GroupBy(x => x.CodigoInternoColaborador).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var col in colaboradores)
            {
                var cod = col.CodigoInternoColaborador;
                col.HardSkills = dictHard.GetValueOrDefault(cod) ?? new List<HardSkillsDTO>();
                col.SoftSkills = dictSoft.GetValueOrDefault(cod) ?? new List<SoftSkillsDTO>();
                col.Formacoes = dictForm.GetValueOrDefault(cod) ?? new List<FormacoesDTO>();
                col.Linguas = dictLing.GetValueOrDefault(cod) ?? new List<LinguasDTO>();
                col.Cargos = dictCarg.GetValueOrDefault(cod) ?? new List<CargosDTO>();
                col.Metodologias = dictMet.GetValueOrDefault(cod) ?? new List<MetodologiasDTO>();
                col.Nacionalidades = dictNac.GetValueOrDefault(cod) ?? new List<NacionalidadesDTO>();
                col.Clientes = dictCli.GetValueOrDefault(cod) ?? new List<ClientesDTO>();
                col.Gestores = dictGest.GetValueOrDefault(cod) ?? new List<GestoresDTO>();
            }
        }

        public async Task<List<MapaDemograficoPcdSumarioDTO>> ListarSumarioPcdsAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT 
                    COALESCE(tcs.pcd, 'Sem resposta') AS descricao,
                    COUNT(*) AS quantidade,
                    CASE 
                        WHEN ((COUNT(*) * 100.0) / SUM(COUNT(*)) OVER ()) < 0.01 
                            THEN CONCAT(FORMAT((COUNT(*) * 100.0) / SUM(COUNT(*)) OVER (), 3), '%')
                        ELSE CONCAT(FORMAT((COUNT(*) * 100.0) / SUM(COUNT(*)) OVER (), 2), '%')
                    END AS porcentagem
                FROM tb_colaborador tc
                INNER JOIN tb_colaborador_org tco 
                    ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador
                LEFT JOIN tb_colaborador_saude tcs 
                    ON tcs.id = tc.colaborador_saude_id
                WHERE 
                    tco.tb_org_id = @OrgId
                    AND tco.ativo = 1
                GROUP BY 
                    COALESCE(tcs.pcd, 'Sem resposta')
                ORDER BY 
                    CASE 
                        WHEN COALESCE(tcs.pcd, 'Sem resposta') = 'Sem resposta' THEN 0
                        ELSE 1
                    END,
                    ((COUNT(*) * 100.0) / SUM(COUNT(*)) OVER ()) DESC;
            ";

            var parametros = new
            {
                OrgId = orgId
            };

            var resultado = await connection.QueryAsync<MapaDemograficoPcdSumarioDTO>(query, parametros);
            return resultado.ToList();
        }
    }
}

