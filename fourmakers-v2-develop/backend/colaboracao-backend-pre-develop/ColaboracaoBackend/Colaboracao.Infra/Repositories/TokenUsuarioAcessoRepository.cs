using Colaboracao.Infra.Context;
using Core.DomainModel;
using DataTransferObject.Domain;
using System;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class TokenUsuarioAcessoRepository : ITokenUsuarioAcessoDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public TokenUsuarioAcessoRepository(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }

        public TokenValidacaoAcessoDTO GetModelByKey(string key)
        {
            var row = _colaboradorContext.tb_token_acesso
                .Where(x => x.token == key && x.ativo == 1)
                .OrderByDescending(x => x.id)
                .FirstOrDefault();

            if (row == null)
            {
                return null;
            }

            return new TokenValidacaoAcessoDTO
            {
                Token = row.token,
                Validade = row.validade,
                Tipo = TipoTokenAcessoEnum.Email
            };
        }

        public TokenValidacaoAcessoDTO SaveModel(TokenValidacaoAcessoDTO model)
        {
            try
            {
                if (_colaboradorContext.tb_token_acesso.Where(x => x.token == model.Token && x.ativo == 1).Count() > 0)
                    throw new Exception("Este token já existe.");

                var row = new tb_token_acesso
                {
                    token = model.Token,
                    validade = model.Validade,
                    ativo = 1,
                    data_criacao = DateTime.Now
                };

                _colaboradorContext.tb_token_acesso.Add(row);
                _colaboradorContext.SaveChanges();

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteModel(TokenValidacaoAcessoDTO model)
        {
            try
            {
                var row = _colaboradorContext.tb_token_acesso
                    .Where(x => x.token == model.Token && x.ativo == 1)
                    .OrderByDescending(x => x.id)
                    .FirstOrDefault();

                if (row != null)
                {
                    row.ativo = 0;
                    _colaboradorContext.tb_token_acesso.Update(row);
                    _colaboradorContext.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
