using Colaborador.Domain.Interfaces.Services;
using Core.Domain.Colaborador;
using System.Collections.Generic;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Colaborador.Domain.Impl.Services
{
    [LogDomainClass]
    public class ColaboradorSugestaoService : IColaboradorSugestaoService
    {
        private IColaboradorSugestaoRepository _colaboradorSugestaoRepository { get; set; }

        public ColaboradorSugestaoService(IColaboradorSugestaoRepository colaboradorSugestaoRepository)
        {
            _colaboradorSugestaoRepository = colaboradorSugestaoRepository;
        }

        public async Task<List<string>> ListarEmpresasRelacionadas(string nomeEmpresa, int limite, int cursor, int orgId)
        {
            var listaEmpresasRelacionadas = await _colaboradorSugestaoRepository.ListarEmpresasRelacionadas(nomeEmpresa, limite, cursor, orgId);

            return listaEmpresasRelacionadas;
        }
    }
}