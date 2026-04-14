using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Social
{
    /// <summary>
    /// Request para enviar um novo feedback 360 (estrutura STAR).
    /// </summary>
    public class EnviarFeedback360DTO
    {
        /// <summary>Destinatários (<c>codigo_interno_colaborador</c>, GUID). Obrigatório: ao menos um.</summary>
        public List<Guid> CodigosInternosColaboradoresDestinatarios { get; set; }
        /// <summary>Situação em que ocorreu a interação (obrigatório). Ex: "Durante a reunião de retrospectiva em 05/03/2026."</summary>
        public string Situacao { get; set; }
        /// <summary>Tarefa/objetivo da interação (obrigatório). Ex: "Alinhar informações e responder dúvidas."</summary>
        public string Tarefa { get; set; }
        /// <summary>Ação realizada pelo colaborador (obrigatório). Ex: "Você conduziu a reunião com clareza e cuidado..."</summary>
        public string Acao { get; set; }
        /// <summary>Resultado gerado pela ação (obrigatório). Ex: "Facilitou a tomada de decisão."</summary>
        public string Resultado { get; set; }
        /// <summary>Prévia enviada pelo front (ex.: texto gerado/ajustado por IA). Opcional.</summary>
        public string Previa { get; set; }
        /// <summary>Data da interação (hoje ou passado).</summary>
        public DateTime DataInteracao { get; set; }
        /// <summary>Id do nível de relacionamento (tb_feedback360_relacionamento).</summary>
        public int Feedback360RelacionamentoId { get; set; }
        /// <summary>Quando o relacionamento for outro;: especificação livre (ex.: cliente, gestor). Obrigatório nesse caso; máx. 60 caracteres.</summary>
        public string RelacionamentoOutroEspecificacao { get; set; }
        /// <summary>Id da avaliação (tb_feedback360_avaliacoes). Opcional; pode ser null.</summary>
        public int? Feedback360AvaliacaoId { get; set; }
    }
}
