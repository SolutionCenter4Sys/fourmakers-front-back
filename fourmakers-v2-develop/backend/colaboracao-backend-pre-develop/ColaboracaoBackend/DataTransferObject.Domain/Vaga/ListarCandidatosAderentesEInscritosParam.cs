using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Domain.Vaga
{
    public class ListarCandidatosAderentesEInscritosParam
    {
        [Required]
        public string VagaId { get; set; }

        [Required]
        public int Cursor { get; set; }

        [Required]
        public int Limite { get; set; }

        public string Busca { get; set; }

        public string DataInicio { get; set; }

        public string DataFim { get; set; }

        public List<string> Origens { get; set; }
        public double PesoHardSkills { get; set; }
        public double PesoSoftSkills { get; set; }
        public double PesoMetodologias { get; set; }
        public double PesoDominiosNegocio { get; set; }
        public double PesoIdiomas { get; set; }
        public double PesoDisponibilidades { get; set; }
        public string? LocalizacaoCidade { get; set; }
        public string? LocalizacaoEstado { get; set; }
    }
}
