using System.Collections.Generic;

namespace DataTransferObject.Domain.Social
{
    /// <summary>
    /// Resultado do endpoint de gestor: quantidade total e lista de feedbacks recebidos.
    /// </summary>
    public class FeedbackGestorResultadoDTO
    {
        /// <summary>Número total de feedbacks recebidos no período com os filtros aplicados.</summary>
        public int TotalFeedbacks { get; set; }

        /// <summary>Lista detalhada dos feedbacks recebidos.</summary>
        public IEnumerable<Feedback360DTO> ListaFeedbacks { get; set; }
    }
}
