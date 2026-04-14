using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DomainModel.Calculos;
using Core.Domain.SRS;
using Core.Domain.Vaga;
using DataTransferObject.Domain.Calculos;
using DataTransferObject.Domain.SRS.RemuneracaoClt;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.VagasSRS;
using Labs.Domain.Interfaces;
using Moq;
using SRS.Domain.Impl.Service;
using SRS.Domain.Interfaces.Service;
using SRS.Domain.Interfaces.Service.Validadores;
using Xunit;

namespace SRS.Domain.Tests
{
    /// <summary>
    /// Snapshot de <see cref="CalculosService.SimularRemuneracaoTotalAsync"/> com o mesmo body do curl
    /// (idVaga, líquido pretendido, benefícios, admissaoCargoId, nivelVagaCod), vaga com custo 15120 e
    /// tabelas INSS/IRRF 2026 mockadas. Os totais esperados são os produzidos por este serviço com esses mocks;
    /// a resposta da API dev pode diferir levemente se <c>tb_admissao_remuneracao_clt</c> ou tabelas reais divergirem.
    /// </summary>
    public class SimularRemuneracaoTotalSnapshotTests
    {
        private const int AnoBase = 2026;
        private const decimal ToleranciaCentavos = 0.02m;

        /// <summary>
        /// Corpo do POST /api/Vaga/Calculos/SimularRemuneracaoTotal (dev) usado como referência.
        /// </summary>
        private static SimularRemuneracaoTotalInputDTO CriarInputFixo()
        {
            return new SimularRemuneracaoTotalInputDTO
            {
                IdVaga = Guid.Parse("0034b4de-b69c-4206-9c25-b6a3f967e358"),
                LiquidoPretendido = 10000.00m,
                QuantidadeDependentes = 2,
                Alimentacao = 679.19m,
                Mobilidade = 300m,
                Educacao = 0m,
                Km = 0m,
                AdmissaoCargoId = Guid.Parse("f7685a03-26da-11f1-8c05-12248fda2329"),
                NivelVagaCod = 3
            };
        }

        /// <summary>
        /// Custo da vaga = CustoProfissional * 168 = 15120 (conforme resposta da API).
        /// </summary>
        private static VagaRecrutamentoDTO CriarVagaFake(Guid idVaga)
        {
            return new VagaRecrutamentoDTO
            {
                Id = idVaga.ToString(),
                CustoProfissional = 15120.00m / 168m,
                Frequencia = "2",
                ModeloTrabalhoCod = 3,
                Skills = new List<VagaSkillRecrutamentoDTO>()
            };
        }

        /// <summary>
        /// Registro mínimo para validações de política (Piso e faixa do nível 3).
        /// Faixa3_final &gt; 0 evita retorno nulo em ObterValorBaseRemuneracaoCltPorVagaAsync.
        /// </summary>
        private static RemuneracaoCltResult CriarRegistroRemuneracaoCltFake()
        {
            // Piso tal que 25% do piso &lt; CLT ao longo da convergência (evita parada prematura
            // em CalculosService) e compatível com a política de CLT mínimo na API de referência.
            const decimal piso = 18000m;
            return new RemuneracaoCltResult
            {
                Ativo = true,
                Piso = piso,
                Faixa1Final = 12000m,
                Faixa2Final = 15000m,
                Faixa3Final = 17361.06m,
                Faixa4Final = 22000m
            };
        }

        private static InssTabelaDTO TabelaInss2026()
        {
            return new InssTabelaDTO
            {
                Ano = AnoBase,
                Faixa1Min = 0,
                Faixa1Max = 1621.00m,
                Faixa1Aliquota = 7.50m,
                Faixa1ParcelaDeduzir = 0m,
                Faixa2Min = 1621.01m,
                Faixa2Max = 2902.84m,
                Faixa2Aliquota = 9.00m,
                Faixa2ParcelaDeduzir = 24.32m,
                Faixa3Min = 2902.85m,
                Faixa3Max = 4354.27m,
                Faixa3Aliquota = 12.00m,
                Faixa3ParcelaDeduzir = 111.40m,
                Faixa4Min = 4354.28m,
                Faixa4Max = 8475.55m,
                Faixa4Aliquota = 14.00m,
                Faixa4ParcelaDeduzir = 198.49m,
                TetoInss = 8475.55m
            };
        }

        /// <summary>
        /// Tabela IRRF alinhada ao payload retornado pela API (faixa 5 com valorMinimo 4664.68).
        /// </summary>
        private static IrrfTabelaDTO TabelaIrrf2026()
        {
            return new IrrfTabelaDTO
            {
                Ano = AnoBase,
                Faixa1Min = 0m,
                Faixa1Max = 2428.80m,
                Faixa1Aliquota = 0m,
                Faixa1Deducao = 0m,
                Faixa2Min = 2428.81m,
                Faixa2Max = 2826.65m,
                Faixa2Aliquota = 7.50m,
                Faixa2Deducao = 182.16m,
                Faixa3Min = 2826.66m,
                Faixa3Max = 3751.05m,
                Faixa3Aliquota = 15.00m,
                Faixa3Deducao = 394.16m,
                Faixa4Min = 3751.06m,
                Faixa4Max = 4664.68m,
                Faixa4Aliquota = 22.50m,
                Faixa4Deducao = 675.49m,
                Faixa5Min = 4664.68m,
                Faixa5Max = 99999999.99m,
                Faixa5Aliquota = 27.50m,
                Faixa5Deducao = 908.73m,
                DeducaoPorDependente = 189.59m
            };
        }

        private static IrrfReducaoDTO TabelaReducaoIrrf2026()
        {
            return new IrrfReducaoDTO
            {
                Ano = AnoBase,
                Faixa1Max = 2000m,
                Faixa1DescontoMaximo = 0m,
                Faixa2Min = 2000.01m,
                Faixa2Max = 10000m,
                Faixa2ValorBase = 0m,
                Faixa2Coeficiente = 0m,
                Faixa3Min = 10000.01m
            };
        }

        private static ICalculosService CriarCalculosService(
            SimularRemuneracaoTotalInputDTO input,
            out VagaRecrutamentoDTO vagaFake)
        {
            vagaFake = CriarVagaFake(input.IdVaga);
            var registroFake = CriarRegistroRemuneracaoCltFake();

            var mockValidator = new Mock<ICalculosValidatorService>();
            mockValidator
                .Setup(x => x.ValidarSimularRemuneracaoTotal(It.IsAny<SimularRemuneracaoTotalInputDTO>()))
                .Returns(Task.CompletedTask);

            var mockInss = new Mock<IInssRepository>();
            mockInss
                .Setup(x => x.ObterTabelaInssPorAnoAsync(It.IsAny<int>()))
                .ReturnsAsync(TabelaInss2026());

            var mockIrrf = new Mock<IIrrfRepository>();
            mockIrrf
                .Setup(x => x.ObterTabelaIrrfPorAnoAsync(It.IsAny<int>()))
                .ReturnsAsync(TabelaIrrf2026());

            var mockIrrfReducao = new Mock<IIrrfReducaoRepository>();
            mockIrrfReducao
                .Setup(x => x.ObterReducaoPorAnoAsync(It.IsAny<int>()))
                .ReturnsAsync(TabelaReducaoIrrf2026());

            var mockVaga = new Mock<IVagaFourmakersRepository>();
            mockVaga
                .Setup(x => x.ObterVagaRecrutamentoPorId(It.IsAny<string>()))
                .ReturnsAsync(vagaFake);

            var mockMatch = new Mock<IMatchService>();

            var mockRemuneracaoClt = new Mock<IRemuneracaoCltRepository>();
            mockRemuneracaoClt
                .Setup(x => x.ObterPorAdmissaoCargoIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(registroFake);

            var tributosCltService = new TributosCltService();
            var verbasCalculador = new RemuneracaoVerbasAnuaisCalculador(tributosCltService);

            return new CalculosService(
                mockValidator.Object,
                tributosCltService,
                verbasCalculador,
                mockInss.Object,
                mockIrrf.Object,
                mockIrrfReducao.Object,
                mockVaga.Object,
                mockMatch.Object,
                mockRemuneracaoClt.Object
            );
        }

        private static void AssertDecimal(decimal expected, decimal actual, string campo)
        {
            Assert.True(
                Math.Abs(expected - actual) <= ToleranciaCentavos,
                $"{campo}: esperado {expected}, obtido {actual} (tolerância {ToleranciaCentavos})");
        }

        [Fact]
        public async Task SimularRemuneracaoTotal_InputFixo_Proposta_MantemResultadosEsperados()
        {
            var input = CriarInputFixo();
            var service = CriarCalculosService(input, out _);

            var result = await service.SimularRemuneracaoTotalAsync(input);

            Assert.NotNull(result?.PrimeiraOpcao);
            var p = result.PrimeiraOpcao;

            Assert.NotNull(p.CLT);
            // Snapshot: mesmo body do curl (liquido 10k, VA, mobilidade, ids) + mocks INSS/IRRF e remuneração CLT.
            AssertDecimal(8677.70m, p.CLT.SalarioBruto, nameof(p.CLT.SalarioBruto));
            AssertDecimal(6587.97m, p.CLT.SalarioLiquido, nameof(p.CLT.SalarioLiquido));
            AssertDecimal(988.09m, p.CLT.DescontoInss, nameof(p.CLT.DescontoInss));
            AssertDecimal(1101.64m, p.CLT.DescontoIrrf, nameof(p.CLT.DescontoIrrf));

            AssertDecimal(704.00m, p.ValeRefeicao, nameof(p.ValeRefeicao));
            AssertDecimal(679.19m, p.ValeAlimentacao, nameof(p.ValeAlimentacao));
            AssertDecimal(300.00m, p.Mobilidade, nameof(p.Mobilidade));
            AssertDecimal(1735.54m, p.AjudaDeCusto, nameof(p.AjudaDeCusto));
            AssertDecimal(10006.70m, p.RemuneracaoTotalLiquidaMensal, nameof(p.RemuneracaoTotalLiquidaMensal));
            AssertDecimal(11337.31m, p.RemuneracaoTotalLiquidaMensalComVerbasAnuais, nameof(p.RemuneracaoTotalLiquidaMensalComVerbasAnuais));
            AssertDecimal(136047.67m, p.PrevisaoAnual, nameof(p.PrevisaoAnual));
            AssertDecimal(18511.42m, p.CustoTotalEmpresa, nameof(p.CustoTotalEmpresa));
            AssertDecimal(12096.43m, p.BrutoComposto, nameof(p.BrutoComposto));
            AssertDecimal(15120.00m, p.CustoVaga, nameof(p.CustoVaga));
        }

        [Fact]
        public async Task SimularRemuneracaoTotal_InputFixo_Pretendida_MantemResultadosEsperados()
        {
            var input = CriarInputFixo();
            var service = CriarCalculosService(input, out _);

            var result = await service.SimularRemuneracaoTotalAsync(input);

            Assert.NotNull(result?.SegundaOpcao);
            var p = result.SegundaOpcao;

            Assert.NotNull(p.CLT);
            AssertDecimal(5094.13m, p.CLT.SalarioBruto, nameof(p.CLT.SalarioBruto));
            AssertDecimal(4309.87m, p.CLT.SalarioLiquido, nameof(p.CLT.SalarioLiquido));
            AssertDecimal(514.69m, p.CLT.DescontoInss, nameof(p.CLT.DescontoInss));
            AssertDecimal(269.57m, p.CLT.DescontoIrrf, nameof(p.CLT.DescontoIrrf));

            AssertDecimal(704.00m, p.ValeRefeicao, nameof(p.ValeRefeicao));
            AssertDecimal(679.19m, p.ValeAlimentacao, nameof(p.ValeAlimentacao));
            AssertDecimal(300m, p.Mobilidade, nameof(p.Mobilidade));
            AssertDecimal(4006.94m, p.AjudaDeCusto, nameof(p.AjudaDeCusto));
            AssertDecimal(10000.00m, p.RemuneracaoTotalLiquidaMensal, nameof(p.RemuneracaoTotalLiquidaMensal));
            AssertDecimal(10842.44m, p.RemuneracaoTotalLiquidaMensalComVerbasAnuais, nameof(p.RemuneracaoTotalLiquidaMensalComVerbasAnuais));
            AssertDecimal(130109.23m, p.PrevisaoAnual, nameof(p.PrevisaoAnual));
            AssertDecimal(15110.97m, p.CustoTotalEmpresa, nameof(p.CustoTotalEmpresa));
            AssertDecimal(10784.26m, p.BrutoComposto, nameof(p.BrutoComposto));
            AssertDecimal(15120.00m, p.CustoVaga, nameof(p.CustoVaga));
        }

        [Fact]
        public async Task SimularRemuneracaoTotal_InputFixo_TerceiraOpcao_MantemResultadosEsperados()
        {
            var input = CriarInputFixo();
            var service = CriarCalculosService(input, out _);

            var result = await service.SimularRemuneracaoTotalAsync(input);

            Assert.NotNull(result?.TerceiraOpcao);
            var p = result.TerceiraOpcao;

            Assert.NotNull(p.CLT);
            AssertDecimal(7584.46m, p.CLT.SalarioBruto, nameof(p.CLT.SalarioBruto));
            AssertDecimal(5885.82m, p.CLT.SalarioLiquido, nameof(p.CLT.SalarioLiquido));
            AssertDecimal(863.34m, p.CLT.DescontoInss, nameof(p.CLT.DescontoInss));
            AssertDecimal(835.30m, p.CLT.DescontoIrrf, nameof(p.CLT.DescontoIrrf));

            AssertDecimal(704.00m, p.ValeRefeicao, nameof(p.ValeRefeicao));
            AssertDecimal(679.19m, p.ValeAlimentacao, nameof(p.ValeAlimentacao));
            AssertDecimal(300.00m, p.Mobilidade, nameof(p.Mobilidade));
            AssertDecimal(1516.89m, p.AjudaDeCusto, nameof(p.AjudaDeCusto));
            AssertDecimal(9085.91m, p.RemuneracaoTotalLiquidaMensal, nameof(p.RemuneracaoTotalLiquidaMensal));
            AssertDecimal(10262.65m, p.RemuneracaoTotalLiquidaMensalComVerbasAnuais, nameof(p.RemuneracaoTotalLiquidaMensalComVerbasAnuais));
            AssertDecimal(123151.76m, p.PrevisaoAnual, nameof(p.PrevisaoAnual));
            AssertDecimal(16486.50m, p.CustoTotalEmpresa, nameof(p.CustoTotalEmpresa));
            AssertDecimal(10784.55m, p.BrutoComposto, nameof(p.BrutoComposto));
            AssertDecimal(15120.00m, p.CustoVaga, nameof(p.CustoVaga));
        }
    }
}
