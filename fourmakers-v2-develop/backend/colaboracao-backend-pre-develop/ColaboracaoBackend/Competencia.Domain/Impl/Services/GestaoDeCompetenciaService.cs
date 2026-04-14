using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Util.Competencia;
using Competencia.Domain.Enums;
using Competencia.Domain.Interfaces.Services;
using Core.DomainModel.Competencia;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Competencia.GestaoDeCompetencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Helper.Enum;

using Logs.Infra.Attributes;

namespace MapaDeAlocacao.Domain.Impl.GestaoDeHabilidades
{
    [LogDomainClass]
    public class GestaoDeCompetenciaService : IGestaoDeCompetenciaService
    {
        private readonly IGestaoDeCompetenciaRepository _gestaoDeCompetenciaRepository;
        private readonly IDBConnectionUnitOfWork _dBConnectionUnitOfWork;
        private readonly ICompetenciaService _competenciaService;
        private readonly ISRSClient _srsClient;

        public GestaoDeCompetenciaService(
            IGestaoDeCompetenciaRepository gestaoDeCompetenciaRepository,
            ICompetenciaService competenciaService,
            IDBConnectionUnitOfWork dBConnectionUnitOfWork,
            ISRSClient srsClient)
        {
            _gestaoDeCompetenciaRepository = gestaoDeCompetenciaRepository;
            _competenciaService = competenciaService;
            _dBConnectionUnitOfWork = dBConnectionUnitOfWork;
            _srsClient = srsClient;
        }

        public async Task<AdicionarCompetenciaDTO> AlterarCategoriaDaHabilidade(int id, TipoCompetenciaSRSEnum tipoAtual, TipoCompetenciaSRSEnum tipoDestino, string cpf, string tokenUsuario, int orgId)
        {
            _dBConnectionUnitOfWork.BeginTransaction();
            try
            {
                if (tipoAtual == tipoDestino)
                {
                    throw new Exception("Não é possivel transferir para a própria categoria de habilidade.");
                }
                var converterAtual = ConverterStringParaTipoId(tipoAtual);
                var converterDestino = ConverterStringParaTipoId(tipoDestino);
                var habilidadeAtual = await _gestaoDeCompetenciaRepository.VerificaHabilidadeExistentePorIdETipo(id, converterAtual);

                if (habilidadeAtual == null)
                {
                    throw new Exception("Habilidade não encontrada!");
                }

                var colaboradores = await _gestaoDeCompetenciaRepository.ListarColaboradoresPorSkillId(habilidadeAtual.Id, tipoAtual);

                var verificaDestino = await _gestaoDeCompetenciaRepository.VerificaHabilidadeExistentePorDescricaoETipo(habilidadeAtual.Descricao, converterDestino);

                var novaHabilidade = new AdicionarCompetenciaDTO();

                if (verificaDestino != null)
                {
                    novaHabilidade.IdCompetencia = verificaDestino.Id;
                    novaHabilidade.DescricaoCompetencia = verificaDestino.Descricao;

                    await UnificarCategorias(habilidadeAtual, tipoAtual, tipoDestino, colaboradores, novaHabilidade);
                }
                else
                {
                    novaHabilidade = await GerarNovaHabilidade(tipoAtual, tipoDestino, cpf, habilidadeAtual, colaboradores, orgId);
                }

                var tipoAntigoSRS = ConverterTipoCompetenciaParaTipoSRS(tipoAtual);
                var tipoNovoSRS = ConverterTipoCompetenciaParaTipoSRS(tipoDestino);
                await AtualizarTabelas(converterAtual, converterDestino, habilidadeAtual.Id, novaHabilidade.IdCompetencia, tokenUsuario, tipoAntigoSRS, tipoNovoSRS);

                _dBConnectionUnitOfWork.Commit();

                return novaHabilidade;
            }
            catch (Exception ex)
            {
                _dBConnectionUnitOfWork.SafeRollback();
                throw;
            }
        }

        private int ConverterStringParaTipoId(TipoCompetenciaSRSEnum tipoCompetenciaEnum)
        {
            return tipoCompetenciaEnum switch
            {
                TipoCompetenciaSRSEnum.HardSkill => 1,
                TipoCompetenciaSRSEnum.SoftSkill => 8,
                TipoCompetenciaSRSEnum.Metodologia => 3,
                TipoCompetenciaSRSEnum.Dominio => 4,
                TipoCompetenciaSRSEnum.Idioma => 9,
                TipoCompetenciaSRSEnum.Desconhecida => 14,
                _ => throw new Exception("Tipo de competência não reconhecido.")
            };
        }

        private int ConverterTipoCompetenciaParaTipoSRS(TipoCompetenciaSRSEnum tipoCompetenciaEnum)
        {
            return tipoCompetenciaEnum switch
            {
                TipoCompetenciaSRSEnum.HardSkill => 1,
                TipoCompetenciaSRSEnum.SoftSkill => 2,
                TipoCompetenciaSRSEnum.Idioma => 3,
                TipoCompetenciaSRSEnum.Metodologia => 4,
                TipoCompetenciaSRSEnum.Dominio => 5,
                TipoCompetenciaSRSEnum.Desconhecida => 6,
                _ => throw new Exception("Tipo de competência não reconhecido.")
            };
        }

        private async Task UnificarCategorias(SkillItemDTO habilidadeAtual, TipoCompetenciaSRSEnum tipoCompetenciaAtual, TipoCompetenciaSRSEnum tipoCompetenciaDestino, List<ColaboradorHabilidadeDTO> colaboradoresSkillAnterior, AdicionarCompetenciaDTO novaHabilidade)
        {
            var descricaoHabilidadeInativa = $"{habilidadeAtual.Descricao}_{DateTime.Now}_Inativo";

            var colaboradores = await _gestaoDeCompetenciaRepository.ListarColaboradoresPorSkillId(novaHabilidade.IdCompetencia, tipoCompetenciaDestino);

            var colaboradoresParaAdicionarCompetencias = colaboradoresSkillAnterior
            .Where(x => !colaboradores.Any(c => c.Id == x.Id))
            .ToList();

            await _competenciaService.EditarCompetencia(
                new()
                {
                    Ativo = false,
                    Id = (int)habilidadeAtual.Id,
                    Descricao = descricaoHabilidadeInativa,
                    TipoCompetencia = tipoCompetenciaAtual
                }
            );

            foreach (var colaborador in colaboradoresParaAdicionarCompetencias)
            {
                int? novoNivelId = CategoriaHabilidadeUtil.GetNivelEquivalente(tipoCompetenciaDestino, colaborador.Skill.Nivel?.Id);
                await _gestaoDeCompetenciaRepository.AdicionaNovaCompetenciaAoColaborador(novaHabilidade, novoNivelId, colaborador.Id, tipoCompetenciaDestino);
                await _gestaoDeCompetenciaRepository.InativarHabilidadeDoColaborador((int)habilidadeAtual.Id, colaborador.Id, tipoCompetenciaAtual);
            }
        }

        private async Task<AdicionarCompetenciaDTO> GerarNovaHabilidade(TipoCompetenciaSRSEnum tipoCompetenciaAtual, TipoCompetenciaSRSEnum tipoCompetenciaDestino, string cpf, SkillItemDTO habilidadeAtual, List<ColaboradorHabilidadeDTO> colaboradores, int orgId)
        {
            var novaHabilidade = await _competenciaService.AdicionarCompetenciaSemTransaction(habilidadeAtual.Descricao, tipoCompetenciaDestino, cpf, orgId);
            foreach (var colaborador in colaboradores)
            {
                int? novoNivelId = CategoriaHabilidadeUtil.GetNivelEquivalente(tipoCompetenciaDestino, colaborador.Skill.Nivel?.Id);
                await _gestaoDeCompetenciaRepository.AdicionaNovaCompetenciaAoColaborador(novaHabilidade, novoNivelId, colaborador.Id, tipoCompetenciaDestino);
                await _gestaoDeCompetenciaRepository.InativarHabilidadeDoColaborador((int)habilidadeAtual.Id, colaborador.Id, tipoCompetenciaAtual);
            }
            var descricaoHabilidadeInativa = $"{habilidadeAtual.Descricao}_{DateTime.Now}_Inativo";

            await _competenciaService.EditarCompetencia(
                new()
                {
                    Ativo = false,
                    Id = (int)habilidadeAtual.Id,
                    Descricao = descricaoHabilidadeInativa,
                    TipoCompetencia = tipoCompetenciaAtual
                }
            );

            return novaHabilidade;
        }

        private async Task AtualizarTabelas(int tipoAtual, int tipoDestino, long habilidadeAtualId, long novaHabilidadeId, string tokenUsuario, int tipoAntigoSRS, int tipoNovoSRS)
        {
            await _gestaoDeCompetenciaRepository.AtualizarGestorExternoPerfilSkills(tipoAtual, habilidadeAtualId, tipoDestino, novaHabilidadeId);
            await _gestaoDeCompetenciaRepository.AtualizarAlocadoSkills(tipoAtual, habilidadeAtualId, tipoDestino, novaHabilidadeId);
            await _gestaoDeCompetenciaRepository.AtualizarPerfilSkills(tipoAtual, habilidadeAtualId, tipoDestino, novaHabilidadeId);
            await _gestaoDeCompetenciaRepository.AtualizarSkillsVaga(tipoAtual, habilidadeAtualId, tipoDestino, novaHabilidadeId);
            await _gestaoDeCompetenciaRepository.AtualizarSkillsVagaFourmakers(tipoAtual, habilidadeAtualId, tipoDestino, novaHabilidadeId);
            await _gestaoDeCompetenciaRepository.AtualizarSkillsVagaSRS(tipoAtual, habilidadeAtualId, tipoDestino, novaHabilidadeId);
            await _gestaoDeCompetenciaRepository.AtualizarSkillsVagaCandidato(tipoAtual, habilidadeAtualId, tipoDestino, novaHabilidadeId);

            var srsResponse = await _srsClient.AlterarCategoriaHabilidade(
                new()
                {
                    TipoAntigo = tipoAntigoSRS,
                    TipoNovo = tipoNovoSRS,
                    IdHabilidadeAntiga = (int)habilidadeAtualId,
                    IdHabilidadeNova = (int)novaHabilidadeId,
                },
                tokenUsuario
            );

            if (!srsResponse.Retorno)
            {
                throw new Exception("Erro ao atualizar SRS");
            }
        }
    }
}