using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Classificacao.Candidato;

public class CandidatoClassificacaoDTO
{
    [JsonPropertyName("codigoInternoColaborador")]
    public string CodigoInternoColaborador { get; set; }

    [JsonPropertyName("nomeCompleto")]
    public string NomeCompleto { get; set; }

    [JsonPropertyName("experiencias")]
    public List<CandidatoClassificacaoExperienciaDTO> Experiencias { get; set; }

    [JsonPropertyName("hardSkills")]
    public List<CandidatoSkillClassificacaoDTO> HardSkills { get; set; }

    [JsonPropertyName("sobre")]
    public string Sobre { get; set; }
}