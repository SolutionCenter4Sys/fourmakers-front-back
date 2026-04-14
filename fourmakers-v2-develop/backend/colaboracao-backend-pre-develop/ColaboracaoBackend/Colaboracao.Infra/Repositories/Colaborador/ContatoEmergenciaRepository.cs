using Colaboracao.Core.Interfaces;
using Core.DomainModel.Colaborador;
using Dapper;
using DataTransferObject.Domain.Colaborador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Colaborador
{
    public class ContatoEmergenciaRepository : IContatoEmergenciaRepository
    {
        private IConnectionStringCore _connectionString;

        public ContatoEmergenciaRepository(IConnectionStringCore connectionString)
        {
            _connectionString = connectionString;
        }
        public void AlteraContatoEmergencia(ContatoEmergenciaDTO contatoEmergencia)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    string sql = @"update tb_contato_emergencia
                        set nome = @Nome, grau_parentesco = @GrauParentesco, telefone = @Telefone
                        where id = @Ordem;";

                    if (_connection.Execute(sql, contatoEmergencia) == 0)
                        throw new Exception("Falha ao alterar contato de emergencia");
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

        public void DeletaContatoEmergencia(ContatoEmergenciaDTO contatoEmergencia)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    string sql = @"DELETE from tb_contato_emergencia where id = @Ordem;";

                    if (_connection.Execute(sql, contatoEmergencia) == 0)
                        throw new Exception("Falha ao deletar contato de emergencia");
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

        public void InsereContatoEmergencia(ContatoEmergenciaDTO contatoEmergencia, string cpf)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    string sql = @"INSERT INTO tb_contato_emergencia(id, nome, telefone, grau_parentesco, codigo_interno_colaborador)
                        VALUES (@Ordem, @Nome, @Telefone, @GrauParentesco, @Cpf);";

                    if (_connection.Execute(sql, new
                    {
                        contatoEmergencia.Ordem,
                        contatoEmergencia.Nome,
                        contatoEmergencia.Telefone,
                        contatoEmergencia.GrauParentesco,
                        Cpf = cpf
                    }) == 0)
                        throw new Exception("Falha ao inserir contato de emergencia");
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

        public async Task<List<ContatoEmergenciaDTO>> ListaContatoEmergencia(string cpf)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    string sql = @"SELECT * FROM tb_contato_emergencia where codigo_interno_colaborador = @Cpf;";

                    var parametros = new { Cpf = cpf };
                    var resultado = await _connection.QueryAsync(sql, parametros);

                    var atividadesPorColaborador = resultado.Select(r => new ContatoEmergenciaDTO
                    {
                        GrauParentesco = r.grau_parentesco,
                        Nome = r.nome,
                        Telefone = r.telefone,
                        Ordem = r.id.ToString()
                    });

                    return atividadesPorColaborador.ToList();
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
    }
}