using System;

namespace DataTransferObject.Domain.Vaga
{
    public class MotivoPerdaVagaDTO
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; }
        public string Explicacao { get; set; }
        public int Ordem { get; set; }
    }
}

