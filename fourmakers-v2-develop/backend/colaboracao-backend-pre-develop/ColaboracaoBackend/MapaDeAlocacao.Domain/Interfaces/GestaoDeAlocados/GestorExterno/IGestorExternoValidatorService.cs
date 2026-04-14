using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Projeto.GestorExterno;
using System.Threading.Tasks;

namespace Projeto.Domain.Interfaces.Services
{
    public interface IGestorExternoValidatorService
    {
        Task ValidaGestorExterno(GestorExternoInput gestorExternoRepositoryInput, CRUDEnum create, string codGestorExternoChave = null);
    }
}