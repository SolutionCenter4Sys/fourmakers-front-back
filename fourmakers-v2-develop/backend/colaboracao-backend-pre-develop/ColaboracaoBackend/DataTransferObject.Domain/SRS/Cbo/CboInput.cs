namespace DataTransferObject.Domain.SRS.Cbo;

/// <summary>
/// Request para criar ou atualizar um CBO (Código Brasileiro de Ocupações).
/// </summary>
public class CboInput
{
    public string Codigo { get; set; }
    public string Titulo { get; set; }

    /// <summary>Descrição detalhada (ex.: texto da CBO no site do governo).</summary>
    public string Descricao { get; set; }

    public bool Ativo { get; set; } = true;
}
