using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Contratacao;
using System;
using System.Threading.Tasks;

namespace Core.Domain.Contratacao
{
    public interface ITemplateContratacaoValidatorService
    {
        Task ValidarTemplate(Guid templateId, TemplateDTO template, CRUDEnum cRUDEnum);
        Task ValidarCamposDeEntrada(TemplateDTO template);
        Task ValidarCriacaoTemplate(TemplateDTO template);
        Task ValidarAtualizacaoTemplate(TemplateDTO template);
    }
}
