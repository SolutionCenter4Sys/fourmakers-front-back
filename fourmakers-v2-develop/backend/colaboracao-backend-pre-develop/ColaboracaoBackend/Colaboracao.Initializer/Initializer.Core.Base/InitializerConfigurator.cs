using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;

namespace Colaboracao.Initializer.Initializer.Core.Base
{
    public class ApplicationConfigurator
    {
        public static WebApplication ConfigureApplication(WebApplicationBuilder builder, string appName, IInitializerConfigurator initializer)
        {
            // configuração de services específicos de cada aplicação
            initializer.ConfigureServices(builder.Services, builder.Configuration);

            builder.Services.AddControllers();

            ConfigureSpecificConfigurationByAppName(builder.Services, appName);

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = appName, Version = "v1" });

                c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    Description = "Basic Authorization header using the Bearer scheme."
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "bearer"
                              }
                          },
                         new string[] {}
                    }
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddHealthChecks();

            var app = builder.Build();

            initializer.ConfigureAppStandard(app, app.Environment, appName);

            return app;
        }

        private static void ConfigureSpecificConfigurationByAppName(IServiceCollection services, string appName)
        {
            if (appName == "Apontamento.API"
                || appName == "Usuario.API")
            {
                services.AddMemoryCache();
            }

            if (appName == "Colaborador.API")
            {
                // Configuração utilizada para converter enums para strings no JSON com o StringEnumConverter.
                // Além disso, AddNewtonsoftJson() habilita o uso de máscaras de data no formato "dd-MM-yyyy". 🤮
                // Atualmente, esta configuração é utilizada exclusivamente no serviço Colaborador.API.

                services.AddSwaggerGenNewtonsoftSupport();
                services.AddMvc().AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });
            }
        }
    }
}