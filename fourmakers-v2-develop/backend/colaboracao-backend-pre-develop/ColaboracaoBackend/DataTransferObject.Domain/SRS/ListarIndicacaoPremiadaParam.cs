namespace DataTransferObject.Domain.SRS
{
    public class ListarIndicacaoPremiadaParcialParam
    {
        public string DataDe { get; set; }
        public string DataAte { get; set; }
        public string CodigoInternoColaboradorAnalista { get; set; }
        public bool? CvValido { get; set; }
        public bool? RetornoAoProfissional { get; set; }
        public string CodDiretoria { get; set; }
        public string Pesquisa { get; set; }
        public string OrderByPropertie { get; set; }
        public string OrderType { get; set; }
        public int? Limite { get; set; }

        public void AjustarParametros()
        {
            if (string.IsNullOrEmpty(DataDe))
                DataDe = null;

            if (string.IsNullOrEmpty(DataAte))
                DataAte = null;

            if (string.IsNullOrEmpty(CodigoInternoColaboradorAnalista))
                CodigoInternoColaboradorAnalista = null;

            if (string.IsNullOrEmpty(CodDiretoria))
                CodDiretoria = null;

            if (string.IsNullOrEmpty(Pesquisa))
                Pesquisa = null;

            if (string.IsNullOrEmpty(OrderByPropertie))
                OrderByPropertie = null;

            if (string.IsNullOrEmpty(OrderType))
                OrderType = null;
        }
    }
}