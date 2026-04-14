using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Colaborador
{
    public class ColaboradorDashboardDTO
    {
        public MeuPainelDTO MeuPainel { get; set; }
        public List<RegistroCriticoDTO> RegistrosCriticos { get; set; }
        public ColaboradorDashboardFeedbacksDTO DashboardFeedbacks { get; set; }
        public ColaboradorDashboardOneOnOneDTO DashboardOneOnOne { get; set; }
    }

    public class MeuPainelDTO
    {
        public int QtdFeedbacks { get; set; }
        public int QtdOneOnOne { get; set; }
    }

    public class RegistroCriticoDTO
    {
        public DateTime? DataReuniao { get; set; }
        public string DescricaoAnotacoes { get; set; }
    }

    public class ColaboradorDashboardFeedbacksDTO
    {
        public int QtdVistos { get; set; }
        public int QtdNaoVistos { get; set; }
        public List<ColaboradorFeedbackDetalheDTO> Feedbacks { get; set; }
    }

    public class ColaboradorFeedbackDetalheDTO
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

    public class ColaboradorDashboardOneOnOneDTO
    {
        public int QtdVistos { get; set; }
        public int QtdNaoVistos { get; set; }
        public List<ColaboradorPautaSugeridaDTO> PautasSugeridas { get; set; }
        public List<ColaboradorOneOnOneDetalheDTO> OneOnOnes { get; set; }
    }

    public class ColaboradorPautaSugeridaDTO
    {
        public string Id { get; set; }
        public string DescricaoPautaSugerida { get; set; }
        public DateTime? DataCriacao { get; set; }
        public string CodigoInternoColaboradorCriacao { get; set; }
        public string NomeCompletoColaboradorCriacao { get; set; }
        public string TipoOrigem { get; set; }
        
    }

    public class ColaboradorOneOnOneDetalheDTO
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
