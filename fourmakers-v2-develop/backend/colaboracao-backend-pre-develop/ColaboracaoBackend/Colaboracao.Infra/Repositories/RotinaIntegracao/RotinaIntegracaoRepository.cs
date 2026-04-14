using Colaboracao.Infra.Context;
using Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.RotinaIntegracao
{
    public class RotinaIntegracaoRepository : IRotinaIntegracaoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public RotinaIntegracaoRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }
        public const int idFoursys = 2;

        public void AtualizaStatusColaboradorPorEmail(int ativo, string email)
        {
            var colaborador = _colaboradorContext.tb_colaborador
                .FirstOrDefault(x => x.tb_usuario.Any(u => u.email == email));

            if (colaborador == null)
            {
                throw new Exception("Colaborador com este email não encontador");
            }

            if (colaborador.ativo != (sbyte)ativo)
            {
                colaborador.ativo = (sbyte)ativo;
                _colaboradorContext
                    .SaveChanges();
            }
;
        }

        public List<string> ListaEmailColaboradoresFoursys()
        {
            //Essa rotina não é mais necessária
            List<string> lista = new List<string>();
            return lista;
        }
    }
}