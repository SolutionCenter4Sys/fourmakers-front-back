using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Colaboracao.Helper.Enum;
using Core.Domain.FourmakersLead;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Fourmakers;
using Foursys.Domain.Interfaces.Services;
using Logs.Infra.Attributes;

namespace Foursys.Domain.Impl.Services;

[LogDomainClass]
public class FourmakersLeadsService(IFourmakersLeadsRepository fourmakersLeadsRepository): IFourmakersLeadsService
{
    public async Task<ApiGenericResult> InserirLeadAsync(string nome, string email, string telefone, string nomeEmpresa, CaputraLeadColaboradorQuantidadeEnum opcaoColaboradorEnum)
    {
        var result = new ApiGenericResult();
        try
        {
            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(telefone) ||
                string.IsNullOrEmpty(nomeEmpresa))
            {
                throw new ValidationException("Todos os campos são obrigatorios");
            }

            await fourmakersLeadsRepository.InserirLeadAsync(nome, email, telefone, nomeEmpresa, opcaoColaboradorEnum);
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Create, "Coleta de infomações");
        }
        
        return result;
    }
}