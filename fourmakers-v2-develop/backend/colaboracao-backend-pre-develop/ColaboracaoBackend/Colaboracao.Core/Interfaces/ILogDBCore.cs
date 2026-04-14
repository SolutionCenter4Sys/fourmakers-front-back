using DataTransferObject.Domain.Log;
using System;
using System.Threading.Tasks;

namespace Colaboracao.Core
{
    public interface ILogDBCore
    {
        void SaveExceptionLogInDatabase(string message, Exception ex, ProcessIdentifierEnum processIdentifier, string codigoInternoColaboradorOrigin = null);
        void SaveExceptionLogInDatabaseAndSendEmail(string message, Exception ex, ProcessIdentifierEnum processIdentifier, string codigoInternoColaboradorOrigin = null);
        void SaveLogDefaultInDatabase(string message, string completeMessage, ProcessIdentifierEnum processIdentifier, string codigoInternoColaboradorOrigin = null);
        Task<LogDTO> ObterUltimoLogPorProcessIdentifierAsync(ProcessIdentifierEnum processIdentifier);

    }
}