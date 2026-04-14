using DataTransferObject.Domain.Base;

namespace DataTransferObject.Domain.Usuario
{
    public class PrestadorServicoResult : StatusResult
    {
        public PrestadorServicoDTO prestadorServico { get; set; }
        //public bool primeiroAcesso { get; set; }
        //public string token { get; set; }
    }
}