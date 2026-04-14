
using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using Aws.Infra.Impl;
using Aws.Infra.Interfaces;
using Colaboracao.Infra.Repositories.Apontamento;
using Colaboracao.Infra.Repositories.Financeiro.Conciliacao;
using Colaboracao.Infra.Repositories.Financeiro.Holerite;
using Colaboracao.Infra.Repositories.Financeiro.IntegracaoContabil;
using Colaboracao.Infra.Repositories.Lote;
using Colaboracao.Infra.Repositories.Projeto;
using Colaboracao.Infra.Repositories.Usuario;
using Core.Domain.Apontamento;
using Core.Domain.Financeiro.Conciliacao;
using Core.Domain.Financeiro.Holerite;
using Core.Domain.Financeiro.IntegracaoContabil;
using Core.Domain.Projeto;
using Core.Domain.Usuario;
using Core.DomainModel.Projeto;
using Financeiro.Domain.Impl.Conciliacao;
using Financeiro.Domain.Impl.Holerite;
using Financeiro.Domain.Impl.IntegracaoContabil;
using Financeiro.Domain.Interfaces.Conciliacao;
using Financeiro.Domain.Interfaces.Holerite;
using Financeiro.Domain.Interfaces.IntegracaoContabil;
using Logs.Infra.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Colaboracao.Initializer
{
    public static partial class InitializerExtension
	{
        public static void AddHoleriteServicesDependencies(this IServiceCollection services)
        {
            // Adicionar dependências do Holerite
            services.AddScoped(typeof(IQueueProducer), typeof(AmazonSQSProducer));
            services.AddScoped(typeof(ILoteRepository), typeof(LoteRepository));
            services.AddScoped(typeof(IUsuarioColaboradorRepository), typeof(UsuarioColaboradorDapperRepository));
            services.AddScoped(typeof(ICurriculoClient), typeof(CurriculoClient));
            services.AddScoped(typeof(IHoleriteRepository), typeof(HoleriteRepository));
            services.AddScopedDomainService<IHoleriteService, HoleriteService>();
            services.AddScopedDomainService<IIntegracaoContabilService, IntegracaoContabilService>();
            services.AddScoped(typeof(IIntegracaoContabilRepository), typeof(IntegracaoContabilRepository));
            services.AddScopedDomainService<IConciliacaoService, ConciliacaoService>();
            services.AddScoped(typeof(IConciliacaoFolhaPontoRepository), typeof(ConciliacaoFolhaPontoRepository));
            services.AddScoped(typeof(IFolhaPontoRepository), typeof(FolhaPontoRepository));
            services.AddScopedDomainService<IHoleriteColaboradorService, HoleriteColaboradorService>();
            services.AddScoped(typeof(IUploadFilesClient), typeof(UploadFilesClient));
            services.AddScoped(typeof(IClienteOrgRepository), typeof(ClienteOrgRepository));
            services.AddScoped(typeof(IAtividadeProjetoRepository), typeof(AtividadeProjetoRepository));
            services.AddScoped(typeof(IProjetoOrgRepository), typeof(ProjetoOrgRepository));
        }
    }
}
