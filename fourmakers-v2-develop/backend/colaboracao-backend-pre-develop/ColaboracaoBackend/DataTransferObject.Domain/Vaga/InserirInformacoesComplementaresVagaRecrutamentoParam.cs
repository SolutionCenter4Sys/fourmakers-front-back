using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Vaga
{
    public class InserirInformacoesComplementaresVagaRecrutamentoParam
    {
        public string IdVaga { get; set; }
        public string ColaboradorCodigoInternoColaboradorGestorOrgLogada { get; set; }
        public string PropostaCrm { get; set; }
        public string? TipoVagaId { get; set; }
        public int? TipoContratacaoId { get; set; }
        public string UnidadeId { get; set; }
        public List<string> CodColaboradoresEntrevistadores { get; set; }
        public int NumeroDeVagas { get; set; }
        public List<string> EmailsAnaliseGestor { get; set; }
        public string RecrutadorVaga { get; set; }
        public string MaquinaColaborador { get; set; }
        public string ObservacoesInternas { get; set; }
    }
} 