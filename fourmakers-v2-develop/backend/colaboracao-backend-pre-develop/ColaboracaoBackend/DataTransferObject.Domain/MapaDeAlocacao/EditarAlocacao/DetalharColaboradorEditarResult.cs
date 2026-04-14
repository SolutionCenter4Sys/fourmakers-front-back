using DataTransferObject.Domain.Base;

namespace DataTransferObject.Domain.MapaDeAlocacao.DetalharColaborador
{
    public class DetalharColaboradorEditarResult : StatusResult
    {
        public DetalharColaboradorEditarDTO DetalharColaborador { get; set; }
    }
}