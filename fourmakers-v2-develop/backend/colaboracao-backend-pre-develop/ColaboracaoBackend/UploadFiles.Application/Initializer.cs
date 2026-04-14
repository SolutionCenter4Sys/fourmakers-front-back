using Amazon.S3;
using ApiClient.Domain;
using Aws.Infra.Impl.S3;
using Aws.Infra.Interfaces.S3;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Repositories;
using Colaboracao.Initializer;
using Core.DomainModel;
using Microsoft.AspNetCore.Builder;
using UploadFiles.Domain.Interfaces.Services;
using UploadFiles.Domain.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UploadFiles.Application
{
    public class Initializer : IInitializerConfigurator
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            ClientConfig.SetConfiguration(ref config);

            JWTAuth.ConfigureJWT(services);

            // UploadFiles não utiliza ColaboradorContext; Dapper (IDBConnection) é usado para token file / token file temp

            services.AddServicosComunsColaboracao();
            services.AddMemoryCache();

            services.AddTransient(typeof(IAmazonS3), typeof(AmazonS3Client));
            services.AddTransient(typeof(IAmazonS3Uploader), typeof(AmazonS3Uploader));
            services.AddTransient(typeof(IUploadFiles), typeof(Colaboracao.Core.Impl.UploadFiles));
            services.AddTransient(typeof(ITokenFileRepository), typeof(TokenFileRepository));
            services.AddScoped(typeof(ITokenFileTempRepository), typeof(TokenFileTempRepository));
            services.AddScoped(typeof(ITokenFileTempService), typeof(TokenFileTempService));
            
        }

        public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projetcName)
        {
            app.ConfigureAppStandard(env, projetcName);
        }
    }
}