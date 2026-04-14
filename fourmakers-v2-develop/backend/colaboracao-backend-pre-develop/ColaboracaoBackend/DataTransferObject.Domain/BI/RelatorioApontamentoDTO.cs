using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BI
{
    public class RelatorioApontamentoDTO
    {
        [JsonPropertyName("CodColaborador")]
        public string CodColaborador { get; set; }
        [JsonPropertyName("Colaborador")]
        public string Colaborador { get; set; }
        [JsonPropertyName("CodCliente")]
        public string CodCliente { get; set; }
        [JsonPropertyName("Cliente")]
        public string Cliente { get; set; }
        [JsonPropertyName("CodProjeto")]
        public string CodProjeto { get; set; }
        [JsonPropertyName("Projeto")]
        public string Projeto { get; set; }
        [JsonPropertyName("Departamento")]
        public string Departamento { get; set; }
        [JsonPropertyName("Atividade")]
        public string Atividade { get; set; }
        [JsonIgnore]
        private string _statusAprovacao;
        [JsonPropertyName("StatusAprovacao")]
        public string StatusAprovacao
        {
            get
            {
                return _statusAprovacao ?? "Pendente gestor do projeto";
            }
            set
            {
                _statusAprovacao = value;
            }
        }
        [JsonPropertyName("CodigoAprovador")]
        public string CodigoAprovador { get; set; }
        [JsonPropertyName("Aprovador")]
        public string Aprovador { get; set; }
        [JsonPropertyName("Justificativa")]
        public string Justificativa { get; set; }
        private string _horas;
        [JsonPropertyName("Horas")]
        public string Horas
        {
            get
            {
                return _horas?.Replace(".", ",") ?? "";
            }
            set
            {
                _horas = value;
            }
        }
        [JsonPropertyName("Semana")]
        public string Semana { get; set; }
        [JsonPropertyName("Data")]
        public string Data { get; set; }
        [JsonPropertyName("Observacao")]
        public string Observacao { get; set; }
        [JsonPropertyName("ModeloContratacao")]
        public string ModeloContratacao { get; set; }
        [JsonPropertyName("EmpresaRelacionada")]
        public string EmpresaRelacionada { get; set; }
        [JsonPropertyName("ContatoPrincipal")]
        public string ContatoPrincipal { get; set; }

        public string? Aprovadores { get; set; } = null;
    }
    
    public class ProjetoAprovadoresRelatorioBIDTO
    {
        public string CodProjeto { get; set; }
        public string Aprovadores { get; set; }
    }
}