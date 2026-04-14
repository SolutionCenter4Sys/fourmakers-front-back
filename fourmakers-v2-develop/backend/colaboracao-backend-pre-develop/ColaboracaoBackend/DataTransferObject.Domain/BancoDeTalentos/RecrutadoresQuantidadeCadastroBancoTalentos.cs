using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.BancoDeTalentos
{
    public class RecrutadoresQuantidadeCadastroBancoTalentos
    {
        public string CodigoInternoColaborador { get; set; }
        public string NomeColaborador { get; set; }
        public int TotalTalentosCadastrados { get; set; }
    }
}
