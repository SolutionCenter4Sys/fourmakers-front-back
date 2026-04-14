namespace Colaboracao.Helper
{
    public static class UploadFileUtil
    {
        public static string GetBaseUrlDownloadTokenFile()
        {
            return VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("SERVICE_MEDIA_BASE_URL")?.Replace("$1", "tokenfile").TrimEnd('/');
        }
    }
}
