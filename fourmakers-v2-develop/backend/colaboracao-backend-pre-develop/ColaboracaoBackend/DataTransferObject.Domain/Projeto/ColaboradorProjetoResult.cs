using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Projeto
{
    public class ColaboradorProjetoResult : StatusResult
    {
        [JsonPropertyName("colaboradorProjeto")]
        public List<ColaboradorProjetoDTO> ColaboradorProjeto { get; set; }
    }
}