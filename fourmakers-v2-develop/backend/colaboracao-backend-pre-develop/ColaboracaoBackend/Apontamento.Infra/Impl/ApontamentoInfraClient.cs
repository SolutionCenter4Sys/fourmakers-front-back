using Apontamento.Infra.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Apontamento.Infra.Impl
{
    public class ApontamentoInfraClient : IApontamentoInfraClient
    {
        private readonly IConfiguration _configuration;

        public ApontamentoInfraClient(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
    }
}