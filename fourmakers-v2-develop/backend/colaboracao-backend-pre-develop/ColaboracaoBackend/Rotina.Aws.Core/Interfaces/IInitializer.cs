using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rotina.Aws.Core.Interfaces;

public interface IInitializer
{
    void Configure(IServiceCollection services, IConfiguration config);
}