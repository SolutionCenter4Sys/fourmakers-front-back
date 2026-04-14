using DataTransferObject.Domain.Apontamento;
using Logs.Infra.Attributes;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Usuario.Domain.Interfaces.Services;

namespace Usuario.Domain.Impl.Services
{
    [LogDomainClass]
    public class UsuarioCacheService : IUsuarioCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheExpirationTime = TimeSpan.FromMinutes(1);

        public UsuarioCacheService(IMemoryCache cache, IServiceProvider serviceProvider)
        {
            _cache = cache;
        }

        public IEnumerable<RecursoMenuAninhadoResult> ObterListarRecursosVisaoMenuCache(string codigoInternoColaborador, int orgId)
        {
            string cacheKey = $"TemplateSemanaVigencia_{codigoInternoColaborador}_{orgId}";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<RecursoMenuAninhadoResult> resultado))
            {
                return null;
            }

            return resultado;
        }

        public void AtualizarRecursosVisaoMenuCache(IEnumerable<RecursoMenuAninhadoResult> recursoMenuAninhado, string codigoInternoColaborador, int orgId)
        {
            string cacheKey = $"TemplateSemanaVigencia_{codigoInternoColaborador}_{orgId}";
            _cache.Set(cacheKey, recursoMenuAninhado, _cacheExpirationTime);
        }

    }
}