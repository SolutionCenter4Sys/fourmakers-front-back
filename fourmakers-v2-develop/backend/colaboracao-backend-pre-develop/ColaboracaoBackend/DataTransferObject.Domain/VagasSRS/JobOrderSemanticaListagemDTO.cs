namespace DataTransferObject.Domain.VagasSRS;

/// <summary>
/// Resposta pública de listagem semântica (sem detalhe de skills por linha).
/// </summary>
public class JobOrderSemanticaListagemDTO
{
    public int JoborderId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int? CargoId { get; set; }
    public string Cargo { get; set; }
    public string StackPrincipal { get; set; }
    public string Skills { get; set; }
}
