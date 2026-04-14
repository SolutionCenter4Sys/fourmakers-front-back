using System;

namespace DataTransferObject.Domain.Social
{
    /// <summary>
    /// Request para o endpoint que gera modelo STAR do feedback 360 com IA (Moxe).
    /// A IA organiza os dados em formato STAR e retorna situação, tarefa, ação, resultado e prévia.
    /// </summary>
    public class GerarModeloStarMoxeDTO
    {
        /// <summary>Onde ocorreu a interação (ex: durante a reunião de retrospectiva, presencialmente).</summary>
        public string Situacao { get; set; }

        /// <summary>Data em que ocorreu a interação.</summary>
        public DateTime? DataInteracao { get; set; }

        /// <summary>Contexto completo: o que ocorreu, impacto positivo, ação da pessoa, resultado, etc.</summary>
        public string Contexto { get; set; }
    }
}
