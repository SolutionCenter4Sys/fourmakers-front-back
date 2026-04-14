using Colaborador.Domain.Interfaces.Factorys;
using Colaborador.Domain.Interfaces.Services;
using DataTransferObject.Domain.Colaborador;
using System;
using System.Collections.Generic;

using Logs.Infra.Attributes;

namespace Colaborador.Domain.Impl.Services
{
    [LogDomainClass]
    public class NacionalidadeService : INacionalidadeService
    {
        private readonly INacionalidadeDomainFactory _nacionalidadeDomainFactory;

        public NacionalidadeService(INacionalidadeDomainFactory nacionalidadeDomainFactory)
        {
            _nacionalidadeDomainFactory = nacionalidadeDomainFactory;
        }

        public List<NacionalidadeDTO> BuscarTodos()
        {
            try
            {
                var ret = new List<NacionalidadeDTO>();

                var nacionalidade = _nacionalidadeDomainFactory.buildNacionalidadeModel();
                var nacionalidades = nacionalidade.BuscarTodos(_nacionalidadeDomainFactory);

                foreach (var item in nacionalidades)
                    ret.Add(item.NacionalidadeDTO);

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}