using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Competencia.Domain.Interfaces.Services;
using Core.Domain.Colaborador;
using DataTransferObject.Domain.Colaborador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Competencia.Domain.Impl.Services
{
    [LogDomainClass]
    public class SkillGenericService : ISkillGenericService
    {
        private readonly IColaboradorPerfilRepository _colaboradorPerfilRepository;

        public SkillGenericService(IColaboradorPerfilRepository colaboradorPerfilRepository)
        {
            _colaboradorPerfilRepository = colaboradorPerfilRepository;
        }

        /// <summary>
        /// Busca ou cria skills de forma genérica, retornando um dicionário com a descrição normalizada e o ID
        /// </summary>
        /// <param name="skills">Lista de descrições das skills</param>
        /// <param name="tipoSkill">Tipo da skill (HardSkill, SoftSkill, Metodologia, Dominio)</param>
        /// <param name="cpf">CPF do usuário que está criando (usado se precisar criar novas skills)</param>
        /// <returns>Lista de pares chave-valor com descrição normalizada e ID da skill</returns>
        public async Task<List<KeyValuePair<string, long>>> GetSkillInfoByDescricaoAsync(List<string> skills, TipoSkillEnum tipoSkill, string cpf)
        {
            if (skills == null || !skills.Any())
                return new List<KeyValuePair<string, long>>();

            // Mapa: skill normalizada (UPPER sem acentos) -> skill original com trim
            var mapaOriginais = skills
                .Select(s => s.Trim())
                .GroupBy(s => StringUtil.RemoveDiacritics(s.ToUpper()))
                .ToDictionary(g => g.Key, g => g.First());

            var skillsNormalizadas = mapaOriginais.Keys.ToList();

            var skillsExistentesRaw = await _colaboradorPerfilRepository
                .BuscarSkillsEmLoteAsync(skillsNormalizadas, tipoSkill);

            // Normalizar chaves do banco (remover acentos)
            var skillsExistentes = skillsExistentesRaw.ToDictionary(
                kvp => StringUtil.RemoveDiacritics(kvp.Key.ToUpper()),
                kvp => kvp.Value
            );

            // Criar skills que não existem
            var skillsParaCriar = skillsNormalizadas.Where(s => !skillsExistentes.ContainsKey(s)).ToList();
            if (skillsParaCriar.Any())
            {
                var novasSkills = await _colaboradorPerfilRepository
                    .CriarSkillsEmLoteAsync(skillsParaCriar, tipoSkill, cpf);

                foreach (var skill in novasSkills)
                    skillsExistentes[StringUtil.RemoveDiacritics(skill.Key.ToUpper())] = skill.Value;
            }

            // Retornar com descrições originais normalizadas (sem acentos, preserva capitalização)
            return skillsNormalizadas
                .Select(sn => new KeyValuePair<string, long>(StringUtil.RemoveDiacritics(mapaOriginais[sn]), skillsExistentes[sn].Id))
                .ToList();
        }
    }
}
