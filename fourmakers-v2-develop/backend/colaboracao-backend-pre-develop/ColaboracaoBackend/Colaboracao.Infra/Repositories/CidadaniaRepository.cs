using Colaboracao.Core.Interfaces;
using Core.DomainModel;
using Dapper;
using DataTransferObject.Domain.Colaborador.Cidadania;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories
{
    public class CidadaniaRepository : ICidadaniaRepository
    {
        private readonly IDBConnection dapperConnection;

        public CidadaniaRepository(IDBConnection dapperConnection)
        {
            this.dapperConnection = dapperConnection;
        }

        public async Task<List<CidadaniaDTO>> ListarCidadanias()
        {
            var connection = dapperConnection.GetConnection();

            var query = @"
                SELECT
                    tcn.id as Id,
                    tcn.descricao AS Descricao
                FROM
                    tb_cidadania tcn;
            ";

            var result = await connection.QueryAsync<CidadaniaDTO>(query);
            return result.ToList();
        }

        public async Task<List<CidadaniaStatusDTO>> ListarStatusCidadania()
        {
            var connection = dapperConnection.GetConnection();

            var query = @"
                SELECT
                    tcns.id as Id,
                    tcns.descricao AS Descricao
                FROM
                    tb_cidadania_status tcns;
            ";

            var result = await connection.QueryAsync<CidadaniaStatusDTO>(query);
            return result.ToList();
        }

        public async Task<CidadaniaColaboradorDTO> InserirCidadaniaColaborador(int cidadaniaId, int cidadaniaStatusId, string codigoInternoColaborador)
        {
            var connection = dapperConnection.GetConnection();

            var query = @"
                INSERT INTO tb_cidadania_colaborador (codigo_interno_colaborador, tb_cidadania_id, tb_cidadania_status_id)
                VALUES (@CodigoInternoColaborador, @CidadaniaId, @CidadaniaStatusId);

                SELECT LAST_INSERT_ID();
            ";

            var parameteros = new
            {
                CodigoInternoColaborador = codigoInternoColaborador,
                CidadaniaId = cidadaniaId,
                CidadaniaStatusId = cidadaniaStatusId,
            };

            var result = await connection.ExecuteScalarAsync<int>(query, parameteros);

            var buscaCidadaniaColaborador = await BuscarCidadaniaColaboradorPorId(result);

            return buscaCidadaniaColaborador;
        }

        public async Task<List<CidadaniaColaboradorDTO>> ListarCidadaniasColaborador(string codigoInternoColaborador)
        {
            var connection = dapperConnection.GetConnection();

            var query = @"
                    SELECT
                        tcc.id AS Id,
                        tcc.codigo_interno_colaborador AS CodigoInternoColaborador,
                        tcc.tb_cidadania_id AS CidadaniaId,
                        tcc.tb_cidadania_status_id AS CidadaniaStatusId,
                        tcc.data_criacao AS DataCriacao,
                        tcc.data_alteracao AS DataAlteracao,
                        tcns.descricao AS StatusDescricao,
                        tcn.descricao AS CidadaniaDescricao
                    FROM
                        tb_cidadania_colaborador tcc
                    JOIN
                        tb_cidadania tcn ON tcc.tb_cidadania_id = tcn.id
                    JOIN
                        tb_cidadania_status tcns ON tcc.tb_cidadania_status_id = tcns.id
                    WHERE
                        tcc.codigo_interno_colaborador = @CodigoInternoColaborador
                        AND tcc.ativo = 1;
            ";

            var parametros = new
            {
                CodigoInternoColaborador = codigoInternoColaborador
            };

            var result = await connection.QueryAsync<CidadaniaColaboradorDTO>(query, parametros);

            return result.ToList();
        }

        public async Task<CidadaniaColaboradorDTO> BuscarCidadaniaColaboradorPorId(int cidadaniaColaboradorId)
        {
            var connection = dapperConnection.GetConnection();

            var query = @"
                    SELECT
                        tcc.id AS Id,
                        tcc.codigo_interno_colaborador AS CodigoInternoColaborador,
                        tcc.tb_cidadania_id AS CidadaniaId,
                        tcc.tb_cidadania_status_id AS CidadaniaStatusId,
                        tcc.data_criacao AS DataCriacao,
                        tcc.data_alteracao AS DataAlteracao,
                        tcns.descricao AS StatusDescricao,
                        tcn.descricao AS CidadaniaDescricao
                    FROM
                        tb_cidadania_colaborador tcc
                    JOIN
                        tb_cidadania tcn ON tcc.tb_cidadania_id = tcn.id
                    JOIN
                        tb_cidadania_status tcns ON tcc.tb_cidadania_status_id = tcns.id
                    WHERE
                        tcc.id = @CidadaniaColaboradorId
                        AND tcc.ativo = 1;
            ";

            var parametros = new
            {
                CidadaniaColaboradorId = cidadaniaColaboradorId
            };

            var result = await connection.QueryFirstOrDefaultAsync<CidadaniaColaboradorDTO>(query, parametros);

            return result;
        }

        public async Task<bool> VerificaSeExisteCidadaniaJaCadastradaParaColaborador(int cidadaniaId, string codigoInternoColaborador)
        {
            var connection = dapperConnection.GetConnection();

            var query = @"
                SELECT
                    tcc.id
                FROM
                    tb_cidadania_colaborador tcc
                WHERE
                    tcc.tb_cidadania_id = @CidadaniaId
                    AND tcc.codigo_interno_colaborador = @CodigoInternoColaborador
                    AND tcc.ativo = 1;
            ";

            var parametros = new
            {
                CidadaniaId = cidadaniaId,
                CodigoInternoColaborador = codigoInternoColaborador
            };

            var count = await connection.QueryFirstOrDefaultAsync<int>(query, parametros);

            return count > 0;
        }
        public async Task<CidadaniaColaboradorDTO> AtualizarCidadaniaColaborador(int statusId, int id)
        {
            var connection = dapperConnection.GetConnection();

            var query = @"
                UPDATE
                    tb_cidadania_colaborador
                SET
                    tb_cidadania_status_id = @StatusId,
                    data_alteracao = CURRENT_TIMESTAMP
                WHERE
                    id = @CidadaniaColaboradorId;
            ";

            var parametros = new
            {
                StatusId = statusId,
                CidadaniaColaboradorId = id
            };

            await connection.ExecuteAsync(query, parametros);

            var buscaCidadaniaColaborador = await BuscarCidadaniaColaboradorPorId(id);
            return buscaCidadaniaColaborador;
        }

        public async Task<bool> RemoverCidadaniaColaborador(int id)
        {
            var connection = dapperConnection.GetConnection();

            var query = @"
                UPDATE tb_cidadania_colaborador
                SET ativo = 0,
                    data_alteracao = CURRENT_TIMESTAMP
                WHERE id = @Id;
            ";

            var parametros = new
            {
                Id = id
            };

            var linhasAfetadas = await connection.ExecuteAsync(query, parametros);

            return linhasAfetadas > 0;
        }
    }
}