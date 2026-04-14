using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;

namespace DataTransferObject.Domain.Usuario
{
    public class PrimeiroAcessoResult : StatusResult
    {
        public PrimeiroAcessoResult()
        {
            this.colaborador = new ColaboradorDTO();
        }

        public ColaboradorDTO colaborador { get; set; }
    }
}