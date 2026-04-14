namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class CalculoMensalDTO
    {
        public string CodigoColaborador { get; set; }
        public int? CodTbdAlocado { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
        public double Horas { get; set; }
        public string StatusColaboradorPeriodoAlocacao { get; set; }
        public int OrgId { get; set; }
    }
}