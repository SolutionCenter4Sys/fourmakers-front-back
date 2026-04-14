using ApiClient.Domain.Interfaces;
using Colaboracao.Core;
using Core.Domain;
using Core.Domain.RotinaContratosVencidos;
using DataTransferObject.Domain.Notificacao;
using DataTransferObject.Domain.RotinaNotificacaoContratosVencidos;
using DataTransferObject.Domain.Usuario;
using Firebase.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RotinasBackoffice.API
{
    public class RotinaNotificacaoContratosVencidos : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<RotinaNotificacaoContratosVencidos> _logger;
        private CrontabSchedule _schedule;
        private DateTime _nextRun;

        public RotinaNotificacaoContratosVencidos(
            IConfiguration configuration,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<RotinaNotificacaoContratosVencidos> logger)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _schedule = CrontabSchedule.Parse(_configuration["Schedule:CronNotificacaoContratosVencidos"], new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(120000, cancellationToken);
            do
            {
                var now = DateTime.UtcNow;
                if (now > _nextRun)
                {
                    await EnviaNotificacao();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
                }
                await Task.Delay(1000, cancellationToken);
            }
            while (!cancellationToken.IsCancellationRequested);
        }

        private async Task EnviaNotificacao()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var rotinaContratosVencidosRepository = scope.ServiceProvider.GetRequiredService<IRotinaContratosVencidosRepository>();
                    var envioEmail = scope.ServiceProvider.GetRequiredService<IEnvioEmail>();
                    var notificacaoService = scope.ServiceProvider.GetRequiredService<INotificacaoService>();

                    var emailsContratosNotificacao = await rotinaContratosVencidosRepository.BuscarEmailContratosNotificacao();

                    foreach (var emailContratos in emailsContratosNotificacao.ContratosAVencer)
                    {
                        try
                        {
                            var dadosColaboradar = await rotinaContratosVencidosRepository.BuscarDadosColaboradorPorEmail(emailContratos.Email);
                            var corpo = MontarCorpoEmailComTabela(
                                $"Você tem {emailContratos.QuantidadeContratos} contrato(s) próximo(s) do vencimento.",
                                emailContratos.Contratos);

                            envioEmail.EnviaEmailTemplateFoursys(dadosColaboradar?.NomeColaborador ?? string.Empty, corpo, "Contratos próximos do vencimento", emailContratos.Email);

                            if (dadosColaboradar != null)
                            {
                                await notificacaoService.EnviarNotificacaoColaborador(dadosColaboradar.Cpf,
                                                                                       dadosColaboradar.OrgId,
                                                                                       "Contratos próximos do vencimento",
                                                                                       $"Você tem {emailContratos.QuantidadeContratos} contratos próximos do vencimento. Clique aqui para mais informações.",
                                                                                       "",
                                                                                       FuncionalidadeSistemaEnum.ALIANCAS_PARCERIAS
                                );
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Falha ao enviar notificação de contratos próximos do vencimento para {Email}.", emailContratos.Email);
                        }
                    }

                    foreach (var emailContratos in emailsContratosNotificacao.ContratosVencidos)
                    {
                        try
                        {
                            var dadosColaboradar = await rotinaContratosVencidosRepository.BuscarDadosColaboradorPorEmail(emailContratos.Email);
                            var corpo = MontarCorpoEmailComTabela(
                                $"Você tem {emailContratos.QuantidadeContratos} contrato(s) vencido(s).",
                                emailContratos.Contratos);

                            envioEmail.EnviaEmailTemplateFoursys(dadosColaboradar?.NomeColaborador ?? string.Empty, corpo, "Contratos vencidos", emailContratos.Email);

                            if (dadosColaboradar != null)
                            {
                                await notificacaoService.EnviarNotificacaoColaborador(dadosColaboradar.Cpf,
                                                                                       dadosColaboradar.OrgId,
                                                                                       "Contratos Vencidos",
                                                                                       $"Você tem {emailContratos.QuantidadeContratos} contratos vencidos. Clique aqui para mais informações.",
                                                                                       "",
                                                                                       FuncionalidadeSistemaEnum.ALIANCAS_PARCERIAS
                                );
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Falha ao enviar notificação de contratos vencidos para {Email}.", emailContratos.Email);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao executar rotina RotinaNotificacaoContratosVencidos. ERRO: {ex.Message}");
            }
        }

        private static string MontarCorpoEmailComTabela(string resumo, IReadOnlyList<ContratoNotificacaoDetalhe> contratos)
        {
            var sb = new StringBuilder();
            sb.Append(WebUtility.HtmlEncode(resumo));
            sb.Append(" &#9888;&#65039;");
            sb.Append("<br><br>");
            sb.Append(MontarTabelaHtmlContratos(contratos));
            sb.Append("<br><br>Acesse o módulo Gestão de Contratos do Fourmakers para mais informações.");
            return sb.ToString();
        }

        private static string MontarTabelaHtmlContratos(IReadOnlyList<ContratoNotificacaoDetalhe> contratos)
        {
            if (contratos == null || contratos.Count == 0)
            {
                return "<p>Nenhum contrato listado.</p>";
            }

            const string thStyle = "text-align:left;padding:4px 8px;height:auto;line-height:1.25;font-size:12px;font-weight:bold;border:1px solid #ccc;background-color:#f5f5f5;";
            const string tdStyle = "text-align:left;padding:4px 8px;line-height:1.25;font-size:12px;border:1px solid #ccc;color:#222239;word-wrap:break-word;";

            var sb = new StringBuilder();
            sb.Append("<table cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse:collapse;font-size:12px;width:100%;max-width:560px;\">");
            sb.Append("<thead><tr>");
            sb.Append("<th style=\"").Append(thStyle).Append("\" scope=\"col\">Empresa</th>");
            sb.Append("<th style=\"").Append(thStyle).Append("\" scope=\"col\">Contrato</th>");
            sb.Append("<th style=\"").Append(thStyle).Append("white-space:nowrap;\" scope=\"col\">Data fim</th>");
            sb.Append("</tr></thead><tbody>");
            foreach (var c in contratos)
            {
                var dataFim = c.FimContrato.HasValue
                    ? c.FimContrato.Value.ToString("dd/MM/yyyy")
                    : string.Empty;
                sb.Append("<tr><td style=\"").Append(tdStyle).Append("\">")
                    .Append(WebUtility.HtmlEncode(c.NomeEmpresa ?? string.Empty))
                    .Append("</td><td style=\"").Append(tdStyle).Append("\">")
                    .Append(WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(c.NomeContrato) ? string.Empty : c.NomeContrato))
                    .Append("</td><td style=\"").Append(tdStyle).Append("\">")
                    .Append(WebUtility.HtmlEncode(dataFim))
                    .Append("</td></tr>");
            }
            sb.Append("</tbody></table>");
            return sb.ToString();
        }
    }
}
