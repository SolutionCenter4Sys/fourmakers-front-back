using Microsoft.AspNetCore.Builder;

namespace Rotina.Aws.Core.Interfaces;

public interface IAppBuilder
{
    Task<int> CreateJobBuilder<T>(string[] args, IInitializer initializer);
    
}