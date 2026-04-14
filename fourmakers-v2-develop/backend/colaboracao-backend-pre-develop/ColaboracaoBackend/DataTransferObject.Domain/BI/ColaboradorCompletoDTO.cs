using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Dominio;
using DataTransferObject.Domain.Escolaridade;
using DataTransferObject.Domain.Experiencia;
using DataTransferObject.Domain.Idioma;
using DataTransferObject.Domain.Metodologia;
using DataTransferObject.Domain.Softskill;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BI
{
    public class ColaboradorCompletoDTO
    {
        [JsonPropertyName("colaborador")]
        public ColaboradorBIDTO Colaborador { get; set; }
        [JsonPropertyName("listaOrgColaborador")]
        public List<ListaOrgColaboradorDTO> ListaOrgColaborador { get; set; }
        [JsonPropertyName("competencias")]
        public List<CompetenciaColaboradorDTO> Competencias { get; set; }
        [JsonPropertyName("softskills")]
        public List<SoftskillColaboradorDTO> Softskills { get; set; }
        [JsonPropertyName("metodologias")]
        public List<ListaMetodologiaColaboradorResult> Metodologias { get; set; }
        [JsonPropertyName("dominiosNegocio")]
        public List<DominioColaboradorDTO> Dominios { get; set; }
        [JsonPropertyName("idiomas")]
        public List<IdiomaColaboradorDTO> Idiomas { get; set; }
        [JsonPropertyName("experiencias")]
        public List<ListaExperienciaDTO> Experiencias { get; set; }
        [JsonPropertyName("escolaridades")]
        public List<EscolaridadeDTO> Escolaridade { get; set; }
    }
}