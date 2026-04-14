using Colaboracao.Infra.Context;
using Core.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class TokenRepository : ITokenDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public TokenRepository(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }

        public void SaveModel(string token, DateTime validade, sbyte ativo, long usuarioId)
        {
            try
            {
                if (_colaboradorContext.tb_token_acesso.Any(x => x.token == token && x.ativo == 1))
                    throw new Exception("Este token já existe.");

                var tokenRow = new tb_token_acesso
                {
                    ativo = ativo,
                    token = token,
                    validade = validade,
                    data_criacao = DateTime.Now
                };

                _colaboradorContext.tb_token_acesso.Add(tokenRow);
                _colaboradorContext.SaveChanges();

                if (usuarioId > 0)
                {
                    var usuarioTokenAcesso = new tb_usuario_tokenacesso
                    {
                        usuario_id = usuarioId,
                        token_acesso_id = tokenRow.id,
                        data_criacao = DateTime.Now
                    };

                    _colaboradorContext.tb_usuario_tokenacesso.Add(usuarioTokenAcesso);
                    _colaboradorContext.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteModel(long id)
        {
            try
            {
                var tokenBanco = _colaboradorContext.tb_token_acesso.FirstOrDefault(x => x.id == id);
                if (tokenBanco == null)
                    return;

                tokenBanco.ativo = 0;
                _colaboradorContext.tb_token_acesso.Update(tokenBanco);
                _colaboradorContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
