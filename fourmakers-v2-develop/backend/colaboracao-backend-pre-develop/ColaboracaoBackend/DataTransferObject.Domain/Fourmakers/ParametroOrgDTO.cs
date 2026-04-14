using System;

namespace DataTransferObject.Domain.Fourmakers
{
    public class ParametroOrgDTO
    {
        public string Id { get; set; }
        public string NomeParametro { get; set; }
        public string DescricaoParametro { get; set; }
        public string CodigoParametro { get; set; }
        public string CodigoModuloSistema { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public bool Ativo { get; set; }
        public string TipoParametro { get; set; }
    }
}