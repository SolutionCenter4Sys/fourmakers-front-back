using System;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    /// <summary>
    /// Resumo de PDI para listagem "PDIs do time".
    /// </summary>
    public class PdiResumoTimeDTO
    {
        public string ColaboradorId { get; set; }
        public string NomeColaborador { get; set; }
        public Guid PdiId { get; set; }
        public string Titulo { get; set; }
        public string Status { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public double Progress { get; set; }
    }
}
