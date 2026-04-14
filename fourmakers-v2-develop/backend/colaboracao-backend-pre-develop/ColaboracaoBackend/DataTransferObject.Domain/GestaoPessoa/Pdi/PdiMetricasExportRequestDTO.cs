using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    public class PdiMetricasExportRequestDTO
    {
        public List<Guid> PdiIds { get; set; }
        /// <summary>Mesmos filtros da busca para validar escopo dos IDs.</summary>
        public PdiMetricasFiltroRequestDTO Filtro { get; set; }
    }
}
