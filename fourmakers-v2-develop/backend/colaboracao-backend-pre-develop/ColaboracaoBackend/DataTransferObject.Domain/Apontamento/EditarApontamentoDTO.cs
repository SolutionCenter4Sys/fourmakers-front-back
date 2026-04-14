using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Apontamento
{
    public class EditarApontamentoDTO
    {
        public string ApontamentoId { get; set; }
        public string ProjetoId { get; set; }
        public string AtividadeId { get; set; }
        public long Horas { get; set; }
        public string DataRegistro { get; set; }
        public string Observacao { get; set; }
        public DateTime DataColetaDeDados { get; set; }
    }
}