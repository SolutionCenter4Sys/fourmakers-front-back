using DataTransferObject.Domain.TemplateEmail;
using TemplateOrg.Constantes;

namespace TemplateOrg.Interfaces
{
    public interface ITemplateOrgService
    {
        TemplateEmailDTO BuscaTemplateEmail(TemplateOrgParametroEnum templateEnum);
        void RegistraTemplateEmail(int orgId, string email, string assunto, string templateParametrizado);
    }
}