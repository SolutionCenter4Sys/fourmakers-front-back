using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain;
using Dapper;
using DataTransferObject.Domain.Classificacao.Candidato;
using DataTransferObject.Domain.Classificacao.Perfil;

namespace Colaboracao.Infra.Repositories;

public class ClassificacaoRepository(IDBConnection dapperConnection) : IClassificacaoRepository
{
    private readonly IDbConnection _connection =  dapperConnection.GetConnection();

    public async Task<List<CandidatoClassificacaoDTO>> ListarCandidatoClassificacaoPorListaDeCodigoInterno(List<string> codigosInternos)
    {
        // ============================================
        // 1. Buscar dados principais do colaborador
        // ============================================
        var colaboradores = (await _connection.QueryAsync<CandidatoClassificacaoDTO>(
            @"SELECT 
                tc.codigo_interno_colaborador AS CodigoInternoColaborador,
                tc.nome_completo AS NomeCompleto,
                tcs.descricao AS Sobre
              FROM tb_colaborador tc
              LEFT JOIN tb_colaborador_sobre tcs 
                ON tcs.codigo_interno_colaborador = tc.codigo_interno_colaborador
              WHERE tc.codigo_interno_colaborador IN @CodigosInternos",
            new { CodigosInternos = codigosInternos }
        )).ToList();

        if (!colaboradores.Any())
            return [];

        // ============================================
        // 2. Buscar Skills
        // ============================================
        var skills = await _connection.QueryAsync<CandidatoSkillClassificacaoDTO>(
            @"SELECT 
                tcc.codigo_interno_colaborador AS CodigoInternoColaborador,
                tc2.descricao AS Nome
              FROM tb_colaborador_competencia tcc
              INNER JOIN tb_competencia tc2 
                    ON tc2.id = tcc.competencia_id AND tc2.ativo = 1
              WHERE tcc.ativo = 1 
                AND tcc.codigo_interno_colaborador IN @CodigosInternos",
            new { CodigosInternos = codigosInternos }
        );

        // ============================================
        // 3. Buscar Experiências
        // ============================================
        var experiencias = await _connection.QueryAsync<CandidatoClassificacaoExperienciaDTO>(
            @"SELECT 
                codigo_interno_colaborador AS CodigoInternoColaborador,
                id AS Id,
                titulo AS Titulo,
                descricao AS Descricao,
                empresa AS Empresa,
                data_inicio AS DataInicio,
                data_saida AS DataSaida,
                atual AS Atual
              FROM tb_experiencia
              WHERE ativo = 1
                AND codigo_interno_colaborador IN @CodigosInternos",
            new { CodigosInternos = codigosInternos }
        );

        // ============================================
        // 4. Montar DTO final
        // ============================================
        foreach (var colaborador in colaboradores)
        {
            colaborador.HardSkills = skills
                .Where(x => x.CodigoInternoColaborador == colaborador.CodigoInternoColaborador)
                .ToList();

            colaborador.Experiencias = experiencias
                .Where(x => x.CodigoInternoColaborador == colaborador.CodigoInternoColaborador)
                .ToList();
        }

        return colaboradores;
    }

    private async Task<bool> ExisteClassificacaoProfissionalAsync(string codigoInternoColaborador)
    {
        var query = @"
            SELECT CASE WHEN EXISTS (
                SELECT 1 FROM tb_classificacao_profissional 
                WHERE codigo_interno_colaborador = @CodigoInternoColaborador
            ) THEN 1 ELSE 0 END AS Existe;
        ";

        var parametros = new
        {
            CodigoInternoColaborador = codigoInternoColaborador
        };
        
        var result = await _connection.QuerySingleOrDefaultAsync<bool>(query, parametros);
        return result;
    }

    public async Task AtualizarClassificacaoProfissionalColaborador(string codigoInternoColaborador, double score, string categoria)
    {
        var existe = await ExisteClassificacaoProfissionalAsync(codigoInternoColaborador);
        if (!existe)
        {
            var query = @"
                INSERT INTO tb_classificacao_profissional
                    (id, codigo_interno_colaborador, categoria, score)
                VALUES
                    (@Id, @CodigoInternoColaborador, @Categoria, @Score);
            ";

            var parametros = new
            {
                Id = Guid.NewGuid(),
                CodigoInternoColaborador = codigoInternoColaborador,
                Categoria = categoria,
                Score = score
            };
            
            await _connection.ExecuteAsync(query, parametros);
        }
        else
        {
            var query = @"
                UPDATE tb_classificacao_profissional
                SET 
                    score = @Score,
                    categoria = @Categoria
                WHERE
                    codigo_interno_colaborador = @CodigoInternoColaborador
            ";

            var parametros = new
            {
                CodigoInternoColaborador = codigoInternoColaborador,
                Categoria = categoria,
                Score = score
            };
            
            await _connection.ExecuteAsync(query, parametros);
        }
    }

    public async Task<PerfilClassificacaoDTO> ObterPerfilClassificacaoPorId(Guid gestorExternoPerfilId)
    {
        // Buscar dados do perfil
        var perfil = await _connection.QueryFirstOrDefaultAsync<dynamic>(
            @"SELECT 
                tgep.id AS Id,
                tgep.nome_perfil AS NomePerfil,
                COALESCE(CONVERT(tgep.informacoes_relevantes USING utf8mb4), '') AS InformacoesRelevantes,
                COALESCE(CONVERT(tgep.atribuicoes USING utf8mb4), '') AS Atribuicoes
              FROM tb_gestor_externo_perfil tgep
              WHERE tgep.id = @GestorExternoPerfilId AND tgep.ativo = 1",
            new { GestorExternoPerfilId = gestorExternoPerfilId }
        );

        if (perfil == null)
            return null;

        // Buscar skills do perfil
        var skills = await _connection.QueryAsync<PerfilSkillClassificacaoDTO>(
            @"SELECT 
                tgeps.skill_id AS SkillId,
                tgeps.tb_nivel_id AS NivelId,
                tgeps.tb_item_perfil_id AS ItemPerfilId,
                COALESCE(tgeps.relevante, 0) AS Relevante
              FROM tb_gestor_externo_perfil_skill tgeps
              WHERE tgeps.tb_gestor_externo_perfil_id = @GestorExternoPerfilId",
            new { GestorExternoPerfilId = gestorExternoPerfilId }
        );

        var perfilDTO = new PerfilClassificacaoDTO
        {
            IdVaga = perfil.Id?.ToString() ?? gestorExternoPerfilId.ToString(),
            CodVaga = null,
            Titulo = perfil.NomePerfil ?? string.Empty,
            Descricao = $"{perfil.InformacoesRelevantes ?? string.Empty} {perfil.Atribuicoes ?? string.Empty}".Trim(),
            Cargo = perfil.NomePerfil ?? string.Empty,
            Skills = skills.ToList()
        };

        return perfilDTO;
    }

    private async Task<bool> ExisteClassificacaoPerfilAsync(Guid gestorExternoPerfilId)
    {
        var query = @"
            SELECT CASE WHEN EXISTS (
                SELECT 1 FROM tb_classificacao_perfil 
                WHERE tb_gestor_externo_perfil_id = @GestorExternoPerfilId
            ) THEN 1 ELSE 0 END AS Existe;
        ";

        var parametros = new
        {
            GestorExternoPerfilId = gestorExternoPerfilId
        };
        
        var result = await _connection.QuerySingleOrDefaultAsync<bool>(query, parametros);
        return result;
    }

    public async Task AtualizarClassificacaoPerfil(Guid gestorExternoPerfilId, double score, string categoria)
    {
        var existe = await ExisteClassificacaoPerfilAsync(gestorExternoPerfilId);
        if (!existe)
        {
            var query = @"
                INSERT INTO tb_classificacao_perfil
                    (id, tb_gestor_externo_perfil_id, categoria, score)
                VALUES
                    (@Id, @GestorExternoPerfilId, @Categoria, @Score);
            ";

            var parametros = new
            {
                Id = Guid.NewGuid().ToString(),
                GestorExternoPerfilId = gestorExternoPerfilId.ToString(),
                Categoria = categoria,
                Score = score
            };
            
            await _connection.ExecuteAsync(query, parametros);
        }
        else
        {
            var query = @"
                UPDATE tb_classificacao_perfil
                SET 
                    score = @Score,
                    categoria = @Categoria
                WHERE
                    tb_gestor_externo_perfil_id = @GestorExternoPerfilId
            ";

            var parametros = new
            {
                GestorExternoPerfilId = gestorExternoPerfilId.ToString(),
                Categoria = categoria,
                Score = score
            };
            
            await _connection.ExecuteAsync(query, parametros);
        }
    }

    public async Task<PerfilClassificacaoDTO> ObterVagaClassificacaoPorId(string vagaId)
    {
        // Buscar dados da vaga
        var vaga = await _connection.QueryFirstOrDefaultAsync<dynamic>(
            @"SELECT 
                tv.id AS Id,
                tv.codigo AS Codigo,
                tv.titulo AS Titulo,
                COALESCE(CONVERT(tv.descricao USING utf8mb4), '') AS Descricao,
                tv.cargo AS Cargo
              FROM tb_vaga tv
              WHERE tv.id = @VagaId",
            new { VagaId = vagaId }
        );

        if (vaga == null)
            return null;

        // Buscar skills da vaga
        var skills = await _connection.QueryAsync<PerfilSkillClassificacaoDTO>(
            @"SELECT 
                tvs.skill_id AS SkillId,
                tvs.skill_nivel_id AS NivelId,
                tvs.tb_item_perfil_id AS ItemPerfilId,
                COALESCE(tvs.relevante, 0) AS Relevante
              FROM tb_vaga_skill tvs
              WHERE tvs.tb_vaga_id = @VagaId AND tvs.ativo = 1",
            new { VagaId = vagaId }
        );

        var vagaDTO = new PerfilClassificacaoDTO
        {
            IdVaga = vaga.Id?.ToString() ?? vagaId,
            CodVaga = vaga.Codigo != null ? (int?)Convert.ToInt32(vaga.Codigo) : null,
            Titulo = vaga.Titulo ?? string.Empty,
            Descricao = vaga.Descricao ?? string.Empty,
            Cargo = vaga.Cargo ?? string.Empty,
            Skills = skills.ToList()
        };

        return vagaDTO;
    }

    private async Task<bool> ExisteClassificacaoVagaAsync(string vagaId)
    {
        var query = @"
            SELECT CASE WHEN EXISTS (
                SELECT 1 FROM tb_classificacao_vaga 
                WHERE tb_vaga_id = @VagaId
            ) THEN 1 ELSE 0 END AS Existe;
        ";

        var parametros = new
        {
            VagaId = vagaId
        };
        
        var result = await _connection.QuerySingleOrDefaultAsync<bool>(query, parametros);
        return result;
    }

    public async Task<bool> ExisteClassificacaoVaga(string vagaId)
    {
        return await ExisteClassificacaoVagaAsync(vagaId);
    }

    public async Task<string> ObterCategoriaVagaPorId(string vagaId)
    {
        var query = @"
            SELECT categoria 
            FROM tb_classificacao_vaga 
            WHERE tb_vaga_id = @VagaId
            LIMIT 1;
        ";

        var parametros = new
        {
            VagaId = vagaId
        };
        
        var result = await _connection.QueryFirstOrDefaultAsync<string>(query, parametros);
        return result;
    }

    public async Task AtualizarClassificacaoVaga(string vagaId, double score, string categoria)
    {
        var existe = await ExisteClassificacaoVagaAsync(vagaId);
        if (!existe)
        {
            var query = @"
                INSERT INTO tb_classificacao_vaga
                    (id, tb_vaga_id, categoria, score)
                VALUES
                    (@Id, @VagaId, @Categoria, @Score);
            ";

            var parametros = new
            {
                Id = Guid.NewGuid().ToString(),
                VagaId = vagaId,
                Categoria = categoria,
                Score = score
            };
            
            await _connection.ExecuteAsync(query, parametros);
        }
        else
        {
            var query = @"
                UPDATE tb_classificacao_vaga
                SET 
                    score = @Score,
                    categoria = @Categoria
                WHERE
                    tb_vaga_id = @VagaId
            ";

            var parametros = new
            {
                VagaId = vagaId,
                Categoria = categoria,
                Score = score
            };
            
            await _connection.ExecuteAsync(query, parametros);
        }
    }
}