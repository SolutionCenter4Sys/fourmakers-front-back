using ApiClient.Domain;
using Colaboracao.Helper;
using DataTransferObject.Domain.Candidato;
using DataTransferObject.Domain.Contratacao;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Comunicacao.Infra
{
    public class EnvioEmail
    {
        private readonly string _usuario;
        private readonly string _senha;
        private readonly string _servidor;
        private readonly IConfiguration _configuration;

        public EnvioEmail(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._usuario = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.MAIL_USER);
            this._senha = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.MAIL_PASSWORD);
            this._servidor = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.MAIL_SERVER);
        }

        public void EnviaEmailSemTemplate(string nomeDestinatario, string mensagem, string assunto, string destinatario)
        {
            var email = new EnvioEmailNugetFoursys(_usuario, _senha, _servidor, false);

            email.EnviaEmail(nomeDestinatario, mensagem, assunto, destinatario);
        }

        public void EnviaEmailTemplateFoursys(string nomeDestinatario, string mensagem, string assunto, string destinatario)
        {
            var email = new EnvioEmailNugetFoursys(_usuario, _senha, _servidor, true);

            email.EnviaEmail(nomeDestinatario, mensagem, assunto, destinatario);
        }
        public void EnviaEmailTemplateCandidatos(IEnumerable<TemplateDestinatarioEmail> destinatarios, TemplateDTO template, byte[] documento, bool anexo, string nomePDF, int orgId)
        {
            var email = new EnvioEmailNugetFoursys(_usuario, _senha, _servidor, false);
            email.EnviaEmailTemplateCandidatos(destinatarios, template, documento, anexo, nomePDF, orgId);
        }
        public void EnviaEmailCandidatoReprovado(string nomeCandidato, string emailCandidato, string emailResponsavelCC, string assunto, string mensagem)
        {
            var email = new EnvioEmailNugetFoursys(_usuario, _senha, _servidor, true);

            email.EnviaEmailCandidatoReprovado(nomeCandidato, emailCandidato, emailResponsavelCC, assunto, mensagem);
        }
    }
}