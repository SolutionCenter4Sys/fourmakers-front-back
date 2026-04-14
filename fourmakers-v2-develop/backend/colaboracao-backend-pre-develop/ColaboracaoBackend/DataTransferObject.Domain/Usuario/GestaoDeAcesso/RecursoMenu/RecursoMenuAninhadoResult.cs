using DataTransferObject.Domain.Usuario.GestaoDeAcesso.Recurso.Constantes;
using DataTransferObject.Domain.Usuario.GestaoDeAcesso.RecursoMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

public class RecursoMenuAninhadoResult : RecursoMenuResult
{
    [JsonPropertyOrder(int.MaxValue)]
    public List<RecursoMenuAninhadoResult> SubMenus { get; set; } = new();
    public new string CodigoRecurso { get; set; }
    public new string CodigoRecursoMenu { get; set; }
    public static List<RecursoMenuAninhadoResult> MontarHierarquia(IEnumerable<RecursoMenuAninhadoResult> recursos)
    {
        // Ordem desejada para tipo_menu
        var ordemTipoMenu = new[] { TipoMenuEnum.item_header.ToString(),
                                    TipoMenuEnum.item_profile.ToString(),
                                    TipoMenuEnum.group_sidebar.ToString(),
                                    TipoMenuEnum.item_sidebar.ToString()
                                  };

        // Mapeia os recursos em um dicionário
        var dicCodigoRecursoERecurso = recursos.ToDictionary(r => r.TipoMenu + r.CodigoRecursoMenu, r => r);

        // Lista para armazenar o resultado aninhado
        var resultadoAninhado = new List<RecursoMenuAninhadoResult>();

        // Ordena recursos por tipo_menu e ordenacao
        var recursosOrdenados = recursos
            .OrderBy(r => Array.IndexOf(ordemTipoMenu, r.TipoMenu)) // Ordena pela ordem definida no tipo_menu
            .ThenBy(r => r.Ordenacao); // Ordena por ordenacao em seguida

        foreach (var recurso in recursosOrdenados)
        {
            if (string.IsNullOrEmpty(recurso.CodigoRecursoMenuPai))
            {
                resultadoAninhado.Add(recurso); // Recursos sem pai são os níveis superiores
            }
            else if (dicCodigoRecursoERecurso.TryGetValue(TipoMenuEnum.group_sidebar.ToString() + recurso.CodigoRecursoMenuPai, out var recursoPai))
            {
                recursoPai.SubMenus.Add(recurso);
            }
        }

        // remover os grupos vazios diretamente antes de retornar o resultado
        resultadoAninhado.RemoveAll(r => r.TipoMenu == TipoMenuEnum.group_sidebar.ToString() && (r.SubMenus == null || !r.SubMenus.Any()));

        return resultadoAninhado;
    }

    private static void OrdenarSubMenus(IEnumerable<RecursoMenuAninhadoResult> menus, string[] ordemTipoMenu)
    {
        foreach (var menu in menus)
        {
            // Ordena os submenus por tipo_menu e ordenacao
            menu.SubMenus = menu.SubMenus
                .OrderBy(sub => Array.IndexOf(ordemTipoMenu, sub.TipoMenu)) // Primeiro por tipo_menu
                .ThenBy(sub => sub.Ordenacao) // Depois por ordenacao
                .ToList();

            OrdenarSubMenus(menu.SubMenus, ordemTipoMenu); // Ordena submenus recursivamente
        }
    }



}
