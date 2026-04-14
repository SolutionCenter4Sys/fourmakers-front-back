using Microsoft.Extensions.Localization;

namespace Colaboracao.Helper
{
    public static class StringLocalizerExtension
    {
        public static string GetStringOuVazio(this IStringLocalizer stringLocalizer, string name)
        {
            return stringLocalizer.GetString(name).Value ?? string.Empty;
        }
    }
}