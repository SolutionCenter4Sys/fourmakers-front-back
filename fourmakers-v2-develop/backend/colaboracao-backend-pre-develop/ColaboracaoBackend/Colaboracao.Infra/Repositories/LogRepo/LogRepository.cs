using Colaboracao.Infra.Context;
using Core.Domain.LogRepo;
using DataTransferObject.Domain.Log;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.LogRepo
{
    public class LogRepository : ILogRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        public LogRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }

        public void InsertLog(string message, string completeMessage, DateTime date, string stackTrace, LogTypeEnum logType, ProcessIdentifierEnum? processIdentifier = null, string codigoInternoColaboradorOrigin = null)
        {
            _colaboradorContext.Add(new tb_log
            {
                message = message,
                complete_message = completeMessage,
                date = date,
                stacktrace = stackTrace,
                log_type = logType.ToString(),
                process_identifier = processIdentifier?.ToString(),
                codigo_interno_colaborador_origin = codigoInternoColaboradorOrigin
            });
            _colaboradorContext.SaveChanges();
        }

        public async Task<LogDTO> ObterUltimoLogPorProcessIdentifierAsync(ProcessIdentifierEnum processIdentifier)
        {
            return await _colaboradorContext.tb_log
                .Where(log => log.process_identifier == processIdentifier.ToString())
                .OrderByDescending(log => log.date)
                .Select(log => new LogDTO
                {
                    Data = log.date,
                    Mensagem = log.message,
                    MensagemCompleta = log.complete_message,
                    StackTrace = log.stacktrace,
                    Tipo = log.log_type,
                    Identificador = log.process_identifier,
                    CodigoColaboradorInternoOrigem = log.codigo_interno_colaborador_origin
                })
                .FirstOrDefaultAsync();
        }

    }
}