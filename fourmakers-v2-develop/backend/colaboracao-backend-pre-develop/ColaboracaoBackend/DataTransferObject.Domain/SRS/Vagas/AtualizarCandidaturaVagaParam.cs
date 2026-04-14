using DataTransferObject.Domain.Vaga;

namespace DataTransferObject.Domain.SRS.Vagas
{
    public class AtualizarCandidaturaVagaParam
    {
        public int VagaId { get; set; }
        public int CandidatoId { get; set; }
        public int StatusId { get; set; }
        public string Descricao { get; set; }
        public OrigemVagaEnum Origem { get; set; }
    }
}