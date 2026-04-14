using Colaboracao.Infra.Context;
using Core.Domain;
using Core.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class CompetenciaGenericoRepository : ICompetenciaGenericoDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public CompetenciaGenericoRepository(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }

        public List<long> ListarIdsPorCompetenciaId(long id)
        {
            var ret = new List<long>();

            var rows = _colaboradorContext.tb_colaborador_competencia.Where(x => x.competencia_id == id && x.ativo == 1).ToList();

            foreach (var item in rows)
            {
                ret.Add(item.id);
            }

            return ret;
        }
    }
}
