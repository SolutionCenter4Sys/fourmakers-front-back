using Colaboracao.Helper;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao.AreaAtuacao;
using Projeto.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Projeto.Domain.Impl.Services
{
    [LogDomainClass]
    public class AreaAtuacaoService : IAreaAtuacaoService
    {
        private readonly IAreaAtuacaoRepository _areaAtuacaoRepository;

        public AreaAtuacaoService(IAreaAtuacaoRepository areaAtuacaoRepository)
        {
            _areaAtuacaoRepository = areaAtuacaoRepository;
        }

        public async Task<ApiGenericResult<IEnumerable<AreaAtuacaoBase>>> ListarAreasAtuacao(string cpfRequest, int orgId)
        {
            //ValidaAcessoGestorExterno(cpfRequest, orgId);

            var apiGenericResult = new ApiGenericResult<IEnumerable<AreaAtuacaoBase>>();

            var result = await _areaAtuacaoRepository.ListarAreasAtuacaoAsync(orgId);

            apiGenericResult.Retorno = result;

            return apiGenericResult;
        }

        public async Task<List<AreaAtuacaoResult>> InserirAreasAtuacaoCasoNaoExista(List<GestorExternoAreaAtuacaoInput> listaAreasAtuacaoBase, int orgId)
        {
            var listaAreasAtuacaoResult = new List<AreaAtuacaoResult>();

            foreach (var gestExtAreaAtuacao in listaAreasAtuacaoBase)
            {
                var areaAtuacao = await _areaAtuacaoRepository.GetAreaAtuacaoPorDescricaoAsync(gestExtAreaAtuacao.AreaDeAtuacao.Descricao, orgId);

                if (areaAtuacao.IsNull())
                {
                    var input = new AreaAtuacaoInput();
                    input.Descricao = gestExtAreaAtuacao.AreaDeAtuacao.Descricao;
                    input.ConfigurarParaPersistencia(Guid.NewGuid(), orgId);
                    areaAtuacao = await _areaAtuacaoRepository.InserirAreaAtuacaoAsync(input);
                }

                listaAreasAtuacaoResult.Add(areaAtuacao);
            }

            return listaAreasAtuacaoResult;
        }
    }
}