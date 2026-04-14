using Apontamento.Domain.Interfaces.Service;
using DataTransferObject.Domain.Apontamento;
using Logs.Infra.Attributes;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace Apontamento.Domain.Impl.Service
{
    [LogDomainClass]
    public class ApontamentoCacheService : IApontamentoCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheExpirationTime = TimeSpan.FromMinutes(30);

        public ApontamentoCacheService(IMemoryCache cache, IServiceProvider serviceProvider)
        {
            _cache = cache;
        }

        public TemplateSemanaVigenciaResult ObterTemplateSemanaVigenciaCache(int mes, int ano, int diaQuebraSemana, string idioma, int orgId)
        {
            string cacheKey = $"TemplateSemanaVigencia_{mes}_{ano}_{diaQuebraSemana}_{idioma}_{orgId}";

            if (!_cache.TryGetValue(cacheKey, out TemplateSemanaVigenciaResult resultado))
            {
                return null;
            }

            return resultado;
        }

        public void AtualizarTemplateSemanaVigenciaCache(int mes, int ano, int diaQuebraSemana, TemplateSemanaVigenciaResult templateSemanaVigencia, string idioma, int orgId)
        {
            string cacheKey = $"TemplateSemanaVigencia_{mes}_{ano}_{diaQuebraSemana}_{idioma}_{orgId}";
            _cache.Set(cacheKey, templateSemanaVigencia, _cacheExpirationTime);
        }

        public List<StatusApontamentoResult> ObterListaStatusCache(string idioma)
        {
            string cacheKey = $"ListaStatus_{idioma}";

            if (!_cache.TryGetValue(cacheKey, out List<StatusApontamentoResult> resultado))
            {
                return null;
            }

            return resultado;
        }

        public void AtualizarListaStatusCache(List<StatusApontamentoResult> listaStatus, string idioma)
        {
            string cacheKey = $"ListaStatus_{idioma}";
            _cache.Set(cacheKey, listaStatus, _cacheExpirationTime);
        }

        public List<FeriadoDTO> ObterListaFeriadoCache(int orgId)
        {
            string cacheKey = $"ListaFeriado_{orgId}";

            if (!_cache.TryGetValue(cacheKey, out List<FeriadoDTO> resultado))
            {
                return null;
            }

            return resultado;
        }

        public void AtualizarListaFeriadoCache(List<FeriadoDTO> listaFeriados, int orgId)
        {
            string cacheKey = $"ListaFeriado_{orgId}";
            _cache.Set(cacheKey, listaFeriados, _cacheExpirationTime);
        }
    }
}