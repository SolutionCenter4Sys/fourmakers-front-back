using DataTransferObject.Domain.Colaborador;
using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Candidato
{
    public class CandidatoDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("dataEstagioProcesso")]
        public DateTime DataEstagioProcesso { get; set; }

        [JsonPropertyName("descricaoEstagioProcesso")]
        public string DescricaoEstagioProcesso { get; set; }

        [JsonPropertyName("pathCurriculo")]
        public string PathCurriculo { get; set; }

        [JsonPropertyName("pretencaoSalarial")]
        public double? PretencaoSalarial { get; set; }

        [JsonPropertyName("cargoAtualUltimo")]
        public string CargoAtualUltimo { get; set; }
        [JsonPropertyName("salarioAtualUltimo")]
        public decimal? SalarioAtualUltimo { get; set; }
        [JsonPropertyName("tipoContratoAtualUltimo")]
        public string TipoContratoAtualUltimo { get; set; }

        [JsonPropertyName("modalidadeAtualUltima")]
        public ModalidadeEnum ModalidadeAtualUltima { get; set; }
        [JsonPropertyName("aceitaSugestoes")]
        public bool? AceitaSugestoes { get; set; }

        [JsonPropertyName("ativo")]
        public int Ativo { get; set; }

        [JsonPropertyName("codigoInternoColaborador")]
        public string CodigoInternoColaborador { get; set; }

        [JsonPropertyName("estagioProcessoSeletivoId")]
        public int EstagioProcessoSeletivoId { get; set; }

        [JsonPropertyName("dataCriacao")]
        public DateTime? DataCriacao { get; set; }

        [JsonPropertyName("dataAlteracao")]
        public DateTime? DataAlteracao { get; set; }

        [JsonIgnore]
        public string Cpf { get; set; }

        [JsonIgnore]
        public string NomeCompleto { get; set; }
    }

    //------------CandidatoTemplateEmail
    public class CandidatoTemplateEmailParamDTO
    {
        public int OrgId { get; set; }
        public string EmailsCC { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
    }

    public class CandidatoTemplateEmailResponseDTO
    {
        public int Id { get; set; }
        public int OrgId { get; set; }
        public string EmailsCC { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }

    public class TemplateOrgEmailResponseDTO
    {
        public string EmailsCC { get; set; }
        public string Descricao { get; set; }
        public string Titulo { get; set; }

    }

    //--- Métricas Vagas
    public class DashboardBigNumbers
    {
        public int? EmFoco { get; set; }
        public int? EmAndamento { get; set; }
        public int? Contratacoes { get; set; }
        public int? EntrevistaCliente { get; set; }
        public int? VagasCanceladas { get; set; }
        public int? VagasPerdidas { get; set; }
        public decimal TempoMedioDiasRecrutamento { get; set; }
        public decimal PercentagemEntrevistaEmAndamento { get; set; }
    }

    public class DashboardBigNumbersParam
    {
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string? CodigoGestor { get; set; }
        public string? CodigoCliente { get; set; }
        public string? CodigoRecrutador { get; set; }
    }
    public class DashboardNovosCandidatosParam
    {
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
    }


    public class DashboardVagasEmFocoResponse
    {
        public string Cliente { get; set; }
        public long CodVaga { get; set; }
        public string Vaga { get; set; }
        public string Status { get; set; }
        public string Responsavel { get; set; }
        public DateTime? UltimaMovimentacao { get; set; }
        public int? TempoNaEtapa { get; set; }
        public string SLA { get; set; }
    }

    public class DashboardFunilVagasResponse
    {
        public int? EmFoco { get; set; }
        public int? EntrevistaInicial { get; set; }
        public int? AplicacaoTestes { get; set; }
        public int? EntrevistaTecnica { get; set; }
        public int? EntrevistaComCliente { get; set; }
        public int? CartaOferta { get; set; }
        public int? ProcurandoCandidatos { get; set; }
    }

    public class DashboardPerdidasMotivoResponse
    {
        public int? Total { get; set; }
        public string Id { get; set; }
        public string Motivo { get; set; }
    }
    
    public class DashboardAquisicaoCandidatos
    {
        public int? Total { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string Origem { get; set; }
        public int OrgId { get; set; }
    }
    public class DashboardAquisicaoCandidatosResponse
    {
        public int? Total { get; set; }
        public string Origem { get; set; }
        public int OrgId { get; set; }
    }
    /// <summary>
    /// Linha bruta do dashboard de novos candidatos (banco de talentos) para agrupamento por origem.
    /// </summary>
    public class DashboardNovosCandidatosPorOrigemItem
    {
        public int? Total { get; set; }
        public int OrgId { get; set; }
        public string TipoCadastro { get; set; }
        public int? FormaCadastro { get; set; }
    }

    public class RecrutadorListagemResponse
    {
        public string CodigoRecrutador { get; set; }
        public string? NomeRecrutador { get; set; }
    }

}
