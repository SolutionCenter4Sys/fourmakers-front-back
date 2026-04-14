using System;

namespace DataTransferObject.Domain.SRS
{
    public class MotivoReprovacaoDTO
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}

