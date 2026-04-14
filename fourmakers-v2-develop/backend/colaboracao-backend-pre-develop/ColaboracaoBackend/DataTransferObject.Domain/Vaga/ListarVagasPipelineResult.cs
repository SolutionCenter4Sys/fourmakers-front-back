using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Vaga
{
    public class ListarVagasPipelineResult
    {
        public int VagaId { get; set; }
        public string Cliente { get; set; }
        public string TituloVaga { get; set; }
        public string GestorExterno { get; set; }
        public string CriadorVaga { get; set; }
        public DateTime Criacao { get; set; }
        public int Posicoes { get; set; }
    }
}
