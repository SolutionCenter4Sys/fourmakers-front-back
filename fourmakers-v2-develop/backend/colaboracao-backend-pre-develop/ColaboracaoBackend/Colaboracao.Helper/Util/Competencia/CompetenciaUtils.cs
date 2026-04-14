using Competencia.Domain.Enums;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.ItemPerfil;
using System;
using System.Collections.Generic;

namespace Colaboracao.Helper.Util.Competencia
{
    public static class CompetenciaUtils
    {
        public static TipoCompetenciaSRSEnum ConverterPerfilItemParaTipoCompetenciaSRS(ItemPerfilEnum itemPerfilEnum)
        {
            return itemPerfilEnum switch
            {
                ItemPerfilEnum.COMPETENCIA => TipoCompetenciaSRSEnum.HardSkill,
                ItemPerfilEnum.METODOLOGIA => TipoCompetenciaSRSEnum.Metodologia,
                ItemPerfilEnum.SOFTSKILL => TipoCompetenciaSRSEnum.SoftSkill,
                ItemPerfilEnum.DOMINIONEGOCIO => TipoCompetenciaSRSEnum.Dominio,
                ItemPerfilEnum.IDIOMA => TipoCompetenciaSRSEnum.Idioma,
                ItemPerfilEnum.DESCONHECIDO => TipoCompetenciaSRSEnum.Desconhecida,
                _ => throw new Exception("Item perfil tipo invalido")
            };
        }

        public static int ConverterItemPerfilIdParaTipoCompetenciaSRSId(int itemPerfilId)
        {
            var itemPerfilEnum = (ItemPerfilEnum)itemPerfilId;
            var tipoCompetencia = ConverterPerfilItemParaTipoCompetenciaSRS(itemPerfilEnum);
            return (int)tipoCompetencia;
        }

        public static ItemPerfilEnum ConverterTipoCompetenciaSRSToItemPerfil(TipoCompetenciaSRSEnum tipoCompetencia)
        {
            return tipoCompetencia switch
            {
                TipoCompetenciaSRSEnum.HardSkill => ItemPerfilEnum.COMPETENCIA,
                TipoCompetenciaSRSEnum.Metodologia => ItemPerfilEnum.METODOLOGIA,
                TipoCompetenciaSRSEnum.SoftSkill => ItemPerfilEnum.SOFTSKILL,
                TipoCompetenciaSRSEnum.Dominio => ItemPerfilEnum.DOMINIONEGOCIO,
                TipoCompetenciaSRSEnum.Idioma => ItemPerfilEnum.IDIOMA,
                TipoCompetenciaSRSEnum.Desconhecida => ItemPerfilEnum.DESCONHECIDO,
                _ => throw new Exception("Tipo de competência inválido")
            };
        }

        public static int ConverterTipoCompetenciaSRSIdParaItemPerfilId(int tipoCompetenciaSRSId)
        {
            var tipoCompetenciaEnum = (TipoCompetenciaSRSEnum)tipoCompetenciaSRSId;
            var itemPerfilEnum = ConverterTipoCompetenciaSRSToItemPerfil(tipoCompetenciaEnum);
            return (int)itemPerfilEnum;
        }

        public static string GetDescricaoCompetenciaById(int id)
        {
            var itemPerfilEnum = (ItemPerfilEnum)id;

            return itemPerfilEnum switch
            {
                ItemPerfilEnum.COMPETENCIA => ItemPerfilEnum.COMPETENCIA.ToString(),
                ItemPerfilEnum.METODOLOGIA => ItemPerfilEnum.METODOLOGIA.ToString(),
                ItemPerfilEnum.SOFTSKILL => ItemPerfilEnum.SOFTSKILL.ToString(),
                ItemPerfilEnum.DOMINIONEGOCIO => ItemPerfilEnum.DOMINIONEGOCIO.ToString(),
                ItemPerfilEnum.IDIOMA => ItemPerfilEnum.IDIOMA.ToString(),
                ItemPerfilEnum.DESCONHECIDO => ItemPerfilEnum.DESCONHECIDO.ToString(),
                _ => throw new Exception("Item perfil tipo inválido")
            };
        }

        public static string GetTipoSkillDescricao(string tipo)
        {
            return tipo switch
            {
                "COMPETENCIA" => "HARDSKILL",
                "IDIOMA" => "IDIOMA",
                "METODOLOGIA" => "METODOLOGIA",
                "DOMINIONEGOCIO" => "DOMINIO",
                "SOFTSKILL" => "SOFTSKILL",
                _ => "HARDSKILL",
            };
        }

        public static string GetTipoSkillDescricaoBD(string tipo)
        {
            return tipo switch
            {
                "HARDSKILL" => "COMPETENCIA",
                "IDIOMA" => "IDIOMA",
                "METODOLOGIA" => "METODOLOGIA",
                "DOMINIO" => "DOMINIONEGOCIO",
                "SOFTSKILL" => "SOFTSKILL",
                _ => "COMPETENCIA",
            };
        }

        /// <summary>
        /// Converte uma string contendo informações de habilidades, onde as habilidades são separadas por um delimitador específico
        /// e os campos dentro de cada habilidade são separados por outro delimitador, em uma lista de objetos do tipo `SkillNivelDTO`.
        /// </summary>
        /// <param name="habilidades">Uma string contendo múltiplas habilidades separadas pelo delimitador `separador`,
        /// onde cada habilidade é representada por campos separados por `divisor` (Ex: id$descricao$nivel_id$nivel$tipo).</param>
        /// <param name="separador">O delimitador utilizado para separar as habilidades dentro da string `habilidades` (Ex: "¨").</param>
        /// <param name="divisor">O delimitador utilizado para separar os campos dentro de cada habilidade (Ex: "$").</param>
        /// <returns>Uma lista de objetos do tipo `SkillNivelDTO` com os dados extraídos das habilidades.</returns>
        /// <remarks>
        /// Este método divide a string de entrada `habilidades` em partes, verificando a estrutura e criando um objeto `SkillNivelDTO`
        /// para cada habilidade encontrada. Ele garante que as habilidades tenham pelo menos 5 campos válidos (id, descricao, nivel_id, nivel, tipo).
        /// </remarks>
        public static List<SkillNivelDTO> ConverterStringHabilidadesParaListaSkillNivelDTO(string habilidades, string separador, string divisor)
        {
            var objetosSeparados = habilidades.Split(separador);
            var listaHabilidades = new List<SkillNivelDTO>();

            foreach (var objeto in objetosSeparados)
            {
                var habilidade = objeto.Split(divisor);

                if (habilidade.Length >= 5)
                {
                    listaHabilidades.Add(new SkillNivelDTO()
                    {
                        Id = long.Parse(habilidade[0]),
                        Descricao = habilidade[1],
                        Nivel = new()
                        {
                            Id = long.Parse(habilidade[2]),
                            Descricao = habilidade[3]
                        },
                        TipoSkill = habilidade[4]
                    });
                }
            }

            return listaHabilidades;
        }
    }
}