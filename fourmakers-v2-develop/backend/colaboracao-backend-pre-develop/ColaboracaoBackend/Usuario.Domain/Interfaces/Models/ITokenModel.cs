using System;
using Usuario.Domain.Interfaces.Factorys;

namespace Usuario.Domain.Interfaces.Models
{
    public interface ITokenModel
    {
        long Id { get; set; }
        string Token { get; set; }
        DateTime Validade { get; set; }
        sbyte Ativo { get; set; }
        long UsuarioId { get; set; }
        string UsuarioCpf { get; set; }

        ITokenModel SaveModel();
        ITokenModel GetTokenById(long id);
        ITokenModel UpdateModel();
        ITokenModel GetModel(string tokenAcesso, ITokenDomainFactory factory);
    }
}