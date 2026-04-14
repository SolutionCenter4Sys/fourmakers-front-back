using System.Collections.Generic;

namespace DataTransferObject.Domain.Formacao
{
    public class FormacaoSumarioDTO

    {
        public string NivelEscolaridade { get; set; }
        public int QtdUsuarios { get; set; }
        public List<EscolaridadeSumarioDTO> Cursos { get; set; }
    }

    public class EscolaridadeSumarioDTO
    {
        public string NivelEscolaridade { get; set; }
        public long CdFormacao { get; set; }
        public string DescricaoFormacao { get; set; }
        public int QtdUsuarios { get; set; }
    }
}