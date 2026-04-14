using DataTransferObject.Domain.Softskill;
using System.Collections.Generic;

namespace Competencia.Domain.Interfaces.Services
{
    public interface ISoftskillColaboradorRepository
    {
        SoftskillColaboradorDTO AddSoftskillColaborador(SoftskillColaboradorDTO softskillColaboradorDTO);
        List<SoftskillColaboradorDTO> ListarSoftskillColaborador(string cpf);
        void RemoveSoftskillColaborador(SoftskillColaboradorDTO softskillColaboradorDTO);
        public SoftskillColaboradorDTO AlterarSoftskillColaborador(SoftskillColaboradorDTO softskillColaboradorDTO);
    }
}