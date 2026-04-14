using DataTransferObject.Domain.Candidato;
using DataTransferObject.Domain.Contratacao;
using System.Collections.Generic;

namespace Colaboracao.Core
{
    public interface IEnvioEmail
    {
        void EnviaEmailSemTemplate(string nomeDestinatario, string mensagem, string assunto, string destinatario);
        void EnviaEmailTemplateFoursys(string nomeDestinatario, string mensagem, string assunto, string destinatario);
        void EnviaEmailCandidatoReprovado(string nomeCandidato, string emailCandidato, string emailResponsavelCC, string assunto, string mensagem);

        void EnviaEmailTemplateCandidatos(IEnumerable<TemplateDestinatarioEmail> destinatarios, TemplateDTO dadosCandidato, byte[] documento, bool anexo, string nomePDF, int orgId);
    }
}