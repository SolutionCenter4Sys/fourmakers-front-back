using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using System.Threading.Tasks;

namespace MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestorExternoPerfil
{
    public interface IGestorExternoPerfilValidatorService
    {
        Task ValidaGestorExternoPerfil(GestorExternoPerfilInput gestorExternoPerfilInput, CRUDEnum create);
    }
}