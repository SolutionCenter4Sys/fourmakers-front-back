using Colaboracao.Helper;
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
    public class ColaboradorDepartamentoService : IColaboradorDepartamentoService
    {
        private readonly IColaboradorDepartamentoRepository _colaboradorDepartamentoRepository;
        private readonly IRestricaoDeAcessoService _restricaoDeAcessoService;

        public ColaboradorDepartamentoService(IColaboradorDepartamentoRepository colaboradorDepartamentoRepository, IRestricaoDeAcessoService restricaoDeAcessoService)
        {
            _colaboradorDepartamentoRepository = colaboradorDepartamentoRepository;
            _restricaoDeAcessoService = restricaoDeAcessoService;
        }

        public async Task<List<DepartamentoColaboradorDTO>> ListarDepartamentosDosColaboradoresAsync(int orgId, string codigoDiretoria, string cpfRequest)
        {
            var restricaoDiretorias = await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(cpfRequest, orgId, RestricaoDeAcessoTipoConstants.DIRETORIA);
            var ret = _colaboradorDepartamentoRepository.ListarDepartamentosDosColaboradores(orgId, codigoDiretoria.ToNullSeTextoNullOuZero(), restricaoDiretorias);
            return ret;
        }
    }
}