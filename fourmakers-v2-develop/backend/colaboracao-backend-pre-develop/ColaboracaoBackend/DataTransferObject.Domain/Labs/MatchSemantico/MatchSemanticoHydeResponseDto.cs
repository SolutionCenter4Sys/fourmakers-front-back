using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Labs.MatchSemantico
{
    /// <summary>
    /// Estrutura do JSON retornado pelo endpoint best_candidates/hyde (propriedade "result").
    /// </summary>
    public class MatchSemanticoHydeResponseDto
    {
        [JsonPropertyName("result")]
        public List<MatchSemanticoCandidatoHydeDto> Result { get; set; }
    }

    /// <summary>
    /// Candidato retornado no result do Match Semântico Hyde.
    /// </summary>
    public class MatchSemanticoCandidatoHydeDto
    {
        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("sobre")]
        public string Sobre { get; set; }

        [JsonPropertyName("cidade")]
        public string Cidade { get; set; }

        [JsonPropertyName("estado")]
        public string Estado { get; set; }

        [JsonPropertyName("experiencia")]
        public List<MatchSemanticoExperienciaHydeDto> Experiencia { get; set; }

        [JsonPropertyName("categoria")]
        public string Categoria { get; set; }

        [JsonPropertyName("origem")]
        public string Origem { get; set; }

        [JsonPropertyName("is_pula_pula")]
        public bool IsPulaPula { get; set; }

        [JsonPropertyName("parecer_IA_senioridade")]
        public MatchSemanticoParecerIAHydeDto ParecerIASenioridade { get; set; }
    }

    /// <summary>
    /// Item de experiência do candidato (Hyde).
    /// </summary>
    public class MatchSemanticoExperienciaHydeDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; }

        [JsonPropertyName("empresa")]
        public string Empresa { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("dataInicio")]
        public string DataInicio { get; set; }

        [JsonPropertyName("dataSaida")]
        public string DataSaida { get; set; }

        [JsonPropertyName("atual")]
        public bool Atual { get; set; }

        [JsonPropertyName("ativo")]
        public bool Ativo { get; set; }
    }

    /// <summary>
    /// Parecer de IA sobre senioridade do candidato (Hyde).
    /// </summary>
    public class MatchSemanticoParecerIAHydeDto
    {
        [JsonPropertyName("parecer")]
        public bool Parecer { get; set; }

        [JsonPropertyName("motivo_parecer")]
        public List<string> MotivoParecer { get; set; }
    }
}
