using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using CargaCore.Domain.Interfaces;
using CargaFourmaker.API.Services;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Repositories.MapaAlocacao.MapaAlocacao.GestaoDeAlocados;
using Colaboracao.Infra.Repositories.Projeto;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Core.DomainModel.Projeto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Colaboracao.Initializer
{
    public static partial class InitializerExtension
    {
        public static void AddCargaClienteSincronizarCRMServiceDependencies(this IServiceCollection services, bool transient = false)
        {
            if (transient)
            {
                services.TryAddTransient<ISincronizaCRMService, SincronizaCRMService>();
                services.TryAddTransient<IColaboracaoBridgeClient, ColaboracaoBridgeClient>();
                services.TryAddTransient<IClienteOrgRepository, ClienteOrgRepository>();
                services.TryAddTransient<IProjetoOrgRepository, ProjetoOrgRepository>();
                services.TryAddTransient<IGestorExternoRepository, GestorExternoRepository>();
                services.TryAddTransient<IDBConnectionUnitOfWork, DBConnectionUnitOfWork>();
                services.AddServicoLogDBDependencies(transient);
            }
            else
            {
                services.TryAddScoped<ISincronizaCRMService, SincronizaCRMService>();
                services.TryAddScoped<IColaboracaoBridgeClient, ColaboracaoBridgeClient>();
                services.TryAddScoped<IClienteOrgRepository, ClienteOrgRepository>();
                services.TryAddScoped<IProjetoOrgRepository, ProjetoOrgRepository>();
                services.TryAddScoped<IGestorExternoRepository, GestorExternoRepository>();
                services.TryAddScoped<IDBConnectionUnitOfWork, DBConnectionUnitOfWork>();
                services.AddServicoLogDBDependencies(transient);
            }
        }
    }
}