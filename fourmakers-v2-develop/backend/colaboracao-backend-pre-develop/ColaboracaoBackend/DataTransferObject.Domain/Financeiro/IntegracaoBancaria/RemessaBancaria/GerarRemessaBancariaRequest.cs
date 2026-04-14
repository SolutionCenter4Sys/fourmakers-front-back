using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria
{
    public class GerarRemessaBancariaRequest
    {
        public string TipoRemessa { get; set; }
        public string CodDiretoria { get; set; }
        public List<string> SolicitacoesPagamentoIds { get; set; }
    }
}
