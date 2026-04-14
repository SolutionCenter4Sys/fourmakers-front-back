using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Apontamento
{
    public class ApontamentosPorVigenciaDTO
    {
        public VigenciaDTO Vigencia { get; set; }
        public List<ApontamentoMensalDTO> ApontamentosMensais { get; set; }
        public TotalizadorApontamentosDTO TotalizadorApontamentosMensais { get; set; }
        public List<ApontamentoColaboradorDTO> ColaboradorApontamentos { get; set; }
        public TotalizadorApontamentosBigNumbersDTO TotalizadorApontamentosBigNumbers { get; set; }
        public string PdfFolhaPontoUrl { get; set; }
        public string PdfHoleriteUrl { get; set; }
        public DateTime DataColetaDeDados { get; set; }
        public bool OcultaTimeSheet { get; set; }
    }
}