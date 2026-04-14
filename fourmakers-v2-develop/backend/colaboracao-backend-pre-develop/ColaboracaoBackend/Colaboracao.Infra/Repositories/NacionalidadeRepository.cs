using Colaboracao.Infra.Context;
using Core.DomainModel;
using DataTransferObject.Domain.Colaborador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class NacionalidadeRepository : INacionalidadeDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public NacionalidadeRepository(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }

        public List<NacionalidadeDTO> BuscarTodos()
        {
            try
            {
                var ret = new List<NacionalidadeDTO>();

                var nacionalidades = _colaboradorContext.tb_nacionalidade.ToList();

                foreach (var item in nacionalidades)
                    ret.Add(new NacionalidadeDTO { Id = item.Id, Descricao = item.Descricao });

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
