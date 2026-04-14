using Colaboracao.Core;
using Colaboracao.Helper.Enum;
using Core.Domain.Contratacao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Contratacao;
using DataTransferObject.Domain.Log;
using Newtonsoft.Json;
using SRS.Domain.Interfaces.Service;
using SRS.Infra.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace SRS.Domain.Impl.Service
{
    [LogDomainClass]
    public class TemplateContratacaoLogService : ITemplateContratacaoLogService
    {
        private readonly ITemplateContratacaoLogRepository _logRepository;
        private readonly ILogCore _log;

        public TemplateContratacaoLogService(ITemplateContratacaoLogRepository logRepository, ILogCore log)
        {
            _logRepository = logRepository;
            _log = log;
        }

        public async Task<ApiGenericResult<string>> RegistrarLogAsync(
            Guid templateId, 
            string colaboradorCodigoInterno, 
            AcaoLogTemplateEnum acao, 
            TemplateDTO objetoTemplate = null, 
            string alteracoes = null)
        {
            var result = new ApiGenericResult<string>();
            try
            {
                if (templateId == Guid.Empty)
                {
                    result.Sucesso = false;
                    result.Mensagem = "ID do template é obrigatório";
                    return result;
                }

                if (string.IsNullOrWhiteSpace(colaboradorCodigoInterno))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Código interno do colaborador é obrigatório";
                    return result;
                }

                string objetoTemplateJson = null;
                if (objetoTemplate != null)
                {
                    objetoTemplateJson = JsonConvert.SerializeObject(objetoTemplate, Formatting.Indented);
                }

                var logId = await _logRepository.InserirLogTemplateAsync(
                    templateId, 
                    colaboradorCodigoInterno, 
                    acao, 
                    objetoTemplateJson, 
                    alteracoes);

                result.Retorno = logId;
                result.Sucesso = true;
                result.Mensagem = "Log registrado com sucesso";
                
                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao registrar log do template: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);
                
                result.Sucesso = false;
                result.Mensagem = "Erro interno ao registrar log";
                return result;
            }
        }

        public async Task<ApiGenericResult<List<TemplateContratacaoLogDTO>>> ListarLogsTemplateAsync(Guid templateId, int limite = 50, int cursor = 0)
        {
            var result = new ApiGenericResult<List<TemplateContratacaoLogDTO>>();
            try
            {
                if (templateId == Guid.Empty)
                {
                    result.Sucesso = false;
                    result.Mensagem = "ID do template é obrigatório";
                    return result;
                }

                if (limite <= 0 || limite > 100)
                {
                    limite = 50;
                }

                if (cursor < 0)
                {
                    cursor = 0;
                }

                var logs = await _logRepository.ListarLogsTemplateAsync(templateId, limite, cursor);
                
                result.Retorno = logs.ToList();
                result.Sucesso = true;
                result.Mensagem = "Logs listados com sucesso";
                
                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao listar logs do template: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);
                
                result.Sucesso = false;
                result.Mensagem = "Erro interno ao listar logs";
                return result;
            }
        }

        public async Task<ApiGenericResult<List<TemplateContratacaoLogDTO>>> ListarLogsPorColaboradorAsync(string colaboradorCodigoInterno, int limite = 50, int cursor = 0)
        {
            var result = new ApiGenericResult<List<TemplateContratacaoLogDTO>>();
            try
            {
                if (string.IsNullOrWhiteSpace(colaboradorCodigoInterno))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Código interno do colaborador é obrigatório";
                    return result;
                }

                if (limite <= 0 || limite > 100)
                {
                    limite = 50;
                }

                if (cursor < 0)
                {
                    cursor = 0;
                }

                var logs = await _logRepository.ListarLogsPorColaboradorAsync(colaboradorCodigoInterno, limite, cursor);
                
                result.Retorno = logs.ToList();
                result.Sucesso = true;
                result.Mensagem = "Logs listados com sucesso";
                
                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao listar logs por colaborador: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);
                
                result.Sucesso = false;
                result.Mensagem = "Erro interno ao listar logs";
                return result;
            }
        }

        public async Task<ApiGenericResult<List<TemplateContratacaoLogDTO>>> ListarLogsPorPeriodoAsync(DateTime dataInicio, DateTime dataFim, int limite = 100, int cursor = 0)
        {
            var result = new ApiGenericResult<List<TemplateContratacaoLogDTO>>();
            try
            {
                if (dataInicio > dataFim)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Data de início deve ser menor que data de fim";
                    return result;
                }

                if (limite <= 0 || limite > 200)
                {
                    limite = 100;
                }

                if (cursor < 0)
                {
                    cursor = 0;
                }

                var logs = await _logRepository.ListarLogsPorPeriodoAsync(dataInicio, dataFim, limite, cursor);
                
                result.Retorno = logs.ToList();
                result.Sucesso = true;
                result.Mensagem = "Logs listados com sucesso";
                
                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao listar logs por período: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);
                
                result.Sucesso = false;
                result.Mensagem = "Erro interno ao listar logs";
                return result;
            }
        }
    }
}
