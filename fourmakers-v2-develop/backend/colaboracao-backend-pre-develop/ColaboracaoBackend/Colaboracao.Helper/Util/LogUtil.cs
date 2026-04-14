using DataTransferObject.Domain.Log;
using Newtonsoft.Json;

namespace Colaboracao.Helper.Util
{
    public static class LogUtil
    {
        public static string MontarMensagemLogFormatada(LogDTO log)
        {
            if (log == null) return string.Empty;

            var mensagemObj = new
            {
                Log__Tipo = log.Tipo,
                Log__Data = log.Data,
                Log__Identificador = log.Identificador,
                Log__CodigoColaboradorInternoOrigem = log.CodigoColaboradorInternoOrigem
            };

            var mensagem = JsonConvert.SerializeObject(mensagemObj, Formatting.None);

            mensagem = mensagem.Trim('{', '}');
            mensagem = mensagem.Replace("\":\"", "\" = \"");
            mensagem = mensagem.Replace("\"", "");
            mensagem = mensagem.Replace(",", ", ");
            mensagem = mensagem.Replace("Log__", "Log.");

            return mensagem;
        }

        public static T MontarRetornoLog<T>(LogDTO log)
        {
            if (log == null || string.IsNullOrWhiteSpace(log.MensagemCompleta))
                return default;

            try
            {
                return JsonConvert.DeserializeObject<T>(log.MensagemCompleta);
            }
            catch
            {
                return default;
            }
        }
    }
}
