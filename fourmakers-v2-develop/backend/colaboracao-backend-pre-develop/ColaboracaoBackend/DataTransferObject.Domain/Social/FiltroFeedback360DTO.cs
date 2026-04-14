using System;

namespace DataTransferObject.Domain.Social
{
    /// <summary>
    /// Filtros opcionais para ListarRecebidos e ListarEnviados.
    /// Quando não informados, nenhum filtro é aplicado.
    /// </summary>
    public class FiltroFeedback360DTO
    {
        /// <summary>Data de início do período (filtra por data_criacao).</summary>
        public DateTime? DataInicio { get; set; }

        /// <summary>Data de fim do período (filtra por data_criacao).</summary>
        public DateTime? DataFim { get; set; }

        /// <summary>Id da avaliação/sentimento (1 a 5). Quando não informado, retorna todos.</summary>
        public int? SentimentoId { get; set; }

        /// <summary>Id do tipo de relacionamento. Quando não informado, retorna todos.</summary>
        public int? RelacionamentoId { get; set; }

        /// <summary>Quantidade máxima de registros a retornar (LIMIT). Padrão: 10.</summary>
        public int Limit { get; set; } = 10;

        /// <summary>Deslocamento inicial dos registros (OFFSET). Padrão: 0.</summary>
        public int Cursor { get; set; } = 0;

        /// <summary>Texto de busca (mural): filtra por nome do destinatário, situação, ação ou prévia. Opcional.</summary>
        public string Busca { get; set; }
    }
}
