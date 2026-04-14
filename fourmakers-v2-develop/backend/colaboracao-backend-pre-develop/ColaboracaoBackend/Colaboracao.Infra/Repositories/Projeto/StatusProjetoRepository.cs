using Colaboracao.Infra.Context;
using Core.Domain.Projeto;
using DataTransferObject.Domain.Projeto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Projeto
{
    public class StatusProjetoRepository : IStatusProjetoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        public StatusProjetoRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }

        Task<List<StatusProjetosDTO>> IStatusProjetoRepository.ListarStatusProjeto(int orgId)
        {
            var listaStatusProjeto = _colaboradorContext.tb_status_projeto.Where(x => x.tb_org_id == orgId).Select(x => new StatusProjetosDTO
            {
                CodigoStatusProjeto = x.cod_status,
                NomeStatusProjeto = x.descricao_status
            }).ToListAsync();

            return listaStatusProjeto;
        }
    }
}