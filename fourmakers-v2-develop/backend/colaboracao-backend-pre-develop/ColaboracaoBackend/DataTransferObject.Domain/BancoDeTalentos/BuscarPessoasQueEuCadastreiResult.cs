using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.BancoDeTalentos
{
    public class BuscarPessoasQueEuCadastrei
    {
        public string Nome { get; set; }
        public string CodigoInternoColaborador { get; set; }
        
        public string NomeCadastrante { get; set; }
        
        public DateTime DataDoCadastro { get; set; }
        public bool PossuiCandidatura { get; set; }
        public List<CandidaturaComTituloDTO> Candidaturas { get; set; }
        public double Match { get; set; }
    }
}