using ApiClient.Domain.Interfaces;
using Core.Domain.Social.AtendimentoFourmakers;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using Logs.Infra.Attributes;
using Social.Domain.Interfaces.AtendimentoFourmakers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Social.Domain.Impl.AtendimentoFourmakers
{
    [LogDomainClass]
    public class KnowledgeBaseService : IKnowledgeBaseService
    {
        private readonly IKbChunkRepository _kbChunkRepository;
        private readonly IKbFonteMetaRepository _fonteMetaRepository;
        private readonly IAuditoriaAtendimentoRepository _auditoriaRepository;
        private readonly IOpenAiClient _openAiClient;

        private const int MaxChunkSize = 1200;
        private const int ChunkOverlap = 120;

        public KnowledgeBaseService(
            IKbChunkRepository kbChunkRepository,
            IKbFonteMetaRepository fonteMetaRepository,
            IAuditoriaAtendimentoRepository auditoriaRepository,
            IOpenAiClient openAiClient)
        {
            _kbChunkRepository = kbChunkRepository;
            _fonteMetaRepository = fonteMetaRepository;
            _auditoriaRepository = auditoriaRepository;
            _openAiClient = openAiClient;
        }

        public async Task<ApiGenericResult<IngestaoResult>> IngerirAsync(IngerirDocumentoInput input, string codigoInternoColaborador, string nomeColaborador, int orgId)
        {
            var result = new ApiGenericResult<IngestaoResult>();

            if (string.IsNullOrWhiteSpace(input?.Texto))
                throw new ArgumentException("Texto é obrigatório.");

            var texto = input.Texto.Replace("\r\n", "\n");
            var chunks = CriarChunks(texto);

            var textoChunks = chunks.ToList();
            var embeddingsResponse = await _openAiClient.GerarEmbeddingsEmBatchAsync(textoChunks);
            if (!embeddingsResponse.Sucesso)
                throw new ApplicationException("Falha ao gerar embeddings.");

            var tipoFonte = string.IsNullOrEmpty(input.TipoFonte) ? "manual" : input.TipoFonte;

            if (input.SubstituirFonte && !string.IsNullOrEmpty(input.FonteId))
                await _kbChunkRepository.DeletarPorFonteAsync(input.FonteId, orgId);

            var insertInputs = textoChunks.Select((chunk, i) => new KbChunkInsertInput
            {
                Conteudo = chunk,
                EmbeddingJson = JsonSerializer.Serialize(embeddingsResponse.Resposta[i].Vetor),
                TipoFonte = tipoFonte,
                FonteId = input.FonteId,
                Titulo = input.Titulo,
                IndiceChunk = i
            });

            var inseridos = await _kbChunkRepository.InserirEmBatchAsync(insertInputs, orgId);

            if (!string.IsNullOrEmpty(input.FonteId))
            {
                if (!string.IsNullOrEmpty(input.AreaId))
                    await _fonteMetaRepository.UpsertAsync(input.FonteId, input.AreaId, orgId);
                else
                    await _fonteMetaRepository.DeletarAsync(input.FonteId, orgId);
            }

            await _auditoriaRepository.InserirAsync(new AuditoriaInsertInput
            {
                CodigoInternoColaborador = codigoInternoColaborador,
                ActorLabel = nomeColaborador ?? "Sistema",
                Acao = "kb.ingested",
                Detalhe = $"{inseridos} chunks inseridos para '{input.Titulo ?? "sem título"}'",
                Payload = JsonSerializer.Serialize(new
                {
                    chunksInseridos = inseridos,
                    fonteId = input.FonteId,
                    tipoFonte,
                    substituiu = input.SubstituirFonte,
                    tamanhoTexto = input.Texto.Length,
                    titulo = input.Titulo
                }),
                TipoEntidade = "kb_chunk",
                EntidadeId = input.FonteId
            }, orgId);

            result.Retorno = new IngestaoResult { Ok = true, ChunksInseridos = inseridos };
            return result;
        }

        public async Task<ApiGenericResult<RemocaoFonteResult>> RemoverFonteAsync(RemoverFonteInput input, string codigoInternoColaborador, string nomeColaborador, int orgId)
        {
            var result = new ApiGenericResult<RemocaoFonteResult>();

            if (string.IsNullOrWhiteSpace(input?.FonteId))
                throw new ArgumentException("FonteId é obrigatório.");

            var removidos = await _kbChunkRepository.DeletarPorFonteAsync(input.FonteId, orgId);
            await _fonteMetaRepository.DeletarAsync(input.FonteId, orgId);

            await _auditoriaRepository.InserirAsync(new AuditoriaInsertInput
            {
                CodigoInternoColaborador = codigoInternoColaborador,
                ActorLabel = nomeColaborador ?? "Sistema",
                Acao = "kb.deleted",
                Detalhe = $"{removidos} chunks removidos",
                Payload = JsonSerializer.Serialize(new { fonteId = input.FonteId, chunksRemovidos = removidos }),
                TipoEntidade = "kb_chunk",
                EntidadeId = input.FonteId
            }, orgId);

            result.Retorno = new RemocaoFonteResult { Ok = true, ChunksRemovidos = removidos };
            return result;
        }

        public async Task<ApiGenericResult<KbTotaisResult>> ObterTotaisAsync(int orgId)
        {
            var result = new ApiGenericResult<KbTotaisResult>();
            var chunks = await _kbChunkRepository.ContarAsync(orgId);
            var fontes = await _fonteMetaRepository.ContarFontesAsync(orgId);
            result.Retorno = new KbTotaisResult { Chunks = chunks, Fontes = fontes };
            return result;
        }

        public async Task<ApiGenericResult<List<KbFonteResult>>> ListarFontesAsync(int orgId)
        {
            var result = new ApiGenericResult<List<KbFonteResult>>();
            var fontes = await _fonteMetaRepository.ListarAsync(orgId);
            result.Retorno = fontes.ToList();
            return result;
        }

        public async Task<ApiGenericResult<KbFonteDetalheResult>> ObterFontePorIdAsync(string fonteId, int orgId)
        {
            var result = new ApiGenericResult<KbFonteDetalheResult>();

            if (string.IsNullOrWhiteSpace(fonteId))
                throw new ArgumentException("FonteId é obrigatório.");

            var fonte = await _fonteMetaRepository.ObterPorFonteIdAsync(fonteId, orgId);
            if (fonte == null)
                throw new ApplicationException("Fonte não encontrada.");

            var chunks = (await _kbChunkRepository.ListarPorFonteAsync(fonteId, orgId)).ToList();

            result.Retorno = new KbFonteDetalheResult
            {
                FonteId = fonte.FonteId,
                Titulo = fonte.Titulo,
                TipoFonte = fonte.TipoFonte,
                AreaId = fonte.AreaId,
                AreaNome = fonte.AreaNome,
                ChunkCount = fonte.ChunkCount,
                DataCriacao = fonte.DataCriacao,
                DataAlteracao = fonte.DataAlteracao,
                Chunks = chunks
            };

            return result;
        }

        private static List<string> CriarChunks(string texto)
        {
            var paragrafos = Regex.Split(texto, @"\n\s*\n")
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim())
                .ToList();

            var chunks = new List<string>();
            var buffer = "";

            foreach (var paragrafo in paragrafos)
            {
                if (paragrafo.Length > MaxChunkSize)
                {
                    if (!string.IsNullOrEmpty(buffer))
                    {
                        chunks.Add(buffer);
                        buffer = "";
                    }

                    for (var i = 0; i < paragrafo.Length; i += MaxChunkSize - ChunkOverlap)
                    {
                        var end = Math.Min(i + MaxChunkSize, paragrafo.Length);
                        chunks.Add(paragrafo[i..end]);
                    }
                    continue;
                }

                if (buffer.Length + paragrafo.Length + 2 > MaxChunkSize)
                {
                    chunks.Add(buffer);
                    var overlapStart = Math.Max(0, buffer.Length - ChunkOverlap);
                    buffer = buffer[overlapStart..] + "\n\n" + paragrafo;
                }
                else
                {
                    buffer = string.IsNullOrEmpty(buffer) ? paragrafo : buffer + "\n\n" + paragrafo;
                }
            }

            if (!string.IsNullOrEmpty(buffer))
                chunks.Add(buffer);

            return chunks;
        }
    }
}
