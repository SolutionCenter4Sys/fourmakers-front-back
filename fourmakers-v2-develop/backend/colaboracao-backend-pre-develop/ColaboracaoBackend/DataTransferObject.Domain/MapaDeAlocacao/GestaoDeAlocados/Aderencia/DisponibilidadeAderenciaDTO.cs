namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.Aderencia
{
    public enum DisponbilidadeAderenciaEnum
    {
        Proximos15Dias,
        Entre15e30Dias,
        Acima30Dias
    }

    public class DisponibilidadeAderenciaDTO
    {
        public DisponbilidadeAderenciaEnum DisponibilidadeEnum { get; set; }
        public double TotalHorasAlocadasQueVaoFicarDisponiveis { get; set; }
        public double Percentual { get; set; }
    }
}