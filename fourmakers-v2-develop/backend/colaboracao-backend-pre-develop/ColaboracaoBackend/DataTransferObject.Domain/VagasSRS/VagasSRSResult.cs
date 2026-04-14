using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Vaga;
using System.Collections.Generic;

namespace DataTransferObject.Domain.VagasSRS
{
    public class VagasSRSResult : StatusResult
    {
        public List<VagaFourmakersSRSDTO> VagasSRS { get; set; }
    }
}