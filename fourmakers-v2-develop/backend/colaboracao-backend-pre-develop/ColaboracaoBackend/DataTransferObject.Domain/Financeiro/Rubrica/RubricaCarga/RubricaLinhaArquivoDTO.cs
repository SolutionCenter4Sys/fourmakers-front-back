using System;

namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;

public class RubricaColaboradorLinhaArquivoDTO
{
    // Dados originais do arquivo
    public string Vigencia { get; set; }
    public string NomeColaborador { get; set; }
    public string CPF { get; set; }
    public string CodRubrica { get; set; }
    public string DescricaoRubrica { get; set; }
    public string ValorGerado { get; set; }
    public int NumeroLinha { get; set; }
    
    // Informações adicionais para processamento
    public RubricaColaboradorProcessamentoInfo ProcessamentoInfo { get; set; } = new();
}

public class RubricaColaboradorProcessamentoInfo
{
    public string LinhaStringOriginal { get; set; }
    public string ErroFormatacao { get; set; }
    public Guid? RubricaId { get; set; }
    public decimal? ValorTratado { get; set; }
    public Guid? CodigoInternoColaborador { get; set; }
    public int MesVigencia { get; set; }
    public int AnoVigencia { get; set; }
    public Guid? IdRubricaColaboradorJaExistente { get; set; } // Para atualização quando política permite
}