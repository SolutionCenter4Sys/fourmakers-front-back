using System;
using Usuario.Domain.Interfaces.Factorys;

namespace Usuario.Domain.Interfaces.Models
{
    public interface ITokenSistemaModel
    {
        long Id { get; set; }
        string Sistema { get; set; }
        string Token { get; set; }
        DateTime DataCriacao { get; set; }
        DateTime DataAlteracao { get; set; }
        sbyte Ativo { get; set; }

        ITokenSistemaModel GetModelByToken(string token, ITokenSistemaDomainFactory factory);
    }
}