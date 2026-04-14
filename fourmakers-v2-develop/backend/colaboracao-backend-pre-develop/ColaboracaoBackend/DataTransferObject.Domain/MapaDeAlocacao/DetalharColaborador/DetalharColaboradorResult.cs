using DataTransferObject.Domain.Base;

namespace DataTransferObject.Domain.MapaDeAlocacao.DetalharColaborador
{
    public class DetalharColaboradorResult : StatusResult
    {
        public DetalharColaboradorDTO DetalharColaborador { get; set; }
    }
}