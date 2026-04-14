using DataTransferObject.Domain.Vaga;
using System.Collections.Generic;

namespace RotinasBackoffice.Domain.Interfaces.Services
{
    public interface IVagasSRSServices
    {
        void InserirVagas(List<VagaFourmakersSRSDTO> param);
        IEnumerable<VagaFourmakersSRSDTO> ObterTodasAsVagas();
        void AtualizarVagas(List<VagaFourmakersSRSDTO> param);
    }
}