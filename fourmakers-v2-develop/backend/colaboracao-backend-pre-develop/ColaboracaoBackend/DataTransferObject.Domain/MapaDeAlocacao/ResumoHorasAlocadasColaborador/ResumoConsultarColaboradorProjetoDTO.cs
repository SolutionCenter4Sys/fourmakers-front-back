namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class ResumoConsultarColaboradorProjetoDTO
    {
        public string? NomeProjeto { get; set; }
        public long? HorasComerciais { get; set; }
        public long? HorasAlocadas { get; set; }
        public long? HorasDisponiveis { get; set; }
    }
}