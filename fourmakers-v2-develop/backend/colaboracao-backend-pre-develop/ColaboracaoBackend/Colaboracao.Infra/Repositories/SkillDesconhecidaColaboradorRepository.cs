using Colaboracao.Core.Interfaces;
using Core.Domain;
using Dapper;
using DataTransferObject.Domain.Nivel;
using DataTransferObject.Domain.SkillDesconhecida;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories
{
    public class SkillDesconhecidaColaboradorRepository : ISkillDesconhecidaColaboradorRepository
    {
        private readonly IDBConnection _dapperConnection;

        public SkillDesconhecidaColaboradorRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public void InserirSkillDesconhecidaColaborador(SkillDesconhecidaColaboradorDTO skillDesconhecidaColaboradorDTO)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                INSERT INTO tb_colaborador_skill_desconhecida
                (
                    codigo_interno_colaborador,
                    skill_desconhecida_id,
                    tb_nivel_id
                )
                VALUES
                (
                    @CodInternoColaborador,
                    @SkillDesconhecidaId,
                    @NivelId
                )";

            var parameters = new
            {
                SkillDesconhecidaId = skillDesconhecidaColaboradorDTO.IdSkillDesconhecida,
                CodInternoColaborador = skillDesconhecidaColaboradorDTO.ColaboradorCpf,
                NivelId = skillDesconhecidaColaboradorDTO.IdNivel
            };

            connection.Execute(query, parameters);
        }

        public async Task<List<SkillDesconhecidaColaboradorDTO>> ListarSkillDesconhecidasColaboador(string codInternoColaborador)
        {
            try
            {
                var connection = _dapperConnection.GetConnection();
                var query = @"
                    SELECT
                        ci.id AS Id,
                        ci.data_criacao AS Data,
                        ci.codigo_interno_colaborador AS ColaboradorCpf,
                        ci.skill_desconhecida_id AS IdIdioma,
                        ci.tb_nivel_id AS IdNivel,
                        i.id AS Id,
                        i.descricao AS Descricao,
                        n.id AS Id,
                        n.descricao AS Descricao
                    FROM
                        tb_colaborador_skill_desconhecida ci
                    INNER JOIN
                        tb_skill_desconhecida i ON ci.skill_desconhecida_id = i.id
                    LEFT JOIN
                        tb_nivel n ON ci.tb_nivel_id = n.id
                    WHERE
                        ci.codigo_interno_colaborador = @ColaboradorCpf
                        AND ci.ativo = 1
                    ORDER BY
                        i.descricao";

                var parametros = new { ColaboradorCpf = codInternoColaborador };

                var resultado = await connection.QueryAsync<SkillDesconhecidaColaboradorDTO, SkillDesconhecidaDTO, NivelDTO, SkillDesconhecidaColaboradorDTO>(
                    query,
                    (dto, skillDesconhecida, nivel) =>
                    {
                        dto.SkillDesconhecida = skillDesconhecida;
                        dto.Nivel = nivel ?? new NivelDTO { Id = 32, Descricao = "A definir" };
                        return dto;
                    },
                    parametros,
                    splitOn: "Id" //separar objetos pelo id para o dapper conseguir transformar em dtos (tb_colaborador_idioma/ tb_idioma/ tb_nivel)
                );

                return resultado.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}