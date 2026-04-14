using System;

namespace DataTransferObject.Domain.Colaborador
{
    public class CadastroColaboradorInput
    {
        public string CodColaborador { get; set; }
        public string NomeColaborador { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string CodDiretoria { get; set; }
        public string Diretoria { get; set; }
        public string CodDepartamento { get; set; }
        public string Departamento { get; set; }
        public string CodGestor { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime? DataInativacao { get; set; }
        public string DocumentoColaborador { get; set; }
        public string ContatoPrincipalDDI { get; set; }
        public string ContatoPrincipal { get; set; }
        public string ModeloContratacao { get; set; }
        public string? EmpresaRelacionada { get; set; }
        public string ModeloTrabalho { get; set; }
        public int? DiasPorSemana { get; set; }
        public decimal? ValorHora { get; set; }
        public decimal? CustoHora { get; set; }
        public int? BaseHoraMes { get; set; }
        public bool? ConsiderarBancoDeTalentos { get; set; }
        public bool ConsiderarVisualizacaoAderentes { get; set; }
        public string Cargo { get; set; }
        public string CodigoCargo { get; set; }
    }
}