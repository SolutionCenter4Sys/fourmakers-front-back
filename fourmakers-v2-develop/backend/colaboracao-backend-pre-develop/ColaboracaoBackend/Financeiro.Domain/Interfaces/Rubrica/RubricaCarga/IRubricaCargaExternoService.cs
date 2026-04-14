using DataTransferObject.Domain.Arquivo;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Rubrica.Rubrica;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;

namespace Financeiro.Domain.Interfaces.Rubrica.RubricaCarga;

public interface IRubricaCargaExternoService
{
    Task<ApiGenericResult<RubricaCargaMassivaResult>> ProcessarCargaMassiva(string tokenSistema, Base64DTO base64DTO, PoliticaErroCarga politicaErro = PoliticaErroCarga.ContinuarComAviso, PoliticaConflitoCarga politicaConflito = PoliticaConflitoCarga.NaoAtualizar, string arquivoOrigem = null);
    Task<string> ConverterJsonParaCSV(List<RubricaCargaJsonItemDTO> rubricasJson);
}