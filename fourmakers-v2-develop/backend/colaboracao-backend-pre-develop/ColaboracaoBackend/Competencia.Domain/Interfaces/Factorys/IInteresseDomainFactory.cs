using Competencia.Domain.Interfaces.Models;
using System;

namespace Competencia.Domain.Interfaces.Factorys
{
    public interface IInteresseDomainFactory
    {
        IInteresseModel buildInteresseModel(long id, string descricao, long usuarioCriacaoId);
        IInteresseModel buildInteresseModel(long usuarioCriacaoId);
        IInteresseModel buildInteresseModel();
        IInteresseColaboradorModel buildInteresseColaboradorModel(long idInteresseColaborador, long interesseId, string colaboradorCpf, DateTime dataAlteracao);
        IInteresseColaboradorModel buildInteresseColaboradorModel();
    }
}