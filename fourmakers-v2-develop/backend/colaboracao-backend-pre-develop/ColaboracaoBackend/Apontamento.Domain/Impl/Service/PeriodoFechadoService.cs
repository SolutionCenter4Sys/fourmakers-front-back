using Apontamento.Domain.Interfaces.Service;
using Colaboracao.Core;
using Colaboracao.Helper;
using Core.Domain.Apontamento;
using DataTransferObject.Domain.Apontamento.FecharAlterarPeriodo;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Apontamento.Domain.Impl.Service
{
    [LogDomainClass]
    public class PeriodoFechadoService : IPeriodoFechadoService
    {
        private readonly IPeriodoFechadoRepository _periodoFechadoRepository;
        private readonly IStringLocalizer<ApontamentoMessage> _stringLocalizer;

        public PeriodoFechadoService(IPeriodoFechadoRepository periodoFechadoRepository,
                                     IStringLocalizer<ApontamentoMessage> stringLocalizer)
        {
            _periodoFechadoRepository = periodoFechadoRepository;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<BuscaPeriodoFechadoResult> BuscaPeriodoFechado(int orgId)
        {
            try
            {
                return await _periodoFechadoRepository.BuscaPeriodoFechado(orgId);
            }
            catch
            {
                throw;
            }
        }

        public FecharAlterarPeriodoDTO FecharAlterarPeriodo(DateTime dataFim, int orgId, string cpf)
        {
            try
            {
                int idPeriodoFechado = _periodoFechadoRepository.ExistePeriodo(orgId);

                if (idPeriodoFechado > 0)
                {
                    return _periodoFechadoRepository.AlterarDataPeriodoFechado(dataFim, orgId, cpf, idPeriodoFechado);
                }
                else
                {
                    return _periodoFechadoRepository.CriarDataPeriodoFechado(dataFim, orgId, cpf);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task ValidaSeEstaNoPeriodoFechado(string dataValidacao, int orgId)
        {
            await ValidaSeEstaNoPeriodoFechado((List<string>)new() { dataValidacao }, orgId);
        }

        public async Task ValidaSeEstaNoPeriodoFechado(List<string> datasValidacao, int orgId)
        {
            var periodoFechado = await _periodoFechadoRepository.BuscaPeriodoFechado(orgId);

            if (periodoFechado.DataFim.HasValue)
            {
                //string messageClosedPeriod = "Período fechado para lançamentos.";
                var messageClosedPeriod = _stringLocalizer.GetStringOuVazio("MESSAGE_CLOSED_PERIOD");

                //string messageInvalidaDate = "Não foi possível validar a data, data inválida.";
                var messageInvalidaDate = _stringLocalizer.GetStringOuVazio("MESSAGE_INVALID_DATE");

                foreach (var dataValidacao in datasValidacao)
                {
                    if (!DateTime.TryParse(dataValidacao, out DateTime dataFormatada))
                    {
                        throw new ArgumentException(messageInvalidaDate);
                    }

                    if (periodoFechado.DataFim.Value >= dataFormatada)
                    {
                        throw new ArgumentException(messageClosedPeriod);
                    }
                }
            }
        }
    }
}