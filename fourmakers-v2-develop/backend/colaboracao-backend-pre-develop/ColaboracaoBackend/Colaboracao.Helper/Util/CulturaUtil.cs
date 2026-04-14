using System.Globalization;
using System.Linq;

namespace Colaboracao.Helper.Util
{
    public static class CultureUtil
    {
        public static string GetCurrentCulture()
        {
            return CultureInfo.CurrentCulture.Name;
        }

        public static CultureInfo[] GetSupportedLanguages()
        {
            var supportedCultures = new[]
            {
                new CultureInfo("pt-BR"),
                new CultureInfo("es-ES"),
                new CultureInfo("en-US")
            };

            return supportedCultures;
        }

        public static bool IsSupportedLanguage(string idioma)
        {
            // Verifica se o idioma informado está entre os suportados
            return CultureUtil.GetSupportedLanguages().Any(culture => culture.Name.Equals(idioma));
        }
    }
}