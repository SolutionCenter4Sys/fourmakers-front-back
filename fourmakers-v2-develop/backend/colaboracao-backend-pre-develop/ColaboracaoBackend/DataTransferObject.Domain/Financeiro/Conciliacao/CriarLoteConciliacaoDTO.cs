namespace DataTransferObject.Domain.Financeiro.Conciliacao
{
    public class CriarLoteConciliacaoDTO
    {
        /// <summary>
        /// CNPJ da empresa
        /// </summary>
        public string Cnpj { get; set; }

        /// <summary>
        /// Competência no formato MM/YYYY
        /// </summary>
        public string Competencia { get; set; }

    }
} 