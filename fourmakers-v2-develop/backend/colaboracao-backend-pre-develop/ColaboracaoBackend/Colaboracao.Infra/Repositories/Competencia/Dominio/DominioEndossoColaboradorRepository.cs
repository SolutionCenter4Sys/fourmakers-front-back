using Colaboracao.Infra.Context;
using Core.DomainModel;

namespace Colaboracao.Infra.Repositories
{
    public class DominioEndossoColaboradorRepository : IDominioEndossoColaboradorDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public DominioEndossoColaboradorRepository(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }
    }
}
