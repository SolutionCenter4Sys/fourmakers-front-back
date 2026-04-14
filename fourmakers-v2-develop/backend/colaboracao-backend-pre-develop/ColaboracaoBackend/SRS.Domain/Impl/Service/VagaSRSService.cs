using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.Vagas;
using SRS.Domain.Interfaces.Service;
using SRS.Infra.Interfaces;
using System.Collections.Generic;
using DataTransferObject.Domain.VagasSRS;
using Logs.Infra.Attributes;

namespace SRS.Domain.Impl.Service
{
    [LogDomainClass]
    public class VagaSRSService : IVagaSRSService
    {
        private readonly ISRSVagaRepository _vagaRepository;
        public VagaSRSService(ISRSVagaRepository vagaRepository)
        {
            _vagaRepository = vagaRepository;
        }

        public List<DropDownItemDTO> ListarAprovadores()
        {
            return _vagaRepository.ListarAprovadores();
        }

        public List<DropDownItemDTO> ListarCargaHoraria()
        {
            return _vagaRepository.ListarCargaHoraria();
        }

        public List<CargoDropdownItemDTO> ListarCargos()
        {
            return _vagaRepository.ListarCargos();
        }

        public List<DropDownItemDTO> ListarConfiguracaoMaquina(string idContaCrm, int hardskillId)
        {
            return _vagaRepository.ListarConfiguracaoMaquina(idContaCrm, hardskillId);
        }

        public List<DropDownItemDTO> ListarDuracaoContrato()
        {
            return _vagaRepository.ListarDuracaoContrato();
        }

        public List<DropDownItemDTO> ListarLocalTrabalho()
        {
            return _vagaRepository.ListarLocalTrabalho();
        }

        public List<DropDownItemDTO> ListarSolicitantes()
        {
            return _vagaRepository.ListarSolicitantes();
        }

        public List<DropDownItemDTO> ListarStackPrincipal()
        {
            return _vagaRepository.ListarStackPrincipal();
        }

        public List<DropDownItemDTO> ListarTermometroVagas()
        {
            return _vagaRepository.ListarTermometroVagas();
        }

        public List<DropDownItemDTO> ListarTipoContratacao()
        {
            return _vagaRepository.ListarTipoContratacao();
        }

        public List<DropDownItemDTO> ListarTipoVaga()
        {
            return _vagaRepository.ListarTipoVaga();
        }

        public List<DropDownItemDTO> ListarUnidadesSRS()
        {
            return _vagaRepository.ListarUnidadesSRS();
        }
        
        public List<JobOrderSemanticaDTO> ListarVagasParaSemanticaComSkill(int cursor, int limite)
        {
            return _vagaRepository.ListarVagasParaSemanticaComSkill(cursor, limite);
        }
    }
}