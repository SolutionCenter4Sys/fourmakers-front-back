using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.Vaga;
using System;

namespace SRS.Domain.Impl.Service.Strategy
{
    /// <summary>
    /// Contexto que contém todas as informações necessárias para executar as estratégias de mudança de status
    /// </summary>
    public class MudancaStatusCandidaturaContext
    {
        public CandidaturaRecrutamentoDTO Candidatura { get; set; }
        public VagaRecrutamentoDTO Vaga { get; set; }
        public int NovoStatusId { get; set; }
        public int StatusAnteriorId { get; set; }
        public string CodColaborador { get; set; }
        public string Comentario { get; set; }
        public string ComentarioId { get; set; }
        public string MensagemRetorno { get; set; }
    }
}

