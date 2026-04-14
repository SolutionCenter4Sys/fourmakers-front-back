using Core.Domain.TemplateEmail;
using DataTransferObject.Domain.TemplateEmail;
using TemplateOrg.Constantes;
using TemplateOrg.Interfaces;

namespace TemplateOrg.Impl
{
    public class TemplateOrgService : ITemplateOrgService
    {
        private readonly ITemplateRepository _templateRepository;

        public TemplateOrgService(ITemplateRepository TemplateRepository)
        {
            _templateRepository = TemplateRepository;
        }

        public TemplateEmailDTO BuscaTemplateEmail(TemplateOrgParametroEnum templateEnum)
        {
            return _templateRepository.BuscaTemplateEmail(templateEnum);
        }

        public void RegistraTemplateEmail(int orgId, string email, string assunto, string templateParametrizado)
        {
            _templateRepository.RegistraTemplateEmail(orgId, email, assunto, templateParametrizado);
        }
    }
}