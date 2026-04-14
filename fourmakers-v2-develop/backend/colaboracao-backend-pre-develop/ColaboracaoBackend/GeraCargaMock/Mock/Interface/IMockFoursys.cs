using DataTransferObject.Domain.Carga;

namespace RotinasBackoffice.API.Mock.Interface
{
    public interface IMockFoursys
    {
        List<ColaboradorCargaDTO> GeraCargaColaborador();
        List<ColaboradorHierarquiaCargaDTO> GeraCargaHierarquia();
        List<ProjetoCargaDTO> GeraCargaProjeto();
        List<ProjetoGerenteCargaDTO> GeraCargaProjetoGerente();
    }
}