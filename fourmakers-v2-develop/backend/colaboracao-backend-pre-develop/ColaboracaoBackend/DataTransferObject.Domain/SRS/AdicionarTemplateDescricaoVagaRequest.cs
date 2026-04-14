using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class AdicionarTemplateDescricaoVagaRequest
    {
        public string TextoIntroducao { get; set; }
        public string TextoFinalizacao { get; set; }
    }
}