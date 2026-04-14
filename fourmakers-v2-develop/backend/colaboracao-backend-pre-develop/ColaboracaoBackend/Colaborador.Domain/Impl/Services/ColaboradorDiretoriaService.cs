using Colaborador.Domain.Interfaces.Services;
using Core.Domain.Colaborador;
using DataTransferObject.Domain.Diretoria;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using SRS.Infra.Constantes;
using Logs.Infra.Attributes;

namespace Colaborador.Domain.Impl.Services
{
    [LogDomainClass]
    public class ColaboradorDiretoriaService : IColaboradorDiretoriaService
    {
        private readonly IColaboradorDiretoriaRepository _colaboradorDiretoriaRepository;
        private readonly IRestricaoDeAcessoService _restricaoDeAcessoService;

        public ColaboradorDiretoriaService(IColaboradorDiretoriaRepository colaboradorDiretoriaRepository, IRestricaoDeAcessoService restricaoDeAcessoService)
        {
            _colaboradorDiretoriaRepository = colaboradorDiretoriaRepository;
            _restricaoDeAcessoService = restricaoDeAcessoService;
        }

        public async Task<List<DiretoriaColaboradorDTO>> ListarDiretoriaDosColaboradoresAsync(int orgId, string cpfRequest)
        {
            var restricaoDiretorias = await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(cpfRequest, orgId, RestricaoDeAcessoTipoConstants.DIRETORIA);
            var ret = _colaboradorDiretoriaRepository.ListarDiretoriaDosColaboradores(orgId, restricaoDiretorias);
            return ret;
        }
    }
}