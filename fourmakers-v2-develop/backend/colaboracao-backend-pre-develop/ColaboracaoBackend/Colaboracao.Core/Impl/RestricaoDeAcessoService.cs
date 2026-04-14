#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Core.Domain.Usuario.Restricao;

namespace Colaboracao.Core.Impl;

public class RestricaoDeAcessoService(
    IRestricaoDeAcessoRepository restricaoDeAcessoRepository,
    IMemoryCacheService memoryCacheService
) : IRestricaoDeAcessoService
{
    private readonly TimeSpan _cacheExpirationTime = TimeSpan.FromMinutes(0);
    
    public async Task<List<string>?> ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(
        string? codigoInternoColaborador,
        int? orgId,
        string restricaoTipo,
        string? valorPersonalizado = null)
    {
        var restricoesCache = await ListarRestricoesCache(codigoInternoColaborador, orgId, restricaoTipo);

        // Se não houver restrições no cache
        if (restricoesCache is { Count: 0 } || restricoesCache == null)
        {
            if (valorPersonalizado != null && valorPersonalizado.EhStringValidaEDiferenteDeZero())
            {
                return new List<string> { valorPersonalizado };
            }
            return null;
        }

        // Se houver valor personalizado, validar se ele está no cache
        if (valorPersonalizado != null && valorPersonalizado.EhStringValidaEDiferenteDeZero())
        {
            if (!restricoesCache.Contains(valorPersonalizado))
            {
                throw new ValidationException(
                    $"RESTRIÇÃO DE ACESSO: Acesso não permitido a {restricaoTipo}, com o código: {valorPersonalizado}");
            }

            return new List<string> { valorPersonalizado };
        }

        return restricoesCache;
    }
    
    public async Task<List<string>?> ListarMinhasRestricoesDeAcessoPorCodigoInternoETipoComListaDeValores(
        string? codigoInternoColaborador,
        int orgId,
        string restricaoTipo,
        List<string>? valoresPersonalizados = null)
    {
        var restricoesCache = await ListarRestricoesCache(codigoInternoColaborador, orgId, restricaoTipo);

        if (restricoesCache is { Count: 0 })
            return null;

        if (valoresPersonalizados == null || valoresPersonalizados.Count == 0)
            return restricoesCache;

        var codigosNaoPermitidos = valoresPersonalizados
            .Where(c => restricoesCache != null && !restricoesCache.Contains(c))
            .ToList();

        if (codigosNaoPermitidos.Any())
            throw new ValidationException(
                $"RESTRIÇÃO DE ACESSO: Acesso não permitido a {restricaoTipo}, com os códigos: {string.Join(", ", codigosNaoPermitidos)}");

        return valoresPersonalizados;
    }

    private async Task<List<string>?> ListarRestricoesCache(
        string? codigoInternoColaborador,
        int? orgId,
        string tipo)
    {
        string cacheKey = $"RestricoesDeAcesso_{codigoInternoColaborador}_{orgId}_{tipo}";

        // Usa o serviço de cache genérico
        if (!memoryCacheService.GetCachedValue(cacheKey, out List<string>? resultado) || resultado == null)
        {
            var restricoes = await restricaoDeAcessoRepository
                .ListarRestricoesPorCodigoInternoETipo(codigoInternoColaborador, orgId, tipo);

            memoryCacheService.SetCachedValue(cacheKey, restricoes, _cacheExpirationTime);

            return restricoes;
        }

        return resultado;
    }
}
