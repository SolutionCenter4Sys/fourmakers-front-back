using System;

namespace DataTransferObject.Domain.Org
{
    public class ColaboradorOrgDTO
    {
        public string Cpf { get; set; }
        public int OrgId { get; set; }
        public string CodColaborador { get; set; }
        public string CodCargo { get; set; }
        public string Cargo { get; set; }
        public string CodDiretoria { get; set; }
        public string Diretoria { get; set; }
        public string CodDepartamento { get; set; }
        public string Departamento { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public DateTime? DataInativacao { get; set; }
        public string ModeloContratacao { get; set; }
        public string EmpresaRelacionada { get; set; }
        public string ModeloTrabalho { get; set; }
        public int? DiasPorSemana { get; set; }
        public decimal? ValorHora { get; set; }
        public decimal? CustoHora { get; set; }
        public int? BaseHoraMes { get; set; }
        public string Idioma { get; set; }
        public bool Ativo { get; set; }
        public string? CodigoModeloContratacao { get; set; }
    }
}