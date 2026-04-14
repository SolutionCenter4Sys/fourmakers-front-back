using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RetornoBancaria;
using System.Threading.Tasks;

namespace Financeiro.Domain.Interfaces.IntegracaoBancaria.RetornoBancaria
{
    public interface IRetornoBancariaService
    {
        Task<ApiGenericResult<ProcessarRetornoCnabResult>> ProcessarRetornoCnab(
            string hashRemessa,
            string nomeArquivo,
            string conteudoArquivo,
            string cpfUsuario,
            string tipoRemessa,
            int orgId);
    }
}
