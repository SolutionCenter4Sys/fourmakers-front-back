using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Colaborador;
using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Gestor
{
    public class DashboardColaboradorDTO
    {
        public string CodigoInternoColaboradorAvaliado { get; set; }
        public string CodigoColaboradorExterno { get; set; }
        public string NomeCompletoColaboradorAvaliado { get; set; }
        public string Status { get; set; }
        public string Cargo { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public DateTime? DataNascimento { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public string TempoCasa { get; set; }
        /// <summary>Jornada de trabalho (ex.: 44h/mês). Origem: tb_colaborador_org.base_hora_mes.</summary>
        public string JornadaTrabalho { get; set; }
        /// <summary>Tipo de trabalho (ex.: presencial, híbrido, remoto). Origem: tb_colaborador_org.modelo_trabalho.</summary>
        public string TipoTrabalho { get; set; }
        /// <summary>Regime de trabalho (ex.: CLT, PJ). Origem: tb_modelo_contratacao_org.descricao.</summary>
        public string RegimeTrabalho { get; set; }
        /// <summary>Modalidade de contratação (ex.: CLT, PJ). Origem: tb_colaborador_org.modelo_contratacao.</summary>
        public string ModalidadeContratacao { get; set; }
        public DashboardFeedbacksDTO DashboardFeedbacks { get; set; }
        public DashboardOneOnOneDTO DashboardOneOnOne { get; set; }
    }

    public class DashboardFeedbacksDTO
    {
        public int QtdVistos { get; set; }
        public int QtdNaoVistos { get; set; }
        public List<FeedbackDetalheDTO> Feedbacks { get; set; }
    }

    public class FeedbackDetalheDTO
    {
        public string Id { get; set; }
        public string CodigoInternoColaboradorSuperior { get; set; }
        public string NomeCompletoColaboradorSuperior { get; set; }
        public DateTime? DataReuniao { get; set; }
        public string DescricaoContinuar { get; set; }
        public string DescricaoComecar { get; set; }
        public string DescricaoParar { get; set; }
        public string DescricaoObservacoesGerais { get; set; }
        public bool? VisualizadoPeloColaborador { get; set; }
        public DateTime? DataVisualizadoColaborador { get; set; }
    }

    public class DashboardOneOnOneDTO
    {
        public int QtdVistos { get; set; }
        public int QtdNaoVistos { get; set; }
        public List<ColaboradorPautaSugeridaDTO> PautasSugeridas { get; set; }
        public List<OneOnOneDetalheDTO> OneOnOnes { get; set; }
    }

    public class OneOnOneDetalheDTO
    {
        public string Id { get; set; }
        public string CodigoInternoColaboradorSuperior { get; set; }
        public string NomeCompletoColaboradorSuperior { get; set; }
        public DateTime? DataReuniao { get; set; }
        public string DescricaoAnotacoes { get; set; }
        public bool? RegistroCritico { get; set; }
        public bool? VisualizadoPeloColaborador { get; set; }
        public DateTime? DataVisualizadoColaborador { get; set; }
    }
}
