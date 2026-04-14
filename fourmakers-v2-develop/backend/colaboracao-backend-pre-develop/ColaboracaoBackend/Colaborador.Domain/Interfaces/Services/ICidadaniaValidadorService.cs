using System.Threading.Tasks;

namespace Colaborador.Domain.Interfaces.Services
{
    public interface ICidadaniaValidadorService
    {
        Task ValidaSeExisteCidadaniaParaOColaborador(int id, string codigoInternoColaborador);
        Task ValidaSeExisteCidadaniaJaCadastradaParaColaborador(int id, string codigoInternoColaborador);
    }
}