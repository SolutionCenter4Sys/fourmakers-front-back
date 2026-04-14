using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.SRS
{
    public class MeusTalentos
    {
        public string NomeColaborador { get; set; }
        public string CodigoVaga { get; set; }
        public string NomeVaga { get; set; }
        public string NomeCliente { get; set; }
        public string NomeGestor { get; set; }
        public string StatusVaga { get; set; }
        public string StatusMovimentacao { get; set; }
        public DateTime? UltimaAlteracao { get; set; }
        public string CodigoColaboradorUltimaMovimentacao { get; set; }
        public string RecrutadorUltimaMovimentacao { get; set; }
        public string CodColaborador { get; set; }
        public string CriadoPor { get; set; }
    }
}
