using ApiClient.Domain;
using Colaboracao.Helper;
using Core.Domain.Marketing.Comunicacao.Configuracao;
using Core.Domain.Marketing.Comunicacao.Publicacao;
using Core.Domain.TemplateEmail;
using DataTransferObject.Domain.Marketing.Comunicacao.Configuracao;
using DataTransferObject.Domain.Marketing.Comunicacao.Publicacao;
using DataTransferObject.Domain.Usuario;
using Firebase.Domain.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TemplateOrg.Constantes;

namespace RotinasBackoffice.API
{
    /// <summary>
    /// Rotina que: (1) ativa publicações agendadas e expira publicações ativas;
    /// (2) notificação in-app e (3) e-mail para publicações ativas não processadas.
    /// Oficial: escopo por tb_mkt_publicaco_grupo (vazio = org); com enviar_*_publicacao_oficial_sempre envia a todos no escopo, senão opt-in.
    /// Comunidade (não oficial + tb_mkt_comunidade_publicacao): grupos da comunidade ∪ participando, menos não participando, sempre opt-in.
    /// </summary>
    public class RotinasMarketingComunicacao : BackgroundService
    {
        private const string StatusAtiva = "ativa";
        private const string StatusExpirada = "expirada";

        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private CrontabSchedule _schedule;
        private DateTime _nextRun;

        public RotinasMarketingComunicacao(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            var cronExpression = _configuration["Schedule:CronMarketingPublicacaoAgendamentoEExpiracao"] ?? "0 * * * * *";
            _schedule = CrontabSchedule.Parse(cronExpression, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
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
                    await ProcessarAsync();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
                }
                await Task.Delay(1000, cancellationToken);
            }
            while (!cancellationToken.IsCancellationRequested);
        }

        private async Task ProcessarAsync()
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IComunicacaoPublicacaoRepository>();
                var agora = DateTime.UtcNow;

                // 1) Publicações agendadas -> ativar; ativas com validade vencida -> expirar
                var idsParaAtivar = await repository.ListarIdsAgendadasParaAtivarAsync(agora);
                if (idsParaAtivar.Count > 0)
                    await repository.AtualizarStatusPublicacaoEmLoteAsync(idsParaAtivar, StatusAtiva, dataPublicacao: agora);

                var idsParaExpirar = await repository.ListarIdsAtivasParaExpirarAsync(agora);
                if (idsParaExpirar.Count > 0)
                    await repository.AtualizarStatusPublicacaoEmLoteAsync(idsParaExpirar, StatusExpirada);

                // 2) Publicações ativas não processadas: notificação e e-mail
                var publicacoes = await repository.ListarPublicacoesAtivasNaoProcessadasEnvioAsync();
                if (publicacoes.Count == 0)
                    return;

                await ProcessarEnvioNotificacaoAsync(scope, publicacoes);
                await ProcessarEnvioEmailAsync(scope, publicacoes);

                var idsProcessados = publicacoes.Select(p => p.Id).ToList();
                await repository.AtualizarProcessadoEnvioNotificacaoEmailAsync(idsProcessados);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao executar rotina RotinasMarketingComunicacao. ERRO: {ex.Message}", ex);
            }
        }

        private async Task ProcessarEnvioNotificacaoAsync(IServiceScope scope, List<PublicacaoOficialEnvioDTO> publicacoes)
        {
            var notificacaoService = scope.ServiceProvider.GetRequiredService<INotificacaoService>();
            var configRepository = scope.ServiceProvider.GetRequiredService<IComunicacaoConfiguracaoRepository>();
            var publicacaoRepository = scope.ServiceProvider.GetRequiredService<IComunicacaoPublicacaoRepository>();
            var urlBase = (VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.URL_BASE) ?? "").TrimEnd('/');

            foreach (var pub in publicacoes)
            {
                var enviarSempreNotificacao = pub.PublicacaoOficial
                    && await configRepository.ObterEnviarNotificacaoPublicacaoOficialSempreAsync(pub.OrgId);

                var colaboradores = await ObterDestinatariosNotificacaoOficialAsync(configRepository, pub, enviarSempreNotificacao);

                var titulo = "Nova comunicação" + (pub.PublicacaoOficial ? " oficial" : "");
                var tituloPublicacao = pub.Subtitulo?.Length > 50 ? pub.Subtitulo.Substring(0, 47) + "…" : (pub.Subtitulo ?? "");
                var mensagem = string.IsNullOrEmpty(tituloPublicacao) ? "Toque para abrir." : $"{tituloPublicacao} Toque para abrir.";
                // Sempre persiste destino do post: com URL_BASE igual ao e-mail; sem URL_BASE grava path (RotaCompleta montada na leitura com URL_BASE do API).
                var pathPublicacao = $"comunicacao/post?id={pub.Id}";
                var linkPublicacao = string.IsNullOrEmpty(urlBase) ? pathPublicacao : $"{urlBase}/{pathPublicacao}";

                foreach (var colab in colaboradores)
                {
                    try
                    {
                        await notificacaoService.EnviarNotificacaoColaborador(
                            colab.CodigoInternoColaborador,
                            pub.OrgId,
                            titulo,
                            mensagem,
                            null,
                            FuncionalidadeSistemaEnum.GESTAO_COMUNICADOS,
                            linkPublicacao);
                        await publicacaoRepository.InserirLogEnvioPublicacaoOficialAsync(pub.Id, "notificacao", colab.CodigoInternoColaborador, pub.OrgId);
                    }
                    catch
                    {
                        // Log e continua para os demais
                    }
                }
            }

            await Task.CompletedTask;
        }

        private async Task ProcessarEnvioEmailAsync(IServiceScope scope, List<PublicacaoOficialEnvioDTO> publicacoes)
        {
            var configRepository = scope.ServiceProvider.GetRequiredService<IComunicacaoConfiguracaoRepository>();
            var templateRepository = scope.ServiceProvider.GetRequiredService<ITemplateRepository>();
            var publicacaoRepository = scope.ServiceProvider.GetRequiredService<IComunicacaoPublicacaoRepository>();
            var urlBase = (VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.URL_BASE) ?? "").TrimEnd('/');

            var templateOficialDto = templateRepository.BuscaTemplateEmail(TemplateOrgParametroEnum.NOTIFICACAO_COMUNICACAO_OFICIAL, "pt-BR");
            var templateComunidadeDto = templateRepository.BuscaTemplateEmail(TemplateOrgParametroEnum.NOTIFICACAO_COMUNICACAO_COMUNIDADE, "pt-BR");

            foreach (var pub in publicacoes)
            {
                var enviarSempreEmail = pub.PublicacaoOficial
                    && await configRepository.ObterEnviarEmailPublicacaoOficialSempreAsync(pub.OrgId);

                var colaboradores = await ObterDestinatariosEmailOficialAsync(configRepository, pub, enviarSempreEmail);
                var assunto = pub.PublicacaoOficial
                    ? "Nova comunicação oficial: " + pub.Subtitulo
                    : $"Nova comunicação da comunidade {pub.ComunidadeNome ?? ""}".TrimEnd();
                
                foreach (var colab in colaboradores)
                {
                    try
                    {
                        var linkBaseComunicados = string.IsNullOrEmpty(urlBase)
                            ? "#"
                            : $"{urlBase}/comunicacao/post?id={pub.Id}&ref={pub.OrgId}";

                        var template = pub.PublicacaoOficial ? templateOficialDto?.Template : templateComunidadeDto?.Template;
                        if (string.IsNullOrWhiteSpace(template))
                            continue;

                        var corpo = template
                            .Replace("${NOME}", colab.Nome ?? colab.CodigoInternoColaborador)
                            .Replace("${LINK_PUBLICACAO}", linkBaseComunicados)
                            .Replace("${TITULO}", pub.Titulo ?? "")
                            .Replace("${SUBTITULO}", pub.Subtitulo ?? "");

                        if (!pub.PublicacaoOficial)
                            corpo = corpo.Replace("${COMUNIDADE}", pub.ComunidadeNome ?? "");

                        await templateRepository.RegistraTemplateEmailAsync(pub.OrgId, colab.Email, assunto, corpo);
                        await publicacaoRepository.InserirLogEnvioPublicacaoOficialAsync(pub.Id, "email", colab.CodigoInternoColaborador, pub.OrgId);
                    }
                    catch
                    {
                        // Log e continua para os demais
                    }
                }
            }

            await Task.CompletedTask;
        }

        private static bool PublicacaoRestringeAosGrupos(PublicacaoOficialEnvioDTO pub) =>
            pub.GrupoIds != null && pub.GrupoIds.Count > 0;

        private static async Task<List<ColaboradorNotificaPlataformaDTO>> ObterDestinatariosNotificacaoOficialAsync(
            IComunicacaoConfiguracaoRepository repo,
            PublicacaoOficialEnvioDTO pub,
            bool enviarSempreNotificacao)
        {
            if (!pub.PublicacaoOficial && !string.IsNullOrWhiteSpace(pub.ComunidadeId))
                return await repo.ListarColaboradoresComNotificaPlataformaPorComunidadeAsync(pub.OrgId, pub.ComunidadeId);

            var porGrupo = PublicacaoRestringeAosGrupos(pub);
            if (pub.PublicacaoOficial && enviarSempreNotificacao)
            {
                if (porGrupo)
                    return await repo.ListarColaboradoresAtivosNaOrgParaNotificacaoPorGruposAsync(pub.OrgId, pub.GrupoIds);
                return await repo.ListarColaboradoresAtivosNaOrgParaNotificacaoAsync(pub.OrgId);
            }
            if (porGrupo)
                return await repo.ListarColaboradoresComNotificaPlataformaPorGruposAsync(pub.OrgId, pub.GrupoIds);
            return await repo.ListarColaboradoresComNotificaPlataformaAsync(pub.OrgId);
        }

        private static async Task<List<ColaboradorNotificaEmailDTO>> ObterDestinatariosEmailOficialAsync(
            IComunicacaoConfiguracaoRepository repo,
            PublicacaoOficialEnvioDTO pub,
            bool enviarSempreEmailParaPubOficial)
        {
            if (!pub.PublicacaoOficial && !string.IsNullOrWhiteSpace(pub.ComunidadeId))
                return await repo.ListarColaboradoresComNotificaEmailPorComunidadeAsync(pub.OrgId, pub.ComunidadeId);

            var porGrupo = PublicacaoRestringeAosGrupos(pub);
            if (pub.PublicacaoOficial && enviarSempreEmailParaPubOficial)
            {
                if (porGrupo)
                    return await repo.ListarColaboradoresComEmailCadastradoAtivosNaOrgPorGruposAsync(pub.OrgId, pub.GrupoIds);
                return await repo.ListarColaboradoresComEmailCadastradoAtivosNaOrgAsync(pub.OrgId);
            }
            if (porGrupo)
                return await repo.ListarColaboradoresComNotificaEmailPorGruposAsync(pub.OrgId, pub.GrupoIds);
            return await repo.ListarColaboradoresComNotificaEmailAsync(pub.OrgId);
        }
    }
}
