using Competencia.Domain.Interfaces.Models;
using System;

namespace Competencia.Domain.Interfaces.Factorys
{
    public interface IHobbyDomainFactory
    {
        IHobbyModel buildHobbyModel(long id, string descricao, long usuarioCriacaoId);
        IHobbyModel buildHobbyModel(long usuarioCriacaoId);
        IHobbyModel buildHobbyModel();
        IHobbyColaboradorModel buildHobbyColaboradorModel(long idHobbyColaborador, long hobbyId, string colaboradorCpf, DateTime dataAlteracao);
        IHobbyColaboradorModel buildHobbyColaboradorModel();
    }
}