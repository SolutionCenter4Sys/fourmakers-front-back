using Colaboracao.Infra.Context;
using Core.DomainModel;

namespace Colaboracao.Infra.Repositories
{
    public class DominioEndossoRepository : IDominioEndossoDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public DominioEndossoRepository(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }
    }
}
