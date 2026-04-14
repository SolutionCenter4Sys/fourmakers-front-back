using DataTransferObject.Domain.TemplateEmail;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateOrg.Constantes;

namespace Core.Domain.TemplateEmail
{
    public interface ITemplateRepository
    {
        TemplateEmailDTO BuscaTemplateEmail(TemplateOrgParametroEnum templateEnum, string idioma = null);
        void RegistraTemplateEmail(int orgId, string emails, string assunto, string templateParametrizado);
        Task RegistraTemplateEmailAsync(int orgId, string emails, string assunto, string templateParametrizado);
        List<EmailDTO> GetEmailPendente(int numeroMaximoTentativas);
        void UpdateEmailPendente(EmailDTO emailInfo);
    }
}