using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Context;
using Core.DomainModel.Org;
using Dapper;
using DataTransferObject.Domain.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Org
{
    public class OrgRepository : IOrgRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        private readonly IConnectionStringCore _connectionString;
        private readonly IDBConnection _dbConnection;
        public OrgRepository(ColaboradorContext colaboradorContext, IConnectionStringCore connectionString, IDBConnection dbConnection)
        {
            _colaboradorContext = colaboradorContext;
            _connectionString = connectionString;
            _dbConnection = dbConnection;
        }

        public OrgDTO BuscarOrg(int orgId)
        {
            var orgDto = new OrgDTO();
            var orgResult = _colaboradorContext.tb_org.Where(x => x.id == orgId).FirstOrDefault();
            if (orgResult != null)
            {
                orgDto = new OrgDTO()
                {
                    Id = orgResult.id,
                    Descricao = orgResult.descricao,
                    Dominio_email = orgResult.dominio_email,
                    Subdominio = orgResult.subdominio,
                };
            }

            return orgDto;
        }

        public List<OrgDTO> GetAllOrgsId(bool ignorarOrgFourmakers)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    string sql = $@"SELECT
                            t.id AS Id,
                            t.descricao AS Descricao,
                            t.prioridade AS Prioridade,
                            t.subdominio AS Subdominio,
                            t.dominio_email AS Dominio_email
                        FROM
                            tb_org t
                        {(ignorarOrgFourmakers ? " WHERE t.id <> 1;" : ";")}
                        ";

                    return _connection.Query<OrgDTO>(sql).ToList();
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
        public int? BuscarColaboradorOrgId(string codColaborador)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    //Garante a preferencia pela ORG diferente da Fourmakers (org_id = 1)
                    string sql = @"SELECT MAX(tco.tb_org_id) FROM tb_colaborador_org tco WHERE tco.codigo_interno_colaborador = @CodColaborador;";

                    var result = _connection.Query<int>(sql, new { CodColaborador = codColaborador });

                    return result.FirstOrDefault();
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

        public async Task<string> BuscaSubDominioOrg(int orgId)
        {
            var connection = _dbConnection.GetConnection();
            var query = @"
                SELECT 
                    subdominio
                FROM tb_org
                WHERE id = @Id;
            ";

            var parametros = new
            {
                Id = orgId
            };
            
            return await connection.QueryFirstAsync<string>(query, parametros);
        }
    }
}