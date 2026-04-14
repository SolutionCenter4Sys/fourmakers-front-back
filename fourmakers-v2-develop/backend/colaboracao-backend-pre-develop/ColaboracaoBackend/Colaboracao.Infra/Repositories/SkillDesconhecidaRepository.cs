using Colaboracao.Core.Interfaces;
using Core.DomainModel;
using Dapper;
using DataTransferObject.Domain.SkillDesconhecida;
using DataTransferObject.Domain.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories
{
    public class SkillDesconhecidaRepository : ISkillDesconhecidaRepository
    {
        private readonly UsuarioLogadoDTO _usuarioLogadoDTO;
        private readonly IDBConnection _dapperConnection;

        public SkillDesconhecidaRepository(IAspNetUser aspNetUser, IDBConnection dapperConnection)
        {
            _usuarioLogadoDTO = aspNetUser.GetUsuarioLogado();
            _dapperConnection = dapperConnection;
        }

        public async Task<SkillDesconhecidaDTO> AddSkillDesconhecida(string descricao, long usuarioId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                INSERT INTO tb_skill_desconhecida
                (
                    descricao,
                    usuario_criacao_id
                )
                VALUES
                (
                    @Descricao,
                    @UsuarioCriacao
                );

                SELECT LAST_INSERT_ID();";

            var parameters = new
            {
                Descricao = descricao,
                UsuarioCriacao = usuarioId,
            };

            var result = await connection.ExecuteScalarAsync<int>(query, parameters);

            var skillDto = await BuscarSkillDesconhecidaPorId(result);

            return skillDto;
        }

        public async Task<long> GetUsuarioCriacaoId(string cpf)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                    SELECT
                        tu.id
                    FROM tb_usuario tu
                    WHERE
                        tu.codigo_interno_colaborador = @Cpf;
                ";

            var parameters = new
            {
                Cpf = cpf
            };

            var result = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

            return result;
        }

        public async Task<SkillDesconhecidaDTO> BuscarSkillDesconhecidaPorId(int id)
        {
            try
            {
                var connection = _dapperConnection.GetConnection();

                var query = @"
                    SELECT
                        tsd.id AS Id,
                        tsd.descricao AS Descricao,
                        tsd.usuario_criacao_id AS UsuarioCriacaoId
                    FROM tb_skill_desconhecida tsd
                    WHERE
                        tsd.id = @Id;";

                var parametros = new
                {
                    Id = id
                };

                var resultado = await connection.QueryAsync<SkillDesconhecidaDTO>(query, parametros);

                return resultado.FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<SkillDesconhecidaDTO>> ListSkillDesconhecida(string busca, int limite)
        {
            try
            {
                var connection = _dapperConnection.GetConnection();

                var query = @"
                    SELECT
                        id AS Id,
                        descricao AS Descricao,
                        usuario_criacao_id AS UsuarioCriacaoId
                    FROM tb_skill_desconhecida tsd
                    WHERE
                        LOWER(descricao) LIKE LOWER(CONCAT('%', @Filtro, '%'))
                    LIMIT @Limite";

                var parametros = new
                {
                    Filtro = busca,
                    Limite = limite
                };

                var resultado = await connection.QueryAsync<SkillDesconhecidaDTO>(query, parametros);

                return resultado.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}