using DataTransferObject.Domain.Arquivo;

namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;

public class RubricaCargaMassivaInput
{
    public string CargaCSVBase64 { get; set; }
    public PoliticaErroCarga PoliticaErro { get; set; } = PoliticaErroCarga.ContinuarComAviso;
    public PoliticaConflitoCarga PoliticaConflito { get; set; } = PoliticaConflitoCarga.NaoAtualizar;
}