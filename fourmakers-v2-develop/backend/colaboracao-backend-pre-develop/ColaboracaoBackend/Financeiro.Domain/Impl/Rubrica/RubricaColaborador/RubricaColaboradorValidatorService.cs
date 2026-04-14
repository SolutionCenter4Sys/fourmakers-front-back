using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.Financeiro.Rubrica;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaborador;
using Financeiro.Domain.Impl.Rubrica.RubricaColaborador.Constants;
using Financeiro.Domain.Impl.Rubrica.Rubrica.Constants;
using Financeiro.Domain.Interfaces.Rubrica.RubricaColaborador;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Logs.Infra.Attributes;

namespace Financeiro.Domain.Impl.Rubrica.RubricaColaborador
{
    [LogDomainClass]
    public class RubricaColaboradorValidatorService : IRubricaColaboradorValidatorService
    {
        private readonly IRubricaColaboradorRepository _rubricaColaboradorRepository;
        private readonly IRubricaRepository _rubricaRepository;

        public RubricaColaboradorValidatorService(IRubricaColaboradorRepository rubricaColaboradorRepository,
                                                  IRubricaRepository rubricaRepository)
        {
            _rubricaColaboradorRepository = rubricaColaboradorRepository;
            _rubricaRepository = rubricaRepository;
        }

        public async Task ValidaRubricaColaborador(RubricaColaboradorInput input, CRUDEnum cRUDEnum)
        {
            if (cRUDEnum == CRUDEnum.Update || cRUDEnum == CRUDEnum.Delete)
            {
                await ValidaSeExisteRubricaColaborador(input.Id);
            }

            if (cRUDEnum == CRUDEnum.Create || cRUDEnum == CRUDEnum.Update)
            {
                await ValidarCamposDeEntrada(input, cRUDEnum);
                await ValidarRubricaExiste(input.RubricaId);
                await ValidarColaboradorExiste(input.CodigoInternoColaborador);
                await ValidarCamposDeCalculoPorTipo(input);
                ValidarMesAno(input);
            }
        }

        private async Task ValidarColaboradorExiste(Guid codigoInternoColaborador)
        {
            if (codigoInternoColaborador.IsNotEmpty())
            {
                var encontrouCodigoInternoColaborador = await _rubricaColaboradorRepository.ObterColaboradorPorCodigo(codigoInternoColaborador);
                if (encontrouCodigoInternoColaborador == null)
                {
                    throw new ApplicationException($"CodigoInternoColaborador {codigoInternoColaborador} informado não existe no DB.");
                }
            }
        
        }

        private async Task ValidarCamposDeEntrada(RubricaColaboradorInput input, CRUDEnum cRUDEnum)
        {
            var campos = new List<CampoValidacao>();

            if (cRUDEnum == CRUDEnum.Update)
                campos.Add(new("Id", input.Id, TipoValidacaoEnum.Obrigatoriedade));

            campos.Add(new("Id", input.Id, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Ativo", input.Ativo, TipoValidacaoEnum.Obrigatoriedade));
            //campos.Add(new("CodigoInternoColaboradorAlteracao", input.CodigoInternoColaboradorAlteracao, TipoValidacaoEnum.Obrigatoriedade));
            //campos.Add(new("CodigoInternoColaboradorAlteracao", input.CodigoInternoColaboradorAlteracao, TipoValidacaoEnum.TamanhoExato) { TamanhoExato = 36 });
            campos.Add(new("RubricaId", input.RubricaId, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("CodigoInternoColaborador", input.CodigoInternoColaborador, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("CodigoRubricaFrequencia", input.CodigoRubricaFrequencia, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Observação", input.Observacao, TipoValidacaoEnum.TamanhoMaximo) { TamanhoMaximo = 1000 });
            if (!string.IsNullOrWhiteSpace(input.Hora))
                campos.Add(new("Hora", input.Hora, TipoValidacaoEnum.ValidarFormatoHora));
            campos.Add(new ("MesInicial", input.MesInicial,TipoValidacaoEnum.Obrigatoriedade ));
            campos.Add(new ("AnoInicial", input.AnoInicial,TipoValidacaoEnum.Obrigatoriedade ));

            await ValidadorCamposUtil.ValidaCampos(campos);
        }

        private async Task ValidaSeExisteRubricaColaborador(Guid id)
        {
            var existente = await _rubricaColaboradorRepository.ObterRubricaColaboradorPorIdAsync(id);
            if (existente == null)
                throw new ApplicationException($"Rubrica colaborador com id {id} não encontrada.");

            var currentDate = DateTime.UtcNow;
            var month = currentDate.Month;
            var year = currentDate.Year;
            if (existente.MesInicial < month && existente.AnoInicial < year)
            {
                throw new ApplicationException($"Não é possivel excluir ou editar uma rubrica com o mes e ano anterior ao atual.");
            }
        }

        private async Task ValidarRubricaExiste(Guid rubricaId)
        {
            var existe = (await _rubricaRepository.ObterRubricaPorIdAsync(rubricaId)) != null;
            if (!existe)
                throw new ApplicationException("RubricaId informada não existe.");
        }

        private async Task ValidarVigenciaExiste(Guid? vigenciaId)
        {
            if (vigenciaId != null)
            {
                var data = await _rubricaColaboradorRepository.ObterDataVigenciaPorId(vigenciaId.Value);
                if (data == null)
                {
                    throw new ApplicationException($"Vigência {vigenciaId} informada não existe no DB.");
                }
            }
        }

        private async Task ValidarCamposDeCalculoPorTipo(RubricaColaboradorInput input)
        {
            // Obter o tipo de cálculo da rubrica
            var rubrica = await _rubricaRepository.ObterRubricaPorIdAsync(input.RubricaId);
            if (rubrica == null)
                throw new ApplicationException("Rubrica não encontrada.");

            bool valorPreenchido = input.Valor > 0;
            bool percentualPreenchido = input.Percentual > 0;
            bool horaPreenchida = !string.IsNullOrWhiteSpace(input.Hora);

            switch (rubrica.CalculoTipo)
            {
                case CalculoTipoConstant.VALOR:
                    if (!valorPreenchido)
                        throw new ApplicationException("Para rubrica do tipo 'Valor', o campo Valor é obrigatório.");
                    if (percentualPreenchido || horaPreenchida)
                        throw new ApplicationException("Para rubrica do tipo 'Valor', apenas o campo Valor deve ser preenchido.");
                    break;

                case CalculoTipoConstant.PORCENTAGEM:
                    if (!percentualPreenchido)
                        throw new ApplicationException("Para rubrica do tipo 'Porcentagem', o campo Percentual é obrigatório.");
                    if (valorPreenchido || horaPreenchida)
                        throw new ApplicationException("Para rubrica do tipo 'Porcentagem', apenas o campo Percentual deve ser preenchido.");
                    break;

                case CalculoTipoConstant.HORA:
                    if (!horaPreenchida)
                        throw new ApplicationException("Para rubrica do tipo 'Hora', o campo Hora é obrigatório.");
                    if (valorPreenchido || percentualPreenchido)
                        throw new ApplicationException("Para rubrica do tipo 'Hora', apenas o campo Hora deve ser preenchido.");
                    break;

                default:
                    throw new ApplicationException($"Tipo de cálculo '{rubrica.CalculoTipo}' não é válido.");
            }
        }

        private void ValidarMesAno(RubricaColaboradorInput input)
        {
            //Valida mês e ano inicial
            if (input.MesInicial < 1 || input.MesInicial > 12)
                throw new ArgumentException("Mês inicial inválido. Deve estar entre 1 e 12.");

            if (input.AnoInicial < 1000 || input.AnoInicial > 9999)
                throw new ArgumentException("Ano inicial inválido. Deve conter 4 dígitos.");

            //Valida presença conjunta de mês/ano final
            bool mesFinalInformado = input.MesFinal.HasValue;
            bool anoFinalInformado = input.AnoFinal.HasValue;

            if (mesFinalInformado ^ anoFinalInformado) // apenas um informado (Operador XOR Alternativa curta do == null, ambos devem ser true para continuar)
                throw new ArgumentException("Se mês final for informado, o ano final também deve ser (e vice-versa).");

            //Valida que data final é >= data inicial, se ambos informados
            if (mesFinalInformado && anoFinalInformado)
            {
                if (input.MesFinal < 1 || input.MesFinal > 12)
                    throw new ArgumentException("Mês final inválido. Deve estar entre 1 e 12.");

                if (input.AnoFinal < 1000 || input.AnoFinal > 9999)
                    throw new ArgumentException("Ano final inválido. Deve conter 4 dígitos.");

                var dataInicial = new DateTime(input.AnoInicial, input.MesInicial, 1);
                var dataFinal = new DateTime(input.AnoFinal.Value, input.MesFinal.Value, 1);

                if (dataFinal < dataInicial)
                    throw new ArgumentException("Data final não pode ser menor que a data inicial.");
            }
        }
    }
}
