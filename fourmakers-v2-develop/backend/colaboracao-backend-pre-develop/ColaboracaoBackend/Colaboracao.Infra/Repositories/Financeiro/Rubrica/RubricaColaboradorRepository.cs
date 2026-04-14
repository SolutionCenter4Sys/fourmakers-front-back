using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Core.Domain.Financeiro.Rubrica;
using Dapper;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaborador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Helper.Extension;
using DataTransferObject.Domain.Apontamento;
using System.Linq.Expressions;

namespace Colaboracao.Infra.Repositories.Financeiro.Financeiro.Rubrica
{
    public class RubricaColaboradorRepository : IRubricaColaboradorRepository
    {
        private readonly IDBConnection _dapperConnection;

        public RubricaColaboradorRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        private string SELECT_DEFAULT => @"
            SELECT 
                trc.id AS Id,
                trc.ativo AS Ativo,
                trc.codigo_interno_colaborador_alteracao AS CodigoInternoColaboradorAlteracao,
                trc.codigo_interno_colaborador_criacao AS CodigoInternoColaboradorCriacao,
                trc.valor AS Valor,
                trc.percentual AS Percentual,
                trc.tb_rubrica_id AS RubricaId,
                trc.codigo_interno_colaborador AS CodigoInternoColaborador,
                trc.codigo_rubrica_frequencia AS CodigoRubricaFrequencia,
                trc.observacao as Observacao,
                trc.hora AS Hora,
                trc.data_criacao AS DataCriacao,
                trc.data_alteracao AS DataAlteracao,
                tr.descricao AS Descricao,
                tc.nome_completo AS NomeColaborador,
                tr.calculo_tipo AS TipoCalculo,
                trf.descricao AS FrequenciaDescricao,
                trc.mes_inicial AS MesInicial,
                trc.mes_final AS MesFinal,
                trc.ano_inicial AS AnoInicial,
                trc.ano_final AS AnoFinal,
                trc.codigo_carga_rubrica AS CodigoCargaRubrica,
                CONCAT(
                    LPAD(trc.mes_inicial, 2, '0'), '/', trc.ano_inicial,
                    IF(
                        trc.mes_final IS NOT NULL AND trc.ano_final IS NOT NULL,
                        CONCAT(' - ', LPAD(trc.mes_final, 2, '0'), '/', trc.ano_final),
                        ''
                    )
                ) AS PeriodoVigencia
            FROM 
                tb_rubrica_colaborador trc
            INNER JOIN tb_rubrica tr
                ON tr.id = trc.tb_rubrica_id
            INNER JOIN tb_rubrica_frequencia trf
                ON trf.codigo_rubrica_frequencia = trc.codigo_rubrica_frequencia
            INNER JOIN tb_colaborador tc 
                ON tc.codigo_interno_colaborador = trc.codigo_interno_colaborador
            INNER JOIN tb_colaborador_org tco
                ON tco.codigo_interno_colaborador = trc.codigo_interno_colaborador
            LEFT JOIN tb_modelo_contratacao_org tmco ON tmco.codigo_modelo_contratacao = tco.codigo_modelo_contratacao AND tmco.tb_org_id = tco.tb_org_id
            WHERE 
                trc.ativo = 1";

        
            public static string FILTRO_DATA_REF_DEFAULT = @"(
                                                            -- Se @DataRef for NULL, ignora o filtro de período
                                                            (@DataRef IS NULL)

                                                            OR

                                                            -- Caso UNICA
                                                            (trc.codigo_rubrica_frequencia = 'UNICA'
                                                                AND STR_TO_DATE(CONCAT(trc.ano_inicial, '-', trc.mes_inicial, '-01'), '%Y-%m-%d') = @DataRef
                                                            )

                                                            OR

                                                            -- Caso MENSAL_SEM_VIGENCIA_FINAL
                                                            (trc.codigo_rubrica_frequencia = 'MENSAL_SEM_VIGENCIA_FINAL'
                                                                AND STR_TO_DATE(CONCAT(trc.ano_inicial, '-', trc.mes_inicial, '-01'), '%Y-%m-%d') <= @DataRef
                                                            )

                                                            OR

                                                            -- Caso MENSAL_COM_VIGENCIA_FINAL
                                                            (trc.codigo_rubrica_frequencia = 'MENSAL_COM_VIGENCIA_FINAL'
                                                                AND @DataRef BETWEEN
                                                                    STR_TO_DATE(CONCAT(trc.ano_inicial, '-', trc.mes_inicial, '-01'), '%Y-%m-%d')
                                                                    AND STR_TO_DATE(CONCAT(trc.ano_final, '-', trc.mes_final, '-01'), '%Y-%m-%d')
                                                            )
                                                       )";

        public async Task<IEnumerable<RubricaColaboradorResult>> ListarRubricasColaboradorAsync(int orgId, List<string>? diretorias, string? codigoInternoColaborador, int? mes, int? ano, string? rubricaId, int cursor, int limite)
        {
            var connection = _dapperConnection.GetConnection();
            var query = SELECT_DEFAULT;
            
            var inClauseUnidades = diretorias.BuildInClauseOrNull();
            
            query += @$"
                AND tco.tb_org_id = @OrgId
                {(inClauseUnidades == null ? "" : $"AND tco.cod_diretoria IN {inClauseUnidades}")}
                AND (@CodigoInternoColaborador = '' OR trc.codigo_interno_colaborador = @CodigoInternoColaborador)
                AND {FILTRO_DATA_REF_DEFAULT}
                AND (@RubricaId = '' OR trc.tb_rubrica_id = @RubricaId)
                ORDER BY trc.data_criacao DESC
                LIMIT @Cursor, @Limite;
            ";

            var dataRef = MontaDataRef(mes, ano);

            var parametros = new
            {
                OrgId = orgId,
                CodigoInternoColaborador = codigoInternoColaborador ?? "",
                RubricaId = rubricaId ?? "",
                Cursor = cursor,
                Limite = limite,
                DataRef = dataRef
            };

            var result = await connection.QueryAsync<RubricaColaboradorResult>(query, parametros);
            return result;

        }

        public static DateTime? MontaDataRef(int? mes, int? ano)
        {
            DateTime? dataRef = null;

            if (mes.HasValue && ano.HasValue && mes.Value is > 0 and <= 12 && ano.Value > 0)
            {
                dataRef = new DateTime(ano.Value, mes.Value, 1);
            }

            return dataRef;
        }

        public async Task<RubricaColaboradorResult> ObterRubricaColaboradorPorIdAsync(Guid id)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT;
            query += @"
                AND trc.id = @Id
            ";
            return await connection.QueryFirstOrDefaultAsync<RubricaColaboradorResult>(query, new { Id = id});

        }

        public async Task<RubricaColaboradorResult> ObterRubricaColaboradorPorCodigoAsync(string codigo, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND trc.cod_gestor_externo = @Codigo AND trc.tb_org_id = @OrgId";
            return await connection.QueryFirstOrDefaultAsync<RubricaColaboradorResult>(query, new
            {
                Codigo = codigo,
                OrgId = orgId
            });
        }
        
        public async Task<List<RubricaColaboradorResult>> ObterRubricasColaboradorPorListaDeCodigoAsync(List<Guid> ids)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND trc.id IN @Ids";
            
            var result = await connection.QueryAsync<RubricaColaboradorResult>(query, new
            {
                Ids = ids
            });
            return result.ToList();
        }

        public async Task<RubricaColaboradorResult> InserirRubricaColaboradorAsync(RubricaColaboradorInput input)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                        INSERT INTO tb_rubrica_colaborador 
                            (id, ativo, codigo_interno_colaborador_alteracao, codigo_interno_colaborador_criacao, valor, percentual, hora, tb_rubrica_id, codigo_interno_colaborador, codigo_rubrica_frequencia, observacao, mes_inicial, mes_final, ano_inicial, ano_final, codigo_carga_rubrica)
                        VALUES 
                            (@Id, @Ativo, @CodigoInternoColaboradorAlteracao, @CodigoInternoColaboradorAlteracao, @Valor, @Percentual, @Hora, @RubricaId, @CodigoInternoColaborador, @CodigoRubricaFrequencia, @Observacao, @MesInicial, @MesFinal, @AnoInicial, @AnoFinal, @CodigoCargaRubrica)";

            var parameters = new
            {
                input.Id,
                input.Ativo,
                input.CodigoInternoColaboradorAlteracao,
                input.Valor,
                input.Percentual,
                input.Hora,
                input.RubricaId,
                input.CodigoInternoColaborador,
                input.CodigoRubricaFrequencia,
                input.Observacao,
                input.MesInicial,
                input.MesFinal,
                input.AnoInicial,
                input.AnoFinal,
                input.CodigoCargaRubrica
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
            if (rowsAffected > 0)
            {
                return await ObterRubricaColaboradorPorIdAsync(input.Id);
            }

            return null;

        }

        public async Task<RubricaColaboradorResult> AtualizarRubricaColaboradorAsync(RubricaColaboradorInput input)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                        UPDATE 
                            tb_rubrica_colaborador 
                        SET 
                            valor = @Valor,
                            percentual = @Percentual,
                            hora = @Hora,
                            tb_rubrica_id = @RubricaId,
                            codigo_interno_colaborador = @CodigoInternoColaborador,
                            codigo_rubrica_frequencia = @CodigoRubricaFrequencia,
                            codigo_interno_colaborador_alteracao = @CodigoInternoColaboradorAlteracao,
                            observacao = @Observacao,
                            mes_inicial = @MesInicial,
                            mes_final = @MesFinal,
                            ano_inicial = @AnoInicial,
                            ano_final = @AnoFinal,
                            codigo_carga_rubrica = @CodigoCargaRubrica
                        WHERE
                            id = @Id";

            var parameters = new
            {
                input.Id,
                input.Valor,
                input.Percentual,
                input.Hora,
                input.RubricaId,
                input.CodigoInternoColaborador,
                input.CodigoRubricaFrequencia,
                input.CodigoInternoColaboradorAlteracao,
                input.Observacao,
                input.MesInicial,
                input.MesFinal,
                input.AnoInicial,
                input.AnoFinal,
                input.CodigoCargaRubrica
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
            if (rowsAffected > 0)
            {
                return await ObterRubricaColaboradorPorIdAsync(input.Id);
            }

            return null;

        }

        public async Task<bool> DeletarRubricaColaboradorAsync(Guid id)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"UPDATE 
                             tb_rubrica_colaborador
                          SET
                             ativo = @Ativo
                          WHERE
                             id = @Id";

            int rowsAffected = await connection.ExecuteAsync(query, new { Id = id, Ativo = false });
            return rowsAffected > 0;
        }

        public async Task<DateTime?> ObterDataVigenciaPorId(Guid vigenciaId)
        {
            var connection = _dapperConnection.GetConnection();

            string sql = @$"
                                SELECT
                                    ano,
                                    mes
                                FROM
                                    tb_vigencia
                                WHERE
                                    id = @VigenciaId;
                            ";

            var result = await connection.QueryFirstOrDefaultAsync<(int Ano, int Mes)?>(sql, new { VigenciaId = vigenciaId });

            if (result.HasValue)
            {
                return new DateTime(result.Value.Ano, result.Value.Mes, 1);
            }

            return null;
        }
        
        public async Task<Guid?> ObterIdVigenciaPorMesEAno(int mes, int ano)
        {
            var connection = _dapperConnection.GetConnection();

            string sql = @$"
                                SELECT
                                    id
                                FROM
                                    tb_vigencia
                                WHERE
                                    mes = @Mes
                                    ano = @Ano;
                            ";

            var result = await connection.QueryFirstOrDefaultAsync<Guid?>(sql, new { Mes = mes, Ano = ano });
            return result;
        }

        public async Task<string> ObterColaboradorPorCodigo(Guid codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();

            string sql = @$"
                                SELECT
                                    codigo_interno_colaborador
                                FROM
                                    tb_colaborador
                                WHERE
                                    codigo_interno_colaborador = @CodigoInternoColaborador;
                            ";

            var result = await connection.QueryFirstOrDefaultAsync<string>(sql, new { CodigoInternoColaborador = codigoInternoColaborador });

            return result;
            
        }

        public async Task<List<VigenciaDTO>> ListarMesEAnosLancadosPorOrgId(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT 
                    trc.mes_inicial as Mes,
                    trc.ano_inicial as Ano,
                    CONCAT(LPAD(trc.mes_inicial, 2, '0'), '/', trc.ano_inicial) AS Label
                FROM tb_rubrica_colaborador trc 
                INNER JOIN tb_colaborador_org tco 
                    ON trc.codigo_interno_colaborador = tco.codigo_interno_colaborador AND tco.tb_org_id = @OrgId
                GROUP BY 
                    trc.mes_inicial,
                    trc.ano_inicial;";
            var result = await connection.QueryAsync<VigenciaDTO>(query, new { OrgId = orgId });
            return result.ToList();
        }

        public async Task<IEnumerable<RubricaColaboradorDetalhadoDTO>> ListarRubricasColaboradorDetalhadoAsync(int mes, int ano, int orgId)
        {

            var connection = _dapperConnection.GetConnection();
            var query = @$"
                SELECT 
                    trc.id AS Id,
                    trc.valor AS Valor,
                    trc.percentual AS Percentual,
                    trc.hora AS Hora,
                    trc.codigo_interno_colaborador AS CodigoInternoColaborador,
                    trc.codigo_rubrica_frequencia AS CodigoRubricaFrequencia,
                    tr.id AS RubricaId,
                    tr.descricao AS RubricaDescricao,
                    tr.rubrica_tipo AS Descricao,
                    tr.codigo_rubrica AS CodigoRubrica,
                    LOWER(tr.rubrica_tipo) AS CodigoRubricaTipo,
                    CASE 
                        WHEN tr.rubrica_tipo = 'Desconto' THEN 'DEBITO'
                        WHEN tr.rubrica_tipo = 'Provento' THEN 'CREDITO'
                        ELSE 'DEBITO'
                    END AS Natureza
                FROM 
                    tb_rubrica_colaborador trc
                INNER JOIN tb_rubrica tr
                    ON tr.id = trc.tb_rubrica_id
                WHERE 
                    trc.ativo = 1
                    AND tr.tb_org_id = @OrgId
                    AND {FILTRO_DATA_REF_DEFAULT}
                ORDER BY 
                    trc.codigo_interno_colaborador, tr.descricao";


            var dataRef = MontaDataRef(mes, ano);

            var rawResult = await connection.QueryAsync(query, new { DataRef = dataRef, OrgId = orgId });

            var result = new List<RubricaColaboradorDetalhadoDTO>();

            foreach (var row in rawResult)
            {
                var item = new RubricaColaboradorDetalhadoDTO
                {
                    Id = row.Id.ToString(),
                    Valor = row.Valor,
                    Percentual = row.Percentual,
                    Hora = row.Hora,
                    CodigInternoColaborador = row.CodigoInternoColaborador.ToString(),
                    CodigoRubricaFrequencia = row.CodigoRubricaFrequencia,
                    Rubrica = new RubricaInfoDTO
                    {
                        RubricaId = row.RubricaId.ToString(),
                        RubricaDescricao = row.RubricaDescricao,
                        CodigoRubrica = row.CodigoRubrica,
                        RubricaTipo = new RubricaTipoInfoDTO
                        {
                            Descricao = row.Descricao,
                            CodigoRubricaTipo = row.CodigoRubricaTipo,
                            Natureza = row.Natureza
                        }
                    }
                };

                result.Add(item);
            }

            return result;
        }
        
        public async Task<IEnumerable<RubricaColaboradorResult>> ListarRubricasContabeisColaboradorPorVigenciaAsync(int mes, int ano, string? codigoDiretoria, int orgId)
        {
            
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT;
            query += @$"
                 AND tr.tb_org_id = @OrgId
                 AND tr.refletir_contabil = 1
                 AND tmco.deve_criar_nf = 1
                 AND (@CodigoDiretoria IS NULL OR tco.cod_diretoria = @CodigoDiretoria)
                 AND {FILTRO_DATA_REF_DEFAULT}
            ";

            var dataRef = MontaDataRef(mes, ano);

            return await connection.QueryAsync<RubricaColaboradorResult>(query, new { Mes = mes, Ano = ano, CodigoDiretoria = codigoDiretoria, OrgId = orgId, DataRef = dataRef });
        }

        public async Task<bool> ValidaSePodeRefletirEmissaoDeRubricaNFColaborador(string codigoInternoColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT 
                    tmco.deve_criar_nf
                FROM tb_colaborador_org tco
                INNER JOIN tb_modelo_contratacao_org tmco 
                    ON tmco.codigo_modelo_contratacao = tco.codigo_modelo_contratacao
                    AND tmco.tb_org_id = @OrgId
                WHERE 
                    tco.codigo_interno_colaborador = @CodigoInternoColaborador
                    AND tco.tb_org_id = @OrgId";

            var result = await connection.QueryFirstOrDefaultAsync<bool>(
                query, 
                new { CodigoInternoColaborador = codigoInternoColaborador, OrgId = orgId }
            );

            return result;
        }
    }
    
}