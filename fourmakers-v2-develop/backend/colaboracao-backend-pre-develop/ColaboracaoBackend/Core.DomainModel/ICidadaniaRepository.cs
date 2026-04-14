using DataTransferObject.Domain.Colaborador.Cidadania;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.DomainModel
{
    public interface ICidadaniaRepository
    {
        Task<List<CidadaniaDTO>> ListarCidadanias();

        Task<List<CidadaniaStatusDTO>> ListarStatusCidadania();

        Task<CidadaniaColaboradorDTO> InserirCidadaniaColaborador(int cidadaniaId, int cidadaniaStatusId, string codigoInternoColaborador);

        Task<CidadaniaColaboradorDTO> BuscarCidadaniaColaboradorPorId(int cidadaniaColaboradorId);
        Task<List<CidadaniaColaboradorDTO>> ListarCidadaniasColaborador(string codigoInternoColaborador);
        Task<bool> VerificaSeExisteCidadaniaJaCadastradaParaColaborador(int cidadaniaId, string codigoInternoColaborador);

        Task<CidadaniaColaboradorDTO> AtualizarCidadaniaColaborador(int statusId, int id);

        Task<bool> RemoverCidadaniaColaborador(int id);
    }
}