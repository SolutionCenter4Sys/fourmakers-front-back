using System.Collections.Generic;

namespace DataTransferObject.Domain.Social
{
    /// <summary>
    /// Resultado do endpoint ListarRecebidos: quantidade total e lista de feedbacks.
    /// </summary>
    public class Feedback360RecebidosResultadoDTO
    {
        /// <summary>Número total de feedbacks recebidos com os filtros aplicados.</summary>
        public int TotalFeedbacks { get; set; }

        /// <summary>Lista detalhada dos feedbacks recebidos.</summary>
        public IEnumerable<Feedback360DTO> ListaFeedbacks { get; set; }
    }
}
