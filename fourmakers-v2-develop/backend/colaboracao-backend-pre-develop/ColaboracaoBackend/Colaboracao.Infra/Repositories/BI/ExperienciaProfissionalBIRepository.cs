using Colaboracao.Core.Interfaces;
using Core.Domain.BI;
using Dapper;
using DataTransferObject.Domain.Experiencia;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories.BI
{
    public class ExperienciaProfissionalBIRepository : IExperienciaProfissionalBIRepository
    {
        private IConnectionStringCore _connectionString;

        public ExperienciaProfissionalBIRepository(IConnectionStringCore connectionString)
        {
            _connectionString = connectionString;
        }

        private List<KeyValuePair<long, string>> getProjetos()
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var query = @"select * from tb_experiencia_projeto";

                    return _connection.Query<dynamic>(query)
                        .Select(x => new KeyValuePair<long, string>(x.experiencia_id, x.nome_projeto)).ToList();
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

        public List<KeyValuePair<string, ListaExperienciaDTO>> Listar()
        {
            var projetos = getProjetos();
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var query = @"select * from tb_experiencia;";

                    return _connection.Query<dynamic>(query)
                        .Select(x => new KeyValuePair<string, ListaExperienciaDTO>(x.codigo_interno_colaborador, new ListaExperienciaDTO
                        {
                            Atividades = x.descricao,
                            Atual = x.data_saida == null,
                            ColaboradorCpf = x.codigo_interno_colaborador,
                            DataInicio = x.data_inicio,
                            DataSaida = x.data_saida,
                            Empresa = x.empresa,
                            Funcao = x.titulo,
                            Id = x.id,
                            Projetos = projetos.Where(y => y.Key == x.id).Select(y => y.Value).ToList()
                        })).ToList();
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