using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra;
using Colaboracao.Infra.Repositories;
using Colaboracao.Infra.Repositories.Firebase;
using Colaboracao.Infra.Repositories.Notificacao;
using Colaboracao.Infra.Repositories.Social;
using Colaboracao.Infra.Repositories.Social.AtendimentoFourmakers;
using Colaboracao.Infra.Repositories.Vaga;
using Colaboracao.Initializer;
using Comunicacao.Infra;
using Core.Domain.Notificacao;
using Core.Domain.Social;
using Core.Domain.Social.AtendimentoFourmakers;
using Core.Domain.Vaga;
using Core.DomainModel;
using Firebase.Domain.Impl.Services;
using Firebase.Domain.Interfaces;
using Firebase.Domain.Interfaces.Services;
using Logs.Infra.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Social.Domain.Impl;
using Social.Domain.Impl.AtendimentoFourmakers;
using Social.Domain.Interfaces;
using Social.Domain.Interfaces.AtendimentoFourmakers;

namespace Social.Application;

public class Initializer : IInitializerConfigurator
{
    public void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        services.AddBaseGeralServicosColaboracao(config);
        services.AddServicoLogDBDependencies();

        services.AddScoped(typeof(IAspNetUser), typeof(AspNetUser));
        services.AddScopedDomainService<IComentarioCandidaturaService, ComentarioCandidaturaService>();
        services.AddScopedDomainService<IComentarioVagaService, ComentarioVagaService>();
        services.AddScoped(typeof(IComentarioCandidaturaRepository), typeof(ComentarioCandidaturaRepository));
        services.AddScoped(typeof(IComentarioVagaRepository), typeof(ComentarioVagaRepository));
        services.AddScopedDomainService<IJornadaComercialAppService, JornadaComercialAppService>();
        services.AddScoped(typeof(IJornadaComercialAppRepository), typeof(JornadaComercialAppRepository));
        services.AddScoped(typeof(ICandidaturaRepository), typeof(CandidaturaRepository));
        services.AddScoped(typeof(IVagaFourmakersRepository), typeof(VagaFourmakersRepository));
        services.AddScopedDomainService<IFeedback360Service, Feedback360Service>();
        services.AddScoped(typeof(IFeedback360Repository), typeof(Feedback360Repository));
        services.AddScoped(typeof(IFirebaseSDK), typeof(FirebaseSDK));
        services.AddScopedDomainService<IFirebaseService, FirebaseService>();
        services.AddScoped(typeof(IFirebaseRepository), typeof(FirebaseRepository));
        services.AddScopedDomainService<INotificacaoService, NotificacaoService>();
        services.AddScoped(typeof(INotificacaoRepository), typeof(NotificacaoRepository));
        services.AddScoped(typeof(IConnectionStringCore), typeof(ConnectionStringCore));
        services.AddScoped(typeof(IEnvioEmail), typeof(Colaboracao.Core.EnvioEmail));
        services.AddScoped(typeof(IEncontrosBigNumbersRepository), typeof(EncontrosBigNumbersRepository));
        services.AddScopedDomainService<IEncontrosBigNumbersService, EncontrosBigNumbersService>();

        // AtendimentoFourmakers — Services
        services.AddScopedDomainService<IAssistenteChatService, AssistenteChatService>();
        services.AddScopedDomainService<IChamadoService, ChamadoService>();
        services.AddScopedDomainService<IKnowledgeBaseService, KnowledgeBaseService>();
        services.AddScopedDomainService<IMaterialAreaService, MaterialAreaService>();
        services.AddScopedDomainService<IAssistenteConfigService, AssistenteConfigService>();
        services.AddScopedDomainService<IAuditoriaAtendimentoService, AuditoriaAtendimentoService>();
        services.AddScopedDomainService<ICuradoriaService, CuradoriaService>();
        services.AddScopedDomainService<IWebhookWhatsAppService, WebhookWhatsAppService>();

        // AtendimentoFourmakers — Repositories
        services.AddScoped(typeof(IKbChunkRepository), typeof(KbChunkRepository));
        services.AddScoped(typeof(IChamadoRepository), typeof(ChamadoRepository));
        services.AddScoped(typeof(IAssistenteChatLogRepository), typeof(AssistenteChatLogRepository));
        services.AddScoped(typeof(IAssistenteConfigRepository), typeof(AssistenteConfigRepository));
        services.AddScoped(typeof(IMaterialAreaRepository), typeof(MaterialAreaRepository));
        services.AddScoped(typeof(IKbFonteMetaRepository), typeof(KbFonteMetaRepository));
        services.AddScoped(typeof(IAuditoriaAtendimentoRepository), typeof(AuditoriaAtendimentoRepository));
        services.AddScoped(typeof(IWhatsAppChamadoPromptRepository), typeof(WhatsAppChamadoPromptRepository));

        // AtendimentoFourmakers — Clients externos
        services.AddScoped(typeof(IOpenAiClient), typeof(OpenAiClient));
        services.AddScoped(typeof(IEvolutionApiService), typeof(EvolutionApiService));
    }

    public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projetcName)
    {
        app.ConfigureAppStandard(env, projetcName);
    }
}