using DataTransferObject.Domain.Competencia;
using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class AlocacaoColabETbdDTO
    {
        public long? ColaboradorAlocadoId { get; set; }
        public long? PeriodoAlocadoId { get; set; }
        public string CodDepartamento { get; set; }
        public string NomeDepartamento { get; set; }
        public string CodColaborador { get; set; }
        public string NomeColaborador { get; set; }
        public string Cpf { get; set; }
        public string NomeGestorAdm { get; set; }
        public AlocacoesColabETbdProjetoDTO Projeto { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public double? QuantidadeDeHoras { get; set; }
        public double? Percentual { get; set; }
        public bool Tbd { get; set; }
        public bool Prioridade { get; set; }
        public string Oportunidade { get; set; }
        public string Observacao { get; set; }
        public bool RetroalimentaCv { get; set; }
        public PerfilAlocacaoDTO Perfil { get; set; }
        public List<SkillNivelDTO> HabilidadesAlocacao { get; set; } = new();
        public List<SkillNivelDTO> HabilidadesTecnicas { get; set; } = new();
    }

    public class AlocacoesColabETbdProjetoDTO
    {
        public string CodProjeto { get; set; }
        public string NomeProjeto { get; set; }
        public string CodCliente { get; set; }
        public string NomeCliente { get; set; }
        public string LabelClienteProjeto { get; set; }
        public string StatusProjeto { get; set; }
        public List<AlocacoesColabETbdGestorDTO> GestoresProjeto { get; set; } = new List<AlocacoesColabETbdGestorDTO>();
    }

    public class AlocacoesColabETbdGestorDTO
    {
        public string NomeGestor { get; set; }
        public string CodGestor { get; set; }
    }
}