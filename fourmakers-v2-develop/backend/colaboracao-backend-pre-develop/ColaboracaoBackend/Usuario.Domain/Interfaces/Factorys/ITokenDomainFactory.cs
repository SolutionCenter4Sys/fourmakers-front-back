using System;
using Usuario.Domain.Interfaces.Models;

namespace Usuario.Domain.Interfaces.Factorys
{
    public interface ITokenDomainFactory
    {
        ITokenModel buildTokenModel();
        ITokenModel buildTokenModel(long id, string token, DateTime validade, sbyte ativo, long usuarioId, string usuarioCpf);
    }
}