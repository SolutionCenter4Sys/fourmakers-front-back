using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Core.Domain.Financeiro.Rubrica;
using Dapper;
using DataTransferObject.Domain.Financeiro.NotaFiscal;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaborador;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaboradorLiberacao;

namespace Colaboracao.Infra.Repositories.Financeiro.Rubrica;

public class RubricaColaboradorLiberacaoNfRepository(IDBConnection dapperConnection) :  IRubricaColaboradorLiberacaoNfRepository
{
    private readonly IDbConnection _connection = dapperConnection.GetConnection();

    private const string DefaultSql = @"
        SELECT 
             trclf.id AS Id,
             EXISTS (
		        SELECT 1
		        FROM tb_nota_fiscal tnf
		        LEFT JOIN tb_nota_fiscal_rubrica tnfr
		            ON tnfr.tb_nota_fiscal_id = tnf.id
		            AND tnfr.tb_org_id = tnf.tb_org_id
		        WHERE tnfr.tb_rubrica_colaborador_id = trclf.tb_rubrica_colaborador_id
		          AND tnf.vigencia_mes = trclf.vigencia_mes
		          AND tnf.vigencia_ano = trclf.vigencia_ano
		          AND tnf.tb_org_id = trclf.tb_org_id
		          AND tnf.tb_nota_fiscal_status_id IS NOT NULL
		          AND tnf.tb_nota_fiscal_status_id NOT IN (4, 7)
		    ) AS EmUso,
             trc.valor AS Valor,
             tr.descricao AS Descricao,
             tc.nome_completo AS NomeCompleto,
             tco.codigo_interno_colaborador AS CodigoInternoColaborador,
             nota_fiscal.tb_nota_fiscal_status_id AS StatusNotaFiscal,
             nota_fiscal.numero_nf AS NumeroNotaFiscal,
             COALESCE(NULLIF(nota_fiscal.descricao, ''), 'Aguardando Emissão da NF') AS StatusDescricao,
             trc.id AS RubricaColaboradorId,
             trclf.vigencia_mes AS VigenciaMes,
             trclf.vigencia_ano AS VigenciaAno,
             CASE 
                            WHEN tr.rubrica_tipo = 'Provento' THEN 'Crédito'
                            WHEN tr.rubrica_tipo = 'Desconto' THEN 'Débito'
                            ELSE ''
                        END AS Natureza
         FROM tb_rubrica_colaborador_liberacao_nf trclf
         INNER JOIN tb_rubrica_colaborador trc
             ON trc.id = trclf.tb_rubrica_colaborador_id
         INNER JOIN tb_colaborador_org tco ON tco.codigo_interno_colaborador = trc.codigo_interno_colaborador AND tco.tb_org_id = trclf.tb_org_id
         INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
         INNER JOIN tb_rubrica tr
             ON tr.id = trc.tb_rubrica_id
         LEFT JOIN (
            SELECT DISTINCT
                  tnf.id,
               tnf.numero_nf,
               tnf.tb_nota_fiscal_status_id,
               tnfs.descricao,
               tnf.vigencia_mes,
               tnf.vigencia_ano,
               tnf.tb_org_id,
               tnfr.tb_rubrica_colaborador_id
            FROM tb_nota_fiscal tnf
          LEFT JOIN tb_nota_fiscal_status tnfs ON tnfs.id = tnf.tb_nota_fiscal_status_id
          LEFT JOIN tb_nota_fiscal_rubrica tnfr
              ON tnfr.tb_nota_fiscal_id = tnf.id
              AND tnfr.tb_org_id = tnf.tb_org_id
         ) AS nota_fiscal
             ON nota_fiscal.tb_org_id = trclf.tb_org_id
                    AND nota_fiscal.vigencia_mes = trclf.vigencia_mes
                    AND nota_fiscal.vigencia_ano = trclf.vigencia_ano
                    AND nota_fiscal.tb_rubrica_colaborador_id = trclf.tb_rubrica_colaborador_id
         
    ";

    public async Task<List<Guid>> ListarRubricaColaboradorIdsEmLiberacaoPorVigenciaAsync(int mes, int ano, int orgId)
    {
        var query = @"
            SELECT
                tb_rubrica_colaborador_id
            FROM tb_rubrica_colaborador_liberacao_nf
            WHERE
                vigencia_mes = @Mes
                AND vigencia_ano = @Ano
                AND tb_org_id = @OrgId
        ";

        var parametros = new
        {
            Mes = mes,
            Ano = ano,
            OrgId = orgId
        };
        
        var result = await _connection.QueryAsync<Guid>(query, parametros);

        return result.ToList();
    }

    public async Task InserirRubricaLiberacaoColaboradorAsync(int orgId, Guid rubricaColaboradorId, int mes, int ano)
    {
        var query = @"
            INSERT INTO tb_rubrica_colaborador_liberacao_nf
            (id, tb_rubrica_colaborador_id, vigencia_mes, vigencia_ano, tb_org_id)
            VALUES
            (@Id, @RubricaColaboradorId, @Mes, @Ano, @OrgId)
        ";
        var parametros = new
        {
            Id = Guid.NewGuid(),
            OrgId = orgId,
            RubricaColaboradorId = rubricaColaboradorId,
            Mes = mes,
            Ano = ano
        };
        await _connection.ExecuteAsync(query, parametros);
    }

    public async Task<List<RubricaColaboradorLiberacaoNfDTO>> ListarRubricasColaboradorLiberacaoNfPorVigenciaAsync(int? mes, int? ano, int orgId, string codigoInternoColaborador)
    {
        var sql = DefaultSql;
        sql += @"
            WHERE
                trc.codigo_interno_colaborador = @CodigoInternoColaborador
                AND trclf.tb_org_id = @OrgId
                AND (@Mes IS NULL OR  trclf.vigencia_mes = @Mes)
                AND (@Ano IS NULL OR  trclf.vigencia_ano = @Ano)
        ";

        var parametros = new
        {
            CodigoInternoColaborador = codigoInternoColaborador,
            OrgId = orgId,
            Mes = mes.ToNullSeTextoNullOuZero(),
            Ano = ano.ToNullSeTextoNullOuZero()
        };
        
        var rows = await _connection.QueryAsync<dynamic>(sql, parametros);
        var result = ConverterDynamicResultParaListaDeNotasFiscais(rows);
        return result.ToList();
    }
    
    public async Task<List<RubricaColaboradorLiberacaoNfDTO>> ListarRubricasLiberacaoNaoUsadasPorVigenciaAsync(int? mes, int? ano, int orgId, string? codigoInternoColaborador, string? codigoDiretoria)
    {
        var sql = DefaultSql;
        sql += @"
            WHERE
                trclf.tb_org_id = @OrgId
                AND (@CodigoInternoColaborador IS NULL OR trc.codigo_interno_colaborador = @CodigoInternoColaborador)
                AND (@Mes IS NULL OR  trclf.vigencia_mes = @Mes)
                AND (@Ano IS NULL OR  trclf.vigencia_ano = @Ano)
                AND (@CodigoUnidade IS NULL OR  tco.cod_diretoria = @CodigoUnidade)
        ";

        var parametros = new
        {
            CodigoInternoColaborador = codigoInternoColaborador.ToNullSeTextoNullOuZero(),
            OrgId = orgId,
            Mes = mes.ToNullSeTextoNullOuZero(),
            Ano = ano.ToNullSeTextoNullOuZero(),
            CodigoUnidade = codigoDiretoria.ToNullSeTextoNullOuZero()
        };
        
        var rows = await _connection.QueryAsync<dynamic>(sql, parametros);
        var result = ConverterDynamicResultParaListaDeNotasFiscais(rows);
        var resultFiltrado = result.Where(x => x.EmUso == false).ToList();
        return resultFiltrado;
    }

    public async Task<List<RubricaColaboradorLiberacaoNfDTO>> ListarRubricasColaboradorLiberacaoNfPorListaDeIds(List<Guid> ids)
    {
        var sql = DefaultSql + " WHERE trclf.id IN @Ids";

        var parametros = new { Ids = ids };

        var rows = await _connection.QueryAsync(sql, parametros);

        var result = ConverterDynamicResultParaListaDeNotasFiscais(rows);

        return result;
    }

    private List<RubricaColaboradorLiberacaoNfDTO> ConverterDynamicResultParaListaDeNotasFiscais(IEnumerable<dynamic> dynamicObject)
    {
        var result = dynamicObject
            .GroupBy(r => (Guid)r.Id)
            .Select(g =>
            {
                dynamic first = g.First();

                return new RubricaColaboradorLiberacaoNfDTO
                {
                    Id = first.Id,
                    EmUso = first.EmUso == 1 ? true : false ,
                    Valor = first.Valor,
                    Descricao = first.Descricao,
                    RubricaColaboradorId = first.RubricaColaboradorId,
                    VigenciaMes = first.VigenciaMes,
                    VigenciaAno = first.VigenciaAno,
                    Natureza = first.Natureza,
                    NomeColaborador = first.NomeCompleto,
                    CodigoInternoColaborador = first.CodigoInternoColaborador,
                    NotasFiscais = g
                        .Where(x => x.StatusNotaFiscal != null)
                        .Select(x => new NotaFiscalInfoDTO
                        {
                            StatusNotaFiscal = (int?)x.StatusNotaFiscal,
                            NumeroNotaFiscal = (string)x.NumeroNotaFiscal,
                            StatusDescricao = (string)x.StatusDescricao
                        })
                        .Distinct()
                        .ToList()
                };
            })
            .ToList();

        return result;
    }
}