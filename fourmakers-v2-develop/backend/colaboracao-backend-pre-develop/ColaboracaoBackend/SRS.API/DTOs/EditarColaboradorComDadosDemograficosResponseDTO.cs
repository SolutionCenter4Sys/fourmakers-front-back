using System.Collections.Generic;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Vaga;

namespace SRS.API.DTOs
{
    /// <summary>
    /// DTO de resposta para buscar dados do colaborador, dados demográficos e candidatura para edição
    /// </summary>
    public class EditarColaboradorComDadosDemograficosResponseDTO
    {
        public EditarColaboradorInputDTO Colaborador { get; set; }
        public DadosDemograficosColaboradorDTO DadosDemograficos { get; set; }
        
        /// <summary>
        /// Dados da candidatura - opcional
        /// Preenchido quando idCandidatura é informado na requisição
        /// </summary>
        public DadosCandidaturaInputDTO DadosCandidatura { get; set; }

        /// <summary>
        /// Todas as candidaturas do colaborador no recrutamento (mesmo conjunto de BuscarCandidaturasColaborador).
        /// </summary>
        public List<ListarCandidaturasPorCodCandidatoResult> Candidaturas { get; set; }
    }
}

