using Colaboracao.Infra.Context;
using Core.DomainModel;
using DataTransferObject.Domain.Colaborador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class PaisRepository : IPaisDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public PaisRepository(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }

        public List<PaisDTO> BuscarTodos()
        {
            try
            {
                var ret = new List<PaisDTO>();

                var paises = _colaboradorContext.tb_pais.ToList();

                foreach (var item in paises)
                    ret.Add(new PaisDTO { Id = item.Id, Descricao = item.Descricao });

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
