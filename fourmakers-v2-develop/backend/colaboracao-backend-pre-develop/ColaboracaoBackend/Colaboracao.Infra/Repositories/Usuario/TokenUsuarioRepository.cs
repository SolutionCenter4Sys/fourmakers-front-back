using Colaboracao.Infra.Context;
using Core.Domain.Usuario;
using System;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Usuario
{
    public class TokenUsuarioRepository : ITokenUsuarioRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        public TokenUsuarioRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }

        public long GetUserIdByToken(string token, bool isSSO)
        {
            try
            {
                long ret = -1;
                if (isSSO)
                    ret = _colaboradorContext.tb_token_sso.Where(x => x.token == token).Select(x => x.tb_usuario_id).FirstOrDefault();
                else
                    ret = _colaboradorContext.tb_token_acesso.Where(x => x.token == token).Select(x => x.tb_usuario_tokenacesso.FirstOrDefault().usuario_id).FirstOrDefault();

                if (ret == -1)
                    throw new Exception("Usuario não encontrado");
                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}