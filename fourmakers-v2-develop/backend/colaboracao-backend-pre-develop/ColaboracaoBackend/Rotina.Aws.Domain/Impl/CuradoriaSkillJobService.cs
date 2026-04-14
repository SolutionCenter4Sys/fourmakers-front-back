using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Util;
using Competencia.Domain.Enums;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Curadoria;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rotina.Aws.Core.Interfaces;

namespace Rotina.Aws.Domain.Impl;

public class CuradoriaSkillJobService(ICuradoriaClient curadoriaClient, ICompetenciaClient competenciaClient) : IJobRunner
{
    public async Task ExecuteJobAsync()
    {
        await SincronizarSkillsCuradoriaAsync(TipoCompetenciaSRSEnum.HardSkill, MockData.HARDSKILL_MOCK);
        await SincronizarSkillsCuradoriaAsync(TipoCompetenciaSRSEnum.SoftSkill, MockData.SOFTSKILL_MOCK);
        await SincronizarSkillsCuradoriaAsync(TipoCompetenciaSRSEnum.Idioma, MockData.IDIOMA_MOCK);
        await SincronizarSkillsCuradoriaAsync(TipoCompetenciaSRSEnum.Dominio, MockData.DOMINIO_MOCK);
        await SincronizarSkillsCuradoriaAsync(TipoCompetenciaSRSEnum.Metodologia, MockData.METODOLOGIA_MOCK);
    }

    private async Task SincronizarSkillsCuradoriaAsync(TipoCompetenciaSRSEnum tipo, string mockData)
    {
        var curadoriaList = new List<CuradoriaSkillResponse>();
        var curadoriaTodas = new List<CuradoriaSkillResponse>();

        var token = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO);
        
        var skills = await competenciaClient.ListarNomeDeSkillsPorTipo(tipo, token);
        if(skills is null) return;

        // var skills = JsonConvert.DeserializeObject<List<string>>(mockData)?? [];
        int index = 0;

        var listaLog = new List<AlterarNomeCompetenciaResultDTO>();
        
        foreach (var skill in skills)
        {
            index++;
            var curadoriaResult = await curadoriaClient.ObterComparativoSkill(skill.Descricao, tipo);
            if(curadoriaResult is null) continue;
            curadoriaTodas.Add(curadoriaResult);
            Console.WriteLine($"Skill: {index} de {skills.Count}");
            Console.WriteLine(JsonConvert.SerializeObject(curadoriaResult));
            if(curadoriaResult.Suggested == skill.Descricao) continue;
            if (curadoriaResult.Type == CuradoriaSkillType.embedding && curadoriaResult.Distance <= 0.85) continue;
            curadoriaList.Add(curadoriaResult);
            Console.WriteLine($"Alterando De: {curadoriaResult.Input} para {curadoriaResult.Suggested}");
            var resultado = await competenciaClient.AlterarNomeCompetencia(new()
            {
                NomeCompetencia = curadoriaResult.Input,
                NovoNomeCompetencia = curadoriaResult.Suggested,
                TipoCompetencia = tipo
            }, token);
            
            listaLog.Add(resultado.Retorno);
        }

        var listaDinamicaLog = listaLog.Select(x => new
        {
            x.NomeCompetencia,
            x.CompetenciaID,
            x.NovoNome,
            NovoId = x.NovoId == null ? "Não Alterado" : x.NovoId.ToString(),
            x.Tipo,
            ColaboradoresAfetados = x.LogsAlteracao.Colaboradores,
            AlocadosAfetados = x.LogsAlteracao.Alocados,
            PerfisAfetados = x.LogsAlteracao.Perfis,
            PerfisExternosAfetados = x.LogsAlteracao.PerfisExterno,
            VagasAfetados = x.LogsAlteracao.Vagas,
            VagasSrsAfetados = x.LogsAlteracao.VagasSrs,
            VagasCandidatoAfetados = x.LogsAlteracao.VagasCandidato
        }).ToList();
        
        CreateExcelFile(curadoriaList, $"{tipo.ToString()}-Comparativo");
        CreateExcelFile(curadoriaTodas, $"{tipo.ToString()}-Todas");
        CreateExcelFile(listaDinamicaLog, $"{tipo.ToString()}-Alteracao");
    }
    

    private void CreateExcelFile(dynamic relatorio, string excelFileName)
    {
        var fileBytes = ExcelFileUtil.CreateExcelFile(relatorio); // byte[]
    
        var fileName = excelFileName + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xlsx";
    
        // Caminho da pasta onde o arquivo será salvo
        var folderPath = Path.Combine(Environment.CurrentDirectory, "ArquivosExcel"); // ou um caminho fixo: "C:\\MeusArquivos\\Excel"
    
        // Garante que a pasta existe
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fullPath = Path.Combine(folderPath, fileName);

        // Salva o arquivo
        File.WriteAllBytes(fullPath, fileBytes);

        Console.WriteLine($"Arquivo salvo com sucesso em: {fullPath}");
    }
}