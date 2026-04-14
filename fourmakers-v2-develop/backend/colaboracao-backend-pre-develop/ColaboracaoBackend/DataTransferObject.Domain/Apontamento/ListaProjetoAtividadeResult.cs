using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Apontamento
{
    public class ListaProjetoAtividadeResult : StatusResult
    {
        [JsonPropertyName("projetos_atividades")]
        public IEnumerable<ProjetoAtividadeDTO> ProjetosAtividades { get; set; }
    }
}