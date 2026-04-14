using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.Vagas
{
    public class MudarStatusVagaRecrutamentoParam
    {
        public int CodigoVaga { get; set; }
        public int CodigoStatus { get; set; }
        public string ComentarioVaga { get; set; }
    }
}