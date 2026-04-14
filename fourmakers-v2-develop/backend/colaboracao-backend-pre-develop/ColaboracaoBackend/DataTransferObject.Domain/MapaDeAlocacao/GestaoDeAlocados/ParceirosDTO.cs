using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados
{
    // ──────────────── Parceiro ────────────────
    public class ParceiroDTO
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }

        [JsonPropertyName("codigoInternoColaborador")]
        [Description("Chave estrangeira para tb_colaborador.codigo_interno_colaborador")]
        public string CodigoInternoColaborador { get; set; }

        [JsonPropertyName("nomeColaborador")]
        public string NomeColaborador { get; set; }

        [JsonPropertyName("nomeParceiro")]
        public string NomeParceiro { get; set; }

        [JsonPropertyName("descricaoCurta")]
        public string DescricaoCurta { get; set; }

        [JsonPropertyName("descricaoLonga")]
        public string DescricaoLonga { get; set; }

        [JsonPropertyName("tipoParceria")]
        public string TipoParceria { get; set; }

        [JsonPropertyName("avaliacao")]
        public double? Avaliacao { get; set; }

        [JsonPropertyName("urlSite")]
        public string UrlSite { get; set; }

        [JsonPropertyName("urlLinkedin")]
        public string UrlLinkedin { get; set; }

        [JsonPropertyName("urlLogo")]
        public string UrlLogo { get; set; }

        [JsonPropertyName("urlNda")]
        public string UrlNda { get; set; }

        [JsonPropertyName("urlContrato")]
        public string UrlContrato { get; set; }

        [JsonPropertyName("urlAditivos")]
        public string UrlAditivos { get; set; }

        [JsonPropertyName("maisInfo")]
        public string MaisInfo { get; set; }

        [JsonPropertyName("dataCadastro")]
        public string DataCadastro { get; set; }

        [JsonPropertyName("dataAtualizacao")]
        public string DataAtualizacao { get; set; }

        [JsonPropertyName("parceirosCategoria")]
        public List<ParceiroCategoriaDTO> ParceirosCategoria { get; set; }

        [JsonPropertyName("parceirosContato")]
        public List<ParceiroContatoDTO> ParceirosContato { get; set; }

        [JsonPropertyName("parceirosGestaoContrato")]
        //public ParceiroGestaoContratoDTO ParceirosGestaoContrato { get; set; }
        public List<ParceiroGestaoContratoDTO> ParceirosGestaoContrato { get; set; } = new();

        [JsonPropertyName("codigoColaboradorUltimaAtualizacao")]
        public string CodigoColaboradorUltimaAtualizacao { get; set; }

        [JsonPropertyName("nomeColaboradorUltimaAtualizacao")]
        public string NomeColaboradorUltimaAtualizacao { get; set; }

        [JsonPropertyName("orgId")]
        public int OrgId { get; set; }

        [JsonPropertyName("unidade")]
        public string Unidade { get; set; }
    }
    public class ParceiroParamDTO
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }

        [JsonPropertyName("codigoInternoColaborador")]
        [Description("Chave estrangeira para tb_colaborador.codigo_interno_colaborador")]
        public string CodigoInternoColaborador { get; set; }

        [JsonPropertyName("nomeColaborador")]
        public string NomeColaborador { get; set; }

        [JsonPropertyName("nomeParceiro")]
        public string NomeParceiro { get; set; }

        [JsonPropertyName("descricaoCurta")]
        public string DescricaoCurta { get; set; }

        [JsonPropertyName("descricaoLonga")]
        public string DescricaoLonga { get; set; }

        [JsonPropertyName("tipoParceria")]
        public string TipoParceria { get; set; }

        [JsonPropertyName("avaliacao")]
        public double? Avaliacao { get; set; }

        [JsonPropertyName("urlSite")]
        public string UrlSite { get; set; }

        [JsonPropertyName("urlLinkedin")]
        public string UrlLinkedin { get; set; }

        [JsonPropertyName("urlLogo")]
        public string UrlLogo { get; set; }

        [JsonPropertyName("urlNda")]
        public string UrlNda { get; set; }

        [JsonPropertyName("urlContrato")]
        public string UrlContrato { get; set; }

        [JsonPropertyName("urlAditivos")]
        public string UrlAditivos { get; set; }

        [JsonPropertyName("maisInfo")]
        public string MaisInfo { get; set; }

        [JsonPropertyName("dataCadastro")]
        public string DataCadastro { get; set; }

        [JsonPropertyName("dataAtualizacao")]
        public string DataAtualizacao { get; set; }

        [JsonPropertyName("parceirosCategoria")]
        public List<ParceiroCategoriaDTO> ParceirosCategoria { get; set; }

        [JsonPropertyName("parceirosContato")]
        public List<ParceiroContatoDTO> ParceirosContato { get; set; }

        [JsonPropertyName("codigoColaboradorUltimaAtualizacao")]
        public string CodigoColaboradorUltimaAtualizacao { get; set; }

        [JsonPropertyName("nomeColaboradorUltimaAtualizacao")]
        public string NomeColaboradorUltimaAtualizacao { get; set; }

        [JsonPropertyName("orgId")]
        public int OrgId { get; set; }

        [JsonPropertyName("unidade")]
        public string Unidade { get; set; }
    }

    public class ParceiroParam
    {
        [JsonPropertyName("parceiroId")]
        public string ParceiroId { get; set; }

        [Required, JsonPropertyName("codigoInternoColaborador")]
        public string CodigoInternoColaborador { get; set; }

        [Required, JsonPropertyName("nomeParceiro")]
        public string NomeParceiro { get; set; }

        [Required, JsonPropertyName("tipoParceria")]
        public string TipoParceria { get; set; }

        // Demais campos opcionais
        [JsonPropertyName("descricaoCurta")]
        public string DescricaoCurta { get; set; }

        [JsonPropertyName("descricaoLonga")]
        public string DescricaoLonga { get; set; }

        [JsonPropertyName("avaliacao")]
        public double? Avaliacao { get; set; }

        [JsonPropertyName("urlSite")]
        public string UrlSite { get; set; }

        [JsonPropertyName("urlLinkedin")]
        public string UrlLinkedin { get; set; }

        [JsonPropertyName("urlLogo")]
        public string UrlLogo { get; set; }

        [JsonPropertyName("urlNda")]
        public string UrlNda { get; set; }

        [JsonPropertyName("urlContrato")]
        public string UrlContrato { get; set; }

        [JsonPropertyName("urlAditivos")]
        public string UrlAditivos { get; set; }

        [JsonPropertyName("maisInfo")]
        public string MaisInfo { get; set; }

        [JsonPropertyName("orgId")]
        public int OrgId { get; set; }

        [JsonPropertyName("unidade")]
        public string Unidade { get; set; }

        [JsonPropertyName("parceirosCategoria")]
        public List<string> ParceirosCategoria { get; set; }

        [JsonPropertyName("parceirosContato")]
        public List<ParceiroContatoParam> ParceirosContato { get; set; }

        [JsonPropertyName("parceirosGestaoContrato")]
        public ParceiroGestaoContratoParam ParceirosGestaoContrato { get; set; }

    }

    public class ParceiroIDParam
    {
        [JsonPropertyName("parceiroId")]
        public string ParceiroId { get; set; }

        [Required, JsonPropertyName("codigoInternoColaborador")]
        public string CodigoInternoColaborador { get; set; }

        [Required, JsonPropertyName("nomeParceiro")]
        public string NomeParceiro { get; set; }

        [Required, JsonPropertyName("tipoParceria")]
        public string TipoParceria { get; set; }

        // Demais campos opcionais
        [JsonPropertyName("descricaoCurta")]
        public string DescricaoCurta { get; set; }

        [JsonPropertyName("descricaoLonga")]
        public string DescricaoLonga { get; set; }

        [JsonPropertyName("avaliacao")]
        public double? Avaliacao { get; set; }

        [JsonPropertyName("urlSite")]
        public string UrlSite { get; set; }

        [JsonPropertyName("urlLinkedin")]
        public string UrlLinkedin { get; set; }

        [JsonPropertyName("urlLogo")]
        public string UrlLogo { get; set; }

        [JsonPropertyName("urlNda")]
        public string UrlNda { get; set; }

        [JsonPropertyName("urlContrato")]
        public string UrlContrato { get; set; }

        [JsonPropertyName("urlAditivos")]
        public string UrlAditivos { get; set; }

        [JsonPropertyName("maisInfo")]
        public string MaisInfo { get; set; }

        [JsonPropertyName("orgId")]
        public int OrgId { get; set; }

        [JsonPropertyName("unidade")]
        public string Unidade { get; set; }

        [JsonPropertyName("parceirosCategoria")]
        public List<string> ParceirosCategoria { get; set; }

        [JsonPropertyName("parceirosContato")]
        public List<ParceiroContatoParam> ParceirosContato { get; set; }

      }

    // ──────────────── Categoria ────────────────
    public class ParceiroCategoriaDTO
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }

        [JsonPropertyName("parceiroId")]
        public string ParceiroID { get; set; }

        [JsonPropertyName("categoria")]
        public string Categoria { get; set; }
    }


    // ──────────────── Contato ────────────────
    public class ParceiroContatoDTO
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }

        [JsonPropertyName("parceiroId")]
        public string ParceiroID { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("telefone")]
        public string Telefone { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public class ParceiroContatoParam
    {


        [Required, JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("telefone")]
        public string Telefone { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    // ──────────────── Gestão de Contratos ────────────────
    public class ParceiroGestaoContratoDTO
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }

        [JsonPropertyName("parceiroId")]
        public string ParceiroID { get; set; }

        //[JsonPropertyName("unidade")]
        //public string Unidade { get; set; }

        [JsonPropertyName("contratoAssinado")]
        public bool? ContratoAssinado { get; set; }

        [JsonPropertyName("inicioContrato")]
        public string InicioContrato { get; set; }

        [JsonPropertyName("fimContrato")]
        public string FimContrato { get; set; }

        [JsonPropertyName("clausulaPenalidade")]
        public string ClausulaPenalidade { get; set; }

        public int? NumeroPagina { get; set; }

        public decimal? ValorContrato { get; set; }

        public string Contrato { get; set; }

        public string ContratoAnterior { get; set; }

        public string CotacaoRelacionada { get; set; }

        public string Status { get; set; }

        public string NecessidadeAdicional { get; set; }

        public string PlataformaDigital { get; set; }

        public string ReajusteAnual { get; set; }

        public bool? Renovado { get; set; }

        public string UrlAnexo { get; set; }
        public List<string> EmailsNotificacao { get; set; } = new();
    }

    public class ParceiroGestaoContratoParam
    {
        //[Required, JsonPropertyName("unidade")]
        //public string Unidade { get; set; }
        [JsonPropertyName("id")]
        public string ID { get; set; }

        [JsonPropertyName("contratoAssinado")]
        public bool? ContratoAssinado { get; set; }

        [JsonPropertyName("inicioContrato")]
        public DateTime InicioContrato { get; set; }

        [JsonPropertyName("fimContrato")]
        public DateTime FimContrato { get; set; }

        [JsonPropertyName("clausulaPenalidade")]
        public string ClausulaPenalidade { get; set; }

        public int? NumeroPagina { get; set; }

        public decimal? ValorContrato { get; set; }

        public string Contrato { get; set; }

        public string ContratoAnterior { get; set; }

        public string CotacaoRelacionada { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EnumStatusContrato? Status { get; set; }

        public string NecessidadeAdicional { get; set; }

        public string PlataformaDigital { get; set; }

        public string ReajusteAnual { get; set; }

        public bool? Renovado { get; set; }
        public string UrlAnexo { get; set; }
        public List<string> EmailsNotificacao { get; set; } = new();
    }

    public class EmailContratoDTO
    {
        public string Email { get; set; }
        public string GestaoContratoID { get; set; }
    }

    public class ParceiroGestaoContratoParceiroIDParam : ParceiroGestaoContratoParam
    {
        [JsonPropertyName("parceiroId")]
        public string ParceiroID { get; set; }

    }

    // ──────────────── Tipo de Arquivo ────────────────
    public class ParceiroArchiveTypeDTO
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }

        [JsonPropertyName("arquivoTipo")]
        public string ArquivoTipo { get; set; } // DOC, SHEET, IMAGEM, OUTROS
    }

    // Normalmente não precisa de Param porque é tabela de lookup fixa.
    // ──────────────── Arquivo ────────────────
    public class ParceiroArchiveDTO
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }

        [JsonPropertyName("parceiroId")]
        public string ParceiroID { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("arquivoTipoId")]
        public string ArquivoTipoID { get; set; }

        [JsonPropertyName("arquivoOriginId")]
        public string ArquivoOriginID { get; set; }

        //[JsonPropertyName("parceiroGestaoContratoId")]
        //public string ParceiroGestaoContratoId { get; set; }
    }

    public enum EnumStatusContrato
    {
        Andamento = 0,
        Arquivado = 1,
        Completo = 2
    }

    public enum ParceirosArchiveType
    {
        DOC = 1,
        SHEET = 2,
        IMAGEM = 3,
        OUTROS = 4
    }

    public enum ParceirosArchiveOrigin
    {
        LOGO = 1,
        NDA = 2,
        CONTRATO = 3,
        ADTIVO = 4,
        OUTROS = 5
    }

    public class ParceiroArchiveParam
    {
        [Required, JsonPropertyName("parceiroId")]
        public string ParceiroID { get; set; }


        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("arquivoTipoId")]
        public ParceirosArchiveType ArquivoTipoID { get; set; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("arquivoOriginId")]
        public ParceirosArchiveOrigin ArquivoOriginID { get; set; }

        [Required, JsonPropertyName("bytes")]
        public byte[] bytes { get; set; }

        //[Required, JsonPropertyName("parceiroGestaoContratoId")]
        //public string ParceiroGestaoContratoId { get; set; }
    }
    public class ParceiroInserirParam
    {
        [Required, JsonPropertyName("codigoInternoColaborador")]
        public string CodigoInternoColaborador { get; set; }

        [Required, JsonPropertyName("nomeParceiro")]
        public string NomeParceiro { get; set; }

        [Required, JsonPropertyName("tipoParceria")]
        public string TipoParceria { get; set; }

        // Demais campos opcionais
        [JsonPropertyName("descricaoCurta")]
        public string DescricaoCurta { get; set; }

        [JsonPropertyName("descricaoLonga")]
        public string DescricaoLonga { get; set; }

        [JsonPropertyName("avaliacao")]
        public double? Avaliacao { get; set; }

        [JsonPropertyName("urlSite")]
        public string UrlSite { get; set; }

        [JsonPropertyName("urlLinkedin")]
        public string UrlLinkedin { get; set; }

        [JsonPropertyName("urlLogo")]
        public string UrlLogo { get; set; }

        [JsonPropertyName("urlNda")]
        public string UrlNda { get; set; }

        [JsonPropertyName("urlContrato")]
        public string UrlContrato { get; set; }

        [JsonPropertyName("urlAditivos")]
        public string UrlAditivos { get; set; }

        [JsonPropertyName("maisInfo")]
        public string MaisInfo { get; set; }

        [JsonPropertyName("orgId")]
        public int OrgId { get; set; }

        [JsonPropertyName("unidade")]
        public string Unidade { get; set; }

        [JsonPropertyName("parceirosCategoria")]
        public List<string> ParceirosCategoria { get; set; }

        [JsonPropertyName("parceirosContato")]
        public List<ParceiroContatoParam> ParceirosContato { get; set; }

    }
    public class ImportacaoPlanilhaParam
    {
        [JsonPropertyName("caminhoArquivo")]
        public string CaminhoArquivo { get; set; }
    }
}
