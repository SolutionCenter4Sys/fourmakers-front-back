using System.Collections.Generic;

namespace DataTransferObject.Domain.Formacao
{
    public class LocalizacaoSumarioDTO

    {
        public string Estado { get; set; }
        public int QtdUsuarios { get; set; }
        public List<CidadesSumarioDTO> Cidades { get; set; }
    }

    public class CidadesSumarioDTO
    {
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public int QtdUsuarios { get; set; }
    }
}