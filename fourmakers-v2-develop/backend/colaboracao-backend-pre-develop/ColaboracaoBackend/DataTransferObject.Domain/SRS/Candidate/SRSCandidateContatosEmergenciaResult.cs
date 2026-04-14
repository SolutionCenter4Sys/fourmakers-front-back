using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using System.Collections.Generic;

namespace DataTransferObject.Domain.SRS.Candidate
{
    public class ContatosEmergenciaResult : StatusResult
    {
        public List<ContatoEmergenciaDTO> contatos { get; set; }
    }
}