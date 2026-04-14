using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.Aderencia
{
    public class ListaAderenciaParam
    {
        public int OrgId { get; set; }
        public Guid GestorExternoPerfilId { get; set; }
        public string? Filtro { get; set; }
        public int Cursor { get; set; }
        public int Limite { get; set; }
    }

    public class ListasPerfisAderentesParams
    {
        [Required]
        [JsonPropertyName("orgId")]
        public int OrgId { get; set; }

        [Required]
        [JsonPropertyName("codigoInternoColaborador")]
        [Description("Código interno colaborador")]
        public string CodigoInternoColaborador { get; set; }

        [JsonPropertyName("filtro")]
        [Description("Filtragem de resultados")]
        public string? Filtro { get; set; }

        [Required]
        [JsonPropertyName("cursor")]
        public int Cursor { get; set; }

        [Required]
        [JsonPropertyName("limite")]
        [Description("Limite de perfis aderentes à este colaborador")]
        public int Limite { get; set; }
    }
}