namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    /// <summary>Contagens por status para painel RH (mesmos buckets das métricas PDI).</summary>
    public class PdiRhBigNumbersResultDTO
    {
        public int NaoIniciado { get; set; }

        public int EmAnalise { get; set; }

        /// <summary>Status <c>IN_PROGRESS</c> no banco.</summary>
        public int EmAndamento { get; set; }

        public int Finalizados { get; set; }

        public int Cancelados { get; set; }
    }
}
