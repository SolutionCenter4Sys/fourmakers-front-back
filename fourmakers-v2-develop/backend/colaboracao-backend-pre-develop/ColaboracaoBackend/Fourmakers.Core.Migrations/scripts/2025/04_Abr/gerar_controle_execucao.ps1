# Obtém a data atual
$dataAtual = Get-Date -Format "dd/MM/yyyy"

# Define o diretório raiz como o diretório onde o script está sendo executado
$rootDirectory = $PSScriptRoot

# Obtém todos os arquivos dentro dos subdiretórios do diretório raiz
$arquivos = Get-ChildItem -Path $rootDirectory -Recurse -File | Where-Object { $_.Directory.FullName -ne $rootDirectory }

# Define o nome do arquivo de controle
$arquivoControle = "controle_execucao_script_temp.txt"

# Cria o arquivo de controle ou sobrescreve se ele já existir
New-Item -Path $arquivoControle -ItemType File -Force | Out-Null

# Adiciona o cabeçalho ao arquivo de controle
Add-Content -Path $arquivoControle -Value "Controle Mensal de Execução de Scripts`n"
Add-Content -Path $arquivoControle -Value "aws.gcolb_hml`taws.gcolb_prd`tscript`n"

# Percorre cada arquivo encontrado
foreach ($arquivo in $arquivos) {

    # Obtém o caminho relativo do arquivo em relação ao diretório raiz
    $caminhoRelativo = $arquivo.FullName.Substring($rootDirectory.Length + 1)

    # Escreve a entrada de controle no formato desejado no arquivo de controle
    Add-Content -Path $arquivoControle -Value "[ ]$dataAtual`t[ ]`t`t`t`t$caminhoRelativo"
}

Write-Host "Arquivo de controle gerado com sucesso: $arquivoControle"
