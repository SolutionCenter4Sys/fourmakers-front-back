using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Social
{
    /// <summary>
    /// Registro de um feedback 360 enviado (estrutura STAR).
    /// </summary>
    public class Feedback360DTO
    {
        public Guid Id { get; set; }
        public Guid CodigoInternoColaboradorRemetente { get; set; }
        /// <summary>Nome do colaborador remetente (tb_colaborador.nome_completo).</summary>
        public string NomeRemetente { get; set; }

        /// <summary>Path relativo em tb_imagem (uso interno após SELECT; não serializar).</summary>
        [JsonIgnore]
        public string RemetenteImagemPathRelativa { get; set; }

        [JsonPropertyName("remetenteUrlFoto")]
        public string RemetenteUrlFoto { get; set; }

        [JsonPropertyName("remetenteUrlFotoThumb")]
        public string RemetenteUrlFotoThumb { get; set; }

        [JsonPropertyName("remetenteUrlFotoThumbMini")]
        public string RemetenteUrlFotoThumbMini { get; set; }

        [JsonPropertyName("remetenteUrlFotoThumbVeryMini")]
        public string RemetenteUrlFotoThumbVeryMini { get; set; }

        /// <summary>Todos os destinatários deste feedback (tb_feedback360_destinatario).</summary>
        public List<Feedback360DestinatarioItemDTO> Destinatarios { get; set; } = new List<Feedback360DestinatarioItemDTO>();
        public int OrgId { get; set; }
        public string Situacao { get; set; }
        public string Tarefa { get; set; }
        public string Acao { get; set; }
        public string Resultado { get; set; }
        /// <summary>Prévia gerada por IA a partir do prompt do usuário (preenchida por endpoint futuro).</summary>
        public string Previa { get; set; }
        public DateTime DataInteracao { get; set; }
        public int Feedback360RelacionamentoId { get; set; }
        public string RelacionamentoDescricao { get; set; }
        /// <summary>Texto quando relacionamento é Outro (tb_feedback360.relacionamento_outro).</summary>
        public string RelacionamentoOutroEspecificacao { get; set; }
        /// <summary>Id da avaliação (1 a 5). Opcional; pode ser null.</summary>
        public int? Feedback360AvaliacaoId { get; set; }
        public string AvaliacaoDescricao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public bool Editado { get; set; }
        /// <summary>True se ainda está na primeira hora após a criação e pode ser editado.</summary>
        public bool Editavel { get; set; }
    }
}
