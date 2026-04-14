using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Helper.Extension;

public static class ListExtension
{
    public static string? BuildInClauseOrNull(this List<string>? list)
    {
        if (list == null || list.Count == 0)
            return null; // safe default
        // Gera ('1254','1255',...)
        return $"('{string.Join("','", list)}')";
    }
    
    public static List<string>? ToInSqlFormat(this List<string>? list)
    {
        if (list == null || list.Count == 0)
            return null; // safe default
        return list;
    }
}