using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    /// <summary>Parâmetros internos para consultas de métricas PDI (join org + diretoria + datas + status).</summary>
    public class PdiMetricasQueryDTO
    {
        public int OrgId { get; set; }
        /// <summary>Quando informado, restringe a colaboradores dessas diretorias (unidades).</summary>
        public IReadOnlyList<string> CodDiretorias { get; set; }
        /// <summary>Quando informado, restringe a estes códigos internos (após cruzar com diretoria).</summary>
        public IReadOnlyList<string> CodigosColaborador { get; set; }
        public DateTime? DataCriacaoMin { get; set; }
        /// <summary>Filtra PDIs com dead_line &lt;= esta data (quando informada). dead_line nulo não entra no filtro.</summary>
        public DateTime? DataDeadlineMax { get; set; }
        /// <summary>Status para a listagem; vazio ou null = todos.</summary>
        public IReadOnlyList<string> StatusesListagem { get; set; }

        /// <summary>OFFSET na listagem (0-based). Usado com <see cref="Take"/>.</summary>
        public int? Skip { get; set; }

        /// <summary>LIMIT na listagem.</summary>
        public int? Take { get; set; }
    }
}
