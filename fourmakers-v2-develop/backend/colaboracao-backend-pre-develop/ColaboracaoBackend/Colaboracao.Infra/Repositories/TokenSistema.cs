using Colaboracao.Infra.Context;
using Core.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class TokenSistemaRepositoryDiscontinued : ITokenSistemaDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public TokenSistemaRepositoryDiscontinued(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }

        public bool ValidaTokenSistema(string token, int orgId)
        {
            try
            {
                return _colaboradorContext.tb_token_sistema.Any(x => x.token == token && x.tb_org_id == orgId);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
