using Colaborador.Domain.Interfaces.Factorys;
using Colaborador.Domain.Interfaces.Services;
using DataTransferObject.Domain.Colaborador;
using System;
using System.Collections.Generic;

using Logs.Infra.Attributes;

namespace Colaborador.Domain.Impl.Services
{
    [LogDomainClass]
    public class PaisService : IPaisService
    {
        private readonly IPaisDomainFactory _paisDomainFactory;

        public PaisService(IPaisDomainFactory paisDomainFactory)
        {
            _paisDomainFactory = paisDomainFactory;
        }

        public List<PaisDTO> BuscarTodos()
        {
            try
            {
                var ret = new List<PaisDTO>();

                var pais = _paisDomainFactory.buildPaisModel();
                var paises = pais.BuscarTodos(_paisDomainFactory);

                foreach (var item in paises)
                    ret.Add(item.PaisDTO);

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}