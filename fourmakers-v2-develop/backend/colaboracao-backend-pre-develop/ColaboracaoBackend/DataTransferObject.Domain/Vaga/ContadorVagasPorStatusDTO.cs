using System.Collections.Generic;

namespace DataTransferObject.Domain.Vaga
{
    public class ContadorVagasPorStatusDTO
    {
        public int CodigoStatus { get; set; }
        public string DescricaoStatus { get; set; }
        public int Quantidade { get; set; }
    }

    public class ContadorVagasPorStatusResultDTO
    {
        public IEnumerable<ContadorVagasPorStatusDTO> ContadoresPorStatus { get; set; }
        public int TotalGeral { get; set; }

        public ContadorVagasPorStatusResultDTO()
        {
            ContadoresPorStatus = new List<ContadorVagasPorStatusDTO>();
        }
    }
}
