using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text.Json.Serialization;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.ClienteOrg;
using DataTransferObject.Domain.Projeto.GestorExterno;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados
{
    public enum TipoInteracao
    {
        [Description("Reuniao")] Reuniao = 1,
        [Description("call")] Call = 2,
        [Description("whatsApp")] WhatsApp = 3,
        [Description("email")] Email = 4,
        [Description("presencial")] Presencial = 5
    }
    public class IntervaloBuscaAgendasParametros
    {
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
    }
    public class EncontrosDTO
    {
        public int Id { get; set; }

        [JsonPropertyName("agendaId")]
        public int AgendaId { get; set; }

        [JsonPropertyName("tituloInteracao")]
        public string TituloInteracao { get; set; }

        [JsonPropertyName("descricaoInteracao")]
        public string? DescricaoInteracao { get; set; }

        [JsonPropertyName("encontroAi")]
        public EncontroAi EncontroAi { get; set; }

        [JsonPropertyName("resumoInteracao")]
        public string? ResumoInteracao { get; set; }
        public DateTime? DataRequisicao { get; set; }
    }

    public class InteracoesDTO
    {
        public int Id { get; set; }
        public int AgendaId { get; set; }

        public string CodigoInternoColaborador { get; set; }
        
        public string TituloInteracao { get; set; }

        public string? DescricaoInteracao { get; set; }

        public string? ResumoInteracao { get; set; }
    }

    public class InteracoesComCategoriaDTO
    {
        [JsonPropertyName("agendaId")]
        public int AgendaId { get; set; }

        [JsonPropertyName("tituloInteracao")]
        public string TituloInteracao { get; set; }

        [JsonPropertyName("descricaoInteracao")]
        public string? DescricaoInteracao { get; set; }

        [JsonPropertyName("encontroAi")]
        public EncontroAi EncontroAi { get; set; }

        [JsonPropertyName("resumoInteracao")]
        public string? ResumoInteracao { get; set; }

        //[JsonPropertyName("categoriaAssuntoId")]
        //public int? CategoriaAssuntoId { get; set; }

        //[JsonPropertyName("subCategoriaAssuntoId")]
        //public int? SubCategoriaAssuntoId { get; set; }
    }

    public class InteracoesComCategoriaAtualizacaoDTO : InteracoesComCategoriaDTO
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }
    }

    public class EncontrosResponse : EncontrosDTO
    {
        // --- colunas da própria tb_interacoes ---------------------------------
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("codigoInternoColaborador")]
        public string? CodigoInternoColaborador { get; set; }

        [JsonPropertyName("nomeCompletoColaboradorCriador")]
        public string? NomeCompletoColaboradorCriador { get; set; }

        //[JsonPropertyName("dataRequisicao")]
        //public DateTime? DataRequisicao { get; set; }


        [JsonPropertyName("arquivos")]
        public List<ArquivoEncontroDto>? Arquivos { get; set; }

        public List<InteracaoCategoriaSubResponseDTO> Categorias { get; set; }
    }

    public class EncontrosParam : EncontrosDTO
    {

    }
    public class EncontroAiRelacional : EncontroAi
    {
        public int InteracaoId { get; set; }
    }
    public class ArquivoEncontroDtoRelacional : ArquivoEncontroDto
    {
        public int InteracaoId { get; set; }
    }

    public class EncontroAiPassosRelacional : EncontroAiPassos 
    { 
    }
    public class ArquivoRelacional : ArquivoEncontroDto 
    { 
        public int InteracaoId { get; set; } 
    }

    public class AgendaEncontroParam : AgendaEncontroDto
    {
        [JsonPropertyName("codigoCliente")]
        public string CodigoCliente { get; set; }

        [JsonPropertyName("codigosGestoresExternos")]
        public List<string>? CodigosGestoresExternos { get; set; }

        [JsonPropertyName("codigosColaboradores")]
        public List<string>? CodigosColaboradores { get; set; }

        [JsonPropertyName("participantesExternos")]
        public List<ParticipanteExterno>? ParticipantesExternos { get; set; }

        [JsonPropertyName("agendaPaiId")]
        public int? AgendaPaiId { get; set; }
    }

    public class AgendaEncontroResult : AgendaEncontroDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("codColaboradorCriador")]
        public string codColaboradorCriador { get; set; }

        [JsonPropertyName("nomeCompletoColaboradorCriador")]
        public string NomeCompletoColaboradorCriador { get; set; }

        [JsonPropertyName("encontros")]
        public List<EncontrosResponse> Encontros { get; set; }

        [JsonPropertyName("dataCriacao")]
        public DateTime? DataCriacao { get; set; }

        [JsonPropertyName("dataAtualizacao")]
        public DateTime? DataAtualizacao { get; set; }

        public StatusSolicitacaoParticipante? StatusSolicitacao { get; set; }


        // --- relações --------------------------------------------------------

        public class StatusSolicitacaoParticipanteRel
        {
            public int AgendaId { get; set; }
            public int Status { get; set; }
        }
        public class EncontroAgendaResult
        {
            public int Id { get; set; }
            public int AgendaId { get; set; }
            public DateTime DataEncontro { get; set; }
            public TimeSpan DataInicio { get; set; }
            public TimeSpan DataFim { get; set; }
        }

        [JsonIgnore]
        [JsonPropertyName("codigoCliente")]
        public string? CodigoCliente { get; set; }

        [JsonPropertyName("cliente")]
        public ClienteOrgDaGestaoAlocadosResult? Cliente { get; set; }

        [JsonPropertyName("gestoresExternos")]
        public List<GestorExternoResult>? GestoresExternos { get; set; }

        [JsonPropertyName("colaboradores")]
        public List<ColaboradorAgenda>? Colaboradores { get; set; }

        [JsonPropertyName("participantes")]
        public List<ParticipanteConvidadoDto>? Participantes { get; set; }

        [JsonPropertyName("participantesExterno")]
        public List<ParticipanteExterno>? ParticipantesExterno { get; set; }

        [JsonPropertyName("statusSolicitacaoParticipante")]
        public StatusSolicitacaoParticipante? StatusSolicitacaoParticipante { get; set; }
        [JsonPropertyName("participacaoUsuarioLogado")]
        public ParticipacaoUsuarioConvidado? ParticipacaoUsuarioLogado { get; set; }

        [JsonPropertyName("agendaPaiId")]
        public int? AgendaPaiId { get; set; }
    }

    public enum StatusSolicitacaoParticipante
    {
        Pendente = 0,
        Aprovado = 1,
        Recusado = 2
    }

    public class StatusReadDTO
    {
        public int AgendaId { get; set; }
        public int? Status { get; set; }
    }

    public class ParticipacaoUsuarioLogado
    {
        public int AgendaId { get; set; }

        [JsonPropertyName("confirmado")]
        public StatusSolicitacaoParticipante? Confirmado { get; set; }
        [JsonPropertyName("dataConfirmacao")]
        public DateTime? DataConfirmacao { get; set; }
    }

    public class ParticipacaoUsuarioConvidado
    {
        public int AgendaId { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("dataResposta")]
        public DateTime? DataResposta { get; set; }
    }

    public class AgendaEncontroDto
    {
        [JsonPropertyName("tipoInteracao")]
        public TipoInteracao TipoInteracao { get; set; }

        [JsonPropertyName("graphEventId")]
        public string? GraphEventId { get; set; }

        [JsonPropertyName("dataAgendada")]
        public DateTime? DataAgendada { get; set; }

        [JsonPropertyName("dataInicio")]
        public DateTime? DataInicio { get; set; }

        [JsonPropertyName("dataFim")]
        public DateTime? DataFim { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Pendente";

        [JsonPropertyName("localizacao")]
        public string? Localizacao { get; set; }

        [JsonPropertyName("linkReuniao")]
        public string? LinkReuniao { get; set; }

        [JsonPropertyName("quantidadeParticipantes")]
        public int QuantidadeParticipantes { get; set; }

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; }

        [JsonPropertyName("descricao")]
        public string? Descricao { get; set; }

        [JsonPropertyName("agendaObjetivoId")]
        public int? AgendaObjetivoId { get; set; }

    }

    public class ColaboradorAgenda
    {
        [JsonPropertyName("codInternoColaborador")]
        public string CodInternoColaborador { get; set; }

        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }

    public class ParticipanteExterno
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }
    }

    public class ParticipanteEncontroDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("codigoGestorExterno")]
        public string? CodigoGestorExterno { get; set; }

        [JsonPropertyName("codigoColaborador")]
        public string? CodigoColaborador { get; set; }

        [JsonPropertyName("Nome")]
        public string? Nome { get; set; }

        [JsonPropertyName("confirmado")]
        public bool? Confirmado { get; set; }

        [JsonPropertyName("dataConfirmacao")]
        public DateTime? DataConfirmacao { get; set; }

        [JsonPropertyName("interessado")]
        public bool? Interessado { get; set; }

        [JsonPropertyName("dataInteresse")]
        public DateTime? DataInteresse { get; set; }
    }

    public class ParticipanteConvidadoDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("codigoGestorExterno")]
        public string? CodigoGestorExterno { get; set; }

        [JsonPropertyName("codigoColaborador")]
        public string? CodigoColaborador { get; set; }

        [JsonPropertyName("Nome")]
        public string? Nome { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("dataConvite")]
        public DateTime? DataConvite { get; set; }

        [JsonPropertyName("dataResposta")]
        public DateTime? DataResposta { get; set; }

    }

    public class ArquivoEncontroDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("dataArquivo")]
        public DateTime? DataArquivo { get; set; }

        [JsonPropertyName("linkAudioInteracao")]
        public string? LinkAudioInteracao { get; set; }

        [JsonPropertyName("linkImagemInteracao")]
        public string? LinkImagemInteracao { get; set; }

        [JsonPropertyName("transcricao")]
        public string? Transcricao { get; set; }
    }

    public class ArquivoEncontroParam
    {
        [JsonPropertyName("encontroId")]
        public int EncontroId { get; set; }

        [JsonPropertyName("bytes")]
        public byte[] bytes { get; set; }

        [JsonPropertyName("nomeArquivo")]
        public string NomeArquivo { get; set; }

        [JsonPropertyName("tipoArquivo")]
        public string TipoArquivo { get; set; }

        [JsonPropertyName("transcricao")]
        public string Transcricao { get; set; }
    }
    public class EncontroAi
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("encontroId")]
        public int EncontroId { get; set; }

        [JsonPropertyName("resumo")]
        public string Resumo { get; set; }

        [JsonPropertyName("dataGerada")]
        public DateTime? DataGerada { get; set; }

        [JsonPropertyName("passos")]
        public List<EncontroAiPassos>? Passos { get; set; }
    }

    public class EncontroAiPassos
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("encontroAiId")]
        public int EncontroAiId { get; set; }

        [JsonPropertyName("texto")]
        public string Texto { get; set; }

        [JsonPropertyName("codigoColaborador")]
        public string? CodigoColaborador { get; set; }

        [JsonPropertyName("nomeColaborador")]
        public string? NomeColaborador { get; set; }

        [JsonPropertyName("dataLimite")]
        public DateTime? DataLimite { get; set; }

        [JsonPropertyName("statusAcoesId")]
        public int StatusAcoesId { get; set; }
        
        [JsonPropertyName("comentariosAcoes")]
        public List<ComentariosAcoesResponseDTO>? ComentariosAcoes { get; set; }
    }

    public class ConvidarParticipantesAgendaParam
    {
        [JsonPropertyName("agendaId")]
        public int AgendaId { get; set; }

        [JsonPropertyName("codigosGestoresExternos")]
        public List<string> CodigosGestoresExternos { get; set; }

        [JsonPropertyName("codigosInternoColaboradores")]
        public List<string> CodigosInternoColaboradores { get; set; }

        [JsonPropertyName("mensagem")]
        public string Mensagem { get; set; }
    }
    public class AtualizarIteracoesAcoesParam
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("interacaoAiIid")]
        public int InteracaoAiIid { get; set; }

        [JsonPropertyName("statusAcoesId")]
        public int StatusAcoesId { get; set; }

        //[JsonPropertyName("comentariosAcao")]
        //public string ComentariosAcao { get; set; }

    }
    public class InteracaoAcoesResponseDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("interacaoAiId")]
        public int InteracaoAiId { get; set; }

        [JsonPropertyName("codigoColaborador")]
        public string? CodigoColaborador { get; set; }

        [JsonPropertyName("nomeColaborador")]
        public string? NomeColaborador { get; set; }

        [JsonPropertyName("texto")]
        public string Texto { get; set; }

        [JsonPropertyName("dataLimite")]
        public DateTime? DataLimite { get; set; }

        [JsonPropertyName("statusAcoesId")]
        public int StatusAcoesId { get; set; }

        [JsonPropertyName("descricaoStatusAcao")]
        public string DescricaoStatusAcao { get; set; }

        //[JsonPropertyName("descricaoComentario")]
        //public string DescricaoComentario { get; set; }
    }
    public class EncontroAiParamInclusao
    {

        [JsonPropertyName("interacaoId")]
        public int InteracaoId { get; set; }

        [JsonPropertyName("resumo")]
        public string Resumo { get; set; }

    }
    public class EncontroAiParamAtualizacao
    {

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("resumo")]
        public string Resumo { get; set; }

    }

    public class InteracaiAIResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("interacaoId")]
        public int InteracaoId { get; set; }

        [JsonPropertyName("resumo")]
        public string Resumo { get; set; }

        [JsonPropertyName("dataGerada")]
        public DateTime? DataGerada { get; set; }

    }
    public class ComentariosAcoesParamDTO
    {
        [JsonPropertyName("interacaoAcoesId")]
        public int InteracaoAcoesId { get; set; }

        [JsonPropertyName("codInternoColaborador")]
        public string CodInternoColaborador { get; set; }

        [JsonPropertyName("nomeColaborador")]
        public string? NomeColaborador { get; set; }

        [JsonPropertyName("data")]
        public DateTime? Data { get; set; }

        [JsonPropertyName("comentario")]
        public string? Comentario { get; set; }
    }

    public class ComentariosAcoesResponseDTO : ComentariosAcoesParamDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
    }

    //---------DTO CATEGORIA--------//

    public class CategoriaAssuntoDTO
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        //public int InteracoesId { get; set; }
    }

    public class SubcategoriaAssuntoDTO
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public int CategoriaAssuntoId { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Ativo { get; set; }

    }
    public class CategoriaAssuntoComSubDTO
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public List<SubcategoriaAssuntoDTO> Subcategorias { get; set; } = new();
    }

    public class InteracoesCategoriaSubDTO
    {
        public int Id { get; set; }
        public int InteracaoId { get; set; }
        public int CategoriaAssuntoId { get; set; }
        public int SubCategoriaAssuntoId { get; set; }
    }

    public class InteracoesCategoriaSubResponseDTO : InteracoesCategoriaSubDTO
    {
        public string DescricaoCategoria { get; set; }
        public string DescricaoSubCategoria { get; set; }
        //public int CategoriaAssuntoId { get; set; }
        //public int SubCategoriaAssuntoId { get; set; }
    }


    public class CategoriaInsercaoParamDTO
    {
        public int? InteracaoId { get; set; }
        public int? CategoriaAssuntoId { get; set; }
        public int? SubCategoriaAssuntoId { get; set; }
    }

    public class CategoriaAtualizacaoParamDTO
    {
        public long? InteracaoCategoriaId { get; set; }
        public int? CategoriaAssuntoId { get; set; }
        public int? SubCategoriaAssuntoId { get; set; }
    }

    //---------DTO RELATORIO--------//

    public class InteracaoCategoriaSubResponseDTO
    {
        public int? InteracaoId { get; set; }
        public int? InteracaoCategoriaId { get; set; }
        public int? CategoriaAssuntoId { get; set; }
        public int? SubCategoriaAssuntoId { get; set; }
        public DateTime? DataCriacao { get; set; }
        public bool Ativo { get; set; }
    }
    public class PaginadoDTO<T>
    {
        public int Pagina { get; set; }
        public int Limite { get; set; }
        public int TotalRegistros { get; set; }
        public List<T> Dados { get; set; }
    }

    public class SubCategoriaDTO
    {
        public int SubCategoriaId { get; set; }
        public string SubCategoriaNome { get; set; }

    }

    public class CategoriaDTO
    {
        public int CategoriaId { get; set; }
        public string CategoriaNome { get; set; }

        public List<SubCategoriaDTO> SubCategorias { get; set; } = new();
    }

    public class InteracaoDTO
    {
        public int InteracaoId { get; set; }
        public string TituloInteracao { get; set; }
        public List<CategoriaDTO> Categorias { get; set; } = new();
    }

    public class AgendaHierarquicaDTO
    {
        public int AgendaId { get; set; }
        public string Titulo { get; set; }
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public int? QuantidadeParticipante { get; set; }
        public DateTime? DataAgendado { get; set; }
        public List<InteracaoDTO> Interacoes { get; set; } = new();
    }

    //----RESPONSE

    // ============================================================================
    // NOVO DTO - NÃO IMPACTA EncontroAiPassos EXISTENTE
    // ============================================================================
    public class EncontroAiPassosComComentarios : EncontroAiPassos
    {
        [JsonPropertyName("comentarios")]
        public List<ComentariosAcoesResponseDTO>? Comentarios { get; set; }
    }

    // ============================================================================
    // OU CRIAR DTO TOTALMENTE SEPARADO (RECOMENDADO)
    // ============================================================================
    public class EncontroAiPassosDetalhado
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("encontroAiId")]
        public int EncontroAiId { get; set; }

        [JsonPropertyName("texto")]
        public string Texto { get; set; }

        [JsonPropertyName("codigoColaborador")]
        public string? CodigoColaborador { get; set; }

        [JsonPropertyName("nomeColaborador")]
        public string? NomeColaborador { get; set; }

        [JsonPropertyName("dataLimite")]
        public DateTime? DataLimite { get; set; }

        [JsonPropertyName("statusAcoesId")]
        public int StatusAcoesId { get; set; }

        [JsonPropertyName("comentarios")]
        public List<ComentariosAcoesResponseDTO>? Comentarios { get; set; }
    }

    // ============================================================================
    // ATUALIZAR EncontroAi PARA USAR O NOVO DTO
    // ============================================================================
    public class EncontroAiDetalhado
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("encontroId")]
        public int EncontroId { get; set; }

        [JsonPropertyName("resumo")]
        public string Resumo { get; set; }

        [JsonPropertyName("dataGerada")]
        public DateTime? DataGerada { get; set; }

        [JsonPropertyName("passos")]
        public List<EncontroAiPassosDetalhado>? Passos { get; set; }
    }

    // ============================================================================
    // NOVO EncontrosResponse COM DTO DETALHADO
    // ============================================================================
    public class EncontrosResponseDetalhado : EncontrosDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("codigoInternoColaborador")]
        public string? CodigoInternoColaborador { get; set; }

        [JsonPropertyName("nomeCompletoColaboradorCriador")]
        public string? NomeCompletoColaboradorCriador { get; set; }

        [JsonPropertyName("dataRequisicao")]
        public DateTime? DataRequisicao { get; set; }

        [JsonPropertyName("arquivos")]
        public List<ArquivoEncontroDto>? Arquivos { get; set; }

        [JsonPropertyName("encontroAi")]
        public new EncontroAiDetalhado? EncontroAi { get; set; }

        [JsonPropertyName("categoriasSubcategorias")]
        public List<CategoriaSubcategoriaDTO>? CategoriasSubcategorias { get; set; }
    }
    public class CategoriaSubcategoriaDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("interacaoId")]
        public int InteracaoId { get; set; }

        [JsonPropertyName("categoriaId")]
        public int CategoriaId { get; set; }

        [JsonPropertyName("categoriaDescricao")]
        public string CategoriaDescricao { get; set; }

        [JsonPropertyName("subcategoriaId")]
        public int SubcategoriaId { get; set; }

        [JsonPropertyName("subcategoriaDescricao")]
        public string SubcategoriaDescricao { get; set; }

        [JsonPropertyName("dataCriacao")]
        public DateTime? DataCriacao { get; set; }

        [JsonPropertyName("ativo")]
        public bool Ativo { get; set; }
    }

    public class InteracaoComCategoriaDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("interacaoId")]
        public int InteracaoId { get; set; }

        [JsonPropertyName("categoriaId")]
        public int CategoriaId { get; set; }

        [JsonPropertyName("categoriaDescricao")]
        public string CategoriaDescricao { get; set; }

        [JsonPropertyName("ativo")]
        public bool Ativo { get; set; }
    }

    public class InteracaoComCategoriaSubDTO
    {
        public int Id { get; set; }
        public int InteracaoId { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaDescricao { get; set; }
        public int SubcategoriaId { get; set; }
        public string SubcategoriaDescricao { get; set; }
        public bool Ativo { get; set; }
    }

    public class AgendaObjetivoDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Ativo { get; set; }
    }


};
