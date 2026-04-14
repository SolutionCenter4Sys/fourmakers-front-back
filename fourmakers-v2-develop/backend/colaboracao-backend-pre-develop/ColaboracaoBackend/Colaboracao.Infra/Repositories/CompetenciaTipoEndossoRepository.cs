using Colaboracao.Infra.Context;
using Core.Domain;
using Core.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class CompetenciaTipoEndossoRepository : ICompetenciaTipoEndossoDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public CompetenciaTipoEndossoRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }
    }
}
