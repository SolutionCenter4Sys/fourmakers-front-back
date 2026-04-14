using DataTransferObject.Domain.Base;

namespace DataTransferObject.Domain
{
    public class AcessoUsuarioResult : StatusResult
    {
        public TipoAcessoEnum TipoAcesso { get; set; }
    }
}