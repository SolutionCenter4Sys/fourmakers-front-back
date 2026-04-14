namespace DataTransferObject.Domain.Marketing.Comunicacao.Analytics
{
    public class AnalyticsBigNumbersDTO
    {
        public int PostsComunidades { get; set; }
        public int EngajamentoPosts { get; set; }
        public int LikesPosts { get; set; }
        public int ComentariosPosts { get; set; }

        public int ComunicadosPublicados { get; set; }
        public int ComunicadosInformativos { get; set; }
        public int ComunicadosDocumentos { get; set; }

        public int AceitesComunicados { get; set; }
        public int ComunicadosObrigatorios { get; set; }
        public int ComunicadosOpcionais { get; set; }

        public int TotalVisualizacoesComunicados { get; set; }
        public int LikesComunicados { get; set; }
        public int ComentariosComunicados { get; set; }
        public decimal TaxaEngajamentoComunicados { get; set; }
    }
}
