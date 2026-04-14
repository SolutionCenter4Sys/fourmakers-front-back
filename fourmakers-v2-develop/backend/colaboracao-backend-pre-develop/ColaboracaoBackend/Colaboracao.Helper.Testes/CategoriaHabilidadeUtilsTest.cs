using Colaboracao.Helper.Util.Competencia;
using Competencia.Domain.Enums;
using DataTransferObject.Domain.Colaborador;
using System;
using System.Linq;
using Xunit;

namespace Colaboracao.Helper.Testes
{
    public class CategoriaHabilidadeUtilTests
    {
        [Fact]
        public void TestarTodasCombinacoesGetNivelEquivalente_DeveRetornarNivelEquivalenteCorreto()
        {
            int nivelEquivalente = 0;

            var tiposCompetencia = (TipoCompetenciaSRSEnum[])System.Enum.GetValues(typeof(TipoCompetenciaSRSEnum));
            var tiposItemPerfil = (ItemPerfilEnum[])System.Enum.GetValues(typeof(ItemPerfilEnum));
            var categoriasSkill = (CategoriaSkillEnum[])System.Enum.GetValues(typeof(CategoriaSkillEnum));
            var niveis = CategoriaHabilidadeUtil.GetNiveisMigracao().Select(x => x.Id).ToList();

            foreach (var tipoCompetencia in tiposCompetencia)
            {
                foreach (var idNivel in niveis)
                {
                    nivelEquivalente = CategoriaHabilidadeUtil.GetNivelEquivalente(tipoCompetencia, idNivel).ToIntOuZero();
                }
            }

            foreach (var tipoItemPerfil in tiposItemPerfil)
            {
                foreach (var idNivel in niveis)
                {
                    nivelEquivalente = CategoriaHabilidadeUtil.GetNivelEquivalenteAlocado((long)tipoItemPerfil, idNivel);
                }
            }

            foreach (var categoriaSkill in categoriasSkill)
            {
                foreach (var idNivel in niveis)
                {
                    nivelEquivalente = CategoriaHabilidadeUtil.GetNivelEquivalenteSRS(categoriaSkill, idNivel);
                }
            }
            Assert.NotEqual(nivelEquivalente, 0);
        }

        [Fact]
        public void TestarGetNivelEquivalente_DeveLancarExcecaoQuandoNivelNaoExistir()
        {
            TipoCompetenciaSRSEnum tipoCompetencia = TipoCompetenciaSRSEnum.HardSkill;
            long? idNivelInexistente = 999;

            Assert.Throws<Exception>(() => CategoriaHabilidadeUtil.GetNivelEquivalente(tipoCompetencia, idNivelInexistente));
        }

        [Fact]
        public void TestarGetNivelEquivalenteAlocado_DeveLancarExcecaoQuandoNivelNaoExistirAlocado()
        {
            long itemPerfilTipo = (long)ItemPerfilEnum.COMPETENCIA;
            long? idNivelInexistente = 999; // Nível inexistente

            Assert.Throws<Exception>(() => CategoriaHabilidadeUtil.GetNivelEquivalenteAlocado(itemPerfilTipo, idNivelInexistente));
        }

        [Fact]
        public void TestarGetNivelEquivalenteSRS_DeveLancarExcecaoQuandoNivelNaoExistirSRS()
        {
            CategoriaSkillEnum tipoCategoria = CategoriaSkillEnum.Hardskills; // Usando CategoriaSkillEnum aqui
            long? idNivelInexistente = 999; // Nível inexistente

            // Assert que espera a exceção
            Assert.Throws<Exception>(() => CategoriaHabilidadeUtil.GetNivelEquivalenteSRS(tipoCategoria, idNivelInexistente));
        }
    }
}