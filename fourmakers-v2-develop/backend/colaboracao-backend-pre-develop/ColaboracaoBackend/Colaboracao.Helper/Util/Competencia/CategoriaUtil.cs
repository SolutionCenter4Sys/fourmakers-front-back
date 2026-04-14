using Competencia.Domain.Enums;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia.Enums;
using DataTransferObject.Domain.Competencia.GestaoDeCompetencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.ItemPerfil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Helper.Util.Competencia
{
    public static class CategoriaHabilidadeUtil
    {
        public static List<NivelMigracaoDTO> GetNiveisMigracao()
        {
            return new List<NivelMigracaoDTO>
            {
                // Níveis Hardskill
                new (29, "A definir", TipoCompetenciaSRSEnum.HardSkill, ItemPerfilEnum.COMPETENCIA, CategoriaSkillEnum.Hardskills, NivelSenioridadeMigracaoEnum.Nivel1),
                new (1, "Trainee", TipoCompetenciaSRSEnum.HardSkill, ItemPerfilEnum.COMPETENCIA, CategoriaSkillEnum.Hardskills, NivelSenioridadeMigracaoEnum.Nivel2),
                new (2, "Júnior", TipoCompetenciaSRSEnum.HardSkill, ItemPerfilEnum.COMPETENCIA, CategoriaSkillEnum.Hardskills, NivelSenioridadeMigracaoEnum.Nivel3),
                new (3, "Pleno", TipoCompetenciaSRSEnum.HardSkill, ItemPerfilEnum.COMPETENCIA, CategoriaSkillEnum.Hardskills, NivelSenioridadeMigracaoEnum.Nivel4),
                new (4, "Sênior", TipoCompetenciaSRSEnum.HardSkill, ItemPerfilEnum.COMPETENCIA, CategoriaSkillEnum.Hardskills, NivelSenioridadeMigracaoEnum.Nivel5),
                new (34, "Especialista", TipoCompetenciaSRSEnum.HardSkill, ItemPerfilEnum.COMPETENCIA, CategoriaSkillEnum.Hardskills, NivelSenioridadeMigracaoEnum.Nivel6),

                // Níveis Softskill
                new (30, "A definir", TipoCompetenciaSRSEnum.SoftSkill, ItemPerfilEnum.SOFTSKILL, CategoriaSkillEnum.Softskills, NivelSenioridadeMigracaoEnum.Nivel1),
                new (23, "Iniciante", TipoCompetenciaSRSEnum.SoftSkill, ItemPerfilEnum.SOFTSKILL, CategoriaSkillEnum.Softskills, NivelSenioridadeMigracaoEnum.Nivel2),
                new (22, "Intermediário", TipoCompetenciaSRSEnum.SoftSkill, ItemPerfilEnum.SOFTSKILL, CategoriaSkillEnum.Softskills, NivelSenioridadeMigracaoEnum.Nivel3),
                new (21, "Avançado", TipoCompetenciaSRSEnum.SoftSkill, ItemPerfilEnum.SOFTSKILL, CategoriaSkillEnum.Softskills, NivelSenioridadeMigracaoEnum.Nivel4),
                new (35, "Especialista", TipoCompetenciaSRSEnum.SoftSkill, ItemPerfilEnum.SOFTSKILL, CategoriaSkillEnum.Softskills, NivelSenioridadeMigracaoEnum.Nivel5),

                // Níveis Idioma
                new (32, "A definir", TipoCompetenciaSRSEnum.Idioma, ItemPerfilEnum.IDIOMA, CategoriaSkillEnum.Idiomas, NivelSenioridadeMigracaoEnum.Nivel1),
                new (24, "Básico", TipoCompetenciaSRSEnum.Idioma, ItemPerfilEnum.IDIOMA, CategoriaSkillEnum.Idiomas, NivelSenioridadeMigracaoEnum.Nivel2),
                new (25, "Intermediário", TipoCompetenciaSRSEnum.Idioma, ItemPerfilEnum.IDIOMA, CategoriaSkillEnum.Idiomas, NivelSenioridadeMigracaoEnum.Nivel3),
                new (26, "Avançado", TipoCompetenciaSRSEnum.Idioma, ItemPerfilEnum.IDIOMA, CategoriaSkillEnum.Idiomas, NivelSenioridadeMigracaoEnum.Nivel4),
                new (27, "Fluente", TipoCompetenciaSRSEnum.Idioma, ItemPerfilEnum.IDIOMA, CategoriaSkillEnum.Idiomas, NivelSenioridadeMigracaoEnum.Nivel5),
                new (28, "Nativo", TipoCompetenciaSRSEnum.Idioma, ItemPerfilEnum.IDIOMA, CategoriaSkillEnum.Idiomas, NivelSenioridadeMigracaoEnum.Nivel6),

                // Níveis Metodologia
                new (31, "A definir", TipoCompetenciaSRSEnum.Metodologia, ItemPerfilEnum.METODOLOGIA, CategoriaSkillEnum.Metodologia, NivelSenioridadeMigracaoEnum.Nivel1),
                new (13, "Iniciante", TipoCompetenciaSRSEnum.Metodologia, ItemPerfilEnum.METODOLOGIA, CategoriaSkillEnum.Metodologia, NivelSenioridadeMigracaoEnum.Nivel2),
                new (12, "Intermediário", TipoCompetenciaSRSEnum.Metodologia, ItemPerfilEnum.METODOLOGIA, CategoriaSkillEnum.Metodologia, NivelSenioridadeMigracaoEnum.Nivel3),
                new (11, "Avançado", TipoCompetenciaSRSEnum.Metodologia,  ItemPerfilEnum.METODOLOGIA, CategoriaSkillEnum.Metodologia, NivelSenioridadeMigracaoEnum.Nivel4),
                new (10, "Especialista", TipoCompetenciaSRSEnum.Metodologia, ItemPerfilEnum.METODOLOGIA, CategoriaSkillEnum.Metodologia, NivelSenioridadeMigracaoEnum.Nivel5),

                // Níveis Dominio
                new (14, "Avançado", TipoCompetenciaSRSEnum.Dominio, ItemPerfilEnum.DOMINIONEGOCIO, CategoriaSkillEnum.ConhecimentoNegocio, NivelSenioridadeMigracaoEnum.Nivel1),
                new (15, "Intermediário", TipoCompetenciaSRSEnum.Dominio, ItemPerfilEnum.DOMINIONEGOCIO, CategoriaSkillEnum.ConhecimentoNegocio, NivelSenioridadeMigracaoEnum.Nivel2),
                new (16, "Iniciante", TipoCompetenciaSRSEnum.Dominio, ItemPerfilEnum.DOMINIONEGOCIO, CategoriaSkillEnum.ConhecimentoNegocio, NivelSenioridadeMigracaoEnum.Nivel3),
                new (36, "Especialista", TipoCompetenciaSRSEnum.Dominio, ItemPerfilEnum.DOMINIONEGOCIO, CategoriaSkillEnum.ConhecimentoNegocio, NivelSenioridadeMigracaoEnum.Nivel4),
                new (33, "A definir", TipoCompetenciaSRSEnum.Dominio, ItemPerfilEnum.DOMINIONEGOCIO, CategoriaSkillEnum.ConhecimentoNegocio, NivelSenioridadeMigracaoEnum.Nivel5),

                // Niveis Desconhecidas
                new  (37, "A definir", TipoCompetenciaSRSEnum.Desconhecida, ItemPerfilEnum.DESCONHECIDO, CategoriaSkillEnum.Desconhecida, NivelSenioridadeMigracaoEnum.Nivel1)
            };
        }

        public static int GetNivelEquivalenteAlocado(long itemPerfilTipo, long? idNivel)
        {
            var niveis = CategoriaHabilidadeUtil.GetNiveisMigracao();

            var buscarNivelAtual = niveis.Where(x => x.Id == idNivel).FirstOrDefault();
            if (buscarNivelAtual == null)
            {
                throw new Exception("Nivel não encontrado");
            }

            var buscarNivelDestino = niveis.Where(x => x.TipoItemPerfil == (ItemPerfilEnum)itemPerfilTipo && x.NivelEquivalencia == buscarNivelAtual.NivelEquivalencia).FirstOrDefault();
            if (buscarNivelDestino == null)
            {
                var nivelInferior = niveis.Where(n => n.TipoItemPerfil == (ItemPerfilEnum)itemPerfilTipo && n.NivelEquivalencia < buscarNivelAtual.NivelEquivalencia).OrderByDescending(n => n.NivelEquivalencia).FirstOrDefault();
                if (nivelInferior != null)
                {
                    buscarNivelDestino = nivelInferior;
                }
            }
            if (buscarNivelDestino == null)
            {
                buscarNivelDestino = niveis.Where(x => x.TipoItemPerfil == (ItemPerfilEnum)itemPerfilTipo).FirstOrDefault();
            }

            if (buscarNivelDestino == null)
            {
                throw new Exception("Não foi possível encontrar um nível válido para a categoria.");
            }

            return buscarNivelDestino.Id;
        }

        public static int? GetNivelEquivalente(TipoCompetenciaSRSEnum tipo, long? idNivel)
        {
            if (idNivel == null)
            {
                return null;
            }

            var niveis = CategoriaHabilidadeUtil.GetNiveisMigracao();

            var buscarNivelAtual = niveis.Where(x => x.Id == idNivel).FirstOrDefault();
            if (buscarNivelAtual == null)
            {
                throw new Exception("Nivel não encontrado");
            }

            var buscarNivelDestino = niveis.Where(x => x.Tipo == tipo && x.NivelEquivalencia == buscarNivelAtual.NivelEquivalencia).FirstOrDefault();
            if (buscarNivelDestino == null)
            {
                var nivelInferior = niveis.Where(n => n.Tipo == tipo && n.NivelEquivalencia < buscarNivelAtual.NivelEquivalencia).OrderByDescending(n => n.NivelEquivalencia).FirstOrDefault();
                if (nivelInferior != null)
                {
                    buscarNivelDestino = nivelInferior;
                }
            }
            if (buscarNivelDestino == null)
            {
                buscarNivelDestino = niveis.Where(x => x.Tipo == tipo).FirstOrDefault();
            }

            if (buscarNivelDestino == null)
            {
                throw new Exception("Não foi possível encontrar um nível válido para a categoria.");
            }

            return buscarNivelDestino.Id;
        }

        public static int GetNivelEquivalenteSRS(CategoriaSkillEnum tipo, long? idNivel)
        {
            var niveis = CategoriaHabilidadeUtil.GetNiveisMigracao();

            var buscarNivelAtual = niveis.Where(x => x.Id == idNivel).FirstOrDefault();
            if (buscarNivelAtual == null)
            {
                throw new Exception("Nivel não encontrado");
            }

            var buscarNivelDestino = niveis.Where(x => x.categoriaSkillSRS == tipo && x.NivelEquivalencia == buscarNivelAtual.NivelEquivalencia).FirstOrDefault();
            if (buscarNivelDestino == null)
            {
                var nivelInferior = niveis.Where(n => n.categoriaSkillSRS == tipo && n.NivelEquivalencia < buscarNivelAtual.NivelEquivalencia).OrderByDescending(n => n.NivelEquivalencia).FirstOrDefault();
                if (nivelInferior != null)
                {
                    buscarNivelDestino = nivelInferior;
                }
            }
            if (buscarNivelDestino == null)
            {
                buscarNivelDestino = niveis.Where(x => x.categoriaSkillSRS == tipo).FirstOrDefault();
            }

            if (buscarNivelDestino == null)
            {
                throw new Exception("Não foi possível encontrar um nível válido para a categoria.");
            }

            return buscarNivelDestino.Id;
        }
    }
}