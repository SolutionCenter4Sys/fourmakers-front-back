using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using Aws.Infra.Impl;
using BotFourmakers.Domain.Impl;
using BotFourmakers.Domain.Impl.Chat;
using BotFourmakers.Domain.Impl.Feedback;
using BotFourmakers.Domain.Impl.Questao;
using BotFourmakers.Domain.Interfaces;
using BotFourmakers.Domain.Interfaces.Chat;
using BotFourmakers.Domain.Interfaces.Feedback;
using BotFourmakers.Domain.Interfaces.Questao;
using Colaboracao.Infra.Repositories.BotFourmakers;
using Colaboracao.Infra.Repositories.BotFourmakers.Chat;
using Colaboracao.Infra.Repositories.BotFourmakers.Feedback;
using Colaboracao.Infra.Repositories.BotFourmakers.Questao;
using Core.Domain.BotFourmakers;
using Core.Domain.BotFourmakers.Chat;
using Core.Domain.BotFourmakers.Feedback;
using Core.Domain.BotFourmakers.Questao;
using Logs.Infra.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Colaboracao.Initializer;

public static partial class InitializerExtension
{
    public static void AddBotFourmakersDependencies(this IServiceCollection services, bool transient = false)
    {
        if (transient)
        {
            services.TryAddTransient<IQuestaoServices, QuestaoServices>();
            services.TryAddTransient<IQuestaoRepository, QuestaoRepository>();
            services.TryAddTransient<IChatServices, ChatServices>();
            services.TryAddTransient<IChatRepository, ChatRepository>();
            services.TryAddTransient<IChatIAClient, ChatIAClient>();
            services.TryAddTransient<IFeedbackIAService, FeedbackIAService>();
            services.TryAddTransient<IFeedbackAIRepository, FeedbackAIRepository>();
        }
        else
        {
            services.TryAddScoped<IQuestaoServices, QuestaoServices>();
            services.TryAddScoped<IQuestaoRepository, QuestaoRepository>();
            services.TryAddScoped<IChatServices, ChatServices>();
            services.TryAddScoped<IChatRepository, ChatRepository>();
            services.TryAddScoped<IChatIAClient, ChatIAClient>();
            services.TryAddScopedDomainService<IFeedbackIAService, FeedbackIAService>();
            services.TryAddScoped<IFeedbackAIRepository, FeedbackAIRepository>();
        }
    }
}