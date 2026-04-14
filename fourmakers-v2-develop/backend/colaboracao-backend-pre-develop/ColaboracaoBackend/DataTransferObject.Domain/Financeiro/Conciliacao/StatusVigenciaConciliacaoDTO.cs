using System;

namespace DataTransferObject.Domain.Financeiro.Conciliacao
{
    public class StatusVigenciaConciliacaoDTO
    {
        public string Cnpj { get; set; }
        public string Vigencia { get; set; } 
        public string Descricao { get; set; }
        public StatusConciliacaoEnum Status { get; set; }
    }
}
