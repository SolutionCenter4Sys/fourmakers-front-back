using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace Colaborador.API.DTOs
{
    public class FormularioColaboradorResult : StatusResult
    {
        [JsonPropertyName("formulario")]
        public FormularioColaborador Formulario { get; set; }
    }
}