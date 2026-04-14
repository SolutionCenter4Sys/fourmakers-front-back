using System.Collections.Generic;

namespace DataTransferObject.Domain.Apontamento
{
    public class AprovarProjetoVigenciaEmLoteResult
    {
        public int QuantidadeProjetosAprovados { get; set; }
        public int QuantidadeProjetosPulados { get; set; }
        public List<MensagemAprovarProjetoVigenciaResult> Mensagens { get; set; }
    }

    public class MensagemAprovarProjetoVigenciaResult
    {
        public string CodProjeto { get; set; }
        public string Cpf { get; set; }
        public bool ProjetoAprovado { get; set; }
        public string Mensagem { get; set; }
    }
}