using DataTransferObject.Domain.Log;
using System;
using System.Threading.Tasks;

namespace Core.Domain.LogRepo
{
    public interface ILogRepository
    {
        void InsertLog(string message, string completeMessage, DateTime date, string stackTrace, LogTypeEnum logType, ProcessIdentifierEnum? processIdentifier = null, string codigoInternoColaboradorOrigin = null);
        Task<LogDTO> ObterUltimoLogPorProcessIdentifierAsync(ProcessIdentifierEnum processIdentifier);
    }
}