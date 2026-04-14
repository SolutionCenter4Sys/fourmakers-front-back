namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class AlocacaoDTO
    {
        public string Mes { get; set; }
        public string Horas { get; set; }
        public int MesAlocacao { get; set; }
        public int AnoAlocacao { get; set; }
        public StatusHorasEnum StatusHorasRecursoFiltro { get; set; }
    }
}