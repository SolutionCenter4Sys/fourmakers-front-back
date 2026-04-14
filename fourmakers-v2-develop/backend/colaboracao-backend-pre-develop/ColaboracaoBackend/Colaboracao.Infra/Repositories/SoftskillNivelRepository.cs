using Colaboracao.Infra.Context;
using Competencia.Domain.Interfaces.Services;
using DataTransferObject.Domain.Nivel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class SoftskillNivelRepository : ISoftskillNivelRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public SoftskillNivelRepository(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }

        public long BuscarIdItemPerfilPorDescricao(string descricaoItemPerfil)
        {
            try
            {
                var softSkillId = _colaboradorContext.tb_item_perfil.Where(x => x.descricao.Equals(descricaoItemPerfil)).Select(X => X.id).FirstOrDefault();
                return softSkillId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<NivelDTO> ListarNivelSoftskill(long itemPerfilId)
        {
            try
            {
                var ret = new List<NivelDTO>();
                var niveisDaSoftskill = _colaboradorContext.tb_nivel.Where(x => x.tb_item_perfil_id == itemPerfilId);

                ret = niveisDaSoftskill.Select(x => new NivelDTO()
                {
                    Id = x.id,
                    Descricao = x.descricao,
                    PrioridadeUnificacao = x.prioridade_unificacao,
                    OrdemExibicao = x.ordem_exibicao
                })
                                        .OrderBy(x => x.OrdemExibicao)
                                        .ToList();
                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}