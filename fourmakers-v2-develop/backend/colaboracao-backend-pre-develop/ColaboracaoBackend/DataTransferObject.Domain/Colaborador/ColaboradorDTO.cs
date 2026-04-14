using DataTransferObject.Domain.Candidato;
using DataTransferObject.Domain.Cargo;
using DataTransferObject.Domain.Colaborador.Cidadania;
using DataTransferObject.Domain.Diretoria;
using DataTransferObject.Domain.Endereco;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace DataTransferObject.Domain.Colaborador
{
    public class ColaboradorDTO
    {
        public ColaboradorDTO()
        {
            Cargo = new CargoDTO();

            Diretoria = new DiretoriaDTO();

            Status = new StatusColaboradorResult();

            Endereco = new EnderecoDTO();

            Candidato = new CandidatoDTO();

            Saude = new ColaboradorSaudeDTO();
        }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("nomeCompleto")]
        public string NomeCompleto { get; set; }
        public string Sobre { get; set; }
        public List<VistoColaboradorDTO> Vistos { get; set; }
        public List<PassaporteColaboradorDTO> Passaportes { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        private string _contatoPrincipal { get; set; }

        [JsonPropertyName("contatoPrincipal")]
        public string ContatoPrincipal
        {
            get
            {
                return !string.IsNullOrEmpty(_contatoPrincipal) ? Regex.Replace(_contatoPrincipal, "[^0-9]", string.Empty) : null;
            }

            set
            {
                _contatoPrincipal = !string.IsNullOrEmpty(value) ? Regex.Replace(value, "[^0-9]", string.Empty) : null;
            }
        }
        public string ContatoPrincipalDDI { get; set; }

        [JsonPropertyName("contatoOutros")]
        public string ContatoOutros { get; set; }

        [JsonPropertyName("slack_id")]
        public string Slack_id { get; set; }

        [JsonPropertyName("dataNascimento")]
        public DateTime? DataNascimento { get; set; }

        [JsonPropertyName("cargo")]
        public CargoDTO Cargo { get; set; }

        [JsonPropertyName("diretoria")]
        public DiretoriaDTO Diretoria { get; set; }

        [JsonPropertyName("status")]
        public StatusColaboradorResult Status { get; set; }

        [JsonPropertyName("rg")]
        public string Rg { get; set; }

        [JsonPropertyName("fcmToken")]
        public string FcmToken { get; set; }

        [JsonPropertyName("matricula")]
        public string Matricula { get; set; }

        [JsonPropertyName("dataAdmissao")]
        public DateTime? DataAdmissao { get; set; }

        public string ModeloContratacao { get; set; }
        public string EmpresaRelacionada { get; set; }
        public string ModeloTrabalho { get; set; }
        public int? DiasPorSemana { get; set; }
        public decimal? ValorHora { get; set; }
        public decimal? CustoHora { get; set; }
        public int? BaseHoraMes { get; set; }

        [JsonPropertyName("endereco")]
        public EnderecoDTO Endereco { get; set; }

        [JsonPropertyName("flagCandidato")]
        public bool FlagCandidato { get; set; }

        [JsonPropertyName("flagAtivo")]
        public bool FlagAtivo { get; set; }

        [JsonPropertyName("candidato")]
        public CandidatoDTO Candidato { get; set; }

        [JsonPropertyName("urlFoto")]
        public string UrlFoto { get; set; }

        [JsonPropertyName("urlFotoThumb")]
        public string UrlFotoThumb { get; set; }

        [JsonPropertyName("urlFotoThumbMini")]
        public string UrlFotoThumbMini { get; set; }

        [JsonPropertyName("urlFotoThumbVeryMini")]
        public string UrlFotoThumbVeryMini { get; set; }

        [JsonPropertyName("meSegue")]
        public bool MeSegue { get; set; }

        [JsonPropertyName("estouSeguindo")]
        public bool EstouSeguindo { get; set; }

        //[JsonIgnore]
        //public string searchAux { get; set; }

        //[JsonIgnore]
        //public string Error { get; set; }

        [JsonPropertyName("acessoBackoffice")]
        public bool AcessoBackoffice { get; set; }

        [JsonPropertyName("acessoBuscaAvançada")]
        public bool AcessoBuscaAvançada { get; set; }

        [JsonPropertyName("passaporte")]
        public string Passaporte { get; set; }

        [JsonPropertyName("estadoCivil")]
        public string EstadoCivil { get; set; }

        [JsonPropertyName("etnia")]
        public string Etnia { get; set; }

        [JsonPropertyName("genero")]
        public string Genero { get; set; }

        [JsonPropertyName("orientacaoSexual")]
        public string OrientacaoSexual { get; set; }

        [JsonPropertyName("escolaridade")]
        public string Escolaridade { get; set; }

        [JsonPropertyName("pessoa_refugiada")]
        public bool PessoaRefugiada { get; set; }

        [JsonPropertyName("email_alternativo")]
        public string EmailAlternativo { get; set; }

        public override int GetHashCode()
        {
            return BitConverter.ToInt32(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(Cpf)), 0);
        }
        [JsonPropertyName("saude")]
        public ColaboradorSaudeDTO Saude { get; set; }

        [JsonPropertyName("nacionaliade")]
        public string Nacionalidade { get; set; }

        [JsonPropertyName("curriculo")]
        public ColaboradorCurriculoDTO ColaboradorCurriculo { get; set; }

        [JsonPropertyName("empresas_vinculadas")]
        public List<VinculoEmpresaColaboradorDTO> EmpresasVinculadas { get; set; }

        [JsonPropertyName("documentoColaborador")]
        public string DocumentoColaborador { get; set; }
        [JsonPropertyName("urlLinkedin")]
        public string UrlLinkedin { get; set; }
        [JsonPropertyName("dataSincronizacaoLinkedin")]
        public DateTime? DataSincronizacaoLinkedin { get; set; }
        public bool VisualizarBuscaAderencia { get; set; }
        public List<CidadaniaColaboradorDTO> Cidadanias { get; set; }
        public string QuemCadastrou { get; set; }
        public string? CodigoModeloContratacao  { get; set; }

        [JsonPropertyName("ultimaPretensaoSalarial")]
        public decimal? UltimaPretensaoSalarial { get; set; }

        [JsonPropertyName("ultimoModeloTrabalhoId")]
        public string UltimoModeloTrabalhoId { get; set; }

        [JsonPropertyName("ultimoModeloTrabalhoDescricao")]
        public string UltimoModeloTrabalhoDescricao { get; set; }
    }
}