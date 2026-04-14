using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.Vagas;
using System.Collections.Generic;
using DataTransferObject.Domain.VagasSRS;

namespace SRS.Infra.Interfaces
{
    public interface ISRSVagaRepository
    {
        List<DropDownItemDTO> ListarSolicitantes();
        List<DropDownItemDTO> ListarAprovadores();
        List<DropDownItemDTO> ListarTermometroVagas();
        List<DropDownItemDTO> ListarStackPrincipal();
        List<DropDownItemDTO> ListarConfiguracaoMaquina(string idContaCrm, int hardskillId);
        List<DropDownItemDTO> ListarTipoVaga();
        List<DropDownItemDTO> ListarDuracaoContrato();
        List<CargoDropdownItemDTO> ListarCargos();
        List<DropDownItemDTO> ListarTipoContratacao();
        List<DropDownItemDTO> ListarCargaHoraria();
        List<DropDownItemDTO> ListarLocalTrabalho();
        List<DropDownItemDTO> ListarUnidadesSRS();
        List<JobOrderSemanticaDTO> ListarVagasParaSemanticaComSkill(int cursor, int limite);
    }
}