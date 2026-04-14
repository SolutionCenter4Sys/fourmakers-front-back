#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class forcaperfilcolaborador
    {
        public string cpf_colaborador { get; set; }
        public long ForcaPerfil { get; set; }
        public int PossuiCompetencia { get; set; }
        public int PossuiFormacao { get; set; }
        public int PossuiDominioNegocio { get; set; }
        public int PossuiMetodologia { get; set; }
        public int PossuiModeloReferencia { get; set; }
        public int PossuiInteresse { get; set; }
        public int PossuiHobbie { get; set; }
        public int PossuiEndereco { get; set; }
        public int PossuiFoto { get; set; }
        public int PediuEndosso { get; set; }
        public int ForneceuEndosso { get; set; }
        public int PossuiCertificadoCompetencia { get; set; }
        public int PossuiCertificadoFormacao { get; set; }
        public int PossuiContato { get; set; }
        public int ForneceuLike { get; set; }
    }
}