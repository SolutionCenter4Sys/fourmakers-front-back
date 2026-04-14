using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Infra.Context;
using Colaboracao.Infra.Repositories.MapaAlocacao;
using Core.Domain.Projeto;
using Core.DomainModel.Projeto;
using Dapper;
using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Projeto;
using DataTransferObject.Domain.Projeto.ProjetoOrg;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Helper.Extension;

namespace Colaboracao.Infra.Repositories.Projeto
{
    public class ProjetoOrgRepository : IProjetoOrgRepository
    {
        private ColaboradorContext _colaboradorContext;
        private IClienteOrgRepository _clienteRepository;
        private IAtividadeProjetoRepository _atividadeRepository;
        private IConnectionStringCore _connectionString;
        private IDBConnection _dapperConnection;

        private const sbyte ATIVO = 1;

        public ProjetoOrgRepository(ColaboradorContext colaboradorContext,
                                    IClienteOrgRepository clienteOrgRepository,
                                    IAtividadeProjetoRepository atividadeRepository,
                                    IConnectionStringCore connectionString, IDBConnection dapperConnection)
        {
            _colaboradorContext = colaboradorContext;
            _clienteRepository = clienteOrgRepository;
            _atividadeRepository = atividadeRepository;
            _connectionString = connectionString;
            _dapperConnection = dapperConnection;
        }
        public List<String> GetProjetosColaboradorOrg(string codColaboradorExterno, int orgId)
        {
            return _colaboradorContext.tb_colaborador_projeto_org.Where(x => x.cod_colaborador == codColaboradorExterno && x.tb_org_id == orgId).ToList()
                .Select(x => x.nome_projeto).OrderBy(x => x).ToList();
        }

        public async Task<List<ProjetosColaboradorDTO>> GetProjetosCodColaboradorOrg(string codColaboradorExterno, int orgId, bool associacaoAutomaticaColaboradorAoProjeto, bool tbd, string codigoGerenteProjeto, List<string> listaCodigoCliente, string status, FiltroProjetosPrioritariosEnum prioritarioFiltroEnum)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    List<ProjetosColaboradorDTO> projetosLista = new List<ProjetosColaboradorDTO>();

                    string filtrosEmComum = $@"  tpo.tb_org_id = @OrgId
                                                AND (tpo.data_fim >= CURDATE() OR tpo.data_fim IS NULL OR tpo.data_fim = '0001-01-01')
                                                {(string.IsNullOrEmpty(codigoGerenteProjeto) ? "" : "AND (@CodigoGerenteProjeto IS NULL OR @CodigoGerenteProjeto = tpg.cod_colaborador_gerente)")}
                                                {(listaCodigoCliente.Count > 0 ? "AND tpo.cod_cliente IN @CodigoCliente" : "")}
                                                AND (@Status IS NULL OR @Status = tpo.`status`)
                                                AND
                                                (
                                                    (@PrioritarioFiltroEnum IS NULL OR @PrioritarioFiltroEnum = 0)
                                                    OR
                                                    (@PrioritarioFiltroEnum = 1 AND tpo.prioritario = true)
                                                    OR
                                                    (@PrioritarioFiltroEnum = 2 AND tpo.prioritario = false)
                                                )";
                    string sql = "";

                    if (associacaoAutomaticaColaboradorAoProjeto || tbd || codColaboradorExterno.IsNull())
                    {
                        sql = @$"   SELECT
                                        tpo.cod_cliente, tclo.nome_cliente as cliente, tpo.cod_projeto, tpo.projeto
                                    FROM
                                        tb_projeto_org tpo
                                    LEFT JOIN
		                                tb_cliente_org tclo ON tpo.cod_cliente = tclo.codigo_cliente AND tpo.tb_org_id = tclo.tb_org_id
                                    {(string.IsNullOrEmpty(codigoGerenteProjeto) ? "" : "JOIN tb_projeto_gerente tpg ON tpo.cod_projeto = tpg.cod_projeto and tpo.tb_org_id = tpg.tb_org_id")}
                                    WHERE
                                        {filtrosEmComum}
                                    ORDER BY
                                        tpo.projeto;
                                ";
                    }
                    else
                    {
                        sql = @$"   SELECT
                                        tpo.cod_cliente, tclo.nome_cliente as cliente, tpo.cod_projeto, tpo.projeto
                                    FROM
                                        tb_colaborador_projeto_org AS tcpo
                                    JOIN
                                        tb_projeto_org AS tpo ON tcpo.cod_projeto = tpo.cod_projeto and tcpo.tb_org_id = tpo.tb_org_id
                                    LEFT JOIN
		                                tb_cliente_org tclo ON tpo.cod_cliente = tclo.codigo_cliente AND tpo.tb_org_id = tclo.tb_org_id
                                    {(string.IsNullOrEmpty(codigoGerenteProjeto) ? "" : "JOIN tb_projeto_gerente tpg ON tpo.cod_projeto = tpg.cod_projeto and tcpo.tb_org_id = tpg.tb_org_id")}
                                    WHERE
                                        tcpo.cod_colaborador = @CodigoColaborador
                                        AND tcpo.tb_org_id = @OrgId
                                        AND {filtrosEmComum}
                                    ORDER BY
                                        tpo.projeto;
                                ";
                    }

                    var result = await _connection.QueryAsync<dynamic>(sql,
                        new
                        {
                            OrgId = orgId,
                            CodigoColaborador = codColaboradorExterno,
                            CodigoGerenteProjeto = codigoGerenteProjeto,
                            CodigoCliente = listaCodigoCliente,
                            Status = status,
                            PrioritarioFiltroEnum = (int)prioritarioFiltroEnum
                        }
                    );

                    var resultListDB = result.ToList();
                    if (resultListDB != null && resultListDB.Any())
                    {
                        projetosLista = resultListDB.Select(x => new ProjetosColaboradorDTO
                        {
                            NomeProjeto = MapaDeAlocacaoConstants.RetornarLabelCliente(x.cod_cliente, x.cliente, x.cod_projeto, x.projeto),
                            CodigoProjeto = x.cod_projeto,
                            CodigoCliente = x.cod_cliente
                        }).ToList();
                    }

                    return projetosLista;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }
        public async Task<List<GestorDTO>> GetGestoresOrg(int orgId, List<string>? diretorias, string codDepartamento)
        {

            var connection = _dapperConnection.GetConnection();
            
            var inClauseUnidades = diretorias.BuildInClauseOrNull();
            
            var query = $@"
                SELECT 
                    vgo.nome_completo AS NomeGestor,
                    vgo.cod_colaborador_superior AS CodigoProfissional
                FROM vw_gestores_org vgo
                WHERE 
                    vgo.tb_org_id = @OrgId
                    AND vgo.ativo = 1
                    AND (@CodDepartamento IS NULL or vgo.cod_departamento = @CodDepartamento)
                    {(inClauseUnidades == null ? "" : $"AND vgo.cod_diretoria IN {inClauseUnidades}")}
                ORDER BY
                    nome_completo
                
            ";

            var parametros = new
            {
                OrgId = orgId,
                CodDepartamento = codDepartamento.ToNullSeTextoNull()
            };
            
            var result = await connection.QueryAsync<GestorDTO>(query, parametros);
            return result.ToList();
        }
        public List<NomeRecursoDTO> GetColaboradoresGestor(string codGestor, int orgId, string cpf)
        {
            var query = _colaboradorContext.vw_colaboradores_gestor
                            .Where(x => x.tb_org_id == orgId && x.cod_gerente == codGestor);

            if (!string.IsNullOrEmpty(cpf))
            {
                query = query.Where(x => x.codigo_interno_colaborador == cpf);
            }

            return query
                    .Select(x => new NomeRecursoDTO
                    {
                        Nome = x.nome_completo_colaborador,
                        Cpf = x.codigo_interno_colaborador,
                        CodigoColaborador = x.cod_colaborador
                    })
                    .OrderBy(x => x.Nome)
                    .ToList();
        }

        public List<ProjetosColaboradorDTO> GetProjetosGestor(string codGestor, int orgId)
        {
            return _colaboradorContext.tb_projeto_gerente.Where(x => x.tb_org_id == orgId && x.cod_colaborador_gerente == codGestor).ToList().Select(x =>
                new ProjetosColaboradorDTO
                {
                    NomeProjeto = _colaboradorContext.tb_projeto_org.Where(y => y.tb_org_id == orgId && y.cod_projeto == x.cod_projeto).FirstOrDefault()?.projeto ?? "",
                    CodigoProjeto = x.cod_projeto,
                }
            ).OrderBy(x => x.NomeProjeto).ToList();
        }

        public async Task<IEnumerable<ProjetosOrgResult>> ListarProjetos(int orgId, int cursor, int limite, int codStatus, string nomeProjeto)
        {
            try
            {

                var connection = _dapperConnection.GetConnection();

                var query = @"
                    SELECT 
                        p.cod_projeto AS CodProjeto,
                        p.projeto AS NomeProjeto,
                        p.cod_cliente AS CodClienteProjeto,
                        c.nome_cliente AS NomeCliente,
                        p.cod_status AS CodStatus,
                        p.status AS Status,
                        p.prioritario AS Prioritario,
                        p.codigo_oportunidade AS CodOportunidade,
                        p.data_inicio AS DataInicio,
                        p.data_fim AS DataFim,
                        p.permite_apont_sem_alocacao AS ApontSemAlocacao,
                        p.permite_apont_sem_alocacao_outro_colab AS ApontSemAlocacaoOutroColab
                    FROM tb_projeto_org p
                    INNER JOIN tb_cliente_org c 
                        ON p.cod_cliente = c.codigo_cliente
                       AND c.tb_org_id = @OrgId
                    WHERE p.tb_org_id = @OrgId
                      AND (@CodStatus = 0 OR p.cod_status = @CodStatus)
                      AND (
                            @NomeProjeto IS NULL OR @NomeProjeto = '' 
                            OR p.projeto LIKE CONCAT('%', @NomeProjeto, '%')
                            OR p.cod_projeto = @NomeProjeto
                          )
                    ORDER BY p.projeto
                    LIMIT @Cursor, @Limite;
                ";

                var parametros = new
                {
                    OrgId = orgId,
                    CodStatus = codStatus,
                    NomeProjeto = nomeProjeto,
                    Cursor = cursor,
                    Limite = limite
                };

                var result = await connection.QueryAsync<ProjetosOrgResult>(query, parametros);
                
                foreach (var item in result)
                {
                    item.LabelCodigoCliente = ProjetoConstants.RetornarLabelCliente(item.CodClienteProjeto, item.NomeCliente);
                }
                
                var aprovadoresQuery = @"
                    SELECT 
                        IFNULL(pg.cod_colaborador_gerente, 'Sem código') AS Codigo,
                        IFNULL(c.nome_completo, 'Sem nome') AS Nome,
                        pg.cod_projeto AS CodProjeto
                    FROM tb_projeto_gerente pg
                    INNER JOIN tb_colaborador_org co 
                        ON pg.cod_colaborador_gerente = co.cod_colaborador_externo 
                       AND pg.tb_org_id = co.tb_org_id
                    INNER JOIN tb_colaborador c 
                        ON co.codigo_interno_colaborador = c.codigo_interno_colaborador
                    WHERE pg.tb_org_id = @OrgId
                      AND pg.cod_projeto IN @Projetos
                    ORDER BY c.nome_completo;
                ";

                var aprovadores = await connection.QueryAsync<ProjetoOrgGerenteDTO>(aprovadoresQuery, new {
                    OrgId = orgId,
                    Projetos = result.Select(r => r.CodProjeto).ToList()
                });

                // associa os aprovadores a cada projeto
                foreach (var projeto in result)
                {
                    var aprovadoresParaEsseProjeto =  aprovadores
                        .Where(a => a.CodProjeto.TrimEnd() == projeto.CodProjeto.TrimEnd())
                        .ToList();
                    projeto.Aprovadores = aprovadoresParaEsseProjeto;
                }

                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Ocorreu um erro: " + e.Message);
            }
        }

        public async Task<bool> CadastrarProjeto(ProjetoOrgDTO param, int orgId, string codDiretoria, string diretoria, StatusProjetosDTO status, TipoCadastroProjetoOrgEnum tipoCadastroProjeto, bool clienteExiste, bool ocultoNaGestaoAlocados)
        {
            try
            {
                foreach (var codGerente in param.CodigoColaboradorGerente)
                {
                    await CadastraGerenteAsync(codGerente, param.CodProjeto, orgId);
                }

                foreach (var codColaborador in param.CodigoColaborador)
                {
                    await VincularColaboradorAoProjetoAsync(codColaborador, param.CodProjeto, param.NomeProjeto, orgId);
                }

                var connection = _dapperConnection.GetConnection();
                
                var query = @"
                INSERT INTO tb_projeto_org
                (
                    cod_projeto,
                    projeto,
                    cod_cliente,
                    cod_diretoria,
                    diretoria,
                    cod_status,
                    status,
                    data_inicio,
                    data_fim,
                    tb_org_id,
                    prioritario,
                    codigo_oportunidade,
                    permite_apont_sem_alocacao,
                    permite_apont_sem_alocacao_outro_colab,
                    cod_cliente_registro_carga,
                    nome_cliente_registro_carga,
                    tipo_cadastro
                )
                VALUES
                (
                    @CodProjeto,
                    @NomeProjeto,
                    @CodigoCliente,
                    @CodDiretoria,
                    @Diretoria,
                    @CodStatus,
                    @NomeStatusProjeto,
                    @DataInicio,
                    @DataFim,
                    @OrgId,
                    @Prioritario,
                    @CodOportunidade,
                    @ApontSemAlocacao,
                    @ApontSemAlocacaoOutroColab,
                    @CodigoClienteRegistroCarga,
                    @NomeClienteRegistroCarga,
                    @TipoCadastro
                );";

                var parametros = new
                {
                    CodProjeto = param.CodProjeto,
                    NomeProjeto = param.NomeProjeto,
                    CodigoCliente = param.ClienteProjeto.CodigoCliente,
                    CodDiretoria = codDiretoria,
                    Diretoria = diretoria,
                    CodStatus = param.CodStatus,
                    NomeStatusProjeto = status.NomeStatusProjeto,
                    DataInicio = param.DataInicio,
                    DataFim = param.DataFim,
                    OrgId = orgId,
                    Prioritario = param.Prioritario,
                    CodOportunidade = param.CodOportunidade,
                    ApontSemAlocacao = param.ApontSemAlocacao,
                    ApontSemAlocacaoOutroColab = param.ApontSemAlocacaoOutroColab,
                    CodigoClienteRegistroCarga = param.ClienteProjeto.CodigoCliente,
                    NomeClienteRegistroCarga = param.ClienteProjeto.NomeCliente,
                    TipoCadastro = tipoCadastroProjeto.ToString()
                };

                await connection.ExecuteAsync(query, parametros);

                await CadastraAtividade(orgId, param.AtividadeProjeto, param.CodProjeto);

                if (!clienteExiste)
                    await CadastraClienteAsync(param.ClienteProjeto, TipoCadastroClienteEnum.CADASTRO_VIA_PROJETO, ocultoNaGestaoAlocados, orgId);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro no repositorio: Cadastro de Projeto");
                Console.WriteLine(e.Message);
                throw;
            }
        }
        public bool ProjetoExiste(string codigoProjeto, int orgId)
        {
            var existe = _colaboradorContext.tb_projeto_org.Any(p => p.cod_projeto == codigoProjeto && p.tb_org_id == orgId);
            return existe;
        }
        
        public async Task<(string codDiretoria, string diretoria)> BuscaDadosDiretoriaAsync(string codColaboradorGerente, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT 
                    cod_diretoria AS codDiretoria,
                    diretoria
                FROM tb_colaborador_org
                WHERE cod_colaborador_externo = @CodColaboradorGerente
                  AND tb_org_id = @OrgId
                LIMIT 1
            ";

            var result = await connection.QuerySingleOrDefaultAsync<(string codDiretoria, string diretoria)>(
                query,
                new { CodColaboradorGerente = codColaboradorGerente, OrgId = orgId }
            );

            return result;
        }
        
        public async Task<StatusProjetosDTO> BuscaDadosStatusAsync(int codStatus, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT 
                    descricao_status AS NomeStatusProjeto,
                    cod_status AS CodigoStatusProjeto
                FROM tb_status_projeto
                WHERE cod_status = @CodStatus
                  AND tb_org_id = @OrgId
                LIMIT 1
            ";

            var result = await connection.QuerySingleOrDefaultAsync<StatusProjetosDTO>(
                query,
                new { CodStatus = codStatus, OrgId = orgId }
            );

            return result;
        }
        
        public async Task CadastraGerenteAsync(string codGerente, string codProjeto, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            try
            {
                // 1️⃣ Verificar se o gerente já existe
                var gerenteExiste = await connection.QuerySingleOrDefaultAsync<int>(
                    @"SELECT 1
              FROM tb_projeto_gerente
              WHERE cod_projeto = @CodProjeto
                AND cod_colaborador_gerente = @CodGerente
                AND tb_org_id = @OrgId",
                    new { CodProjeto = codProjeto, CodGerente = codGerente, OrgId = orgId }
                );

                // 2️⃣ Inserir se não existir
                if (gerenteExiste == 0) // QuerySingleOrDefaultAsync retorna 0 se não encontrar
                {
                    var queryInsert = @"
                INSERT INTO tb_projeto_gerente (cod_projeto, cod_colaborador_gerente, tipo_gerente, tb_org_id)
                VALUES (@CodProjeto, @CodGerente, @TipoGerente, @OrgId)
            ";

                    await connection.ExecuteAsync(queryInsert, new
                    {
                        CodProjeto = codProjeto,
                        CodGerente = codGerente,
                        TipoGerente = "GerenteProjeto",
                        OrgId = orgId
                    });
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Ocorreu um erro ao inserir gerente.", e);
            }
        }

        public async Task VincularColaboradorAoProjetoAsync(string codColaborador, string codProjeto, string nomeProjeto, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            // 1️⃣ Buscar colaborador
            var cpfColaborador = await connection.QuerySingleOrDefaultAsync<string>(
                @"SELECT codigo_interno_colaborador 
                  FROM tb_colaborador_org
                  WHERE cod_colaborador_externo = @CodColaborador
                    AND tb_org_id = @OrgId",
                new { CodColaborador = codColaborador, OrgId = orgId }
            );

            if (cpfColaborador == null)
                throw new Exception("Colaborador não encontrado.");

            // 2️⃣ Inserir vínculo no projeto
            var queryInsert = @"
                INSERT INTO tb_colaborador_projeto_org (cod_projeto, cod_colaborador, tb_org_id, nome_projeto)
                VALUES (@CodProjeto, @CodColaborador, @OrgId, @NomeProjeto)
            ";

            await connection.ExecuteAsync(
                queryInsert,
                new { CodProjeto = codProjeto, CodColaborador = codColaborador, OrgId = orgId, NomeProjeto = nomeProjeto }
            );
        }

        public async Task<List<AtividadeProjetoDTO>> CadastraAtividade(int orgId, List<string> atividades, string codProjeto)
        {
            try
            {
                return await _atividadeRepository.CadastroAtividades(orgId, atividades, codProjeto);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Ocorreu um erro ao cadastrar ativiadde.", e);
            }
        }

        public async Task CadastraClienteAsync(ClienteOrgDTO cliente, TipoCadastroClienteEnum tipoCadastro, bool cadastraClienteOcultoNaGestaoAlocados, int orgId)
        {
            try
            {
                await _clienteRepository.CadastrarClienteOrgAsync(cliente.CodigoCliente, cliente.NomeCliente, orgId, (bool)cliente.Ativo, cadastraClienteOcultoNaGestaoAlocados, tipoCadastro);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        
        public async Task<bool> EditarProjetoAsync( ProjetoOrgDTO param, int orgId, string codDiretoria, string diretoria, StatusProjetosDTO status, string codClienteRegistroCarga, string nomeClienteRegistroCarga, string tipoCadastroProjetoOrg, bool clienteExiste, bool deveOcultarNaGestaoDeAlocados)
        {
            try
            {
                var connection = _dapperConnection.GetConnection();

                // 1️⃣ Gerentes
                var gerentesAtuais = (await connection.QueryAsync<string>(
                    @"SELECT cod_colaborador_gerente
                      FROM tb_projeto_gerente
                      WHERE cod_projeto = @CodProjeto AND tb_org_id = @OrgId",
                    new { CodProjeto = param.CodProjeto, OrgId = orgId }
                )).ToList();

                var novosGerentes = param.CodigoColaboradorGerente.Except(gerentesAtuais).ToList();
                var gerentesDeletados = gerentesAtuais.Except(param.CodigoColaboradorGerente).ToList();

                foreach (var novoGerente in novosGerentes)
                    await CadastraGerenteAsync(novoGerente, param.CodProjeto, orgId);

                foreach (var gerente in gerentesDeletados)
                    await connection.ExecuteAsync(
                        @"DELETE FROM tb_projeto_gerente 
                          WHERE cod_colaborador_gerente = @Gerente
                            AND cod_projeto = @CodProjeto
                            AND tb_org_id = @OrgId",
                        new { Gerente = gerente, CodProjeto = param.CodProjeto, OrgId = orgId }
                    );

                // 2️⃣ Colaboradores
                var colaboradoresAtuais = (await connection.QueryAsync<string>(
                    @"SELECT cod_colaborador 
                      FROM tb_colaborador_projeto_org
                      WHERE cod_projeto = @CodProjeto AND tb_org_id = @OrgId",
                    new { CodProjeto = param.CodProjeto, OrgId = orgId }
                )).ToList();

                var novosColaboradores = param.CodigoColaborador.Except(colaboradoresAtuais).ToList();
                var colaboradoresDeletados = colaboradoresAtuais.Except(param.CodigoColaborador).ToList();

                foreach (var novoColaborador in novosColaboradores)
                    await VincularColaboradorAoProjetoAsync(novoColaborador, param.CodProjeto, param.NomeProjeto, orgId);

                foreach (var colaborador in colaboradoresDeletados)
                    await connection.ExecuteAsync(
                        @"DELETE FROM tb_colaborador_projeto_org
                          WHERE cod_colaborador = @Colaborador
                            AND cod_projeto = @CodProjeto
                            AND tb_org_id = @OrgId",
                        new { Colaborador = colaborador, CodProjeto = param.CodProjeto, OrgId = orgId }
                    );

                // 3️⃣ Cliente
                if (!clienteExiste)
                    await CadastraClienteAsync(param.ClienteProjeto, TipoCadastroClienteEnum.CADASTRO_VIA_PROJETO, deveOcultarNaGestaoDeAlocados, orgId);

                // 4️⃣ Atualizar projeto
                var queryUpdateProjeto = @"
                    UPDATE tb_projeto_org
                    SET projeto = @NomeProjeto,
                        cod_cliente = @CodCliente,
                        cod_diretoria = @CodDiretoria,
                        diretoria = @Diretoria,
                        cod_status = @CodStatus,
                        status = @StatusNome,
                        data_inicio = @DataInicio,
                        data_fim = @DataFim,
                        prioritario = @Prioritario,
                        codigo_oportunidade = @CodOportunidade,
                        permite_apont_sem_alocacao = @ApontSemAlocacao,
                        permite_apont_sem_alocacao_outro_colab = @ApontSemAlocacaoOutroColab,
                        cod_cliente_registro_carga = @CodClienteRegistroCarga,
                        nome_cliente_registro_carga = @NomeClienteRegistroCarga,
                        tipo_cadastro = @TipoCadastro
                    WHERE cod_projeto = @CodProjeto AND tb_org_id = @OrgId
                ";

                await connection.ExecuteAsync(queryUpdateProjeto, new
                {
                    NomeProjeto = param.NomeProjeto,
                    CodCliente = param.ClienteProjeto.CodigoCliente,
                    CodDiretoria = codDiretoria,
                    Diretoria = diretoria,
                    CodStatus = param.CodStatus,
                    StatusNome = status.NomeStatusProjeto,
                    DataInicio = param.DataInicio,
                    DataFim = param.DataFim,
                    Prioritario = param.Prioritario,
                    CodOportunidade = param.CodOportunidade,
                    ApontSemAlocacao = param.ApontSemAlocacao,
                    ApontSemAlocacaoOutroColab = param.ApontSemAlocacaoOutroColab,
                    CodClienteRegistroCarga = codClienteRegistroCarga,
                    NomeClienteRegistroCarga = nomeClienteRegistroCarga,
                    TipoCadastro = tipoCadastroProjetoOrg,
                    CodProjeto = param.CodProjeto,
                    OrgId = orgId
                });

                // 5️⃣ Remover atividades associadas
                await _atividadeRepository.RemoverAtividadesAssociadasComProjetoAsync(orgId, param.AtividadeProjeto, param.CodProjeto);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro na Repo: EditarProjeto: " + e.Message);
                throw;
            }
        }


        public async Task<ProjetoOrgDetalhesDTO> ObterProjetoPorCodigo(string codProjeto, int orgId)
        {
            try
            {
                var projetoDTO = new ProjetoOrgDetalhesDTO();
                var clienteOrgDTO = new ClienteOrgDTO();
                List<string> atividadesProjeto = new List<string>();

                var connection = _dapperConnection.GetConnection();

                // 1. Atividades do projeto
                var atividadesProjetoDTO = await _atividadeRepository.ListarAtividadesAssociadasComProjeto(codProjeto, orgId); atividadesProjeto = atividadesProjetoDTO.Select(x => x.Descricao).ToList();

                // 2. Projeto
                var projetoQuery = @"
                    SELECT *
                    FROM tb_projeto_org
                    WHERE cod_projeto = @CodProjeto AND tb_org_id = @OrgId;";

                var projetoContext = await connection.QueryFirstOrDefaultAsync<tb_projeto_org>(projetoQuery, new { CodProjeto = codProjeto, OrgId = orgId });
                if (projetoContext == null) return null;

                // 3. Gerentes do projeto
                var gerenteQuery = @"
                    SELECT tpg.cod_colaborador_gerente
                    FROM tb_projeto_gerente tpg
                    WHERE tpg.cod_projeto = @CodProjeto AND tpg.tb_org_id = @OrgId
                    ORDER BY tpg.cod_colaborador_gerente;";

                var gerenteProjetoContext = (await connection.QueryAsync<dynamic>(gerenteQuery, new { CodProjeto = codProjeto, OrgId = orgId })).ToList();

                // 4. Colaboradores do projeto
                var colaboradorQuery = @"
                    SELECT tcpo.cod_colaborador
                    FROM tb_colaborador_projeto_org tcpo
                    WHERE tcpo.cod_projeto = @CodProjeto AND tcpo.tb_org_id = @OrgId
                    ORDER BY tcpo.cod_colaborador;";

                var colaboradorProjetoContext = (await connection.QueryAsync<dynamic>(colaboradorQuery, new { CodProjeto = codProjeto, OrgId = orgId })).ToList();

                // 5. Cliente
                var clienteQuery = @"
                    SELECT *
                    FROM tb_cliente_org
                    WHERE codigo_cliente = @CodigoCliente AND tb_org_id = @OrgId;";

                var cliente = await connection.QueryFirstOrDefaultAsync<dynamic>(clienteQuery, new { CodigoCliente = projetoContext.cod_cliente, OrgId = orgId });

                // --- Preenchendo DTOs ---
                // Cliente
                clienteOrgDTO.CodigoCliente = projetoContext.cod_cliente;
                clienteOrgDTO.NomeCliente = cliente.nome_cliente;

                // Projeto
                projetoDTO.CodProjeto = projetoContext.cod_projeto;
                projetoDTO.NomeProjeto = projetoContext.projeto;
                projetoDTO.DataInicio = (DateTime)projetoContext.data_inicio;
                projetoDTO.DataFim = projetoContext.data_fim;
                projetoDTO.QuantidadeHorasPlanejadas = projetoContext.qtd_horas_planejadas;
                projetoDTO.Prioritario = projetoContext.prioritario;
                projetoDTO.CodOportunidade = projetoContext.codigo_oportunidade;
                projetoDTO.CodStatus = projetoContext.cod_status;
                projetoDTO.ApontSemAlocacao = (int)projetoContext.permite_apont_sem_alocacao;
                projetoDTO.ApontSemAlocacaoOutroColab = (int)projetoContext.permite_apont_sem_alocacao_outro_colab;
                projetoDTO.CodClienteRegistroCarga = projetoContext.cod_cliente_registro_carga;
                projetoDTO.NomeClienteRegistroCarga = projetoContext.nome_cliente_registro_carga;
                projetoDTO.TipoCadastro = projetoContext.tipo_cadastro;

                // Gerentes
                projetoDTO.ColaboradoresGerentes = new List<ProjetoOrgColaboradorGerenteDetalhesDTO>();
                foreach (var g in gerenteProjetoContext)
                {
                    var nomeGerente = await connection.QueryFirstOrDefaultAsync<string>(@"
                        SELECT tc.nome_completo
                        FROM tb_colaborador_org c
                        JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = c.codigo_interno_colaborador
                        WHERE c.cod_colaborador_externo = @CodColab AND c.tb_org_id = @OrgId;",
                        new { CodColab = g.cod_colaborador_gerente, OrgId = orgId });

                    projetoDTO.ColaboradoresGerentes.Add(new ProjetoOrgColaboradorGerenteDetalhesDTO
                    {
                        CodGerente = g.cod_colaborador_gerente,
                        NomeGerente = nomeGerente
                    });
                }
                projetoDTO.ColaboradoresGerentes = projetoDTO.ColaboradoresGerentes.OrderBy(x => x.NomeGerente).ToList();

                // Colaboradores
                projetoDTO.Colaboradores = new List<ProjetoOrgColaboradoresDetalhesDTO>();
                foreach (var c in colaboradorProjetoContext)
                {
                    var nomeColaborador = await connection.QueryFirstOrDefaultAsync<string>(@"
                        SELECT tc.nome_completo
                        FROM tb_colaborador_org c
                        JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = c.codigo_interno_colaborador
                        WHERE c.cod_colaborador_externo = @CodColab AND c.tb_org_id = @OrgId;",
                        new { CodColab = c.cod_colaborador, OrgId = orgId });

                    projetoDTO.Colaboradores.Add(new ProjetoOrgColaboradoresDetalhesDTO
                    {
                        CodColaborador = c.cod_colaborador,
                        NomeColaborador = nomeColaborador
                    });
                }
                projetoDTO.Colaboradores = projetoDTO.Colaboradores.OrderBy(x => x.NomeColaborador).ToList();

                // Atividades e cliente
                projetoDTO.AtividadeProjeto = atividadesProjeto;
                projetoDTO.ClienteProjeto = clienteOrgDTO;
                projetoDTO.ClienteProjeto.LabelCodigoCliente = ProjetoConstants.RetornarLabelCliente(clienteOrgDTO.CodigoCliente, clienteOrgDTO.NomeCliente);

                return projetoDTO;
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro na Repo: ObterProjetoPorCodigo");
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<ProjetoOrgColaboradorGerenteDetalhesDTO>> ListarGestoresProjeto(List<string>? diretorias, string codigoDepartamento, string codigoGestorAdm, int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    List<ProjetoOrgColaboradorGerenteDetalhesDTO> gerenteLista = new List<ProjetoOrgColaboradorGerenteDetalhesDTO>();
                    
                    
                    var inClauseUnidades = diretorias.BuildInClauseOrNull();
                    
                    string sql = $@" SELECT DISTINCT
                                        tc.nome_completo, tco.cod_colaborador_externo
                                    FROM
                                        tb_projeto_gerente tpg
                                    JOIN
                                        tb_colaborador_org tco ON tco.cod_colaborador_externo = tpg.cod_colaborador_gerente AND tco.tb_org_id = tpg.tb_org_id
                                    JOIN
                                        tb_colaborador tc ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador
                                    LEFT JOIN
                                        vw_colaboradores_gestor vcg ON vcg.cod_colaborador = tco.cod_colaborador_externo AND vcg.tb_org_id = tco.tb_org_id
                                    WHERE
                                        tpg.tb_org_id = @OrgId
                                        {(inClauseUnidades == null ? "" : $"AND tco.cod_diretoria IN {inClauseUnidades}")}
                                        AND (@CodigoDepartamento IS NULL OR @CodigoDepartamento = tco.cod_departamento)
                                        AND (@CodigoGestorAdm IS NULL OR @CodigoGestorAdm = vcg.cod_gerente);";

                    var result = await _connection.QueryAsync<dynamic>(sql,
                        new
                        {
                            OrgId = orgId,
                            CodigoDepartamento = codigoDepartamento,
                            CodigoGestorAdm = codigoGestorAdm
                        }
                    );

                    var resultListDB = result.ToList();
                    if (resultListDB != null && resultListDB.Any())
                    {
                        gerenteLista = resultListDB.Select(x => new ProjetoOrgColaboradorGerenteDetalhesDTO
                        {
                            CodGerente = x.cod_colaborador_externo,
                            NomeGerente = x.nome_completo,
                            LabelGerenteProjeto = ProjetoConstants.RetornarLabelCliente(x.cod_colaborador_externo, x.nome_completo)
                        }).ToList();
                    }

                    return gerenteLista;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<bool> RemoverAssociacaoProjetoColaborador(string codColaborador, int orgId, string codProjeto)
        {
            try
            {
                using (var _connection = _connectionString.CreateMySqlConnection())
                {
                    string query = @" DELETE FROM
                                            tb_colaborador_projeto_org
                                      WHERE
                                            cod_colaborador = @CodColaborador
                                            AND cod_projeto = @CodProjeto
                                            AND tb_org_id = @OrgId;
                                    ";

                    var parameters = new
                    {
                        CodColaborador = codColaborador,
                        CodProjeto = codProjeto,
                        OrgId = orgId
                    };

                    var remover = await _connection.ExecuteAsync(query, parameters);

                    return remover > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao executar RemoverProjetoColaborador.", ex);
            }
        }

        public async Task<IEnumerable<ProjetoComPropostaDTO>> ListarProjetosComPropostasAsync(int orgId)
        {
            using (var connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    connection.Open();

                    string sql = @"
                                    SELECT
                                        tpo.cod_cliente AS CodCliente,
                                        tpo.cod_projeto AS CodigoProjeto,
                                        tpo.projeto AS NomeProjeto,
                                        tpp.cod_proposta AS Proposta
                                    FROM
                                        tb_projeto_org tpo
                                    JOIN
                                        tb_projeto_proposta tpp ON tpo.cod_projeto = tpp.cod_projeto and tpo.tb_org_id = tpp.tb_org_id
                                    WHERE
                                        tpo.tb_org_id = @OrgId";

                    var projetoComPropostaDict = new Dictionary<string, ProjetoComPropostaDTO>();

                    var result = await connection.QueryAsync<ProjetoComPropostaDTO, string, ProjetoComPropostaDTO>(
                        sql,
                        (projeto, proposta) =>
                        {
                            if (!projetoComPropostaDict.TryGetValue(projeto.CodigoProjeto, out var projetoEntry))
                            {
                                projetoEntry = projeto;
                                projetoEntry.CodigosProposta = new List<string>();
                                projetoComPropostaDict.Add(projeto.CodigoProjeto, projetoEntry);
                            }
                            if (!string.IsNullOrEmpty(proposta))
                            {
                                projetoEntry.CodigosProposta.Add(proposta);
                            }
                            return projetoEntry;
                        },
                        new { OrgId = orgId },
                        splitOn: "Proposta");

                    return projetoComPropostaDict.Values.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao listar projetos com propostas.", ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public async Task<bool> VerificarSeCodigoClienteAntigoExiste(string codClienteAntigo, int orgId)
        {

            var _connection = _connectionString.CreateMySqlConnection();

            string query = @"SELECT 
                                1
                             FROM 
                                tb_projeto_org
                             WHERE 
                                cod_cliente = @CodClienteAntigo
                                AND tb_org_id = @OrgId
                             LIMIT 1;";

            var parameters = new
            {
                CodClienteAntigo = codClienteAntigo,
                OrgId = orgId
            };

            var result = await _connection.QueryFirstOrDefaultAsync<int?>(query, parameters);
            return result.HasValue;

        }

        
        public async Task<bool> VerificarSeCodClienteAtualEhDiferenteDoCodigoClienteCRM(string codClienteCRM, string codClienteCCH, int orgId)
        {

            var _connection = _connectionString.CreateMySqlConnection();

            string query = @"SELECT 
                                1
                             FROM 
                                tb_projeto_org
                             WHERE 
                                cod_cliente <> @CodClienteCRM 
                                and cod_cliente_registro_carga = @CodClienteCCH
                                AND tb_org_id = @OrgId
                             LIMIT 1;";

            var parameters = new
            {
                CodClienteCRM = codClienteCRM,
                CodClienteCCH = codClienteCCH,
                OrgId = orgId
            };

            var result = await _connection.QueryFirstOrDefaultAsync<int?>(query, parameters);
            return result.HasValue;

        }
        public async Task<ProjetoOrgDTO?> GetProjetoPorCodigoClienteRegistroCargaAsync(string codigoClienteCCH, int orgId)
        {
            var connection = _connectionString.CreateMySqlConnection();

            string query = @$"SELECT 
                          po.cod_projeto AS CodProjeto,
                          po.projeto AS NomeProjeto,
                          po.data_inicio AS DataInicio,
                          po.data_fim AS DataFim,
                          po.prioritario AS Prioritario,
                          po.codigo_oportunidade AS CodOportunidade,
                          po.cod_status AS CodStatus,
                          po.permite_apont_sem_alocacao AS ApontSemAlocacao,
                          po.permite_apont_sem_alocacao_outro_colab AS ApontSemAlocacaoOutroColab,
                          cl.codigo_cliente AS CodigoCliente,
                          cl.nome_cliente AS NomeCliente,
                          cl.ativo AS Ativo
                      FROM 
                          tb_projeto_org po
                      INNER JOIN 
                          tb_cliente_org cl ON po.cod_cliente = cl.codigo_cliente AND po.tb_org_id = cl.tb_org_id
                      WHERE 
                          po.cod_cliente_registro_carga = @CodClienteCCH
                          AND po.tb_org_id = @OrgId
                      LIMIT 1;";

            var parameters = new
            {
                CodClienteCCH = codigoClienteCCH,
                OrgId = orgId
            };

            var result = await connection.QueryAsync<ProjetoOrgDTO, ClienteOrgDTO, ProjetoOrgDTO>(
                query,
                (projeto, cliente) =>
                {
                    projeto.ClienteProjeto = cliente;
                    return projeto;
                },
                parameters,
                splitOn: "CodigoCliente" // Define a coluna para o split
            );

            return result.FirstOrDefault();
        }

        public async Task<int> AtualizarCodigoClienteNosProjetosPorCodClienteAntigo(string codClienteNovo, string codClienteAntigo, int orgId)
        {
            try
            {
                using (var _connection = _connectionString.CreateMySqlConnection())
                {
                    string query = @"UPDATE
                                       tb_projeto_org
                                    SET
                                       cod_cliente = @CodClienteNovo
                                    WHERE
                                       cod_cliente_registro_carga = @CodClienteAntigo
                                       AND tb_org_id = @OrgId;

                                    ";

                    var parameters = new
                    {
                        CodClienteNovo = codClienteNovo,
                        CodClienteAntigo = codClienteAntigo,
                        OrgId = orgId
                    };

                    var linhasAfetadas = await _connection.ExecuteAsync(query, parameters);

                    return linhasAfetadas;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar o código do cliente no projeto.", ex);
            }
        }

        public async Task<int> AtualizarCodigoClienteNosGestoresExternosPorCodClienteAntigo(string codClienteNovo, string codClienteAntigo, int orgId)
        {
            try
            {
                using (var _connection = _connectionString.CreateMySqlConnection())
                {
                    string query = @"UPDATE
                                       tb_gestor_externo
                                    SET
                                        codigo_cliente_registro_carga = @CodClienteAntigo,
                                        codigo_cliente = @CodClienteNovo
                                    WHERE
                                       codigo_cliente = @CodClienteAntigo
                                       AND tb_org_id = @OrgId;";

                    var parameters = new
                    {
                        CodClienteNovo = codClienteNovo,
                        CodClienteAntigo = codClienteAntigo,
                        OrgId = orgId
                    };

                    return await _connection.ExecuteAsync(query, parameters);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar o código do cliente nos gestores externos.", ex);
            }
        }
        
        public async Task<IEnumerable<ProjetoSimplesDTO>> ListarProjetosDoCliente(string codigoCliente, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            string sql = @"
                            SELECT
                                tpo.cod_projeto AS CodigoProjeto,
                                tpo.projeto AS NomeProjeto,
                                CONCAT(tpo.cod_projeto, ' - ', tpo.projeto) AS Label
                            FROM
                                tb_projeto_org tpo
                            WHERE
                                tpo.tb_org_id = @OrgId
                                AND tpo.cod_cliente = @CodigoCliente";

            var parametros = new
            {
                OrgId = orgId,
                CodigoCliente = codigoCliente
            };

            var result = await connection.QueryAsync<ProjetoSimplesDTO>(sql, parametros);

            return result;
        }

        public async Task<string> ObterDescricaoProjeto(string codProjeto, int orgId)
        {
            try
            {
                var connection = _dapperConnection.GetConnection();
                
                var query = @"
                    SELECT projeto
                    FROM tb_projeto_org
                    WHERE cod_projeto = @CodProjeto 
                      AND tb_org_id = @OrgId";

                var parametros = new
                {
                    CodProjeto = codProjeto,
                    OrgId = orgId
                };

                var descricao = await connection.QueryFirstOrDefaultAsync<string>(query, parametros);

                return descricao ?? string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter descrição do projeto {codProjeto}: {ex.Message}");
                throw;
            }
        }
    }
}