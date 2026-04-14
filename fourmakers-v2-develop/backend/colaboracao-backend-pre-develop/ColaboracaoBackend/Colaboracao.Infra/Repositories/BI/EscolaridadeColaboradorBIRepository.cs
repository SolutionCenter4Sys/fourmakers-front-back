using Colaboracao.Core.Interfaces;
using Core.Domain.BI;
using Dapper;
using DataTransferObject.Domain.Escolaridade;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories.BI
{
    public class EscolaridadeColaboradorBIRepository : IEscolaridadeColaboradorBIRepository
    {
        private IConnectionStringCore _connectionString;

        public EscolaridadeColaboradorBIRepository(IConnectionStringCore connectionString)
        {
            _connectionString = connectionString;
        }

        public List<KeyValuePair<string, EscolaridadeDTO>> Listar()
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var query = @"  select
                                        te.id, te.codigo_interno_colaborador, te.instituicao, te.descricao,
                                        te.data_inicio, te.data_termino, te.tb_formacao_id, tf.descricao as formacao_descricao,
                                        te.tipo_diploma_id, ttd.descricao as tipo_diploma_descricao
                                    from
                                        tb_escolaridade te
                                        left join tb_formacao tf on tf.id = te.tb_formacao_id
                                        left join tb_tipo_diploma ttd on ttd.id = te.tipo_diploma_id
                                    where
                                        te.ativo = 1;";

                    return _connection.Query<dynamic>(query)
                        .Select(x => new KeyValuePair<string, EscolaridadeDTO>(x.codigo_interno_colaborador, new EscolaridadeDTO
                        {
                            Ativo = true,
                            ColaboradorCpf = x.codigo_interno_colaborador,
                            DataInicio = x.data_inicio,
                            DataTermino = x.data_termino,
                            Descricao = x.descricao,
                            FormacaoDescricao = x.formacao_descricao,
                            FormacaoId = x.tb_formacao_id,
                            Id = x.id,
                            Instituicao = x.instituicao,
                            TipoDiplomaId = x.tipo_diploma_id
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