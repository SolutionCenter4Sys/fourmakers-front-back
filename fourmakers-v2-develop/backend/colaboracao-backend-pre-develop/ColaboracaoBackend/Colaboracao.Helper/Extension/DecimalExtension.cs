using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Colaboracao.Helper.Extension;

public static class DecimalExtensions
{
    public static decimal ParseDecimalUniversal(this object valor)
    {
        if (valor == null)
            return 0m;

        // decimal ou decimal?
        if (valor is decimal d)
            return d;

        var texto = valor.ToString()?.Trim();

        if (string.IsNullOrWhiteSpace(texto))
            return 0m;

        // Remove símbolos de moeda, espaços, etc
        texto = Regex.Replace(texto, @"[^\d.,\-]", "");

        bool hasComma = texto.Contains(",");
        bool hasDot   = texto.Contains(".");

        decimal result;

        // vírgula como separador decimal
        if (hasComma && (!hasDot || texto.LastIndexOf(',') > texto.LastIndexOf('.')))
        {
            texto = texto.Replace(".", "");
            texto = texto.Replace(",", ".");
        }
        // ponto como separador decimal
        else if (hasDot)
        {
            texto = texto.Replace(",", "");
        }

        if (decimal.TryParse(
                texto,
                NumberStyles.Number | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out result))
        {
            return result;
        }

        throw new FormatException($"Não foi possível converter '{valor}' para decimal.");
    }
}