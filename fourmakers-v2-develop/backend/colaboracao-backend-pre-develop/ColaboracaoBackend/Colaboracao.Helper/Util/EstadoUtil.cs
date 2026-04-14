using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Helper
{
    public static class EstadoUtil
    {

        /// <summary>
        /// Converte o nome de um estado brasileiro para sua abreviação de 2 letras
        /// </summary>
        /// <param name="nomeEstado">Nome do estado (ex: "São Paulo", "Goiás", "Minas Gerais")</param>
        /// <returns>Abreviação de 2 letras do estado (ex: "SP", "GO", "MG")</returns>
        public static string ConverterParaAbreviacao(string nomeEstado)
        {

           var EstadosBrasileiros = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // Estados com nomes completos
                { "Acre", "AC" },
                { "Alagoas", "AL" },
                { "Amapá", "AP" },
                { "Amazonas", "AM" },
                { "Bahia", "BA" },
                { "Ceará", "CE" },
                { "Distrito Federal", "DF" },
                { "Espírito Santo", "ES" },
                { "Goiás", "GO" },
                { "Maranhão", "MA" },
                { "Mato Grosso", "MT" },
                { "Mato Grosso do Sul", "MS" },
                { "Minas Gerais", "MG" },
                { "Pará", "PA" },
                { "Paraíba", "PB" },
                { "Paraná", "PR" },
                { "Pernambuco", "PE" },
                { "Piauí", "PI" },
                { "Rio de Janeiro", "RJ" },
                { "Rio Grande do Norte", "RN" },
                { "Rio Grande do Sul", "RS" },
                { "Rondônia", "RO" },
                { "Roraima", "RR" },
                { "Santa Catarina", "SC" },
                { "São Paulo", "SP" },
                { "Sergipe", "SE" },
                { "Tocantins", "TO" },
                
                // Variações comuns
                { "Espirito Santo", "ES" },
                { "Sao Paulo", "SP" }
            };

            if (string.IsNullOrWhiteSpace(nomeEstado))
                return string.Empty;

            // Remove espaços extras e normaliza
            var estadoNormalizado = nomeEstado.Trim();

            // Tenta encontrar correspondência exata
            if (EstadosBrasileiros.TryGetValue(estadoNormalizado, out string abreviacao))
            {
                return abreviacao;
            }

            // Se não encontrou correspondência exata, tenta gerar abreviação das primeiras letras
            return GerarAbreviacaoDasPrimeirasLetras(estadoNormalizado);
        }

        /// <summary>
        /// Gera abreviação baseada nas primeiras letras das duas primeiras palavras do estado
        /// </summary>
        /// <param name="nomeEstado">Nome do estado</param>
        /// <returns>Abreviação de 2 letras</returns>
        private static string GerarAbreviacaoDasPrimeirasLetras(string nomeEstado)
        {
            var palavras = nomeEstado.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            if (palavras.Length == 0)
                return string.Empty;

            if (palavras.Length == 1)
            {
                // Se tem apenas uma palavra, pega as duas primeiras letras
                return palavras[0].Length >= 2 
                    ? palavras[0].Substring(0, 2).ToUpperInvariant()
                    : palavras[0].ToUpperInvariant();
            }

            // Se tem duas ou mais palavras, pega a primeira letra de cada uma das duas primeiras
            var primeiraLetra = palavras[0].Length > 0 ? palavras[0][0].ToString() : string.Empty;
            var segundaLetra = palavras[1].Length > 0 ? palavras[1][0].ToString() : string.Empty;

            return (primeiraLetra + segundaLetra).ToUpperInvariant();
        }

        
    }
}
