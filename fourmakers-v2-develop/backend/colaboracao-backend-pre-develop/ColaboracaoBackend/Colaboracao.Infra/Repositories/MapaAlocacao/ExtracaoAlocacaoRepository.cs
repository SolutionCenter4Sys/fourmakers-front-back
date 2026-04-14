using Colaboracao.Core.Interfaces;
using Core.Domain.MapaAlocacao;
using Dapper;
using DataTransferObject.Domain.MapaDeAlocacao;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.MapaAlocacao
{
    public class ExtracaoAlocacaoRepository : IExtracaoAlocacaoRepository
    {
        private IConnectionStringCore _connectionString;
        private readonly IDBConnection _dapperConnection;
        public ExtracaoAlocacaoRepository(IConnectionStringCore connectionString, IDBConnection dapperConnection)
        {
            this._connectionString = connectionString;
            _dapperConnection = dapperConnection;
        }

        public async Task<List<dynamic>> ExportarAlocacoesTodasExcel(int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    string sql = $@"call spr_rpt_extracao_mapa_alocacao(@OrgId);";

                    var extracaoAlocacoesList = await _connection.QueryAsync<dynamic>(sql, new
                    {
                        OrgId = orgId
                    });

                    return extracaoAlocacoesList.ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }
    
        //TODO: Remover caso passar por QA e for aprovado usando o repositorio do mapa de alocacao
        public async Task<List<dynamic>> ExportarAlocacoesPorPesquisaExcel(string pesquisa, int? periodoAlocacaoId, int orgId, string codigoUnidade = null, string codigoDepartamento = null, string codigoGestorAdm = null, string codigoColabOuTbd = null, TipoProfissionalEnum filtroTipoProfissional = 0, string codigoGestorProjeto = null, string listaCodigoClientes = null, bool apenasProjetosPrioritarios = false, string listaCodigoProjetos = null, string codigoStatusProjeto = null, string qtdGerenteProjetoPrioridade = null, bool incluirInativos = false, string habilidades = null, string perfis = null)
        {
            try
            {
                var connection = _dapperConnection.GetConnection();

                var listarAlocacoes = await connection.QueryAsync<dynamic>(
                    "spr_get_alocacao_colaborador_e_tbd",
                    new
                    {
                        p_pesquisa = pesquisa,
                        p_org_id = orgId,
                        p_periodo_alocado_id = periodoAlocacaoId,
                        p_codigo_diretoria = codigoUnidade,
                        p_codigo_departamento = codigoDepartamento,
                        p_codigo_gestor_adm = codigoGestorAdm,
                        p_lista_codigo_colab_ou_tbd = codigoColabOuTbd,
                        p_filtro_tipo_profissional = (int)filtroTipoProfissional,
                        p_codigo_gestor_projeto = codigoGestorProjeto,
                        p_lista_codigo_clientes = listaCodigoClientes,
                        p_apenas_projetos_prioritarios = apenasProjetosPrioritarios,
                        p_lista_codigo_projetos = listaCodigoProjetos,
                        p_codigo_status_projeto = codigoStatusProjeto,
                        p_qtd_gerente_projeto_prioridade = qtdGerenteProjetoPrioridade,
                        p_incluir_inativos = incluirInativos,
                        p_lista_codigo_perfis = perfis,
                        p_lista_codigo_habilidades = habilidades
                    },
                    commandType: CommandType.StoredProcedure
                );

                return listarAlocacoes.ToList();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                throw new Exception("Erro ao listar alocações", ex);
            }
        }
    }
}