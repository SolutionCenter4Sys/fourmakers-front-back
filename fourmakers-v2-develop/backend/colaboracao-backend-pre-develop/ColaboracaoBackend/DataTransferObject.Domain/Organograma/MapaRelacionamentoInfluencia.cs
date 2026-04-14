using System;

namespace DataTransferObject.Domain
{
    // Ids fixos = coluna id em tb_mapa_relacionamento_influencia
    public enum MapaRelacionamentoInfluencia
    {
        Decisor = 1,
        Influenciador = 2,
        Bloqueador = 3,
        Operacional = 4
    }

    public static class MapaRelacionamentoInfluenciaExtensions
    {
        public static bool IsValid(int id)
            => Enum.IsDefined(typeof(MapaRelacionamentoInfluencia), id);
    }
}
