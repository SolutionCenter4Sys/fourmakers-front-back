using Colaboracao.Core;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.Vaga;
using Core.DomainModel;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.Vaga.Enums;
using SRS.Infra.Constantes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SRS.Domain.Impl.Service.Strategy
{
    /// <summary>
    /// Estratégia para o status AnaliseGestor.
    /// Regras:
    /// 1. Verifica se há quantidade mínima de candidatos para enviar vaga para EntrevistaComCliente
    /// 2. Envia email para gestores externos em background
    /// </summary>
    public class AnaliseGestorEnvioEmailStatusStrategy : MudancaStatusCandidaturaStrategyBase
    {
        private readonly ILogCore _log;
        private readonly Func<CandidaturaRecrutamentoDTO, string, Task> _enviarEmailParaGestoresExternos;

        public AnaliseGestorEnvioEmailStatusStrategy(
            ILogCore log,
            Func<CandidaturaRecrutamentoDTO, string, Task> enviarEmailParaGestoresExternos)
        {
            _log = log;
            _enviarEmailParaGestoresExternos = enviarEmailParaGestoresExternos;
        }

        public override bool DeveExecutar(int statusAnteriorId, int novoStatusId, VagaRecrutamentoDTO vaga)
        {
            return novoStatusId == StatusCandidaturaRecrutamento.AnaliseDoCvPeloGestor.ToInt();
        }

        public override async Task<string> ExecutarAsync(MudancaStatusCandidaturaContext context)
        {
            try
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _enviarEmailParaGestoresExternos(context.Candidatura, context.CodColaborador);
                    }
                    catch (Exception ex)
                    {
                        _log.Log($"Erro ao enviar email para gestores externos em background - {ex.Message}", LevelsEnum.Error);
                    }
                });
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao iniciar envio de email para gestores externos - {ex.Message}", LevelsEnum.Error);
            }

            return String.Empty;
        }
    }
}

