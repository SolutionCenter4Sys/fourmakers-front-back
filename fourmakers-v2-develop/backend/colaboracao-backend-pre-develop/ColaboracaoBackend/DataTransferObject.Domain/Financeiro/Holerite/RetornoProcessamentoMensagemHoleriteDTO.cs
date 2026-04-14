using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Holerite
{
    public class RetornoProcessamentoMensagemHoleriteDTO
    {   
        public List<string> Erros { get; set; }
        public string StackTrace { get; set; }
        public bool ProcessadoComSucesso { get; set; }
        public HoleriteDTO Holerite { get; set; }
    }
   
}


