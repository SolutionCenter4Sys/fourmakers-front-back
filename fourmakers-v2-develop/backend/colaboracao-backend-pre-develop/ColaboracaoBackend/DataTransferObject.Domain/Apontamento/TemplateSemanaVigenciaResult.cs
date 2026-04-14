using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Apontamento
{
    public class TemplateSemanaVigenciaResult
    {
        public List<SemanaDTO> semanas { get; set; }
    }

    public class SemanaDTO
    {
        public int NumeroSemana { get; set; }
        public int PrimeiroDiaSemana { get; set; }
        public int UltimoDiaSemana { get; set; }
        public List<DiaDTO> Dias { get; set; }
    }

    public class DiaDTO
    {
        public DateTime Data { get; set; }
        public int NumeroSemanaDia { get; set; }
        public string Label { get; set; }
        public int NumeroSemana { get; set; }
        public bool MesAtivo { get; set; }
        public bool Feriado { get; set; }
    }
}