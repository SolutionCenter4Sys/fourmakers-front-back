using ApiClient.Domain;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Infra.Repositories.Financeiro.Financeiro.Rubrica;
using Core.Domain.Financeiro.IntegracaoContabil;
using Dapper;
using DataTransferObject.Domain.Financeiro.IntegracaoContabil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Financeiro.IntegracaoContabil
{
    public class IntegracaoContabilRepository : IIntegracaoContabilRepository
    {
        private readonly IDBConnection _dbConnection;

        public IntegracaoContabilRepository(IDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<RemessaContabilRegistroOutrosDTO>> ObterDadosRelatorioFechamentoContabilAsync(string cnpj, string competencia, int orgId)
        {
            var remessa = await GerarRemessaContabilMensalFolhaPontoERubrica(cnpj, competencia, orgId);
            return remessa.Dados;
        }

        public async Task<RemessaContabilDTO> GerarRemessaContabilMensalFolhaPontoERubrica(string cnpj, string competencia, int orgId)
        {
            var (dadosFolhaPonto, idsFolhaPonto) = await ObterDadosEIds<RemessaContabilRegistroOutrosDTO>("folha_ponto", cnpj, competencia, orgId);
            var (dadosRubricas, idsRubrica) = await ObterDadosEIds<RemessaContabilRegistroOutrosDTO>("rubrica", cnpj, competencia, orgId);
            //var (dadosReembolso, idsReembolso) = await ObterDadosEIds<RemessaContabilRegistroDTO>("reembolso", cnpj, competencia, orgId); #12468 - retirar reembolso

            var todosDados = dadosFolhaPonto.Concat(dadosRubricas)//.Concat(dadosReembolso)
                                            .OrderBy(x => x.NomeColaborador)
                                            .ThenBy(x => x.Categoria)
                                            .ThenBy(x => x.Tipo)
                                            .ToList();

            return new RemessaContabilDTO
            {
                Dados = todosDados,
                Ids = new RemessaContabilIdsDTO
                {
                    FolhaPonto = idsFolhaPonto,
                    Rubrica = idsRubrica
                }
            };
        }

        public async Task<RemessaContabilDTO> GerarRemessaContabilMensalReembolso(string cnpj, string competencia, int orgId)
        {
            var (dadosReembolso, idsReembolso) = await ObterDadosEIds<RemessaContabilRegistroReembolsoDTO>("reembolso", cnpj, competencia, orgId);

            var todosDados = dadosReembolso.OrderBy(x => x.NomeColaborador)
                                           .ThenBy(x => x.Categoria)
                                           .ThenBy(x => x.Tipo)
                                           .ToList();

            return new RemessaContabilDTO
            {
                DadosReembolso = todosDados,
                Ids = new RemessaContabilIdsDTO
                {
                    Reembolso = idsReembolso
                }
            };
        }

        private async Task<(List<T> dados, List<string> ids)> ObterDadosEIds<T>(string origem, string cnpj, string competencia, int orgId)
        {
            var connection = _dbConnection.GetConnection();
            string query = origem switch
            {
                "folha_ponto" => GetQueryFolhaPonto(),
                "rubrica" => GetQueryRubrica(),
                "reembolso" => GetQueryReembolso(),
                _ => throw new ArgumentException("Origem desconhecida.")
            };

            int? mes = null, ano = null;
            
            try
            {
                if (competencia is not null) //competencia pode ser null
                {
                    mes = int.Parse(competencia.Split('/')[0]);
                    ano = int.Parse(competencia.Split('/')[1]);
                }
            }
            catch
            {
                throw new Exception($"Erro interno: Formato de competência inválido: {competencia}.");
            }

            var dataRef = RubricaColaboradorRepository.MontaDataRef(mes, ano);

            var resultado = await connection.QueryAsync<T>(query, new { cnpj, competencia, orgId, DataRef = dataRef });
            var dados = resultado.ToList();

            var ids = resultado.Select(x => (string)(x.GetType().GetProperty("IdRegistro__ND")?.GetValue(x)?.ToString() ?? ""))
                               .Where(id => !string.IsNullOrEmpty(id))
                               .Distinct()
                               .ToList();

            return (dados, ids);
        }

        private string GetQueryFolhaPonto()
        {
            return @"
                        SELECT
                            'folha_ponto' AS Origem,
                            fp.competencia AS Competencia,
                            fp.cnpj AS Cnpj,
                            tco.diretoria AS Unidade,
                            tc.documento_colaborador AS Cpf,
                            tco.cod_colaborador_externo AS Matricula,
                            JSON_UNQUOTE(JSON_EXTRACT(fp.objeto_folhaponto, '$.RelatorioPonto.Funcionario.Nome')) AS NomeColaborador,
                            '' AS Tipo,
                            resumo_item.valor AS Valor,
                            resumo_item.categoria AS Categoria,
                            CAST(fp.tb_item_lote_id AS CHAR) AS IdRegistro__ND
                        FROM tb_folhaponto_colaborador fp
                        LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = fp.codigo_interno_colaborador
                        LEFT JOIN tb_colaborador_org tco ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador AND tco.tb_org_id = @orgId
                        CROSS JOIN JSON_TABLE(
                            JSON_ARRAY(
                                JSON_OBJECT('Tipo', 'Dsr', 'Valor', JSON_UNQUOTE(JSON_EXTRACT(fp.objeto_folhaponto, '$.RelatorioPonto.Resumo.Dsr')), 'Categoria', 'Resumo Dsr'),
                                JSON_OBJECT('Tipo', 'Atrasos', 'Valor', JSON_UNQUOTE(JSON_EXTRACT(fp.objeto_folhaponto, '$.RelatorioPonto.Resumo.Atrasos')), 'Categoria', 'Resumo Atrasos'),
                                JSON_OBJECT('Tipo', 'FaltasDias', 'Valor', JSON_UNQUOTE(JSON_EXTRACT(fp.objeto_folhaponto, '$.RelatorioPonto.Resumo.FaltasDias')), 'Categoria', 'Resumo FaltasDias'),
                                JSON_OBJECT('Tipo', 'DiasTrabalhados', 'Valor', JSON_UNQUOTE(JSON_EXTRACT(fp.objeto_folhaponto, '$.RelatorioPonto.Resumo.DiasTrabalhados')), 'Categoria', 'Resumo DiasTrabalhados'),
                                JSON_OBJECT('Tipo', 'Extras', 'Valor', JSON_UNQUOTE(JSON_EXTRACT(fp.objeto_folhaponto, '$.RelatorioPonto.Totais.Extras')), 'Categoria', 'Totais Extras'),
                                JSON_OBJECT('Tipo', 'Faltas', 'Valor', JSON_UNQUOTE(JSON_EXTRACT(fp.objeto_folhaponto, '$.RelatorioPonto.Totais.Faltas')), 'Categoria', 'Totais Faltas'),
                                JSON_OBJECT('Tipo', 'Trabalhadas', 'Valor', JSON_UNQUOTE(JSON_EXTRACT(fp.objeto_folhaponto, '$.RelatorioPonto.Totais.Trabalhadas')), 'Categoria', 'Totais Trabalhadas'),
                                JSON_OBJECT('Tipo', 'AdicionalNoturno', 'Valor', JSON_UNQUOTE(JSON_EXTRACT(fp.objeto_folhaponto, '$.RelatorioPonto.Totais.AdicionalNoturno')), 'Categoria', 'Totais AdicionalNoturno'),
                                JSON_OBJECT('Tipo', 'SaldoAtual', 'Valor', JSON_UNQUOTE(JSON_EXTRACT(fp.objeto_folhaponto, '$.RelatorioPonto.Totais.SaldoAtual')), 'Categoria', 'Totais SaldoAtual'),
                                JSON_OBJECT('Tipo', 'BancoHorasCredito', 'Valor', JSON_UNQUOTE(JSON_EXTRACT(fp.objeto_folhaponto, '$.RelatorioPonto.Totais.BancoHorasCredito')), 'Categoria', 'Totais BancoHorasCredito'),
                                JSON_OBJECT('Tipo', 'BancoHorasDebito', 'Valor', JSON_UNQUOTE(JSON_EXTRACT(fp.objeto_folhaponto, '$.RelatorioPonto.Totais.BancoHorasDebito')), 'Categoria', 'Totais BancoHorasDebito')
                            ),
                            '$[*]' COLUMNS (
                                tipo VARCHAR(50) PATH '$.Tipo',
                                valor VARCHAR(50) PATH '$.Valor',
                                categoria VARCHAR(200) PATH '$.Categoria'
                            )
                        ) AS resumo_item
                        WHERE fp.competencia = @competencia
                            AND fp.cnpj = @cnpj
                            AND resumo_item.valor != '00:00' AND resumo_item.valor != '0' AND resumo_item.valor != ''
                        
                        UNION ALL
                        
                        SELECT
                            'folha_ponto' AS Origem,
                            fp.competencia AS Competencia,
                            fp.cnpj AS Cnpj,
                            tco.diretoria AS Unidade,
                            tc.documento_colaborador AS Cpf,
                            tco.cod_colaborador_externo AS Matricula,
                            JSON_UNQUOTE(JSON_EXTRACT(fp.objeto_folhaponto, '$.RelatorioPonto.Funcionario.Nome')) AS NomeColaborador,
                            '' AS Tipo,
                            he_item.quantidade AS Valor,
                            CONCAT('Resumo ', he_item.tipo) AS Categoria,
                            CAST(fp.tb_item_lote_id AS CHAR) AS IdRegistro__ND
                        FROM tb_folhaponto_colaborador fp
                        LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = fp.codigo_interno_colaborador
                        LEFT JOIN tb_colaborador_org tco ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador AND tco.tb_org_id = @orgId
                        CROSS JOIN JSON_TABLE(
                            JSON_EXTRACT(fp.objeto_folhaponto, '$.RelatorioPonto.Resumo.HorasExtras'),
                            '$[*]' COLUMNS (
                                tipo VARCHAR(100) PATH '$.Tipo',
                                quantidade VARCHAR(20) PATH '$.Quantidade'
                            )
                        ) AS he_item
                        WHERE fp.competencia = @competencia
                            AND fp.cnpj = @cnpj
                            AND he_item.quantidade != '00:00' AND he_item.quantidade != '0' AND he_item.quantidade != ''";
        }

        private string GetQueryReembolso()
        {
            string baseUrlDownloadTokenFile = UploadFileUtil.GetBaseUrlDownloadTokenFile();

            return @$"
                   SELECT
                        tsr.id as ReembolsoId,
                        'reembolso' AS Origem,
                        DATE_FORMAT(tsr.data_despesa, '%m/%Y') AS Competencia,
                        tsr.data_despesa AS DataDespesa,
                        tsp.data_solicitacao AS DataSolicitacao,
                        tsr.data_aprovacao AS DataAprovacao,
                        tco.cod_diretoria AS Cnpj,
                        tco.diretoria AS Unidade,
                        tc.documento_colaborador AS Cpf,
                        tco.cod_colaborador_externo AS Matricula,
                        tc.nome_completo AS NomeColaborador,
                        CASE 
                            WHEN tvt.operacao = '-' THEN 'débito'
                            WHEN tvt.operacao = '+' THEN 'crédito'
                            ELSE LOWER(tvt.operacao)
                        END AS Tipo,
                        CAST(tsp.valor AS CHAR) AS Valor,
                        tv.categoria AS Categoria,
	                    CAST(
                            JSON_ARRAYAGG(
                                CONCAT('{baseUrlDownloadTokenFile}/', ttf.token, '/', ttf.nome_arquivo)
                            ) AS JSON
                        ) AS DocumentosJson,
                        CAST(tsp.id AS CHAR) AS IdRegistro__ND,
                        CASE 
                            WHEN tcdb.forma_pagamento = 'PIX' 
                                THEN CONCAT('PIX: ', tcdb.chave_pix)
                            WHEN tcdb.forma_pagamento = 'TED' 
                                THEN CONCAT(
                                    'BANCO: ', tcdb.codigo_banco_ted,
                                    ' AG: ', tcdb.agencia_ted,
                                    ' CC: ', tcdb.conta_ted, '-', tcdb.conta_dv_ted
                                )
                            ELSE NULL
                        END AS FormaPagamento
                   FROM
                        tb_solicitacao_pagamento tsp
                        JOIN tb_solicitacao_reembolso tsr ON tsr.id = tsp.tb_solicitacao_reembolso_id
                        LEFT JOIN tb_solicitacao_documento tsd ON tsr.id = tsd.tb_solicitacao_reembolso_id
                        LEFT JOIN tb_token_file ttf ON tsd.relative_path = ttf.nome_arquivo
                        JOIN tb_cliente_org tcli ON tcli.codigo_cliente = tsr.tb_cliente_id AND tcli.tb_org_id = tsr.tb_org_id
                        JOIN tb_projeto_org tpo ON tpo.cod_projeto = tsr.tb_projeto_id AND tpo.tb_org_id = tsr.tb_org_id
                        JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = tsr.codigo_interno_colaborador
                        JOIN tb_colaborador_org tco ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador AND tco.tb_org_id = tsr.tb_org_id
                        JOIN tb_verba tv ON tv.id = tsr.tb_verba_id
                        JOIN tb_verba_tipo tvt ON tvt.id = tv.tipo_custo
						LEFT JOIN tb_colaborador_dados_bancarios tcdb ON tcdb.codigo_interno_colaborador = tco.codigo_interno_colaborador
                   WHERE
                        (@competencia IS NULL OR @competencia = '' OR DATE_FORMAT(tsr.data_despesa, '%m/%Y') = @competencia)
                        AND (@cnpj IS NULL OR @cnpj = '' OR tco.cod_diretoria = @cnpj)
                        AND tsr.tb_org_id = @orgId
                        AND tsp.tb_status_solicitacao_pagamento_id = 1
                        AND tsp.valor != 0
                   GROUP BY
                       tsr.id";
        }

        private string GetQueryRubrica()
        {
            return @$"
                    SELECT
                        'rubrica' AS Origem,
                        @competencia AS Competencia,
                        tco.cod_diretoria AS Cnpj,
                        tco.diretoria AS Unidade,
                        tc.documento_colaborador AS Cpf,
                        tco.cod_colaborador_externo AS Matricula,
                        tc.nome_completo AS NomeColaborador,    
                        CASE 
                            WHEN tr.rubrica_tipo = 'Provento' THEN 'crédito'
                            WHEN tr.rubrica_tipo = 'Desconto' THEN 'débito'
                            ELSE ''
                        END AS Tipo,
                        CASE 
                            WHEN tr.calculo_tipo = 'Valor' THEN CAST(trc.valor AS CHAR)
                            WHEN tr.calculo_tipo = 'Porcentagem' THEN CONCAT(CAST(trc.percentual AS CHAR), '%')
                            WHEN tr.calculo_tipo = 'Hora' THEN trc.hora
                            ELSE NULL
                        END AS Valor,
                        tr.descricao AS Categoria,
                        CAST(trc.id AS CHAR) AS IdRegistro__ND
                    FROM tb_rubrica_colaborador trc
                    JOIN tb_rubrica tr ON tr.id = trc.tb_rubrica_id
                    JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = trc.codigo_interno_colaborador
                    JOIN tb_colaborador_org tco ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador
                    WHERE 
                        trc.ativo = 1
                        AND tr.ativo = 1
                        AND tr.refletir_contabil = 1
                        AND tco.cod_diretoria = @cnpj
                        AND tco.tb_org_id = @orgId
                        AND (
                            (tr.calculo_tipo = 'Valor' AND trc.valor != 0)
                            OR (tr.calculo_tipo = 'Porcentagem' AND trc.percentual != 0)
                            OR (tr.calculo_tipo = 'Hora' AND trc.hora IS NOT NULL AND trc.hora != '')
                        )
                        AND {RubricaColaboradorRepository.FILTRO_DATA_REF_DEFAULT}"; 
        }

        public async Task<string> BuscarOuCriarVigenciaAsync(string competencia)
        {
            var connection = _dbConnection.GetConnection();
          
            if (!DateTime.TryParseExact(competencia, "MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime data))
            {
                throw new ArgumentException($"Formato de competência inválido: {competencia}. Use formato MM/yyyy ou MM-yyyy.");
            }

            var mes = data.Month;
            var ano = data.Year;

            var vigenciaExistente = await connection.QuerySingleOrDefaultAsync<string>(
                "SELECT CAST(id AS CHAR) FROM tb_vigencia WHERE mes = @mes AND ano = @ano",
                new { mes, ano });

            if (!string.IsNullOrEmpty(vigenciaExistente))
            {
                return vigenciaExistente;
            }

            // Criar nova vigência
            var novoId = Guid.NewGuid().ToString();
            await connection.ExecuteAsync(
                "INSERT INTO tb_vigencia (id, mes, ano) VALUES (@id, @mes, @ano)",
                new { id = novoId, mes, ano });

            return novoId;
        }

        public async Task GravarLogRemessaContabilAsync(string vigenciaId, string cnpj, int orgId, object idsRegistros, string codigoInternoColaboradorCriacao)
        {
            var connection = _dbConnection.GetConnection();

            var query = @"
                INSERT INTO tb_remessa_contabil 
                (tb_vigencia_id, cnpj, tb_org_id, ids_registros, codigo_interno_colaborador_criacao)
                VALUES (@vigenciaId, @cnpj, @orgId, @idsRegistros, @codigoInternoColaboradorCriacao)";

            await connection.ExecuteAsync(query, new
            {
                vigenciaId,
                cnpj,
                orgId,
                idsRegistros = System.Text.Json.JsonSerializer.Serialize(idsRegistros),
                codigoInternoColaboradorCriacao
            });
        }
    }
}
