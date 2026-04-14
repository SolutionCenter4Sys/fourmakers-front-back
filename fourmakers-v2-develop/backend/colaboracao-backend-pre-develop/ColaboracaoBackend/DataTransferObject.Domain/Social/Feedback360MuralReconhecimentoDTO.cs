using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Social
{
    /// <summary>
    /// Colaborador resumido (id e nome) para exibição em cards do mural de reconhecimento.
    /// URLs de foto seguem a mesma convenção de ColaboradorDTO no ShowMe (tb_imagem.path + SERVICE_MIDIA).
    /// </summary>
    public class Feedback360ColaboradorCardDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }

        [JsonPropertyName("urlFoto")]
        public string UrlFoto { get; set; }

        [JsonPropertyName("urlFotoThumb")]
        public string UrlFotoThumb { get; set; }

        [JsonPropertyName("urlFotoThumbMini")]
        public string UrlFotoThumbMini { get; set; }

        [JsonPropertyName("urlFotoThumbVeryMini")]
        public string UrlFotoThumbVeryMini { get; set; }
    }

    /// <summary>
    /// Item do mural de reconhecimento para exibição em card no front (feedbacks da organização).
    /// Inclui reações (emojis) com contagem e se o usuário logado já reagiu.
    /// </summary>
    public class Feedback360MuralReconhecimentoDTO
    {
        public Guid Id { get; set; }
        public Feedback360ColaboradorCardDTO ColaboradorRemetente { get; set; }
        /// <summary>Todos os destinatários exibidos no card do mural.</summary>
        public List<Feedback360ColaboradorCardDTO> ColaboradoresDestinatarios { get; set; } = new List<Feedback360ColaboradorCardDTO>();
        public DateTime DataCriacao { get; set; }
        public DateTime DataInteracao { get; set; }
        public string Situacao { get; set; }
        public string Tarefa { get; set; }
        public string Acao { get; set; }
        public string Resultado { get; set; }
        public string Previa { get; set; }
        public string Relacionamento { get; set; }
        /// <summary>Lista de reações (emoji + quantidade) para exibir no card; inclui se o usuário logado já reagiu.</summary>
        public List<Feedback360MuralReacaoCardDTO> Reacoes { get; set; } = new List<Feedback360MuralReacaoCardDTO>();
    }
}
