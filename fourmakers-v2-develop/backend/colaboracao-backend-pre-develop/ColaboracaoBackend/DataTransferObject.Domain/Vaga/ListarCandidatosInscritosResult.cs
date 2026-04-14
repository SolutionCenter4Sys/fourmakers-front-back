using System;
using System.Collections.Generic;
using DataTransferObject.Domain.Match;

namespace DataTransferObject.Domain.Vaga
{
    public class ListarCandidatosInscritosResult
    {
        public string IdCandidatura { get; set; }
        public string Nome { get; set; }
        public string Codigo { get; set; }
        public string EmailUsuario { get; set; }
        public string EmailAlternativo { get; set; }
        public DateTime Candidatura { get; set; }
        public DateTime? UltimaAlteracao { get; set; }
        public TimeSpan SlaDecorridoTotal { get; set; }
        public TimeSpan SlaDecorridoDaEtapaAtual { get; set; }
        public string IdStatusCandidatura { get; set; }
        public string DescricaoStatusCandidatura { get; set; }
        public int OrgId { get; set; }
        public string OrgDescricao { get; set; }
        public bool? AtivoNaOrg { get; set; }
        public List<OrganizacaoCandidatoDTO> Organizacoes { get; set; } = new List<OrganizacaoCandidatoDTO>();
        public string CodigoInternoColaboradorDeQuemCadastrou { get; set; }
        public string NomeCompletoDeQuemCadastrou { get; set; }
        public string Origem { get; set; }
        public decimal? PretencaoSalarial { get; set; }
        public string ModeloTrabalhoId { get; set; }
        public string ModeloTrabalhoDescricao { get; set; }
        public string DisponibilidadeEntrevistaId { get; set; }
        public string DisponibilidadeEntrevistaDescricao { get; set; }
        public int? QuantidadeDiasPresencial { get; set; }
        public bool? Qualificado { get; set; }
        public DateTime? DataQualificacao { get; set; }
        public string NomeDeQuemQualificou { get; set; }
        public double? Match { get; set; }
        public bool? Candidatouse { get; set; }
        public bool OrigemSrsLinkedin { get; set; }
        public CandidatosMatchResponse RetornoMatch { get; set; }
        public string TipoCadastroBancoDeTalentos { get; set; }
        public string CodDiretoria { get; set; }
        public bool ExibirRemuneracao { get; set; }
        public string RecrutadorResponsavel { get; set; }
        public string CodRecrutadorResponsavel { get; set; }
        public bool UsuarioLogadoRecrutadorResponsavel { get; set; }
        public int TotalInscritoOutrasVagas { get; set; }
    }
}