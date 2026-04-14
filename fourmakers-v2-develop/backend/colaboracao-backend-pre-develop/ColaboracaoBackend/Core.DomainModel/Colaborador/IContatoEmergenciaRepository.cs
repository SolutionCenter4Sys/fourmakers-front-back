using DataTransferObject.Domain.Colaborador;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.DomainModel.Colaborador
{
    public interface IContatoEmergenciaRepository
    {
        void InsereContatoEmergencia(ContatoEmergenciaDTO contatoEmergencia, string cpf);
        void AlteraContatoEmergencia(ContatoEmergenciaDTO contatoEmergencia);
        void DeletaContatoEmergencia(ContatoEmergenciaDTO contatoEmergencia);
        Task<List<ContatoEmergenciaDTO>> ListaContatoEmergencia(string cpf);
    }
}