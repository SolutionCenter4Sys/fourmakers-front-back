using Colaboracao.Infra.Context;
using Core.DomainModel;

namespace Colaboracao.Infra.Repositories
{
    public class DominioAntigoRepository : IDominioAntigoDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public DominioAntigoRepository(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }
    }
}
