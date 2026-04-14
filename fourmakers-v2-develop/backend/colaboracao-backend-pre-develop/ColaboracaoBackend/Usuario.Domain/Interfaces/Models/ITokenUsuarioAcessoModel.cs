using System;
using System.Collections.Generic;
using Usuario.Domain.Interfaces.Factorys;

namespace Usuario.Domain.Interfaces.Models
{
    public interface ITokenUsuarioAcessoModel
    {
        long Id { get; set; }
        long TokenAcessoId { get; set; }
        long UsuarioId { get; set; }
        DateTime DataCriacao { get; set; }

        ITokenUsuarioAcessoModel SaveModel();
        List<ITokenUsuarioAcessoModel> ListByUserId(long usuarioId, ITokenUsuarioAcessoDomainFactory factory);
        ITokenUsuarioAcessoModel UpdateModel();
        ITokenModel GerarNovoToken(ITokenDomainFactory _tokenDomainFactory, ITokenUsuarioAcessoDomainFactory _tokenUsuarioAcessoDomainFactory, IUsuarioModel usuario);
    }
}