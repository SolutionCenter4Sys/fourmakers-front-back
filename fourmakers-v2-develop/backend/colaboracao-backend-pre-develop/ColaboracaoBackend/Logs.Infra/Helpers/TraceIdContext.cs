using System.Threading;

namespace Logs.Infra.Helpers
{
    /// <summary>
    /// Contexto para propagar o Trace ID e Frontend Trace ID através da cadeia de chamadas assíncronas
    /// </summary>
    public static class TraceIdContext
    {
        private static readonly AsyncLocal<string> _traceId = new AsyncLocal<string>();
        private static readonly AsyncLocal<string> _frontendTraceId = new AsyncLocal<string>();

        /// <summary>
        /// Obtém o Trace ID atual do contexto assíncrono
        /// </summary>
        public static string Current
        {
            get => _traceId.Value;
            set => _traceId.Value = value;
        }

        /// <summary>
        /// Obtém o Frontend Trace ID atual do contexto assíncrono
        /// </summary>
        public static string FrontendTraceId
        {
            get => _frontendTraceId.Value;
            set => _frontendTraceId.Value = value;
        }

        /// <summary>
        /// Limpa o Trace ID e Frontend Trace ID do contexto
        /// </summary>
        public static void Clear()
        {
            _traceId.Value = null;
            _frontendTraceId.Value = null;
        }
    }
}

