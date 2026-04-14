using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using Colaboracao.Infra.Repositories;
using Colaboracao.Infra.Repositories.Colaborador;
using Colaboracao.Infra.Repositories.Projeto;
using Colaboracao.Infra.Repositories.Questionario;
using Colaborador.Domain.Impl.Services;
using Colaborador.Domain.Interfaces.Services;
using Core.Domain;
using Core.Domain.Colaborador;
using Core.Domain.Questionario;
using Core.DomainModel.Projeto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Questionario.Domain.Impl;
using Questionario.Domain.Interfaces;

namespace Colaboracao.Initializer;

public static partial class InitializerExtension
{
    public static void AddQuestionarioServicesDependencies(this IServiceCollection services)
    {
        services.TryAddScoped<IQuestionarioRespostaRepository, QuestionarioRespostaRepository>();
        services.TryAddScoped
            <IQuestionarioService, QuestionarioService>();
        services.AddColaboradorServicesDependencies();
        services.AddHistoricoCVDependencies();
        services.AddTemplateRepositoryDependencies();
        services.AddCidadaniaServicesDependencies();     
    }
}