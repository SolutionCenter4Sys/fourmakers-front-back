using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.Financeiro.IntegracaoBancaria;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RetornoBancaria;
using DataTransferObject.Domain.Financeiro.Reembolso.ControleDeSaldo;
using Financeiro.Domain.Interfaces.IntegracaoBancaria.RetornoBancaria;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain.Financeiro.NotaFiscal;
using DataTransferObject.Domain.Financeiro.NotaFiscal;

namespace Financeiro.Domain.Impl.IntegracaoBancaria.RetornoBancaria
{
    public class RetornoBancariaService : IRetornoBancariaService
    {
        public readonly static string DESCRICAO_ENTIDADE = "Retorno Bancária";

        private readonly ICnabRetornoRepository _cnabRetornoRepository;
        private readonly IPagamentoCnabRepository _pagamentoCnabRepository;
        private readonly IUploadFilesClient _uploadFilesClient;
        private readonly IDBConnectionUnitOfWork _dbConnectionUnitOfWork;
        private readonly INotaFiscalRepository _notaFiscalRepository;
        private readonly ICnabRemessaRepository  _cnabRemessaRepository;

        public RetornoBancariaService(
            ICnabRetornoRepository cnabRetornoRepository,
            IPagamentoCnabRepository pagamentoCnabRepository,
            IUploadFilesClient uploadFilesClient,
            IDBConnectionUnitOfWork dbConnectionUnitOfWork, 
            INotaFiscalRepository notaFiscalRepository, 
            ICnabRemessaRepository cnabRemessaRepository)
        {
            _cnabRetornoRepository = cnabRetornoRepository;
            _pagamentoCnabRepository = pagamentoCnabRepository;
            _uploadFilesClient = uploadFilesClient;
            _dbConnectionUnitOfWork = dbConnectionUnitOfWork;
            _notaFiscalRepository = notaFiscalRepository;
            _cnabRemessaRepository = cnabRemessaRepository;
        }

        public async Task<ApiGenericResult<ProcessarRetornoCnabResult>> ProcessarRetornoCnab(
            string hashRemessa,
            string nomeArquivo,
            string conteudoArquivo,
            string cpfUsuario,
            string tipoRemessa,
            int orgId)
        {
            var apiGenericResult = new ApiGenericResult<ProcessarRetornoCnabResult>();

            try
            {
                _dbConnectionUnitOfWork.BeginTransaction();

                // 0. Validar documento CNAB antes de processar
                ParseCnab240RetornoService.ValidarDocumento(conteudoArquivo);

                // 1. Buscar remessa pelo hash
                var remessaId = await _cnabRetornoRepository.BuscarRemessaIdPorHash(hashRemessa, orgId);
                if (remessaId == Guid.Empty)
                {
                    throw new ApplicationException($"Remessa não encontrada com o hash: {hashRemessa}");
                }

                // 1.1. Buscar todos os pagamentos da remessa ordenados
                var pagamentosOrdenados = await _cnabRetornoRepository.BuscarPagamentosCnabOrdenadosPorRemessa(remessaId);

                // 2. Criar registro de retorno
                var retornoId = await _cnabRetornoRepository.CriarRetorno(
                    remessaId,
                    hashRemessa,
                    nomeArquivo,
                    cpfUsuario);

                // 3. Parsear arquivo CNAB 240 e extrair ocorrências
                var parser = new ParseCnab240RetornoService();
                var segmentos = parser.ParsearArquivoRetorno(conteudoArquivo);

                // 4. Processar e persistir linhas do arquivo com dados parseados
                var totalLinhas = 0;
                foreach (var segmento in segmentos)
                {
                    await _cnabRetornoRepository.CriarRetornoItem(
                        retornoId,
                        segmento.LinhaCompleta,
                        totalLinhas + 1,
                        segmento.CodigoOcorrencia ?? "",
                        segmento.TipoRegistroDescritivo ?? "");
                    totalLinhas++;
                }

                // 5. Obter apenas segmentos A para processamento de pagamentos
                var segmentosA = parser.ObterSegmentosA(segmentos);

                // 5.1. Validar se a quantidade de pagamentos está correta com os segmentos
                if (pagamentosOrdenados.Count != segmentosA.Count)
                {
                    throw new ApplicationException(
                        $"Quantidade de pagamentos ({pagamentosOrdenados.Count}) não corresponde à quantidade de segmentos A no arquivo de retorno ({segmentosA.Count}). " +
                        $"Remessa ID: {remessaId}");
                }

                // 5.2. Buscar segmentos A da remessa para validação
                var segmentosARemessa = await _cnabRemessaRepository.BuscarRemessaItemSegmentoAPorRemessaId(remessaId);

                // 5.3. Validar se a quantidade de segmentos A da remessa corresponde à quantidade do retorno
                if (segmentosARemessa.Count != segmentosA.Count)
                {
                    throw new ApplicationException(
                        $"O documento de retorno não corresponde à remessa informada. " +
                        $"Quantidade de segmentos A divergente: Remessa={segmentosARemessa.Count}, Retorno={segmentosA.Count}. " +
                        $"Remessa ID: {remessaId}");
                }

                // 5.4. Comparar cada segmento A da remessa com o correspondente do retorno
                for (int i = 0; i < segmentosA.Count; i++)
                {
                    var segmentoRetorno = segmentosA[i];
                    var linhaRemessa = segmentosARemessa[i];
                    
                    // Extrair campos chave do retorno
                    var camposRetorno = ExtrairCamposChaveSegmentoA(segmentoRetorno.LinhaCompleta);
                    
                    // Extrair campos chave da remessa
                    var camposRemessa = ExtrairCamposChaveSegmentoA(linhaRemessa);
                    
                    // Comparar campos chave
                    var divergencias = new List<string>();
                    
                    if (camposRetorno.numeroSequencial != camposRemessa.numeroSequencial)
                    {
                        divergencias.Add($"Número sequencial: Retorno={camposRetorno.numeroSequencial}, Remessa={camposRemessa.numeroSequencial}");
                    }
                    
                    if (camposRetorno.nomeFavorecido != camposRemessa.nomeFavorecido)
                    {
                        divergencias.Add($"Nome favorecido: Retorno='{camposRetorno.nomeFavorecido}', Remessa='{camposRemessa.nomeFavorecido}'");
                    }
                    
                    if (camposRetorno.valorPagamento != camposRemessa.valorPagamento)
                    {
                        divergencias.Add($"Valor pagamento: Retorno={camposRetorno.valorPagamento}, Remessa={camposRemessa.valorPagamento}");
                    }
                    
                    if (camposRetorno.cpfCnpj != camposRemessa.cpfCnpj)
                    {
                        divergencias.Add($"CPF/CNPJ: Retorno={camposRetorno.cpfCnpj}, Remessa={camposRemessa.cpfCnpj}");
                    }
                    
                    if (divergencias.Any())
                    {
                        throw new ApplicationException(
                            $"O documento de retorno não corresponde à remessa informada. " +
                            $"Divergência encontrada no segmento A índice {i} (posição {i + 1}): {string.Join("; ", divergencias)}. " +
                            $"Remessa ID: {remessaId}");
                    }
                }
                
                // 6. Fazer upload do arquivo para S3
                var urlArquivo = await FazerUploadArquivoCnab(conteudoArquivo, nomeArquivo);

                // 7. Atualizar URL no banco
                await _cnabRetornoRepository.AtualizarNomeArquivoRetorno(retornoId, urlArquivo);
                
                // 8. Separar segmentos A em sucesso e erro
                var segmentosComSucesso = new List<(ParseCnab240RetornoService.SegmentoRetorno Segmento, int Indice)>();
                var segmentosComErro = new List<(ParseCnab240RetornoService.SegmentoRetorno Segmento, int Indice, string DescricaoErro)>();

                for (int i = 0; i < segmentosA.Count; i++)
                {
                    var segmentoA = segmentosA[i];
                    var ocorrenciaInfo = ParseCnab240RetornoService.ObterOcorrencia(segmentoA.CodigoOcorrencia ?? "");

                    if (ParseCnab240RetornoService.EhSucesso(segmentoA.CodigoOcorrencia ?? ""))
                    {
                        segmentosComSucesso.Add((segmentoA, i));
                    }
                    else
                    {
                        segmentosComErro.Add((segmentoA, i, ocorrenciaInfo.Descricao));
                    }
                }
                
                // 9. Atualizar pagamentos com sucesso para PAGO
                var pagamentosAtualizados = 0;
                foreach (var (segmentoA, indice) in segmentosComSucesso)
                {
                    if (indice < pagamentosOrdenados.Count)
                    {
                        var pagamento = pagamentosOrdenados[indice];
                        await _pagamentoCnabRepository.AtualizarStatusPagamentoCnab(
                            pagamento.Id,
                            "PAGO",
                            segmentoA.CodigoOcorrencia);
                        pagamentosAtualizados++;
                    }
                }

                // 9.1. Atualizar pagamentos com erro para REJEITADO
                var pagamentosComErro = 0;
                foreach (var (segmentoA, indice, descricaoErro) in segmentosComErro)
                {
                    if (indice < pagamentosOrdenados.Count)
                    {
                        var pagamento = pagamentosOrdenados[indice];
                        await _pagamentoCnabRepository.AtualizarStatusPagamentoCnab(
                            pagamento.Id,
                            "REJEITADO",
                            segmentoA.CodigoOcorrencia,
                            descricaoErro);
                        pagamentosComErro++;
                    }
                }
                
                //10. Atualizar reembolso (SEPARADO PARA INCLUIR LOGICA NF FUTURAMENTE => TIPOREMESSA.NF TIPOREMESSA.REEMBOLSO)
                int pagos = 0;
                int rejeitados = 0;
                int total = 0;

                if (tipoRemessa == "NF")
                {
                    (pagos, rejeitados, total) = await AtualizarPagamentosNF(remessaId);
                }

                if (tipoRemessa == "REEMBOLSO")
                {
                    (pagos, rejeitados, total) = await AtualizarPagamentosReembolso(remessaId);
                }

                // 11. Atualizar status da remessa para FINALIZADO
                await _cnabRetornoRepository.AtualizarStatusRemessa(remessaId, "FINALIZADO");

                // 12. Commit da transação
                _dbConnectionUnitOfWork.Commit();

                var getAlias = tipoRemessa == "REEMBOLSO" ? "reembolso" : "notas fiscais";
                apiGenericResult.Retorno = new ProcessarRetornoCnabResult
                {
                    RetornoId = retornoId,
                    TotalLinhasProcessadas = totalLinhas,
                    PagamentosAtualizados = pagamentosAtualizados,
                    Mensagem = $"Retorno processado com sucesso. {total} pagamentos atualizados, {rejeitados} {getAlias} marcados como REJEITADOS e {pagos} {getAlias} marcados como PAGO."
                };
            }
            catch (Exception ex)
            {
                // 14. Rollback da transação
                _dbConnectionUnitOfWork.Rollback();

                ExceptionUtil.GerenciarRetornoExcecao(
                    new ApplicationException($"Erro ao processar retorno CNAB. Detalhe erro: {ex.Message}"),
                    CRUDEnum.Create,
                    DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        private async Task<(int pagos, int rejeitados, int total)> AtualizarPagamentosReembolso(Guid remessaId)
        {
            
            // 10. Buscar e atualizar solicitações de pagamento para PAGO
            var solicitacoesPagosIds = await _cnabRetornoRepository.BuscarSolicitacoesPagamentoPorRemessa(remessaId, "PAGO");
            foreach (var solicitacaoId in solicitacoesPagosIds)
            {
                await _pagamentoCnabRepository.AtualizarStatusSolicitacaoPagamento(
                    solicitacaoId,
                    StatusSolicitacaoPagamentoEnum.PAGO);
            }

            // 10.1. Buscar e atualizar solicitações de pagamento rejeitadas
            var solicitacoesRejeitadosIds = await _cnabRetornoRepository.BuscarSolicitacoesPagamentoPorRemessa(remessaId, "REJEITADO");
            foreach (var solicitacaoId in solicitacoesRejeitadosIds)
            {
                await _pagamentoCnabRepository.AtualizarStatusSolicitacaoPagamento(
                    solicitacaoId,
                    StatusSolicitacaoPagamentoEnum.AGUARDANDO_PAGAMENTO);
            }

            // 11. Buscar e atualizar reembolsos para PAGO (status_id = 4)
            var reembolsosIds = await _cnabRetornoRepository.BuscarReembolsosPorRemessa(remessaId);
            foreach (var reembolsoId in reembolsosIds)
            {
                await _cnabRetornoRepository.AtualizarStatusReembolso(reembolsoId, 4);
            }

            return
            (
                pagos: solicitacoesPagosIds.Count(),
                rejeitados: solicitacoesRejeitadosIds.Count(),
                total: reembolsosIds.Count()
            );
        }
        
        private async Task<(int pagos, int rejeitados, int total)> AtualizarPagamentosNF(Guid remessaId)
        {
            
            // 10. Buscar e atualizar solicitações de pagamento para PAGO
            var solicitacoesPagosIds = await _cnabRetornoRepository.BuscarSolicitacoesPagamentoNFPorRemessa(remessaId, "PAGO");

            if (solicitacoesPagosIds.Any())
            {
                await _notaFiscalRepository.MudarStatusNotaFiscalPorListaDeIdsAsync(solicitacoesPagosIds, NotaFiscalStatusEnum.NF_PAGA, null, null);
            }

            // 10.1. Buscar e atualizar solicitações de pagamento rejeitadas
            var solicitacoesRejeitadosIds = await _cnabRetornoRepository.BuscarSolicitacoesPagamentoNFPorRemessa(remessaId, "REJEITADO");
            
            if (solicitacoesRejeitadosIds.Any())
            {
                await _notaFiscalRepository.MudarStatusNotaFiscalPorListaDeIdsAsync(solicitacoesRejeitadosIds, NotaFiscalStatusEnum.NF_APROVADA, null, null);
            }

            return
            (
                pagos: solicitacoesPagosIds.Count(),
                rejeitados: solicitacoesRejeitadosIds.Count(),
                total: solicitacoesPagosIds.Count + solicitacoesRejeitadosIds.Count()
            );
        }

        private (int numeroSequencial, string nomeFavorecido, string valorPagamento, string cpfCnpj) ExtrairCamposChaveSegmentoA(string linhaCnab)
        {
            if (string.IsNullOrWhiteSpace(linhaCnab) || linhaCnab.Length < 240)
            {
                return (0, "", "", "");
            }

            // Número sequencial (posições 8-12, índice 7-11)
            var numeroSequencial = int.TryParse(linhaCnab.Substring(7, 5).Trim(), out var seq) ? seq : 0;
            
            // Nome do favorecido (posições 44-73, índice 43-72)
            var nomeFavorecido = linhaCnab.Length > 72 ? linhaCnab.Substring(43, 30).Trim() : "";
            
            // Valor do pagamento (posições 120-134, índice 119-133)
            var valorPagamento = linhaCnab.Length > 133 ? linhaCnab.Substring(119, 15).Trim() : "";
            
            // CPF/CNPJ do favorecido (posições 204-217, índice 203-216)
            var cpfCnpj = linhaCnab.Length > 216 ? linhaCnab.Substring(203, 14).Trim() : "";

            return (numeroSequencial, nomeFavorecido, valorPagamento, cpfCnpj);
        }

        private async Task<string> FazerUploadArquivoCnab(string conteudoArquivo, string nomeArquivo)
        {
            var bytesArquivo = Encoding.UTF8.GetBytes(conteudoArquivo);

            var pathArquivoS3 = $"arquivos/cnab/retorno/{nomeArquivo}";
            await _uploadFilesClient.UploadFile(pathArquivoS3, bytesArquivo);

            return $"{VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA)}{pathArquivoS3}";
        }
    }
}
