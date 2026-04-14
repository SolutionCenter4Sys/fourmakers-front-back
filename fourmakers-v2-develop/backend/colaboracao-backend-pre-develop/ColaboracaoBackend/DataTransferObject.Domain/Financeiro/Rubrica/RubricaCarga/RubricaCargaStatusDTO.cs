using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;

public class RubricaCargaStatusDTO
{
    public string Id { get; set; }
    public string StatusProcessamento { get; set; }
    public string StatusSimplificado { get; set; }
    public string CodDiretoria { get; set; }
    public string Diretoria { get; set; }
    public string Ano { get; set; }
    public string Mes { get; set; }
    public DateTime Data { get; set; }
    public int RegistrosProcessadosSucesso { get; set; }
    public int RegistrosRetornados { get; set; }
    public string Descricao  { get; set; }
    public string CodigoCarga { get; set; }
    public string NomeUsuario { get; set; }
    public List<RubricaCargaItemLogDTO> ItensLog { get; set; } = new List<RubricaCargaItemLogDTO>();
}