using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.SolicitacaoParticipacao
{
    public class SolicitacaoResponse
    {
        public string AgendaId { get; set; }
        public string CodigoInternoColaboradorCriador { get; set; }
        public string CodigoInternoColaboradorSolicitante { get; set; }
        public string NomeColaboradorCriador { get; set; }
        public string NomeColaboradorSolicitante { get; set; }
        public StatusSolicitacaoParticipante Status { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public DateTime? DataDecisao { get; set; }
    }
}
