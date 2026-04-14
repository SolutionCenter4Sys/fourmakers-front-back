using ApiClient.Domain;
using Colaboracao.Helper;
using Core.Domain.LogRepo;
using DataTransferObject.Domain.Log;
using System;
using System.Threading.Tasks;

namespace Colaboracao.Core
{
    public class LogDBCore : ILogDBCore
    {
        private readonly ILogRepository _logRepository;
        private readonly IEnvioEmail _envioEmail;

        public LogDBCore(ILogRepository logRepository, IEnvioEmail envioEmail)
        {
            _logRepository = logRepository;
            _envioEmail = envioEmail;
        }

        public void SaveExceptionLogInDatabase(string message, Exception ex, ProcessIdentifierEnum processIdentifier, string codigoInternoColaboradorOrigin)
        {
            var completeMessage = message + "\n" + ex.Message;
            var innerException = ex.InnerException;
            while (innerException != null)
            {
                completeMessage += "\n" + innerException.Message;
                innerException = innerException.InnerException;
            }

            SaveLogInDatabase(
                message: message,
                completeMessage: completeMessage,
                logType: LogTypeEnum.Exception,
                processIdentifier: processIdentifier,
                stackTrace: ex.StackTrace,
                codigoInternoColaboradorOrigin: codigoInternoColaboradorOrigin
            );
        }

        public void SaveExceptionLogInDatabaseAndSendEmail(string message, Exception ex, ProcessIdentifierEnum processIdentifier, string codigoInternoColaboradorOrigin = null)
        {
            SaveExceptionLogInDatabase(message, ex, processIdentifier, codigoInternoColaboradorOrigin);

            var completeMessage = message + "\n" + ex.Message;

            if (VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.AMBIENTE) == "PRD")
            {
                _envioEmail.EnviaEmailTemplateFoursys(
                                                        "Backoffice",
                                                        completeMessage + (ex.StackTrace != null ? "Stack trace: </br>" + ex.StackTrace : ""),
                                                        "Log importante do Backoffice",
                                                        VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.MAIL_DESTINATION)
                                                     );
            }
        }

        public void SaveLogDefaultInDatabase(string message, string completeMessage, ProcessIdentifierEnum processIdentifier, string codigoInternoColaboradorOrigin = null)
        {
            SaveLogInDatabase(
                message: message,
                completeMessage: completeMessage,
                logType: LogTypeEnum.LogDefault,
                processIdentifier: processIdentifier,
                stackTrace: null,
                codigoInternoColaboradorOrigin: codigoInternoColaboradorOrigin
            );
        }

        private void SaveLogInDatabase(string message, string completeMessage, LogTypeEnum logType, ProcessIdentifierEnum processIdentifier, string stackTrace, string codigoInternoColaboradorOrigin = null)
        {
            if (_logRepository != null)
            {
                _logRepository.InsertLog(
                    message: message,
                    completeMessage: completeMessage,
                    date: DateTime.Now,
                    stackTrace: stackTrace,
                    logType: logType,
                    processIdentifier: processIdentifier,
                    codigoInternoColaboradorOrigin: codigoInternoColaboradorOrigin
                );
            }
        }

        public async Task<LogDTO> ObterUltimoLogPorProcessIdentifierAsync(ProcessIdentifierEnum processIdentifier)
        {
            return await _logRepository.ObterUltimoLogPorProcessIdentifierAsync(processIdentifier);
        }

    }
}