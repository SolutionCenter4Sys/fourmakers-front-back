using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rotina.Aws.Core.Interfaces;

namespace Rotina.Aws.Core
{
    public class AppBuilder : IAppBuilder
    {
        public async Task<int> CreateJobBuilder<T>(string[] args, IInitializer initializer)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            initializer.Configure(builder.Services, builder.Configuration);

            var app = builder.Build();

            int exitCode = 0;

            try
            {
                var job = app.Services.GetRequiredService<T>();

                if (job is IJobRunner jobRunner)
                {
                    await jobRunner.ExecuteJobAsync();
                }
                else
                {
                    Console.WriteLine($"The resolved service is not an {nameof(IJobRunner)}");
                    exitCode = 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                exitCode = 1;
            }
            finally
            {
                app.Lifetime.StopApplication();
            }

            return exitCode;
        }
    }
}