using DataTransferObject.Domain.Base;
using System.Collections.Generic;

namespace DataTransferObject.Domain.LG.Holerite
{
    public class BuscarHoleritesResult : StatusResult
    {
        public Dictionary<int, List<HoleriteSimplesDTO>> HoleritesPorAno { get; set; }
    }
}