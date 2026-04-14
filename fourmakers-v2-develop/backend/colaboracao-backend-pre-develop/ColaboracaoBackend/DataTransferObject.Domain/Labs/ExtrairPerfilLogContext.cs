namespace DataTransferObject.Domain.Labs
{
    /// <summary>
    /// Contexto opcional para gravar log ao chamar ExtrairPerfilDeUmPrompt.
    /// Quando informado, o ExtractorService grava o request/response em tb_labs_log_extractor_extract_vaga.
    /// </summary>
    public class ExtrairPerfilLogContext
    {
        public int OrgId { get; set; }
        public string? VagaId { get; set; }
        public string? CodigoInternoColaborador { get; set; }
    }
}
