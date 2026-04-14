using Colaboracao.Helper;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Vaga;
using System.Collections.Generic;

namespace SRS.Domain.Impl.Util
{
    public static class SRSUtil
    {
        public static List<SkillsVagasSrsDTO> ConverterParaListaSkillsVagasSrs(
            List<(IEnumerable<SkillNivelDTO> Skills, CategoriaSkillEnum Category)> skillCategories)
        {
            var result = new List<SkillsVagasSrsDTO>();

            foreach (var (skills, category) in skillCategories)
            {
                foreach (var skill in skills)
                {
                    result.Add(new SkillsVagasSrsDTO
                    {
                        DescricaoId = (int)skill.Id,
                        NivelId = (int)skill.Nivel.Id,
                        CategoriaId = category.ToInt()
                    });
                }
            }

            return result;
        }
    }
}