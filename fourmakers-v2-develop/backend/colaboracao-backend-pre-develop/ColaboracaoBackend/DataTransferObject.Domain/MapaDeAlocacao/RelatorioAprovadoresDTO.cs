using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeAlocacao;

public class RelatorioAprovadoresDTO
{
    public string ProjetoId { get; set; }
    public string NomeProjeto { get; set; }
    public string ClienteId { get; set; }
    public string? NomeCliente { get; set; } = null;
    public List<string> Aprovadores { get; set; } = [];
}