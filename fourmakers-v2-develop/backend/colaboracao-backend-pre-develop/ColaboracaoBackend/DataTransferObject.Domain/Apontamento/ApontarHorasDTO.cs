namespace DataTransferObject.Domain.Apontamento
{
    public class ApontarHorasDTO
    {
        public string cpfColaborador { get; set; }
        public string ProjetoId { get; set; }
        public string AtividadeId { get; set; }
        public long Horas { get; set; }
        public string DataRegistro { get; set; }
        public int? DiaQuebraSemana { get; set; }
        public bool? DeveSomarApontamentoDia { get; set; }
        public string Observacao { get; set; }
    }
}