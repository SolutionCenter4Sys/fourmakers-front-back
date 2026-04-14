using Competencia.Domain.Interfaces.Factorys;
using DataTransferObject.Domain.Interesse;
using System.Collections.Generic;

namespace Competencia.Domain.Interfaces.Models
{
    public interface IInteresseColaboradorModel
    {
        InteresseColaboradorDTO InteresseColaboradorDTO { get; set; }
        long InteresseId { get; set; }
        string ColaboradorCpf { get; set; }
        int TipoId { get; set; }
        int SkillId { get; set; }
        bool InteresseAtivo { get; set; }
        IInteresseColaboradorModel SaveModel();

        IInteresseColaboradorModel SaveModelDapper();

        void RemoverInteresseDoColaborador();
        List<IInteresseColaboradorModel> ListarInteressesDeColaboradores(IInteresseDomainFactory factory);
    }
}