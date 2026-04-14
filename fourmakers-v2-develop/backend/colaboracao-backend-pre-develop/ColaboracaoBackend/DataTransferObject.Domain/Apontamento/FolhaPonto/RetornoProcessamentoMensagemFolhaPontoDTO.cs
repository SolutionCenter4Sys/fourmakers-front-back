using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Apontamento.FolhaPonto
{
    public class RetornoProcessamentoMensagemFolhaPontoDTO
    {
        public List<string> Erros { get; set; }
        public string StackTrace { get; set; }
        public bool ProcessadoComSucesso { get; set; }
        public RelatorioPontoRootDTO RelatorioFolhaPonto { get; set; }
    }
} 