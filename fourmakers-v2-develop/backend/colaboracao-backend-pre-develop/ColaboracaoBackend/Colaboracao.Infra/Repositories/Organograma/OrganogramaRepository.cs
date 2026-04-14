using Colaboracao.Core.Interfaces;
using Core.Domain.Organograma;
using Dapper;
using DataTransferObject.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Organograma
{
    public class OrganogramaRepository : IOrganogramaRepository
    {
        private readonly IDBConnection _dapperConnection;

        private const string SqlJoinUltimoOrcamentoPosicao = @"
                                    LEFT JOIN (
                                        SELECT historico_com_numero_linha.id,
                                               historico_com_numero_linha.tb_organograma_posicao_id,
                                               historico_com_numero_linha.orcamento,
                                               historico_com_numero_linha.data_inicio,
                                               historico_com_numero_linha.data_fim
                                        FROM (
                                            SELECT historico_orcamento.id,
                                                   historico_orcamento.tb_organograma_posicao_id,
                                                   historico_orcamento.orcamento,
                                                   historico_orcamento.data_inicio,
                                                   historico_orcamento.data_fim,
                                                   ROW_NUMBER() OVER (PARTITION BY historico_orcamento.tb_organograma_posicao_id ORDER BY historico_orcamento.data_criacao DESC, historico_orcamento.id DESC) AS rn
                                            FROM tb_organograma_posicao_orcamento historico_orcamento
                                        ) historico_com_numero_linha WHERE historico_com_numero_linha.rn = 1
                                    ) ultimo_orcamento_posicao ON ultimo_orcamento_posicao.tb_organograma_posicao_id = tpo.id";

        private const string OrganogramaCompletoBaseSql = @"
                                    SELECT tpo.id AS PosicaoId,
                                           tpo.tb_org_id AS OrgId,
                                           tpo.tb_organograma_departamento_id AS DepartamentoId,
                                           tod.nome AS DepartamentoNome,
                                           tpo.codigo_cliente AS CodigoCliente,
                                           tpo.tb_perfil_corporativo_id AS PerfilCorporativoId,
                                           tpc.descricao AS PerfilCorporativoNome,
                                           tpo.tb_organograma_posicao_id_superior AS PosicaoIdSuperior,
                                           tpo.ativo AS PosicaoAtivo,
                                           tpo.data_criacao AS PosicaoDataCriacao,
                                           tpo.data_alteracao AS PosicaoDataAlteracao,
                                           tpo.c_level AS CLevel,
                                           tpo.profissional_externo AS ProfissionalExterno,
                                           tpo.tb_mapa_relacionamento_influencia_id AS MapaRelacionamentoInfluenciaId,
                                           tmfi.descricao AS MapaRelacionamentoInfluenciaDescricao,
                                           ultimo_orcamento_posicao.orcamento AS PosicaoOrcamento,
                                           ultimo_orcamento_posicao.data_inicio AS PosicaoOrcamentoDataInicio,
                                           ultimo_orcamento_posicao.data_fim AS PosicaoOrcamentoDataFim,
                                           tpa.id AS AlocacaoId,
                                           tpa.tb_perfil_corporativo_alocacao_id AS TbPerfilCorporativoAlocacaoId,
                                           tpa_aloc.codigo_interno_colaborador AS CodigoInternoColaborador,
                                           tc.nome_completo AS NomeColaborador,
                                           tpa.data_inicio AS DataInicio,
                                           tpa.data_fim AS DataFim,
                                           tpa.ativo AS AlocacaoAtivo,
                                           tpa.data_criacao AS AlocacaoDataCriacao,
                                           tpa.data_alteracao AS AlocacaoDataAlteracao
                                    FROM tb_organograma_posicao tpo
                                    LEFT JOIN tb_organograma_departamento tod
                                           ON tpo.tb_organograma_departamento_id = tod.id
                                          AND tod.tb_org_id = tpo.tb_org_id
                                    LEFT JOIN tb_perfil_corporativo tpc
                                           ON tpo.tb_perfil_corporativo_id = tpc.id
                                          AND tpc.tb_org_id = tpo.tb_org_id
                                    INNER JOIN tb_mapa_relacionamento_influencia tmfi
                                           ON tmfi.id = tpo.tb_mapa_relacionamento_influencia_id
                                    " + SqlJoinUltimoOrcamentoPosicao + @"
                                    LEFT JOIN tb_organograma_posicao_alocacao tpa
                                           ON tpa.tb_organograma_posicao_id = tpo.id
                                          AND tpa.tb_org_id = tpo.tb_org_id
                                    LEFT JOIN tb_perfil_corporativo_alocacao tpa_aloc
                                           ON tpa.tb_perfil_corporativo_alocacao_id = tpa_aloc.id
                                          AND tpa_aloc.tb_org_id = tpa.tb_org_id
                                    LEFT JOIN tb_colaborador tc
                                           ON tpa_aloc.codigo_interno_colaborador = tc.codigo_interno_colaborador";

        public OrganogramaRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        #region Departamento
        public async Task<OrganogramaDeptoListarPorIdResponseDTO> InserirDepartamento(OrganogramaDeptoInserirParamDTO param, int orgId)
        {
            var conn = _dapperConnection.GetConnection();

            var sqlExiste = @"
                    SELECT
                        EXISTS (SELECT 1 FROM tb_cliente_org WHERE codigo_cliente = @CodigoCliente) AS ClienteExiste,
                        EXISTS (SELECT 1 FROM tb_organograma_departamento WHERE codigo_cliente = @CodigoCliente AND tb_org_id = @OrgId AND nome = @Nome) AS DeptoExiste,
                        EXISTS (SELECT 1 FROM tb_organograma_posicao WHERE id = @PosicaoLiderId) AS LiderExiste;
                    ";

            var result = await conn.QuerySingleAsync<(bool ClienteExiste, bool DeptoExiste, bool LiderExiste)>(
                sqlExiste,
                new
                {
                    OrgId = orgId,
                    Nome = param.Nome,
                    CodigoCliente = param.CodigoCliente,
                    PosicaoLiderId = param.OrganogramaPosicaoIdLider
                }
            );

            if (result.DeptoExiste)
                throw new Exception($"Já foi cadastrado um departamento com este nome: {param.Nome}");

            if (!result.ClienteExiste)
                throw new Exception($"Código Cliente: {param.CodigoCliente} não encontrado!");

            if (!result.LiderExiste)
                throw new Exception($"Posição líder: {param.OrganogramaPosicaoIdLider} não encontrada!");

            var organogramaId = Guid.NewGuid().ToString();

            const string qInsert = @"
                                INSERT INTO tb_organograma_departamento
                                    (id,
                                     tb_org_id,
                                     nome,
                                     codigo_cliente,
                                     tb_organograma_posicao_id_lider)
                                VALUES
                                    (@Id, 
                                     @OrgId, 
                                     @Nome, 
                                     @CodigoCliente, 
                                     @PosicaoLiderId);
                                ";

            var parametros = new
            {
                Id = organogramaId,
                OrgId = orgId,
                Nome = param.Nome,
                CodigoCliente = param.CodigoCliente,
                PosicaoLiderId = param.OrganogramaPosicaoIdLider
            };

            await conn.ExecuteAsync(qInsert, parametros);

            return (OrganogramaDeptoListarPorIdResponseDTO)await ListaDepartamentoPorId(organogramaId);
        }

        public async Task<OrganogramaDeptoListarPorIdResponseDTO> AtualizarDepartamento(OrganogramaDeptoAtualizarParamDTO param, int orgId)
        {
            var conn = _dapperConnection.GetConnection();

            try
            {
                var sqlExiste = @"
                    SELECT
                        EXISTS (SELECT 1 FROM tb_cliente_org WHERE codigo_cliente = @CodigoCliente) AS ClienteExiste,
                        EXISTS (SELECT 1 FROM tb_organograma_posicao WHERE id = @PosicaoLiderId) AS LiderExiste;
                    ";

                var result = await conn.QuerySingleAsync<(bool ClienteExiste, bool LiderExiste)>(
                    sqlExiste,
                    new
                    {
                        OrgId = orgId,
                        param.CodigoCliente,
                        PosicaoLiderId = param.OrganogramaPosicaoIdLider
                    }
                );

                if (!result.ClienteExiste)
                    throw new Exception($"Código Cliente: {param.CodigoCliente} não encontrado!");

                if (!result.LiderExiste)
                    throw new Exception($"Posição líder: {param.OrganogramaPosicaoIdLider} não encontrada!");

                const string qUpdate = @"
                                UPDATE  tb_organograma_departamento SET
                                        tb_org_id       = @OrgId,
                                        nome            = @Nome,
                                        codigo_cliente  = @CodigoCliente,
                                        tb_organograma_posicao_id_lider = @OrganogramaPosicaoLider,
                                        ativo = @Ativo
                                WHERE id = @Id;
                               ";

                await conn.ExecuteAsync(qUpdate, new
                {
                    Id = param.Id,
                    OrgId = orgId,
                    Nome = param.Nome,
                    CodigoCliente = param.CodigoCliente,
                    OrganogramaPosicaoLider = param.OrganogramaPosicaoIdLider,
                    Ativo = param.Ativo
                });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao atualizar Departamento - " + ex.Message);
            }

            return (OrganogramaDeptoListarPorIdResponseDTO)await ListaDepartamentoPorId(param.Id.ToString());
        }

        public async Task<bool> DeletarDepartamento(string id)
        {
            var conn = _dapperConnection.GetConnection();
            //return await conn.ExecuteAsync("DELETE FROM tb_organograma_departamento WHERE id = @Id;", new { Id = id }) > 0;
            return await conn.ExecuteAsync("UPDATE tb_organograma_departamento SET ativo = 0 WHERE id = @Id;", new { Id = id }) > 0;
        }

        public async Task<OrganogramaDeptoListarPorIdResponseDTO> ListaDepartamentoPorId(string buscaId)
        {
            var conn = _dapperConnection.GetConnection();

            var listaDepto = await conn.QuerySingleOrDefaultAsync<OrganogramaDeptoListarPorIdResponseDTO>(
                @"SELECT    tic.id AS Id, 
                            tic.tb_org_id AS OrgId,
                            tic.nome AS Nome,
                            tic.codigo_cliente AS CodigoCliente,
                            tic.tb_organograma_posicao_id_lider AS OrganogramaPosicaoIdLider,
                            tic.ativo AS Ativo,
                            tic.data_criacao AS DataCriacao,
                            tic.data_alteracao AS DataAlteracao
                FROM tb_organograma_departamento tic
                WHERE tic.id = @Id;"
                , new { Id = buscaId });

            return listaDepto;
        }

        public async Task<List<OrganogramaDeptoListarPorIdResponseDTO>> ListarDepartamentoPorCliente(string codCliente)
        {
            var conn = _dapperConnection.GetConnection();

            const string sql = @"
                SELECT
                    tic.id AS Id, 
                    tic.tb_org_id AS OrgId,
                    tic.nome AS Nome,
                    tic.codigo_cliente AS CodigoCliente,
                    tic.tb_organograma_posicao_id_lider AS OrganogramaPosicaoIdLider,
                    tic.ativo AS Ativo,
                    tic.data_criacao AS DataCriacao,
                    tic.data_alteracao AS DataAlteracao
                FROM tb_organograma_departamento tic
                WHERE tic.codigo_cliente = @codCliente;
            ";

            var listaDepto = (await conn.QueryAsync<OrganogramaDeptoListarPorIdResponseDTO>(
                sql,
                new { codCliente }
            )).ToList();

            return listaDepto;
        }

        public async Task<IEnumerable<OrganogramaDeptoListarPorIDCompletoResponseDTO>> ListaDepartamentoCompletoPorId(string buscaId)
        {
            var conn = _dapperConnection.GetConnection();

            var listaDepto = await conn.QueryAsync<OrganogramaDeptoListarPorIDCompletoResponseDTO>(
                @"SELECT    tic.id AS Id, 
                            tic.tb_org_id AS OrgId,
                            tca.descricao AS DescricaoOrg,
                            tic.nome AS Nome,
                            tic.codigo_cliente AS CodigoCliente,
                            tco.nome_cliente AS NomeCliente,
                            tic.tb_organograma_posicao_id_lider AS OrganogramaPosicaoIdLider,
                            tic.ativo AS Ativo,
                            tic.data_criacao AS DataCriacao
                FROM tb_organograma_departamento tic
                LEFT JOIN tb_org tca on tca.id = tic.tb_org_id
                LEFT JOIN tb_cliente_org tco on tco.codigo_cliente = tic.codigo_cliente                         
                WHERE tic.id = @Id
                AND   tic.ativo = 1;"
                , new { Id = buscaId });

            if (listaDepto == null)
                return null;

            return listaDepto;
        }
        #endregion Departamento

        #region Posição
        public async Task<OrganogramaPosicaoListarPorIdResponseDTO> PosicaoInserir(OrganogramaPosicaoInserirParamDTO param, int orgId)
        {
            var conn = _dapperConnection.GetConnection();

            var sqlExiste = @"
                    SELECT
                        EXISTS (SELECT 1 FROM tb_cliente_org WHERE codigo_cliente = @CodigoCliente) AS ClienteExiste,
                        EXISTS (SELECT 1 FROM tb_perfil_corporativo WHERE id = @CodPfCorp AND ativo = 1) AS PfCorpExiste,
                        EXISTS (SELECT 1 FROM tb_organograma_posicao WHERE id = @CodLiderSup OR @CodLiderSup = '' OR @CodLiderSup IS NULL) AS LiderExiste;
                    ";

            var result = await conn.QuerySingleAsync<(bool ClienteExiste, bool PfCorpExiste, bool LiderExiste)>(
                sqlExiste,
                new
                {
                    OrgId = orgId,
                    CodigoCliente = param.CodigoCliente,
                    CodigoDepto = param.OrganogramaDepartamentoId,
                    CodPfCorp = param.PerfilCorporativoId,
                    CodLiderSup = param.OrganogramaPosicaoIdSuperior
                }
            );

            if (!result.ClienteExiste)
                throw new Exception($"Código Cliente: {param.CodigoCliente} não encontrado!");

            if (!result.PfCorpExiste)
                throw new Exception($"Código Perfil Corporativo: {param.PerfilCorporativoId} não encontrado!");

            //if (!result.LiderExiste)
            //    throw new Exception($"Posição líder: {param.OrganogramaPosicaoIdSuperior} não encontrada!");


            var organogramaId = Guid.NewGuid().ToString();

            const string qInsert = @"
                                INSERT INTO tb_organograma_posicao
                                    (id,
                                     tb_org_id,
                                     codigo_cliente,
                                     tb_organograma_departamento_id,
                                     tb_perfil_corporativo_id,
                                     tb_organograma_posicao_id_superior,
                                     c_level,
                                     profissional_externo,
                                     tb_mapa_relacionamento_influencia_id
                                    )
                                VALUES
                                    (@Id, 
                                     @OrgId,
                                     @CodigoCliente,
                                     @CodigoDepto, 
                                     @CodPfCorp,
                                     @CodLider,
                                     @CLevel,
                                     @ProfExterno,
                                     @MapaRelacionamentoInfluenciaId
                                    );
                                ";

            var parametros = new
            {
                Id = organogramaId,
                OrgId = orgId,
                CodigoCliente = param.CodigoCliente,
                CodigoDepto = param.OrganogramaDepartamentoId,
                CodPfCorp = param.PerfilCorporativoId,
                CodLider = param.OrganogramaPosicaoIdSuperior,
                CLevel = param.CLevel,
                ProfExterno = param.ProfissionalExterno,
                MapaRelacionamentoInfluenciaId = param.MapaRelacionamentoInfluenciaId
            };

            await conn.ExecuteAsync(qInsert, parametros);

            return await PosicaoListaPorId(organogramaId);
        }


        public async Task<OrganogramaPosicaoListarPorIdResponseDTO> PosicaoAtualizar(OrganogramaPosicaoAtualizarParamDTO param, int orgId)
        {
            var conn = _dapperConnection.GetConnection();

            try
            {
                var sqlExiste = @"
                    SELECT
                        EXISTS (SELECT 1 FROM tb_cliente_org WHERE codigo_cliente = @CodigoCliente) AS ClienteExiste,
                        EXISTS (SELECT 1 FROM tb_organograma_departamento WHERE id = @CodigoDepto OR @CodigoDepto IS NULL) AS DeptoExiste,
                        EXISTS (SELECT 1 FROM tb_perfil_corporativo WHERE id = @CodPfCorp AND ativo = 1) AS PfCorpExiste,
                        EXISTS (SELECT 1 FROM tb_organograma_posicao WHERE id = @CodLiderSup OR @CodLiderSup = '' OR @CodLiderSup IS NULL) AS LiderExiste;
                    ";

                var result = await conn.QuerySingleAsync<(bool ClienteExiste, bool DeptoExiste, bool PfCorpExiste, bool LiderExiste)>(
                    sqlExiste,
                    new
                    {
                        OrgId = orgId,
                        CodigoCliente = param.CodigoCliente,
                        CodigoDepto = param.OrganogramaDepartamentoId,
                        CodPfCorp = param.PerfilCorporativoId,
                        CodLiderSup = param.OrganogramaPosicaoIdSuperior
                    }
                );

                if (!result.ClienteExiste)
                    throw new Exception($"Código Cliente: {param.CodigoCliente} não encontrado!");

                if (!result.DeptoExiste)
                    throw new Exception($"Código Departamento: {param.OrganogramaDepartamentoId} não encontrado!");

                if (!result.PfCorpExiste)
                    throw new Exception($"Código Perfil Corporativo: {param.PerfilCorporativoId} não encontrado!");

                //if (!result.LiderExiste)
                //    throw new Exception($"Posição líder: {param.OrganogramaPosicaoIdSuperior} não encontrada!");


                const string qUpdate = @"
                                UPDATE  tb_organograma_posicao SET
                                        tb_org_id                           = @orgId,
                                        codigo_cliente                      = @CodigoCliente,
                                        tb_organograma_departamento_id      = @CodigoDepto,
                                        tb_perfil_corporativo_id            = @CodPfCorp,
                                        tb_organograma_posicao_id_superior  = @CodLider,
                                        ativo = @Ativo,
                                        c_level = @CLevel,
                                        profissional_externo = @ProfExterno,
                                        tb_mapa_relacionamento_influencia_id = @MapaRelacionamentoInfluenciaId
                                WHERE id = @Id;
                               ";

                await conn.ExecuteAsync(qUpdate, new
                {
                    Id = param.Id,
                    orgId = orgId,
                    CodigoCliente = param.CodigoCliente,
                    CodigoDepto = param.OrganogramaDepartamentoId,
                    CodPfCorp = param.PerfilCorporativoId,
                    CodLider = param.OrganogramaPosicaoIdSuperior,
                    Ativo = param.Ativo,
                    CLevel = param.CLevel,
                    ProfExterno = param.ProfissionalExterno,
                    MapaRelacionamentoInfluenciaId = param.MapaRelacionamentoInfluenciaId
                });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao atualizar Posição no Organograma - " + ex.Message);
            }

            return await PosicaoListaPorId(param.Id.ToString());
        }

        public async Task<bool> PosicaoDeletar(string id)
        {
            var conn = _dapperConnection.GetConnection();
            return await conn.ExecuteAsync("UPDATE tb_organograma_posicao SET ativo = 0 WHERE id = @Id;", new { Id = id }) > 0;
        }

        public async Task<OrganogramaPosicaoListarPorIdResponseDTO> PosicaoListaPorId(string buscaId)
        {
            var conn = _dapperConnection.GetConnection();

            var listaPosicao = await conn.QuerySingleOrDefaultAsync<OrganogramaPosicaoListarPorIdResponseDTO>(
                @"SELECT    tpo.id AS Id, 
                            tpo.tb_org_id AS OrgId,
                            tpo.codigo_cliente AS CodigoCliente,
                            tpo.tb_organograma_departamento_id AS DepartamentoId,
                            CAST(tpo.tb_perfil_corporativo_id AS CHAR) AS PerfilCorporativoId,
                            tpo.tb_organograma_posicao_id_superior AS PosicaoIdSuperior,
                            tpo.ativo AS Ativo,
                            tpo.c_level AS CLevel,
                            tpo.profissional_externo AS ProfissionalExterno,
                            tpo.tb_mapa_relacionamento_influencia_id AS MapaRelacionamentoInfluenciaId,
                            tmfi.descricao AS MapaRelacionamentoInfluenciaDescricao,
                            tpo.data_criacao AS DataCriacao,
                            tpo.data_alteracao AS DataAlteracao,
                            ultimo_orcamento_posicao.orcamento AS Orcamento,
                            ultimo_orcamento_posicao.data_inicio AS OrcamentoDataInicio,
                            ultimo_orcamento_posicao.data_fim AS OrcamentoDataFim
                FROM tb_organograma_posicao tpo
                INNER JOIN tb_mapa_relacionamento_influencia tmfi ON tmfi.id = tpo.tb_mapa_relacionamento_influencia_id
                " + SqlJoinUltimoOrcamentoPosicao + @"
                WHERE tpo.id = @Id;"
                , new { Id = buscaId });

            return listaPosicao;
        }

        public async Task<List<OrganogramaPosicaoListarPorIdResponseDTO>> PosicaoListarPorCliente(string codCliente)
        {
            var conn = _dapperConnection.GetConnection();

            var sql = @"
                SELECT
                    tpo.id AS Id, 
                    tpo.tb_org_id AS OrgId,
                    tpo.codigo_cliente AS CodigoCliente,
                    tpo.tb_organograma_departamento_id AS DepartamentoId,
                    CAST(tpo.tb_perfil_corporativo_id AS CHAR) AS PerfilCorporativoId,
                    tpo.tb_organograma_posicao_id_superior AS PosicaoIdSuperior,
                    tpo.ativo AS Ativo,
                    tpo.c_level AS CLevel,
                    tpo.profissional_externo AS ProfissionalExterno,
                    tpo.tb_mapa_relacionamento_influencia_id AS MapaRelacionamentoInfluenciaId,
                    tmfi.descricao AS MapaRelacionamentoInfluenciaDescricao,
                    tpo.data_criacao AS DataCriacao,
                    tpo.data_alteracao AS DataAlteracao,
                    ultimo_orcamento_posicao.orcamento AS Orcamento,
                    ultimo_orcamento_posicao.data_inicio AS OrcamentoDataInicio,
                    ultimo_orcamento_posicao.data_fim AS OrcamentoDataFim
                FROM tb_organograma_posicao tpo
                INNER JOIN tb_mapa_relacionamento_influencia tmfi ON tmfi.id = tpo.tb_mapa_relacionamento_influencia_id
                " + SqlJoinUltimoOrcamentoPosicao + @"
                WHERE tpo.codigo_cliente = @codCliente;
            ";

            var listaPosicao = (await conn.QueryAsync<OrganogramaPosicaoListarPorIdResponseDTO>(
                sql,
                new { codCliente }
            )).ToList();

            return listaPosicao;
        }

        #endregion Posição

        #region Alocação
        public async Task<OrganogramaAlocacaoListarPorIdResponseDTO> AlocacaoInserir(OrganogramaAlocacaoInserirParamDTO param, int orgId)
        {
            var conn = _dapperConnection.GetConnection();
            using var tx = await conn.BeginTransactionAsync();

            try
            {
                // Passo 1 - Buscar a posição pelo OrganogramaPosicaoId e obter o PerfilCorporativo da posição
                var sqlPosicao = @"
                    SELECT id, tb_perfil_corporativo_id AS PerfilCorporativoId
                    FROM tb_organograma_posicao
                    WHERE id = @PosicaoId AND tb_org_id = @OrgId;
                    ";

                var posicao = await conn.QuerySingleOrDefaultAsync<(string Id, Guid PerfilCorporativoId)>(
                    sqlPosicao,
                    new
                    {
                        PosicaoId = param.OrganogramaPosicaoId,
                        OrgId = orgId
                    },
                    tx
                );

                if (posicao.Id == null || posicao.PerfilCorporativoId == null)
                    throw new Exception($"Posição {param.OrganogramaPosicaoId} não encontrada!");

                var dataInicio = param.DataInicio ?? DateTime.Now;
                var dataFim = param.DataFim;

                // Passo 2 - Verificar se o colaborador tem alocação na tb_perfil_corporativo_alocacao com o mesmo perfil da posição
                var sqlBuscarPerfilAlocacao = @"
                    SELECT id
                    FROM tb_perfil_corporativo_alocacao
                    WHERE tb_org_id = @OrgId
                      AND codigo_interno_colaborador = @CodColaborador
                      AND tb_perfil_corporativo_id = @PerfilCorporativoId
                      AND ativo = 1
                      AND (data_fim IS NULL OR data_fim >= @DataInicio)
                      AND (data_inicio <= @DataFim OR @DataFim IS NULL)
                    ORDER BY data_criacao DESC
                    LIMIT 1;
                    ";

                var perfilAlocacaoId = await conn.QuerySingleOrDefaultAsync<string>(
                    sqlBuscarPerfilAlocacao,
                    new
                    {
                        OrgId = orgId,
                        CodColaborador = param.CodigoInternoColaborador,
                        PerfilCorporativoId = posicao.PerfilCorporativoId,
                        DataInicio = dataInicio,
                        DataFim = dataFim
                    },
                    tx
                );

                // Passo 3 - Se não tem alocação com o mesmo perfil, criar uma em tb_perfil_corporativo_alocacao
                if (string.IsNullOrEmpty(perfilAlocacaoId))
                {
                    perfilAlocacaoId = Guid.NewGuid().ToString();
                    const string qInsertPerfilAlocacao = @"
                    INSERT INTO tb_perfil_corporativo_alocacao
                        (id,
                         tb_org_id,
                         tb_perfil_corporativo_id,
                         codigo_interno_colaborador,
                         data_inicio,
                         data_fim,
                         ativo,
                         criado_por_mapa_relacionamento,
                         data_criacao
                        )
                    VALUES
                        (@Id,
                         @OrgId,
                         @PerfilCorporativoId,
                         @CodigoInternoColaborador,
                         @DataInicio,
                         @DataFim,
                         1,
                         1,
                         NOW()
                        );";

                    await conn.ExecuteAsync(qInsertPerfilAlocacao, new
                    {
                        Id = perfilAlocacaoId,
                        OrgId = orgId,
                        PerfilCorporativoId = posicao.PerfilCorporativoId,
                        CodigoInternoColaborador = param.CodigoInternoColaborador,
                        DataInicio = dataInicio,
                        DataFim = dataFim
                    }, tx);
                }

                var sqlExiste = @"
                    SELECT
                        EXISTS (SELECT 1 FROM tb_organograma_posicao WHERE id = @PosicaoLiderId) AS LiderExiste,
                        EXISTS (SELECT 1 FROM tb_colaborador WHERE codigo_interno_colaborador = @CodColaborador) AS ColaboradorExiste
                    ";

                var result = await conn.QuerySingleAsync<(bool LiderExiste, bool ColaboradorExiste)>(
                    sqlExiste,
                    new
                    {
                        PosicaoLiderId = param.OrganogramaPosicaoId,
                        CodColaborador = param.CodigoInternoColaborador
                    },
                    tx
                );

                if (!result.LiderExiste)
                    throw new Exception($"Posição líder: {param.OrganogramaPosicaoId} não encontrada!");

                if (!result.ColaboradorExiste)
                    throw new Exception($"Colaborador: {param.CodigoInternoColaborador} não encontrada!");

                // Inativa alocação ativa da mesma posição (se existir)
                const string qInativar = @"
                                UPDATE tb_organograma_posicao_alocacao
                                SET ativo = 0,
                                    data_alteracao = NOW()
                                WHERE tb_org_id = @OrgId
                                  AND tb_organograma_posicao_id = @PosicaoLiderId
                                  AND ativo = 1;";

                await conn.ExecuteAsync(qInativar, new
                {
                    OrgId = orgId,
                    PosicaoLiderId = param.OrganogramaPosicaoId
                }, tx);

                var organogramaId = Guid.NewGuid().ToString();

                const string qInsert = @"
                                INSERT INTO tb_organograma_posicao_alocacao
                                    (id,
                                     tb_org_id,
                                     tb_organograma_posicao_id,
                                     tb_perfil_corporativo_alocacao_id,
                                     data_inicio,
                                     data_fim,
                                     ativo,
                                     data_criacao
                                    )
                                VALUES
                                    (@Id,
                                     @OrgId,
                                     @PosicaoLiderId,
                                     @TbPerfilCorporativoAlocacaoId,
                                     @DataInicio,
                                     @DataFim,
                                     @Ativo,
                                     NOW()
                                    );
                                ";

                var parametros = new
                {
                    Id = organogramaId,
                    OrgId = orgId,
                    PosicaoLiderId = param.OrganogramaPosicaoId,
                    TbPerfilCorporativoAlocacaoId = perfilAlocacaoId,
                    DataInicio = dataInicio,
                    DataFim = dataFim,
                    Ativo = true
                };

                await conn.ExecuteAsync(qInsert, parametros, tx);

                await tx.CommitAsync();

                return await AlocacaoListaPorId(organogramaId);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<OrganogramaAlocacaoListarPorIdResponseDTO> AlocacaoAtualizar(OrganogramaAlocacaoAtualizarParamDTO param, int orgId)
        {
            var conn = _dapperConnection.GetConnection();

            try
            {
                // Verifica se existe a alocacao
                var sqlAlocacao = @"
                        SELECT id 
                        FROM tb_organograma_posicao_alocacao
                        WHERE id = @IdAlocacao AND tb_org_id = @OrgId;
                        ";

                var alocacaoExiste = await conn.QuerySingleOrDefaultAsync<string>(
                    sqlAlocacao,
                    new
                    {
                        IdAlocacao = param.Id,
                        OrgId = orgId
                    }
                );

                if (string.IsNullOrEmpty(alocacaoExiste))
                    throw new Exception($"Alocacao {param.Id} não encontrada.");

                // Buscar registro em tb_perfil_corporativo_alocacao
                var sqlBuscarPerfilAlocacao = @"
                    SELECT id 
                    FROM tb_perfil_corporativo_alocacao 
                    WHERE tb_org_id = @OrgId 
                      AND codigo_interno_colaborador = @CodColaborador
                      AND ativo = 1
                      AND (data_fim IS NULL OR data_fim >= @DataAtual)
                    ORDER BY data_criacao DESC
                    LIMIT 1;
                    ";

                var perfilAlocacaoId = await conn.QuerySingleOrDefaultAsync<string>(
                    sqlBuscarPerfilAlocacao,
                    new
                    {
                        OrgId = orgId,
                        CodColaborador = param.CodigoInternoColaborador,
                        DataAtual = DateTime.Now
                    }
                );

                if (string.IsNullOrEmpty(perfilAlocacaoId))
                    throw new Exception($"Alocação não encontrada para o colaborador selecionado.");

                var sqlExiste = @"
                    SELECT EXISTS (
                        SELECT 1
                        FROM tb_organograma_posicao
                        WHERE id = @PosicaoLiderId
                    )
                ";

                var liderExiste = await conn.QuerySingleAsync<bool>(
                    sqlExiste,
                    new
                    {
                        PosicaoLiderId = param.OrganogramaPosicaoId
                    }
                );


                if (!liderExiste)
                    throw new Exception($"Posição líder: {param.OrganogramaPosicaoId} não encontrada!");

                const string qUpdate = @"
                                UPDATE  tb_organograma_posicao_alocacao SET
                                        tb_org_id                       = @orgId,
                                        tb_organograma_posicao_id       = @OrganogramaPosicaoId,
                                        tb_perfil_corporativo_alocacao_id = @TbPerfilCorporativoAlocacaoId,
                                        data_inicio                     = @DataInicio,
                                        data_fim                        = @DataFim,
                                        ativo = @Ativo,
                                        data_alteracao = NOW()
                                WHERE id = @Id;
                               ";

                await conn.ExecuteAsync(qUpdate, new
                {
                    param.Id,
                    orgId,
                    param.OrganogramaPosicaoId,
                    TbPerfilCorporativoAlocacaoId = perfilAlocacaoId,
                    param.DataInicio,
                    param.DataFim,
                    param.Ativo
                });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao atualizar Alocação no Organograma - " + ex.Message);
            }

            return await AlocacaoListaPorId(param.Id.ToString());
        }

        public async Task<bool> AlocacaoDeletar(string id)
        {
            var conn = _dapperConnection.GetConnection();
            return await conn.ExecuteAsync("UPDATE tb_organograma_posicao_alocacao SET ativo = 0 WHERE id = @Id;", new { Id = id }) > 0;
        }


        public async Task<OrganogramaAlocacaoListarPorIdResponseDTO> AlocacaoListaPorId(string buscaId)
        {
            var conn = _dapperConnection.GetConnection();

            var listaAlocacao = await conn.QuerySingleOrDefaultAsync<OrganogramaAlocacaoListarPorIdResponseDTO>(
                @"SELECT    tic.id AS Id, 
                            tic.tb_org_id AS OrgId,
                            tic.tb_organograma_posicao_id AS OrganogramaPosicaoId,
                            tic.tb_perfil_corporativo_alocacao_id AS TbPerfilCorporativoAlocacaoId,
                            tpa.codigo_interno_colaborador AS CodigoInternoColaborador,
                            tic.data_inicio AS DataInicio,
                            tic.data_fim AS DataFim,
                            tic.ativo AS Ativo,
                            tic.data_criacao AS DataCriacao,
                            tic.data_alteracao AS DataAlteracao
                FROM tb_organograma_posicao_alocacao tic
                INNER JOIN tb_perfil_corporativo_alocacao tpa
                    ON tic.tb_perfil_corporativo_alocacao_id = tpa.id
                WHERE tic.id = @Id;"
                , new { Id = buscaId });

            return listaAlocacao;
        }

        #endregion Alocação

        #region Perfil Corporativo Alocação
        public async Task<OrganogramaPerfilCorporativoAlocacaoListarPorIdResponseDTO> InserirPerfilCorporativoAlocacao(OrganogramaPerfilCorporativoAlocacaoInserirParamDTO param, int orgId)
        {
            var conn = _dapperConnection.GetConnection();

            var sqlExiste = @"
                    SELECT
                        EXISTS (SELECT 1 FROM tb_perfil_corporativo WHERE id = @PerfilCorporativoId AND tb_org_id = @OrgId) AS PerfilExiste,
                        EXISTS (SELECT 1 FROM tb_colaborador WHERE codigo_interno_colaborador = @CodColaborador) AS ColaboradorExiste;
                    ";

            var result = await conn.QuerySingleAsync<(bool PerfilExiste, bool ColaboradorExiste)>(
                sqlExiste,
                new
                {
                    OrgId = orgId,
                    PerfilCorporativoId = param.PerfilCorporativoId,
                    CodColaborador = param.CodigoInternoColaborador
                }
            );

            if (!result.PerfilExiste)
                throw new Exception($"Código Perfil Corporativo: {param.PerfilCorporativoId} não encontrado!");

            if (!result.ColaboradorExiste)
                throw new Exception($"Colaborador: {param.CodigoInternoColaborador} não encontrada!");

            var perfilAlocacaoId = Guid.NewGuid().ToString();

            const string qInsert = @"
                                INSERT INTO tb_perfil_corporativo_alocacao
                                    (id,
                                     tb_org_id,
                                     tb_perfil_corporativo_id,
                                     codigo_interno_colaborador,
                                     data_inicio,
                                     data_fim,
                                     ativo,
                                     data_criacao
                                    )
                                VALUES
                                    (@Id,
                                     @OrgId,
                                     @PerfilCorporativoId,
                                     @CodigoInternoColaborador,
                                     @DataInicio,
                                     @DataFim,
                                     @Ativo,
                                     @DataCriacao
                                    );";

            await conn.ExecuteAsync(qInsert, new
            {
                Id = perfilAlocacaoId,
                OrgId = orgId,
                PerfilCorporativoId = param.PerfilCorporativoId,
                CodigoInternoColaborador = param.CodigoInternoColaborador,
                DataInicio = param.DataInicio ?? DateTime.Now,
                DataFim = param.DataFim,
                Ativo = param.Ativo,
                DataCriacao = DateTime.Now
            });

            return await BuscarPerfilCorporativoAlocacaoPorId(perfilAlocacaoId);
        }

        public async Task<OrganogramaPerfilCorporativoAlocacaoListarPorIdResponseDTO> AtualizarPerfilCorporativoAlocacao(OrganogramaPerfilCorporativoAlocacaoAtualizarParamDTO param, int orgId)
        {
            var conn = _dapperConnection.GetConnection();

            var sqlExiste = @"
                    SELECT
                        EXISTS (SELECT 1 FROM tb_perfil_corporativo WHERE id = @PerfilCorporativoId AND tb_org_id = @OrgId) AS PerfilExiste,
                        EXISTS (SELECT 1 FROM tb_colaborador WHERE codigo_interno_colaborador = @CodColaborador) AS ColaboradorExiste;
                    ";

            var result = await conn.QuerySingleAsync<(bool PerfilExiste, bool ColaboradorExiste)>(
                sqlExiste,
                new
                {
                    OrgId = orgId,
                    PerfilCorporativoId = param.PerfilCorporativoId,
                    CodColaborador = param.CodigoInternoColaborador
                }
            );

            if (!result.PerfilExiste)
                throw new Exception($"Código Perfil Corporativo: {param.PerfilCorporativoId} não encontrado!");

            if (!result.ColaboradorExiste)
                throw new Exception($"Colaborador: {param.CodigoInternoColaborador} não encontrada!");

            const string qUpdate = @"
                                UPDATE tb_perfil_corporativo_alocacao SET
                                     tb_org_id = @OrgId,
                                     tb_perfil_corporativo_id = @PerfilCorporativoId,
                                     codigo_interno_colaborador = @CodigoInternoColaborador,
                                     data_inicio = @DataInicio,
                                     data_fim = @DataFim,
                                     ativo = @Ativo,
                                     data_alteracao = CURRENT_TIMESTAMP
                                WHERE id = @Id;";

            await conn.ExecuteAsync(qUpdate, new
            {
                Id = param.Id,
                OrgId = orgId,
                PerfilCorporativoId = param.PerfilCorporativoId,
                CodigoInternoColaborador = param.CodigoInternoColaborador,
                DataInicio = param.DataInicio ?? DateTime.Now,
                DataFim = param.DataFim,
                Ativo = param.Ativo
            });

            return await BuscarPerfilCorporativoAlocacaoPorId(param.Id);
        }

        public async Task<bool> DeletarPerfilCorporativoAlocacao(string id)
        {
            var conn = _dapperConnection.GetConnection();
            return await conn.ExecuteAsync("UPDATE tb_perfil_corporativo_alocacao SET ativo = 0 WHERE id = @Id;", new { Id = id }) > 0;
        }

        public async Task<OrganogramaPerfilCorporativoAlocacaoListarPorIdResponseDTO> BuscarPerfilCorporativoAlocacaoPorId(string buscaId)
        {
            var conn = _dapperConnection.GetConnection();

            var lista = await conn.QuerySingleOrDefaultAsync<OrganogramaPerfilCorporativoAlocacaoListarPorIdResponseDTO>(
                @"SELECT id AS Id,
                         tb_org_id AS OrgId,
                         tb_perfil_corporativo_id AS PerfilCorporativoId,
                         codigo_interno_colaborador AS CodigoInternoColaborador,
                         data_inicio AS DataInicio,
                         data_fim AS DataFim,
                         ativo AS Ativo,
                         data_criacao AS DataCriacao,
                         data_alteracao AS DataAlteracao
                  FROM tb_perfil_corporativo_alocacao
                  WHERE id = @Id;"
                , new { Id = buscaId });

            return lista;
        }

        public async Task<IEnumerable<OrganogramaColaboradorAlocadoDTO>> ListarColaboradoresExternosAlocados(int orgId, int limit, int cursor, string nome)
        {
            var conn = _dapperConnection.GetConnection();

            const string sql = @"
                SELECT DISTINCT
                       pca.codigo_interno_colaborador AS CodigoInternoColaborador,
                       tc.nome_completo AS NomeColaborador
                FROM tb_perfil_corporativo_alocacao pca
                INNER JOIN tb_colaborador tc
                        ON pca.codigo_interno_colaborador = tc.codigo_interno_colaborador
                INNER JOIN tb_gestor_externo tge
                        ON pca.codigo_interno_colaborador = tge.codigo_interno_colaborador
                       AND tge.tb_org_id = pca.tb_org_id
                WHERE pca.tb_org_id = @OrgId
                  AND pca.ativo = 1
                  AND (@Nome IS NULL OR @Nome = '' OR tc.nome_completo LIKE CONCAT('%', @Nome, '%'))
                LIMIT @Limit OFFSET @Cursor;";

            return await conn.QueryAsync<OrganogramaColaboradorAlocadoDTO>(sql, new { OrgId = orgId, Limit = limit, Cursor = cursor, Nome = nome });
        }

        public async Task<IEnumerable<OrganogramaColaboradorAlocadoDTO>> ListarColaboradoresExternos(int orgId, int limit, int cursor, string nome)
        {
            var conn = _dapperConnection.GetConnection();

            const string sql = @"
                SELECT DISTINCT
                       tc.codigo_interno_colaborador AS CodigoInternoColaborador,
                       tc.nome_completo AS NomeColaborador
                FROM tb_colaborador tc
                INNER JOIN tb_gestor_externo tge
                        ON tc.codigo_interno_colaborador = tge.codigo_interno_colaborador
                       AND tge.tb_org_id = @OrgId
                       AND tge.ativo = 1
                WHERE (@Nome IS NULL OR @Nome = '' OR tc.nome_completo LIKE CONCAT('%', @Nome, '%'))
                LIMIT @Limit OFFSET @Cursor;";

            return await conn.QueryAsync<OrganogramaColaboradorAlocadoDTO>(sql, new { OrgId = orgId, Limit = limit, Cursor = cursor, Nome = nome });
        }

        public async Task<IEnumerable<OrganogramaColaboradorAlocadoDTO>> ListarColaboradoresExternosPorCliente(int orgId, string codigoCliente, int limit, int cursor, string nome)
        {
            var conn = _dapperConnection.GetConnection();

            const string sql = @"
                SELECT DISTINCT
                       tc.codigo_interno_colaborador AS CodigoInternoColaborador,
                       tc.nome_completo AS NomeColaborador
                FROM tb_colaborador tc
                INNER JOIN tb_gestor_externo tge
                        ON tc.codigo_interno_colaborador = tge.codigo_interno_colaborador
                       AND tge.tb_org_id = @OrgId
                       AND tge.ativo = 1
                       AND tge.codigo_cliente = @CodigoCliente
                WHERE (@Nome IS NULL OR @Nome = '' OR tc.nome_completo LIKE CONCAT('%', @Nome, '%'))
                LIMIT @Limit OFFSET @Cursor;";

            return await conn.QueryAsync<OrganogramaColaboradorAlocadoDTO>(sql, new { OrgId = orgId, CodigoCliente = codigoCliente, Limit = limit, Cursor = cursor, Nome = nome });
        }
        #endregion Perfil Corporativo Alocação

        #region Perfil Corporativo
        public async Task<OrganogramaPerfilCorporativoListarPorIdResponseDTO> PerfilCorporativoInserir(OrganogramaPerfilCorporativoInserirParamDTO param, string codColaborador, int orgId)
        {
            var conn = _dapperConnection.GetConnection();

            var sqlExiste = @"
                    SELECT
                        EXISTS (SELECT 1 FROM tb_permanencia WHERE id = @PermanenciaId) AS PermanenciaExiste,
                        EXISTS (SELECT 1 FROM tb_modelo_trabalho WHERE id = @ModeloTrabalhoId) AS ModTrabalhoExiste,
                        EXISTS (SELECT 1 FROM tb_profissional_localidade WHERE id = @ProfissionalLocal) AS ProfLocalExiste,
                        EXISTS (SELECT 1 FROM tb_tipos_emprego_linkedin WHERE id = @EmpregoLkdin) AS EmprLkdExiste,
                        EXISTS (SELECT 1 FROM tb_niveis_experiencia_linkedin WHERE id = @ExperienciaLkd) AS ExpLkdExiste;
                    ";

            var result = await conn.QuerySingleAsync<(bool PermanenciaExiste, bool ModTrabalhoExiste, bool ProfLocalExiste, bool EmprLkdExiste, bool ExpLkdExiste, bool ColaboradorCriaExiste, bool ColaboradorAlteExiste)>(
                sqlExiste,
                new
                {
                    OrgId = orgId,
                    PermanenciaId = param.PermanenciaId,
                    ModeloTrabalhoId = param.ModeloTrabalhoId,
                    ProfissionalLocal = param.ProfissionalLocalidadeId,
                    EmpregoLkdin = param.EmpregoLinkdinId,
                    ExperienciaLkd = param.ExperienciaLinkedinId
                }
            );

            if (!result.PermanenciaExiste)
                throw new Exception($"Código Permanência: {param.PermanenciaId} não encontrado!");

            if (!result.ModTrabalhoExiste)
                throw new Exception($"Modelo Trabalho: {param.ModeloTrabalhoId} não encontrada!");

            if (!result.ProfLocalExiste)
                throw new Exception($"Profissional Localidade: {param.ProfissionalLocalidadeId} não encontrada!");
            
            if (!result.EmprLkdExiste)
                throw new Exception($"Tipo Emprego Linkedin: {param.EmpregoLinkdinId} não encontrada!");

            if (!result.ExpLkdExiste)
                throw new Exception($"Nível Experiência Linkedin: {param.ExperienciaLinkedinId} não encontrada!");

            var pfCorpId = Guid.NewGuid().ToString();

            const string qInsert = @"
                                INSERT INTO tb_perfil_corporativo
                                    (id,
                                     tb_org_id,
                                     descricao,
                                     tb_permanencia_id,
                                     tb_modelo_trabalho_id,
                                     tb_profissional_localidade_id,
                                     tipo_emprego_linkedin,
                                     nivel_experiencia_linkedin,
                                     atribuicoes,
                                     codigo_interno_colaborador_criacao
                                    )
                                VALUES
                                    (@Id, 
                                     @OrgId, 
                                     @Descricao, 
                                     @PermanenciaId,
                                     @ModTrabalho,
                                     @ProfLocal,
                                     @EmprLkd,
                                     @ExperLkd,
                                     @Atribuicoes,
                                     @CodColaborador
                                    );
                                ";

            var parametros = new
            {
                Id = pfCorpId,
                OrgId = orgId,
                Descricao = param.Descricao,
                PermanenciaId = param.PermanenciaId,
                ModTrabalho = param.ModeloTrabalhoId,
                ProfLocal = param.ProfissionalLocalidadeId,
                EmprLkd = param.EmpregoLinkdinId,
                ExperLkd = param.ExperienciaLinkedinId,
                Atribuicoes = param.Atribuicoes,
                CodColaborador = codColaborador
            };

            await conn.ExecuteAsync(qInsert, parametros);

            return await PerfilCorporativoListaPorId(pfCorpId);
        }

        public async Task<OrganogramaPerfilCorporativoListarPorIdResponseDTO> PerfilCorporativoAtualizar(OrganogramaPerfilCorporativoAtualizarParamDTO param, string codColaborador, int orgId)
        {
            var conn = _dapperConnection.GetConnection();

            var sqlExiste = @"
                    SELECT
                        EXISTS (SELECT 1 FROM tb_permanencia WHERE id = @PermanenciaId) AS PermanenciaExiste,
                        EXISTS (SELECT 1 FROM tb_modelo_trabalho WHERE id = @ModeloTrabalhoId) AS ModTrabalhoExiste,
                        EXISTS (SELECT 1 FROM tb_profissional_localidade WHERE id = @ProfissionalLocal) AS ProfLocalExiste,
                        EXISTS (SELECT 1 FROM tb_tipos_emprego_linkedin WHERE id = @EmpregoLkdin) AS EmprLkdExiste,
                        EXISTS (SELECT 1 FROM tb_niveis_experiencia_linkedin WHERE id = @ExperienciaLkd) AS ExpLkdExiste;
                    ";

            var result = await conn.QuerySingleAsync<(bool PermanenciaExiste, bool ModTrabalhoExiste, bool ProfLocalExiste, bool EmprLkdExiste, bool ExpLkdExiste, bool ColaboradorCriaExiste, bool ColaboradorAlteExiste)>(
                sqlExiste,
                new
                {
                    OrgId = orgId,
                    PermanenciaId = param.PermanenciaId,
                    ModeloTrabalhoId = param.ModeloTrabalhoId,
                    ProfissionalLocal = param.ProfissionalLocalidadeId,
                    EmpregoLkdin = param.EmpregoLinkdinId,
                    ExperienciaLkd = param.ExperienciaLinkedinId
                }
            );

            if (!result.PermanenciaExiste)
                throw new Exception($"Código Permanência: {param.PermanenciaId} não encontrado!");

            if (!result.ModTrabalhoExiste)
                throw new Exception($"Modelo Trabalho: {param.ModeloTrabalhoId} não encontrada!");

            if (!result.ProfLocalExiste)
                throw new Exception($"Profissional Localidade: {param.ProfissionalLocalidadeId} não encontrada!");

            if (!result.EmprLkdExiste)
                throw new Exception($"Tipo Emprego Linkedin: {param.EmpregoLinkdinId} não encontrada!");

            if (!result.ExpLkdExiste)
                throw new Exception($"Nível Experiência Linkedin: {param.ExperienciaLinkedinId} não encontrada!");

            var pfCorpId = Guid.NewGuid().ToString();

            const string qUpdate = @"
                                UPDATE tb_perfil_corporativo SET
                                     tb_org_id = @OrgId, 
                                     descricao = @Descricao,
                                     tb_permanencia_id = @PermanenciaId,
                                     tb_modelo_trabalho_id = @ModTrabalho,
                                     tb_profissional_localidade_id = @ProfLocal,
                                     tipo_emprego_linkedin = @EmprLkd,
                                     nivel_experiencia_linkedin = @ExperLkd,
                                     atribuicoes = @Atribuicoes,
                                     codigo_interno_colaborador_alteracao = @CodColaborador,
                                     ativo = @Ativo
                                WHERE id = @Id; ";

            await conn.ExecuteAsync(qUpdate, new
            {
                Id = param.Id,
                OrgId = orgId,
                Descricao = param.Descricao,
                PermanenciaId = param.PermanenciaId,
                ModTrabalho = param.ModeloTrabalhoId,
                ProfLocal = param.ProfissionalLocalidadeId,
                EmprLkd = param.EmpregoLinkdinId,
                ExperLkd = param.ExperienciaLinkedinId,
                Atribuicoes = param.Atribuicoes,
                Ativo = param.Ativo,
                CodColaborador = codColaborador
            });

            return await PerfilCorporativoListaPorId(param.Id);
        }

        public async Task<bool> PerfilCorporativoDeletar(string id)
        {
            var conn = _dapperConnection.GetConnection();
            return await conn.ExecuteAsync("UPDATE tb_perfil_corporativo SET ativo = 0 WHERE id = @Id;", new { Id = id }) > 0;
        }

        public async Task<OrganogramaPerfilCorporativoListarPorIdResponseDTO> PerfilCorporativoListaPorId(string buscaId)
        {
            var conn = _dapperConnection.GetConnection();

            var listaPerfil = await conn.QuerySingleOrDefaultAsync<OrganogramaPerfilCorporativoListarPorIdResponseDTO>(
                @"SELECT  CAST(id AS CHAR) AS Id, 
                          tb_org_id AS OrgId,
                          descricao AS Descricao,
                          CAST(tb_permanencia_id AS CHAR)  AS PermanenciaId,
                          CAST(tb_modelo_trabalho_id AS CHAR) AS ModeloTrabalhoId,
                          CAST(tb_profissional_localidade_id AS CHAR) AS ProfissionalLocalidadeId,
                          CAST(tipo_emprego_linkedin AS CHAR) AS EmpregoLinkdinId,
                          CAST(nivel_experiencia_linkedin AS CHAR) AS ExperienciaLinkedinId,
                          atribuicoes AS Atribuicoes,
                          codigo_interno_colaborador_criacao    AS CodigoInternoColaboradorCriacao,
                          codigo_interno_colaborador_alteracao 	AS CodigoInternoColaboradorAlteracao,
                          data_criacao   AS DataCriacao,
                          data_alteracao AS DataAlteracao,
                          ativo  AS Ativo
                FROM tb_perfil_corporativo
                WHERE id = @Id;"
                , new { Id = buscaId });

            return listaPerfil;
        }

        public async Task<IEnumerable<OrganogramaPerfilCorporativoListarPorIdResponseDTO>> BuscarPerfisPorOrgId(int orgId)
        {
            var conn = _dapperConnection.GetConnection();

            var listaPerfis = await conn.QueryAsync<OrganogramaPerfilCorporativoListarPorIdResponseDTO>(
                @"SELECT  CAST(id AS CHAR) AS Id, 
                          tb_org_id AS OrgId,
                          descricao AS Descricao,
                          CAST(tb_permanencia_id AS CHAR)  AS PermanenciaId,
                          CAST(tb_modelo_trabalho_id AS CHAR) AS ModeloTrabalhoId,
                          CAST(tb_profissional_localidade_id AS CHAR) AS ProfissionalLocalidadeId,
                          CAST(tipo_emprego_linkedin AS CHAR) AS EmpregoLinkdinId,
                          CAST(nivel_experiencia_linkedin AS CHAR) AS ExperienciaLinkedinId,
                          atribuicoes AS Atribuicoes,
                          codigo_interno_colaborador_criacao    AS CodigoInternoColaboradorCriacao,
                          codigo_interno_colaborador_alteracao 	AS CodigoInternoColaboradorAlteracao,
                          data_criacao   AS DataCriacao,
                          data_alteracao AS DataAlteracao,
                          ativo  AS Ativo
                FROM tb_perfil_corporativo
                WHERE tb_org_id = @OrgId;"
                , new { OrgId = orgId });

            return listaPerfis;
        }

        #endregion Perfil Corporativo


        #region Perfil Corporativo Skill

        public async Task<OrganogramaPerfilCorporativoSkillListarPorIdResponseDTO> PerfilCorporativoSkillInserir(OrganogramaPerfilCorporativoSkillInserirParamDTO param, string codColaborador, int orgId)
        {
            var conn = _dapperConnection.GetConnection();

            var sqlExiste = @"
                    SELECT
                        EXISTS (SELECT 1 FROM tb_perfil_corporativo WHERE id = @PfCorpId) AS PfCorpExiste,
                        EXISTS (SELECT 1 FROM tb_item_perfil WHERE id = @ItemPerfilId) AS ItemPerfilExiste,
                        EXISTS (SELECT 1 FROM tb_nivel WHERE id = @NivelId) AS NivelExiste,
                        EXISTS (SELECT 1 FROM vw_skills WHERE id = @SkillId AND tipo_id = @ItemPerfilId) AS SkillExiste;
                    ";

            var result = await conn.QuerySingleAsync<(bool PfCorpExiste, bool ItemPerfilExiste, bool NivelExiste, bool SkillExiste)>(
                sqlExiste,
                new
                {
                    OrgId = orgId,
                    PfCorpId = param.PerfilCorporativoId,
                    ItemPerfilId = param.ItemPerfilId,
                    NivelId = param.NivelId,
                    SkillId = param.SkillId
                }
            );

            if (!result.PfCorpExiste)
                throw new Exception($"Código Perfil Corporativo: {param.PerfilCorporativoId} não encontrado!");

            if (!result.ItemPerfilExiste)
                throw new Exception($"Item Perfil: {param.ItemPerfilId} não encontrada!");

            if (!result.SkillExiste)
                throw new Exception($"Skill: {param.SkillId} não encontrada!");
            
            if (!result.NivelExiste)
                throw new Exception($"Nível: {param.NivelId} não encontrada!");

            var pfCorpId = Guid.NewGuid().ToString();

            const string qInsert = @"
                                INSERT INTO tb_perfil_corporativo_skill
                                    (id,
                                     tb_org_id,
                                     tb_perfil_corporativo_id,
                                     tb_item_perfil_id,
                                     skill_id,
                                     tb_nivel_id,
                                     codigo_interno_colaborador_criacao
                                    )
                                VALUES
                                    (@Id, 
                                     @OrgId, 
                                     @PfCorpId, 
                                     @ItemPerfilId,
                                     @SkillId,
                                     @NivelId,
                                     @CodColaborador
                                    );
                                ";

            var parametros = new
            {
                Id = pfCorpId,
                OrgId = orgId,
                PfCorpId = param.PerfilCorporativoId,
                ItemPerfilId = param.ItemPerfilId,
                NivelId = param.NivelId,
                SkillId = param.SkillId,
                CodColaborador = codColaborador
            };

            await conn.ExecuteAsync(qInsert, parametros);

            return await PerfilCorporativoSkillListaPorId(pfCorpId);
        }

        public async Task<OrganogramaPerfilCorporativoSkillListarPorIdResponseDTO> PerfilCorporativoSkillAtualizar(OrganogramaPerfilCorporativoSkillAtualizarParamDTO param, string codColaborador, int orgId)
        {
            var conn = _dapperConnection.GetConnection();

            var sqlExiste = @"
                    SELECT
                        EXISTS (SELECT 1 FROM tb_org WHERE id = @OrgId) AS OrgExiste,
                        EXISTS (SELECT 1 FROM tb_perfil_corporativo WHERE id = @PfCorpId) AS PfCorpExiste,
                        EXISTS (SELECT 1 FROM tb_item_perfil WHERE id = @ItemPerfilId) AS ItemPerfilExiste,
                        EXISTS (SELECT 1 FROM tb_nivel WHERE id = @NivelId) AS NivelExiste,
                        EXISTS (SELECT 1 FROM vw_skills WHERE id = @SkillId AND tipo_id = @ItemPerfilId) AS SkillExiste;
                    ";

            var result = await conn.QuerySingleAsync<(bool PfCorpExiste, bool ItemPerfilExiste, bool NivelExiste, bool SkillExiste)>(
                sqlExiste,
                new
                {
                    OrgId = orgId,
                    PfCorpId = param.PerfilCorporativoId,
                    ItemPerfilId = param.ItemPerfilId,
                    NivelId = param.NivelId,
                    SkillId = param.SkillId
                }
            );

            if (!result.PfCorpExiste)
                throw new Exception($"Código Perfil Corporativo: {param.PerfilCorporativoId} não encontrado!");

            if (!result.ItemPerfilExiste)
                throw new Exception($"Item Perfil: {param.ItemPerfilId} não encontrada!");

            if (!result.SkillExiste)
                throw new Exception($"Skill: {param.SkillId} não encontrada!");

            if (!result.NivelExiste)
                throw new Exception($"Nível: {param.NivelId} não encontrada!");

            var pfCorpId = Guid.NewGuid().ToString();

            const string qUpdate = @"
                                UPDATE tb_perfil_corporativo_skill SET
                                     tb_org_id = @OrgId, 
                                     tb_perfil_corporativo_id = @PfCorpId,
                                     tb_item_perfil_id = @ItemPerfilId,
                                     skill_id = @SkillId,
                                     tb_nivel_id = @NivelId,
                                     codigo_interno_colaborador_alteracao = @CodColaborador,
                                     ativo = @Ativo
                                WHERE id = @Id; ";

            await conn.ExecuteAsync(qUpdate, new
            {
                Id = param.Id,
                OrgId = orgId,
                PfCorpId = param.PerfilCorporativoId,
                ItemPerfilId = param.ItemPerfilId,
                NivelId = param.NivelId,
                SkillId = param.SkillId,
                Ativo = param.Ativo,
                CodColaborador = codColaborador
            });

            return await PerfilCorporativoSkillListaPorId(param.Id);
        }

        public async Task<bool> PerfilCorporativoSkillDeletar(string id)
        {
            var conn = _dapperConnection.GetConnection();
            return await conn.ExecuteAsync("UPDATE tb_perfil_corporativo_skill SET ativo = 0 WHERE id = @Id;", new { Id = id }) > 0;
        }

        public async Task<OrganogramaPerfilCorporativoSkillListarPorIdResponseDTO> PerfilCorporativoSkillListaPorId(string buscaId)
        {
            var conn = _dapperConnection.GetConnection();

            var listaPerfil = await conn.QuerySingleOrDefaultAsync<OrganogramaPerfilCorporativoSkillListarPorIdResponseDTO>(
                @"SELECT  CAST(id AS CHAR) AS Id, 
                          tb_org_id AS OrgId,
                          CAST(tb_perfil_corporativo_id AS CHAR) AS PerfilCorporativoId,
                          tb_item_perfil_id AS ItemPerfilId,
                          skill_id AS SkillId,
                          tb_nivel_id AS NivelId,
                          codigo_interno_colaborador_criacao    AS CodigoInternoColaboradorCriacao,
                          codigo_interno_colaborador_alteracao 	AS CodigoInternoColaboradorAlteracao,
                          data_criacao   AS DataCriacao,
                          data_alteracao AS DataAlteracao,
                          ativo  AS Ativo
                FROM tb_perfil_corporativo_skill
                WHERE id = @Id;"
                , new { Id = buscaId });

            return listaPerfil;
        }

        #endregion Perfil Corporativo Skill

        #region Cliente Org
        public async Task<IEnumerable<OrganogramaClienteOrgDTO>> RetornarClientesPorOrgId(int orgId, int limite, int cursor, string busca)
        {
            var conn = _dapperConnection.GetConnection();

            const string sql = @"
                SELECT id AS Id,
                       tco.codigo_cliente AS CodigoCliente,
                       tco.nome_cliente AS NomeCliente
                FROM tb_cliente_org tco
                WHERE tco.tb_org_id = @OrgId
                  AND tco.ativo = 1
                  AND (@Busca IS NULL OR @Busca = '' OR tco.nome_cliente LIKE CONCAT('%', @Busca, '%') OR tco.codigo_cliente LIKE CONCAT('%', @Busca, '%'))
                LIMIT @Limite OFFSET @Cursor;";

            return await conn.QueryAsync<OrganogramaClienteOrgDTO>(sql, new
            {
                OrgId = orgId,
                Limite = limite,
                Cursor = cursor,
                Busca = busca
            });
        }
        #endregion Cliente Org

        #region Organograma Completo

        public async Task<OrganogramaCompletoResponseDTO> RetornarOrganogramaCompletoPorCliente(
    int orgId,
    string codigoCliente)
        {
            var conn = _dapperConnection.GetConnection();

            var sql = OrganogramaCompletoBaseSql + @"
                                    WHERE tpo.tb_org_id = @OrgId
                                      AND tpo.codigo_cliente = @CodigoCliente;";

            var rows = (await conn.QueryAsync<OrganogramaPosicaoCompletaRow>(
                sql,
                new { OrgId = orgId, CodigoCliente = codigoCliente }
            )).ToList();

            var posicoesPorId = new Dictionary<string, OrganogramaPosicaoCompletaDTO>();

            foreach (var row in rows)
            {
                if (row.PosicaoId == string.Empty)
                    continue;

                if (!posicoesPorId.TryGetValue(row.PosicaoId, out var posicao))
                {
                    posicao = new OrganogramaPosicaoCompletaDTO
                    {
                        Id = Guid.Parse(row.PosicaoId),
                        OrgId = row.OrgId,
                        DepartamentoId = string.IsNullOrWhiteSpace(row.DepartamentoId) ? null : Guid.Parse(row.DepartamentoId),
                        DepartamentoNome = row.DepartamentoNome,
                        CodigoCliente = row.CodigoCliente,
                        PerfilCorporativoId = row.PerfilCorporativoId,
                        PerfilCorporativoNome = row.PerfilCorporativoNome,
                        PosicaoIdSuperior = string.IsNullOrWhiteSpace(row.PosicaoIdSuperior) ? null : Guid.Parse(row.PosicaoIdSuperior),
                        Ativo = row.PosicaoAtivo,
                        DataCriacao = row.PosicaoDataCriacao,
                        DataAlteracao = row.PosicaoDataAlteracao,
                        CLevel = row.CLevel,
                        ProfissionalExterno = row.ProfissionalExterno,
                        MapaRelacionamentoInfluenciaId = row.MapaRelacionamentoInfluenciaId,
                        MapaRelacionamentoInfluenciaDescricao = row.MapaRelacionamentoInfluenciaDescricao,
                        Orcamento = row.PosicaoOrcamento,
                        OrcamentoDataInicio = row.PosicaoOrcamentoDataInicio,
                        OrcamentoDataFim = row.PosicaoOrcamentoDataFim,
                        Alocacoes = new List<OrganogramaPosicaoAlocacaoCompletaDTO>()
                    };

                    posicoesPorId.Add(row.PosicaoId, posicao);
                }

                // 🔹 Alocações
                if (!string.IsNullOrEmpty(row.AlocacaoId) &&
                    !posicao.Alocacoes.Any(a => a.Id == Guid.Parse(row.AlocacaoId)))
                {
                    posicao.Alocacoes.Add(new OrganogramaPosicaoAlocacaoCompletaDTO
                    {
                        Id = Guid.Parse(row.AlocacaoId),
                        TbPerfilCorporativoAlocacaoId = row.TbPerfilCorporativoAlocacaoId,
                        CodigoInternoColaborador = !string.IsNullOrEmpty(row.CodigoInternoColaborador) ? Guid.Parse(row.CodigoInternoColaborador) : Guid.Empty,
                        NomeColaborador = row.NomeColaborador,
                        DataInicio = row.DataInicio,
                        DataFim = row.DataFim,
                        Ativo = row.AlocacaoAtivo,
                        DataCriacao = row.AlocacaoDataCriacao,
                        DataAlteracao = row.AlocacaoDataAlteracao
                    });
                }
            }

            // ===== Montagem da hierarquia =====

            var posicoes = posicoesPorId.Values.ToList();
            var ordered = new List<OrganogramaPosicaoCompletaDTO>();
            var visited = new HashSet<Guid>();

            var childrenLookup = posicoes
                .GroupBy(p => p.PosicaoIdSuperior ?? Guid.Empty)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(p => p.DataCriacao ?? DateTime.MinValue)
                          .ThenBy(p => p.Id)
                          .ToList());

            void AddWithChildren(OrganogramaPosicaoCompletaDTO posicao)
            {
                if (posicao == null || !visited.Add(posicao.Id))
                    return;

                ordered.Add(posicao);

                if (childrenLookup.TryGetValue(posicao.Id, out var children))
                {
                    foreach (var child in children)
                        AddWithChildren(child);
                }
            }

            var roots = posicoes
                .Where(p => p.PosicaoIdSuperior == null ||
                            !posicoesPorId.ContainsKey(p.PosicaoIdSuperior.Value.ToString()))
                .OrderBy(p => p.DataCriacao ?? DateTime.MinValue)
                .ThenBy(p => p.Id)
                .ToList();

            foreach (var root in roots)
                AddWithChildren(root);

            foreach (var posicao in posicoes)
                if (!visited.Contains(posicao.Id))
                    AddWithChildren(posicao);

            return new OrganogramaCompletoResponseDTO
            {
                OrgId = orgId,
                CodigoCliente = codigoCliente,
                Posicoes = ordered
            };
        }

        public async Task<OrganogramaCompletoResponseDTO> RetornarOrganogramaCompletoPorClienteCLevel(
            int orgId,
            string codigoCliente)
        {
            var conn = _dapperConnection.GetConnection();

            var sql = OrganogramaCompletoBaseSql + @"
                                    WHERE tpo.tb_org_id = @OrgId
                                      AND tpo.codigo_cliente = @CodigoCliente
                                      AND tpo.c_level = 1;";

            var rows = (await conn.QueryAsync<OrganogramaPosicaoCompletaRow>(
                sql,
                new { OrgId = orgId, CodigoCliente = codigoCliente }
            )).ToList();

            var posicoesPorId = new Dictionary<string, OrganogramaPosicaoCompletaDTO>();

            foreach (var row in rows)
            {
                if (row.PosicaoId == string.Empty)
                    continue;

                if (!posicoesPorId.TryGetValue(row.PosicaoId, out var posicao))
                {
                    posicao = new OrganogramaPosicaoCompletaDTO
                    {
                        Id = Guid.Parse(row.PosicaoId),
                        OrgId = row.OrgId,
                        DepartamentoId = string.IsNullOrWhiteSpace(row.DepartamentoId) ? null : Guid.Parse(row.DepartamentoId),
                        DepartamentoNome = row.DepartamentoNome,
                        CodigoCliente = row.CodigoCliente,
                        PerfilCorporativoId = row.PerfilCorporativoId,
                        PerfilCorporativoNome = row.PerfilCorporativoNome,
                        PosicaoIdSuperior = string.IsNullOrWhiteSpace(row.PosicaoIdSuperior) ? null : Guid.Parse(row.PosicaoIdSuperior),
                        Ativo = row.PosicaoAtivo,
                        DataCriacao = row.PosicaoDataCriacao,
                        DataAlteracao = row.PosicaoDataAlteracao,
                        CLevel = row.CLevel,
                        ProfissionalExterno = row.ProfissionalExterno,
                        MapaRelacionamentoInfluenciaId = row.MapaRelacionamentoInfluenciaId,
                        MapaRelacionamentoInfluenciaDescricao = row.MapaRelacionamentoInfluenciaDescricao,
                        Orcamento = row.PosicaoOrcamento,
                        OrcamentoDataInicio = row.PosicaoOrcamentoDataInicio,
                        OrcamentoDataFim = row.PosicaoOrcamentoDataFim,
                        Alocacoes = new List<OrganogramaPosicaoAlocacaoCompletaDTO>()
                    };

                    posicoesPorId.Add(row.PosicaoId, posicao);
                }

                if (!string.IsNullOrEmpty(row.AlocacaoId) &&
                    !posicao.Alocacoes.Any(a => a.Id == Guid.Parse(row.AlocacaoId)))
                {
                    posicao.Alocacoes.Add(new OrganogramaPosicaoAlocacaoCompletaDTO
                    {
                        Id = Guid.Parse(row.AlocacaoId),
                        TbPerfilCorporativoAlocacaoId = row.TbPerfilCorporativoAlocacaoId,
                        CodigoInternoColaborador = !string.IsNullOrEmpty(row.CodigoInternoColaborador) ? Guid.Parse(row.CodigoInternoColaborador) : Guid.Empty,
                        NomeColaborador = row.NomeColaborador,
                        DataInicio = row.DataInicio,
                        DataFim = row.DataFim,
                        Ativo = row.AlocacaoAtivo,
                        DataCriacao = row.AlocacaoDataCriacao,
                        DataAlteracao = row.AlocacaoDataAlteracao
                    });
                }
            }

            var posicoes = posicoesPorId.Values.ToList();
            var ordered = new List<OrganogramaPosicaoCompletaDTO>();
            var visited = new HashSet<Guid>();

            var childrenLookup = posicoes
                .GroupBy(p => p.PosicaoIdSuperior ?? Guid.Empty)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(p => p.DataCriacao ?? DateTime.MinValue)
                          .ThenBy(p => p.Id)
                          .ToList());

            void AddWithChildren(OrganogramaPosicaoCompletaDTO posicao)
            {
                if (posicao == null || !visited.Add(posicao.Id))
                    return;

                ordered.Add(posicao);

                if (childrenLookup.TryGetValue(posicao.Id, out var children))
                {
                    foreach (var child in children)
                        AddWithChildren(child);
                }
            }

            var roots = posicoes
                .Where(p => p.PosicaoIdSuperior == null ||
                            !posicoesPorId.ContainsKey(p.PosicaoIdSuperior.Value.ToString()))
                .OrderBy(p => p.DataCriacao ?? DateTime.MinValue)
                .ThenBy(p => p.Id)
                .ToList();

            foreach (var root in roots)
                AddWithChildren(root);

            foreach (var posicao in posicoes)
                if (!visited.Contains(posicao.Id))
                    AddWithChildren(posicao);

            return new OrganogramaCompletoResponseDTO
            {
                OrgId = orgId,
                CodigoCliente = codigoCliente,
                Posicoes = ordered
            };
        }

        #endregion Organograma Completo
    }
}