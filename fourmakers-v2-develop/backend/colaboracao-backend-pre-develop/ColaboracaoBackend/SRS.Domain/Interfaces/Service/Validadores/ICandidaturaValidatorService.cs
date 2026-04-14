using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Vaga;
using System.Threading.Tasks;

namespace SRS.Domain.Interfaces.Service.Validadores
{
    public interface ICandidaturaValidatorService
    {
        Task ValidaCandidaturaStatus(int vagaId, int candidatoId, int statusId, string descricao, OrigemVagaEnum origem, CRUDEnum cRUDEnum);
    }
}