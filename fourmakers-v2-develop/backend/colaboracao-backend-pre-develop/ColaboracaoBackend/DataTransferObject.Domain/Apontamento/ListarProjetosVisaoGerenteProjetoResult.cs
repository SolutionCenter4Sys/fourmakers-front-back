using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Apontamento
{
    public class ListaComTotalizadorHorasResult<T>
    {
        public IEnumerable<T> Lista { get; set; }
        public TotalizadorApontamentosDTO Totalizador { get; set; }

        public TotalizadorApontamentosBigNumbersDTO TotalizadorBigNumbers { get; set; }
        public DateTime DataColetaDeDados { get; set; }
    }
}