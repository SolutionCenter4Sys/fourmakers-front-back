using Colaboracao.Helper;
using DataTransferObject.Domain.Candidato;
using DataTransferObject.Domain.Contratacao;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Comunicacao.Infra
{
    public class EnvioEmailNugetFoursys
    {
        private readonly string _emailNoReply;
        private readonly string _senhaNoReply;
        private readonly string _clientHost;
        private readonly bool _templateDefaultFoursysEmail;

        public EnvioEmailNugetFoursys(string emailNoReply, string senhaNoReply, string clientHost, bool templateDefaultFoursysEmail)
        {
            _emailNoReply = emailNoReply;
            _senhaNoReply = senhaNoReply;
            _clientHost = clientHost;
            _templateDefaultFoursysEmail = templateDefaultFoursysEmail;
        }

        public void EnviaEmail(string nomeDestinatario, string mensagem, string assunto, string destinatario)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(_emailNoReply, _senhaNoReply);
                client.Port = 587;
                client.Host = _clientHost;// "smtp.office365.com";
                client.Timeout = 60000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;

                MailMessage mm = new MailMessage();
                mm.From = new MailAddress(_emailNoReply);
                mm.Body = _templateDefaultFoursysEmail ? montaTemplateEmail(nomeDestinatario, mensagem) : mensagem;
                mm.Subject = assunto;
                foreach (var address in destinatario.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mm.To.Add(address);
                }

                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.IsBodyHtml = true;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                client.Send(mm);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Converte quebras de linha do texto (ex.: do campo descricao) em parágrafos HTML (&lt;p&gt;),
        /// para exibição correta em e-mail HTML.
        /// </summary>
        private static string QuebrasDeLinhaParaHtml(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return texto ?? string.Empty;
            var linhas = texto.Replace("\r\n", "\n").Replace("\r", "\n").Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();
            foreach (var linha in linhas)
                sb.Append("<p>").Append(linha.Trim()).Append("</p>");
            return sb.ToString();
        }

        private static string montaTemplateEmail(string nome, string mensagem)
        {
            StringBuilder retStr = new StringBuilder();

            retStr.Append("<html>");
            retStr.Append("<head>");
            retStr.Append("<meta http-equiv=\"Content - Type\" content=\"text / html; charset = UTF - 8\" />");
            retStr.Append("<meta name=\"viewport\" content=\"width = device - width, initial - scale = 1.0\"/>");
            retStr.Append("<title>Foursys</title>");
            retStr.Append("<style>");
            retStr.Append("body, table, td, th, p, li, a, h1, h2, h3, strong, em {");
            retStr.Append("    font-family: Roboto, Verdana, Arial, sans-serif;");
            retStr.Append("    font-size: 12px;");
            retStr.Append("    color: #000;");
            retStr.Append("}");
            retStr.Append("table{ border-collapse:collapse; }");
            retStr.Append("thead{ text-align:left; }");
            retStr.Append("thead th{ border:1px solid #ddd; height:120px; padding:0 20px; }");
            retStr.Append("tbody td{ color:#222239; padding:20px; }");
            retStr.Append("h2{ color:#ff5214; font-size:14px; }"); // destaque só no título
            retStr.Append("a{ color:#ff5214; font-weight:bold; }");
            retStr.Append("tfoot{ background:#222239; }");
            retStr.Append("tfoot td{ color:#fff; padding:20px; }");
            retStr.Append("tfoot b{ color:#ff5214; }");
            retStr.Append("tfoot p{ font-size:12px; color:#ff5214; }");
            retStr.Append("</style>");
            retStr.Append("</head>");
            retStr.Append("<body>");
            retStr.Append("<table width=\"600\" align=\"center\" border=\"0\">");
            retStr.Append("<thead>");
            retStr.Append("<tr>");
            retStr.Append("<th>");
            retStr.Append("<img src=\"https://i.ibb.co/DW1wWGS/foursys.png\" width=\"200\" height=\"70\" />");
            retStr.Append("</th>");
            retStr.Append("</tr>");
            retStr.Append("</thead>");
            
            retStr.Append("<tbody>");
            retStr.Append("<tr>");
            retStr.Append("<td>");
            retStr.Append("<h2>");
            retStr.Append("Olá " + nome + ",");
            retStr.Append("</h2>");
            retStr.Append(QuebrasDeLinhaParaHtml(mensagem));
            retStr.Append("</td>");
            retStr.Append("</tr>");
            retStr.Append("</tbody>");
            retStr.Append("<!--Footer-->");
            retStr.Append("<tfoot>");
            retStr.Append("<tr>");
            retStr.Append("<td>");
            retStr.Append("<p>");
            retStr.Append("Made by <b>Foursys</b>");
            retStr.Append("</p>");
            retStr.Append("</td>");
            retStr.Append("</tr>");
            retStr.Append("</tfoot>");
            retStr.Append("</table>");
            retStr.Append("</body>");
            retStr.Append("</html>");

            return retStr.ToString();
        }
        public void EnviaEmailTemplateCandidatos(IEnumerable<TemplateDestinatarioEmail> destinatarios, TemplateDTO dadosCandidato, byte[] documento, bool anexo, string nomePDF, int orgId)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(_emailNoReply, _senhaNoReply);
                client.Port = 587;
                client.Host = _clientHost; // "smtp.office365.com";
                client.Timeout = 60000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;

                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(_emailNoReply);
                msg.Subject = "Novo contratado: " + dadosCandidato.NomeCompleto
                    + " - Cargo: " + dadosCandidato.Cargo;
                
                // Org 9 (ROYAL): usar template específico
                msg.Body = orgId == 9 
                    ? TemplateCandidatosEmailRoyal(dadosCandidato, anexo)
                    : TemplateCandidatosEmail(dadosCandidato, anexo);

                foreach (var address in destinatarios)
                {
                    msg.Bcc.Add(address.Email);
                }

                msg.BodyEncoding = UTF8Encoding.UTF8;
                msg.IsBodyHtml = true;
                msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                if (anexo && documento != null && documento.Length > 0)
                {
                    msg.Attachments.Add(new Attachment(
                        new MemoryStream(documento), nomePDF));
                }
                client.Send(msg);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private static string TemplateCandidatosEmail(TemplateDTO dadosColaborador, bool anexo)
        {
            StringBuilder retStr = new StringBuilder();

            retStr.Append("<html>");
            retStr.Append("<head>");
            retStr.Append("<meta http-equiv=\"Content - Type\" content=\"text / html; charset = UTF - 8\" />");
            retStr.Append("<meta name=\"viewport\" content=\"width = device - width, initial - scale = 1.0\"/>");
            retStr.Append("<title>Foursys</title>");
            retStr.Append("<style>");
            retStr.Append("body, table, td, th, p, li, a, h1, h2, h3, strong, em {");
            retStr.Append("    font-family: Roboto, Verdana, Arial, sans-serif;");
            retStr.Append("    font-size: 12px;");
            retStr.Append("    color: #000;");
            retStr.Append("}");
            retStr.Append("table{ border-collapse:collapse; }");
            retStr.Append("thead{ text-align:left; }");
            retStr.Append("thead th{ border:1px solid #ddd; height:120px; padding:0 20px; }");
            retStr.Append("tbody td{ color:#222239; padding:20px; }");
            retStr.Append("h2{ color:#ff5214; font-size:14px; }"); // destaque só no título
            retStr.Append("a{ color:#ff5214; font-weight:bold; }");
            retStr.Append("tfoot{ background:#222239; }");
            retStr.Append("tfoot td{ color:#fff; padding:20px; }");
            retStr.Append("tfoot b{ color:#ff5214; }");
            retStr.Append("tfoot p{ font-size:12px; color:#ff5214; }");
            retStr.Append("</style>");
            retStr.Append("</head>");
            retStr.Append("<body>");
            retStr.Append("<table width=\"600\" align=\"center\" border=\"0\">");
            retStr.Append("<thead>");
            retStr.Append("<tr>");
            retStr.Append("<th>");
            retStr.Append("<img src=\"https://i.ibb.co/DW1wWGS/foursys.png\" width=\"200\" height=\"70\" />");
            retStr.Append("</th>");
            retStr.Append("</tr>");
            retStr.Append("</thead>");
            retStr.Append("<tbody>");
            retStr.Append("<tr>");
            retStr.Append("<td>");

            retStr.Append("<h2>");
            retStr.Append("<p>Olá !</p>");
            retStr.Append("<p>Foi aprovado um candidato para uma vaga nova na <strong>Foursys</strong>.</p>");
            retStr.Append("<p style=color:#c62828;>Fique atento às suas pendências:</p>");
            retStr.Append("</h2>");

            retStr.Append($"<p><strong>Nome do colaborador:</strong> {dadosColaborador.NomeCompleto} </p>");
            retStr.Append($"<p><strong>Cargo:</strong> {dadosColaborador.Cargo}</p>");
            //retStr.Append($"<p><strong>Stack Principal:</strong> </p>");
            retStr.Append($"<p><strong>Endereço:</strong> {FormatarEnderecoCompleto(dadosColaborador.Endereco)} </p>");
            //retStr.Append("<p><strong>Bairro:</strong> </p>");
            //retStr.Append("<p><strong>KM (Ida e Volta):</strong> </p>");
            retStr.Append($"<p><strong>Contato:</strong> {(string.IsNullOrWhiteSpace(dadosColaborador.ContatoPrincipal) ? "Não informado" : $" {dadosColaborador.ContatoPrincipal}")} </p>");
            retStr.Append($"<p><strong>E-mail:</strong> {dadosColaborador.EmailPessoal} </p>");
            retStr.Append("<br/>");

            //retStr.Append($"<p><strong>Nome do Cliente:</strong> {dadosColaborador.Cliente}</ p>");
            //retStr.Append($"<p><strong>Unidade:</strong> {dadosColaborador.UnidadeTrabalho}</p>");
            //retStr.Append($"<p><strong>Gestor Responsável:</strong> {dadosColaborador.GestorResponsavel}</p>");
            //retStr.Append($"<p><strong>Solicitante:</strong> {dadosColaborador.VagaAdmissao.Solicitante}</p>");
            retStr.Append($"<p><strong>E-mail Foursys:</strong> {(string.IsNullOrWhiteSpace(dadosColaborador.EmailCorporativo) ? "Não informado" : $" {dadosColaborador.EmailCorporativo}")}</p>");
            //retStr.Append($"<p><strong>E-mail tipo:</strong> {(string.IsNullOrWhiteSpace(dadosColaborador.AcessosUsuario.TipoEmail) ? "Não informado" : $" {dadosColaborador.AcessosUsuario.TipoEmail}")}</p>");
            retStr.Append($"<p><strong>Login de Rede:</strong> {(string.IsNullOrWhiteSpace(dadosColaborador.LoginRede) ? "Não informado" : $" {dadosColaborador.LoginRede}")}</p>");
            retStr.Append($"<p><strong>Máquina:</strong> {(string.IsNullOrWhiteSpace(dadosColaborador.TipoMaquina) ? "Não informado" : $" {dadosColaborador.TipoMaquina}")}</p>");
            retStr.Append($"<p><strong>Descrição de Máquina:</strong> {(string.IsNullOrWhiteSpace(dadosColaborador.DescricaoMaquina) ? "Não informado" : $" {dadosColaborador.DescricaoMaquina}")}</p>");
            
            // Adicionar primeira opção de equipamento padrão
            if (!string.IsNullOrWhiteSpace(dadosColaborador.PrimeiraOpcaoEquipamentoPadraoCargoFuncao))
            {
                var primeiraOpcaoFormatada = dadosColaborador.PrimeiraOpcaoEquipamentoPadraoCargoFuncao.Replace("|", "<br/>");
                retStr.Append($"<p><strong>Primeira Opção de Equipamento Padrão:</strong><br/>{primeiraOpcaoFormatada}</p>");
            }
            
            // Adicionar segunda opção de equipamento padrão
            if (!string.IsNullOrWhiteSpace(dadosColaborador.SegundaOpcaoEquipamentoPadraoCargoFuncao))
            {
                var segundaOpcaoFormatada = dadosColaborador.SegundaOpcaoEquipamentoPadraoCargoFuncao.Replace("|", "<br/>");
                retStr.Append($"<p><strong>Segunda Opção de Equipamento Padrão:</strong><br/>{segundaOpcaoFormatada}</p>");
            }
            
            retStr.Append($"<p><strong>Tamanho da Camiseta:</strong> {(string.IsNullOrWhiteSpace(dadosColaborador.TamanhoCamiseta) ? "Não informado" : $"{dadosColaborador.TamanhoCamiseta}")}</p>");
            retStr.Append($"<p><strong>Data de início:</strong> {(dadosColaborador?.DataInicio.HasValue == true
                    ? dadosColaborador.DataInicio.Value.ToString("dd/MM/yyyy")
                    : "Não informado")}</p>"
            );

            retStr.Append("</td>");
            retStr.Append("</tr>");
            retStr.Append("</tbody>");
            retStr.Append("<!--Footer-->");
            retStr.Append("<tfoot>");
            retStr.Append("<tr>");
            retStr.Append("<td>");
            retStr.Append("<p>");
            retStr.Append("Made by <b>Foursys</b>");
            retStr.Append("</p>");
            retStr.Append("</td>");
            retStr.Append("</tr>");
            retStr.Append("</tfoot>");
            retStr.Append("</table>");
            retStr.Append("</body>");
            retStr.Append("</html>");

            return retStr.ToString();
        }

        public void EnviaEmailCandidatoReprovado(string nomeCandidato, string emailCandidato, string emailResponsavelCC, string assunto, string mensagem)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(_emailNoReply, _senhaNoReply);
                client.Port = 587;
                client.Host = _clientHost;// "smtp.office365.com";
                client.Timeout = 60000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;

                MailMessage mm = new MailMessage();
                mm.From = new MailAddress(_emailNoReply);
                mm.Subject = assunto;
                mm.Body = _templateDefaultFoursysEmail ? montaTemplateEmail(nomeCandidato, mensagem) : QuebrasDeLinhaParaHtml(mensagem);
                mm.To.Add(emailCandidato);

                foreach (var address in emailResponsavelCC.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mm.Bcc.Add(address);
                }

                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.IsBodyHtml = true;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                client.Send(mm);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static string TemplateCandidatosEmailRoyal(TemplateDTO dadosColaborador, bool anexo)
        {
            StringBuilder retStr = new StringBuilder();

            retStr.Append("<html>");
            retStr.Append("<head>");
            retStr.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />");
            retStr.Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"/>");
            retStr.Append("<style>");
            retStr.Append("body { font-family: Arial, sans-serif; font-size: 14px; line-height: 1.6; color: #333; }");
            retStr.Append("p { margin: 10px 0; }");
            retStr.Append("a { color: #0066cc; }");
            retStr.Append("</style>");
            retStr.Append("</head>");
            retStr.Append("<body>");
            retStr.Append("<div style=\"max-width: 600px; margin: 0 auto; padding: 20px;\">");
            
            retStr.Append($"<p>Olá, {dadosColaborador.NomeCompleto},</p>");
            retStr.Append("<p>Parabéns pela sua aprovação! 🎉<br/>");
            retStr.Append("Estamos felizes em avançar para a próxima etapa do seu processo de contratação.</p>");
            retStr.Append("<p>Para darmos sequência, é necessário que você preencha seus dados no formulário disponível no link abaixo:</p>");
            retStr.Append("<p>👉 <a href=\"https://formulariocontratacao.app.questorpublico.com.br/novo/aHR0cHM6Ly9lc2NyaXRhLmFwcC5xdWVzdG9ycHVibGljby5jb20uYnIv/brpixIxZ2iX03Ciks8F9X1n6oHcpxTy12fXQCJuZouUvK5Ikyaklm9FO8sljptYL0vuWtcPRvBA4CDKHpA51R_2ftb7uREpplJ59m1EbZNQqPV6eu3QcqAqLooeY55JpsoAEWr799OKPrlNd2QqpSK0HAVxhAX_2bdgBqXlIxFdLRbAGVY4beOmZuSLAuWGZwmkc\">https://formulariocontratacao.app.questorpublico.com.br/novo/aHR0cHM6Ly9lc2NyaXRhLmFwcC5xdWVzdG9ycHVibGljby5jb20uYnIv/brpixIxZ2iX03Ciks8F9X1n6oHcpxTy12fXQCJuZouUvK5Ikyaklm9FO8sljptYL0vuWtcPRvBA4CDKHpA51R_2ftb7uREpplJ59m1EbZNQqPV6eu3QcqAqLooeY55JpsoAEWr799OKPrlNd2QqpSK0HAVxhAX_2bdgBqXlIxFdLRbAGVY4beOmZuSLAuWGZwmkc</a></p>");
            retStr.Append("<p><strong>Importante:</strong></p>");
            retStr.Append("<p>O formulário é utilizado pela nossa contabilidade para a formalização da sua contratação.</p>");
            retStr.Append("<p>Pedimos que todas as informações sejam preenchidas com atenção e exatamente como constam em seus documentos oficiais.</p>");
            retStr.Append("<p>Caso tenha qualquer dúvida durante o preenchimento, fique à vontade para responder este e-mail ou entrar em contato com o RH.<br/>");
            retStr.Append("Agradecemos desde já pela agilidade e ficamos à disposição.</p>");
            retStr.Append("<p>Atenciosamente,<br/>");
            retStr.Append("<strong>Leticia Goulart</strong><br/>");
            retStr.Append("<a href=\"mailto:Leticia@royaltecidos.com.br\">Leticia@royaltecidos.com.br</a></p>");
            
            retStr.Append("</div>");
            retStr.Append("</body>");
            retStr.Append("</html>");

            return retStr.ToString();
        }

        private static string FormatarEnderecoCompleto(DataTransferObject.Domain.Endereco.EnderecoDTO endereco)
        {
            if (endereco == null)
                return "Não informado";

            var partesEndereco = new List<string>();

            if (!string.IsNullOrWhiteSpace(endereco.Endereco))
                partesEndereco.Add(endereco.Endereco);

            if (endereco.Numero.HasValue && endereco.Numero.Value > 0)
                partesEndereco.Add($"Nº {endereco.Numero.Value}");

            if (!string.IsNullOrWhiteSpace(endereco.Complemento))
                partesEndereco.Add(endereco.Complemento);

            if (!string.IsNullOrWhiteSpace(endereco.Bairro))
                partesEndereco.Add(endereco.Bairro);

            if (!string.IsNullOrWhiteSpace(endereco.Cidade))
                partesEndereco.Add(endereco.Cidade);

            if (!string.IsNullOrWhiteSpace(endereco.Estado))
                partesEndereco.Add(endereco.Estado);

            if (!string.IsNullOrWhiteSpace(endereco.Cep))
                partesEndereco.Add($"CEP: {endereco.Cep}");

            return partesEndereco.Count > 0 ? string.Join(", ", partesEndereco) : "Não informado";
        }
    }
}