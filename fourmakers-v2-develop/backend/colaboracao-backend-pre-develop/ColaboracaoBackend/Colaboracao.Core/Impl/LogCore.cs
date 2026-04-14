using DataTransferObject.Domain.Log;
using Microsoft.Extensions.Logging;

namespace Colaboracao.Core
{
    public class LogCore : ILogCore
    {
        private readonly ILogger _logger;
        public LogCore(ILogger<LogCore> logger)
        {
            _logger = logger;
        }

        public void Log(string message, LevelsEnum level)
        {
            switch (level)
            {
                case LevelsEnum.Trace:
                    _logger.LogTrace(message);
                    break;

                case LevelsEnum.Debug:
                    _logger.LogDebug(message);
                    break;

                case LevelsEnum.Information:
                    _logger.LogInformation(message);
                    break;

                case LevelsEnum.Warning:
                    _logger.LogWarning(message);
                    break;

                case LevelsEnum.Error:
                    _logger.LogError(message);
                    break;

                case LevelsEnum.Critical:
                    _logger.LogCritical(message);
                    break;

                default:
                    _logger.LogTrace(message);
                    break;
            }
        }
    }
}