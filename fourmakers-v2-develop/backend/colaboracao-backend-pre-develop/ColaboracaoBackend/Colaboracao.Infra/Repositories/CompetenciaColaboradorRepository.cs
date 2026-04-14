using Colaboracao.Infra.Context;
using Core.Domain;
using Core.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class CompetenciaColaboradorRepository : ICompetenciaColaboradorDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public CompetenciaColaboradorRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }
    }
}
