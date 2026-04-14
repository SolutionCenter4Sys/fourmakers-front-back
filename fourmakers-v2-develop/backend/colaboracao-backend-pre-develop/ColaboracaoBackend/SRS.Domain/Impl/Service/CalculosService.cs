using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Competencia.Domain.Enums;
using Core.Domain.SRS;
using Core.Domain.Vaga;
using Core.DomainModel.Calculos;
using DataTransferObject.Domain.Calculos;
using DataTransferObject.Domain.Labs;
using DataTransferObject.Domain.Match;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.SRS.RemuneracaoClt;
using DataTransferObject.Domain.Util.Enum;
using DataTransferObject.Domain.Vaga;
using Labs.Domain.Interfaces;
using Logs.Infra.Attributes;
using Microsoft.Win32;
using Sprache;
using SRS.Domain.Interfaces.Service;
using SRS.Domain.Interfaces.Service.Validadores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SRS.Domain.Impl.Service
{
    [LogDomainClass]
    public class CalculosService : ICalculosService
    {
        private readonly ICalculosValidatorService _calculosValidatorService;
        private readonly ICalculoTributosCltService _tributosCltService;
        private readonly IRemuneracaoVerbasAnuaisCalculador _remuneracaoVerbasAnuaisCalculador;
        private readonly IInssRepository _inssRepository;
        private readonly IIrrfRepository _irrfRepository;
        private readonly IIrrfReducaoRepository _irrfReducaoRepository;
        private readonly IVagaFourmakersRepository _vagaFourmakersRepository;
        private readonly IMatchService _matchService;
        private readonly IRemuneracaoCltRepository _remuneracaoCltRepository;
        private readonly IParametrizacaoSimuladorRepository _parametrizacaoSimuladorRepository;

        public CalculosService(
            ICalculosValidatorService calculosValidatorService,
            ICalculoTributosCltService tributosCltService,
            IRemuneracaoVerbasAnuaisCalculador remuneracaoVerbasAnuaisCalculador,
            IInssRepository inssRepository,
            IIrrfRepository irrfRepository,
            IIrrfReducaoRepository irrfReducaoRepository,
            IVagaFourmakersRepository vagaFourmakersRepository,
            IMatchService matchService,
            IRemuneracaoCltRepository remuneracaoCltRepository,
            IParametrizacaoSimuladorRepository parametrizacaoSimuladorRepository)
        {
            _calculosValidatorService = calculosValidatorService;
            _tributosCltService = tributosCltService;
            _remuneracaoVerbasAnuaisCalculador = remuneracaoVerbasAnuaisCalculador;
            _inssRepository = inssRepository;
            _irrfRepository = irrfRepository;
            _irrfReducaoRepository = irrfReducaoRepository;
            _vagaFourmakersRepository = vagaFourmakersRepository;
            _matchService = matchService;
            _remuneracaoCltRepository = remuneracaoCltRepository;
            _parametrizacaoSimuladorRepository = parametrizacaoSimuladorRepository;
        }

        public async Task<CalcularSalarioLiquidoOutputDTO> CalcularSalarioLiquidoAsync(CalcularSalarioLiquidoInputDTO input, InssTabelaDTO tabelaInss = null, IrrfTabelaDTO tabelaIrrf = null, IrrfReducaoDTO tabelaReducaoIrrf = null, bool calculoInterno = false)
        {
            if (!calculoInterno)
            {
                // Validar entrada
                await _calculosValidatorService.ValidarCalcularSalarioLiquido(input);
            }

            // Obter tabela INSS por ano específico
            if (tabelaInss == null)
            {
                tabelaInss = await _inssRepository.ObterTabelaInssPorAnoAsync(input.Ano);
                if (tabelaInss == null)
                    throw new ApplicationException($"Tabela INSS para o ano {input.Ano} não encontrada.");
            }

            // Obter tabela IRRF por ano específico
            if (tabelaIrrf == null)
            {
                tabelaIrrf = await _irrfRepository.ObterTabelaIrrfPorAnoAsync(input.Ano);
                if (tabelaIrrf == null)
                    throw new ApplicationException($"Tabela IRRF para o ano {input.Ano} não encontrada.");
            }

            // Obter tabela de redução IRRF por ano específico
            if (tabelaReducaoIrrf == null)
            {
                tabelaReducaoIrrf = await _irrfReducaoRepository.ObterReducaoPorAnoAsync(input.Ano);
                if (tabelaReducaoIrrf == null)
                    throw new ApplicationException($"Tabela IRRF reducao para o ano {input.Ano} não encontrada.");
            }

            // Calcular INSS
            var descontoInss = _tributosCltService.CalcularInss(input.SalarioBruto, tabelaInss);

            // Calcular base do IRRF
            var baseCalculoIrrf = _tributosCltService.CalcularBaseIrrf(input.SalarioBruto, descontoInss, input.NumeroDependentes, tabelaIrrf);

            // Calcular IRRF
            var descontoIrrf = _tributosCltService.CalcularIrrf(baseCalculoIrrf, tabelaIrrf, input.SalarioBruto, tabelaReducaoIrrf);

            // Calcular salário líquido
            var salarioLiquido = input.SalarioBruto - descontoInss - descontoIrrf;
            var totalDescontos = descontoInss + descontoIrrf;

            return new CalcularSalarioLiquidoOutputDTO
            {
                SalarioBruto = input.SalarioBruto,
                NumeroDependentes = input.NumeroDependentes,
                Ano = input.Ano,
                DescontoInss = descontoInss,
                BaseCalculoIrrf = baseCalculoIrrf,
                DescontoIrrf = descontoIrrf,
                SalarioLiquido = salarioLiquido,
                TotalDescontos = totalDescontos,
                ParametrosInss = CriarParametrosInss(tabelaInss),
                ParametrosIrrf = CriarParametrosIrrf(tabelaIrrf)
            };
        }

        private ParametrosInssDTO CriarParametrosInss(InssTabelaDTO tabelaInss)
        {
            return new ParametrosInssDTO
            {
                Ano = tabelaInss.Ano,
                TetoInss = tabelaInss.TetoInss,
                Faixas = new List<FaixaInssDTO>
                {
                    new FaixaInssDTO
                    {
                        Faixa = 1,
                        ValorMinimo = tabelaInss.Faixa1Min,
                        ValorMaximo = tabelaInss.Faixa1Max,
                        Aliquota = tabelaInss.Faixa1Aliquota
                    },
                    new FaixaInssDTO
                    {
                        Faixa = 2,
                        ValorMinimo = tabelaInss.Faixa2Min,
                        ValorMaximo = tabelaInss.Faixa2Max,
                        Aliquota = tabelaInss.Faixa2Aliquota
                    },
                    new FaixaInssDTO
                    {
                        Faixa = 3,
                        ValorMinimo = tabelaInss.Faixa3Min,
                        ValorMaximo = tabelaInss.Faixa3Max,
                        Aliquota = tabelaInss.Faixa3Aliquota
                    },
                    new FaixaInssDTO
                    {
                        Faixa = 4,
                        ValorMinimo = tabelaInss.Faixa4Min,
                        ValorMaximo = tabelaInss.Faixa4Max,
                        Aliquota = tabelaInss.Faixa4Aliquota
                    }
                }
            };
        }

        private ParametrosIrrfDTO CriarParametrosIrrf(IrrfTabelaDTO tabelaIrrf)
        {
            return new ParametrosIrrfDTO
            {
                Ano = tabelaIrrf.Ano,
                DeducaoPorDependente = tabelaIrrf.DeducaoPorDependente,
                Faixas = new List<FaixaIrrfDTO>
                {
                    new FaixaIrrfDTO
                    {
                        Faixa = 1,
                        ValorMinimo = tabelaIrrf.Faixa1Min,
                        ValorMaximo = tabelaIrrf.Faixa1Max,
                        Aliquota = tabelaIrrf.Faixa1Aliquota,
                        Deducao = tabelaIrrf.Faixa1Deducao
                    },
                    new FaixaIrrfDTO
                    {
                        Faixa = 2,
                        ValorMinimo = tabelaIrrf.Faixa2Min,
                        ValorMaximo = tabelaIrrf.Faixa2Max,
                        Aliquota = tabelaIrrf.Faixa2Aliquota,
                        Deducao = tabelaIrrf.Faixa2Deducao
                    },
                    new FaixaIrrfDTO
                    {
                        Faixa = 3,
                        ValorMinimo = tabelaIrrf.Faixa3Min,
                        ValorMaximo = tabelaIrrf.Faixa3Max,
                        Aliquota = tabelaIrrf.Faixa3Aliquota,
                        Deducao = tabelaIrrf.Faixa3Deducao
                    },
                    new FaixaIrrfDTO
                    {
                        Faixa = 4,
                        ValorMinimo = tabelaIrrf.Faixa4Min,
                        ValorMaximo = tabelaIrrf.Faixa4Max,
                        Aliquota = tabelaIrrf.Faixa4Aliquota,
                        Deducao = tabelaIrrf.Faixa4Deducao
                    },
                    new FaixaIrrfDTO
                    {
                        Faixa = 5,
                        ValorMinimo = tabelaIrrf.Faixa5Min,
                        ValorMaximo = tabelaIrrf.Faixa5Max,
                        Aliquota = tabelaIrrf.Faixa5Aliquota,
                        Deducao = tabelaIrrf.Faixa5Deducao
                    }
                }
            };
        }

        public async Task<SimularRemuneracaoTotalOutputDTO> SimularRemuneracaoTotalAsync(SimularRemuneracaoTotalInputDTO input, int orgId)
        {
            // Validar entrada e acesso
            await _calculosValidatorService.ValidarSimularRemuneracaoTotal(input);

            if (input.CustoVagaProposto.HasValue && input.CustoVagaProposto.Value <= 0)
                throw new ApplicationException("O custo da vaga proposto deve ser maior que zero.");

            VagaRecrutamentoDTO vaga;
            decimal custoVaga;

            if (input.CustoVagaProposto is > 0)
            {
                custoVaga = input.CustoVagaProposto.Value * 168;
                vaga = CriarVagaStubParaSimulacaoComCustoProposto(input.IdVaga, orgId, custoVaga);
            }
            else
            {
                vaga = await _vagaFourmakersRepository.ObterVagaRecrutamentoPorId(input.IdVaga.ToString());

                if (vaga == null)
                    throw new ApplicationException($"Vaga com ID {input.IdVaga} não encontrada.");

                if (vaga.CustoProfissional == 0)
                    throw new ApplicationException("Vaga com custo hora zerado, por favor, alterar para iniciarmos os calculos.");

                custoVaga = vaga.CustoProfissional * 168;
            }

            var parametrizacaoSimulador = await _parametrizacaoSimuladorRepository.ObterPorOrgAsync(orgId);

            if (parametrizacaoSimulador?.QuantidadeMaximaCalculos == 0)
            {
                return new SimularRemuneracaoTotalOutputDTO
                {
                    PrimeiraOpcao = new RemuneracaoDTO(),
                    SegundaOpcao = new RemuneracaoDTO(),
                    TerceiraOpcao = new RemuneracaoDTO()
                };
            }

            // Obter tabelas para cálculos de verbas anuais (e uso no fluxo pretendida)
            var tabelaInss = await _inssRepository.ObterTabelaInssPorAnoAsync(DateTime.Now.Year);
            var tabelaIrrf = await _irrfRepository.ObterTabelaIrrfPorAnoAsync(DateTime.Now.Year);
            var tabelaReducaoIrrfFerias = await _irrfReducaoRepository.ObterReducaoPorAnoAsync(DateTime.Now.Year);
            //var registro = await _remuneracaoCltRepository.ObterPorAdmissaoCargoIdAsync(input.AdmissaoCargoId.Value);
            var propostaIdeal = new RemuneracaoDTO();
            var primeiraOpcao = new RemuneracaoDTO();
            var segundaOpcao = new RemuneracaoDTO();
            var terceiraOpcao = new RemuneracaoDTO();
            decimal? valorBaseRemuneracaoClt = null;
            decimal? outrasDespesasEmpresa = null;
            decimal? valorBaseRemuneracaoCltRecalculado = null;

            for (int i = 0; i < parametrizacaoSimulador.QuantidadeMaximaCalculos; i++)
            {
                (valorBaseRemuneracaoClt, propostaIdeal, outrasDespesasEmpresa) = await CalcularPropostaIdeal(input, vaga, custoVaga, tabelaInss, tabelaIrrf, tabelaReducaoIrrfFerias, valorBaseRemuneracaoCltRecalculado);

                var porcentagemMargemCusto = parametrizacaoSimulador.PorcentagemMargemCusto / 100;
                var custoTotalFuncionario = Math.Round(propostaIdeal.BrutoComposto, 2) + Math.Round(outrasDespesasEmpresa.Value, 2);
                var custoVagaMaximo = Math.Round((custoVaga * (1 + porcentagemMargemCusto)), 2);
                var custoVagaMinimo = Math.Round((custoVaga * (1 - porcentagemMargemCusto)), 2);

                //var porcentagemMinimaPiso = parametrizacaoSimulador.PorcentagemMinimaPiso / 100;
                if ((custoTotalFuncionario < custoVagaMaximo && custoTotalFuncionario > custoVagaMinimo)
                    || (valorBaseRemuneracaoCltRecalculado < PoliticasRemuneracaoEmpresa.SalarioMinimoVigente))
                {
                    var cltIdeal = (propostaIdeal.BrutoComposto * 0.6m);
                    var salarioBrutoMaximoDentoDoCusto = Math.Round((cltIdeal * (1 + 0.02m)), 2);
                    if (propostaIdeal.CLT.SalarioBruto < salarioBrutoMaximoDentoDoCusto && propostaIdeal.CLT.SalarioBruto > cltIdeal)
                        break;
                    valorBaseRemuneracaoCltRecalculado = cltIdeal;
                    continue;
                    //if (propostaIdeal.CLT.SalarioBruto < cltIdeal)
                    //{
                    //    valorBaseRemuneracaoCltRecalculado = valorBaseRemuneracaoClt * 1.001m;
                    //    continue;
                    //}
                    //else
                    //{
                    //    valorBaseRemuneracaoCltRecalculado = valorBaseRemuneracaoClt * 0.999m;
                    //    continue;
                    //}
                }

                if (custoTotalFuncionario < custoVaga)
                    valorBaseRemuneracaoCltRecalculado = valorBaseRemuneracaoClt * 1.01m;
                else
                    valorBaseRemuneracaoCltRecalculado = valorBaseRemuneracaoClt * 0.99m;
            }

            AplicarValidacoesPolitica(propostaIdeal);

            for (int i = 0; i < parametrizacaoSimulador.QuantidadeMaximaCalculos; i++)
            {
                (valorBaseRemuneracaoClt, primeiraOpcao, outrasDespesasEmpresa) = await CalcularPrimeiraOpcao(input, vaga, custoVaga, tabelaInss, tabelaIrrf, tabelaReducaoIrrfFerias, valorBaseRemuneracaoCltRecalculado);

                var liquidoPretendidoMais1PorMIl = Math.Round((input.LiquidoPretendido * 1.001m), 2);
                var liquidoPretendidoMenos1PorMIl = Math.Round((input.LiquidoPretendido * 0.999m), 2);

                if (primeiraOpcao.RemuneracaoTotalLiquidaMensal < liquidoPretendidoMais1PorMIl && primeiraOpcao.RemuneracaoTotalLiquidaMensal > liquidoPretendidoMenos1PorMIl)
                    break;

                if (primeiraOpcao.RemuneracaoTotalLiquidaMensal < input.LiquidoPretendido)
                    valorBaseRemuneracaoCltRecalculado = valorBaseRemuneracaoClt * 1.01m;
                else
                    valorBaseRemuneracaoCltRecalculado = valorBaseRemuneracaoClt * 0.99m;

            }

            AplicarValidacoesPolitica(primeiraOpcao);

            for (int i = 0; i < parametrizacaoSimulador.QuantidadeMaximaCalculos; i++)
            {
                (valorBaseRemuneracaoClt, segundaOpcao, outrasDespesasEmpresa) = await CalcularSegundaOpcao(input, vaga, custoVaga, tabelaInss, tabelaIrrf, tabelaReducaoIrrfFerias, valorBaseRemuneracaoCltRecalculado);

                var porcentagemMargemCusto = parametrizacaoSimulador.PorcentagemMargemCusto / 100;
                var custoTotalFuncionario = Math.Round(segundaOpcao.BrutoComposto, 2) + Math.Round(outrasDespesasEmpresa.Value, 2);
                var custoVagaMaximo = Math.Round((custoVaga * (1 + porcentagemMargemCusto)), 2);
                var custoVagaMinimo = Math.Round((custoVaga * (1 - porcentagemMargemCusto)), 2);

                //var porcentagemMinimaPiso = parametrizacaoSimulador.PorcentagemMinimaPiso / 100;
                if ((custoTotalFuncionario < custoVagaMaximo && custoTotalFuncionario > custoVagaMinimo)
                    || (valorBaseRemuneracaoCltRecalculado < PoliticasRemuneracaoEmpresa.SalarioMinimoVigente))
                    break;

                if (custoTotalFuncionario < custoVaga)
                    valorBaseRemuneracaoCltRecalculado = valorBaseRemuneracaoClt * 1.01m;
                else
                    valorBaseRemuneracaoCltRecalculado = valorBaseRemuneracaoClt * 0.99m;

            }

            AplicarValidacoesPolitica(segundaOpcao);

            valorBaseRemuneracaoCltRecalculado = custoVaga * 0.5m;

            for (int i = 0; i < parametrizacaoSimulador.QuantidadeMaximaCalculos; i++)
            {
                (valorBaseRemuneracaoClt, terceiraOpcao, outrasDespesasEmpresa) = await CalcularTerceiraOpcao(input, vaga, custoVaga, tabelaInss, tabelaIrrf, tabelaReducaoIrrfFerias, valorBaseRemuneracaoCltRecalculado);

                var porcentagemExcedenteCusto = parametrizacaoSimulador.PorcentagemExcedenteCusto / 100;
                var custoTotalFuncionario = Math.Round(terceiraOpcao.BrutoComposto, 2) + Math.Round(outrasDespesasEmpresa.Value, 2);
                var custoVagaMaximo = Math.Round((custoVaga * (1 + porcentagemExcedenteCusto)), 2);
                var custoVagaMinimo = Math.Round((custoVaga * ((1 + porcentagemExcedenteCusto) * 0.99m)), 2);

                //var porcentagemMinimaPiso = parametrizacaoSimulador.PorcentagemMinimaPiso / 100;
                if ((custoTotalFuncionario < custoVagaMaximo && custoTotalFuncionario > custoVagaMinimo)
                    || (valorBaseRemuneracaoCltRecalculado < PoliticasRemuneracaoEmpresa.SalarioMinimoVigente))
                    break;

                if (custoTotalFuncionario < custoVagaMinimo)
                    valorBaseRemuneracaoCltRecalculado = valorBaseRemuneracaoClt * 1.01m;
                else
                    valorBaseRemuneracaoCltRecalculado = valorBaseRemuneracaoClt * 0.99m;

            }

            AplicarValidacoesPolitica(terceiraOpcao);

            return new SimularRemuneracaoTotalOutputDTO
            {
                PropostaIdeal = propostaIdeal,
                PrimeiraOpcao = primeiraOpcao,
                SegundaOpcao = segundaOpcao,
                TerceiraOpcao = terceiraOpcao
            };
        }

        private async Task<(decimal? valorBaseRemuneracaoClt, RemuneracaoDTO primeiraOpcao, decimal? outrasDespesasEmpresa)> CalcularPropostaIdeal(SimularRemuneracaoTotalInputDTO input, VagaRecrutamentoDTO vaga, decimal custoVaga, InssTabelaDTO tabelaInss, IrrfTabelaDTO tabelaIrrf, IrrfReducaoDTO tabelaReducaoIrrfFerias, decimal? valorBaseRemuneracaoCltRecalculado)
        {
            //decimal? valorBaseRemuneracaoClt = valorBaseRemuneracaoCltRecalculado ?? await ObterValorBaseRemuneracaoCltPorVagaAsync(vaga, registro, input.AdmissaoCargoId.Value.ToString(), input.NivelVagaCod.Value.ToString());
            decimal? valorBaseRemuneracaoClt = valorBaseRemuneracaoCltRecalculado ?? custoVaga * 0.6m;

            // Calcular os valores da remuneração proposta
            decimal valeRefeicao = PoliticasRemuneracaoEmpresa.ValeRefeicao;
            decimal assMedica = PoliticasRemuneracaoEmpresa.AssistenciaMedica;
            decimal auxilioEducacao = input.Educacao;

            // Converter Frequencia da vaga para int? (pode ser "2", "3", etc.)
            int? frequenciaDiasPresencial = null;
                if (!string.IsNullOrWhiteSpace(vaga.Frequencia) && int.TryParse(vaga.Frequencia, out int frequenciaParsed))
                    frequenciaDiasPresencial = frequenciaParsed;

            decimal mobilidade = CalcularMobilidade(vaga.ModeloTrabalhoCod, frequenciaDiasPresencial, input.Km, input.Mobilidade);

            decimal valeAlimentacao = CalcularAlimentacao(valorBaseRemuneracaoClt, input.Alimentacao);

            decimal ajudaCusto = PoliticasRemuneracaoEmpresa.CalcularValorMaximoAjudaCusto(valorBaseRemuneracaoClt.Value);

            var calcularSalarioLiquidoInput = new CalcularSalarioLiquidoInputDTO
            {
                SalarioBruto = valorBaseRemuneracaoClt.Value,
                NumeroDependentes = input.QuantidadeDependentes,
                Ano = DateTime.Now.Year
            };

            var calculoSalarioLiquido = await CalcularSalarioLiquidoAsync(calcularSalarioLiquidoInput, tabelaInss, tabelaIrrf, tabelaReducaoIrrfFerias, true);

            auxilioEducacao = CalcularAuxilioEducacao(valorBaseRemuneracaoClt, input.Educacao);

            decimal outrasDespesasEmpresa = CalcularOutrasDespesasEmpresa(valorBaseRemuneracaoClt.Value, valeRefeicao, assMedica, valeAlimentacao, ajudaCusto, mobilidade, auxilioEducacao);
            //decimal rendimentoTotalRecebidoFuncionario = custoVaga - outrasDespesasEmpresa;
            decimal somaBeneficios = valorBaseRemuneracaoClt.Value + valeRefeicao + valeAlimentacao + auxilioEducacao + mobilidade + ajudaCusto;

            // Calcular RemuneracaoTotalLiquidaMensal: soma de todos os valores líquidos
            decimal remuneracaoTotalLiquidaMensal = calculoSalarioLiquido.SalarioLiquido +
                                                   valeRefeicao +
                                                   valeAlimentacao +
                                                   auxilioEducacao +
                                                   mobilidade +
                                                   ajudaCusto;


            // Calcular verbas anuais (13º, FGTS, férias) reutilizando o calculador isolado
            var verbasProposta = _remuneracaoVerbasAnuaisCalculador.Calcular(
                calculoSalarioLiquido.SalarioLiquido,
                valorBaseRemuneracaoClt.Value,
                input.QuantidadeDependentes,
                tabelaInss,
                tabelaIrrf,
                tabelaReducaoIrrfFerias);

            decimal remuneracaoTotalLiquidaMensalComVerbasAnuais = remuneracaoTotalLiquidaMensal +
                verbasProposta.DecimoTerceiroMensal + verbasProposta.FgtsMensal + verbasProposta.FeriasMensal;

            decimal previsaoAnual = (remuneracaoTotalLiquidaMensal * 12m) + verbasProposta.DecimoTerceiroAnual +
                verbasProposta.FgtsAnual + verbasProposta.FeriasLiquido;

            decimal brutoComposto = PoliticasRemuneracaoEmpresa.CalcularBrutoTotalRemuneracao(
                valorBaseRemuneracaoClt.Value, ajudaCusto, mobilidade, valeAlimentacao, valeRefeicao, auxilioEducacao, assMedica);

            // Construir a remuneração pretendida
            var remuneracaoProposta = new RemuneracaoDTO
            {
                CLT = calculoSalarioLiquido,
                ValeRefeicao = Math.Round(valeRefeicao, 2),
                ValeAlimentacao = Math.Round(valeAlimentacao, 2),
                AuxilioEducacao = Math.Round(auxilioEducacao, 2),
                Mobilidade = Math.Round(mobilidade, 2),
                AjudaDeCusto = Math.Round(ajudaCusto, 2),
                RemuneracaoTotalLiquidaMensal = Math.Round(remuneracaoTotalLiquidaMensal, 2),
                RemuneracaoTotalLiquidaMensalComVerbasAnuais = Math.Round(remuneracaoTotalLiquidaMensalComVerbasAnuais, 2),
                PrevisaoAnual = Math.Round(previsaoAnual, 2),
                CustoTotalEmpresa = Math.Round((brutoComposto + outrasDespesasEmpresa), 2),
                CustoVaga = Math.Round((custoVaga), 2),
                BrutoComposto = Math.Round(brutoComposto, 2)
            };
            return (valorBaseRemuneracaoClt, remuneracaoProposta, outrasDespesasEmpresa);
        }

        private decimal CalcularAuxilioEducacao(decimal? valorBaseRemuneracaoClt, decimal auxEducacao)
        {
            var auxEducacaoMaxima = PoliticasRemuneracaoEmpresa.CalcularValorMaximoAuxilioEducacao(valorBaseRemuneracaoClt.Value);
            if(auxEducacao < auxEducacaoMaxima)
                return auxEducacao;
            return auxEducacaoMaxima;
        }

        /// <summary>
        /// Quando o front envia <see cref="SimularRemuneracaoTotalInputDTO.CustoVagaProposto"/>, não há consulta à vaga:
        /// monta um DTO mínimo para frequência/modelo de trabalho (mobilidade) alinhado ao cenário remoto padrão.
        /// </summary>
        private static VagaRecrutamentoDTO CriarVagaStubParaSimulacaoComCustoProposto(Guid idVaga, int orgId, decimal custoVagaTotal)
        {
            return new VagaRecrutamentoDTO
            {
                Id = idVaga.ToString(),
                OrgId = orgId.ToString(),
                CustoProfissional = custoVagaTotal,
                Frequencia = "2",
                ModeloTrabalhoCod = 3,
                Skills = new List<VagaSkillRecrutamentoDTO>()
            };
        }

        private async Task<(decimal? valorBaseRemuneracaoClt, RemuneracaoDTO segundaOpcao, decimal? outrasDespesasEmpresa)> CalcularTerceiraOpcao(SimularRemuneracaoTotalInputDTO input, VagaRecrutamentoDTO vaga, decimal custoVaga, InssTabelaDTO tabelaInss, IrrfTabelaDTO tabelaIrrf, IrrfReducaoDTO tabelaReducaoIrrfFerias, decimal? valorBaseRemuneracaoCltRecalculado)
        {
            //decimal? valorBaseRemuneracaoClt = valorBaseRemuneracaoCltRecalculado ?? await ObterValorBaseRemuneracaoCltPorVagaAsync(vaga, registro, input.AdmissaoCargoId.Value.ToString(), input.NivelVagaCod.Value.ToString());
            decimal? valorBaseRemuneracaoClt = valorBaseRemuneracaoCltRecalculado ?? custoVaga * 0.5m;

            // Calcular os valores da remuneração proposta
            decimal valeRefeicao = PoliticasRemuneracaoEmpresa.ValeRefeicao;
            decimal assMedica = PoliticasRemuneracaoEmpresa.AssistenciaMedica;
            decimal auxilioEducacao = input.Educacao;

            auxilioEducacao = CalcularAuxilioEducacao(valorBaseRemuneracaoClt, input.Educacao);

            // Converter Frequencia da vaga para int? (pode ser "2", "3", etc.)
            int? frequenciaDiasPresencial = null;
            if (!string.IsNullOrWhiteSpace(vaga.Frequencia) && int.TryParse(vaga.Frequencia, out int frequenciaParsed))
                frequenciaDiasPresencial = frequenciaParsed;

            decimal mobilidade = CalcularMobilidade(vaga.ModeloTrabalhoCod, frequenciaDiasPresencial, input.Km, input.Mobilidade);

            decimal valeAlimentacao = CalcularAlimentacao(valorBaseRemuneracaoClt, input.Alimentacao);
            decimal ajudaCusto = valorBaseRemuneracaoClt.Value * PoliticasRemuneracaoEmpresa.PercentualMaximoAjudaCustoSobreCLT;
            var calcularSalarioLiquidoInput = new CalcularSalarioLiquidoInputDTO
            {
                SalarioBruto = valorBaseRemuneracaoClt.Value,
                NumeroDependentes = input.QuantidadeDependentes,
                Ano = DateTime.Now.Year
            };

            var calculoSalarioLiquido = await CalcularSalarioLiquidoAsync(calcularSalarioLiquidoInput, tabelaInss, tabelaIrrf, tabelaReducaoIrrfFerias, true);

            decimal outrasDespesasEmpresa = CalcularOutrasDespesasEmpresa(valorBaseRemuneracaoClt.Value, valeRefeicao, assMedica, valeAlimentacao, ajudaCusto, mobilidade, auxilioEducacao);
            //decimal rendimentoTotalRecebidoFuncionario = custoVaga - outrasDespesasEmpresa;
            decimal somaBeneficios = valorBaseRemuneracaoClt.Value + valeRefeicao + valeAlimentacao + auxilioEducacao + mobilidade + ajudaCusto;

            // Calcular RemuneracaoTotalLiquidaMensal: soma de todos os valores líquidos
            decimal remuneracaoTotalLiquidaMensal = calculoSalarioLiquido.SalarioLiquido +
                                                   valeRefeicao +
                                                   valeAlimentacao +
                                                   auxilioEducacao +
                                                   mobilidade +
                                                   ajudaCusto;


            // Calcular verbas anuais (13º, FGTS, férias) reutilizando o calculador isolado
            var verbasProposta = _remuneracaoVerbasAnuaisCalculador.Calcular(
                calculoSalarioLiquido.SalarioLiquido,
                valorBaseRemuneracaoClt.Value,
                input.QuantidadeDependentes,
                tabelaInss,
                tabelaIrrf,
                tabelaReducaoIrrfFerias);

            decimal remuneracaoTotalLiquidaMensalComVerbasAnuais = remuneracaoTotalLiquidaMensal +
                verbasProposta.DecimoTerceiroMensal + verbasProposta.FgtsMensal + verbasProposta.FeriasMensal;

            decimal previsaoAnual = (remuneracaoTotalLiquidaMensal * 12m) + verbasProposta.DecimoTerceiroAnual +
                verbasProposta.FgtsAnual + verbasProposta.FeriasLiquido;

            decimal brutoComposto = PoliticasRemuneracaoEmpresa.CalcularBrutoTotalRemuneracao(
                valorBaseRemuneracaoClt.Value, ajudaCusto, mobilidade, valeAlimentacao, PoliticasRemuneracaoEmpresa.ValeRefeicao, auxilioEducacao, PoliticasRemuneracaoEmpresa.AssistenciaMedica);

            // Construir a remuneração pretendida
            var remuneracaoProposta = new RemuneracaoDTO
            {
                CLT = calculoSalarioLiquido,
                ValeRefeicao = Math.Round(valeRefeicao, 2),
                ValeAlimentacao = Math.Round(valeAlimentacao, 2),
                AuxilioEducacao = Math.Round(auxilioEducacao, 2),
                Mobilidade = Math.Round(mobilidade, 2),
                AjudaDeCusto = Math.Round(ajudaCusto, 2),
                RemuneracaoTotalLiquidaMensal = Math.Round(remuneracaoTotalLiquidaMensal, 2),
                RemuneracaoTotalLiquidaMensalComVerbasAnuais = Math.Round(remuneracaoTotalLiquidaMensalComVerbasAnuais, 2),
                PrevisaoAnual = Math.Round(previsaoAnual, 2),
                CustoTotalEmpresa = Math.Round((brutoComposto + outrasDespesasEmpresa), 2),
                CustoVaga = Math.Round((custoVaga), 2),
                BrutoComposto = Math.Round(brutoComposto, 2)
            };
            return (valorBaseRemuneracaoClt, remuneracaoProposta, outrasDespesasEmpresa);
        }

        /// <summary>
        /// Manter o liquido pretendido, manter o custo da vaga, ainda que fira politica
        /// </summary>
        /// <param name="input"></param>
        /// <param name="vaga"></param>
        /// <param name="custoVaga"></param>
        /// <param name="tabelaInss"></param>
        /// <param name="tabelaIrrf"></param>
        /// <param name="tabelaReducaoIrrfFerias"></param>
        /// <param name="registro"></param>
        /// <param name="valorBaseRemuneracaoCltRecalculado"></param>
        /// <returns></returns>
        private async Task<(decimal? valorBaseRemuneracaoClt, RemuneracaoDTO remuneracaoPretendida, decimal? outrasDespesasEmpresa)> CalcularSegundaOpcao(SimularRemuneracaoTotalInputDTO input, VagaRecrutamentoDTO vaga, decimal custoVaga, InssTabelaDTO tabelaInss, IrrfTabelaDTO tabelaIrrf, IrrfReducaoDTO tabelaReducaoIrrfFerias, decimal? valorBaseRemuneracaoCltRecalculado)
        {
            var liquidoPretendido = input.LiquidoPretendido;
            var valeAlimentacaoPretendido = input.Alimentacao;
            var auxEducacaoPretendido = input.Educacao;
            var mobilidadePretendida = input.Mobilidade;

            var salarioBrutoClt = valorBaseRemuneracaoCltRecalculado;
            var calcularSalarioLiquidoInput = new CalcularSalarioLiquidoInputDTO
            {
                SalarioBruto = salarioBrutoClt.Value,
                NumeroDependentes = input.QuantidadeDependentes,
                Ano = DateTime.Now.Year
            };

            auxEducacaoPretendido = CalcularAuxilioEducacao(salarioBrutoClt, auxEducacaoPretendido);

            var calculoSalarioLiquido = await CalcularSalarioLiquidoAsync(calcularSalarioLiquidoInput, tabelaInss, tabelaIrrf, tabelaReducaoIrrfFerias, true);

            var ajudaDeCusto = liquidoPretendido - valeAlimentacaoPretendido - auxEducacaoPretendido - mobilidadePretendida - calculoSalarioLiquido.SalarioLiquido - PoliticasRemuneracaoEmpresa.ValeRefeicao;

            decimal outrasDespesasEmpresa = CalcularOutrasDespesasEmpresa(salarioBrutoClt.Value, PoliticasRemuneracaoEmpresa.ValeRefeicao, PoliticasRemuneracaoEmpresa.AssistenciaMedica, valeAlimentacaoPretendido, ajudaDeCusto, mobilidadePretendida, auxEducacaoPretendido);

            // Calcular RemuneracaoTotalLiquidaMensal: soma de todos os valores líquidos
            decimal remuneracaoTotalLiquidaMensal = calculoSalarioLiquido.SalarioLiquido +
                                                   PoliticasRemuneracaoEmpresa.ValeRefeicao +
                                                   valeAlimentacaoPretendido +
                                                   auxEducacaoPretendido +
                                                   mobilidadePretendida +
                                                   ajudaDeCusto;


            // Calcular verbas anuais (13º, FGTS, férias) reutilizando o calculador isolado
            var verbasProposta = _remuneracaoVerbasAnuaisCalculador.Calcular(
                calculoSalarioLiquido.SalarioLiquido,
                salarioBrutoClt.Value,
                input.QuantidadeDependentes,
                tabelaInss,
                tabelaIrrf,
                tabelaReducaoIrrfFerias);

            decimal remuneracaoTotalLiquidaMensalComVerbasAnuais = remuneracaoTotalLiquidaMensal +
                verbasProposta.DecimoTerceiroMensal + verbasProposta.FgtsMensal + verbasProposta.FeriasMensal;

            decimal previsaoAnual = (remuneracaoTotalLiquidaMensal * 12m) + verbasProposta.DecimoTerceiroAnual +
                verbasProposta.FgtsAnual + verbasProposta.FeriasLiquido;

            decimal brutoComposto = PoliticasRemuneracaoEmpresa.CalcularBrutoTotalRemuneracao(
                salarioBrutoClt.Value, ajudaDeCusto, mobilidadePretendida, valeAlimentacaoPretendido, PoliticasRemuneracaoEmpresa.ValeRefeicao, auxEducacaoPretendido, PoliticasRemuneracaoEmpresa.AssistenciaMedica);

            var remuneracaoPretendida = new RemuneracaoDTO
            {
                CLT = calculoSalarioLiquido,
                ValeRefeicao = Math.Round(PoliticasRemuneracaoEmpresa.ValeRefeicao, 2),
                ValeAlimentacao = Math.Round(valeAlimentacaoPretendido, 2),
                AuxilioEducacao = Math.Round(auxEducacaoPretendido, 2),
                Mobilidade = Math.Round(mobilidadePretendida, 2),
                AjudaDeCusto = Math.Round(ajudaDeCusto, 2),
                RemuneracaoTotalLiquidaMensal = Math.Round(remuneracaoTotalLiquidaMensal, 2),
                RemuneracaoTotalLiquidaMensalComVerbasAnuais = Math.Round(remuneracaoTotalLiquidaMensalComVerbasAnuais, 2),
                PrevisaoAnual = Math.Round(previsaoAnual, 2),
                CustoTotalEmpresa = Math.Round((brutoComposto + outrasDespesasEmpresa), 2),
                CustoVaga = Math.Round((custoVaga), 2),
                BrutoComposto = Math.Round(brutoComposto, 2)
            };

            return (valorBaseRemuneracaoCltRecalculado, remuneracaoPretendida, outrasDespesasEmpresa);
        }

        private async Task<(decimal? valorBaseRemuneracaoClt, RemuneracaoDTO remuneracaoProposta, decimal? outrasDespesasEmpresa)> CalcularPrimeiraOpcao(SimularRemuneracaoTotalInputDTO input, VagaRecrutamentoDTO vaga, decimal custoVaga, InssTabelaDTO tabelaInss, IrrfTabelaDTO tabelaIrrf, IrrfReducaoDTO tabelaReducaoIrrfFerias, decimal? valorBaseRemuneracaoCltRecalculado)
        {
            //decimal? valorBaseRemuneracaoClt = valorBaseRemuneracaoCltRecalculado ?? await ObterValorBaseRemuneracaoCltPorVagaAsync(vaga, registro, input.AdmissaoCargoId.Value.ToString(), input.NivelVagaCod.Value.ToString());
            decimal? valorBaseRemuneracaoClt = valorBaseRemuneracaoCltRecalculado ?? custoVaga * 0.5m;

            // Calcular os valores da remuneração proposta
            decimal valeRefeicao = PoliticasRemuneracaoEmpresa.ValeRefeicao;
            decimal assMedica = PoliticasRemuneracaoEmpresa.AssistenciaMedica;
            decimal auxilioEducacao = input.Educacao;

            auxilioEducacao = CalcularAuxilioEducacao(valorBaseRemuneracaoClt, input.Educacao);

            // Converter Frequencia da vaga para int? (pode ser "2", "3", etc.)
            int? frequenciaDiasPresencial = null;
            if (!string.IsNullOrWhiteSpace(vaga.Frequencia) && int.TryParse(vaga.Frequencia, out int frequenciaParsed))
                frequenciaDiasPresencial = frequenciaParsed;

            decimal mobilidade = CalcularMobilidade(vaga.ModeloTrabalhoCod, frequenciaDiasPresencial, input.Km, input.Mobilidade);

            decimal valeAlimentacao = CalcularAlimentacao(valorBaseRemuneracaoClt, input.Alimentacao);
            decimal ajudaCusto = valorBaseRemuneracaoClt.Value * PoliticasRemuneracaoEmpresa.PercentualMaximoAjudaCustoSobreCLT;
            var calcularSalarioLiquidoInput = new CalcularSalarioLiquidoInputDTO
            {
                SalarioBruto = valorBaseRemuneracaoClt.Value,
                NumeroDependentes = input.QuantidadeDependentes,
                Ano = DateTime.Now.Year
            };

            var calculoSalarioLiquido = await CalcularSalarioLiquidoAsync(calcularSalarioLiquidoInput, tabelaInss, tabelaIrrf, tabelaReducaoIrrfFerias, true);

            decimal outrasDespesasEmpresa = CalcularOutrasDespesasEmpresa(valorBaseRemuneracaoClt.Value, valeRefeicao, assMedica, valeAlimentacao, ajudaCusto, mobilidade, auxilioEducacao);
            //decimal rendimentoTotalRecebidoFuncionario = custoVaga - outrasDespesasEmpresa;
            decimal somaBeneficios = valorBaseRemuneracaoClt.Value + valeRefeicao + valeAlimentacao + auxilioEducacao + mobilidade + ajudaCusto;

            // Calcular RemuneracaoTotalLiquidaMensal: soma de todos os valores líquidos
            decimal remuneracaoTotalLiquidaMensal = calculoSalarioLiquido.SalarioLiquido +
                                                   valeRefeicao +
                                                   valeAlimentacao +
                                                   auxilioEducacao +
                                                   mobilidade +
                                                   ajudaCusto;


            // Calcular verbas anuais (13º, FGTS, férias) reutilizando o calculador isolado
            var verbasProposta = _remuneracaoVerbasAnuaisCalculador.Calcular(
                calculoSalarioLiquido.SalarioLiquido,
                valorBaseRemuneracaoClt.Value,
                input.QuantidadeDependentes,
                tabelaInss,
                tabelaIrrf,
                tabelaReducaoIrrfFerias);

            decimal remuneracaoTotalLiquidaMensalComVerbasAnuais = remuneracaoTotalLiquidaMensal +
                verbasProposta.DecimoTerceiroMensal + verbasProposta.FgtsMensal + verbasProposta.FeriasMensal;

            decimal previsaoAnual = (remuneracaoTotalLiquidaMensal * 12m) + verbasProposta.DecimoTerceiroAnual +
                verbasProposta.FgtsAnual + verbasProposta.FeriasLiquido;

            decimal brutoComposto = valorBaseRemuneracaoClt.Value + valeAlimentacao + valeRefeicao + auxilioEducacao + ajudaCusto + mobilidade;

            // Construir a remuneração pretendida
            var remuneracaoProposta = new RemuneracaoDTO
            {
                CLT = calculoSalarioLiquido,
                ValeRefeicao = Math.Round(valeRefeicao, 2),
                ValeAlimentacao = Math.Round(valeAlimentacao, 2),
                AuxilioEducacao = Math.Round(auxilioEducacao, 2),
                Mobilidade = Math.Round(mobilidade, 2),
                AjudaDeCusto = Math.Round(ajudaCusto, 2),
                RemuneracaoTotalLiquidaMensal = Math.Round(remuneracaoTotalLiquidaMensal, 2),
                RemuneracaoTotalLiquidaMensalComVerbasAnuais = Math.Round(remuneracaoTotalLiquidaMensalComVerbasAnuais, 2),
                PrevisaoAnual = Math.Round(previsaoAnual, 2),
                CustoTotalEmpresa = Math.Round((brutoComposto + outrasDespesasEmpresa), 2),
                CustoVaga = Math.Round((custoVaga), 2),
                BrutoComposto = Math.Round(brutoComposto, 2)
            };
            return (valorBaseRemuneracaoClt, remuneracaoProposta, outrasDespesasEmpresa);
        }

        private static decimal CalcularAlimentacao(decimal? valorBaseRemuneracaoClt, decimal alimentacaoPretendida)
        {
            var alimentacaoCalculada = valorBaseRemuneracaoClt.Value* PoliticasRemuneracaoEmpresa.PercentualMaximoAlimentacaoBaixoRendimento;
            if(alimentacaoPretendida < alimentacaoCalculada)
                return alimentacaoPretendida;
            return alimentacaoCalculada;
        }

        /// <summary>
        /// Calcula o salário bruto necessário para atingir um salário líquido desejado
        /// Usa método iterativo (tentativa e erro) porque não há fórmula direta
        /// 
        /// POR QUE ITERATIVO?
        /// - Salário Líquido = Bruto - INSS(Bruto) - IRRF(Bruto - INSS - Dedução)
        /// - INSS e IRRF dependem do bruto (faixas progressivas)
        /// - Não há fórmula inversa direta, então testamos valores até encontrar
        /// </summary>
        private decimal CalcularSalarioBrutoParaLiquido(
            decimal liquidoDesejado,
            int numeroDependentes,
            InssTabelaDTO tabelaInss,
            IrrfTabelaDTO tabelaIrrf,
            IrrfReducaoDTO tabelaReducaoIrrf = null)
        {
            // ETAPA 1: Estimativa inicial inteligente
            // Começamos com uma estimativa baseada em descontos médios
            // INSS médio: ~11%, IRRF médio: ~5-10% (depende da faixa)
            // Então: Líquido ≈ Bruto * 0.80 a 0.85 (descontos totais ~15-20%)
            decimal salarioBrutoMin = liquidoDesejado * 1.15m; // Limite inferior (15% de descontos)
            decimal salarioBrutoMax = liquidoDesejado * 1.35m; // Limite superior (35% de descontos)

            // Limites de segurança
            int maxIteracoes = 50;
            decimal tolerancia = 0.01m; // Precisão de R$ 0,01
            int iteracao = 0;

            // ETAPA 2: Método de bissecção (mais eficiente que tentativa linear)
            // Dividimos o intervalo ao meio até encontrar o valor correto
            while (iteracao < maxIteracoes)
            {
                // Testar o ponto médio do intervalo
                decimal salarioBrutoTeste = (salarioBrutoMin + salarioBrutoMax) / 2m;

                // ETAPA 3: Calcular descontos sobre o valor de teste
                decimal descontoInss = _tributosCltService.CalcularInss(salarioBrutoTeste, tabelaInss);
                decimal baseCalculoIrrf = _tributosCltService.CalcularBaseIrrf(salarioBrutoTeste, descontoInss, numeroDependentes, tabelaIrrf);
                decimal descontoIrrf = _tributosCltService.CalcularIrrf(baseCalculoIrrf, tabelaIrrf, salarioBrutoTeste, tabelaReducaoIrrf);

                // ETAPA 4: Calcular líquido resultante
                decimal liquidoCalculado = salarioBrutoTeste - descontoInss - descontoIrrf;

                // ETAPA 5: Comparar com o desejado
                decimal diferenca = liquidoDesejado - liquidoCalculado;

                // Se a diferença for menor que a tolerância, encontramos!
                if (Math.Abs(diferenca) <= tolerancia)
                {
                    return Math.Round(salarioBrutoTeste, 2);
                }

                // ETAPA 6: Ajustar o intervalo
                // Se o líquido calculado é MENOR que o desejado, o bruto está BAIXO demais
                // Se o líquido calculado é MAIOR que o desejado, o bruto está ALTO demais
                if (liquidoCalculado < liquidoDesejado)
                {
                    // Precisamos de um bruto maior
                    salarioBrutoMin = salarioBrutoTeste;
                }
                else
                {
                    // Precisamos de um bruto menor
                    salarioBrutoMax = salarioBrutoTeste;
                }

                // Se os limites estão muito próximos, usar o último valor testado
                if (Math.Abs(salarioBrutoMax - salarioBrutoMin) < 0.01m)
                {
                    break;
                }

                iteracao++;
            }

            // Retornar o valor médio final
            return Math.Round((salarioBrutoMin + salarioBrutoMax) / 2m, 2);
        }

        /// <summary>
        /// Ajusta vale alimentação e ajuda de custo para respeitar a política (tetos por rendimento/CLT)
        /// e o orçamento da vaga: custoVaga &gt;= outrasDespesasEmpresa + somaBeneficios.
        /// </summary>
        private void AjustarValeAlimentacaoEAjudaCustoParaPoliticaECustoVaga(
            decimal clt,
            decimal custoVaga,
            decimal valeRefeicao,
            decimal assMedica,
            decimal mobilidade,
            decimal auxilioEducacao,
            out decimal valeAlimentacao,
            out decimal ajudaCusto)
        {
            decimal totalPacote(decimal vA, decimal aC)
            {
                decimal o = CalcularOutrasDespesasEmpresa(clt, valeRefeicao, assMedica, vA, aC, mobilidade, auxilioEducacao);
                decimal s = clt + valeRefeicao + vA + auxilioEducacao + mobilidade + aC;
                return o + s;
            }

            // Parte dos tetos máximos permitidos pela política (alimentação depende do rendimento → iteração)
            decimal vA = clt * PoliticasRemuneracaoEmpresa.PercentualMaximoAlimentacaoAltoRendimento;
            decimal aC = clt * PoliticasRemuneracaoEmpresa.PercentualMaximoAjudaCustoSobreCLT;

            for (int i = 0; i < 40; i++)
            {
                decimal outras = CalcularOutrasDespesasEmpresa(clt, valeRefeicao, assMedica, vA, aC, mobilidade, auxilioEducacao);
                decimal rendimento = custoVaga - outras;
                decimal maxAlim = PoliticasRemuneracaoEmpresa.CalcularValorMaximoAlimentacao(clt, rendimento);
                decimal maxAjuda = PoliticasRemuneracaoEmpresa.CalcularValorMaximoAjudaCusto(clt);
                decimal nvA = Math.Min(vA, maxAlim);
                decimal naC = Math.Min(aC, maxAjuda);
                if (nvA == vA && naC == aC)
                    break;
                vA = nvA;
                aC = naC;
            }

            if (totalPacote(vA, aC) <= custoVaga + 0.01m)
            {
                valeAlimentacao = Math.Round(vA, 2);
                ajudaCusto = Math.Round(aC, 2);
                return;
            }

            decimal lo = 0m;
            decimal hi = 1m;
            for (int b = 0; b < 60; b++)
            {
                decimal mid = (lo + hi) / 2m;
                if (totalPacote(vA * mid, aC * mid) <= custoVaga)
                    lo = mid;
                else
                    hi = mid;
            }

            vA *= lo;
            aC *= lo;

            for (int i = 0; i < 25; i++)
            {
                decimal outras = CalcularOutrasDespesasEmpresa(clt, valeRefeicao, assMedica, vA, aC, mobilidade, auxilioEducacao);
                decimal rendimento = custoVaga - outras;
                vA = Math.Min(vA, PoliticasRemuneracaoEmpresa.CalcularValorMaximoAlimentacao(clt, rendimento));
                aC = Math.Min(aC, PoliticasRemuneracaoEmpresa.CalcularValorMaximoAjudaCusto(clt));
            }

            int seguranca = 0;
            while (totalPacote(vA, aC) > custoVaga + 0.01m && seguranca < 500 && (vA > 0.01m || aC > 0.01m))
            {
                vA *= 0.99m;
                aC *= 0.99m;
                seguranca++;
            }

            valeAlimentacao = Math.Max(0, Math.Round(vA, 2));
            ajudaCusto = Math.Max(0, Math.Round(aC, 2));
        }

        /// <summary>
        /// Calcula as outras despesas da empresa: soma das seções Benefícios, Benefícios convencionais, Taxas IBRATI, Encargos Sociais e Encargos Rescisão.
        /// </summary>
        private decimal CalcularOutrasDespesasEmpresa(decimal clt, decimal valeRefeicao, decimal assMedica, decimal vA, decimal ajudaCusto, decimal mobilidade, decimal aE)
        {
            decimal feriasVAAjudaDeCustoMobilidadeAE = CalcularFeriasVAAjudaDeCustoMobilidadeAE(vA, ajudaCusto, mobilidade, aE);
            decimal feriasBeneficios = CalcularOutrasDespesasBeneficios(valeRefeicao, assMedica);
            decimal beneficiosConvencionais = CalcularOutrasDespesasBeneficiosConvencionais();
            decimal taxasIbrati = CalcularOutrasDespesasTaxasIbrati();
            decimal encargosSociais = CalcularOutrasDespesasEncargosSociais(clt);
            decimal encargosRescisao = CalcularOutrasDespesasEncargosRescisao(clt);

            return Math.Round(feriasVAAjudaDeCustoMobilidadeAE + feriasBeneficios + beneficiosConvencionais + taxasIbrati + encargosSociais + encargosRescisao, 2);
        }

        private decimal CalcularFeriasVAAjudaDeCustoMobilidadeAE(decimal vA, decimal ajudaCusto, decimal mobilidade, decimal aE)
        {
            return (vA + ajudaCusto + mobilidade + aE) / 12m;
        }

        /// <summary>
        /// 1 - Benefícios: 1/12 Benefícios (Férias) = (VR + AssMedica) / 12
        /// </summary>
        private static decimal CalcularOutrasDespesasBeneficios(decimal valeRefeicao, decimal assMedica)
        {
            var ferias = (valeRefeicao + assMedica) / 12m;
            return assMedica + ferias;
        }

        /// <summary>
        /// 2 - Benefícios convencionais: Auxílio Creche, Auxílio Creche Excepcional, Seguro de vida e 1/12 (Férias).
        /// </summary>
        private static decimal CalcularOutrasDespesasBeneficiosConvencionais()
        {
            const decimal auxilioCreche = 0m;
            const decimal auxilioCrecheExcepcional = 0m;
            const decimal seguroVida = 17.21m;
            var ferias = (auxilioCreche + auxilioCrecheExcepcional + seguroVida) / 12m;
            return auxilioCreche + auxilioCrecheExcepcional + seguroVida + ferias;
        }

        /// <summary>
        /// 3 - Taxas IBRATI: Administrativa + BWG (Administrativa / 12).
        /// </summary>
        private static decimal CalcularOutrasDespesasTaxasIbrati()
        {
            const decimal administrativa = 30.14m;
            decimal bwg = administrativa / 12m;
            return administrativa + bwg;
        }

        /// <summary>
        /// 4 - Encargos Sociais: 13º, férias, 1/3 férias, Terceiros, FGTS (CLT, 13º e férias+1/3).
        /// </summary>
        private static decimal CalcularOutrasDespesasEncargosSociais(decimal clt)
        {
            decimal decimoTerceiro = clt / 12m;
            decimal ferias = clt / 12m;
            decimal umTercoFerias = ferias / 3m;
            decimal terceiros = clt * 0.168m;
            decimal terceirosDecimoTerceiro = decimoTerceiro * 0.168m;
            decimal terceirosFeriasMaisUmTerco = (ferias + umTercoFerias) * 0.168m;
            decimal fgts = clt * 0.08m;
            decimal fgtsDecimoTerceiro = decimoTerceiro * 0.08m;
            decimal fgtsFeriasMaisUmTerco = (ferias + umTercoFerias) * 0.08m;

            return decimoTerceiro + ferias + umTercoFerias
                + terceiros + terceirosDecimoTerceiro + terceirosFeriasMaisUmTerco
                + fgts + fgtsDecimoTerceiro + fgtsFeriasMaisUmTerco;
        }

        /// <summary>
        /// 5 - Encargos Rescisão: INSS Aviso Prévio, Aviso Prévio, FGTS Aviso Prévio e Multa FGTS.
        /// </summary>
        private static decimal CalcularOutrasDespesasEncargosRescisao(decimal clt)
        {
            decimal decimoTerceiro = clt / 12m;
            decimal ferias = clt / 12m;
            decimal umTercoFerias = ferias / 3m;
            decimal fgts = clt * 0.08m;
            decimal fgtsDecimoTerceiro = decimoTerceiro * 0.08m;
            decimal fgtsFeriasMaisUmTerco = (ferias + umTercoFerias) * 0.08m;

            decimal avisoPrevio = clt / 12m;
            decimal inssAvisoPrevio = avisoPrevio * 0.168m;
            decimal fgtsAvisoPrevio = avisoPrevio * 0.08m;
            decimal multaFgts = (fgts + fgtsDecimoTerceiro + fgtsFeriasMaisUmTerco + fgtsAvisoPrevio) * 0.40m;

            return inssAvisoPrevio + avisoPrevio + fgtsAvisoPrevio + multaFgts;
        }

        ///// <summary>
        ///// Valor base (teto da faixa) em tb_admissao_remuneracao_clt: faixa1_final..faixa4_final conforme tb_vaga.tb_nivel_vaga_cod (1–4).
        ///// </summary>
        //private async Task<decimal?> ObterValorBaseRemuneracaoCltPorVagaAsync(VagaRecrutamentoDTO vaga, string AdmissaoCargoId, string NivelVagaCod)
        //{
        //    if (string.IsNullOrWhiteSpace(AdmissaoCargoId) || !Guid.TryParse(AdmissaoCargoId, out var admissaoCargoId))
        //        return 1621.00m;

        //    registro = await _remuneracaoCltRepository.ObterPorAdmissaoCargoIdAsync(admissaoCargoId);
        //    if (registro == null || !registro.Ativo)
        //    {
        //        registro = await _remuneracaoCltRepository.ObterPorAdmissaoCargoIdAsync(admissaoCargoId);
        //        if (registro == null || !registro.Ativo)
        //            return 1621.00m;
        //    }

        //    if (string.IsNullOrWhiteSpace(NivelVagaCod) || !int.TryParse(NivelVagaCod.Trim(), out int nivelCod) || nivelCod < 1 || nivelCod > 4)
        //        return 1621.00m;

        //    return PoliticasRemuneracaoEmpresa.ObterValorBaseFaixaFinalPorNivel(registro, nivelCod);
        //}

        /// <summary>
        /// Salário CLT (bruto) da proposta: 50% do valor da faixa final (conforme nível da vaga) ou, sem tabela/nível, 50% do custo da vaga;
        /// em seguida ajusta VA e ajuda de custo ao orçamento e política.
        /// </summary>
        private async Task<(decimal cltBruto, CalcularSalarioLiquidoOutputDTO salarioLiquido, decimal valeAlimentacao, decimal ajudaCusto)>
            ResolverCltBrutoPropostaComPoliticaAsync(
                decimal custoVaga,
                SimularRemuneracaoTotalInputDTO input,
                decimal valeRefeicao,
                decimal assMedica,
                decimal mobilidade,
                decimal auxilioEducacao,
                decimal? valorBaseRemuneracaoClt)
        {
            decimal clt = PoliticasRemuneracaoEmpresa.CalcularCltBrutoPropostaInicial(custoVaga, valorBaseRemuneracaoClt);

            var calcularSalarioLiquidoInput = new CalcularSalarioLiquidoInputDTO
            {
                SalarioBruto = clt,
                NumeroDependentes = input.QuantidadeDependentes,
                Ano = DateTime.Now.Year
            };

            var calculoSalarioLiquido = await CalcularSalarioLiquidoAsync(calcularSalarioLiquidoInput);

            AjustarValeAlimentacaoEAjudaCustoParaPoliticaECustoVaga(
                clt, custoVaga, valeRefeicao, assMedica, mobilidade, auxilioEducacao,
                out decimal valeAlimentacao, out decimal ajudaCusto);

            return (clt, calculoSalarioLiquido, valeAlimentacao, ajudaCusto);
        }

        /// <summary>
        /// Aplica validações de política da empresa em todos os campos da remuneração
        /// </summary>
        private bool AplicarValidacoesPolitica(
            RemuneracaoDTO remuneracao)
        {
            const decimal tolerancia = 0.02m;

            bool cltDentroDaPolitica;
            string mensagemOk;
            string mensagemFora;

            (decimal valorMinimoCLT,string msgClt) = PoliticasRemuneracaoEmpresa.CalcularValorMinimoCLTBrutoPelaRegra60PorcentoBrutoTotalOu50PorCentoDoPiso(
                remuneracao.AjudaDeCusto,
                remuneracao.Mobilidade,
                remuneracao.ValeAlimentacao,
                remuneracao.ValeRefeicao,
                remuneracao.CLT.SalarioBruto);
            cltDentroDaPolitica = remuneracao.CLT.SalarioBruto >= valorMinimoCLT - tolerancia;
            mensagemOk = $"O valor da CLT está dentro da política";
            mensagemFora = $"O valor da CLT está abaixo do mínimo permitido.";

            remuneracao.ValidacaoCLT = new ValidacaoPoliticaDTO
            {
                DentroDaPolitica = cltDentroDaPolitica,
                Mensagem = cltDentroDaPolitica ? mensagemOk : mensagemFora
            };

            // Validar Ajuda de Custo: deve ser no máximo 20% do CLT
            decimal valorMaximoAjudaCusto = PoliticasRemuneracaoEmpresa.CalcularValorMaximoAjudaCusto(remuneracao.CLT.SalarioBruto);
            bool ajudaCustoDentroDaPolitica = remuneracao.AjudaDeCusto <= Math.Round(valorMaximoAjudaCusto, 2);
            remuneracao.ValidacaoAjudaDeCusto = new ValidacaoPoliticaDTO
            {
                DentroDaPolitica = ajudaCustoDentroDaPolitica,
                Mensagem = ajudaCustoDentroDaPolitica
                    ? $"A ajuda de custo está dentro da política (máximo de {valorMaximoAjudaCusto:C2})"
                    : $"A ajuda de custo excede o máximo permitido. Valor máximo: {valorMaximoAjudaCusto:C2}"
            };

            // Validar Vale Alimentação: depende do RendimentoTotal
            decimal valorMaximoAlimentacao = PoliticasRemuneracaoEmpresa.CalcularValorMaximoAlimentacao(remuneracao.CLT.SalarioBruto, remuneracao.BrutoComposto);
            bool alimentacaoDentroDaPolitica = remuneracao.ValeAlimentacao <= valorMaximoAlimentacao;
            decimal percentualMaximo = 0;

            var msg = String.Empty;
            if (remuneracao.BrutoComposto > PoliticasRemuneracaoEmpresa.LimiteRendimentoTotalParaAlimentacao)
            {
                msg = $"O vale alimentação excede o máximo permitido. Valor máximo: {valorMaximoAlimentacao:C2} ({percentualMaximo:F0}% do Bruto Composto";
                percentualMaximo = PoliticasRemuneracaoEmpresa.PercentualMaximoAlimentacaoAltoRendimento * 100;
            }
            else
            {
                msg = $"O vale alimentação excede o máximo permitido. Valor máximo: {valorMaximoAlimentacao:C2} ({percentualMaximo:F0}% do CLT)";
                percentualMaximo = PoliticasRemuneracaoEmpresa.PercentualMaximoAlimentacaoBaixoRendimento * 100;
            }

            remuneracao.ValidacaoValeAlimentacao = new ValidacaoPoliticaDTO
            {
                DentroDaPolitica = alimentacaoDentroDaPolitica,
                Mensagem = alimentacaoDentroDaPolitica
                    ? $"O vale alimentação está dentro da política (máximo de {percentualMaximo:F0}% do CLT = {valorMaximoAlimentacao:C2})"
                    : msg
            };

            // Validar Auxílio Educação: deve ser no máximo 25% do CLT
            decimal valorMaximoAuxilioEducacao = PoliticasRemuneracaoEmpresa.CalcularValorMaximoAuxilioEducacao(remuneracao.CLT.SalarioBruto);
            bool auxilioEducacaoDentroDaPolitica = remuneracao.AuxilioEducacao <= Math.Round(valorMaximoAuxilioEducacao, 2);
            remuneracao.ValidacaoAuxilioEducacao = new ValidacaoPoliticaDTO
            {
                DentroDaPolitica = auxilioEducacaoDentroDaPolitica,
                Mensagem = auxilioEducacaoDentroDaPolitica
                    ? $"O auxílio educação está dentro da política"
                    : $"O auxílio educação excede o máximo permitido."
            };

            // Validar Vale Refeição: não há política específica, sempre dentro
            remuneracao.ValidacaoValeRefeicao = new ValidacaoPoliticaDTO
            {
                DentroDaPolitica = true,
                Mensagem = "O vale refeição está dentro da política"
            };

            // Validar Mobilidade: deve ser no máximo 50% do CLT Bruto
            decimal valorMaximoMobilidade = PoliticasRemuneracaoEmpresa.CalcularValorMaximoMobilidade(remuneracao.CLT.SalarioBruto);
            bool mobilidadeDentroDaPolitica = remuneracao.Mobilidade <= valorMaximoMobilidade;
            remuneracao.ValidacaoMobilidade = new ValidacaoPoliticaDTO
            {
                DentroDaPolitica = mobilidadeDentroDaPolitica,
                Mensagem = mobilidadeDentroDaPolitica
                    ? $"A mobilidade está dentro da política (máximo de 50% do CLT Bruto = {valorMaximoMobilidade:C2})"
                    : $"A mobilidade excede o máximo permitido. Valor máximo: {valorMaximoMobilidade:C2} (50% do CLT Bruto)"
            };

            return cltDentroDaPolitica && ajudaCustoDentroDaPolitica && alimentacaoDentroDaPolitica && auxilioEducacaoDentroDaPolitica && mobilidadeDentroDaPolitica;
        }

        /// <summary>
        /// Calcula o valor da mobilidade baseado no modelo de trabalho, quantidade de dias presenciais e KM
        /// </summary>
        /// <param name="modeloTrabalhoCod">Código do modelo de trabalho (1 = 100% Presencial, 2 = Híbrido, 3 = 100% Remoto)</param>
        /// <param name="quantidadeDiasPresencial">Quantidade de dias presenciais (para híbrido: 2 ou 3)</param>
        /// <param name="km">Quantidade de quilômetros</param>
        /// <returns>Valor da mobilidade calculado</returns>
        private decimal CalcularMobilidade(int? modeloTrabalhoCod, int? quantidadeDiasPresencial, decimal km, decimal mobilidade)
        {
            decimal defaultMobilidade = 300.00m;
            // Se não tiver código, retorna valor fixo padrão (remoto)
            if (!modeloTrabalhoCod.HasValue)
            {
                return defaultMobilidade;
            }

            // 100% Remoto - Código 3
            if (modeloTrabalhoCod.Value == 3)
            {
                if(mobilidade < defaultMobilidade)
                    return mobilidade;
                return defaultMobilidade;
            }

            // Se KM > 200, usar a regra especial: (KM * R$ 1,10) + R$ 449,00 + R$ 360,00
            if (km > 200m)
            {
                var mobCalculada = (km * 1.10m) + 449.00m + 360.00m;
                if (mobilidade < mobCalculada)
                    return mobilidade;
                return mobCalculada;
            }

            // 100% Presencial - Código 1
            if (modeloTrabalhoCod.Value == 1)
            {
                var mobCalculada = 22m * km * 1.50m;
                if (mobilidade < mobCalculada)
                    return mobilidade;
                return mobCalculada;
            }

            // Híbrido - Código 2
            if (modeloTrabalhoCod.Value == 2)
            {
                int qtdeVisitas = quantidadeDiasPresencial switch
                {
                    2 => 8,   // Híbrido 2 dias/semana
                    3 => 12,  // Híbrido 3 dias/semana
                    _ => 8    // Default: 2 dias/semana
                };

                var mobCalculada = qtdeVisitas * km * 1.50m;
                if (mobilidade < mobCalculada)
                    return mobilidade;
                return mobCalculada;
            }

            // Se não identificar o modelo, retorna valor fixo padrão (remoto)
            return defaultMobilidade;
        }

        public async Task<CandidatosMatchResponse> CalcularAderenciaColaboradorVaga(string vagaId, string codigoInternoColaborador)
        {
            if (string.IsNullOrWhiteSpace(vagaId))
                throw new ArgumentException("O ID da vaga é obrigatório.");

            if (string.IsNullOrWhiteSpace(codigoInternoColaborador))
                throw new ArgumentException("O código interno do colaborador é obrigatório.");

            // Buscar informações da vaga
            var vaga = await _vagaFourmakersRepository.ObterVagaRecrutamentoPorId(vagaId);
            if (vaga == null)
                throw new ApplicationException("Vaga não encontrada.");

            // Montar o request para o match baseado nas skills da vaga
            var request = new ScoreSingleCandidateRequest
            {
                CodigoInternoColaborador = codigoInternoColaborador,
                HardSkills = vaga.Skills
                    .Where(s => s.TipoSkillId == (int)ItemPerfilEnum.COMPETENCIA)
                    .Select(s => new SkillItem
                    {
                        Nome = s.SkillDescription,
                        Nivel = s.SkillNivelDescription ?? string.Empty,
                        Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel"
                    }).ToList(),
                SoftSkills = vaga.Skills
                    .Where(s => s.TipoSkillId == (int)ItemPerfilEnum.SOFTSKILL)
                    .Select(s => new SkillItem
                    {
                        Nome = s.SkillDescription,
                        Nivel = s.SkillNivelDescription ?? string.Empty,
                        Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel"
                    }).ToList(),
                Metodologias = vaga.Skills
                    .Where(s => s.TipoSkillId == (int)ItemPerfilEnum.METODOLOGIA)
                    .Select(s => new SkillItem
                    {
                        Nome = s.SkillDescription,
                        Nivel = s.SkillNivelDescription ?? string.Empty,
                        Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel"
                    }).ToList(),
                DominiosNegocio = vaga.Skills
                    .Where(s => s.TipoSkillId == (int)ItemPerfilEnum.DOMINIONEGOCIO)
                    .Select(s => new SkillItem
                    {
                        Nome = s.SkillDescription,
                        Nivel = s.SkillNivelDescription ?? string.Empty,
                        Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel"
                    }).ToList(),
                Idiomas = vaga.Skills
                    .Where(s => s.TipoSkillId == (int)ItemPerfilEnum.IDIOMA)
                    .Select(s => new SkillItem
                    {
                        Nome = s.SkillDescription,
                        Nivel = s.SkillNivelDescription ?? string.Empty,
                        Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel"
                    }).ToList(),
                PesoHardSkills = 1,
                PesoSoftSkills = 1,
                PesoMetodologias = 1,
                PesoDominiosNegocio = 1,
                PesoIdiomas = 1,
                PesoDisponibilidades = 1,
                VisibleToOrgIds = new List<int> { vaga.OrgId.ToInt(), EnumORG.FOURMAKERS_1.ToInt(), EnumORG.FMU_7.ToInt() },
                Disponibilidades = new List<DisponibilidadeItem>(),
                NumeroDeCandidatos = 1
            };

            // Chamar o match via MatchService (com contexto de log para tb_labs_log_score_single_candidates)
            var logContext = new ScoreSingleCandidateLogContext
            {
                OrgId = vaga.OrgId.ToInt(),
                VagaId = vagaId,
                CodigoInternoColaborador = codigoInternoColaborador
            };
            var resultado = await _matchService.ScoreSingleCandidate(request, logContext);

            return resultado.Response;
        }
    }
}