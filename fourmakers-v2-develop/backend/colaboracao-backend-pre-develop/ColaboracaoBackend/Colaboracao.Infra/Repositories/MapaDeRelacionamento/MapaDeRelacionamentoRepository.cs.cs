using Colaboracao.Core.Interfaces;
using Core.Domain.MapaDeRelacionamento;

namespace Colaboracao.Infra.Repositories.MapaDeRelacionamento
{
    public class MapaDeRelacionamentoRepository : IMapaDeRelacionamentoRepository
    {
        private readonly IDBConnection _dapperConnection;

        public MapaDeRelacionamentoRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }
    }
}