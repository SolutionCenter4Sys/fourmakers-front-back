using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class CadastroMapaAlocacaoDTO
    {
        [JsonIgnore]
        public long PeriodoAlocadoId { get; set; }
        public string CpfColaborador { get; set; }
        public string CodigoTbd { get; set; }
        public string CodigoColaborador { get; set; }
        public string ColaboradorNome { get; set; }
        public string CodigoGestor { get; set; }
        public string CodigoProjeto { get; set; }
        public string NomeGestor { get; set; }
        public string NomeProjeto { get; set; }
        private DateTime _dataInicio { get; set; }
        private DateTime _dataFim { get; set; }
        public DateTime DataInicio
        {
            get { return new DateTime(_dataInicio.Year, _dataInicio.Month, _dataInicio.Day, 0, 0, 0); }
            set { _dataInicio = value; }
        }
        public DateTime DataFim
        {
            get { return new DateTime(_dataFim.Year, _dataFim.Month, _dataFim.Day, 23, 59, 59); }
            set { _dataFim = value; }
        }
        public DateTime DataAlteracao { get; set; }
        public bool IncluiFimDeSemana { get; set; }
        public double QuantidadeHoras { get; set; }
        public string Oportunidade { get; set; }
        public string Observacao { get; set; }
        public sbyte? Prioritario { get; set; }
        public double? Percentual { get; set; }
        public int OrgId { get; set; }
        public DateTime? DataCriacao { get; set; }
        public bool? Ativo { get; set; }
        public bool? FlagRetroalimentaCV { get; set; }
        public string IdPerfilAlocacao { get; set; }
        public string NomePerfilAlocacao { get; set; }
        public List<ItemSkillPerfilAlocacaoDTO> PerfilSkills { get; set; }
    }
}