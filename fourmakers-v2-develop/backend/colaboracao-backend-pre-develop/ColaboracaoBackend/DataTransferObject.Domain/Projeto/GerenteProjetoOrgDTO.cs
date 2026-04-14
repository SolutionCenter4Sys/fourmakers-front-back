using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Projeto
{
    public class GerenteProjetoOrgDTO
    {
        public string CodProjeto { get; set; }
        public string CodigoColaboradorGerente { get; set; }
        public string TipoGerente { get; set; }
        [JsonIgnore]
        public int OrgId { get; set; }
    }
}