using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Context;
using Core.Domain.Projeto;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Projeto
{
    public class ProjetoRelatorioRepository : IProjetoRelatorioRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        private IConnectionStringCore _connectionString;

        public ProjetoRelatorioRepository(ColaboradorContext colaboradorContext, Microsoft.Extensions.Configuration.IConfiguration configuration, IConnectionStringCore connectionString)
        {
            _colaboradorContext = colaboradorContext;
            this._connectionString = connectionString;
        }

        public async Task<List<dynamic>> ListarProjetosParaExportacao(int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("@org_id", orgId, DbType.Int32);

                    var apontamentoResult = await _connection.QueryAsync<dynamic>(
                        "spr_rpt_relatorio_projeto",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return apontamentoResult.ToList();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }
    }
}