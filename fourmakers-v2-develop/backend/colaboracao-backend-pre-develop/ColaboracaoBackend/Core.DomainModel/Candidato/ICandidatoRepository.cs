using DataTransferObject.Domain.Candidato;
using System;
using System.Threading.Tasks;

namespace Core.Domain.Candidato
{
    public interface ICandidatoRepository
    {
        Task<CandidatoDTO> BuscarCandidatoPorCpf(string cpf);
        Task<int> BuscarEstagioProcessoSeletivoId(int faseId);
        Task<long> SaveCandidato(string cpf, double? pretensaoSalarial, DateTime dataEstagioProcesso, int estagioProcessoSeletivoId, string pathCurriculo);
    }
}