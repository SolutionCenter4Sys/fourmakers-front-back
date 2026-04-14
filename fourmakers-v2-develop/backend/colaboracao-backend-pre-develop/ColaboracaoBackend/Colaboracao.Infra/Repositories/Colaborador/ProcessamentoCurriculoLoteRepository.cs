
using ApiClient.Domain;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Infra.Context;
using Core.Domain.Colaborador;
using Dapper;
using DataTransferObject.Domain.BancoDeTalentos;
using DataTransferObject.Domain.Colaborador;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Colaboracao.Infra.Repositories.Colaborador
{
    public class ProcessamentoCurriculoLoteRepository : IProcessamentoCurriculoLoteRepository
    {
        private readonly IDBConnection _dapperConnection;

        public ProcessamentoCurriculoLoteRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public void AtualizarQuantidadeDeArquivosAProcessar(Guid idLote, int arquivosAProcessar)
        {
            var sql = @"
                        UPDATE tb_processamento_curriculo_lote
                        SET quantidade_a_processar = @ArquivosAProcessar
                        WHERE id = @Id;";

            _dapperConnection.GetConnection().Execute(sql, new
            {
                Id = idLote.ToString(),
                ArquivosAProcessar = arquivosAProcessar
            });
        }

        public async Task<IEnumerable<ProcessamentoCurriculoLoteDTO>> BuscarLotesDiariosPorTipo(DateTime utcNow, string identificador_fila)
        {
            var sql = @"
                                SELECT 
                                    id,
                                    tb_org_id AS OrgId,
                                    data_criacao AS DataCriacao,
                                    data_inicio_processamento AS DataInicioProcessamento,
                                    data_fim_processamento AS DataFimProcessamento,
                                    processado AS Processado,
                                    total_itens AS TotalItens,
                                    quantidade_a_processar AS QuantidadeAProcessar,
                                    quantidade_processada AS QuantidadeProcessada,
                                    tb_colaborador_codigo_interno_colaborador_cadastrante AS ColaboradorCadastrante,
                                    identificador_fila AS Identificadorfila
                                FROM 
                                    tb_processamento_curriculo_lote
                                WHERE 
                                    data_criacao > @utcNow
                                    AND identificador_fila = @Identificadorfila;
                            ";

            return await _dapperConnection.GetConnection().QueryAsync<ProcessamentoCurriculoLoteDTO>(sql, new { utcNow, Identificadorfila = identificador_fila });
        }

        public async Task<IEnumerable<ProcessamentoCurriculoLoteDTO>> BuscarMeusLotes(string codInternoColaborador, int orgId)
        {
            var sql = @"
                                SELECT 
                                    id,
                                    tb_org_id AS OrgId,
                                    data_criacao AS DataCriacao,
                                    data_inicio_processamento AS DataInicioProcessamento,
                                    data_fim_processamento AS DataFimProcessamento,
                                    processado AS Processado,
                                    total_itens AS TotalItens,
                                    quantidade_a_processar AS QuantidadeAProcessar,
                                    quantidade_processada AS QuantidadeProcessada,
                                    tb_colaborador_codigo_interno_colaborador_cadastrante AS ColaboradorCadastrante,
                                    identificador_fila AS IdentificadorFila
                                FROM tb_processamento_curriculo_lote
                                WHERE tb_colaborador_codigo_interno_colaborador_cadastrante = @codInternoColaborador
                                    AND tb_org_id = @orgId;
                            ";

            return await _dapperConnection.GetConnection().QueryAsync<ProcessamentoCurriculoLoteDTO>(sql, new { codInternoColaborador, orgId });
        }

        public async Task<IEnumerable<BuscarPessoasQueEuCadastrei>> BuscarPessoasCadastradasDesteLote(string idLote)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                    SELECT 
                        tc.nome_completo as Nome,
                        tc.codigo_interno_colaborador as CodigoInternoColaborador,
                        tc.data_criacao as DataDoCadastro
                    FROM 
                        tb_banco_talentos_processamento_lote tbtpl
                    INNER JOIN
                        tb_colaborador tc ON tc.codigo_interno_colaborador = tbtpl.tb_banco_talentos_codigo_interno_colaborador
                    WHERE 
                        tbtpl.tb_processamento_curriculo_lote_id = @IdLote;
                ";
            var parametros = new
            {
                IdLote = idLote
            };

            var result = await connection.QueryAsync<BuscarPessoasQueEuCadastrei>(query, parametros);

            return result;
        }

        public ProcessamentoCurriculoLoteDTO BuscarPorId(string idLote)
        {
            var sql = @"
                                SELECT 
                                    id,
                                    tb_org_id AS OrgId,
                                    data_criacao AS DataCriacao,
                                    data_inicio_processamento AS DataInicioProcessamento,
                                    data_fim_processamento AS DataFimProcessamento,
                                    processado AS Processado,
                                    total_itens AS TotalItens,
                                    quantidade_a_processar AS QuantidadeAProcessar,
                                    quantidade_processada AS QuantidadeProcessada,
                                    tb_colaborador_codigo_interno_colaborador_cadastrante AS ColaboradorCadastrante
                                FROM tb_processamento_curriculo_lote
                                WHERE id = @IdLote;
                            ";

            return _dapperConnection.GetConnection().QueryFirstOrDefault<ProcessamentoCurriculoLoteDTO>(sql, new { IdLote = idLote });
        }


        public void Cadastrar(Guid idLote, int orgId, DateTime utcNow, int totalArquivos, string codColaborador, string identificadorFila)
        {
            var sql = @"
                        INSERT INTO tb_processamento_curriculo_lote 
                        (id, tb_org_id, data_criacao, processado, total_itens, tb_colaborador_codigo_interno_colaborador_cadastrante, identificador_fila)
                        VALUES (@Id, @OrgId, @DataCriacao, @Processado, @TotalItens, @CodColaborador, @IdentificadorFila);";

            _dapperConnection.GetConnection().Execute(sql, new
            {
                Id = idLote.ToString(),
                OrgId = orgId,
                DataCriacao = utcNow,
                Processado = false,
                TotalItens = totalArquivos,
                CodColaborador = codColaborador,
                IdentificadorFila = identificadorFila
            });
        }

        public void CadastrarTabelaAuxiliar(string messageReceiptHandle, string idLote, string codColaborador = null, string nomeArquivoCv = null)
        {
            var sql = @"
                        INSERT INTO tb_banco_talentos_processamento_lote 
                        (tb_processamento_curriculo_lote_id, tb_banco_talentos_codigo_interno_colaborador, message_receipt_handle, nome_arquivo_cv)
                        VALUES (@Id, @CodColaborador, @MessageReceiptHandle, @NomeArquivoCv);";

            _dapperConnection.GetConnection().Execute(sql, new
            {
                Id = idLote.ToString(),
                CodColaborador = codColaborador,
                MessageReceiptHandle = messageReceiptHandle,
                NomeArquivoCv = nomeArquivoCv
            });
        }

        public bool ExisteRegistro(string idLote, string messageReceiptHandle)
        {
            var sql = @"
                        SELECT COUNT(*) FROM tb_banco_talentos_processamento_lote 
                        WHERE tb_processamento_curriculo_lote_id = @IdLote 
                          AND message_receipt_handle = @MessageReceiptHandle;";

            var count = _dapperConnection.GetConnection().ExecuteScalar<int>(sql, new
            {
                IdLote = idLote,
                MessageReceiptHandle = messageReceiptHandle
            });

            return count > 0;
        }

        public void AtualizarProcessamento(string idLote, string messageReceiptHandle, string mensagemErro, string stackTrace, string bodyMensagem)
        {
            var sqlUpdate = @"
                        UPDATE tb_banco_talentos_processamento_lote 
                        SET mensagem_erro = @MensagemErro,
                            stack_trace = @StackTrace,
                            data_ocorrencia_erro = @DataOcorrenciaErro,
                            body_mensagem = @BodyMensagem,
                            processado_com_erro = @ProcessadoComErro
                        WHERE tb_processamento_curriculo_lote_id = @IdLote 
                          AND message_receipt_handle = @MessageReceiptHandle;";

            _dapperConnection.GetConnection().Execute(sqlUpdate, new
            {
                IdLote = idLote,
                MessageReceiptHandle = messageReceiptHandle,
                MensagemErro = mensagemErro,
                StackTrace = stackTrace,
                DataOcorrenciaErro = DateTime.UtcNow,
                BodyMensagem = bodyMensagem,
                ProcessadoComErro = true
            });
        }

        public void IncrementarQuantidadeProcessada(string idLote, int quantidadeProcessadaAtualizada)
        {
            var sql = @"
                        UPDATE tb_processamento_curriculo_lote
                        SET quantidade_processada = @QuantidadeProcessadaAtualizada
                        WHERE id = @Id;";

            _dapperConnection.GetConnection().Execute(sql, new
            {
                Id = idLote.ToString(),
                QuantidadeProcessadaAtualizada = quantidadeProcessadaAtualizada
            });
        }

        public void IniciaProcessamento(string idLote)
        {
            var sql = @"
                        UPDATE tb_processamento_curriculo_lote
                        SET data_inicio_processamento = @DataInicioProcessamento
                        WHERE id = @Id;";

            _dapperConnection.GetConnection().Execute(sql, new
            {
                Id = idLote.ToString(),
                DataInicioProcessamento = DateTime.UtcNow
            });
        }

        public void MarcarComoProcessado(string idLote)
        {
            var sql = @"
                        UPDATE tb_processamento_curriculo_lote
                        SET processado = @Processado,
                            data_fim_processamento = NOW()
                        WHERE id = @Id;";

            _dapperConnection.GetConnection().Execute(sql, new
            {
                Id = idLote.ToString(),
                Processado = true
            });
        }

        public async Task<IEnumerable<ErroProcessamentoCurriculoDTO>> BuscarErrosPorLote(string idLote)
        {
            var sql = @"
                        SELECT 
                            tb_banco_talentos_codigo_interno_colaborador AS CodigoInternoColaborador,
                            tb_processamento_curriculo_lote_id AS IdLote,
                            message_receipt_handle AS MessageReceiptHandle,
                            mensagem_erro AS MensagemErro,
                            data_ocorrencia_erro AS DataOcorrenciaErro,
                            body_mensagem AS BodyMensagem,
                            processado_com_erro AS ProcessadoComErro
                        FROM 
                            tb_banco_talentos_processamento_lote
                        WHERE 
                            tb_processamento_curriculo_lote_id = @IdLote
                            AND processado_com_erro = TRUE
                        ORDER BY 
                            data_ocorrencia_erro DESC;";

            return await _dapperConnection.GetConnection().QueryAsync<ErroProcessamentoCurriculoDTO>(sql, new { IdLote = idLote });
        }

        public async Task<IEnumerable<ErroProcessamentoCurriculoDTO>> BuscarErrosPorLoteInformacoesBasicas(string idLote)
        {
            var sql = @"
                        SELECT 
                            tb_banco_talentos_codigo_interno_colaborador AS CodigoInternoColaborador,
                            tb_processamento_curriculo_lote_id AS IdLote,
                            mensagem_erro AS MensagemErro,
                            data_ocorrencia_erro AS DataOcorrenciaErro,
                            processado_com_erro AS ProcessadoComErro,
                            nome_arquivo_cv AS NomeArquivoCv
                        FROM 
                            tb_banco_talentos_processamento_lote
                        WHERE 
                            tb_processamento_curriculo_lote_id = @IdLote
                            AND processado_com_erro = TRUE
                        ORDER BY 
                            data_ocorrencia_erro DESC;";

            return await _dapperConnection.GetConnection().QueryAsync<ErroProcessamentoCurriculoDTO>(sql, new { IdLote = idLote });
        }

        public async Task<IEnumerable<ErroProcessamentoCurriculoDTO>> BuscarErrosPorMessageReceiptHandle(string messageReceiptHandle)
        {
            var sql = @"
                        SELECT 
                            tb_banco_talentos_codigo_interno_colaborador AS CodigoInternoColaborador,
                            tb_processamento_curriculo_lote_id AS IdLote,
                            message_receipt_handle AS MessageReceiptHandle,
                            mensagem_erro AS MensagemErro,
                            stack_trace AS StackTrace,
                            data_ocorrencia_erro AS DataOcorrenciaErro,
                            body_mensagem AS BodyMensagem,
                            processado_com_erro AS ProcessadoComErro
                        FROM 
                            tb_banco_talentos_processamento_lote
                        WHERE 
                            message_receipt_handle = @MessageReceiptHandle
                            AND processado_com_erro = TRUE
                        ORDER BY 
                            data_ocorrencia_erro DESC;";

            return await _dapperConnection.GetConnection().QueryAsync<ErroProcessamentoCurriculoDTO>(sql, new { MessageReceiptHandle = messageReceiptHandle });
        }

        public async Task<IEnumerable<ErroProcessamentoCurriculoDTO>> BuscarErrosPorPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            var sql = @"
                        SELECT 
                            tb_banco_talentos_codigo_interno_colaborador AS CodigoInternoColaborador,
                            tb_processamento_curriculo_lote_id AS IdLote,
                            message_receipt_handle AS MessageReceiptHandle,
                            mensagem_erro AS MensagemErro,
                            stack_trace AS StackTrace,
                            data_ocorrencia_erro AS DataOcorrenciaErro,
                            body_mensagem AS BodyMensagem,
                            processado_com_erro AS ProcessadoComErro
                        FROM 
                            tb_banco_talentos_processamento_lote
                        WHERE 
                            data_ocorrencia_erro BETWEEN @DataInicio AND @DataFim
                            AND processado_com_erro = TRUE
                        ORDER BY 
                            data_ocorrencia_erro DESC;";

            return await _dapperConnection.GetConnection().QueryAsync<ErroProcessamentoCurriculoDTO>(sql, new { DataInicio = dataInicio, DataFim = dataFim });
        }

        public async Task<IEnumerable<ErroProcessamentoCurriculoDTO>> BuscarTodosErros()
        {
            var sql = @"
                        SELECT 
                            tb_banco_talentos_codigo_interno_colaborador AS CodigoInternoColaborador,
                            tb_processamento_curriculo_lote_id AS IdLote,
                            message_receipt_handle AS MessageReceiptHandle,
                            mensagem_erro AS MensagemErro,
                            stack_trace AS StackTrace,
                            data_ocorrencia_erro AS DataOcorrenciaErro,
                            body_mensagem AS BodyMensagem,
                            processado_com_erro AS ProcessadoComErro
                        FROM 
                            tb_banco_talentos_processamento_lote
                        WHERE 
                            processado_com_erro = TRUE
                        ORDER BY 
                            data_ocorrencia_erro DESC;";

            return await _dapperConnection.GetConnection().QueryAsync<ErroProcessamentoCurriculoDTO>(sql);
        }

        public async void InserirCodColaboradorNaTabelaAuxiliar(string messageReceiptHandle, string idLote, string codColaborador)
        {
            var sql = @"
                        UPDATE tb_banco_talentos_processamento_lote
                        SET tb_banco_talentos_codigo_interno_colaborador = @codColaborador
                        WHERE tb_processamento_curriculo_lote_id = @idLote AND message_receipt_handle = @messageReceiptHandle;";

            _dapperConnection.GetConnection().Execute(sql, new
            {
                codColaborador,
                messageReceiptHandle,
                idLote
            });
        }

        public async Task<IEnumerable<string>> BuscarNomesArquivos(string idLote)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                    SELECT 
                        tbtpl.nome_arquivo_cv
                    FROM 
                        tb_banco_talentos_processamento_lote tbtpl
                    WHERE 
                        tbtpl.tb_processamento_curriculo_lote_id = @IdLote;
                ";
            var parametros = new
            {
                IdLote = idLote
            };

            var result = await connection.QueryAsync<string>(query, parametros);

            return result.Distinct();
        }
    }
}