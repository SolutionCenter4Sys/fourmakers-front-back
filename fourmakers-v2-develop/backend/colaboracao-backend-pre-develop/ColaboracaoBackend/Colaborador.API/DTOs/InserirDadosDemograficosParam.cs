using DataTransferObject.Domain.Colaborador;

namespace Colaborador.API.DTOs
{
    public class InserirDadosDemograficosParam
    {
        public string CodigoInternoColaborador { get; set; }
        public DadosDemograficosColaboradorDTO DadosDemograficos { get; set; }
    }
}
