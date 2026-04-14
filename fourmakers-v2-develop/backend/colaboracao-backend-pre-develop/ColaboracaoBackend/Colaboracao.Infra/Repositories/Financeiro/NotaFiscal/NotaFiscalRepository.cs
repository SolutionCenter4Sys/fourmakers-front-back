using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ApiClient.Domain;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Extension;
using Core.Domain.Financeiro.NotaFiscal;
using Dapper;
using DataTransferObject.Domain.Financeiro.NotaFiscal;

namespace Colaboracao.Infra.Repositories.Financeiro.NotaFiscal;

public class NotaFiscalRepository(IDBConnection dapperConnection) : INotaFiscalRepository
{
    private const string DEFAULT_SQL = @"
        SELECT DISTINCT
            tnf.id AS Id,
            tnf.vigencia_mes AS VigenciaMes,
            tnf.vigencia_ano AS VigenciaAno,
            tnf.numero_nf AS NumeroNf,
            tnf.data_emissao_nota_fiscal AS DataEmissaoNotaFiscal,
            tnf.valor AS Valor,
            CONCAT(@BaseUrlDownloadTokenFile, '/', ttf.token, '/', tnf.url_nota_fiscal_download) AS UrlNotaFiscalDownload,
            tnf.tb_nota_fiscal_status_id AS NotaFiscalStatusId,
            tnf.motivo_reprovacao  AS MotivoReprovacao,
            tnf.data_criacao AS DataCriacao,
            tnf.data_alteracao AS DataAlteracao,
            tnf.codigo_interno_colaborador_criacao AS CodigoInternoColaboradorCriacao,
            tnf.codigo_interno_colaborador_alteracao AS CodigoInternoColaboradorAlteracao,
            tc.nome_completo AS NomeColaborador,
            tc.documento_colaborador as DocumentoColaborador,
            tu.email AS Email,
            tnfs.descricao AS NotaFiscalStatusDescricao,
            tnf.Valor_analise AS valorAnalisado
        FROM tb_nota_fiscal tnf
        INNER JOIN tb_nota_fiscal_rubrica tnfr 
            ON tnfr.tb_nota_fiscal_id = tnf.id 
        INNER JOIN tb_rubrica_colaborador trc
            ON trc.id = tnfr.tb_rubrica_colaborador_id 
        INNER JOIN tb_colaborador tc
            ON tc.codigo_interno_colaborador = trc.codigo_interno_colaborador
        JOIN tb_colaborador_org tco
            ON tco.codigo_interno_colaborador = trc.codigo_interno_colaborador AND tco.tb_org_id = tnf.tb_org_id
        INNER JOIN tb_nota_fiscal_status tnfs
            ON tnfs.id = tnf.tb_nota_fiscal_status_id
        INNER JOIN tb_usuario tu 
            ON tu.codigo_interno_colaborador = tc.codigo_interno_colaborador AND tu.tb_org_id = tnf.tb_org_id
        LEFT JOIN tb_token_file ttf 
            ON tnf.url_nota_fiscal_download = ttf.nome_arquivo
    ";

    
    public async Task<Guid> InserirNotaFiscalAsync(int vigenciaMes, int vigenciaAno, string codigoInternoColaborador, int orgId, decimal valor, string numeroNf, decimal? valorAnalise)
    {
        var connection = dapperConnection.GetConnection();
        const string query = @"
            INSERT INTO tb_nota_fiscal
            (
                id,
                vigencia_mes,
                vigencia_ano,
                data_criacao,
                data_alteracao,
                codigo_interno_colaborador_criacao,
                codigo_interno_colaborador_alteracao,
                tb_org_id,
                tb_nota_fiscal_status_id,
                valor,
                numero_nf,
                valor_analise
            )
            VALUES 
            (
                @Id,
                @VigenciaMes,
                @VigenciaAno,
                @DataCriacao,
                @DataAlteracao,
                @CodigoInternoColaboradorCriacao,
                @CodigoInternoColaboradorAlteracao,
                @OrgId,
                @StatusId,
                @Valor,
                @NumeroNf,
                @ValorAnalise
            )
        ";
        
        var uuid = Guid.NewGuid();
        var parameters = new
        {
            Id = uuid,
            VigenciaMes = vigenciaMes,
            VigenciaAno = vigenciaAno,
            DataCriacao = DateTime.UtcNow,
            DataAlteracao = DateTime.UtcNow,
            CodigoInternoColaboradorCriacao = codigoInternoColaborador,
            CodigoInternoColaboradorAlteracao = codigoInternoColaborador,
            OrgId = orgId,
            StatusId = NotaFiscalStatusEnum.AGUARDA_EMISSAO_DA_NF,
            Valor = valor,
            NumeroNf = numeroNf,
            ValorAnalise = valorAnalise
        };
        
        await connection.ExecuteAsync(query, parameters);
        return uuid;
    }


    public async Task<NotaFiscalResult> ObterNotaFiscalPorIdAsync(Guid id)
    {
        var connection = dapperConnection.GetConnection();

        const string query = @"
            SELECT
                id AS Id,
                vigencia_mes AS VigenciaMes,
                vigencia_ano AS VigenciaAno,
                numero_nf AS NumeroNf,
                data_emissao_nota_fiscal AS DataEmissaoNotaFiscal,
                valor AS Valor,
                CONCAT('@BaseUrlDownloadTokenFile', '/', ttf.token, '/', tnf.url_nota_fiscal_download) AS UrlNotaFiscalDownload,
                tb_nota_fiscal_status_id AS NotaFiscalStatusId,
				CONVERT(ttf.token, CHAR(255)) AS TokenFile,
                motivo_reprovacao  AS MotivoReprovacao,
                tnf.data_criacao AS DataCriacao,
                tnf.data_alteracao AS DataAlteracao,
                tnf.codigo_interno_colaborador_criacao AS CodigoInternoColaboradorCriacao,
                tnf.codigo_interno_colaborador_alteracao AS CodigoInternoColaboradorAlteracao
            FROM tb_nota_fiscal tnf
            LEFT JOIN tb_token_file ttf on tnf.url_nota_fiscal_download = ttf.nome_arquivo
            WHERE id = @Id
            LIMIT 1
        ";

        var parameters = new
        {
            Id = id,
            BaseUrlDownloadTokenFile = UploadFileUtil.GetBaseUrlDownloadTokenFile()
        };

        var result = await connection.QuerySingleOrDefaultAsync<NotaFiscalResult>(query, parameters);

        return result;
    }


    public async Task<IEnumerable<NotaFiscalResult>> ListarNotaFiscaisPorListaDeIdsAsync(List<Guid> ids)
    {
        var connection = dapperConnection.GetConnection();
        
        var query = DEFAULT_SQL;
         query += @"
            WHERE tnf.id IN @Ids
        ";

        var parameters = new
        {
            Ids = ids,
            BaseUrlDownloadTokenFile = UploadFileUtil.GetBaseUrlDownloadTokenFile()
        };
        
        var result = await connection.QueryAsync<NotaFiscalResult>(query, parameters);
        return result;
    }

    public async Task EditarNotaFiscalPorIdAsync(Guid id, string numeroNf, DateTime? dataEmissaoNotaFiscal, decimal? valor, string urlNotaFiscalDownload, NotaFiscalStatusEnum? notaFiscalStatusId, string codigoInternoColaboradorAlteracao)
    {
        var connection = dapperConnection.GetConnection();
        
        const string query = @"
            UPDATE tb_nota_fiscal
                SET numero_nf = @NumeroNf,
                    data_emissao_nota_fiscal = @DataEmissaoNotaFiscal,
                    valor = @Valor,
                    url_nota_fiscal_download = @UrlNotaFiscalDownload,
                    tb_nota_fiscal_status_id = @NotaFiscalStatusId,
                    data_alteracao = @DataAlteracao,
                    codigo_interno_colaborador_alteracao = @CodigoInternoColaboradorAlteracao
            WHERE id = @Id
        ";

        var parameters = new
        {
            Id = id,
            NumeroNf = numeroNf,
            DataEmissaoNotaFiscal = dataEmissaoNotaFiscal,
            Valor = valor,
            UrlNotaFiscalDownload = urlNotaFiscalDownload,
            NotaFiscalStatusId = notaFiscalStatusId,
            DataAlteracao = DateTime.UtcNow,
            CodigoInternoColaboradorAlteracao = codigoInternoColaboradorAlteracao
        };
        
        var rows = await connection.ExecuteAsync(query, parameters);
        if (rows == 0)
            throw new ArgumentException("Erro ao tentar atualizar Nota Fiscal.");
    }
    
    public async Task<NotaFiscalResult> MudarStatusNotaFiscalPorIdAsync(Guid id, NotaFiscalStatusEnum? notaFiscalStatusId, string motivoReprovacao, string codigoInternoColaboradorAlteracao)
    {
        var connection = dapperConnection.GetConnection();
        
        const string query = @"
            UPDATE tb_nota_fiscal
                SET numero_nf = @NumeroNf,
                    tb_nota_fiscal_status_id = @NotaFiscalStatusId,
                    motivo_reprovacao = @MotivoReprovacao,
                    data_alteracao = @DataAlteracao,
                    codigo_interno_colaborador_alteracao = @CodigoInternoColaboradorAlteracao
            WHERE id IN @Id
        ";

        var parameters = new
        {
            Id = id,
            NotaFiscalStatusId = notaFiscalStatusId,
            MotivoReprovacao = motivoReprovacao,
            DataAlteracao = DateTime.UtcNow,
            CodigoInternoColaboradorAlteracao = codigoInternoColaboradorAlteracao
        };
        
        var rows = await connection.ExecuteAsync(query, parameters);
        if (rows == 0)
            return null;

        return await ObterNotaFiscalPorIdAsync(id);
    }

    public async Task<IEnumerable<NotaFiscalResult>> MudarStatusNotaFiscalPorListaDeIdsAsync(List<Guid> ids, NotaFiscalStatusEnum notaFiscalStatusId, string motivoReprovacao, string codigoInternoColaboradorAlteracao)
    {
        var connection = dapperConnection.GetConnection();

        var setClauses = new List<string>
        {
            "tb_nota_fiscal_status_id = @NotaFiscalStatusId",
            "motivo_reprovacao = @MotivoReprovacao",
            "data_alteracao = @DataAlteracao"
        };

        if (!string.IsNullOrWhiteSpace(codigoInternoColaboradorAlteracao))
        {
            setClauses.Add("codigo_interno_colaborador_alteracao = @CodigoInternoColaboradorAlteracao");
        }

        var query = $@"
            UPDATE tb_nota_fiscal
               SET {string.Join(",\n           ", setClauses)}
             WHERE id IN @Id
        ";

        var parameters = new
        {
            Id = ids,
            NotaFiscalStatusId = notaFiscalStatusId,
            MotivoReprovacao = motivoReprovacao,
            DataAlteracao = DateTime.UtcNow,
            CodigoInternoColaboradorAlteracao = codigoInternoColaboradorAlteracao
        };

        var rows = await connection.ExecuteAsync(query, parameters);
        if (rows == 0)
            return [];

        return await ListarNotaFiscaisPorListaDeIdsAsync(ids);

    }
    
    public async Task<IEnumerable<NotaFiscalResult>> ListarNotasFiscaisPorVigencia(string filtro, NotaFiscalStatusEnum? statusId, int? mes, int? ano, List<string>? diretorias, string? documentoColaborador, string codigoInternoColaborador,int orgId, int cursor, int? limite, string numeroNf = "")
    {
        var connection = dapperConnection.GetConnection();
        
        var query = DEFAULT_SQL;
        
        var inClauseUnidades = diretorias.BuildInClauseOrNull();
        
        query += $@"
            WHERE
                tnf.tb_org_id = @OrgId
                AND (@Mes IS NULL OR tnf.vigencia_mes = @Mes)
                AND (@Ano IS NULL OR tnf.vigencia_ano = @Ano)
                AND (@StatusId IS NULL OR tnf.tb_nota_fiscal_status_id = @StatusId)
                AND (@CodigoInternoColaborador is NULL or tc.codigo_interno_colaborador = @CodigoInternoColaborador)
                AND (
                    @DocumentoColaborador IS NULL
                    OR REPLACE(REPLACE(tc.documento_colaborador, '.', ''), '-', '') = REPLACE(REPLACE(@DocumentoColaborador, '.', ''), '-', '')
                )
                {(inClauseUnidades == null ? "" : $"AND tco.cod_diretoria IN {inClauseUnidades}")}
                AND (
                    @Filtro IS NULL
                    OR tnfs.descricao = @Filtro
                    OR tnf.numero_nf = @Filtro
                    OR tc.nome_completo = @Filtro
                )
                AND (@NumeroNf IS NULL OR tnf.numero_nf = @NumeroNf)
        ";

        if (limite.HasValue())
        {
            query += @"
            LIMIT @Limite OFFSET @Cursor
        ";
        }

        var parameters = new
        {
            CodigoInternoColaborador = codigoInternoColaborador.ToNullSeTextoNullOuZero(),
            Mes = mes?.ToNullSeTextoNullOuZero(),
            Ano = ano?.ToNullSeTextoNullOuZero(),
            DocumentoColaborador = documentoColaborador.ToNullSeTextoNullOuZero(),
            StatusId = statusId.ToIntOuNull(),
            Filtro = filtro.ToNullSeTextoNullOuZero(),
            Cursor = cursor,
            Limite = limite,
            OrgId = orgId,
            NumeroNf = numeroNf.ToNullSeTextoNullOuZero(),
            BaseUrlDownloadTokenFile = UploadFileUtil.GetBaseUrlDownloadTokenFile()
        };
    
        var result = await connection.QueryAsync<NotaFiscalResult>(query, parameters);
        
        //foreach (var notaFiscalResult in result)
        //{
        //    if (notaFiscalResult.UrlNotaFiscalDownload.EhStringValidaEDiferenteDeZero())
        //    {
        //        notaFiscalResult.UrlNotaFiscalDownload = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("SERVICE_MEDIA_BASE_URL") + notaFiscalResult.UrlNotaFiscalDownload;
        //        notaFiscalResult.UrlNotaFiscalDownload = notaFiscalResult.UrlNotaFiscalDownload.Replace("$1/", $"{notaFiscalResult.TokenFile}/");
        //    }
        //}
        return result;
    }
    
    public async Task<IEnumerable<NotaFiscalStatusDTO>> ListarNotaFiscalStatus()
    {
        var connection = dapperConnection.GetConnection();
        
        var query = @"
            SELECT
                id AS Id,
                descricao AS Descricao
            FROM tb_nota_fiscal_status 
            ORDER BY id ASC
        ";
        
        return await connection.QueryAsync<NotaFiscalStatusDTO>(query);
    }

    public async Task<List<Guid>> ListarIdsNotasFiscaisPendentesParaEmissao(int orgId, int mes, int ano, string? codigoDiretoria)
    {
        var connection = dapperConnection.GetConnection();

        var query = @"
                SELECT DISTINCT
                    tnf.id
                FROM tb_nota_fiscal tnf
                INNER JOIN tb_nota_fiscal_rubrica tnfr
                    ON tnfr.tb_nota_fiscal_id = tnf.id
                INNER JOIN tb_rubrica_colaborador trc
                    ON trc.id = tnfr.tb_rubrica_colaborador_id
                JOIN tb_colaborador_org tco
                    ON tco.codigo_interno_colaborador = trc.codigo_interno_colaborador
                    AND tco.tb_org_id = tnf.tb_org_id
                WHERE
                    tnf.tb_nota_fiscal_status_id = 1
                    AND tnf.tb_org_id = @OrgId
                    AND tnf.vigencia_mes = @Mes
                    AND tnf.vigencia_ano = @Ano
                    AND (
                        @CodDiretoria IS NULL
                        OR REPLACE(REPLACE(tco.cod_diretoria, '.', ''), '-', '') = REPLACE(REPLACE(@CodDiretoria, '.', ''), '-', '')
                    )
            ";

        var parameters = new
        {
            OrgId = orgId,
            Mes = mes,
            Ano = ano,
            CodDiretoria = codigoDiretoria.ToNullSeTextoNullOuZero()
        };

        return (await connection.QueryAsync<Guid>(query, parameters)).ToList();
    }

    public async Task<string> BuscarCodigoInternoColaboradorPorExterno(string codigoColaboradorExterno, int orgId)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
                      SELECT 
                          codigo_interno_colaborador 
                      FROM
                          tb_colaborador_org 
                      WHERE
                          cod_colaborador_externo = @CodigoColaboradorExterno
                          AND tb_org_id = @OrgId
                      ";

        var parametros = new
        {
            CodigoColaboradorExterno = codigoColaboradorExterno,
            OrgId = orgId
        };

        var result = await connection.QuerySingleOrDefaultAsync<string>(query, parametros);
        return result;
    }
}