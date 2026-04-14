using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Experiencia
{
    public class ListaExperienciaProfissionalResult : StatusResult
    {
        [JsonPropertyName("experienciaEmpresas")]
        public List<ExperienciaEmpresaDTO> ExperienciaEmpresas { get; set; }
    }
}