using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.Vagas
{
    public class MudarStatusCandidaturaRecrutamentoParam
    {
        public string IdCandidatura { get; set; }
        public int CodigoStatus { get; set; }
        public string Comentario { get; set; }
    }
}