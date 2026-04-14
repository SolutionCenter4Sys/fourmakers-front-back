using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Endereco;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using DataTransferObject.Domain.Vaga;

namespace DataTransferObject.Domain.Candidato
{
    public class ProfissionalDTO
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("rg")]
        public string RG { get; set; }

        [JsonPropertyName("cpf")]
        public string CPF { get; set; }

        [JsonPropertyName("dataNascimento")]
        public DateTime DataNascimento { get; set; }

        [JsonPropertyName("telefone")]
        public string Telefone { get; set; }

        [JsonPropertyName("emailPessoal")]
        public string EmailPessoal { get; set; }

        [JsonPropertyName("tamanhoCamiseta")]
        public string TamanhoCamiseta { get; set; }

        [JsonPropertyName("superiorImediato")]
        public string SuperiorImediato { get; set; }

        [JsonPropertyName("endereco")]
        public EnderecoDTO Endereco { get; set; }

    }
    
    public class VagaAdmissaoDTO
    {
        [JsonPropertyName("idVaga")]
        public int IdVaga { get; set; }

        [JsonPropertyName("cliente")]
        public string Cliente { get; set; }

        [JsonPropertyName("cargo")]
        public string Cargo { get; set; }

        //[JsonPropertyName("stackPrincipal")]
        //public string StackPrincipal { get; set; } // parametro

        //[JsonPropertyName("nivelCargo")]
        //public string NivelCargo { get; set; } // parametro
        
        [JsonPropertyName("modalidadeTrabalho")]
        public string ModalidadeTrabalho { get; set; }

        [JsonPropertyName("tipoVaga")]
        public string TipoVaga { get; set; }

        [JsonPropertyName("endereco")]
        public string Endereco { get; set; }

        [JsonPropertyName("cep")]
        public string CEP { get; set; }

        [JsonPropertyName("dataInicio")]
        public DateTime? DataInicio { get; set; }

        [JsonPropertyName("diretoria")]
        public string Diretoria { get; set; }

        [JsonPropertyName("solicitante")]
        public string Solicitante { get; set; }

        [JsonPropertyName("gestorResponsavel")]
        public string GestorResponsavel { get; set; }

        [JsonPropertyName("analistaResponsavel")]
        public string AnalistaResponsavel { get; set; }

        [JsonPropertyName("unidadeTrabalho")]
        public string UnidadeTrabalho { get; set; }

        [JsonPropertyName("localAlocacao")]
        public string LocalAlocacao { get; set; }

        [JsonPropertyName("areaStaff")]
        public string AreaStaff { get; set; }

        [JsonPropertyName("horarioTrabalho")]
        public string HorarioTrabalho { get; set; }

        [JsonPropertyName("horasFechadas")]
        public bool HorasFechadas { get; set; }
    }
    
    public class BeneficiosDTO
    {
        [JsonPropertyName("salario")]
        public decimal Salario { get; set; }

        [JsonPropertyName("valeRefeicao")]
        public decimal ValeRefeicao { get; set; }

        [JsonPropertyName("valeAlimentacao")]
        public decimal ValeAlimentacao { get; set; }

        [JsonPropertyName("assistenciaMedica")]
        public decimal AssistenciaMedica { get; set; }

        [JsonPropertyName("ajudaDeCusto")]
        public decimal AjudaDeCusto { get; set; }

        [JsonPropertyName("mobilidade")]
        public decimal Mobilidade { get; set; }

        [JsonPropertyName("assistenciaEducacional")]
        public decimal AssistenciaEducacional { get; set; }

        [JsonPropertyName("remuneracaoTotal")]
        public decimal RemuneracaoTotal { get; set; }
        [JsonPropertyName("custoHora")]
        public decimal CustoHora { get; set; }
    }
    
    public class ChecklistInstalacaoDTO
    {
        [JsonPropertyName("nomeProfissional")]
        public string NomeProfissional { get; set; }

        [JsonPropertyName("descricaoMaquina")]
        public string DescricaoMaquina { get; set; }

        [JsonPropertyName("hardware")]
        public string Hardware { get; set; }

        [JsonPropertyName("outrosSoftwares")]
        public string OutrosSoftwares { get; set; }
    }
    
    public class EquipamentosFoursysDTO
    {
        [JsonPropertyName("celular")]
        public bool Celular { get; set; }

        [JsonPropertyName("planoDados")]
        public bool PlanoDados { get; set; }

        [JsonPropertyName("quantidadeMinutosPlanoDados")]
        public int QuantidadeMinutosPlanoDados { get; set; }

        [JsonPropertyName("cartaoVisitas")]
        public bool CartaoVisitas { get; set; }

        [JsonPropertyName("quantidadeCartaoVisitas")]
        public int QuantidadeCartaoVisitas { get; set; }

        [JsonPropertyName("outros")]
        public string Outros { get; set; }
    }
    
    public class AcessoSistemaDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }          

        [JsonPropertyName("nomeSistema")]
        public string NomeSistema { get; set; } 

        [JsonPropertyName("liberado")]
        public bool Liberado { get; set; }
    }
    public class AcessoDiretorioRedeDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }          

        [JsonPropertyName("diretorio")]
        public string Diretorio { get; set; } 

        [JsonPropertyName("leitura")]
        public bool Leitura { get; set; }

        [JsonPropertyName("escrita")]
        public bool Escrita { get; set; }
    }
    
    public class AcessoGrupoEmailDTO
    {
        [JsonPropertyName("todosFoursys")]
        public bool TodosFoursys { get; set; }

        [JsonPropertyName("foursysAlphaville")]
        public bool FoursysAlphaville { get; set; }

        [JsonPropertyName("foursysPaulista")]
        public bool FoursysPaulista { get; set; }

        [JsonPropertyName("foursysCuritiba")]
        public bool FoursysCuritiba { get; set; }

        [JsonPropertyName("grupoEmailContrato")]
        public string GrupoEmailContrato { get; set; }

        [JsonPropertyName("descricaoOutrosGrupos")]
        public string DescricaoOutrosGrupos { get; set; }

        [JsonPropertyName("observacaoAprovador")]
        public string ObservacaoAprovador { get; set; }

        [JsonPropertyName("maquina")]
        public string Maquina { get; set; }
    }
    
    public class AcessosUsuarioDTO
    {
        [JsonPropertyName("nomeProfissional")]
        public string NomeProfissional { get; set; }

        [JsonPropertyName("emailFoursys")]
        public string EmailFoursys { get; set; }

        [JsonPropertyName("tipoEmail")]
        public string TipoEmail { get; set; }

        [JsonPropertyName("loginRede")]
        public string LoginRede { get; set; }

        [JsonPropertyName("sistemasLiberados")]
        public List<AcessoSistemaDTO> SistemasLiberados { get; set; }

        [JsonPropertyName("diretoriosRede")]
        public List<AcessoDiretorioRedeDTO> DiretoriosRede { get; set; }

        [JsonPropertyName("gruposEmail")]
        public AcessoGrupoEmailDTO GruposEmail { get; set; }

        [JsonPropertyName("observacaoAprovador")]
        public string ObservacaoAprovador { get; set; }

        [JsonPropertyName("maquina")]
        public string Maquina { get; set; }
    }
    
    public class TemplateCandidaturaDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("numeroVaga")]
        public string NumeroVaga { get; set; }

        [JsonPropertyName("tituloVaga")]
        public string TituloVaga { get; set; }

        [JsonPropertyName("cliente")]
        public string Cliente { get; set; }

        [JsonPropertyName("profissional")]
        public ProfissionalDTO Profissional { get; set; }

        [JsonPropertyName("vagaAdmissao")]
        public VagaAdmissaoDTO VagaAdmissao { get; set; }

        [JsonPropertyName("beneficios")]
        public BeneficiosDTO Beneficios { get; set; }

        [JsonPropertyName("checklistInstalacao")]
        public ChecklistInstalacaoDTO ChecklistInstalacao { get; set; }

        [JsonPropertyName("equipamentosFoursys")]
        public EquipamentosFoursysDTO EquipamentosFoursys { get; set; }

        [JsonPropertyName("acessosUsuario")]
        public AcessosUsuarioDTO AcessosUsuario { get; set; }
    }
    
    public class PdfTemplateDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }  
        
        [JsonPropertyName("numeroVaga")]
        public long NumeroVaga { get; set; }

        [JsonPropertyName("tituloVaga")]
        public string TituloVaga { get; set; }

        [JsonPropertyName("cliente")]
        public string Cliente { get; set; }

        [JsonPropertyName("profissional")]
        public ProfissionalDTO Profissional { get; set; }

        [JsonPropertyName("vagaAdmissao")]
        public VagaAdmissaoDTO VagaAdmissao { get; set; }
        
        [JsonPropertyName("equipamentosFoursys")]
        public EquipamentosFoursysDTO EequipamentosFoursys { get; set; }

        [JsonPropertyName("beneficios")]
        public BeneficiosDTO Beneficios { get; set; }

        [JsonPropertyName("checklistInstalacao")]
        public ChecklistInstalacaoDTO ChecklistInstalacao { get; set; }

        [JsonPropertyName("acessosUsuario")]
        public AcessosUsuarioDTO AcessosUsuario { get; set; }
    }
    
    public class CreatePdfTemplateParameters
    {
        [JsonPropertyName("idCandidatura")]
        public string IdCandidatura { get; set; }
    
        [JsonPropertyName("codInternoCandidato")]
        public string CodInternoCandidato { get; set; }
    
        [JsonPropertyName("idVaga")]
        public string IdVaga { get; set; }

        [JsonPropertyName("dataInicio")]
        public DateTime DataInicio { get; set; }

        [JsonPropertyName("codInternoColaboradorSolicitante")]
        public string CodInternoColaboradorSolicitante { get; set; }

        [JsonPropertyName("horarioDeTrabalho")]
        public string HorarioDeTrabalho { get; set; }

        [JsonPropertyName("horasFechadas")]
        public bool HorasFechadas { get; set; }
        
        [JsonPropertyName("areaStaff")]
        public string AreaStaff { get; set; }

        [JsonPropertyName("profissional")]
        public ProfissionalDTO Profissional { get; set; }

        [JsonPropertyName("beneficios")]
        public BeneficiosDTO Beneficios { get; set; }

        [JsonPropertyName("checklistInstalacao")]
        public ChecklistInstalacaoDTO ChecklistInstalacao { get; set; }

        [JsonPropertyName("equipamentosFoursys")]
        public EquipamentosFoursysDTO EquipamentosFoursys { get; set; }

        [JsonPropertyName("acessosUsuario")]
        public AcessosUsuarioDTO AcessosUsuario { get; set; }
    }
    public class TemplateDestinatarioEmail
    {
        public string Email { get; set; }
        public string Assunto { get; set; }
        public string Area { get; set; }
        public int Tb_Org_id { get; set; }
        public int Anexo { get; set; }
        public int Ativo { get; set; }
    }
}