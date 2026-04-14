using System;
using System.Collections.Generic;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Endereco;
using DataTransferObject.Domain.MapaDeAlocacao.Aderencia;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.Aderencia
{
    public class ColaboradorAderenciaSimplificadoDTO
    {
        public string CodigoInternoColaborador { get; set; }
        public string NomeCompleto { get; set; }
        public string Cargo { get; set; }
        public double HorasDisponiveis { get; set; }
        public DateTime? DataDisponibilidade { get; set; }
        public decimal? CustoHora { get; set; }
        public decimal? RateCard { get; set; }
        public bool Ativo { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public LocalidadeDTO? Localidade { get; set; }
        public List<SkillNivelDTO> Skills { get; set; }
        public List<PeriodoDTO> PeriodoDTOs { get; set; }
        
    }
}