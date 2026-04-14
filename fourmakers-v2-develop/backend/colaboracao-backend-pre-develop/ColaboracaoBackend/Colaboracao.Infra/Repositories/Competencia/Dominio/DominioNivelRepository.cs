using Colaboracao.Infra.Context;
using Core.DomainModel;

namespace Colaboracao.Infra.Repositories
{
    public class DominioNivelRepository : IDominioNivelDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public DominioNivelRepository(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }
    }
}
