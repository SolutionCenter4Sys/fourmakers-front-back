using DataTransferObject.Domain.Colaborador;

namespace SRS.API.DTOs
{
    public class EditarColaboradorComDadosDemograficosParam
    {
        public EditarColaboradorInputDTO Colaborador { get; set; }
        public DadosDemograficosColaboradorDTO DadosDemograficos { get; set; }
        
        /// <summary>
        /// Dados da candidatura - opcional
        /// Quando informado, atualiza o modelo de trabalho da candidatura específica
        /// </summary>
        public DadosCandidaturaInputDTO DadosCandidatura { get; set; }
    }
}
