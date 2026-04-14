using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Context;
using Competencia.Domain.Interfaces.Services;
using Dapper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Nivel;
using DataTransferObject.Domain.Softskill;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class SoftskillColaboradorRepository : ISoftskillColaboradorRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        private readonly IDBConnection _dapperConnection;

        public SoftskillColaboradorRepository(ColaboradorContext colaboradorContext, IDBConnection dapperConnection)
        {
            _colaboradorContext = colaboradorContext;
            _dapperConnection = dapperConnection;
        }

        public void RemoveSoftskillColaborador(SoftskillColaboradorDTO softskillColaboradorDTO)
        {
            try
            {
                var itemColabRow = _colaboradorContext.tb_colaborador_softskill
                    .Where(x => x.codigo_interno_colaborador == softskillColaboradorDTO.ColaboradorCpf && x.softskill_id == softskillColaboradorDTO.IdSoftSkill && x.ativo == 1).FirstOrDefault();

                if (itemColabRow == null)
                    throw new KeyNotFoundException("Colaborador não possuí essa Soft Skill");

                itemColabRow.ativo = 0;
                _colaboradorContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SoftskillColaboradorDTO> ListarSoftskillColaborador(string cpfColaborador)
        {
            try
            {
                var connection = _dapperConnection.GetConnection();
                var query = @"
                    SELECT
                        cs.id AS Id,
                        cs.data_criacao AS Data,
                        cs.codigo_interno_colaborador AS ColaboradorCpf,
                        cs.softskill_id AS IdSoftSkill,
                        cs.tb_nivel_id AS IdNivel,
                        ss.id AS Id,
                        ss.descricao AS Descricao,
                        ss.confirmada AS Pendente,
                        n.id AS Id,
                        n.descricao AS Descricao
                    FROM
                        tb_colaborador_softskill cs
                    INNER JOIN
                        tb_softskill ss ON cs.softskill_id = ss.id
                    LEFT JOIN
                        tb_nivel n ON cs.tb_nivel_id = n.id
                    WHERE
                        cs.codigo_interno_colaborador = @ColaboradorCpf
                        AND cs.ativo = 1
                        AND ss.ativo = 1
                    ORDER BY
                        ss.descricao";

                var parametros = new { ColaboradorCpf = cpfColaborador };

                var resultado = connection.Query<SoftskillColaboradorDTO, ItemPerfilDTO, NivelDTO, SoftskillColaboradorDTO>(
                    query,
                    (dto, softskill, nivel) =>
                    {
                        dto.SoftSkill = softskill;
                        dto.Nivel = nivel ?? new NivelDTO { Id = 32, Descricao = "A definir" };
                        return dto;
                    },
                    parametros,
                    splitOn: "Id" //separar objetos pelo id para o dapper conseguir transformar em dtos (tb_colaborador_softskill/ tb_softskill/ tb_nivel)
                ).ToList();

                return resultado.OrderBy(x => x.SoftSkill.Descricao).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SoftskillColaboradorDTO AddSoftskillColaborador(SoftskillColaboradorDTO softskillColaboradorDTO)
        {
            try
            {
                var colabRow = _colaboradorContext.tb_colaborador.Find(softskillColaboradorDTO.ColaboradorCpf);
                var itemRow = _colaboradorContext.tb_softskill.Find(softskillColaboradorDTO.IdSoftSkill);

                var itemColabRow = _colaboradorContext.tb_colaborador_softskill
                    .Where(x => x.codigo_interno_colaborador == softskillColaboradorDTO.ColaboradorCpf && x.softskill_id == softskillColaboradorDTO.IdSoftSkill).FirstOrDefault();

                tb_nivel nivelRow = null;
                if (softskillColaboradorDTO.IdNivel != null)
                {
                    nivelRow = _colaboradorContext.tb_nivel.Find(softskillColaboradorDTO.IdNivel);
                    if (nivelRow == null)
                        throw new Exception("Nível do item não encontrado.");
                }
                if (itemColabRow != null)
                {
                    if (itemColabRow.ativo == 1)
                    {
                        throw new ValidationException("Colaborador já possuí essa Soft Skill.");
                    }
                    else
                    {
                        itemColabRow.ativo = 1;
                        itemColabRow.tb_nivel_id = softskillColaboradorDTO.IdNivel;
                        _colaboradorContext.SaveChanges();
                        softskillColaboradorDTO.Id = itemColabRow.id;
                        return softskillColaboradorDTO;
                    }
                }
                if (itemRow == null)
                {
                    throw new ValidationException("Essa Soft Skill não existe.");
                }
                if (colabRow == null)
                {
                    throw new ValidationException("Colaborador não existe.");
                }
                else
                {
                    var row = new tb_colaborador_softskill();
                    row.ativo = 1;
                    row.codigo_interno_colaborador = colabRow.codigo_interno_colaborador;
                    row.softskill_id = itemRow.id;
                    if (nivelRow != null)
                        row.tb_nivel_id = nivelRow.id;

                    _colaboradorContext.tb_colaborador_softskill.Add(row);
                    _colaboradorContext.SaveChanges();

                    softskillColaboradorDTO.Id = row.id;
                    return softskillColaboradorDTO;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public SoftskillColaboradorDTO AlterarSoftskillColaborador(SoftskillColaboradorDTO softskillColaboradorDTO)
        {
            try
            {
                var softskillRow = _colaboradorContext.tb_softskill.Find(softskillColaboradorDTO.SoftSkill.Id);

                if (softskillRow == null)
                    throw new Exception("Soft Skill não encontrada.");

                var softskillColaboradorRow = softskillRow.tb_colaborador_softskill.Where(x => x.softskill_id == softskillRow.id && x.codigo_interno_colaborador == softskillColaboradorDTO.ColaboradorCpf).FirstOrDefault();

                if (softskillColaboradorRow == null)
                {
                    throw new Exception("Colaborador não possuí esta Soft Skill");
                }

                tb_nivel nivelRow = null;
                if (softskillColaboradorDTO.Nivel.Id != null)
                {
                    nivelRow = _colaboradorContext.tb_nivel.Find(softskillColaboradorDTO.Nivel.Id);
                    if (nivelRow == null)
                        throw new Exception("Nível do item não encontrado.");
                    else if (nivelRow.tb_item_perfil.descricao != "SOFTSKILL")
                        throw new Exception("Nível do item não pertence a Soft Skill");
                }

                if (nivelRow != null)
                    softskillColaboradorRow.tb_nivel = nivelRow;

                _colaboradorContext.SaveChanges();

                softskillColaboradorDTO.Id = softskillColaboradorRow.id;

                return softskillColaboradorDTO;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}