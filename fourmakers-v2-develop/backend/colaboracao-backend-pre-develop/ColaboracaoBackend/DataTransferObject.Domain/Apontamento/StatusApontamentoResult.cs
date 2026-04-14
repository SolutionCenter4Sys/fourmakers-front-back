using System;

namespace DataTransferObject.Domain.Apontamento
{
    public class StatusApontamentoResult
    {
        public Guid id_status_apontamento { get; set; }
        public string descricao_apontamento { get; set; }
        public int cod_status_apontamento { get; set; }
        public int cod_status_grupo { get; set; }
        public Guid id_status_apontamento_grupo { get; set; }
        public string descricao_grupo { get; set; }
    }
}