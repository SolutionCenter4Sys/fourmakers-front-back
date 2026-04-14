using System;
using System.Collections.Generic;

namespace Logs.Infra.Models
{
    public class LogEntry
    {
        public long Id { get; set; }
        public string TraceId { get; set; }
        public string FrontendTraceId { get; set; }
        public string Projeto { get; set; }
        public string ClassePath { get; set; }
        public string Metodo { get; set; }
        public string ParametrosJson { get; set; }
        public string ClaimsJson { get; set; }
        public DateTime DataHora { get; set; }
        public string HttpMethod { get; set; }
        public string RequestPath { get; set; }
        public int? StatusCode { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionStackTrace { get; set; }
        public long? TempoExecucaoMs { get; set; }
    }
}

