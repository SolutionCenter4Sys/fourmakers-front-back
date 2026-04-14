using Colaboracao.Infra.Context;
using Core.DomainModel;
using DataTransferObject.Domain.Colaborador;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class SimpleColaboradorRepository : ISimpleColaboradorDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        private readonly IConfiguration _configuration;

        public SimpleColaboradorRepository(ColaboradorContext colaboradorContext, IConfiguration configuration)
        {
            this._colaboradorContext = colaboradorContext;
            this._configuration = configuration;
        }

        public List<SimpleColaboradorDTO> BuscarListaColaboradores(string nomeCompleto)
        {
            var registroDb = _colaboradorContext.tb_colaborador.Where(x => x.nome_completo.Contains(nomeCompleto));

            if (registroDb != null)
            {
                var colaboradores = new List<SimpleColaboradorDTO>();

                foreach (var colaboradorBanco in registroDb)
                {
                    colaboradores.Add(new SimpleColaboradorDTO
                    {
                        Cpf = colaboradorBanco.codigo_interno_colaborador,
                        NomeCompleto = colaboradorBanco.nome_completo
                    });
                }

                return colaboradores;
            }
            else
            {
                return null;
            }
        }
    }
}
