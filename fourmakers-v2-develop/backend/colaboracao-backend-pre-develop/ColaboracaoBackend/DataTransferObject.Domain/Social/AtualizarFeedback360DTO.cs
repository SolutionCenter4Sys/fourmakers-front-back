using System;

namespace DataTransferObject.Domain.Social
{
    /// <summary>
    /// Request para atualizar um feedback 360 (apenas na primeira 1h) (estrutura STAR).
    /// </summary>
    public class AtualizarFeedback360DTO
    {
        /// <summary>Situação em que ocorreu a interação. Ex: "Durante a reunião de retrospectiva em 05/03/2026."</summary>
        public string Situacao { get; set; }
        /// <summary>Tarefa/objetivo da interação. Ex: "Alinhar informações e responder dúvidas."</summary>
        public string Tarefa { get; set; }
        /// <summary>Ação realizada pelo colaborador. Ex: "Você conduziu a reunião com clareza e cuidado..."</summary>
        public string Acao { get; set; }
        /// <summary>Resultado gerado pela ação. Ex: "Facilitou a tomada de decisão."</summary>
        public string Resultado { get; set; }
        /// <summary>Prévia enviada pelo front (ex.: texto gerado/ajustado por IA). Opcional.</summary>
        public string Previa { get; set; }
        public DateTime DataInteracao { get; set; }
        public int Feedback360RelacionamentoId { get; set; }
        /// <summary>Quando o relacionamento for outro: especificação livre. Obrigatório nesse caso; máx. 60 caracteres.</summary>
        public string RelacionamentoOutroEspecificacao { get; set; }
        /// <summary>Id da avaliação (1 a 5). Opcional; pode ser null.</summary>
        public int? Feedback360AvaliacaoId { get; set; }
    }
}
