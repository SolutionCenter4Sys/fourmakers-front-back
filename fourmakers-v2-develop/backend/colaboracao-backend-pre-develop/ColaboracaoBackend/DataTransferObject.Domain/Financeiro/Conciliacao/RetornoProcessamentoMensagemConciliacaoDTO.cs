using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Conciliacao
{
    public class RetornoProcessamentoMensagemConciliacaoDTO
    {   
        public List<string> Erros { get; set; }
        public string StackTrace { get; set; }
        public bool ProcessadoComSucesso { get; set; }
        public RetornoAnaliseHoleriteDTO RetornoAnaliseHolerite { get; set; }
    }
} 