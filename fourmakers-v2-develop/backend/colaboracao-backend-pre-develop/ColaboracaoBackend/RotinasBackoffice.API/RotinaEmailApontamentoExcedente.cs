using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Colaboracao.Core;
using Core.Domain.Apontamento;
using Core.Domain.Colaborador;
using Core.Domain.TemplateEmail;
using Core.Domain.Usuario;
using Core.DomainModel.Org;
using DataTransferObject.Domain.Fourmakers;
using Foursys.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NCrontab;
using TemplateOrg.Constantes;

namespace RotinasBackoffice.API;

public class RotinaEmailApontamentoExcedente : BackgroundService
{
    private readonly CrontabSchedule _schedule;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private DateTime _nextRun;

    public RotinaEmailApontamentoExcedente(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _schedule = CrontabSchedule.Parse(configuration["Schedule:CronEmailApontamentoExcedente"], new CrontabSchedule.ParseOptions { IncludingSeconds = true });
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

    private async Task EnviaEmail()
    {
        int mes = DateTime.Now.Month;
        int ano = DateTime.Now.Year;

        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var orgRepository = scope.ServiceProvider.GetRequiredService<IOrgRepository>();
            var buscaParametroConfiguracaoService = scope.ServiceProvider.GetRequiredService<IBuscaParametroConfiguracaoService>();
            var apontamentoRepository = scope.ServiceProvider.GetRequiredService<IApontamentoRepository>();
            var templateRepository = scope.ServiceProvider.GetRequiredService<ITemplateRepository>();
            var buscaColaboradorRepository = scope.ServiceProvider.GetRequiredService<IBuscaColaboradorRepository>();
            var envioEmail = scope.ServiceProvider.GetRequiredService<IEnvioEmail>();
            
            var orgs = orgRepository.GetAllOrgsId(false);

            foreach (var org in orgs)
            {
                // Busca o parâmetro de horas excedentes (em horas)
                var parametroHoras = buscaParametroConfiguracaoService
                    .GetParametroConfiguracao<string>(
                        ParametroOrgCodigoEnum.CONFIGURACAO_APONTAMENTO_EXCEDENTE_VALOR,
                        (int)org.Id,
                        ""
                    );

                if (!parametroHoras.IsNullOrEmpty())
                {
                    if (int.TryParse(parametroHoras, out int horasExcedentes))
                    {
                        // Converte para minutos
                        int minutosExcedentes = horasExcedentes * 60;

                        // Lista apontamentos acima do parâmetro (apontamentos já estão em minutos)
                        var apontamentos = await apontamentoRepository
                            .ListarApontamentosExcedentesPorOrgId((int)org.Id, mes, ano, minutosExcedentes);

                        // Busca destinatários do alerta
                        var parametroDestinatarios = buscaParametroConfiguracaoService
                            .GetParametroConfiguracao<string>(
                                ParametroOrgCodigoEnum.APONTAMENTO_EXCEDENTE_DESTINATARIOS,
                                (int)org.Id,
                                ""
                            );

                        if (!parametroDestinatarios.IsNullOrEmpty())
                        {
                            var destinatarioList = parametroDestinatarios.Split(";")
                                .Where(d => !string.IsNullOrWhiteSpace(d))
                                .Distinct()
                                .ToList();

                            if (apontamentos.Any() && destinatarioList.Any())
                            {
                                // Busca template de email
                                var template = templateRepository
                                    .BuscaTemplateEmail(TemplateOrgParametroEnum.APONTAMENTO_EXCEDENTE);

                                var templateEmail = template.Template;

                                // Monta as linhas da tabela usando StringBuilder
                                var sb = new StringBuilder();
                                foreach (var a in apontamentos)
                                {
                                    sb.Append($@"
                                    <tr>
                                        <td style='padding: 12px 24px;'>{a.NomeColaborador}</td>
                                        <td style='padding: 12px 24px; text-align: center;'>{Math.Floor(a.HorasPendentes / 60.0):0}:{a.HorasPendentes % 60:00}</td>
                                        <td style='padding: 12px 24px; text-align: center;'>{Math.Floor(a.HorasAprovadas / 60.0):0}:{a.HorasAprovadas % 60:00}</td>
                                        <td style='padding: 12px 24px; text-align: center;'>{Math.Floor(a.HorasTotais / 60.0):0}:{a.HorasTotais % 60:00}</td>
                                    </tr>");
                                }
                                var tabela = sb.ToString();

                                // Substituição de placeholders comuns
                                var templateEmailComum = templateEmail
                                    .Replace("${HORAS_EXCEDENTES}", horasExcedentes.ToString())
                                    .Replace("${MES}", mes.ToString())
                                    .Replace("${ANO}", ano.ToString())
                                    .Replace("${ITENS_DA_TABELA}", tabela)
                                    .Replace("${ORG}", org.Descricao);

                                // Envia email individual para cada destinatário
                                foreach (var destinatario in destinatarioList)
                                {
                                    // Busca colaborador pelo email
                                    var nomeCompleto = await buscaColaboradorRepository
                                        .BuscaNomeColaboradorPorEmail(destinatario);

                                    // Substitui placeholder do nome do destinatário
                                    var templateEmailFinal = templateEmailComum
                                        .Replace("${NOME}", nomeCompleto ?? "responsável");

                                    // Envia email
                                    envioEmail.EnviaEmailSemTemplate(
                                        "",
                                        templateEmailFinal,
                                        "Apontamentos Excedentes",
                                        destinatario
                                    );
                                }
                            }
                        }
                    }
                    else
                    {
                        // Log caso a conversão falhe
                        Console.WriteLine($"Org {org.Descricao} ({org.Id}): parâmetro '{parametroHoras}' não é um número válido.");
                    }
                }
            }
        }
    }
}