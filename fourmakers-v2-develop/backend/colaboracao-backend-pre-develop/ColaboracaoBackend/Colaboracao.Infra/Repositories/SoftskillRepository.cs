using Colaboracao.Infra.Context;
using Core.DomainModel.Softskill;
using DataTransferObject.Domain.Softskill;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class SoftskillRepository : ISoftskillRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public SoftskillRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }

        public SoftskillDTO GetSoftskillById(long softskillId)
        {
            var ret = new SoftskillDTO();
            var softSkillBanco = _colaboradorContext.tb_softskill.Where(x => x.id == softskillId && x.ativo == 1).FirstOrDefault();
            if (softSkillBanco != null)
            {
                ret.Descricao = softSkillBanco.descricao;
                ret.Pendente = !Convert.ToBoolean(softSkillBanco.confirmada);
            }
            return ret;
        }

        public SoftskillDTO BuscarIdUsuarioPorCpf(string cpf)
        {
            try
            {
                var userRow = _colaboradorContext.tb_usuario.FirstOrDefault(x => x.codigo_interno_colaborador.Equals(cpf));
                var softskill = new SoftskillDTO();
                softskill.UsuarioCriacaoId = userRow.id;
                return softskill;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<long> ListarSoftskillsAtribuidas(string cpfColaborador)
        {
            try
            {
                var listaSoftskillsExistentes = new List<long>();

                listaSoftskillsExistentes = _colaboradorContext.tb_colaborador_softskill
                                            .Join(_colaboradorContext.tb_softskill, cc => cc.softskill_id, c => c.id, (cc, c) => new { cc, c })
                                            .Where(x => x.cc.codigo_interno_colaborador == cpfColaborador && x.c.ativo == 1 && x.cc.ativo == 1)
                                            .Select(x => x.cc.softskill_id)
                                            .ToList();

                return listaSoftskillsExistentes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SoftskillDTO> ListarSoftSkill(string busca, int cursor, int limite)
        {
            try
            {
                var ret = new List<SoftskillDTO>();

                if (String.IsNullOrEmpty(busca))
                {
                    var aux = _colaboradorContext.tb_softskill.Where(x => x.ativo == 1)
                        .OrderBy(x => x.descricao).Skip(cursor).Take(limite)
                        .ToList();

                    foreach (var row in aux)
                    {
                        var pendente = !Convert.ToBoolean(row.confirmada);
                        var softskill = new SoftskillDTO()
                        {
                            Descricao = row.descricao,
                            Id = row.id,
                            Pendente = pendente,
                            UsuarioCriacaoId = 0
                        };
                        ret.Add(softskill);
                    }
                }
                else
                {
                    var result = _colaboradorContext.tb_softskill.Where(x => x.ativo == 1 && EF.Functions.Like(x.descricao, "%" + busca + "%")).OrderBy(x => x.descricao).Skip(cursor).Take(limite).ToList();
                    foreach (var row in result)
                    {
                        var pendente = !Convert.ToBoolean(row.confirmada);
                        var softskill = new SoftskillDTO()
                        {
                            Descricao = row.descricao,
                            Id = row.id,
                            Pendente = pendente,
                            UsuarioCriacaoId = 0
                        };
                        ret.Add(softskill);
                    }
                }
                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SoftskillDTO AdicionarSoftSkill(SoftskillDTO softskill)
        {
            try
            {
                var userRow = _colaboradorContext.tb_usuario.Find(softskill.UsuarioCriacaoId);

                if (_colaboradorContext.tb_softskill
                    .Where(x => x.descricao.ToUpper() == softskill.Descricao.ToUpper()).Count() > 0)
                    throw new Exception("Essa Soft Skill já existe.");

                var row = new tb_softskill();
                row.ativo = 1;
                row.descricao = softskill.Descricao;
                _colaboradorContext.tb_softskill.Add(row);
                _colaboradorContext.SaveChanges();
                softskill.Id = row.id;

                return softskill;
            }
            catch (Exception)
            {
                throw;
            }
            ;
        }
    }
}