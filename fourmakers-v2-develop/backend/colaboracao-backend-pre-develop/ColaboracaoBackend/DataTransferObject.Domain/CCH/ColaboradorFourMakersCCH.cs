using System;

namespace DataTransferObject.Domain.CCH
{
    public class ColaboradorFourMakersCCH
    {
        public int cdProfissional { get; set; }
        public string nmProfissional { get; set; }
        public string cdCpf { get; set; }
        public bool flFuncionarioAtivo { get; set; }
        public DateTime? dtNascimento { get; set; }
        public DateTime? dtDesligamento { get; set; }
        public string? nmTipoContratacao { get; set; }
    }
}