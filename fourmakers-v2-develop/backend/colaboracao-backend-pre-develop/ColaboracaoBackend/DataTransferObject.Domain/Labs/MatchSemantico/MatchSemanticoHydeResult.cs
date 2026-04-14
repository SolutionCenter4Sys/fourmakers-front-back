using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Labs.MatchSemantico
{
    /// <summary>
    /// Resultado da chamada ao endpoint best_candidates/hyde do Match Semântico (GCP).
    /// Inclui a lista tipada em <see cref="Result"/> conforme o JSON do serviço.
    /// </summary>
    public class MatchSemanticoHydeResult
    {
        public bool Sucesso { get; set; }
        public object Resposta { get; set; }
        public string HttpStatus { get; set; }
        public string Mensagem { get; set; }

        /// <summary>
        /// Id (GUID) do registro de log gravado em tb_labs_log_match_semantico. Retornado para o front.
        /// </summary>
        [JsonPropertyName("idLogMatchSemantico")]
        public Guid? IdLogMatchSemantico { get; set; }
    }
}
