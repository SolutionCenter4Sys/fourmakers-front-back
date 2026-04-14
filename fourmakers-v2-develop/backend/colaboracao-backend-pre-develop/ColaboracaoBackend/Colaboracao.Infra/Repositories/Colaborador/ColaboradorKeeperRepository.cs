using Colaboracao.Infra.Context;
using Core.Domain.Colaborador;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Colaborador
{
    public class ColaboradorKeeperRepository : IColaboradorKeeperRepository
    {
        private ColaboradorContext _colaboradorContext;

        public ColaboradorKeeperRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }

        public bool SeKeeper(string cpf)
        {
            try
            {
                return _colaboradorContext.tb_colaborador_referencia_hardskill
                    .Where(x => x.codigo_interno_colaborador == cpf && x.ativo == 1 && x.tb_nivel_referencia_hardskill.descricao == "KEEPER").Any();
            }
            catch
            {
                throw;
            }
        }
    }
}