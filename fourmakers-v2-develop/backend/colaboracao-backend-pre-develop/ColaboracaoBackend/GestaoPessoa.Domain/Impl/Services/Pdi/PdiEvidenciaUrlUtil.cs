namespace GestaoPessoa.Domain.Impl.Services.Pdi
{
    internal static class PdiEvidenciaUrlUtil
    {
        private const int MaxUrlLength = 2048;

        /// <summary>Retorna URL normalizada ou null se vazia; preenche <paramref name="erro"/> se o valor for inválido.</summary>
        public static string TryNormalize(string link, out string erro)
        {
            erro = null;
            if (string.IsNullOrWhiteSpace(link))
                return null;
            var t = link.Trim();
            if (t.Length > MaxUrlLength)
            {
                erro = "Link da evidência excede o tamanho máximo permitido.";
                return null;
            }
            if (!System.Uri.TryCreate(t, System.UriKind.Absolute, out var u)
                || (u.Scheme != System.Uri.UriSchemeHttp && u.Scheme != System.Uri.UriSchemeHttps))
            {
                erro = "Informe um link http ou https válido.";
                return null;
            }
            return t;
        }
    }
}
