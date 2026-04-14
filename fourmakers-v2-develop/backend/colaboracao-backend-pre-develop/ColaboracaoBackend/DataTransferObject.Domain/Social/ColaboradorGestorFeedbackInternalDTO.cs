namespace DataTransferObject.Domain.Social
{
    // codigo_interno_colaborador no MySQL é varchar(36); o driver retorna string.
    public class ColaboradorGestorFeedbackInternalDTO
    {
        public string CodigoInternoColaborador { get; set; }
        public string NomeCompleto { get; set; }
        public string CodColaboradorExterno { get; set; }
    }
}
