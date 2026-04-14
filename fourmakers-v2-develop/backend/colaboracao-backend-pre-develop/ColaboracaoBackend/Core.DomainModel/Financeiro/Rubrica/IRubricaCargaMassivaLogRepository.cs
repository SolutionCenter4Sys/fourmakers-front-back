using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;

namespace Core.Domain.Financeiro.Rubrica;

public interface IRubricaCargaMassivaLogRepository
{
    Task<int> InserirLogCarga(RubricaCargaMassivaLogDTO logCarga);
    Task AtualizarStatusCarga(string codigoCargaRubrica, string status, string mensagemErro = null, int qtdSucesso = 0, int qtdRetornados = 0);
    Task AtualizarMesAnoEQuantidade(string codigoCargaRubrica, int mes, int ano, int qtdRetornados);
    Task<int> InserirLogItem(RubricaCargaMassivaItemLogDTO logItem);
}