using DataTransferObject.Domain.Candidato;
using DataTransferObject.Domain.Contratacao;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Colaboracao.Core
{
    public class EnvioEmail : IEnvioEmail
    {
        private readonly Comunicacao.Infra.EnvioEmail _envioEmail;

        public EnvioEmail(IConfiguration configuration)
        {
            this._envioEmail = new Comunicacao.Infra.EnvioEmail(configuration);
        }

        public void EnviaEmailSemTemplate(string nomeDestinatario, string mensagem, string assunto, string destinatario)
        {
            this._envioEmail.EnviaEmailSemTemplate(nomeDestinatario, mensagem, assunto, destinatario);
        }

        public void EnviaEmailTemplateFoursys(string nomeDestinatario, string mensagem, string assunto, string destinatario)
        {
            this._envioEmail.EnviaEmailTemplateFoursys(nomeDestinatario, mensagem, assunto, destinatario);
        }
        public void EnviaEmailTemplateCandidatos(IEnumerable<TemplateDestinatarioEmail> destinatarios, TemplateDTO template, byte[] documento, bool anexo, string nomePDF, int orgId)
        {
            this._envioEmail.EnviaEmailTemplateCandidatos(destinatarios, template, documento, anexo, nomePDF, orgId);
        }
        public void EnviaEmailCandidatoReprovado(string nomeCandidato, string emailCandidato, string emailResponsavelCC, string assunto, string mensagem)
        {
            this._envioEmail.EnviaEmailCandidatoReprovado(nomeCandidato, emailCandidato, emailResponsavelCC, assunto, mensagem);
        }
    }
}