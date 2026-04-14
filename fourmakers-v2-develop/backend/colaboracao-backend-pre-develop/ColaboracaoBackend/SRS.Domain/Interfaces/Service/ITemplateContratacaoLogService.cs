using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Contratacao;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SRS.Domain.Interfaces.Service
{
    public interface ITemplateContratacaoLogService
    {
        Task<ApiGenericResult<string>> RegistrarLogAsync(
            Guid templateId, 
            string colaboradorCodigoInterno, 
            AcaoLogTemplateEnum acao, 
            TemplateDTO objetoTemplate = null, 
            string alteracoes = null);

        Task<ApiGenericResult<List<TemplateContratacaoLogDTO>>> ListarLogsTemplateAsync(Guid templateId, int limite = 50, int cursor = 0);
        
        Task<ApiGenericResult<List<TemplateContratacaoLogDTO>>> ListarLogsPorColaboradorAsync(string colaboradorCodigoInterno, int limite = 50, int cursor = 0);
        
        Task<ApiGenericResult<List<TemplateContratacaoLogDTO>>> ListarLogsPorPeriodoAsync(DateTime dataInicio, DateTime dataFim, int limite = 100, int cursor = 0);
    }
}
