using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Domain.Candidatura
{
    public class EnviarEmailTemplateCandidatoParam
    {
        [Required]
        public string IdCandidatura { get; set; }

        public List<string> EmailsAdicionais { get; set; }

        public bool Anexo { get; set; } = false;

        public bool OcultarValores { get; set; } = false;
    }
}
