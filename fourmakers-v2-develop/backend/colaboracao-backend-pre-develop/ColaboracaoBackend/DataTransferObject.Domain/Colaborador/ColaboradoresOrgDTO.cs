using DataTransferObject.Domain.Diretoria;
using DataTransferObject.Domain.Foursys;
using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.Util;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class ColaboradoresOrgDTO
    {
        public string CodColaborador { get; set; }

        public string Cpf { get; set; }

        public bool Ativo { get; set; }

        public string ModeloContratacao { get; set; }
        public string? EmpresaRelacionada { get; set; }
        public string ModeloTrabalho { get; set; }
        public int? DiasPorSemana { get; set; }
        public decimal? ValorHora { get; set; }
        public decimal? CustoHora { get; set; }
        public int? BaseHoraMes { get; set; }

        public bool PrimeiroAcessoRealizado { get; set; }
        public string DataInativacao { get; set; }

        public string? Email { get; set; }

        public long? UsuarioId { get; set; }

        public string Nome { get; set; }

        public OrgDTO Org { get; set; }

        public DepartamentoColaboradorDTO DepartamentoColaborador { get; set; }
        public DiretoriaColaboradorDTO DiretoriaColaborador { get; set; }
        public GestorDTO Gestor { get; set; }

        public string ImagemPath { get; set; }

        /// <summary>URL absoluta da foto de perfil (SERVICE_MIDIA + path), para listas/select com avatar.</summary>
        [JsonPropertyName("urlFoto")]
        public string UrlFoto { get; set; }

        public string DataAdmissao { get; set; }

        public string Status { get; set; }

        public string DocumentoColaborador { get; set; }
        public string ContatoPrincipalDDI { get; set; }
        public string ContatoPrincipal { get; set; }
        public bool ConsiderarBancoDeTalentos { get; set; }
        public bool ConsiderarVisualizacaoAderencia { get; set; }
        public CargoColaboradorOrgDTO CargoColaborador { get; set; }
    }
}