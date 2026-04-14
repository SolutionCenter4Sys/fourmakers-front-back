using Colaboracao.Infra.Context;
using Core.DomainModel;
using DataTransferObject.Domain.SSO;
using System;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class TokenSSORepository : ITokenSSODtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public TokenSSORepository(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }

        public TokenSSO SaveTokenSSO(TokenSSO model)
        {
            var userRow = _colaboradorContext.tb_usuario.Where(x => x.codigo_interno_colaborador == model.Cpf).FirstOrDefault();
            if (userRow == null)
                throw new Exception("Usuário não cadastrado");

            var row = new tb_token_sso();
            row.tb_usuario_id = userRow.id;
            row.token = model.Token;
            row.refresh_token = model.RefreshToken;

            _colaboradorContext.tb_token_sso.Add(row);
            _colaboradorContext.SaveChanges();

            return model;
        }

        public TokenSSO GetTokenSSO(TokenSSO model)
        {
            try
            {
                var result = _colaboradorContext.tb_token_sso.Where(x => x.token == model.Token).FirstOrDefault();

                if (result == null)
                {
                    throw new Exception("Token não encontrado");
                }

                model.Cpf = result.tb_usuario.codigo_interno_colaborador;
                model.RefreshToken = result.refresh_token;

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteTokenColaborador(TokenSSO model)
        {
            var rows = _colaboradorContext.tb_token_sso.Where(x => x.tb_usuario.codigo_interno_colaborador == model.Cpf).ToList();
            if (rows != null && rows.Count > 0)
            {
                foreach (var row in rows)
                {
                    _colaboradorContext.tb_token_sso.Remove(row);
                }
                _colaboradorContext.SaveChanges();
            }
        }
    }
}
