using System;
using DataTransferObject.Domain.VagasSRS;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Vaga
{
    public class VagaRecrutamentoDTO
    {
        public string Id { get; set; }
        public long Codigo { get; set; }
        public string Titulo { get; set; }
        public int NumeroDeVagas { get; set; }
        public decimal CustoProfissional { get; set; }
        public decimal RateCard { get; set; }
        public string Descricao { get; set; }
        public string Cargo { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataUltimaAlteracao { get; set; }
        public string Localizacao { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public string Cep { get; set; }
        public string Pais { get; set; }
        //TODO Trocar estes nomes CPFS para codigos, juntamente com o front
        public string CpfUsuarioCriador { get; set; }
        public string NomeUsuarioCriador { get; set; }
        public string CpfUsuarioAprovador { get; set; }
        public string CodigoGestor { get; set; }
        public string StatusVagaCod { get; set; }
        public string OrigemVagaCod { get; set; }
        public string OrgId { get; set; }
        public string IdPerfilGerador { get; set; }
        public string Frequencia { get; set; }
        public int ModeloTrabalhoCod { get; set; }
        public string ModeloTrabalhoDescricao { get; set; }
        public string NomeGestor { get; set; }
        public string NomeCliente { get; set; }
        public string CodigoCliente { get; set; }
        public string NomeUsuarioAlterador { get; set; }
        public bool SlaContando { get; set; }
        public TimeSpan SlaDecorridoTotal { get; set; }
        public TimeSpan SlaDecorridoDaEtapaAtual { get; set; }
        public List<VagaSkillRecrutamentoDTO> Skills { get; set; }
        public List<QuantidadeCandidatosPorEstagioDTO> QuantidadeCandidatosPorEstagio { get; set; }

        // Propriedades para informações complementares
        public string ColaboradorCodigoInternoColaboradorGestorOrgLogada { get; set; }
        public string PropostaCrm { get; set; }
        public string TipoVagaId { get; set; }
        public string TipoVagaDescricao { get; set; }
        public int? TipoContratacaoId { get; set; }
        public string UnidadeId { get; set; }
        public List<string> CodColaboradoresEntrevistadores { get; set; }
        public string Tracking { get; set; }
        public string NumeroVagaCliente { get; set; }
        public int? CodigoClienteFourmakers { get; set; }
        public Guid? TipoEmpregoLinkedin { get; set; }
        public Guid? NivelExperienciaLinkedin { get; set; }
        public string ModeloTrabalhoId { get; set; }
        public string PermanenciaId { get; set; }
        public string RecrutadorVaga { get; set; }
        public string NomeRecrutadorVaga { get; set; }
        public string IdVagaParent { get; set; }
        public string MaquinaColaborador { get; set; }
        public int CandidatosContratados { get; set; }
        public int PosicoesRestantes { get; set; }
        public List<string> EmailsAnaliseGestor { get; set; }
        public string ObservacoesInternas { get; set; }
    }
}