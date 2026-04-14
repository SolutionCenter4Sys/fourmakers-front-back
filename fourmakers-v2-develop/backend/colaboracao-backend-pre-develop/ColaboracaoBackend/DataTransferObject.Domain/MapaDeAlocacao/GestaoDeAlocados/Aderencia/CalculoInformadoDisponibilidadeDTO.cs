using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObject.Domain.MapaDeAlocacao.Aderencia;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.Aderencia
{
    public class CalculoInformadoDisponibilidadeDTO
    {
        public CalculoInformadoDTO Calculo { get; set; }
        public DateTime? DataDisponibilidade { get; set; } = null;
        public double HorasDisponiveis { get; set; } = 0;
    }
}