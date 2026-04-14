using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using DbUp.MySql;

var connectionString = System.Environment.GetEnvironmentVariable("CD_MIGRA_CONN_STR");

// Caminho base dos scripts SQL
string scriptsPath = "scripts";

Console.WriteLine("[INF] Verify if CD_MIGRA_CONN_STR enviroment variable is not null: " + (!string.IsNullOrEmpty(connectionString)).ToString());

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("[ERR] CD_MIGRA_CONN_STR is not set. In GitLab CI, check that the variable (CD_MIGRATION_CONN_STR_DEVELOPMENT or CD_MIGRATION_CONN_STR_PRODUCTION) is defined and, if Protected, that the pipeline runs on the protected branch (not only on a Merge Request).");
    Environment.Exit(1);
}

// Obtendo todos os arquivos SQL, excluindo os que estão dentro de pastas exatamente chamadas "rollback"
var sqlFiles = Directory.GetFiles(scriptsPath, "*.sql", SearchOption.AllDirectories)
                        .Where(f => new DirectoryInfo(Path.GetDirectoryName(f) ?? "").Name.ToLower() != "rollback")
                        .ToArray();

if (sqlFiles.Length == 0)
{
    Console.WriteLine($"[WRN] No SQL scripts found for execution in the directory {scriptsPath}.");
    Environment.Exit(0);
}

// Construindo os scripts com o caminho relativo
var scripts = sqlFiles.Select(f =>
{
    var relativePath = Path.GetRelativePath(scriptsPath, f).Replace("\\", "/");
    return new SqlScript(relativePath, File.ReadAllText(f));
});

var builder = DeployChanges
    .To
    .MySqlDatabase(connectionString)
    .LogToConsole()
    .WithScripts(scripts);

//sobreescrevendo nome da tabela log
builder.Configure(c => c.Journal = new MySqlTableJournal(() => c.ConnectionManager, () => c.Log, null, "tb_fourmakers_migrations"));

var upgrader = builder.Build();

var result = upgrader.PerformUpgrade();

if (result.Successful)
{
    Environment.Exit(0);
}
else
{
    var errorScriptPath = Path.Combine(scriptsPath, result.ErrorScript.Name);
    var scriptDirectory = Path.GetDirectoryName(errorScriptPath) ?? "";
    var rollbackFolder = Path.Combine(scriptDirectory, "rollback");

    if (Directory.Exists(rollbackFolder))
    {
        var rollbackScripts = Directory.GetFiles(rollbackFolder, "*.sql")
        .OrderBy(f => f)
        .ToArray();

        if (rollbackScripts.Length > 0)
        {
            Console.WriteLine("[INF] Executing rollback scripts.");
            foreach (var script in rollbackScripts)
            {
                Console.WriteLine($"Rollback encontrado: {script}");
                var rollbackSql = File.ReadAllText(script);

                try
                {
                    // Executando rollback sem journal para evitar que seja registrado na tabela fourmakers_migration
                    var upgraderRollback = DeployChanges
                    .To
                    .MySqlDatabase(connectionString)
                    .WithScript(new SqlScript(script, rollbackSql))
                    .JournalTo(new NullJournal()) // Evita log no banco
                    .Build();


                    var resultRollback = upgraderRollback.PerformUpgrade();


                    if (resultRollback.Successful)
                    {
                        Console.WriteLine($"[INF] Script {script} rollback executed successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"[ERR] Error executing rollback: {resultRollback.Error}");
                        Console.WriteLine($"[ERR] Rollback script with error: {resultRollback.ErrorScript.Name}");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERR] Error executing rollback for {script}: {ex.Message}");
                }
            }
        }
        else
        {
            Console.WriteLine("[INF] No rollback scripts found.");
        }
    }
    else
    {
        Console.WriteLine("[INF] No rollback folder found.");
    }

    Environment.Exit(1); //codigo indicando que deu ruim

}