using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.IntegracaoContabil
{
    public class RemessaContabilDTO
    {
        public List<RemessaContabilRegistroOutrosDTO> Dados { get; set; }
        public List<RemessaContabilRegistroReembolsoDTO> DadosReembolso { get; set; }
        public RemessaContabilIdsDTO Ids { get; set; }
    }

    public class RemessaContabilIdsDTO
    {
        public List<string> FolhaPonto { get; set; }
        public List<string> Reembolso { get; set; }
        public List<string> Rubrica { get; set; }
    }
}