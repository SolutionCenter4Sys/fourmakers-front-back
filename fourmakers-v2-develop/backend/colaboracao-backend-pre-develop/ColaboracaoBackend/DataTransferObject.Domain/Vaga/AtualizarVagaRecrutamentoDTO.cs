using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Vaga
{
    public class AtualizarVagaRecrutamentoDTO
    {
        public string Id { get; set; }
        public string Titulo { get; set; }
        public int NumeroDeVagas { get; set; }
        public decimal CustoProfissional { get; set; }
        public decimal RateCard { get; set; }
        public string Descricao { get; set; }
        public string Cargo { get; set; }
        public string Localizacao { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public string Cep { get; set; }
        public string Pais { get; set; }
        public string CodigoGestor { get; set; }
        public string StatusVagaCod { get; set; }
        public string OrigemVagaCod { get; set; }
        public string Frequencia { get; set; }
        public int ModeloTrabalhoCod { get; set; }
        public string NomeGestor { get; set; }
        public string NomeCliente { get; set; }
        public string CodigoCliente { get; set; }
        public string PropostaCrm { get; set; }
        public string TipoVagaId { get; set; }
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
        public string Maquina { get; set; }
        public int CandidatosContratados { get; set; }
        public List<VagaSkillRecrutamentoDTO> Skills { get; set; }
    }
}
