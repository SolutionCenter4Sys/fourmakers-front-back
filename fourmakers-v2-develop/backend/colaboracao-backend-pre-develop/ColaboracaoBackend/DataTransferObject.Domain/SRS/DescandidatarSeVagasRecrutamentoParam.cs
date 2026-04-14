using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class DescandidatarSeVagasRecrutamentoParam
    {
        public string idCandidatura { get; set; }
        public string idMotivoDescandidatura { get; set; }
    }
}