using System;

namespace DataTransferObject.Domain.Social
{
    /// <summary>
    /// Filtros para o endpoint de gestor de equipe que lista feedbacks recebidos por um colaborador.
    /// Todos os campos são opcionais; quando não informados, aplica-se o período padrão de 6 meses e sem filtro de avaliação.
    /// </summary>
    public class FiltroFeedbackGestorDTO
    {
        /// <summary>Código interno (GUID) do colaborador cujos feedbacks recebidos serão listados.</summary>
        public Guid CodigoInternoColaborador { get; set; }

        /// <summary>Data de início do período. Quando não informada, o padrão é 6 meses atrás.</summary>
        public DateTime? DataInicio { get; set; }

        /// <summary>Data de fim do período. Quando não informada, o padrão é a data atual.</summary>
        public DateTime? DataFim { get; set; }

        /// <summary>Filtra pelo id da avaliação (1 a 5). Quando não informado, retorna todas as avaliações.</summary>
        public int? AvaliacaoId { get; set; }
    }
}
