using Microsoft.AspNetCore.Http;

namespace DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RetornoBancaria
{
    public class ProcessarRetornoCnabRequest
    {
        public string HashRemessa { get; set; }
        public IFormFile ArquivoRetorno { get; set; }
    }
}
