using Colaboracao.Infra.Context;
using Core.DomainModel;

namespace Colaboracao.Infra.Repositories
{
    public class DominioGenericoRepository : IDominioGenericoDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public DominioGenericoRepository(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }
    }
}
