using Microsoft.Extensions.Logging;

namespace Logs.Infra.Constants
{
    /// <summary>
    /// Constantes para logs estruturados
    /// </summary>
    public static class StructuredLogConstants
    {
        /// <summary>
        /// Categoria específica para logs estruturados
        /// </summary>
        public const string LogCategory = "ColaboracaoBackend.StructuredLog";

        /// <summary>
        /// LogLevel específico para logs estruturados
        /// Usa Trace para permitir captura de todos os logs estruturados
        /// </summary>
        public const LogLevel StructuredLogLevel = LogLevel.Trace;
    }
}

