using Colaborador.Domain.Interfaces.Services;
using Core.DomainModel;
using Logs.Infra.Attributes;
using System;
using System.Threading.Tasks;

namespace Colaborador.Domain.Impl.Services
{
    [LogDomainClass]
    public class CidadaniaValidadorService : ICidadaniaValidadorService
    {
        public readonly ICidadaniaRepository _cidadaniaRepository;

        public CidadaniaValidadorService(ICidadaniaRepository cidadaniaRepository)
        {
            _cidadaniaRepository = cidadaniaRepository;
        }

        public async Task ValidaSeExisteCidadaniaParaOColaborador(int id, string codigoInternoColaborador)
        {
            var verificaSeExiste = await _cidadaniaRepository.BuscarCidadaniaColaboradorPorId(id);
            if (verificaSeExiste == null || (verificaSeExiste != null && verificaSeExiste.CodigoInternoColaborador != codigoInternoColaborador))
            {
                throw new ApplicationException("Não foi possivel encontrar cidadania para atualizacao");
            }
        }

        public async Task ValidaSeExisteCidadaniaJaCadastradaParaColaborador(int id, string codigoInternoColaborador)
        {
            var verificaSeExiste = await _cidadaniaRepository.VerificaSeExisteCidadaniaJaCadastradaParaColaborador(id, codigoInternoColaborador);
            if (verificaSeExiste)
            {
                throw new ApplicationException("Cidadania Já cadastrada");
            }
        }
    }
}