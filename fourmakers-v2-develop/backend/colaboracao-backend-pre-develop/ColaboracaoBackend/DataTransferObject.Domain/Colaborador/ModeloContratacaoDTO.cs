namespace DataTransferObject.Domain.Colaborador;

public class ModeloContratacaoDTO
{
    public string CodigoModeloContratacao { get; set; }
    public string Descricao  { get; set; }
    public int OrgId { get; set; }
    public bool DeveCriarNF  { get; set; }
}