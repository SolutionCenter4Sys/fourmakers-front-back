using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colaborador.Domain.Interfaces.Services;
using Core.Domain.Colaborador;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Lote;
using Colaboracao.Core.Interfaces;
using Core.Domain.Curriculo;

namespace CurriculoBatchConsumer.Services
{
    public class Service : IService
    {
        private readonly IImportacaoColaboradorService _importacaoColaboradorService;
        private readonly IProcessamentoCurriculoLoteRepository _processamentoCurriculoLoteRepository;
        private readonly ILogger<Service> _logger;
        private readonly IUploadFiles _uploadFiles;
        private readonly ICurriculoColaboradorRespository _curriculoColaboradorRepository;

        public Service(IImportacaoColaboradorService importacaoColaboradorService, IProcessamentoCurriculoLoteRepository processamentoCurriculoLoteRepository, ILogger<Service> logger, IUploadFiles uploadFiles, ICurriculoColaboradorRespository curriculoColaboradorRepository)
        {
            _importacaoColaboradorService = importacaoColaboradorService;
            _processamentoCurriculoLoteRepository = processamentoCurriculoLoteRepository;
            _logger = logger;
            _uploadFiles = uploadFiles;
            _curriculoColaboradorRepository = curriculoColaboradorRepository;
        }

        public async Task<IEnumerable<ProcessamentoCurriculoLoteDTO>> BuscarLotesCurriculosDeHoje()
        {
            return await _processamentoCurriculoLoteRepository.BuscarLotesDiariosPorTipo(DateTime.UtcNow.Date, IdentificadorFilaLote.FILA_CURRICULO_ARQUIVO.ToString());
        }

        public async Task<bool> Processar(DataTransferObject.Domain.Colaborador.ImportacaoColaboradorFilaDTO dto, string idLote, string messageReceiptHandle)
        {
            _logger.LogInformation($"Iniciado processamento do lote {idLote}");
            var lote = _processamentoCurriculoLoteRepository.BuscarPorId(idLote);

            try
            {
                if (lote is null)
                    throw new ArgumentNullException($"Nao existe lote sob este id {idLote}");

                if (lote.DataInicioProcessamento is null)
                    _processamentoCurriculoLoteRepository.IniciaProcessamento(idLote);

                _processamentoCurriculoLoteRepository.CadastrarTabelaAuxiliar(messageReceiptHandle, idLote, nomeArquivoCv: dto.NomeArquivo);

                _logger.LogInformation($"Iniciar processo de importacao - lote {lote.Id} - codigo_interno_colaborador - {lote.ColaboradorCadastrante}");

                // S3Key é obrigatório
                if (string.IsNullOrEmpty(dto.S3Key))
                {
                    throw new Exception("S3Key é obrigatório.");
                }

                _logger.LogInformation($"Recuperando conteúdo do S3: {dto.S3Key}");
                byte[] arquivoBytes;
                
                using (var stream = await _uploadFiles.GetFile(dto.S3Key))
                {
                    if (stream == null)
                    {
                        throw new Exception($"Não foi possível recuperar o arquivo do S3: {dto.S3Key}");
                    }
                    
                    using (var memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);
                        arquivoBytes = memoryStream.ToArray();
                    }
                }
                
                _logger.LogInformation($"Conteúdo recuperado do S3 com sucesso: {dto.S3Key}");
                
                var codColaborador = await _importacaoColaboradorService.ImportarColaborador(arquivoBytes, lote.OrgId, lote.ColaboradorCadastrante, importacaoLote: true);

                _logger.LogInformation($"Colaborador importado {codColaborador}");

                // Gravar o endereço S3 do currículo na tabela tb_curriculo_colaborador usando Dapper
                _curriculoColaboradorRepository.InsereCurriculoColaboradorDapper(dto.S3Key, codColaborador);
                _logger.LogInformation($"Endereço S3 do currículo gravado: {dto.S3Key} para colaborador {codColaborador}");

                _processamentoCurriculoLoteRepository.InserirCodColaboradorNaTabelaAuxiliar(messageReceiptHandle, idLote, codColaborador);
                
                IncrementarQuantidadeProcessadaEMarcarComoProcessadoSeAplicavel(idLote, lote);

                return true;
            }
            catch (Exception e)
            {
                IncrementarQuantidadeProcessadaEMarcarComoProcessadoSeAplicavel(idLote, lote);

                throw e;
            }
        }

        private void IncrementarQuantidadeProcessadaEMarcarComoProcessadoSeAplicavel(string idLote, ProcessamentoCurriculoLoteDTO lote)
        {
            _logger.LogInformation($"IncrementarQuantidadeProcessadaEMarcarComoProcessadoSeAplicavel - lote {lote.Id} - codigo_interno_colaborador - {lote.ColaboradorCadastrante}");
            var quantidadeProcessadaAtualizada = lote.QuantidadeProcessada = lote.QuantidadeProcessada + 1;
            _processamentoCurriculoLoteRepository.IncrementarQuantidadeProcessada(idLote, quantidadeProcessadaAtualizada);

            if (lote.QuantidadeAProcessar == quantidadeProcessadaAtualizada)
            {
                _logger.LogInformation($"MarcarComoProcessado - lote {lote.Id} - codigo_interno_colaborador - {lote.ColaboradorCadastrante}");
                _processamentoCurriculoLoteRepository.MarcarComoProcessado(idLote);
            }
        }

        public async Task<bool> ProcessarImportacaoLinkedin(string body, string idLote, string messageReceiptHandle)
        {
            _logger.LogInformation($"Iniciado processamento do lote {idLote}");
            var lote = _processamentoCurriculoLoteRepository.BuscarPorId(idLote);

            try
            {
                if (lote is null)
                    throw new ArgumentNullException($"Nao existe lote sob este id {idLote} - lote {lote.Id} - codigo_interno_colaborador - {lote.ColaboradorCadastrante}");

                if (lote.DataInicioProcessamento is null)
                    _processamentoCurriculoLoteRepository.IniciaProcessamento(idLote);

                _logger.LogInformation($"Iniciar processo de importacao - lote {lote.Id} - codigo_interno_colaborador - {lote.ColaboradorCadastrante}");

                var codColaborador = await _importacaoColaboradorService.ImportarColaboradorLinkedinLote(body, lote.OrgId, lote.ColaboradorCadastrante);

                _logger.LogInformation($"Colaborador importado {codColaborador} - lote {lote.Id} - codigo_interno_colaborador - {lote.ColaboradorCadastrante}");

                _processamentoCurriculoLoteRepository.CadastrarTabelaAuxiliar(messageReceiptHandle, idLote, codColaborador: codColaborador);

                IncrementarQuantidadeProcessadaEMarcarComoProcessadoSeAplicavel(idLote, lote);

                return true;
            }
            catch (Exception e)
            {
                IncrementarQuantidadeProcessadaEMarcarComoProcessadoSeAplicavel(idLote, lote);

                throw e;
            }
        }

        public async Task<string> ResumoLotesCurriculos()
        {
            var lotesDeHoje = await BuscarLotesCurriculosDeHoje();
            return ConstruirLog(lotesDeHoje);
        }

        private static string ConstruirLog(IEnumerable<ProcessamentoCurriculoLoteDTO> lotesDeHoje)
        {
            return $"Quantidade Processada: {lotesDeHoje.Count(m => m.Processado)} - " +
                            $"Quantidade Nao Processada: {lotesDeHoje.Count(m => !m.Processado)} - " +
                            $"Total: {lotesDeHoje.Count()} - " +
                            $"Quantidade Colaboradores Cadastrados: {lotesDeHoje.Sum(m => m.QuantidadeProcessada)} - " +
                            $"Orgs: - " +
                            string.Join(" - ", lotesDeHoje
                                .GroupBy(m => m.OrgId)
                                .Select(g => $"- OrgId: {g.Key}, Quantidade de Lotes: {g.Count()}"));
        }

        public async Task<string> ResumoLotesLinkedins()
        {
            var lotesDeHoje = await BuscarLotesLinkedinsDeHoje();
            return ConstruirLog(lotesDeHoje);
        }

        private async Task<IEnumerable<ProcessamentoCurriculoLoteDTO>> BuscarLotesLinkedinsDeHoje()
        {
            return await _processamentoCurriculoLoteRepository.BuscarLotesDiariosPorTipo(DateTime.UtcNow.Date, IdentificadorFilaLote.FILA_CURRICULO_LINKEDIN.ToString());
        }

        public void RegistrarOuAtualizarProcessamento(string idLote, string messageReceiptHandle, string mensagemErro, string stackTrace, string bodyMensagem)
        {
            if (!_processamentoCurriculoLoteRepository.ExisteRegistro(idLote, messageReceiptHandle))
                _processamentoCurriculoLoteRepository.CadastrarTabelaAuxiliar(messageReceiptHandle, idLote);

            _processamentoCurriculoLoteRepository.AtualizarProcessamento(idLote, messageReceiptHandle, mensagemErro, stackTrace, bodyMensagem);
        }
    }
}
