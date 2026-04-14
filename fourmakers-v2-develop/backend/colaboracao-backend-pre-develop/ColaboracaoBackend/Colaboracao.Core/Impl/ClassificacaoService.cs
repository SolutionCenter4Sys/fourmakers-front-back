using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Core.Domain;
using DataTransferObject.Domain.Classificacao.Perfil;
using DataTransferObject.Domain.Vaga;

namespace Colaboracao.Core.Impl;

public class ClassificacaoService(
    IClassificacaoRepository classificacaoRepository,
    IClassificacaoClient classificacaoClient,
    IElasticSearchClient elasticSearchClient)
    : IClassificacaoService
{
    public Task AtualizarClassificacaoProfissionalPorCodigoInternoColaboradorAsync(string codigoInternoColaborador)
        => AtualizarClassificacaoProfissionalPorListaDeCodigoInternoColaboradorAsync(new List<string> { codigoInternoColaborador });

    public void AtualizarClassificacaoProfissionalPorCodigoInternoColaborador(string codigoInternoColaborador)
    {
        AtualizarClassificacaoProfissionalPorCodigoInternoColaboradorAsync(codigoInternoColaborador).GetAwaiter().GetResult();
    }

    public async Task AtualizarClassificacaoProfissionalPorListaDeCodigoInternoColaboradorAsync(List<string> codigosInternos)
    {
        try
        {
            var candidatos = await classificacaoRepository
                .ListarCandidatoClassificacaoPorListaDeCodigoInterno(codigosInternos);

            var classificacaoResultado = await classificacaoClient
                .ClassificarCandidatosAsync(candidatos);

            foreach (var candidato in candidatos)
            {
                var classificacao = classificacaoResultado.CandidatoClassificados
                    .FirstOrDefault(x => x.IdColaborador == candidato.CodigoInternoColaborador);

                if (classificacao == null)
                    continue;

                await classificacaoRepository.AtualizarClassificacaoProfissionalColaborador(
                    candidato.CodigoInternoColaborador, 
                    classificacao.ScoreCosseno,           
                    classificacao.Categoria);
                var tokenElastichSearch =
                    VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_ELASTIC_SEARCH);
                if (tokenElastichSearch.ToNullSeTextoNull() != null)
                {
                    await elasticSearchClient.SyncColaborador(candidato.CodigoInternoColaborador, tokenElastichSearch);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao classificar usuário", ex);
        }
    }

    public async Task AtualizarClassificacaoPerfilAsync(Guid gestorExternoPerfilId)
    {
        try
        {
            var perfil = await classificacaoRepository.ObterPerfilClassificacaoPorId(gestorExternoPerfilId);

            if (perfil == null)
                return;

            var classificacaoResultado = await classificacaoClient.ClassificarPerfisAsync(new List<PerfilClassificacaoDTO> { perfil });

            var classificacao = classificacaoResultado.VagasClassificadas
                .FirstOrDefault(x => x.IdVaga == perfil.IdVaga);

            if (classificacao == null)
                return;

            await classificacaoRepository.AtualizarClassificacaoPerfil(
                gestorExternoPerfilId,
                classificacao.ScoreCosseno,
                classificacao.Categoria);
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao classificar perfil", ex);
        }
    }

    public async Task<string> ClassificarPerfilExtraidoAsync(PerfilExtraido perfilExtraido)
    {
        try
        {
            if (perfilExtraido == null)
                return string.Empty;

            var perfilClassificacao = ConverterPerfilExtraidoParaClassificacao(perfilExtraido);

            // Classificar o perfil e obter a categoria (comunidade)
            var classificacaoResultado = await classificacaoClient.ClassificarPerfisAsync(new List<PerfilClassificacaoDTO> { perfilClassificacao });

            // Buscar a categoria do resultado da classificação
            var classificacao = classificacaoResultado.VagasClassificadas?.FirstOrDefault();
            
            return classificacao?.Categoria ?? string.Empty;
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao classificar perfil extraído", ex);
        }
    }

    public async Task AtualizarClassificacaoVagaAsync(string vagaId)
    {
        try
        {
            var vaga = await classificacaoRepository.ObterVagaClassificacaoPorId(vagaId);

            if (vaga == null)
                return;

            var classificacaoResultado = await classificacaoClient.ClassificarPerfisAsync(new List<PerfilClassificacaoDTO> { vaga });

            var classificacao = classificacaoResultado.VagasClassificadas
                .FirstOrDefault(x => x.IdVaga == vaga.IdVaga);

            if (classificacao == null)
                return;

            await classificacaoRepository.AtualizarClassificacaoVaga(
                vagaId,
                classificacao.ScoreCosseno,
                classificacao.Categoria);
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao classificar vaga", ex);
        }
    }

    public async Task AtualizarClassificacaoVagaAsync(DataTransferObject.Domain.Vaga.VagaRecrutamentoDTO vaga)
    {
        try
        {
            if (vaga == null)
                return;

            var vagaClassificacao = ConverterVagaParaClassificacao(vaga);

            var classificacaoResultado = await classificacaoClient.ClassificarPerfisAsync(new List<PerfilClassificacaoDTO> { vagaClassificacao });

            var classificacao = classificacaoResultado.VagasClassificadas
                .FirstOrDefault(x => x.IdVaga == vagaClassificacao.IdVaga);

            if (classificacao == null)
                return;

            await classificacaoRepository.AtualizarClassificacaoVaga(
                vaga.Id,
                classificacao.ScoreCosseno,
                classificacao.Categoria);
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao classificar vaga", ex);
        }
    }

    private PerfilClassificacaoDTO ConverterVagaParaClassificacao(DataTransferObject.Domain.Vaga.VagaRecrutamentoDTO vaga)
    {
        var skills = vaga.Skills?.Select(s => new PerfilSkillClassificacaoDTO
        {
            SkillId = s.SkillId,
            NivelId = (int)s.SkillNivelId,
            ItemPerfilId = s.TipoSkillId,
            Relevante = s.Relevante
        }).ToList() ?? new List<PerfilSkillClassificacaoDTO>();

        return new PerfilClassificacaoDTO
        {
            IdVaga = vaga.Id,
            CodVaga = (int?)vaga.Codigo,
            Titulo = vaga.Titulo ?? string.Empty,
            Descricao = vaga.Descricao ?? string.Empty,
            Cargo = vaga.Cargo ?? string.Empty,
            Skills = skills
        };
    }

    private PerfilClassificacaoDTO ConverterPerfilExtraidoParaClassificacao(PerfilExtraido perfilExtraido)
    {
        var skills = perfilExtraido.gestorExternoPerfilSkills?.Select(s => new PerfilSkillClassificacaoDTO
        {
            SkillId = s.skill?.Id ?? 0,
            NivelId = s.nivel?.Id ?? 0,
            ItemPerfilId = s.itemPerfil?.Id ?? 0,
            Relevante = s.relevante
        }).Where(s => s.SkillId > 0 && s.ItemPerfilId > 0).ToList() ?? new List<PerfilSkillClassificacaoDTO>();

        return new PerfilClassificacaoDTO
        {
            IdVaga = String.Empty,
            CodVaga = 0,
            Titulo = perfilExtraido.nomePerfil ?? string.Empty,
            Descricao = perfilExtraido.informacoesRelevantes ?? string.Empty,
            Cargo = perfilExtraido.nomePerfil ?? string.Empty,
            Skills = skills
        };
    }
}
