using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Domain.Vaga
{
    public class ListarVagasRecrutamentoEPerfisParam
    {
        [Required]
        public int Limite { get; set; } = 10;

        [Required]
        public int Cursor { get; set; } = 0;

        public string Busca { get; set; }

        public List<int> Status { get; set; }

        public string DataInicio { get; set; }

        public string DataFim { get; set; }

        public string Cliente { get; set; }
    }
}
