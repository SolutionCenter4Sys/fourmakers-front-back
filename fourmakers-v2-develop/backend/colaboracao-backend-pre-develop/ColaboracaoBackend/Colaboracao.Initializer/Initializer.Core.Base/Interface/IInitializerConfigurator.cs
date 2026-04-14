using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public interface IInitializerConfigurator
{
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string appName);
}