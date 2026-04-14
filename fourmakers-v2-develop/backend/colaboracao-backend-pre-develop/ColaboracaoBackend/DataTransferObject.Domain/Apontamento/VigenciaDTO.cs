namespace DataTransferObject.Domain.Apontamento
{
    public class VigenciaDTO
    {
        public string Id { get; set; }

        public int Mes { get; set; }

        public int Ano { get; set; }

        public decimal Horas_trabalhadas { get; set; }
        public string Label { get; set; } = string.Empty;

        // public string Status { get; set; }
    }
}