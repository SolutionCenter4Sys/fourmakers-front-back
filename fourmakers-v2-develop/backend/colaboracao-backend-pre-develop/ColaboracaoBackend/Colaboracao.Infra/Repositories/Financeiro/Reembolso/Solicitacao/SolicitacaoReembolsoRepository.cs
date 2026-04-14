using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Core.Domain.Reembolso.Solicitacao;
using Dapper;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao.Aprovacao;

namespace Colaboracao.Infra.Repositories.Financeiro.Reembolso.Solicitacao;

public class SolicitacaoReembolsoRepository(IDBConnection dapperConnection) : ISolicitacaoReembolsoRepository
{
    public async Task<List<SolicitacaoReembolsoColaboradorDTO>> ListarPorColabAsync(string codigoInternoColaborador, int orgId, DateTime? dataInicial, DateTime? dataFinal)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            SELECT 
                tsr.id as Id,
                tsr.tb_cliente_id AS ClienteId,
                tco.nome_cliente AS ClienteDescricao,
                tsr.tb_projeto_id AS ProjetoId,
                tpo.projeto AS ProjetoDescricao,
                tv.categoria AS Categoria,
                tsr.valor AS ValorSolicitado,
                tsr.data_criacao AS Data,
                tsr.data_aprovacao AS DataAprovacao,
                tsr.status_id AS StatusId,
                tss.descricao AS Status,
                tsd.id AS DocumentoId,
                tsd.tipo AS Tipo,
                tsd.url AS Url,
                tsr.valor_aprovado AS ValorAprovado,
                tsr.observacao AS Observacao,
                tsr.objetivo As Objetivo,
                tsr.destino AS Destino,
                tsr.data_inicio AS DataInicio,
                tsr.data_fim AS DataFim
            FROM tb_solicitacao_reembolso tsr 
            INNER JOIN tb_verba tv ON tv.id = tsr.tb_verba_id
            INNER JOIN tb_projeto_org tpo ON tpo.cod_projeto = tsr.tb_projeto_id AND tpo.tb_org_id = tsr.tb_org_id
            INNER JOIN tb_cliente_org tco ON tco.codigo_cliente = tsr.tb_cliente_id AND tco.tb_org_id = tsr.tb_org_id
            INNER JOIN tb_solicitacao_status tss ON tss.id = tsr.status_id
            LEFT JOIN tb_solicitacao_documento tsd ON tsd.tb_solicitacao_reembolso_id = tsr.id
            WHERE 
                tsr.codigo_interno_colaborador = @CodigoInternoColaborador
                AND tsr.tb_org_id = @OrgId
                AND (@DataInicio IS NULL OR DATE(tsr.data_criacao) >= DATE(@DataInicio))
                AND (@DataFinal IS NULL OR DATE(tsr.data_criacao) <= DATE(@DataFinal))
            ORDER BY 
                tsr.data_criacao DESC
        ";
        var parametros = new
        {
            CodigoInternoColaborador = codigoInternoColaborador,
            OrgId = orgId,
            DataInicio = dataInicial,
            DataFinal = dataFinal
        };

        var result = await connection.QueryAsync<dynamic>(query, parametros);
        
        var groupedList = result.GroupBy(x => new
        {
            x.Id,
            x.ClienteId,
            x.ClienteDescricao,
            x.ProjetoId,
            x.ProjetoDescricao,
            x.Categoria,
            x.ValorSolicitado,
            x.Data,
            x.StatusId,
            x.Status,
            x.DataAprovacao,
            x.ValorAprovado,
            x.Observacao,
            x.Objetivo,
            x.Destino,
            x.DataInicio,
            x.DataFim
        }).Select(x => new SolicitacaoReembolsoColaboradorDTO
        {
            Id = x.Key.Id,
            ClienteId = x.Key.ClienteId,
            ClienteDescricao = x.Key.ClienteDescricao,
            ProjetoId = x.Key.ProjetoId,
            ProjetoDescricao = x.Key.ProjetoDescricao,
            Categoria = x.Key.Categoria,
            ValorSolicitado = x.Key.ValorSolicitado,
            Data = x.Key.Data,
            StatusId = x.Key.StatusId,
            Status = x.Key.Status,
            DataAprovacao = x.Key.DataAprovacao,
            ValorAprovado = x.Key.ValorAprovado,
            Observacao = x.Key.Observacao,
            Objetivo = x.Key.Objetivo,
            Destino = x.Key.Destino,
            DataInicio = x.Key.DataInicio,
            DataFim = x.Key.DataFim,
            SolicitacaoDocumentos = x
                .GroupBy(group => new
                {
                    group.DocumentoId,
                    group.Tipo,
                    group.Url
                })
                .Where(g => g.Key.DocumentoId != null)
                .Select(g => new SolicitacaoDocumentoDTO
                {
                    Id = g.Key.DocumentoId,
                    Url = g.Key.Url,
                    Tipo = g.Key.Tipo
                })
                .ToList()
        }).ToList();
        
        return groupedList;

    }

    public async Task<int> InserirAsync(int orgId, string? projetoId, string? clienteId, int verbaId, string codigoInternoColaborador, string descricao, DateTime dataDespesa, decimal valor, decimal? valorUnidade, int? quantidade, int statusId, string objetivo, string? destino, DateTime dataInicio, DateTime dataFim)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            INSERT INTO tb_solicitacao_reembolso (
                tb_org_id,
                tb_projeto_id,
                tb_cliente_id,
                tb_verba_id,
                codigo_interno_colaborador,
                descricao,
                data_despesa,
                valor,
                valor_unidade,
                quantidade,
                status_id,
                codigo_interno_colaborador_criacao,
                codigo_interno_colaborador_edicao,
                objetivo,
                destino,
                data_inicio,
                data_fim
            )
            VALUES(
                   @OrgId,
                   @ProjetoID,
                   @ClienteID,
                   @VerbaID,
                   @CodigoInternoColaborador,
                   @Descricao,
                   @DataDespesa,
                   @Valor,
                   @ValorUnidade,
                   @Quantidade,
                   @StatusId,
                   @CodigoInternoColaborador,
                   @CodigoInternoColaborador,
                   @Objetivo,
                   @Destino,
                   @DataInicio,
                   @DataFim
            );

        SELECT LAST_INSERT_ID();
        ";

        var parametros = new
        {
            OrgId = orgId,
            ProjetoID = projetoId?.ToNullSeTextoNull() == null ? null : projetoId.ToNullSeTextoNull(),
            ClienteID = clienteId?.ToNullSeTextoNull() == null? null : clienteId.ToNullSeTextoNull(),
            VerbaID = verbaId,
            CodigoInternoColaborador = codigoInternoColaborador,
            Descricao = descricao,
            DataDespesa = dataDespesa,
            Valor = valor,
            ValorUnidade = valorUnidade,
            Quantidade = quantidade,
            StatusId = statusId,
            Objetivo = objetivo,
            Destino = destino,
            DataInicio = dataInicio,
            DataFim = dataFim
        };
        
        var result = await connection.ExecuteScalarAsync<int>(query, parametros);
        return result;
    }

    public async Task<List<SolicitacaoReembolsoGerenteDTO>> ListarSolicitacoesGerenteProjeto(string filtro, string clienteId, string projetoId, string dataInicio, string dataFim, string codigoColaborador, int orgId, int? statusId)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            SELECT 
                tsr.id as Id,
                tc.codigo_interno_colaborador AS CodigoInternoColaborador,
                tc.nome_completo AS Colaborador,
                tco.nome_cliente AS ClienteDescricao,
                tpo.projeto AS ProjetoDescricao,
                tsr.valor AS Valor,
                tsr.observacao AS Observacao,
                tss.descricao AS Status,
                tsr.objetivo As Objetivo,
                tsr.destino AS Destino,
                tsr.data_inicio AS DataInicio,
                tsr.data_fim AS DataFim,
                tsr.data_criacao AS DataSolicitacao,
                tsd.id AS DocumentoId,
                tsd.tipo AS Tipo,
                tsd.url AS Url
            FROM tb_solicitacao_reembolso tsr 
            INNER JOIN tb_colaborador tc 
                ON tc.codigo_interno_colaborador = tsr.codigo_interno_colaborador
            INNER JOIN tb_colaborador_org tco_solicitante
            ON tco_solicitante.codigo_interno_colaborador = tc.codigo_interno_colaborador
            INNER JOIN tb_colaborador_org tco2 
                ON tco2.codigo_interno_colaborador = @CodColaborador 
                    AND tco2.tb_org_id = @OrgId
            INNER JOIN tb_projeto_org tpo 
                ON tpo.cod_projeto = tsr.tb_projeto_id and tsr.tb_org_id = tsr.tb_org_id
            LEFT JOIN tb_projeto_gerente tpg -- traz se eu for aprovador
                ON tpg.cod_projeto = tsr.tb_projeto_id 
                   AND tpg.cod_colaborador_gerente = tco2.cod_colaborador_externo 
                   AND tpg.tb_org_id = tpo.tb_org_id
            LEFT JOIN tb_colaborador_hierarquia tch -- trazer se eu for um gestor externo
                ON tch.cod_colaborador_externo = tco_solicitante.cod_colaborador_externo
                AND tch.cod_colaborador_superior = tco2.cod_colaborador_externo
                AND tch.tb_org_id = tsr.tb_org_id
            INNER JOIN tb_cliente_org tco 
                ON tco.codigo_cliente = tsr.tb_cliente_id 
                    AND tco.tb_org_id = tsr.tb_org_id
            INNER JOIN tb_solicitacao_status tss ON tss.id = tsr.status_id
            LEFT JOIN tb_solicitacao_documento tsd
                ON tsd.tb_solicitacao_reembolso_id = tsr.id
            -- Sem GROUP BY tsr.id: múltiplas linhas (uma por documento) são agrupadas no C# para preencher SolicitacaoDocumentos (mesmo padrão de ListarSolicitacoesAprovacaoPorColaborador).
            WHERE 
                tsr.tb_org_id = @OrgId
                AND (@ClienteId IS NULL OR @ClienteId = '' OR tsr.tb_cliente_id = @ClienteId)
                AND (@ProjetoId IS NULL OR @ProjetoId = '' OR tsr.tb_projeto_id = @ProjetoId)
                AND (@DataInicio IS NULL OR @DataInicio = '' OR tsr.data_criacao >= @DataInicio)
                AND (@DataFim IS NULL OR @DataFim = '' OR tsr.data_criacao <= @DataFim)
                AND (@StatusId IS NULL OR @StatusId = 0 OR tsr.status_id = @StatusId)
               AND (
                    -- SE sou gerente do projeto OU sou gestor na hierarquia
                    tpg.cod_projeto IS NOT NULL
                    OR tch.cod_colaborador_externo IS NOT NULL
                )
                AND (
                    @Filtro IS NULL OR @Filtro = '' OR
                    LOWER(tc.nome_completo) LIKE LOWER(CONCAT('%', @Filtro, '%')) OR
                    LOWER(tco.nome_cliente) LIKE LOWER(CONCAT('%', @Filtro, '%')) OR
                    LOWER(tpo.projeto) LIKE LOWER(CONCAT('%', @Filtro, '%')) OR
                    LOWER(tss.descricao) LIKE LOWER(CONCAT('%', @Filtro, '%'))
                )
            ORDER BY tsr.data_criacao DESC;
        ";

        var parametros = new
        {
            OrgId = orgId,
            CodColaborador = codigoColaborador,
            Filtro = string.IsNullOrWhiteSpace(filtro) ? null : filtro,
            ClienteId = string.IsNullOrWhiteSpace(clienteId) ? null : clienteId,
            ProjetoId = string.IsNullOrWhiteSpace(projetoId) ? null : projetoId,
            DataInicio = string.IsNullOrWhiteSpace(dataInicio) ? null : dataInicio,
            DataFim = string.IsNullOrWhiteSpace(dataFim) ? null : dataFim,
            StatusId = statusId
        };

        var result = await connection.QueryAsync<dynamic>(query, parametros);
        
        var groupedList = result.GroupBy(x => new
        {
            x.Id,
            x.Valor,
            x.DataSolicitacao,
            x.Objetivo,
            x.Destino,
            x.DataInicio,
            x.DataFim,
            x.Colaborador,
            x.ClienteDescricao,
            x.ProjetoDescricao,
            x.CodigoInternoColaborador,
            x.Observacao,
            x.Status
        }).Select(x => new SolicitacaoReembolsoGerenteDTO
        {
            Id = x.Key.Id,
            Valor = x.Key.Valor.ToString(),
            ClienteDescricao = x.Key.ClienteDescricao,
            ProjetoDescricao = x.Key.ProjetoDescricao,
            Colaborador = x.Key.Colaborador,
            DataFim = x.Key.DataFim,
            DataInicio = x.Key.DataInicio,
            DataSolicitacao =  x.Key.DataSolicitacao,
            Destino = x.Key.Destino,
            Objetivo = x.Key.Objetivo,
            Observacao = x.Key.Observacao,
            Status = x.Key.Status,
            CodigoInternoColaborador = x.Key.CodigoInternoColaborador,
            SolicitacaoDocumentos = x
                .GroupBy(group => new
                {
                    group.DocumentoId,
                    group.Tipo,
                    group.Url
                })
                .Where(g => g.Key.DocumentoId != null)
                .Select(g => new SolicitacaoDocumentoDTO
                {
                    Id = g.Key.DocumentoId,
                    Url = g.Key.Url,
                    Tipo = g.Key.Tipo
                })
                .ToList()
        }).ToList();
        
        return groupedList.ToList();
    }
    
    public async Task<List<SolicitacaoColaboradorAprovacaoDTO>> ListarSolicitacoesAprovacaoPorColaborador(string clienteId, string projetoId, string codigoGerente, string codigoColaborador, int orgId)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            SELECT 
               tsr.id AS Id,
               tsr.tb_verba_id AS CategoriaId,
               tv.categoria AS CategoriaDescricao,
               tsr.data_despesa AS DataDespesa,
               tsr.valor AS Valor,
               tsr.descricao AS Descricao,
               tsd.id AS DocumentoId,
               tsd.tipo AS Tipo,
               tsd.url AS Url,
               tsr.status_id AS StatusId,
               tss.descricao AS DescricaoStatus,
               tsr.data_criacao AS DataSolicitacao,
               tsr.objetivo As Objetivo,
               tsr.destino AS Destino,
               tsr.data_inicio AS DataInicio,
               tsr.data_fim AS DataFim,
               tc.nome_completo AS Colaborador,
               tco.nome_cliente AS ClienteDescricao,
               tpo.projeto AS ProjetoDescricao
            FROM tb_solicitacao_reembolso tsr 
            INNER JOIN tb_verba tv
                ON tv.id = tsr.tb_verba_id
            INNER JOIN tb_colaborador tc 
                ON tc.codigo_interno_colaborador = tsr.codigo_interno_colaborador
            INNER JOIN tb_colaborador_org tco_solicitante
                ON tco_solicitante.codigo_interno_colaborador = tc.codigo_interno_colaborador
            INNER JOIN tb_colaborador_org tco2 
                ON tco2.codigo_interno_colaborador = @CodGerente 
                    AND tco2.tb_org_id = @OrgId
            INNER JOIN tb_projeto_org tpo 
                ON tpo.cod_projeto = tsr.tb_projeto_id and tpo.tb_org_id = tsr.tb_org_id
            LEFT JOIN tb_projeto_gerente tpg 
                ON tpg.cod_projeto = tsr.tb_projeto_id 
                   AND tpg.cod_colaborador_gerente = tco2.cod_colaborador_externo 
                   AND tpg.tb_org_id = tpo.tb_org_id
            LEFT JOIN tb_colaborador_hierarquia tch -- trazer se eu for um gestor externo
                    ON tch.cod_colaborador_externo = tco_solicitante.cod_colaborador_externo
                    AND tch.cod_colaborador_superior = tco2.cod_colaborador_externo
                    AND tch.tb_org_id = tsr.tb_org_id
            INNER JOIN tb_cliente_org tco 
                ON tco.codigo_cliente = tsr.tb_cliente_id 
                    AND tco.tb_org_id = tsr.tb_org_id
            INNER JOIN tb_solicitacao_status tss ON tss.id = tsr.status_id
            LEFT JOIN tb_solicitacao_documento tsd
                ON tsd.tb_solicitacao_reembolso_id = tsr.id
            WHERE 
                tsr.tb_org_id = @OrgId
                AND tsr.codigo_interno_colaborador = @CodColaborador
                AND (
                    -- SE sou gerente do projeto OU sou gestor na hierarquia
                    tpg.cod_projeto IS NOT NULL
                    OR tch.cod_colaborador_externo IS NOT NULL
                )
                AND (@ClienteId IS NULL OR @ClienteId = '' OR tsr.tb_cliente_id = @ClienteId)
                AND (@ProjetoId IS NULL OR @ProjetoId = '' OR tsr.tb_projeto_id = @ProjetoId)
                
            ORDER BY tsr.data_criacao DESC
        ";

        var parametros = new
        {
            OrgId = orgId,
            CodGerente = codigoGerente,
            CodColaborador = codigoColaborador,
            ClienteId = string.IsNullOrWhiteSpace(clienteId) ? null : clienteId,
            ProjetoId = string.IsNullOrWhiteSpace(projetoId) ? null : projetoId,
        };

        var result = await connection.QueryAsync<dynamic>(query, parametros);

        var groupedList = result.GroupBy(x => new
        {
            x.Id,
            x.CategoriaId,
            x.CategoriaDescricao,
            x.DataDespesa,
            x.Valor,
            x.Descricao,
            x.StatusId,
            x.DescricaoStatus,
            x.DataSolicitacao,
            x.Objetivo,
            x.Destino,
            x.DataInicio,
            x.DataFim,
            x.Colaborador,
            x.ClienteDescricao,
            x.ProjetoDescricao
        }).Select(x => new SolicitacaoColaboradorAprovacaoDTO
        {
            Id = x.Key.Id,
            CategoriaId = x.Key.CategoriaId,
            CategoriaDescricao = x.Key.CategoriaDescricao,
            DataDespesa = x.Key.DataDespesa,
            Valor = x.Key.Valor,
            Descricao = x.Key.Descricao,
            StatusId = x.Key.StatusId,
            StatusDescricao = x.Key.DescricaoStatus,
            ClienteDescricao = x.Key.ClienteDescricao,
            ProjetoDescricao = x.Key.ProjetoDescricao,
            Colaborador = x.Key.Colaborador,
            DataFim = x.Key.DataFim,
            DataInicio = x.Key.DataInicio,
            DataSolicitacao =  x.Key.DataSolicitacao,
            Destino = x.Key.Destino,
            Objetivo = x.Key.Objetivo,
            SolicitacaoDocumentos = x
                .GroupBy(group => new
                {
                    group.DocumentoId,
                    group.Tipo,
                    group.Url
                })
                .Where(g => g.Key.DocumentoId != null)
                .Select(g => new SolicitacaoDocumentoDTO
                {
                    Id = g.Key.DocumentoId,
                    Url = g.Key.Url,
                    Tipo = g.Key.Tipo
                })
                .ToList()
        }).ToList();
        
        return groupedList;
    }

    public async Task<SolicitacaoBigNumberDTO> SolicitacaoBigNumbersAsync(string codColaborador, int orgId)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            SELECT
                COUNT(DISTINCT tsr.codigo_interno_colaborador) AS QtdColaborador,
                SUM(valor) AS ValoresLancados,
                SUM(CASE WHEN tsr.status_id = 3 THEN tsr.valor ELSE 0 END) AS ValoresAprovados,
                SUM(CASE WHEN tsr.status_id = 2 THEN tsr.valor ELSE 0 END) AS ValoresReprovados,
                SUM(CASE WHEN tsr.status_id = 1 THEN tsr.valor ELSE 0 END) AS ValoresPendentes
            FROM tb_solicitacao_reembolso tsr
            INNER JOIN tb_colaborador_org tco2 
                ON tco2.codigo_interno_colaborador = @CodColaborador 
                    AND tco2.tb_org_id = @OrgId
            INNER JOIN tb_projeto_org tpo 
                ON tpo.cod_projeto = tsr.tb_projeto_id and tsr.tb_org_id = tsr.tb_org_id
            INNER JOIN tb_projeto_gerente tpg 
                ON tpg.cod_projeto = tsr.tb_projeto_id 
                   AND tpg.cod_colaborador_gerente = tco2.cod_colaborador_externo 
                   AND tpg.tb_org_id = tpo.tb_org_id
            WHERE 
                tsr.tb_org_id = @OrgId
        ";

        var parametros = new
        {
            OrgId = orgId,
            CodColaborador = codColaborador,
        };

        var result = await connection.QueryFirstAsync<SolicitacaoBigNumberDTO>(query, parametros);
        return result;
    }
    
    public async Task AprovarSolicitacao(int solicitacaoId, string codAprovador, decimal valorAprovado, SolicitacaoStatusEnum status = SolicitacaoStatusEnum.Aprovado)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            UPDATE tb_solicitacao_reembolso tsr
            SET
                tsr.status_id = @Status,
                tsr.codigo_interno_colaborador_aprovador = @CodAprovador,
                tsr.codigo_interno_colaborador_edicao = @CodAprovador,
                tsr.data_aprovacao = @DataAprovacao,
                tsr.valor_aprovado = @ValorAprovado
            WHERE 
                tsr.id = @SolicitacaoId
        ";


        var parametros = new
        {
            CodAprovador = codAprovador,
            SolicitacaoId = solicitacaoId,
            DataAprovacao = DateTime.UtcNow,
            ValorAprovado = valorAprovado,
            Status = status
        };
            
        await connection.ExecuteAsync(query, parametros);
    }
    
    public async Task PagarSolicitacao(int solicitacaoId, string cpfColaboradorPagamento, SolicitacaoStatusEnum status)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            UPDATE tb_solicitacao_reembolso tsr
            SET
                tsr.status_id = @Status,
                tsr.codigo_interno_colaborador_edicao = @CodAprovador
            WHERE 
                tsr.id = @SolicitacaoId
        ";


        var parametros = new
        {
            CodAprovador = cpfColaboradorPagamento,
            SolicitacaoId = solicitacaoId,
            Status = status
        };
            
        await connection.ExecuteAsync(query, parametros);
    }
    
    public async Task ReprovarSolicitacao(int solicitacaoId, string codAprovador, string obs)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            UPDATE tb_solicitacao_reembolso tsr
            SET
                tsr.status_id = 2,
                tsr.codigo_interno_colaborador_aprovador = @CodAprovador,
                tsr.codigo_interno_colaborador_edicao = @CodAprovador,
                tsr.observacao = @Obs
            WHERE 
                tsr.id = @SolicitacaoId
        ";

        var parametros = new
        {
            CodAprovador = codAprovador,
            SolicitacaoId = solicitacaoId,
            Obs = obs
        };
            
        await connection.ExecuteAsync(query, parametros);
    }

    public async Task<SolicitacaoAprovadorDetalheDTO> BuscarSolicitacaoGestorPorId(int solicitacaoId, string codAprovador)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
        SELECT
            tsr.id AS Id,
            tsr.codigo_interno_colaborador AS CodigoColaborador,
            tsr.status_id AS StatusId,
            tsr.valor AS Valor,
            tsr.valor_aprovado AS ValorAprovado,
            tsr.tb_org_id AS OrgId,
            CASE 
                WHEN tpg.cod_projeto IS NOT NULL 
                     OR tch.cod_colaborador_superior IS NOT NULL 
                THEN 1
                ELSE 0
            END AS EhGestorProjeto
        FROM tb_solicitacao_reembolso tsr
        INNER JOIN tb_colaborador_org tco 
            ON tco.codigo_interno_colaborador = tsr.codigo_interno_colaborador
        INNER JOIN tb_colaborador_org tco2 
            ON tco2.codigo_interno_colaborador = @CodAprovador 
                AND tco2.tb_org_id = tsr.tb_org_id
        INNER JOIN tb_projeto_org tpo 
            ON tpo.cod_projeto = tsr.tb_projeto_id AND tpo.tb_org_id = tsr.tb_org_id
        LEFT JOIN tb_projeto_gerente tpg 
            ON tpg.cod_projeto = tsr.tb_projeto_id 
               AND tpg.cod_colaborador_gerente = tco2.cod_colaborador_externo
               AND tpg.tb_org_id = tsr.tb_org_id
        LEFT JOIN tb_colaborador_hierarquia tch
                ON tch.cod_colaborador_superior = tco2.cod_colaborador_externo
                AND tch.cod_colaborador_externo = tco.cod_colaborador_externo
        WHERE
            tsr.id = @SolicitacaoId
    ";

        var parametros = new
        {
            CodAprovador = codAprovador,
            SolicitacaoId = solicitacaoId,
        };

        var result = await connection.QueryFirstOrDefaultAsync<SolicitacaoAprovadorDetalheDTO>(query, parametros);
        return result;
    }
    
    public async Task<bool> VerificaSeEhAprovador(string codigoInternoColaborador, int orgId)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            SELECT 
            CASE 
                WHEN EXISTS (
                    SELECT 1
                    FROM tb_colaborador_org tco 
                    INNER JOIN tb_projeto_gerente tpg 
                        ON tpg.cod_colaborador_gerente = tco.cod_colaborador_externo 
                        AND tpg.tb_org_id = tco.tb_org_id
                    WHERE 
                        tco.tb_org_id = @OrgId
                        AND tco.codigo_interno_colaborador = @CodigoInternoColaborador
                ) THEN 1
                ELSE 0
            END AS Resultado
    ";

        var parametros = new
        {
            OrgId = orgId,
            CodigoInternoColaborador = codigoInternoColaborador,
        };

        var result = await connection.QueryFirstOrDefaultAsync<int>(query, parametros);
        return result == 1;
    }
    
    public async Task<bool> VerificaSeEhGestorAdm(string codigoInternoColaborador, int orgId)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            SELECT 
            CASE 
                WHEN EXISTS (
                    SELECT 1
                    FROM tb_colaborador_org tco 
                    INNER JOIN tb_colaborador_hierarquia tch 
                        ON tch.cod_colaborador_superior = tco.cod_colaborador_externo 
                        AND tch.tb_org_id = tco.tb_org_id
                    WHERE 
                        tco.tb_org_id = @OrgId
                        AND tco.codigo_interno_colaborador = @CodigoInternoColaborador
                ) THEN 1
                ELSE 0
            END AS Resultado
    ";

        var parametros = new
        {
            OrgId = orgId,
            CodigoInternoColaborador = codigoInternoColaborador,
        };

        var result = await connection.QueryFirstOrDefaultAsync<int>(query, parametros);
        return result == 1;
    }

    public async Task<List<SolicitacaoColaboradorVisaoAdmDTO>> ListarSolicitacoesVisaoAdm(int orgId, string codigoCliente, string codigoProjeto, string dataInicio, string dataFinal, string codigoAprovador, int? statusId)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            SELECT 
                reembolso.id AS Id,
                colaborador.codigo_interno_colaborador AS CodigoColaborador,
                colaborador.nome_completo AS NomeColaborador,
                reembolso.valor AS ValorSolicitado,
                reembolso.valor_aprovado AS ValorAprovado,
                reembolso.data_criacao AS DataSolicitacao,
                status.descricao AS Status,
                status.id AS StatusId,
                CONCAT(projeto.cod_projeto , ' - ', projeto.projeto) AS Projeto,
                CONCAT(cliente.codigo_cliente  , ' - ', cliente.nome_cliente) AS Cliente,
                reembolso.observacao AS Observacao,
                reembolso.descricao AS Descricao,
                COUNT(*) OVER (PARTITION BY colaborador.codigo_interno_colaborador) AS TotalDespesas,
                aprovador.nome_completo AS NomeAprovador,
                verba.valor_customizado AS ValorExcecao,
                reembolso.objetivo As Objetivo,
                reembolso.destino AS Destino,
                reembolso.data_inicio AS DataInicio,
                reembolso.data_fim AS DataFim,
                tv.categoria AS Categoria,
                tsd.id AS DocumentoId,
                tsd.tipo AS Tipo,
                tsd.url AS Url
            FROM tb_solicitacao_reembolso reembolso 
            INNER JOIN tb_colaborador colaborador 
                ON colaborador.codigo_interno_colaborador = reembolso.codigo_interno_colaborador
            LEFT JOIN tb_colaborador aprovador
                ON aprovador.codigo_interno_colaborador = reembolso.codigo_interno_colaborador_aprovador
            INNER JOIN tb_projeto_org projeto 
                ON projeto.cod_projeto = reembolso.tb_projeto_id 
                    AND projeto.tb_org_id = reembolso.tb_org_id
            INNER JOIN tb_cliente_org cliente
                ON cliente.codigo_cliente = reembolso.tb_cliente_id
                    AND cliente.tb_org_id = projeto.tb_org_id
            INNER JOIN tb_solicitacao_status status 
                ON status.id = reembolso.status_id
            INNER JOIN tb_verba tv
                ON tv.id = reembolso.tb_verba_id
            LEFT JOIN tb_solicitacao_documento tsd ON tsd.tb_solicitacao_reembolso_id = reembolso.id
            LEFT JOIN LATERAL (
                SELECT 
                    verba_view.valor_customizado,
                    verba_view.custo_cliente
                FROM vw_verba_personalizada_prioritaria verba_view
                WHERE verba_view.tb_verba_id = reembolso.tb_verba_id
                  AND (
                    (verba_view.projeto_id = reembolso.tb_projeto_id AND verba_view.codigo_interno_colaborador = reembolso.codigo_interno_colaborador AND verba_view.cliente_id IS NULL) OR
                    (verba_view.projeto_id = reembolso.tb_projeto_id AND verba_view.codigo_interno_colaborador IS NULL AND verba_view.cliente_id IS NULL) OR
                    (verba_view.cliente_id = cliente.codigo_cliente AND verba_view.codigo_interno_colaborador = reembolso.codigo_interno_colaborador AND verba_view.projeto_id IS NULL) OR
                    (verba_view.cliente_id = cliente.codigo_cliente AND verba_view.codigo_interno_colaborador IS NULL AND verba_view.projeto_id IS NULL) OR
                    (verba_view.codigo_interno_colaborador = reembolso.codigo_interno_colaborador AND verba_view.cliente_id IS NULL AND verba_view.projeto_id IS NULL) OR
                    (verba_view.codigo_interno_colaborador IS NULL AND verba_view.cliente_id IS NULL AND verba_view.projeto_id IS NULL)
                  )
                  AND verba_view.tb_org_id = reembolso.tb_org_id
                ORDER BY verba_view.prioridade
                LIMIT 1
            ) verba ON true
            WHERE 
                reembolso.tb_org_id = @OrgId
                AND (@ClienteId IS NULL OR @ClienteId = '' OR reembolso.tb_cliente_id = @ClienteId)
                AND (@ProjetoId IS NULL OR @ProjetoId = '' OR reembolso.tb_projeto_id = @ProjetoId)
                AND (@StatusId IS NULL OR @StatusId = 0 OR reembolso.status_id = @StatusId)
                AND (@DataInicio IS NULL OR @DataInicio = '' OR DATE(reembolso.data_criacao) >= DATE(@DataInicio))
                AND (@DataFim IS NULL OR @DataInicio = '' OR DATE(reembolso.data_criacao) <= DATE(@DataFim))
                AND (
                    @AprovadorId IS NULL 
                    OR @AprovadorId = ''
                    OR EXISTS (
                        SELECT 1 
                        FROM tb_projeto_gerente tpg
                        WHERE tpg.cod_projeto = reembolso.tb_projeto_id
                          AND tpg.tb_org_id = projeto.tb_org_id
                          AND tpg.cod_colaborador_gerente = @AprovadorId
                    )
                );
        ";

        var parametros = new
        {
            OrgId = orgId,
            ClienteId = codigoCliente,
            ProjetoId = codigoProjeto,
            StatusId = statusId,
            DataInicio = dataInicio,
            DataFim = dataFinal,
            AprovadorId = codigoAprovador
        };

        var result = await connection.QueryAsync<dynamic>(query, parametros);
        
        var groupedList = result
            .GroupBy(x => new
            {
                x.Id,
                x.CodigoColaborador,
                x.NomeColaborador,
                x.ValorSolicitado,
                x.ValorAprovado,
                x.DataSolicitacao,
                x.Status,
                x.StatusId,
                x.Projeto,
                x.Cliente,
                x.Observacao,
                x.Descricao,
                x.TotalDespesas,
                x.NomeAprovador,
                x.ValorExcecao,
                x.Objetivo,
                x.Destino,
                x.DataInicio,
                x.DataFim,
                x.Categoria
            })
            .Select(g => new SolicitacaoColaboradorVisaoAdmDTO
            {
                Id = (int)g.Key.Id,
                CodigoColaborador = g.Key.CodigoColaborador,
                NomeColaborador = g.Key.NomeColaborador,
                ValorSolicitado = g.Key.ValorSolicitado,
                ValorAprovado = g.Key.ValorAprovado,
                DataSolicitacao = g.Key.DataSolicitacao,
                Status = g.Key.Status,
                StatusId = (int)g.Key.StatusId,
                Projeto = g.Key.Projeto,
                Cliente = g.Key.Cliente,
                Observacao = g.Key.Observacao,
                Descricao = g.Key.Descricao,
                TotalDespesas = (int)g.Key.TotalDespesas,
                NomeAprovador = g.Key.NomeAprovador,
                ValorExcecao = g.Key.ValorExcecao,
                Objetivo = g.Key.Objetivo,
                Destino = g.Key.Destino,
                DataInicio = g.Key.DataInicio,
                DataFim = g.Key.DataFim,
                Categoria = g.Key.Categoria,

                SolicitacaoDocumentos = g
                    .Where(d => d.DocumentoId != null)
                    .GroupBy(d => new
                    {
                        d.DocumentoId,
                        d.Tipo,
                        d.Url
                    })
                    .Select(d => new SolicitacaoDocumentoDTO
                    {
                        Id = d.Key.DocumentoId,
                        Tipo = d.Key.Tipo,
                        Url = d.Key.Url
                    })
                    .ToList()
            })
            .ToList();

        
        return groupedList;
    }

    public async Task<string> BuscarOperacaoDaSolicitacao(int solicitacaoId)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            SELECT
                ttc.operacao
            FROM tb_solicitacao_reembolso tsr
            INNER JOIN tb_verba tv ON tv.id = tsr.tb_verba_id
            INNER JOIN tb_verba_tipo ttc ON tv.tipo_custo = ttc.id
            WHERE tsr.id = @SolicitacaoId
        ";

        var parametros = new
        {
            SolicitacaoId = solicitacaoId
        };
        
        var result = await connection.QuerySingleOrDefaultAsync<string>(query, parametros);
        return result;
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