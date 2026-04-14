using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Competencia.DashboardMinhaJornada
{
    public class LogDetalhadoDashboardMinhaJornada
    {
        public Guid Id { get; set; }
        public DateTime? DataCriacao { get; set; }
        public string NomeColaborador { get; set; } = string.Empty;
        public string NomeCliente { get; set; } = string.Empty;
        public string NomePerfil { get; set; } = string.Empty;
        public string NomeSkill { get; set; } = string.Empty;
        public string TipoSkill { get; set; } = string.Empty;
        public string Senioridade { get; set; } = string.Empty;
        public string Evento { get; set; } = string.Empty;
    }
}