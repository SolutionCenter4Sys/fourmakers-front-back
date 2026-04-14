using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Context;
using Core.Domain.Colaborador;
using Dapper;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Escolaridade;
using DataTransferObject.Domain.Experiencia;
using DataTransferObject.Domain.Formacao;
using DataTransferObject.Domain.Idioma;
using DataTransferObject.Domain.Nivel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Colaborador
{
    public class ColaboradorPerfilRepository : IColaboradorPerfilRepository
    {

        private readonly IDBConnection _dapperConnection;
        private readonly IHistoricoCVRepository _historicoCVRepository;
        public ColaboradorPerfilRepository(IDBConnection dapperConnection, IHistoricoCVRepository historicoCVRepository)
        {
            _dapperConnection = dapperConnection;
            _historicoCVRepository = historicoCVRepository;
        }

        /// <summary>
        /// Atualiza a seção "Sobre" do colaborador
        /// </summary>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <param name="sobre">Texto da seção sobre</param>
        /// <returns>True se atualizado com sucesso</returns>
        public async Task<bool> AtualizarSobreColaboradorAsync(string codigoInternoColaborador, string sobre)
        {
            var connection = _dapperConnection.GetConnection();
            
            try
            {
                // Verificar se já existe um registro
                var queryVerificar = @"
                    SELECT COUNT(*) 
                    FROM tb_colaborador_sobre 
                    WHERE codigo_interno_colaborador = @CodigoInternoColaborador";
                
                var existe = await connection.ExecuteScalarAsync<int>(queryVerificar, new { CodigoInternoColaborador = codigoInternoColaborador }) > 0;
                
                if (!existe)
                {
                    // Inserir novo registro
                    var queryInserir = @"
                        INSERT INTO tb_colaborador_sobre (
                            codigo_interno_colaborador, 
                            descricao, 
                            data_criacao
                        ) VALUES (
                            @CodigoInternoColaborador, 
                            @Descricao, 
                            @DataCriacao
                        )";
                    
                    await connection.ExecuteAsync(queryInserir, new 
                    { 
                        CodigoInternoColaborador = codigoInternoColaborador, 
                        Descricao = sobre,
                        DataCriacao = DateTime.Now
                    });
                }
                else
                {
                    // Atualizar registro existente
                    var queryAtualizar = @"
                        UPDATE tb_colaborador_sobre 
                        SET descricao = @Descricao, 
                            data_criacao = @DataCriacao
                        WHERE codigo_interno_colaborador = @CodigoInternoColaborador";
                    
                    await connection.ExecuteAsync(queryAtualizar, new 
                    { 
                        CodigoInternoColaborador = codigoInternoColaborador, 
                        Descricao = sobre,
                        DataCriacao = DateTime.Now
                    });
                }
                
                // Registrar no histórico de CV
                _historicoCVRepository.InserirHistoricoCV(
                    codigoInternoColaborador, 
                    OrigemAlteracaoCVEnum.MANUAL, 
                    TipoItemCVEnum.UPDATE, 
                    null, 
                    null, 
                    ItemCVEnum.SOBRE
                );
                
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        

        #region Métodos de Escolaridade

        /// <summary>
        /// Lista as escolaridades do colaborador
        /// </summary>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <returns>Lista de escolaridades</returns>
        public async Task<List<EscolaridadeDTO>> ListarEscolaridadeColaboradorAsync(string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();
            
            try
            {
                var query = @"
                    SELECT 
                        te.id AS Id,
                        te.instituicao AS Instituicao,
                        te.data_inicio AS DataInicio,
                        te.data_termino AS DataTermino,
                        te.descricao AS Descricao,
                        te.codigo_interno_colaborador AS ColaboradorCpf,
                        te.tipo_diploma_id AS TipoDiplomaId,
                        te.path_diploma AS FilePath,
                        te.tb_formacao_id AS FormacaoId,
                        tf.descricao AS FormacaoDescricao,
                        te.ativo AS Ativo
                    FROM tb_escolaridade te
                    LEFT JOIN tb_formacao tf ON te.tb_formacao_id = tf.id
                    WHERE te.codigo_interno_colaborador = @CodigoInternoColaborador
                    AND te.ativo = 1
                    ORDER BY te.data_inicio DESC";
                
                var escolaridades = await connection.QueryAsync<EscolaridadeDTO>(query, new { CodigoInternoColaborador = codigoInternoColaborador });
                
                return escolaridades.Select(e => 
                {
                    e.Ativo = true;
                    return e;
                }).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Busca múltiplas formações por descrições
        /// </summary>
        /// <param name="descricoes">Lista de descrições</param>
        /// <returns>Dicionário com descrição (normalizada) e FormacaoDTO</returns>
        public async Task<Dictionary<string, FormacaoDTO>> BuscarFormacoesEmLoteAsync(List<string> descricoes)
        {
            var connection = _dapperConnection.GetConnection();
            
            try
            {
                if (descricoes == null || !descricoes.Any())
                {
                    return new Dictionary<string, FormacaoDTO>();
                }

                var query = @"
                    SELECT 
                        id AS Id,
                        descricao AS Descricao
                    FROM tb_formacao
                    WHERE UPPER(descricao) IN @Descricoes
                    AND ativo = 1";
                
                var descricoesUpper = descricoes.Select(d => d.ToUpper()).Distinct().ToList();
                var formacoes = await connection.QueryAsync<FormacaoDTO>(query, new { Descricoes = descricoesUpper });
                
                // Criar dicionário com chave normalizada (uppercase)
                return formacoes.ToDictionary(
                    f => f.Descricao.ToUpper(), 
                    f => f,
                    StringComparer.OrdinalIgnoreCase
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Cria múltiplas formações de uma vez
        /// </summary>
        /// <param name="descricoes">Lista de descrições das formações</param>
        /// <param name="cpfUsuarioCriacao">CPF do usuário que está criando</param>
        /// <returns>Dicionário com descrição (normalizada) e FormacaoDTO criada</returns>
        public async Task<Dictionary<string, FormacaoDTO>> CriarFormacoesEmLoteAsync(List<string> descricoes, string cpfUsuarioCriacao)
        {
            var connection = _dapperConnection.GetConnection();
            
            try
            {
                if (descricoes == null || !descricoes.Any())
                {
                    return new Dictionary<string, FormacaoDTO>();
                }

                var resultado = new Dictionary<string, FormacaoDTO>(StringComparer.OrdinalIgnoreCase);
                
                foreach (var descricao in descricoes.Distinct())
                {
                    // Verificar se já existe
                    var queryVerificar = @"
                        SELECT 
                            id AS Id,
                            descricao AS Descricao
                        FROM tb_formacao
                        WHERE UPPER(descricao) = UPPER(@Descricao)
                        LIMIT 1";
                    
                    var formacaoExistente = await connection.QueryFirstOrDefaultAsync<FormacaoDTO>(queryVerificar, new { Descricao = descricao });
                    
                    if (formacaoExistente != null)
                    {
                        // Reativar se estiver inativa
                        await connection.ExecuteAsync(@"
                            UPDATE tb_formacao 
                            SET ativo = 1 
                            WHERE id = @Id", new { Id = formacaoExistente.Id });
                        
                        resultado[descricao.ToUpper()] = formacaoExistente;
                    }
                    else
                    {
                        // Criar nova formação
                        var queryInserir = @"
                            INSERT INTO tb_formacao (
                                ativo,
                                descricao,
                                usuario_criacao_id
                            ) VALUES (
                                1,
                                @Descricao,
                                @UsuarioId
                            );
                            SELECT LAST_INSERT_ID();";
                        
                        var novoId = await connection.ExecuteScalarAsync<long>(queryInserir, new 
                        { 
                            Descricao = descricao,
                            UsuarioId = 146
                        });
                        
                        resultado[descricao.ToUpper()] = new FormacaoDTO 
                        { 
                            Id = novoId, 
                            Descricao = descricao 
                        };
                    }
                }
                
                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Adiciona múltiplas escolaridades ao colaborador de uma vez
        /// </summary>
        /// <param name="escolaridades">Lista de escolaridades a adicionar</param>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <param name="origem">Origem da alteração</param>
        /// <returns>Número de escolaridades adicionadas</returns>
        public async Task<int> AdicionarEscolaridadesEmLoteAsync(List<EscolaridadeDTO> escolaridades, string codigoInternoColaborador, OrigemAlteracaoCVEnum origem)
        {
            var connection = _dapperConnection.GetConnection();
            
            try
            {
                if (escolaridades == null || !escolaridades.Any())
                {
                    return 0;
                }

                var valores = new List<string>();
                var parametros = new Dictionary<string, object>();
                
                for (int i = 0; i < escolaridades.Count; i++)
                {
                    var escolaridade = escolaridades[i];
                    valores.Add($"(@Instituicao{i}, @DataInicio{i}, @DataTermino{i}, @Descricao{i}, 1, @DataCriacao{i}, @DataAlteracao{i}, @CodigoInternoColaborador{i}, @FormacaoId{i})");
                    
                    parametros[$"Instituicao{i}"] = escolaridade.Instituicao;
                    parametros[$"DataInicio{i}"] = escolaridade.DataInicio;
                    parametros[$"DataTermino{i}"] = escolaridade.DataTermino;
                    parametros[$"Descricao{i}"] = escolaridade.Descricao;
                    parametros[$"DataCriacao{i}"] = DateTime.Now;
                    parametros[$"DataAlteracao{i}"] = DateTime.Now;
                    parametros[$"CodigoInternoColaborador{i}"] = codigoInternoColaborador;
                    parametros[$"FormacaoId{i}"] = escolaridade.FormacaoId;
                }
                
                var queryInserir = $@"
                    INSERT INTO tb_escolaridade (
                        instituicao,
                        data_inicio,
                        data_termino,
                        descricao,
                        ativo,
                        data_criacao,
                        data_alteracao,
                        codigo_interno_colaborador,
                        tb_formacao_id
                    ) VALUES 
                    {string.Join(",", valores)}";
                
                var linhasInseridas = await connection.ExecuteAsync(queryInserir, parametros);
                
                // Registrar no histórico de CV (uma vez para todas)
                if (linhasInseridas > 0)
                {
                    _historicoCVRepository.InserirHistoricoCV(
                        codigoInternoColaborador, 
                        origem, 
                        TipoItemCVEnum.INSERT, 
                        null, 
                        null, 
                        ItemCVEnum.ESCOLARIDADE
                    );
                }
                
                return linhasInseridas;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Métodos de Experiência Profissional

        /// <summary>
        /// Lista as experiências profissionais do colaborador
        /// </summary>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <returns>Lista de experiências profissionais</returns>
        public async Task<List<ListaExperienciaDTO>> ListarExperienciasColaboradorAsync(string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();
            
            try
            {
                var query = @"
                    SELECT 
                        te.id AS Id,
                        te.titulo AS Funcao,
                        te.empresa AS Empresa,
                        te.descricao AS Atividades,
                        te.data_inicio AS DataInicio,
                        te.data_saida AS DataSaida,
                        te.atual AS Atual,
                        te.codigo_interno_colaborador AS ColaboradorCpf
                    FROM tb_experiencia te
                    WHERE te.codigo_interno_colaborador = @CodigoInternoColaborador
                    AND te.ativo = 1
                    ORDER BY te.data_inicio DESC";
                
                var experiencias = await connection.QueryAsync<ListaExperienciaDTO>(query, new { CodigoInternoColaborador = codigoInternoColaborador });
                
                return experiencias.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Adiciona múltiplas experiências profissionais ao colaborador de uma vez
        /// </summary>
        /// <param name="experiencias">Lista de experiências a adicionar</param>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <param name="origem">Origem da alteração</param>
        /// <returns>Número de experiências adicionadas</returns>
        public async Task<int> AdicionarExperienciasEmLoteAsync(List<ExperienciaDTO> experiencias, string codigoInternoColaborador, OrigemAlteracaoCVEnum origem)
        {
            var connection = _dapperConnection.GetConnection();
            
            try
            {
                if (experiencias == null || !experiencias.Any())
                {
                    return 0;
                }

                var valores = new List<string>();
                var parametros = new Dictionary<string, object>();
                
                for (int i = 0; i < experiencias.Count; i++)
                {
                    var experiencia = experiencias[i];
                    valores.Add($"(@Titulo{i}, @Empresa{i}, @Descricao{i}, @DataInicio{i}, @DataSaida{i}, @Atual{i}, 1, @DataCriacao{i}, @DataAlteracao{i}, @CodigoInternoColaborador{i})");
                    
                    parametros[$"Titulo{i}"] = experiencia.Funcao;
                    parametros[$"Empresa{i}"] = experiencia.Empresa;
                    parametros[$"Descricao{i}"] = experiencia.Atividades;
                    parametros[$"DataInicio{i}"] = experiencia.DataInicio;
                    parametros[$"DataSaida{i}"] = experiencia.DataSaida;
                    parametros[$"Atual{i}"] = experiencia.DataSaida == null ? 1 : 0;
                    parametros[$"DataCriacao{i}"] = DateTime.Now;
                    parametros[$"DataAlteracao{i}"] = DateTime.Now;
                    parametros[$"CodigoInternoColaborador{i}"] = codigoInternoColaborador;
                }
                
                var queryInserir = $@"
                    INSERT INTO tb_experiencia (
                        titulo,
                        empresa,
                        descricao,
                        data_inicio,
                        data_saida,
                        atual,
                        ativo,
                        data_criacao,
                        data_alteracao,
                        codigo_interno_colaborador
                    ) VALUES 
                    {string.Join(",", valores)}";
                
                var linhasInseridas = await connection.ExecuteAsync(queryInserir, parametros);
                
                // Registrar no histórico de CV (uma vez para todas)
                if (linhasInseridas > 0)
                {
                    _historicoCVRepository.InserirHistoricoCV(
                        codigoInternoColaborador, 
                        origem, 
                        TipoItemCVEnum.INSERT, 
                        null, 
                        null, 
                        ItemCVEnum.EXPERIENCIA
                    );
                }
                
                return linhasInseridas;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Métodos de Idioma

        /// <summary>
        /// Lista os idiomas do colaborador
        /// </summary>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <returns>Lista de idiomas do colaborador</returns>
        public async Task<List<IdiomaColaboradorDTO>> ListarIdiomasColaboradorAsync(string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();
            
            try
            {
                var query = @"
                    SELECT
                        ci.id AS Id,
                        ci.data_criacao AS Data,
                        ci.codigo_interno_colaborador AS ColaboradorCpf,
                        ci.idioma_id AS IdIdioma,
                        ci.tb_nivel_id AS IdNivel,
                        i.id AS Id,
                        i.descricao AS Descricao,
                        i.confirmada AS Pendente,
                        n.id AS Id,
                        n.descricao AS Descricao
                    FROM tb_colaborador_idioma ci
                    INNER JOIN tb_idioma i ON ci.idioma_id = i.id
                    LEFT JOIN tb_nivel n ON ci.tb_nivel_id = n.id
                    WHERE ci.codigo_interno_colaborador = @CodigoInternoColaborador
                    AND ci.ativo = 1
                    AND i.ativo = 1
                    ORDER BY i.descricao";
                
                var resultado = await connection.QueryAsync<IdiomaColaboradorDTO, IdiomaDTO, NivelDTO, IdiomaColaboradorDTO>(
                    query,
                    (dto, idioma, nivel) =>
                    {
                        dto.Idioma = idioma;
                        dto.Nivel = nivel ?? new NivelDTO { Id = 32, Descricao = "A definir" };
                        return dto;
                    },
                    new { CodigoInternoColaborador = codigoInternoColaborador },
                    splitOn: "Id"
                );
                
                return resultado.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Busca múltiplos idiomas por descrições
        /// </summary>
        /// <param name="descricoes">Lista de descrições de idiomas</param>
        /// <returns>Dicionário com descrição (normalizada) e IdiomaDTO</returns>
        public async Task<Dictionary<string, IdiomaDTO>> BuscarIdiomasEmLoteAsync(List<string> descricoes)
        {
            var connection = _dapperConnection.GetConnection();
            
            try
            {
                if (descricoes == null || !descricoes.Any())
                {
                    return new Dictionary<string, IdiomaDTO>();
                }

                var query = @"
                    SELECT 
                        id AS Id,
                        descricao AS Descricao
                    FROM tb_idioma
                    WHERE UPPER(descricao) IN @Descricoes
                    AND ativo = 1";
                
                var descricoesUpper = descricoes.Select(d => d.ToUpper()).Distinct().ToList();
                var idiomas = await connection.QueryAsync<IdiomaDTO>(query, new { Descricoes = descricoesUpper });
                
                // Criar dicionário com chave normalizada (uppercase)
                return idiomas.ToDictionary(
                    i => i.Descricao.ToUpper(), 
                    i => i,
                    StringComparer.OrdinalIgnoreCase
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Cria múltiplos idiomas de uma vez
        /// </summary>
        /// <param name="descricoes">Lista de descrições dos idiomas</param>
        /// <returns>Dicionário com descrição (normalizada) e IdiomaDTO criado</returns>
        public async Task<Dictionary<string, IdiomaDTO>> CriarIdiomasEmLoteAsync(List<string> descricoes)
        {
            var connection = _dapperConnection.GetConnection();
            
            try
            {
                if (descricoes == null || !descricoes.Any())
                {
                    return new Dictionary<string, IdiomaDTO>();
                }

                var resultado = new Dictionary<string, IdiomaDTO>(StringComparer.OrdinalIgnoreCase);
                
                foreach (var descricao in descricoes.Distinct())
                {
                    // Verificar se já existe
                    var queryVerificar = @"
                        SELECT 
                            id AS Id,
                            descricao AS Descricao
                        FROM tb_idioma
                        WHERE UPPER(descricao) = UPPER(@Descricao)
                        LIMIT 1";
                    
                    var idiomaExistente = await connection.QueryFirstOrDefaultAsync<IdiomaDTO>(queryVerificar, new { Descricao = descricao });
                    
                    if (idiomaExistente != null)
                    {
                        // Reativar se estiver inativo
                        await connection.ExecuteAsync(@"
                            UPDATE tb_idioma 
                            SET ativo = 1 
                            WHERE id = @Id", new { Id = idiomaExistente.Id });
                        
                        resultado[descricao.ToUpper()] = idiomaExistente;
                    }
                    else
                    {
                        // Criar novo idioma
                        var queryInserir = @"
                            INSERT INTO tb_idioma (
                                descricao,
                                ativo,
                                data_criacao,
                                confirmada
                            ) VALUES (
                                @Descricao,
                                1,
                                @DataCriacao,
                                0
                            );
                            SELECT LAST_INSERT_ID();";
                        
                        var novoId = await connection.ExecuteScalarAsync<int>(queryInserir, new 
                        { 
                            Descricao = descricao,
                            DataCriacao = DateTime.Now
                        });
                        
                        resultado[descricao.ToUpper()] = new IdiomaDTO 
                        { 
                            Id = novoId, 
                            Descricao = descricao 
                        };
                    }
                }
                
                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Adiciona múltiplos idiomas ao colaborador de uma vez
        /// </summary>
        /// <param name="idiomas">Lista de idiomas a adicionar (ID e Nível)</param>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <param name="origem">Origem da alteração</param>
        /// <returns>Número de idiomas adicionados</returns>
        public async Task<int> AdicionarIdiomasColaboradorEmLoteAsync(List<(int idiomaId, long? nivelId)> idiomas, string codigoInternoColaborador, OrigemAlteracaoCVEnum origem)
        {
            var connection = _dapperConnection.GetConnection();
            
            try
            {
                if (idiomas == null || !idiomas.Any())
                {
                    return 0;
                }

                var valores = new List<string>();
                var parametros = new Dictionary<string, object>();
                
                for (int i = 0; i < idiomas.Count; i++)
                {
                    var (idiomaId, nivelId) = idiomas[i];
                    valores.Add($"(1, @DataCriacao{i}, @DataAlteracao{i}, @CodigoInternoColaborador{i}, @IdiomaId{i}, @NivelId{i})");
                    
                    parametros[$"DataCriacao{i}"] = DateTime.Now;
                    parametros[$"DataAlteracao{i}"] = DateTime.Now;
                    parametros[$"CodigoInternoColaborador{i}"] = codigoInternoColaborador;
                    parametros[$"IdiomaId{i}"] = idiomaId;
                    parametros[$"NivelId{i}"] = nivelId;
                }
                
                var queryInserir = $@"
                    INSERT INTO tb_colaborador_idioma (
                        ativo,
                        data_criacao,
                        data_alteracao,
                        codigo_interno_colaborador,
                        idioma_id,
                        tb_nivel_id
                    ) VALUES 
                    {string.Join(",", valores)}";
                
                var linhasInseridas = await connection.ExecuteAsync(queryInserir, parametros);
                
                // Registrar no histórico de CV (uma vez para todas)
                if (linhasInseridas > 0)
                {
                    _historicoCVRepository.InserirHistoricoCV(
                        codigoInternoColaborador, 
                        origem, 
                        TipoItemCVEnum.INSERT, 
                        null, 
                        null, 
                        ItemCVEnum.IDIOMA
                    );
                }
                
                return linhasInseridas;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Métodos Genéricos de Skills

        /// <summary>
        /// Obtém as configurações de tabela e campos para cada tipo de skill
        /// </summary>
        private ConfigSkillDTO ObterConfigSkill(TipoSkillEnum tipoSkill)
        {
            return tipoSkill switch
            {
                TipoSkillEnum.HARDSKILL => new ConfigSkillDTO 
                { 
                    TabelaSkill = "tb_competencia", 
                    TabelaColaborador = "tb_colaborador_competencia", 
                    CampoId = "competencia_id", 
                    ItemCV = ItemCVEnum.HARDSKILL 
                },
                TipoSkillEnum.SOFTSKILL => new ConfigSkillDTO 
                { 
                    TabelaSkill = "tb_softskill", 
                    TabelaColaborador = "tb_colaborador_softskill", 
                    CampoId = "softskill_id", 
                    ItemCV = ItemCVEnum.SOFTSKILL 
                },
                TipoSkillEnum.METODOLOGIA => new ConfigSkillDTO 
                { 
                    TabelaSkill = "tb_metodologia", 
                    TabelaColaborador = "tb_colaborador_metodologia", 
                    CampoId = "metodologia_id", 
                    ItemCV = ItemCVEnum.METODOLOGIA 
                },
                TipoSkillEnum.DOMINIO => new ConfigSkillDTO 
                { 
                    TabelaSkill = "tb_dominionegocio", 
                    TabelaColaborador = "tb_colaborador_dominionegocio", 
                    CampoId = "dominionegocio_id", 
                    ItemCV = ItemCVEnum.DOMINIO_NEGOCIO 
                },
                TipoSkillEnum.SKILL_DESCONHECIDA => new ConfigSkillDTO 
                { 
                    TabelaSkill = "tb_skill_desconhecida", 
                    TabelaColaborador = "tb_colaborador_skill_desconhecida", 
                    CampoId = "skill_desconhecida_id", 
                    ItemCV = null 
                },
                _ => throw new ArgumentException($"Tipo de skill não suportado: {tipoSkill}")
            };
        }

        /// <summary>
        /// Lista as skills do colaborador de forma genérica
        /// </summary>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <param name="tipoSkill">Tipo de skill</param>
        /// <returns>Lista de skills do colaborador</returns>
        public async Task<List<SkillGenericaDTO>> ListarSkillsColaboradorAsync(string codigoInternoColaborador, TipoSkillEnum tipoSkill)
        {
            var connection = _dapperConnection.GetConnection();
            
            try
            {
                var config = ObterConfigSkill(tipoSkill);
                
                var query = $@"
                    SELECT 
                        s.id AS Id,
                        s.descricao AS Descricao
                    FROM {config.TabelaColaborador} cs
                    INNER JOIN {config.TabelaSkill} s ON cs.{config.CampoId} = s.id
                    WHERE cs.codigo_interno_colaborador = @CodigoInternoColaborador
                    AND cs.ativo = 1
                    AND s.ativo = 1
                    ORDER BY s.descricao";
                
                var skills = await connection.QueryAsync<SkillGenericaDTO>(query, new { CodigoInternoColaborador = codigoInternoColaborador });
                
                return skills.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Busca múltiplas skills por descrições de forma genérica
        /// </summary>
        /// <param name="descricoes">Lista de descrições</param>
        /// <param name="tipoSkill">Tipo de skill</param>
        /// <returns>Dicionário com descrição (normalizada) e SkillGenericaDTO</returns>
        public async Task<Dictionary<string, SkillGenericaDTO>> BuscarSkillsEmLoteAsync(List<string> descricoes, TipoSkillEnum tipoSkill)
        {
            var connection = _dapperConnection.GetConnection();
            
            try
            {
                if (descricoes == null || !descricoes.Any())
                {
                    return new Dictionary<string, SkillGenericaDTO>();
                }

                var config = ObterConfigSkill(tipoSkill);
                
                var query = $@"
                    SELECT
                        id AS Id,
                        descricao AS Descricao
                    FROM {config.TabelaSkill}
                    WHERE UPPER(descricao) IN @Descricoes
                    AND ativo = 1";
                
                var descricoesUpper = descricoes.Select(d => d.ToUpper()).Distinct().ToList();
                var skills = await connection.QueryAsync<SkillGenericaDTO>(query, new { Descricoes = descricoesUpper });
                
                // Criar dicionário com chave normalizada (uppercase)
                return skills.ToDictionary(
                    s => s.Descricao.ToUpper(), 
                    s => s,
                    StringComparer.OrdinalIgnoreCase
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Cria múltiplas skills de uma vez de forma genérica
        /// </summary>
        /// <param name="descricoes">Lista de descrições das skills</param>
        /// <param name="tipoSkill">Tipo de skill</param>
        /// <param name="cpfUsuarioCriacao">CPF do usuário que está criando</param>
        /// <returns>Dicionário com descrição (normalizada) e SkillGenericaDTO criada</returns>
        public async Task<Dictionary<string, SkillGenericaDTO>> CriarSkillsEmLoteAsync(List<string> descricoes, TipoSkillEnum tipoSkill, string cpfUsuarioCriacao)
        {
            var connection = _dapperConnection.GetConnection();
            
            try
            {
                if (descricoes == null || !descricoes.Any())
                {
                    return new Dictionary<string, SkillGenericaDTO>();
                }

                var config = ObterConfigSkill(tipoSkill);
                var resultado = new Dictionary<string, SkillGenericaDTO>(StringComparer.OrdinalIgnoreCase);
                
                foreach (var descricao in descricoes.Distinct())
                {
                    // Verificar se já existe
                    var queryVerificar = $@"
                        SELECT 
                            id AS Id,
                            descricao AS Descricao
                        FROM {config.TabelaSkill}
                        WHERE UPPER(descricao) = UPPER(@Descricao)
                        LIMIT 1";
                    
                    var skillExistente = await connection.QueryFirstOrDefaultAsync<SkillGenericaDTO>(queryVerificar, new { Descricao = descricao });
                    
                    if (skillExistente != null)
                    {
                        // Reativar se estiver inativa
                        var queryReativar = $@"
                            UPDATE {config.TabelaSkill} 
                            SET ativo = 1 
                            WHERE id = @Id";
                        
                        await connection.ExecuteAsync(queryReativar, new { Id = skillExistente.Id });
                        resultado[descricao.ToUpper()] = skillExistente;
                    }
                    else
                    {
                        // Criar nova skill
                        var queryInserir = $@"
                            INSERT INTO {config.TabelaSkill} (
                                descricao,
                                ativo,
                                data_criacao,
                                usuario_criacao_id
                            ) VALUES (
                                @Descricao,
                                1,
                                @DataCriacao,
                                @UsuarioId
                            );
                            SELECT LAST_INSERT_ID();";
                        
                        var novoId = await connection.ExecuteScalarAsync<long>(queryInserir, new 
                        { 
                            Descricao = descricao,
                            DataCriacao = DateTime.Now,
                            UsuarioId = 146
                        });
                        
                        resultado[descricao.ToUpper()] = new SkillGenericaDTO 
                        { 
                            Id = novoId, 
                            Descricao = descricao 
                        };
                    }
                }
                
                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Adiciona múltiplas skills ao colaborador de uma vez de forma genérica
        /// </summary>
        /// <param name="skills">Lista de skills a adicionar (ID e Nível)</param>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <param name="tipoSkill">Tipo de skill</param>
        /// <param name="origem">Origem da alteração</param>
        /// <returns>Número de skills adicionadas</returns>
        public async Task<int> AdicionarSkillsColaboradorEmLoteAsync(List<(long skillId, long? nivelId)> skills, string codigoInternoColaborador, TipoSkillEnum tipoSkill, OrigemAlteracaoCVEnum origem)
        {
            var connection = _dapperConnection.GetConnection();
            
            try
            {
                if (skills == null || !skills.Any())
                {
                    return 0;
                }

                var config = ObterConfigSkill(tipoSkill);
                
                var valores = new List<string>();
                var parametros = new Dictionary<string, object>();
                
                for (int i = 0; i < skills.Count; i++)
                {
                    var (skillId, nivelId) = skills[i];
                    valores.Add($"(1, @DataCriacao{i}, @DataAlteracao{i}, @CodigoInternoColaborador{i}, @SkillId{i}, @NivelId{i})");
                    
                    parametros[$"DataCriacao{i}"] = DateTime.Now;
                    parametros[$"DataAlteracao{i}"] = DateTime.Now;
                    parametros[$"CodigoInternoColaborador{i}"] = codigoInternoColaborador;
                    parametros[$"SkillId{i}"] = skillId;
                    parametros[$"NivelId{i}"] = nivelId;
                }
                
                var queryInserir = $@"
                    INSERT INTO {config.TabelaColaborador} (
                        ativo,
                        data_criacao,
                        data_alteracao,
                        codigo_interno_colaborador,
                        {config.CampoId},
                        tb_nivel_id
                    ) VALUES 
                    {string.Join(",", valores)}";
                
                var linhasInseridas = await connection.ExecuteAsync(queryInserir, parametros);
                
                // Registrar no histórico de CV (uma vez para todas) - apenas se itemCV não for null
                if (linhasInseridas > 0 && config.ItemCV.HasValue)
                {
                    _historicoCVRepository.InserirHistoricoCV(
                        codigoInternoColaborador, 
                        origem, 
                        TipoItemCVEnum.INSERT, 
                        null, 
                        null, 
                        config.ItemCV.Value
                    );
                }
                
                return linhasInseridas;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}

