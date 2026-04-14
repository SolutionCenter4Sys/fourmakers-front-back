using Core.Domain.MapaDeRelacionamento;
using MapaDeRelacionamento.Domain.Interfaces;

namespace MapaDeRelacionamento.Domain.Impl;

public class MapaDeRelacionamentoService : IMapaDeRelacionamentoService
{
    private readonly IMapaDeRelacionamentoRepository _mapaDeRelacionamentoRepository;

    public MapaDeRelacionamentoService(IMapaDeRelacionamentoRepository mapaDeRelacionamentoRepository)
    {
        _mapaDeRelacionamentoRepository = mapaDeRelacionamentoRepository;
    }

}
