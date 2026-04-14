using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Carga
{
    public class ColaboradorCargaDTO
    {
        public String NomeCompleto { get; set; }
        public String Cpf { get; set; }
        public String Email { get; set; }
        public String IdentificadorColaborador { get; set; }
        public String Cargo { get; set; }
        public String IdentificadorCargo { get; set; }
        public String Filial { get; set; }
        public String IdentificadorFilial { get; set; }
        public bool ColaboradorAtivo { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public DateTime? DataNascimento { get; set; }
        public String Departamento { get; set; }
        public String IdentificadorDepartamento { get; set; }
        public string ModeloContratacao { get; set; }
        public string? EmpresaRelacionada { get; set; }
        public string ModeloTrabalho { get; set; }
        public int? DiasPorSemana { get; set; }
        public decimal? ValorHora { get; set; }
        public decimal? CustoHora { get; set; }
        public int? BaseHoraMes { get; set; }
        public bool Ativo { get; set; }
        public DateTime? DataInativacao { get; set; }
        public List<ColaboradorProjetoCargaDTO> Projetos { get; set; }
    }
}