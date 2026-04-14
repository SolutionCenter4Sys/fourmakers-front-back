namespace DataTransferObject.Domain.Colaborador
{
    public class EstatisticasProcDTO
    {
        public string descricao { get; set; }
        public int quantidade_total { get; set; }
        public int quantidade_gestores { get; set; }
        public int quantidade_nao_gestores { get; set; }
        public decimal percentual { get; set; }
        public int org_id { get; set; }
    }
}