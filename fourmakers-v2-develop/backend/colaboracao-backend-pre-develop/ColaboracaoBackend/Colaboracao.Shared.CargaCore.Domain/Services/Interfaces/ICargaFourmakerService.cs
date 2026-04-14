using DataTransferObject.Domain.Carga;
using System.Collections.Generic;

namespace CargaCore.Domain.Interfaces
{
    public interface ICargaFourmakerService
    {
        void InsereCargaColaborador(List<ColaboradorCargaDTO> cargaColaborador, int orgId);
        void InsereCargaHierarquiaColaborador(List<ColaboradorHierarquiaCargaDTO> cargaHierarquia, int orgId);
        void InsereCargaProjeto(List<ProjetoCargaDTO> cargaProjeto, int orgId);
        void InsereCargaHierarquiaProjeto(List<ProjetoGerenteCargaDTO> cargaHierarquia, int orgId);
    }
}