namespace DataTransferObject.Domain.SRS
{
    public class EditarIndicacaoPremiadaParcialParam
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string PerfilAvaliado { get; set; }
        public bool CvValido { get; set; }
        public bool RetornoAoProfissional { get; set; }
        public string CodDiretoria { get; set; }
        public string CodigoInternoColaboradorAnalista { get; set; }
    }
}