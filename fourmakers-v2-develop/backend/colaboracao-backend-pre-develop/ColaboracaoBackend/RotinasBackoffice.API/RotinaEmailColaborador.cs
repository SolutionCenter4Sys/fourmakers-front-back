using Colaboracao.Core;
using Colaboracao.Helper;
using Core.Domain.TemplateEmail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RotinasBackoffice.API
{
    public class RotinaEmailColaborador : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private CrontabSchedule _schedule;
        private DateTime _nextRun;
        private readonly int _numeroMaximoTentativas;

        public RotinaEmailColaborador(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            _schedule = CrontabSchedule.Parse(_configuration["Schedule:CronEmail"], new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            _numeroMaximoTentativas = int.Parse(_configuration["NumeroMaximoTentativasEnvio"]);
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
                    await EnviaEmail();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
                }
                await Task.Delay(1000, cancellationToken);
            }
            while (!cancellationToken.IsCancellationRequested);
        }

        private async Task<string> EnviaEmail()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var templateRepository = scope.ServiceProvider.GetRequiredService<ITemplateRepository>();
                    var envioEmail = scope.ServiceProvider.GetRequiredService<IEnvioEmail>();
                    
                    var emailsPendentes = templateRepository.GetEmailPendente(_numeroMaximoTentativas);
                    foreach (var emailPendente in emailsPendentes)
                    {
                        emailPendente.NumeroTentativas++;
                        emailPendente.DataAlteracao = DateTime.Now;
                        try
                        {
                            var assunto = emailPendente.Assunto.ToStringOuNull() ?? "Fourmakers";
                            envioEmail.EnviaEmailSemTemplate("", emailPendente.CorpoEmail, assunto, emailPendente.Destinatarios);
                            emailPendente.DataDisparo = DateTime.Now;
                        }
                        catch (Exception e)
                        {
                            emailPendente.MensagemErro = "Mensagem: " + e.Message + "\n\tStack Trace: " + e.StackTrace.ToString();
                        }
                        templateRepository.UpdateEmailPendente(emailPendente);
                    }
                }
                return "Operação Concluida.";
            }
            catch (Exception e)
            {
                return "A operação falhou." + e.Message;
            }
        }
    }
}