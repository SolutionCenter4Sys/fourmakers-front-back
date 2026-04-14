using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Context;
using Core.Domain.Financeiro.Conciliacao;
using Dapper;
using System;
using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.Conciliacao;
using System.Collections.Generic;
using DataTransferObject.Domain.Apontamento.FolhaPonto;
using Colaboracao.Helper;
using ApiClient.Domain;

namespace Colaboracao.Infra.Repositories.Financeiro.Conciliacao
{
    public class ConciliacaoFolhaPontoRepository : IConciliacaoFolhaPontoRepository
    {
        private readonly IDBConnection _dapperConnection;

        public ConciliacaoFolhaPontoRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<bool> CriarConciliacaoFolhaPontoAsync(ConciliacaoFolhaPontoDTO conciliacaoFolhaPonto)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string inserirConciliacaoFolhaPontoSql = @"
                    INSERT INTO tb_conciliacao_folhaponto (
                        id,
                        competencia,
                        cnpj,
                        tb_lote_id,
                        ativo
                    ) VALUES (
                        @Id,
                        @Competencia,
                        @Cnpj,
                        @TbLoteId,
                        1
                    )";

                var linhasInseridas = await connection.ExecuteAsync(inserirConciliacaoFolhaPontoSql, conciliacaoFolhaPonto);

                return linhasInseridas > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CriarConciliacaoFolhaPontoColaboradorAsync(ConciliacaoFolhaPontoColaboradorDTO conciliacaoFolhaPontoColaborador)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string inserirConciliacaoFolhaPontoColaboradorSql = @"
                    INSERT INTO tb_conciliacao_folhaponto_colaborador (
                        id,
                        houve_divergencia,
                        numero_divergencias,
                        tb_item_lote_id,
                        tb_conciliacao_folhaponto_id,
                        codigo_interno_colaborador
                    ) VALUES (
                        @Id,
                        @HouveDivergencia,
                        @NumeroDivergencias,
                        @TbItemLoteId,
                        @TbConciliacaoFolhaPontoId,
                        @CodigoInternoColaborador
                    )";

                var linhasInseridas = await connection.ExecuteAsync(inserirConciliacaoFolhaPontoColaboradorSql, conciliacaoFolhaPontoColaborador);

                return linhasInseridas > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ItemConciliacaoColaboradorDTO>> BuscarItensConciliacaoColaboradorAsync(string loteId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string buscarItensConciliacaoColaboradorSql = @"
                    SELECT 
                        tcfc.id AS Id,
                        tcfc.houve_divergencia AS HouveDivergencia,
                        tcfc.numero_divergencias AS NumeroDivergencias,
                        tcfc.tb_item_lote_id AS TbItemLoteId,
                        tcfc.tb_conciliacao_folhaponto_id AS TbConciliacaoFolhaPontoId,
                        tcfc.codigo_interno_colaborador AS CodigoInternoColaborador,
                        tc.nome_completo AS NomeColaborador,
                        tco.cargo AS Cargo,
                        (select aux.file_path from tb_item_lote aux where aux.id = thc.tb_item_lote_id) AS HoleritePath,
                        (select aux.file_path from tb_item_lote aux where aux.id = tfc.tb_item_lote_id) AS FolhaPontoPath
                    FROM tb_conciliacao_folhaponto_colaborador tcfc
                    INNER JOIN tb_conciliacao_folhaponto tcf ON tcfc.tb_conciliacao_folhaponto_id = tcf.id
                    INNER JOIN tb_item_lote il ON tcfc.tb_item_lote_id = il.id
                    INNER JOIN tb_colaborador tc on tcfc.codigo_interno_colaborador = tc.codigo_interno_colaborador
                    LEFT JOIN tb_colaborador_org tco on tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                    LEFT JOIN tb_holerite_colaborador thc on tcfc.codigo_interno_colaborador = thc.codigo_interno_colaborador and thc.competencia = tcf.competencia and thc.adiantamento = 0 and thc.ferias = 0 and thc.decimo_terceiro = 0 and thc.decimo_terceiro_adiantamento = 0
                    LEFT JOIN tb_folhaponto_colaborador tfc on tcfc.codigo_interno_colaborador = tfc.codigo_interno_colaborador and tfc.competencia = tcf.competencia
                    WHERE il.tb_lote_id = @LoteId";    

                string buscarDivergenciasConciliacaoColaboradorSql = @"
                    SELECT 
                        tcfd.id AS Id,
                        tcfd.campo_divergencia AS CampoDivergencia,
                        tcfd.mensagem AS Mensagem,
                        tcfd.valor_esperado AS ValorEsperado,
                        tcfd.valor_contabilidade AS ValorContabilidade,
                        tcfd.status AS Status,
                        tcfd.regra AS Regra,
                        tcfd.formula AS Formula,
                        tcfd.passos AS Passos,
                        tcfd.variaveis AS Variaveis,
                        tcfd.eh_divergencia AS EhDivergencia
                    FROM tb_conciliacao_folhaponto_divergencia tcfd
                    WHERE tcfd.tb_conciliacao_folhaponto_colaborador_id = @Id";

                var itens = await connection.QueryAsync<dynamic>(buscarItensConciliacaoColaboradorSql, new { LoteId = loteId });
                var itensResult = new List<ItemConciliacaoColaboradorDTO>();
                foreach (var item in itens)
                {
                    var itemConciliacaoColaborador = new ItemConciliacaoColaboradorDTO
                    {
                        NomeColaborador = item.NomeColaborador,
                        CodigoInternoColaborador = item.CodigoInternoColaborador,
                        Cargo = item.Cargo,
                        HoleritePath = String.IsNullOrEmpty(item.HoleritePath) 
                            ? null : VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + item.HoleritePath,
                        FolhaPontoPath = String.IsNullOrEmpty(item.FolhaPontoPath) 
                            ? null : VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + item.FolhaPontoPath,
                        ResultadoConciliacao = new ConciliacaoFolhaPontoColaboradorDTO()
                        {
                            Id = item.Id,
                            HouveDivergencia = item.HouveDivergencia == 1 ? true : false,
                            NumeroDivergencias = item.NumeroDivergencias,
                            TbItemLoteId = item.TbItemLoteId,
                            TbConciliacaoFolhaPontoId = item.TbConciliacaoFolhaPontoId,
                            CodigoInternoColaborador = item.CodigoInternoColaborador
                        }
                    };

                    var divergenciasRaw = await connection.QueryAsync<dynamic>(buscarDivergenciasConciliacaoColaboradorSql, new { item.Id });
                    var divergencias = new List<ConciliacaoFolhaPontoDivergenciaDTO>();
                    
                    foreach (var divergenciaRaw in divergenciasRaw)
                    {
                        var divergencia = new ConciliacaoFolhaPontoDivergenciaDTO
                        {
                            Id = divergenciaRaw.Id,
                            CampoDivergencia = divergenciaRaw.CampoDivergencia,
                            Mensagem = divergenciaRaw.Mensagem,
                            ValorEsperado = divergenciaRaw.ValorEsperado,
                            ValorContabilidade = divergenciaRaw.ValorContabilidade,
                            Status = divergenciaRaw.Status,
                            Regra = divergenciaRaw.Regra,
                            Formula = divergenciaRaw.Formula,
                            Passos = !string.IsNullOrEmpty(divergenciaRaw.Passos) 
                                ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(divergenciaRaw.Passos) ?? new List<string>()
                                : new List<string>(),
                            Variaveis = !string.IsNullOrEmpty(divergenciaRaw.Variaveis) 
                                ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(divergenciaRaw.Variaveis) ?? new List<string>()
                                : new List<string>(),
                            EhDivergencia = divergenciaRaw.EhDivergencia
                        };
                        divergencias.Add(divergencia);
                    }
                    
                    itemConciliacaoColaborador.ResultadoConciliacao.Divergencias = divergencias;
                    
                    itensResult.Add(itemConciliacaoColaborador);
                }
                return itensResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CriarConciliacaoFolhaPontoDivergenciaAsync(ConciliacaoFolhaPontoDivergenciaDTO conciliacaoFolhaPontoDivergencia)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string inserirConciliacaoFolhaPontoDivergenciaSql = @"
                    INSERT INTO tb_conciliacao_folhaponto_divergencia (
                        id,
                        campo_divergencia,
                        mensagem,
                        valor_esperado,
                        valor_contabilidade,
                        status,
                        regra,
                        formula,
                        passos,
                        variaveis,
                        eh_divergencia,
                        tb_conciliacao_folhaponto_colaborador_id
                    ) VALUES (
                        @Id,
                        @CampoDivergencia,
                        @Mensagem,
                        @ValorEsperado,
                        @ValorContabilidade,
                        @Status,
                        @Regra,
                        @Formula,
                        @Passos,
                        @Variaveis,
                        @EhDivergencia,
                        @TbConciliacaoFolhaPontoColaboradorId
                    )";

                var parametros = new
                {
                    Id = conciliacaoFolhaPontoDivergencia.Id,
                    CampoDivergencia = conciliacaoFolhaPontoDivergencia.CampoDivergencia,
                    Mensagem = conciliacaoFolhaPontoDivergencia.Mensagem,
                    ValorEsperado = conciliacaoFolhaPontoDivergencia.ValorEsperado,
                    ValorContabilidade = conciliacaoFolhaPontoDivergencia.ValorContabilidade,
                    Status = conciliacaoFolhaPontoDivergencia.Status.ToString(),
                    Regra = conciliacaoFolhaPontoDivergencia.Regra,
                    Formula = conciliacaoFolhaPontoDivergencia.Formula,
                    Passos = conciliacaoFolhaPontoDivergencia.Passos != null && conciliacaoFolhaPontoDivergencia.Passos.Count > 0 
                        ? System.Text.Json.JsonSerializer.Serialize(conciliacaoFolhaPontoDivergencia.Passos) : null,
                    Variaveis = conciliacaoFolhaPontoDivergencia.Variaveis != null && conciliacaoFolhaPontoDivergencia.Variaveis.Count > 0 
                        ? System.Text.Json.JsonSerializer.Serialize(conciliacaoFolhaPontoDivergencia.Variaveis) : null,
                    EhDivergencia = conciliacaoFolhaPontoDivergencia.EhDivergencia ? 1 : 0,
                    TbConciliacaoFolhaPontoColaboradorId = conciliacaoFolhaPontoDivergencia.TbConciliacaoFolhaPontoColaboradorId
                };

                var linhasInseridas = await connection.ExecuteAsync(inserirConciliacaoFolhaPontoDivergenciaSql, parametros);

                return linhasInseridas > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ValidaCnpjProjetoOrgAsync(string cnpj, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string validarCnpjProjetoOrgSql = @"
                    SELECT COUNT(*) > 0 
                    FROM tb_projeto_org
                    WHERE cod_projeto = @Cnpj
                    AND tb_org_id = @OrgId";

                var resultado = await connection.QueryFirstOrDefaultAsync<bool>(validarCnpjProjetoOrgSql, new { Cnpj = cnpj, OrgId = orgId });

                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> ObterNomeEmpresaAsync(string cnpj, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string obterNomeEmpresaSql = @"
                    SELECT projeto
                    FROM tb_projeto_org
                    WHERE cod_projeto = @Cnpj
                    AND tb_org_id = @OrgId";

                var resultado = await connection.QueryFirstOrDefaultAsync<string>(obterNomeEmpresaSql, new { Cnpj = cnpj, OrgId = orgId });

                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> ObterModalidadePagamentoHorasExtrasAsync(string cnpj, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string obterModalidadePagamentoHorasExtrasSql = @"
                    SELECT modalidade_pagamento_horasextras
                    FROM tb_projeto_regime_he
                    WHERE cod_projeto = @Cnpj
                    AND tb_org_id = @OrgId";

                var resultado = await connection.QueryFirstOrDefaultAsync<string>(obterModalidadePagamentoHorasExtrasSql, new { Cnpj = cnpj, OrgId = orgId });

                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<LoteFilaConciliacaoDTO>> BuscarLotesPorOrgAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string buscarLotesSql = @"
                    SELECT 
                        l.id AS Id,
                        l.quantidade_paginas AS QuantidadeDePaginas,
                        l.file_path AS Pdf,
                        l.data_criacao AS DataCriacao,
                        l.data_finalizacao AS DataFinalizacao,
                        l.aprovado_para_processamento AS AprovadoParaProcessamento,
                        l.sumario AS SumarioConciliacaoString
                    FROM tb_lote l
                    INNER JOIN tb_fila f ON l.tb_fila_id = f.id
                    INNER JOIN tb_conciliacao_folhaponto tcf ON l.id = tcf.tb_lote_id
                    WHERE l.tb_org_id = @OrgId
                    AND f.descricao = @Fila
                    AND tcf.ativo = 1
                    ORDER BY l.data_criacao DESC";

                var lotes = await connection.QueryAsync<LoteFilaConciliacaoDTO>(
                    buscarLotesSql, 
                    new { OrgId = orgId, Fila = TipoFilaEnum.ANALISE_HOLERITE.ToString() }
                );

                return lotes.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> GetLoteContemDivergenciasAsync(string loteId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string buscarConciliacaoFolhaPontoPorLoteSql = @"
                    SELECT tcf.id, COUNT(tcfc.id) > 0 AS ContemDivergencias
                    FROM tb_conciliacao_folhaponto tcf
                    INNER JOIN tb_conciliacao_folhaponto_colaborador tcfc ON tcf.id = tcfc.tb_conciliacao_folhaponto_id
                    WHERE tcf.tb_lote_id = @LoteId
                    AND tcfc.houve_divergencia = 1
                    GROUP BY tcf.id";

                var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    buscarConciliacaoFolhaPontoPorLoteSql, 
                    new { LoteId = loteId }
                );

                return resultado?.ContemDivergencias == 1 ?? false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ConciliacaoFolhaPontoDTO> BuscarConciliacaoFolhaPontoPorLoteAsync(string itemLoteId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string buscarConciliacaoFolhaPontoPorLoteSql = @"
                    SELECT 
                        tcf.id AS Id,
                        tcf.competencia AS Competencia,
                        tcf.cnpj AS Cnpj,
                        tcf.tb_lote_id AS TbLoteId,
                        tcf.aprovado AS Aprovado,
                        tcf.data_aprovacao AS DataAprovacao
                    FROM tb_conciliacao_folhaponto tcf
                    INNER JOIN tb_lote il ON tcf.tb_lote_id = il.id
                    WHERE il.id = @ItemLoteId
                    GROUP BY tcf.id";

                var resultado = await connection.QueryFirstOrDefaultAsync<ConciliacaoFolhaPontoDTO>(
                    buscarConciliacaoFolhaPontoPorLoteSql, 
                    new { ItemLoteId = itemLoteId }
                );

                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ConciliacaoFolhaPontoDTO> BuscarConciliacaoFolhaPontoPorItemLoteAsync(string itemLoteId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string buscarConciliacaoFolhaPontoPorLoteSql = @"
                    SELECT 
                        tcf.id AS Id,
                        tcf.competencia AS Competencia,
                        tcf.cnpj AS Cnpj,
                        tcf.tb_lote_id AS TbLoteId,
                        tcf.aprovado AS Aprovado,
                        tcf.data_aprovacao AS DataAprovacao
                    FROM tb_conciliacao_folhaponto tcf
                    INNER JOIN tb_lote il ON tcf.tb_lote_id = il.id
                    INNER JOIN tb_item_lote il2 ON il.id = il2.tb_lote_id
                    WHERE il2.id = @ItemLoteId
                    GROUP BY tcf.id";

                var resultado = await connection.QueryFirstOrDefaultAsync<ConciliacaoFolhaPontoDTO>(
                    buscarConciliacaoFolhaPontoPorLoteSql, 
                    new { ItemLoteId = itemLoteId }
                );

                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AtualizarStatusAprovacaoAsync(int orgId, string loteId, bool aprovado)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string atualizarStatusAprovacaoSql = @"
                    UPDATE tb_conciliacao_folhaponto tcf
                    INNER JOIN tb_lote l ON tcf.tb_lote_id = l.id
                    SET tcf.aprovado = @Aprovado,
                        tcf.data_aprovacao = @DataAprovacao
                    WHERE l.tb_org_id = @OrgId
                    AND l.id = @LoteId";

                var dataAprovacao = aprovado ? DateTime.Now : (DateTime?)null;

                var linhasAtualizadas = await connection.ExecuteAsync(atualizarStatusAprovacaoSql, new 
                { 
                    OrgId = orgId, 
                    LoteId = loteId, 
                    Aprovado = aprovado ? 1 : 0,
                    DataAprovacao = dataAprovacao
                });

                return linhasAtualizadas > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> InativarConciliacoesPorCnpjCompetenciaAsync(int orgId, string cnpj, string competencia)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string inativarConciliacoesSql = @"
                    UPDATE tb_conciliacao_folhaponto tcf
                    INNER JOIN tb_lote l ON tcf.tb_lote_id = l.id
                    SET tcf.ativo = 0
                    WHERE l.tb_org_id = @OrgId
                    AND tcf.cnpj = @Cnpj
                    AND tcf.competencia = @Competencia";

                var linhasAtualizadas = await connection.ExecuteAsync(inativarConciliacoesSql, new 
                { 
                    OrgId = orgId, 
                    Cnpj = cnpj, 
                    Competencia = competencia
                });

                return linhasAtualizadas > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DesaprovarConciliacaoPorCnpjCompetenciaAsync(int orgId, string cnpj, string competencia)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string desaprovarConciliacaoSql = @"
                    UPDATE tb_conciliacao_folhaponto tcf
                    INNER JOIN tb_lote l ON tcf.tb_lote_id = l.id
                    SET tcf.aprovado = 0
                    WHERE l.tb_org_id = @OrgId
                    AND tcf.cnpj = @Cnpj
                    AND tcf.competencia = @Competencia
                    AND tcf.aprovado = 1";

                var linhasAtualizadas = await connection.ExecuteAsync(desaprovarConciliacaoSql, new 
                { 
                    OrgId = orgId, 
                    Cnpj = cnpj, 
                    Competencia = competencia
                });

                return linhasAtualizadas > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
} 