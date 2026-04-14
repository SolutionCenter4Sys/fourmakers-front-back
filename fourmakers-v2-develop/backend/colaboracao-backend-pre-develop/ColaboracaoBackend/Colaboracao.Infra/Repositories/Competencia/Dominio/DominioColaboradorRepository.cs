using Colaboracao.Infra.Context;
using Core.DomainModel;

namespace Colaboracao.Infra.Repositories.Competencia.Dominio
{
    public class DominioColaboradorRepository : IDominioColaboradorDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public DominioColaboradorRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }
    }
}
