using System;
using System.Linq;
using Competencia.Domain.Enums;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill;

namespace MapaDeAlocacao.Domain.Impl.GestaoDeAlocados.GestorExternoPerfil;

public static class GestorExternoPerfilCriteriosAutomatizacao
{
    public static bool Atende(GestorExternoPerfilInput input)
    {
        if (input is null)
            return false;

        if (string.IsNullOrEmpty(input.NomePerfil)
            || string.IsNullOrEmpty(input.CodGestorExterno)
            || string.IsNullOrEmpty(input.InformacoesRelevantes))
            return false;

        var skills = input.GestorExternoPerfilSkills ?? Enumerable.Empty<GestorExternoPerfilSkillInput>();
        var hardSkills = skills.Where(m => m.ItemPerfil != null && m.ItemPerfil.Id == (long)ItemPerfilEnum.COMPETENCIA);

        if (hardSkills.Count() <= 1)
            return false;

        return true;
    }
}
