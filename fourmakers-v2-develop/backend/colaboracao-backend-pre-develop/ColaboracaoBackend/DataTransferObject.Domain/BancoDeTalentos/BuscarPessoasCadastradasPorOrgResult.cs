using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.BancoDeTalentos
{
    public class BuscarPessoasCadastradasPorOrgResult
    {
        public string Nome { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string NomeCadastrante { get; set; }
        public DateTime DataDoCadastro { get; set; }
        public string EmailUsuario { get; set; }
    }
}

