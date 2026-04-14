using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Comenta ou descomenta blocos *_ci no .gitlab-ci.yml conforme serviços selecionados.
/// O GitLab exige que todo stage referenciado exista em <c>stages:</c>; serviços não
/// escolhidos são desativados comentando o job, não removendo o stage.
/// </summary>
internal static partial class Program
{
    /// <summary>UTF-8 sem BOM (YAML e Git em geral).</summary>
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);

    [GeneratedRegex(@"^([A-Za-z0-9_-]+)_ci:\s*$", RegexOptions.Compiled)]
    private static partial Regex ActiveCiJob();

    [GeneratedRegex(@"^#\s*([A-Za-z0-9_-]+)_ci:\s*$", RegexOptions.Compiled)]
    private static partial Regex CommentedCiJob();

    [GeneratedRegex(@"^[A-Za-z_][A-Za-z0-9_-]*:\s*$", RegexOptions.Compiled)]
    private static partial Regex TopLevelKey();

    [GeneratedRegex(@"^#\s", RegexOptions.Compiled)]
    private static partial Regex CommentPrefix();

    [GeneratedRegex(@"^#\s?", RegexOptions.Compiled)]
    private static partial Regex UncommentOnce();

    private static string? ParseCiHeader(string line)
    {
        var m = ActiveCiJob().Match(line);
        if (m.Success)
            return m.Groups[1].Value;
        m = CommentedCiJob().Match(line);
        return m.Success ? m.Groups[1].Value : null;
    }

    private static bool IsTopLevelNonCi(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return false;
        if (line[0] is ' ' or '\t')
            return false;
        if (line.TrimStart().StartsWith('#'))
            return false;
        if (line.Contains("_ci:", StringComparison.Ordinal))
            return false;
        return TopLevelKey().IsMatch(line.TrimEnd());
    }

    private static void Transform(string path, string[] allServices, HashSet<string> selected)
    {
        var inactive = new HashSet<string>(
            allServices.Where(s => !selected.Contains(s)),
            StringComparer.Ordinal);

        var lines = ReadAllLines(path);
        var outLines = new List<string>(lines.Count);
        var i = 0;
        var n = lines.Count;

        while (i < n)
        {
            var svc = ParseCiHeader(lines[i]);
            if (svc is null)
            {
                outLines.Add(lines[i]);
                i++;
                continue;
            }

            var start = i;
            i++;
            while (i < n)
            {
                var line = lines[i];
                if (ParseCiHeader(line) is not null)
                    break;
                if (IsTopLevelNonCi(line))
                    break;
                i++;
            }

            var block = lines[start..i];

            if (inactive.Contains(svc))
            {
                foreach (var ln in block)
                {
                    var stripped = ln.TrimStart();
                    if (stripped.StartsWith('#'))
                        outLines.Add(ln);
                    else
                        outLines.Add("# " + ln);
                }
            }
            else
            {
                foreach (var ln in block)
                {
                    if (CommentPrefix().IsMatch(ln))
                        outLines.Add(UncommentOnce().Replace(ln, "", 1));
                    else
                        outLines.Add(ln);
                }
            }
        }

        File.WriteAllText(
            path,
            string.Join("\n", outLines) + (outLines.Count > 0 ? "\n" : ""),
            Utf8NoBom);
    }

    private static List<string> ReadAllLines(string path)
    {
        // Leitura: UTF-8 com detecção de BOM (arquivos vindos de editores diferentes).
        var text = File.ReadAllText(path, Encoding.UTF8);
        if (text.Length == 0)
            return [];
        return text.Split(["\r\n", "\n"], StringSplitOptions.None).ToList();
    }

    private static int Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine(
                "Usage: GitLabCiApplyJobSelection <path> <all,csv> <selected,csv>");
            return 2;
        }

        var path = args[0];
        var allServices = args[1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var selectedList = args[2].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (allServices.Length == 0)
        {
            Console.Error.WriteLine("No services listed.");
            return 2;
        }

        var selected = new HashSet<string>(selectedList, StringComparer.Ordinal);
        if (selected.Count == 0)
            selected = new HashSet<string>(allServices, StringComparer.Ordinal);

        Transform(path, allServices, selected);
        return 0;
    }
}
