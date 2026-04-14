using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System.Threading.Tasks;

namespace Social.Domain.Interfaces.AtendimentoFourmakers
{
    public interface IWebhookWhatsAppService
    {
        Task<ApiGenericResult<WebhookWhatsAppResult>> ProcessarWebhookAsync(object payload, string webhookSecret, int orgId);
    }
}
