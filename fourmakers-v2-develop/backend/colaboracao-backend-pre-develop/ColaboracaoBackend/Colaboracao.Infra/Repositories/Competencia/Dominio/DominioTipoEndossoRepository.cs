using Colaboracao.Infra.Context;
using Core.DomainModel;

namespace Colaboracao.Infra.Repositories
{
    public class DominioTipoEndossoRepository : IDominioTipoEndossoDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public DominioTipoEndossoRepository(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }
    }
}
