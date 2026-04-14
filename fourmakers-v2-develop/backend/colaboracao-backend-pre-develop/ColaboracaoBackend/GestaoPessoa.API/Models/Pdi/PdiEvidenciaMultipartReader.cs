using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GestaoPessoa.API.Models.Pdi
{
    /// <summary>Lê multipart ou urlencoded (chaves: arquivo, link, tipo). Não usa <see cref="HttpRequest.HasFormContentType"/> sozinho — alguns clientes/proxies variam o header.</summary>
    public static class PdiEvidenciaMultipartReader
    {
        public static async Task<(IFormFile? Arquivo, string? Link, string? Tipo)> ReadAsync(HttpRequest request)
        {
            var ct = request.ContentType;
            if (string.IsNullOrEmpty(ct))
                return (null, null, null);

            var looksLikeForm =
                ct.Contains("multipart/form-data", StringComparison.OrdinalIgnoreCase)
                || ct.Contains("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase);
            if (!looksLikeForm)
                return (null, null, null);

            IFormCollection form;
            try
            {
                form = await request.ReadFormAsync();
            }
            catch
            {
                return (null, null, null);
            }

            var link = FormString(form, "link");
            var tipo = FormString(form, "tipo");

            if (string.IsNullOrEmpty(link))
                link = await TryReadTextFromFilePartAsync(form, "link");

            IFormFile? arquivo = null;
            foreach (var f in form.Files)
            {
                if (string.Equals(f.Name?.Trim(), "arquivo", StringComparison.OrdinalIgnoreCase))
                {
                    arquivo = f;
                    break;
                }
            }

            return (arquivo, link, tipo);
        }

        private static string? FormString(IFormCollection form, string key)
        {
            foreach (var k in form.Keys)
            {
                if (string.IsNullOrEmpty(k))
                    continue;
                if (!string.Equals(k.Trim(), key, StringComparison.OrdinalIgnoreCase))
                    continue;
                var s = form[k].ToString()?.Trim();
                return string.IsNullOrWhiteSpace(s) ? null : s;
            }
            return null;
        }

        /// <summary>Postman/cliente às vezes envia campo “Text” como parte file; lê o stream se existir parte com esse nome.</summary>
        private static async Task<string?> TryReadTextFromFilePartAsync(IFormCollection form, string partName)
        {
            foreach (var f in form.Files)
            {
                if (!string.Equals(f.Name?.Trim(), partName, StringComparison.OrdinalIgnoreCase) || f.Length <= 0)
                    continue;
                using var reader = new StreamReader(f.OpenReadStream());
                var s = (await reader.ReadToEndAsync()).Trim();
                if (!string.IsNullOrWhiteSpace(s))
                    return s;
            }
            return null;
        }
    }
}
