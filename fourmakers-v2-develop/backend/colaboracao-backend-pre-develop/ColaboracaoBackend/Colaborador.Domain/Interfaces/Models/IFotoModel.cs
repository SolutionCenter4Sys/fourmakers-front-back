using System;

namespace Colaborador.Domain.Interfaces.Models
{
    public interface IFotoModel
    {
        long Id { get; set; }
        string Path { get; set; }
        DateTime DataCriacao { get; set; }
        DateTime DataAlteracao { get; set; }
        sbyte Ativo { get; set; }

        IFotoModel SaveModel();
    }
}