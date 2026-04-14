using Colaboracao.Infra.Context;
using Core.DomainModel;
using DataTransferObject.Domain.Colaborador;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class StatusRepository : IStatusDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public StatusRepository(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }

        public StatusColaboradorDTO GetStatusByCpf(string cpf)
        {
            var colaboradorStatus = _colaboradorContext.tb_colaborador_status
                .Where(x => x.codigo_interno_colaborador == cpf && x.ativo == 1)
                .FirstOrDefault();

            if (colaboradorStatus == null)
                return null;

            return new StatusColaboradorDTO
            {
                Id = colaboradorStatus.status_colaborador_id,
                Descricao = colaboradorStatus.status_colaborador?.descricao
            };
        }

        public StatusColaboradorDTO GetModelByKey(string key)
        {
            var status = _colaboradorContext.tb_status_colaborador
                .Where(x => x.id == int.Parse(key))
                .FirstOrDefault();

            if (status == null)
                return null;

            return new StatusColaboradorDTO
            {
                Id = status.id,
                Descricao = status.descricao
            };
        }

        public List<StatusColaboradorDTO> ListModel(string busca, int cursor, int limite)
        {
            var query = _colaboradorContext.tb_status_colaborador.Where(x => x.ativo == 1);

            if (!string.IsNullOrEmpty(busca))
                query = query.Where(x => x.descricao.Contains(busca));

            return query
                .Skip(cursor)
                .Take(limite)
                .Select(x => new StatusColaboradorDTO
                {
                    Id = x.id,
                    Descricao = x.descricao
                })
                .ToList();
        }
    }
}
