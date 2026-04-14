using DataTransferObject.Domain.Log;
using System.Threading.Tasks;

namespace Core.Domain
{
    public interface ILogBancoTalentoSRSRepository
    {
        /// <summary>
        /// Insere um registro de log da sincronização do Banco de Talentos SRS
        /// </summary>
        /// <param name="logDto">Dados do log a ser inserido</param>
        /// <returns>True se inserido com sucesso</returns>
        Task<bool> InserirLogBancoTalentoSRSAsync(LogBancoTalentoSRSDTO logDto);
    }
}
