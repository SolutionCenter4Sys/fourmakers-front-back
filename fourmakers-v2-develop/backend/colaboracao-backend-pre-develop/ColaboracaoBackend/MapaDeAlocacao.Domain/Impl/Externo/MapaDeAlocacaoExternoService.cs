using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Core.Domain.MapaAlocacao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.Util.Enum;
using MapaDeAlocacao.Domain.Interfaces.Externo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapaDeAlocacao.Domain.Impl.Externo
{
    public class MapaDeAlocacaoExternoService : IMapaDeAlocacaoExternoService
    {
        private readonly IMapaDeAlocacaoExternoRepository _repository;
        private readonly ITokenSistemaService _tokenSistemaService;

        public MapaDeAlocacaoExternoService(
            IMapaDeAlocacaoExternoRepository repository,
            ITokenSistemaService tokenSistemaService)
        {
            _repository = repository;
            _tokenSistemaService = tokenSistemaService;
        }

        public async Task<ApiGenericResult<AlocacaoMensalResultDTO>> ObterAlocacoesHorasMensais(
            string tokenSistema,
            int mes,
            int ano,
            string codigoColaborador = null,
            string codigoProjeto = null)
        {
            int orgId = _tokenSistemaService.GetOrgTokenSistemaWithValidatingOrgs(tokenSistema, EnumORG.FOURSYS_2);

            var apiGenericResult = new ApiGenericResult<AlocacaoMensalResultDTO>();

            try
            {
                var alocacoes = await _repository.BuscarAlocacoesPorMesAno(mes, ano, orgId, codigoColaborador, codigoProjeto);

                var resultado = new AlocacaoMensalResultDTO
                {
                    Mes = mes,
                    Ano = ano,
                    Alocacoes = new List<ColaboradorAlocacaoDTO>()
                };

                if (alocacoes == null || !alocacoes.Any())
                {
                    apiGenericResult.Retorno = resultado;
                    return apiGenericResult;
                }

                var feriados = await _repository.GetFeriadosPorOrgId(orgId);

                var inicioMes = new DateTime(ano, mes, 1);
                var fimMes = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));

                var colaboradoresAgrupados = alocacoes.GroupBy(a => new { a.CodigoColaborador, a.CpfColaborador, a.ColaboradorNome });

                foreach (var grupoColaborador in colaboradoresAgrupados)
                {
                    var colaboradorDTO = new ColaboradorAlocacaoDTO
                    {
                        NomeColaborador = grupoColaborador.Key.ColaboradorNome,
                        CodigoColaborador = grupoColaborador.Key.CodigoColaborador,
                        CodigoInternoColaborador = grupoColaborador.Key.CpfColaborador,
                        Projetos = new List<ProjetoAlocacaoMensalDTO>()
                    };

                    var projetosAgrupados = grupoColaborador.GroupBy(a => new { a.CodigoProjeto, a.NomeProjeto });

                    foreach (var grupoProjeto in projetosAgrupados)
                    {
                        double horasUteisProjeto = 0;

                        foreach (var alocacao in grupoProjeto)
                        {
                            DateTime dataInicio = alocacao.DataInicio > inicioMes ? alocacao.DataInicio : inicioMes;
                            DateTime dataFim = alocacao.DataFim < fimMes ? alocacao.DataFim : fimMes;

                            int diasUteis = DateTimeUtil.CountBusinessDays(dataInicio, dataFim, alocacao.IncluiFimDeSemana, feriados);
                            horasUteisProjeto += diasUteis * alocacao.QuantidadeHoras;
                        }

                        colaboradorDTO.Projetos.Add(new ProjetoAlocacaoMensalDTO
                        {
                            CodigoProjeto = grupoProjeto.Key.CodigoProjeto,
                            NomeProjeto = grupoProjeto.Key.NomeProjeto,
                            HorasUteis = horasUteisProjeto
                        });
                    }

                    resultado.Alocacoes.Add(colaboradorDTO);
                }

                apiGenericResult.Retorno = resultado;
            }
            catch (Exception ex)
            {
                apiGenericResult.Sucesso = false;
                apiGenericResult.Mensagem = $"Erro ao obter alocacoes. Detalhe: {ex.Message}";
            }

            return apiGenericResult;
        }
    }
}
