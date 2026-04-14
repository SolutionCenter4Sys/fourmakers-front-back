using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Domain.BancoDeTalentos
{
    public class BuscarPessoasCadastradasPorOrgInput
    {
        [Required]
        public int Limite { get; set; } = 10;

        [Required]
        public int Cursor { get; set; } = 0;

        public string Busca { get; set; }

        public List<int> StatusVaga { get; set; }

        public List<int> StatusCandidatura { get; set; }

        public string DataInicio { get; set; }

        public string DataFim { get; set; }
    }
}

