using Colaboracao.Core.Exceptions;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Context;
using Core.DomainModel;
using Dapper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Interesse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class InteresseColaboradorRepository : IInteresseColaboradorDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        private readonly IDBConnection _dapperConnection;

        public InteresseColaboradorRepository(ColaboradorContext colaboradorContext, IDBConnection dapperConnection)
        {
            this._colaboradorContext = colaboradorContext;
            _dapperConnection = dapperConnection;
        }

        public InteresseColaboradorDTO SaveInteresseColaboradorDapper(long interesseId, string cpf, int tipoId, int skillId, bool interesseAtivo)
        {
            var dbConnection = _dapperConnection.GetConnection();

            var colabRow = dbConnection.QueryFirstOrDefault<string>(
                "SELECT codigo_interno_colaborador FROM tb_colaborador WHERE codigo_interno_colaborador = @Cpf",
                new { Cpf = cpf }
            );

            if (colabRow == null)
                throw new ValidationException("Colaborador não cadastrado!");

            var itemColabRow = dbConnection.QueryFirstOrDefault<long?>(
                @"SELECT id FROM tb_colaborador_interesse
                  WHERE codigo_interno_colaborador = @Cpf
                    AND tipo_id = @TipoId
                    AND skill_id = @SkillId",
                new { Cpf = cpf, TipoId = tipoId, SkillId = skillId }
            );

            if (itemColabRow != null)
            {
                var ativoAtual = dbConnection.QueryFirst<int>(
                    "SELECT ativo FROM tb_colaborador_interesse WHERE id = @Id",
                    new { Id = itemColabRow.Value }
                );

                if (ativoAtual == 1 && interesseAtivo)
                    throw new ValidationException("Colaborador já possui este Interesse.");

                dbConnection.Execute(
                    @"UPDATE tb_colaborador_interesse
                      SET ativo = @Ativo, data_alteracao = NOW()
                      WHERE id = @Id",
                    new { Ativo = interesseAtivo ? 1 : 0, Id = itemColabRow.Value }
                );

                return new InteresseColaboradorDTO { Id = itemColabRow.Value };
            }

            var newId = dbConnection.ExecuteScalar<long>(
                @"INSERT INTO tb_colaborador_interesse
                  (codigo_interno_colaborador, interesse_id, tipo_id, skill_id, ativo, data_criacao, data_alteracao)
                  VALUES (@Cpf, @InteresseId, @TipoId, @SkillId, @Ativo, NOW(), NOW());
                  SELECT LAST_INSERT_ID();",
                new { Cpf = cpf, InteresseId = interesseId, TipoId = tipoId, SkillId = skillId, Ativo = interesseAtivo ? 1 : 0 }
            );

            return new InteresseColaboradorDTO { Id = newId };
        }

        public List<InteresseColaboradorDTO> ListInteressesColaborador(string cpf)
        {
            var rows = _colaboradorContext.tb_colaborador_interesse
                .Where(x => x.codigo_interno_colaborador == cpf && x.ativo == 1)
                .OrderBy(x => x.id)
                .ToList();

            var interesseIds = rows.Select(x => x.interesse_id).Distinct().ToList();
            var interesses = _colaboradorContext.tb_interesse
                .Where(x => interesseIds.Contains(x.id))
                .ToDictionary(x => x.id, x => x.descricao);

            return rows.Select(row => new InteresseColaboradorDTO
            {
                Id = row.id,
                Data = row.data_alteracao,
                ColaboradorCpf = row.codigo_interno_colaborador,
                Interesse = new ItemPerfilDTO
                {
                    Id = row.interesse_id,
                    Descricao = interesses.ContainsKey(row.interesse_id) ? interesses[row.interesse_id] : null
                }
            }).ToList();
        }

        public void RemoveInteresseColaborador(long interesseId, string cpf)
        {
            var itemColabRow = _colaboradorContext.tb_colaborador_interesse
                .Where(x => x.codigo_interno_colaborador == cpf && x.interesse_id == interesseId && x.ativo == 1)
                .FirstOrDefault();

            if (itemColabRow == null)
                throw new KeyNotFoundException("Colaborador não possuí este interesse.");

            itemColabRow.ativo = 0;
            _colaboradorContext.SaveChanges();
        }
    }
}
