using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Extension;
using Core.Domain.Financeiro.Rubrica;
using Dapper;
using DataTransferObject.Domain.Apontamento.FolhaPonto;
using DataTransferObject.Domain.Financeiro.Holerite;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga.AnaliseDocumentosResult;
using Newtonsoft.Json;

namespace Colaboracao.Infra.Repositories.Financeiro.Financeiro.Rubrica;

public class RubricaCargaRepository(IDBConnection dapperConnection) : IRubricaCargaRepository
{
    private const string RUBRICA_CARGA_LOG_DEFAULT_SQL = @"
        SELECT *
        FROM (
            SELECT 
                trcl.id AS Id,
                trcl.status_processamento AS StatusProcessamento,
                CASE
                    WHEN trcl.status_processamento IN ('iniciado', 'conversao_png', 'extracao_huggingface', 'processamento')
                        THEN 'em andamento'
                    WHEN trcl.status_processamento = 'sucesso' THEN 'sucesso'
                    WHEN trcl.status_processamento = 'erro' THEN 'erro'
                    ELSE 'erro'
                END AS StatusSimplificado,
                trcl.cod_diretoria AS CodDiretoria,
                vdo.diretoria AS Diretoria,
                trcl.ano AS Ano,
                trcl.mes AS Mes,
                trcl.data_criacao AS Data,
                trcl.qtd_registros_processados_sucesso AS RegistrosProcessadosSucesso,
                trcl.qtd_registros_retornados AS RegistrosRetornados,
                tr.descricao AS Descricao,
                trcl.codigo_carga_rubrica AS CodigoCarga,
                tc.nome_completo AS NomeUsuario,
                trcl.tb_org_id AS TbOrgId,
                trcl.tb_rubrica_id AS TbRubricaId,
                ROW_NUMBER() OVER (PARTITION BY trcl.id ORDER BY vdo.diretoria) AS rn
            FROM
                tb_rubrica_carga_log trcl
            JOIN
                vw_diretoria_org vdo 
                ON trcl.cod_diretoria = vdo.cod_diretoria 
                AND trcl.tb_org_id = vdo.tb_org_id
            JOIN tb_rubrica tr 
                ON tr.id = trcl.tb_rubrica_id
            JOIN tb_colaborador tc 
                ON tc.codigo_interno_colaborador = trcl.codigo_interno_colaborador_criacao
        ) AS sub
        WHERE rn = 1
            ";

    public async Task<List<RubricaCargaStatusDTO>> ListarHistoricoDeCargaRecentes(int orgId, List<string>? diretorias)
    {
        var connection = dapperConnection.GetConnection();

        var query = RUBRICA_CARGA_LOG_DEFAULT_SQL;
        
        
        var inClauseUnidades = diretorias.BuildInClauseOrNull();
        
        query += $@"
            {(inClauseUnidades == null ? "" : $"AND sub.CodDiretoria IN {inClauseUnidades}")}
            AND sub.TbOrgId = @OrgId
            ORDER BY
                sub.Data DESC
            LIMIT 20;
        ";

        var result = await connection.QueryAsync<RubricaCargaStatusDTO>(query, new { OrgId = orgId });
        var cargas = result.ToList();
        
        // Buscar os itens de log para cada carga
        foreach (var carga in cargas)
        {
            carga.ItensLog = await ObterItensLogPorCodigoCarga(carga.CodigoCarga);
        }
        
        return cargas;
    }

    public async Task<RubricaCargaStatusDTO> ObterCargaPorCodigoCarga(string cargaId)
    {
        var connection = dapperConnection.GetConnection();

        var query = RUBRICA_CARGA_LOG_DEFAULT_SQL;
        query += @"
            AND 
                sub.CodigoCarga = @CargaId;
        ";

        var result = await connection.QuerySingleOrDefaultAsync<RubricaCargaStatusDTO>(query, new { CargaId = cargaId });
        
        if (result != null)
        {
            result.ItensLog = await ObterItensLogPorCodigoCarga(cargaId);
        }
        
        return result;
    }
    
    public async Task<List<RubricaCargaStatusDTO>> ListarCargasPorOrg(int orgId, string codigoDiretoria, string rubricaId, int mes, int ano)
    {
        var connection = dapperConnection.GetConnection();

        var query = RUBRICA_CARGA_LOG_DEFAULT_SQL;
        query += @"
            AND 
                sub.TbOrgId = @OrgId
                AND sub.TbRubricaId = @RubricaId
                AND sub.CodDiretoria = @CodigoDiretoria
                AND sub.Mes = @Mes
                AND sub.Ano = @Ano;
        ";

        var parametros = new
        {
            OrgId = orgId,
            CodigoDiretoria = codigoDiretoria,
            RubricaId = rubricaId,
            Mes = mes,
            Ano = ano
        };

        var result = await connection.QueryAsync<RubricaCargaStatusDTO>(query, parametros);
        return result.ToList();
    }

    public async Task<IEnumerable<RubricaCargaOrigemPdfDTO>> ObterPDFCargaPorMesEAno(int orgId, string codigoInternoColaborador, int mes, int ano)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
                SELECT 
                    trcl.arquivo_origem AS PathPdf, 
                    trc.tb_rubrica_id  AS RubricaId  
                FROM tb_rubrica_colaborador trc
                INNER JOIN tb_rubrica_carga_log trcl ON trcl.codigo_carga_rubrica = trc.codigo_carga_rubrica 
                WHERE trc.mes_inicial = @Mes
                    AND trc.ano_inicial = @Ano
                    AND trcl.tb_org_id = @OrgId
                    AND trc.codigo_interno_colaborador = @CodigoInternoColaborador
                GROUP BY trcl.codigo_carga_rubrica 
        ";

        var parametros = new
        {
            OrgId = orgId,
            Mes = mes,
            Ano = ano,
            CodigoInternoColaborador = codigoInternoColaborador
        };

        var result = await connection.QueryAsync<RubricaCargaOrigemPdfDTO>(query, parametros);
        return result;
	}
    public async Task<List<RubricaCargaItemLogDTO>> ObterItensLogPorCodigoCarga(string codigoCarga)
    {
        var connection = dapperConnection.GetConnection();

        var query = @"
            SELECT 
                id AS Id,
                codigo_carga_rubrica AS CodigoCargaRubrica,
                json_item_tentativa AS JsonItemTentativa,
                status_processamento AS StatusProcessamento,
                tipo_identificacao AS TipoIdentificacao,
                codigo_interno_colaborador AS CodigoInternoColaborador,
                tb_rubrica_colaborador_id AS TbRubricaColaboradorId,
                mensagem_erro AS MensagemErro,
                data_processamento AS DataProcessamento
            FROM tb_rubrica_carga_item_log
            WHERE codigo_carga_rubrica = @CodigoCarga
            ORDER BY data_processamento ASC;
        ";

        var result = await connection.QueryAsync<RubricaCargaItemLogDTO>(query, new { CodigoCarga = codigoCarga });
        return result.ToList();
    }
    
    public async Task<List<LoteFilaRubricaDTO>> BuscarLotesPorOrgAsync(int orgId, TipoFilaEnum tipoFila)
    {
        var connection = dapperConnection.GetConnection();
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
                        l.sumario AS SumarioRubricaCargaString
                    FROM tb_lote l
                    INNER JOIN tb_fila f ON l.tb_fila_id = f.id
                    WHERE l.tb_org_id = @OrgId
                    AND f.descricao = @Fila
                    ORDER BY l.data_criacao DESC";

            var lotes = await connection.QueryAsync<LoteFilaRubricaDTO>(
                buscarLotesSql, 
                new { OrgId = orgId, Fila = tipoFila.ToString() }
            );

            return lotes.AsList();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<TemplateRubricaDTO> ObterTemplateRubricaPorRubricaId(string rubricaId)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
                SELECT
                    trt.id AS Id,
                    trt.nome_template AS NomeTemplate,
                    trt.mapeamento_campos AS MapeamentoCampos,
                    trt.link_modelo_s3 AS LinkModeloS3,
                    trt.ativo AS Ativo,
                    trt.data_criacao AS DataCriacao,
                    trt.instrucoes_adicionais AS InstrucoesAdicionais,
                    trt.codigo_alternativa_tipo AS CodigoAlternativaTipo
                FROM tb_rubrica tr
                INNER JOIN tb_rubrica_template trt ON tr.tb_rubrica_template_id = trt.id
                WHERE tr.id = @RubricaId
        ";

        var parametros = new
        {
            RubricaId = rubricaId
        };
        
        return await connection.QuerySingleOrDefaultAsync<TemplateRubricaDTO>(query, parametros);
    }

    public async Task<RubricaAnaliseColaboradorDetalhadoDTO> IdentificarColaboradorDetalhadoPorCampo(string codigo, string tipoIdentificacao, int orgId)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
                SELECT
                    tc.codigo_interno_colaborador AS CodigoInternoColaborador,
                    tco.cod_diretoria AS CodigoDiretoria
                FROM tb_colaborador tc
                JOIN tb_colaborador_org tco 
                    ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador
                WHERE tco.tb_org_id = @OrgId
                AND (
                    (@TipoIdentificacao = 'codigo_externo' AND tco.cod_colaborador_externo = @Codigo)
                    OR (@TipoIdentificacao = 'cpf' AND tc.documento_colaborador = @Codigo)
                    OR (@TipoIdentificacao = 'nome_completo' AND tc.nome_completo = @Codigo)
                )
                LIMIT 1;
        ";

        var parametros = new
        {
            Codigo = codigo,
            OrgId = orgId,
            TipoIdentificacao = tipoIdentificacao
        };
        
        return await connection.QuerySingleOrDefaultAsync<RubricaAnaliseColaboradorDetalhadoDTO>(query, parametros);
    }
    
    public async Task SalvarLogItemAsync( string codigoCargaRubrica, object item, string status, string tipoIdentificacao, string? codigoInternoColaborador, string? tbRubricaColaboradorId, string mensagemErro)
    {
        var connection = dapperConnection.GetConnection();

        var query = @"
            INSERT INTO tb_rubrica_carga_item_log 
            (codigo_carga_rubrica, json_item_tentativa, status_processamento, 
             tipo_identificacao, codigo_interno_colaborador, tb_rubrica_colaborador_id, mensagem_erro)
            VALUES (@CodigoCargaRubrica, @JsonItemTentativa, @Status, @TipoIdentificacao, 
                    @CodigoInternoColaborador, @TbRubricaColaboradorId, @MensagemErro);
        ";

        var parametros = new
        {
            CodigoCargaRubrica = codigoCargaRubrica,
            JsonItemTentativa = JsonConvert.SerializeObject(item, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.Default }),
            Status = status,
            TipoIdentificacao = tipoIdentificacao,
            CodigoInternoColaborador = codigoInternoColaborador,
            TbRubricaColaboradorId = tbRubricaColaboradorId,
            MensagemErro = mensagemErro
        };

        await connection.ExecuteAsync(query, parametros);
    }

    public async Task<Guid> SalvarRubricaColaborador(string codigoCargaRubrica, string rubricaId, string codigoRubricaFrequencia, int mesInicial, int anoInicial, string valor, string codigoColaboradorUsuarioLogado, string templateNome, string codigoInternoColaborador)
    {
        var connection = dapperConnection.GetConnection();

        // Gera ID do registro
        var registroId = Guid.NewGuid();

        // Observação
        var observacao = $"Carregado pelo template {templateNome} - Carga: {codigoCargaRubrica}";

        // Conversão do valor brasileiro para double
        double ConverterValorBrasileiro(string valorStr)
        {
            if (string.IsNullOrWhiteSpace(valorStr))
                return 0;

            var valorLimpo = valorStr.Trim();

            // Se o valor contém vírgula, trata como formato BR
            if (valorLimpo.Contains(","))
            {
                valorLimpo = valorLimpo.Replace(".", "").Replace(",", ".");
                return double.Parse(valorLimpo, System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                // Caso contrário, trata como formato US
                return double.Parse(valorLimpo, System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        var query = @"
            INSERT INTO tb_rubrica_colaborador 
            (id, ativo, codigo_interno_colaborador_alteracao, codigo_interno_colaborador_criacao,
             valor, tb_rubrica_id, codigo_interno_colaborador,
             codigo_rubrica_frequencia, observacao, codigo_carga_rubrica, 
             mes_inicial, ano_inicial)
            VALUES (@Id, TRUE, @CodigoColaboradorUsuarioLogado, @CodigoColaboradorUsuarioLogado, 
                    @Valor, @TbRubricaId, @CodigoInternoColaborador, 
                    @CodigoRubricaFrequencia, @Observacao, @CodigoCargaRubrica, 
                    @MesInicial, @AnoInicial);
        ";

        var parametros = new
        {
            Id = registroId,
            CodigoColaboradorUsuarioLogado = codigoColaboradorUsuarioLogado,
            Valor = ConverterValorBrasileiro(valor),
            TbRubricaId = rubricaId,
            CodigoInternoColaborador = codigoInternoColaborador,
            CodigoRubricaFrequencia = codigoRubricaFrequencia,
            Observacao = observacao,
            CodigoCargaRubrica = codigoCargaRubrica,
            MesInicial = mesInicial,
            AnoInicial = anoInicial
        };

        await connection.ExecuteAsync(query, parametros);
        return registroId;
    }

    public async Task<string> CriarRubricaCargaLoteInicial( string pdfPath, string codDiretoria, string rubricaId, string rubricaTemplateId, string codigoInternoColaboradorCriacao, int orgId, int mesInicial, int anoInicial)
    {
        var connection = dapperConnection.GetConnection();

        var CodigoRrica = Guid.NewGuid();
        var query = @"
                INSERT INTO tb_rubrica_carga_log (
                    codigo_carga_rubrica,
                    arquivo_origem,
                    cod_diretoria,
                    tb_rubrica_id,
                    tb_rubrica_template_id,
                    status_processamento,
                    codigo_interno_colaborador_criacao,
                    tb_org_id,
                    mes,
                    ano
                ) VALUES (
                    @CodigoCargaRubrica,
                    @ArquivoOrigem,
                    @CodDiretoria,
                    @TbRubricaId,
                    @TbRubricaTemplateId,
                    'iniciado',
                    @CodigoInternoColaboradorCriacao,
                    @TbOrgId,
                    @Mes,
                    @Ano
                );
        ";

        var parametros = new
        {
            CodigoCargaRubrica = CodigoRrica,
            ArquivoOrigem = pdfPath,
            CodDiretoria = codDiretoria,
            TbRubricaId = rubricaId,
            TbRubricaTemplateId = rubricaTemplateId,
            CodigoInternoColaboradorCriacao = codigoInternoColaboradorCriacao,
            TbOrgId = orgId,
            Mes = mesInicial,
            Ano = anoInicial
        };

        await connection.ExecuteAsync(query, parametros);
        return CodigoRrica.ToString();
    }

    public async Task AtualizarRubricaCargaLogAsync( string cargaId, string jsonFinal, string status, int registrosProcessados, int registrosProcessadosComSucesso, string mensagemErro = null)
    {
        var connection = dapperConnection.GetConnection();

        var query = @"
        UPDATE tb_rubrica_carga_log 
        SET status_processamento = @Status,
            qtd_registros_processados_sucesso = @RegistrosProcessadosComSucesso,
            qtd_registros_retornados = @RegistrosProcessados,
            data_processamento_fim = NOW(),
            mensagem_erro = @MensagemErro,
            json_retorno_huggingface = @JsonFinal
        WHERE codigo_carga_rubrica = @CodigoCargaRubrica;
    ";

        var parametros = new
        {
            Status = status,
            RegistrosProcessadosComSucesso = registrosProcessadosComSucesso,
            RegistrosProcessados = registrosProcessados,
            MensagemErro = mensagemErro,
            JsonFinal = string.IsNullOrWhiteSpace(jsonFinal) ? null : jsonFinal,
            CodigoCargaRubrica = cargaId
        };

        await connection.ExecuteAsync(query, parametros);
    }
}