using System.Collections.Generic;
using DataTransferObject.Domain.Marketing.Comunicacao;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Grupo
{
    public class ColaboradoresResponseDTO
    {
        public List<ColaboradorResumoDTO> Colaboradores { get; set; } = new List<ColaboradorResumoDTO>();
        public List<ColaboradorDisponivelModeloContratacaoResumoDTO> ModelosContratacaoResumo { get; set; } = new List<ColaboradorDisponivelModeloContratacaoResumoDTO>();
        public List<ColaboradorDisponivelDiretoriaResumoDTO> DiretoriasResumo { get; set; } = new List<ColaboradorDisponivelDiretoriaResumoDTO>();
    }
}
