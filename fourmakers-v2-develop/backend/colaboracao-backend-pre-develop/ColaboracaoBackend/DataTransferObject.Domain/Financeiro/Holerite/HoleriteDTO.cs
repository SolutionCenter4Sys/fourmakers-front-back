using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Holerite
{
    public class HoleriteAnaliseResultDTO
    {
        public HoleriteWrapperDTO Data { get; set; }
        public bool Success { get; set; }
    }
    public class HoleriteWrapperDTO
    {
        public HoleriteDTO Holerite { get; set; }
    }

    public class HoleriteDTO
    {
        public EmpresaDTO Empresa { get; set; }
        public FuncionarioDTO Funcionario { get; set; }
        public PeriodoDTO Periodo { get; set; }
        public List<ItemHoleriteDTO> Itens { get; set; }
        public InformacoesBancariasDTO InformacoesBancarias { get; set; }
        public TotaisDTO Totais { get; set; }
        public BasesDeCalculoDTO BasesDeCalculo { get; set; }
        [JsonPropertyName("configuracoes_projeto")]
        public ConfiguracaoProjetoHoleriteDTO ConfiguracoesProjeto { get; set; }
    }

    public class ConfiguracaoProjetoHoleriteDTO
    {
        [JsonPropertyName("modalidade_horas_extras")]
        public string ModalidadeHE { get; set; }
    }

    public class EmpresaDTO
    {
        public string Nome { get; set; }
        public string Localidade { get; set; }
        public string Cnpj { get; set; }
    }

    public class FuncionarioDTO
    {
        public string Matricula { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Cbo { get; set; }
        public string Funcao { get; set; }
        public string DataAdmissao { get; set; }
        public string EmpresaId { get; set; }
        public string LocalId { get; set; }
        public string Departamento { get; set; }
    }

    public class PeriodoDTO
    {
        public string Referencia { get; set; }
        public string Tipo { get; set; }
        public string Folha { get; set; }
    }

    public class ItemHoleriteDTO
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string Referencia { get; set; }
        public string Proventos { get; set; }
        public string Descontos { get; set; }
    }

    public class InformacoesBancariasDTO
    {
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string Conta { get; set; }
    }

    public class TotaisDTO
    {
        public string Proventos { get; set; }
        public string Descontos { get; set; }
        public string Liquido { get; set; }
    }

    public class BasesDeCalculoDTO
    {
        public string SalarioBase { get; set; }
        public string Inss { get; set; }
        public string Fgts { get; set; }
        public string FgtsMes { get; set; }
        public string Irrf { get; set; }
        public string FaixaIrrf { get; set; }
        public string DependentesIrrf { get; set; }
    }
} 