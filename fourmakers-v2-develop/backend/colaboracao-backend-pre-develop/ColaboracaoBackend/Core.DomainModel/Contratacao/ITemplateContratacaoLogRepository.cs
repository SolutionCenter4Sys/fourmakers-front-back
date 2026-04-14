using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Contratacao;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Contratacao
{
    public interface ITemplateContratacaoLogRepository
    {
        Task<string> InserirLogTemplateAsync(
            Guid templateId, 
            string colaboradorCodigoInterno, 
            AcaoLogTemplateEnum acao, 
            string objetoTemplate = null, 
            string alteracoes = null);

        Task<IEnumerable<TemplateContratacaoLogDTO>> ListarLogsTemplateAsync(Guid templateId, int limite = 50, int cursor = 0);
        
        Task<IEnumerable<TemplateContratacaoLogDTO>> ListarLogsPorColaboradorAsync(string colaboradorCodigoInterno, int limite = 50, int cursor = 0);
        
        Task<IEnumerable<TemplateContratacaoLogDTO>> ListarLogsPorPeriodoAsync(DateTime dataInicio, DateTime dataFim, int limite = 100, int cursor = 0);
    }
}
