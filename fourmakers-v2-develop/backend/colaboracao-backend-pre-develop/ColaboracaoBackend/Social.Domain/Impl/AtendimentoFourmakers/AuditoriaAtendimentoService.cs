using Core.Domain.Social.AtendimentoFourmakers;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using Logs.Infra.Attributes;
using Social.Domain.Interfaces.AtendimentoFourmakers;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.Impl.AtendimentoFourmakers
{
    [LogDomainClass]
    public class AuditoriaAtendimentoService : IAuditoriaAtendimentoService
    {
        private readonly IAuditoriaAtendimentoRepository _auditoriaRepository;

        public AuditoriaAtendimentoService(IAuditoriaAtendimentoRepository auditoriaRepository)
        {
            _auditoriaRepository = auditoriaRepository;
        }

        public async Task<ApiGenericResult<AuditoriaListaResult>> ListarAsync(FiltroAuditoriaInput filtro, int orgId)
        {
            var result = new ApiGenericResult<AuditoriaListaResult>();

            var limite = Math.Clamp(filtro?.Limite ?? 30, 1, 100);
            var offset = Math.Max(filtro?.Offset ?? 0, 0);

            DateTime? dataInicio = null;
            DateTime? dataFim = null;

            if (!string.IsNullOrEmpty(filtro?.DataInicio) &&
                DateTime.TryParseExact(filtro.DataInicio, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var di))
                dataInicio = di;

            if (!string.IsNullOrEmpty(filtro?.DataFim) &&
                DateTime.TryParseExact(filtro.DataFim, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var df))
                dataFim = df;

            var busca = filtro?.Busca;
            if (busca != null && busca.Length > 200)
                busca = busca[..200];

            var (itens, temMais) = await _auditoriaRepository.ListarAsync(
                orgId, limite, offset, dataInicio, dataFim, filtro?.Categoria, busca);

            result.Retorno = new AuditoriaListaResult
            {
                Itens = itens.ToList(),
                TemMais = temMais,
                Limite = limite,
                Offset = offset
            };

            return result;
        }

        public async Task RegistrarAsync(AuditoriaInsertInput input, int orgId)
        {
            await _auditoriaRepository.InserirAsync(input, orgId);
        }

        public async Task<byte[]> ExportarCsvAsync(FiltroAuditoriaInput filtro, int orgId)
        {
            DateTime? dataInicio = null;
            DateTime? dataFim = null;

            if (!string.IsNullOrEmpty(filtro?.DataInicio) &&
                DateTime.TryParseExact(filtro.DataInicio, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var di))
                dataInicio = di;

            if (!string.IsNullOrEmpty(filtro?.DataFim) &&
                DateTime.TryParseExact(filtro.DataFim, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var df))
                dataFim = df;

            var itens = await _auditoriaRepository.ListarParaCsvAsync(
                orgId, dataInicio, dataFim, filtro?.Categoria, filtro?.Busca, 2500);

            var sb = new StringBuilder();
            sb.AppendLine("Data,Responsável,Ação,Detalhe,Categoria,Tipo Entidade,ID Entidade");

            foreach (var item in itens)
            {
                sb.AppendLine($"{EscapeCsv(item.DataCriacao.ToString("yyyy-MM-dd HH:mm:ss"))}," +
                    $"{EscapeCsv(item.ActorLabel)}," +
                    $"{EscapeCsv(item.Acao)}," +
                    $"{EscapeCsv(item.Detalhe)}," +
                    $"{EscapeCsv(item.Categoria)}," +
                    $"{EscapeCsv(item.TipoEntidade)}," +
                    $"{EscapeCsv(item.EntidadeId)}");
            }

            var bom = Encoding.UTF8.GetPreamble();
            var csvBytes = Encoding.UTF8.GetBytes(sb.ToString());
            var resultado = new byte[bom.Length + csvBytes.Length];
            Buffer.BlockCopy(bom, 0, resultado, 0, bom.Length);
            Buffer.BlockCopy(csvBytes, 0, resultado, bom.Length, csvBytes.Length);

            return resultado;
        }

        private static string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Contains('"') || value.Contains(',') || value.Contains('\n'))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }
    }
}
