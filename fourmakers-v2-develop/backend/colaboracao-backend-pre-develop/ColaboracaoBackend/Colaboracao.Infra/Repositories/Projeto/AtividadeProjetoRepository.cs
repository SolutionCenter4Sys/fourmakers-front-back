using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Context;
using Core.Domain.Projeto;
using Dapper;
using DataTransferObject.Domain.Projeto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Projeto
{
    public class AtividadeProjetoRepository : IAtividadeProjetoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        private readonly IDBConnection _dapperConnection;
        
        public AtividadeProjetoRepository(ColaboradorContext colaboradorContext, IDBConnection dapperConnection)
        {
            _colaboradorContext = colaboradorContext;
            _dapperConnection = dapperConnection;
        }
        public async Task<List<AtividadeProjetoDTO>> ListarAtividades(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT
                    id AS Id,
                    descricao AS Descricao
                FROM tb_atividade
                WHERE tb_org_id = @orgId
                AND ativo = 1
                ORDER BY descricao
            ";

            var parametros = new
            {
                OrgId = orgId
            };
            
            var result = await connection.QueryAsync<AtividadeProjetoDTO>(query, parametros);

            return result.ToList();
        }

        public async Task<List<AtividadeProjetoDTO>> CadastroAtividades(int orgId, List<string> atividades, string codProjeto)
        {
            var connection = _dapperConnection.GetConnection();    
            try
            {
                // Buscar atividades já existentes no banco para a organização
                string sqlAtividadesExistentes = @"
                    SELECT *
                    FROM tb_atividade
                    WHERE tb_org_id = @OrgId
                    AND descricao IN @ListaDescricao";

                var atividadesExistentesDB = await connection.QueryAsync<tb_atividade>(sqlAtividadesExistentes, new
                {
                    OrgId = orgId,
                    ListaDescricao = atividades
                });

                var atividadesExistentes = atividadesExistentesDB.ToList();

                // Descobrir quais são as novas atividades (case insensitive)
                var descricoesExistentes = atividadesExistentes
                    .Select(a => a.descricao.ToUpperInvariant())
                    .ToList();

                var novasDescricoes = atividades
                    .Where(descricao => !descricoesExistentes.Contains(descricao.ToUpperInvariant()))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var novasAtividades = new List<tb_atividade>();

                // Inserir novas atividades via Dapper
                if (novasDescricoes.Any())
                {
                    foreach (var descricao in novasDescricoes)
                    {
                        var novaAtividade = new tb_atividade
                        {
                            id = Guid.NewGuid(),
                            descricao = descricao,
                            tb_org_id = orgId
                        };

                        string sqlInsert = @"
                            INSERT INTO tb_atividade (id, descricao, tb_org_id)
                            VALUES (@Id, @Descricao, @OrgId);";

                        await connection.ExecuteAsync(sqlInsert, new
                        {
                            Id = novaAtividade.id,
                            Descricao = novaAtividade.descricao,
                            OrgId = novaAtividade.tb_org_id
                        });

                        novasAtividades.Add(novaAtividade);
                    }
                }

                // Montar lista de DTOs para retorno
                List<AtividadeProjetoDTO> associacaoAtividades = atividadesExistentes
                    .Concat(novasAtividades)
                    .Select(x => new AtividadeProjetoDTO()
                    {
                        Descricao = x.descricao,
                        Id = x.id
                    }).ToList();

                // Associar atividades ao projeto, se necessário
                if (!string.IsNullOrEmpty(codProjeto))
                {
                    // Buscar atividades já associadas ao projeto
                    string sqlAtividadesAssociadas = @"
                        SELECT tb_atividade_id
                        FROM tb_projeto_org_atividade
                        WHERE tb_projeto_org_cod_projeto = @CodProjeto
                        AND tb_projeto_tb_org_id = @OrgId
                        AND tb_atividade_id IN @IdsAtividades";

                    var idsAtividades = associacaoAtividades.Select(a => a.Id).ToList();

                    var atividadesAssociadas = await connection.QueryAsync<Guid>(sqlAtividadesAssociadas, new
                    {
                        CodProjeto = codProjeto,
                        OrgId = orgId,
                        IdsAtividades = idsAtividades
                    });

                    var idsAtividadesAssociadas = atividadesAssociadas.ToList();

                    var idsAtividadesNaoAssociadas = idsAtividades
                        .Except(idsAtividadesAssociadas)
                        .ToList();

                    if (idsAtividadesNaoAssociadas.Any())
                    {
                        string sqlInsertAssociacao = @"
                            INSERT INTO tb_projeto_org_atividade (id, tb_atividade_id, tb_projeto_org_cod_projeto, tb_projeto_tb_org_id)
                            VALUES (@Id, @AtividadeId, @CodProjeto, @OrgId);";

                        foreach (var idAtividade in idsAtividadesNaoAssociadas)
                        {
                            await connection.ExecuteAsync(sqlInsertAssociacao, new
                            {
                                Id = Guid.NewGuid(),
                                AtividadeId = idAtividade,
                                CodProjeto = codProjeto,
                                OrgId = orgId
                            });
                        }
                    }
                }

                return associacaoAtividades;
            }
            catch
            {
                throw;
            }
        }

        public async Task AssociarAtividadesComProjeto(int orgId, List<AtividadeProjetoDTO> atividadesCadastradas, string codProjeto)
        {
            try
            {
                var atividadesAssociadasExistentes = await _colaboradorContext.tb_projeto_org_atividade
                                                .Where(a => a.tb_projeto_org_cod_projeto == codProjeto
                                                 && a.tb_projeto_tb_org_id == orgId
                                                 && a.tb_atividade_id.Equals(atividadesCadastradas
                                                .Select(a => a.Id)))
                                                .ToListAsync();

                var idsAtividadesNaoInseridos = atividadesCadastradas
                                                .Select(a => a.Id)
                                                .Except(atividadesAssociadasExistentes.Select(a => a.id))
                                                .ToList();

                if (idsAtividadesNaoInseridos.Any())
                {
                    var novasAssociacoes = idsAtividadesNaoInseridos
                                           .Select(id => new tb_projeto_org_atividade
                                           {
                                               id = Guid.NewGuid(),
                                               tb_atividade_id = id,
                                               tb_projeto_org_cod_projeto = codProjeto,
                                               tb_projeto_tb_org_id = orgId
                                           })
                                           .ToList();

                    _colaboradorContext.tb_projeto_org_atividade.AddRange(novasAssociacoes);
                    _colaboradorContext.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RemoverAtividadesAssociadasComProjeto(int orgId, List<string> atividades, string codProjeto)
        {
            try
            {
                var atividadesAssociadasExistentes = _colaboradorContext.tb_projeto_org_atividade
                    .Where(a => a.tb_projeto_org_cod_projeto == codProjeto
                        && a.tb_projeto_tb_org_id == orgId)
                    .Select(i => new tb_projeto_org_atividade
                    {
                        id = i.id,
                        tb_projeto_org_cod_projeto = i.tb_projeto_org_cod_projeto,
                        tb_projeto_tb_org_id = i.tb_projeto_tb_org_id,
                        tb_atividade_id = i.tb_atividade_id
                    }).ToList();

                if (atividadesAssociadasExistentes.Any())
                {
                    var idsAtividadesParaRemover = atividadesAssociadasExistentes.Select(p => p.id).ToList();

                    var atividadesParaRemover = _colaboradorContext.tb_projeto_org_atividade
                        .Where(a => a.tb_projeto_org_cod_projeto == codProjeto
                            && a.tb_projeto_tb_org_id == orgId
                              && idsAtividadesParaRemover.Contains(a.id));

                    _colaboradorContext.tb_projeto_org_atividade.RemoveRange(atividadesParaRemover);
                    _colaboradorContext.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task RemoverAtividadesAssociadasComProjetoAsync(int orgId, List<string> atividades, string codProjeto)
        {
            if (atividades == null || !atividades.Any())
                return; // Nenhuma atividade informada, nada a remover

            var connection = _dapperConnection.GetConnection();

            // 1️⃣ Buscar IDs das atividades associadas ao projeto
            var atividadesExistentesIds = (await connection.QueryAsync<Guid>(
                @"SELECT tpoa.id
                      FROM tb_projeto_org_atividade tpoa
                      INNER JOIN tb_atividade ta ON ta.id = tpoa.tb_atividade_id 
                      WHERE tpoa.tb_projeto_org_cod_projeto = @CodProjeto
                        AND tpoa.tb_projeto_tb_org_id = @OrgId
            ",
                new { CodProjeto = codProjeto, OrgId = orgId }
            )).ToList();

            // 2️⃣ Se existirem, remover
            if (!atividadesExistentesIds.Any())
                return; // Nada a remover
            
            var atividadesParaCompararIds = (await connection.QueryAsync<Guid>(
                @"SELECT tpoa.id
                      FROM tb_projeto_org_atividade tpoa
                      INNER JOIN tb_atividade ta ON ta.id = tpoa.tb_atividade_id 
                      WHERE tpoa.tb_projeto_org_cod_projeto = @CodProjeto
                        AND tpoa.tb_projeto_tb_org_id = @OrgId
                        AND ta.descricao IN @Atividades;
            ",
                new { CodProjeto = codProjeto, OrgId = orgId, Atividades = atividades }
            )).ToList();

            if (!atividadesParaCompararIds.Any())
                return;

            var atividadesParaRemoverIds = atividadesExistentesIds
                .Except(atividadesParaCompararIds)
                .ToList();

            if (!atividadesParaRemoverIds.Any())
                return;
            var queryDelete = @"
                DELETE FROM tb_projeto_org_atividade
                WHERE tb_projeto_org_cod_projeto = @CodProjeto
                AND tb_projeto_tb_org_id = @OrgId
                AND id IN @Ids
            ";

            await connection.ExecuteAsync(
                queryDelete,
                new { Ids = atividadesParaRemoverIds, CodProjeto = codProjeto, OrgId = orgId }
            );
        }

        public Task<List<AtividadeProjetoDTO>> ListarAtividadesAssociadasComProjeto(string codProjeto, int orgId)
        {
            var row = _colaboradorContext.tb_projeto_org_atividade
                .Join(_colaboradorContext.tb_atividade, tpoa => tpoa.tb_atividade_id, ta => ta.id, (tpoa, ta) => new { tpoa, ta })
                .Where(x => x.tpoa.tb_projeto_tb_org_id == orgId
                            && x.ta.tb_org_id == orgId
                            && x.tpoa.tb_projeto_org_cod_projeto == codProjeto)
                .Select(x => new AtividadeProjetoDTO()
                {
                    Id = x.ta.id,
                    Descricao = x.ta.descricao
                }).ToListAsync();
            return row;
        }
    }
}