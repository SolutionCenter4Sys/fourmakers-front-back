using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Colaborador.Domain.Interfaces.Services;
using Core.Domain.Colaborador;
using Core.Domain.Questionario;
using DataTransferObject.Domain.Arquivo;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador.Campanha._2025_01_COLETA_PERFIL_COLABORADOR;
using Logs.Infra.Attributes;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Questionario.Domain.Interfaces;

namespace Questionario.Domain.Impl;

[LogDomainClass]
public class QuestionarioService(
    IEnderecoService enderecoService, 
    IQuestionarioRespostaRepository questionarioRespostaRepository, 
    IDBConnectionUnitOfWork idbConnectionUnitOfWork, 
    IColaboradorService colaboradorService, 
    ICertificadoRepository certificadoRepository,
    ILinkedinService linkedinService,
    IUploadFilesClient uploadFilesClient,
    IBuscaColaboradorRepository buscaColaboradorRepository) : IQuestionarioService
{

    private const string COLETA_PERFIL_CODIGO = "2025_01_COLETA_PERFIL_COLABORADOR";
    
    public async Task<ApiGenericResult<string>> ColetarPerfilColaboradorAsync(ColetaPerfilColaboradorCampanhaRootDTO param, string codigoInternoColaborador, int orgId)
    {
        var apiResult = new ApiGenericResult<string>();
        idbConnectionUnitOfWork.BeginTransaction();
        try
        {
            var existeQuestionario =
                await questionarioRespostaRepository.ExisteCodigoQuestionarioAsync(COLETA_PERFIL_CODIGO, orgId);
            if (!existeQuestionario)
            {
                throw new ArgumentException("Questionario Inexistente");
            }
            await colaboradorService.EditarModeloTrabalhoAsync(new()
            {
                ModeloTrabalho = param.FormaAtuacao.ModeloTrabalho,
                DiasPorSemana = param.FormaAtuacao.FrequenciaId
            }, orgId, codigoInternoColaborador);

            await colaboradorService.EditarTelefoneColaboradorAsync(param.DadosPessoais.Telefone, param.DadosPessoais.Ddi,  codigoInternoColaborador);
           
            var pathCertificados = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.PATH_CERTIFICADOS);
            if (param.AwsTechnicalAccredited.Root != null && param.AwsTechnicalAccredited.Root.Status == "Concluido" && param.AwsTechnicalAccredited.Root.DataEmissao != null)
            {
                var (path ,file) = await UploadFileAws(param.AwsTechnicalAccredited.ArquivoBase64, pathCertificados, codigoInternoColaborador);
                await certificadoRepository.SalvarCertificadoAsync(codigoInternoColaborador, file, param.AwsTechnicalAccredited.Root.Nome, param.AwsTechnicalAccredited.Root.Emissor, param.AwsTechnicalAccredited.Root.DataEmissao.Value, param.AwsTechnicalAccredited.Root.CargaHoraria);
                param.AwsTechnicalAccredited.Root.FilePath = path;
            }
            
            if (param.AwsTechnical.Root != null && param.AwsTechnical.Root.Status == "Concluido" && param.AwsTechnical.Root.DataEmissao != null)
            {
                var (path ,file) = await UploadFileAws(param.AwsTechnical.ArquivoBase64, pathCertificados, codigoInternoColaborador);
                await certificadoRepository.SalvarCertificadoAsync(codigoInternoColaborador, file, param.AwsTechnical.Root.Nome, param.AwsTechnical.Root.Emissor, param.AwsTechnical.Root.DataEmissao.Value, param.AwsTechnical.Root.CargaHoraria);
                param.AwsTechnical.Root.FilePath = path;
            }
            
            if (param.AwsTechnicalFoundational.Root != null && param.AwsTechnicalFoundational.Root.Status == "Concluido" && param.AwsTechnicalFoundational.Root.DataEmissao != null)
            {
                var (path ,file) = await UploadFileAws(param.AwsTechnicalFoundational.ArquivoBase64, pathCertificados, codigoInternoColaborador);
                await certificadoRepository.SalvarCertificadoAsync(codigoInternoColaborador, file, param.AwsTechnicalFoundational.Root.Nome, param.AwsTechnicalFoundational.Root.Emissor, param.AwsTechnicalFoundational.Root.DataEmissao.Value, param.AwsTechnicalFoundational.Root.CargaHoraria);
                param.AwsTechnicalFoundational.Root.FilePath = path;
            }

            if (!param.DadosPessoais.Linkedin.IsNullOrEmpty())
            {
                try
                {
                    var apenasNomeUsuarioLinkedin = LinkedinUtils.GetVanityName(param.DadosPessoais.Linkedin);
                    await linkedinService.SincronizarPerfilLinkedin(apenasNomeUsuarioLinkedin, codigoInternoColaborador,
                        true, param.DadosPessoais.Linkedin);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao extrair linkedin: {ex.Message}, StackTrace: {ex.StackTrace}");
                    await buscaColaboradorRepository.AtualizaInfoLinkedinAsync(codigoInternoColaborador, param.DadosPessoais.Linkedin);
                }
            }
            
            await enderecoService.EditarEnderecoAsync(new()
            {
                Bairro = param.Endereco.Root.Bairro,
                Cidade = param.Endereco.Root.Cidade,
                Estado = param.Endereco.Root.Estado,
                Numero = param.Endereco.Root.Numero,
                Complemento = param.Endereco.Root.Complemento,
                Cep = param.Endereco.Root.CodigoPostal,
                Endereco = param.Endereco.Root.Endereco,
            }, codigoInternoColaborador);
            
            var (pathEndereco, _) = await UploadFileAws(param.Endereco.ArquivoBase64, "Arquivos/comprovante_endereco/", codigoInternoColaborador);
            param.Endereco.Root.ComprovanteResidenciaPath =  pathEndereco;
            
            var objetoQuestionario = new ColetaPerfilColaboradorJsonDTO
            {
                Endereco = param.Endereco.Root,
                FormaAtuacao =  param.FormaAtuacao,
                AwsTechnical = param.AwsTechnical.Root,
                AwsTechnicalAccredited = param.AwsTechnicalAccredited.Root,
                AwsTechnicalFoundational =  param.AwsTechnicalFoundational.Root,
                DadosPessoais = param.DadosPessoais
            };
            
            var objetoQuestionarioJson = JsonConvert.SerializeObject(objetoQuestionario);
            await InserirQuestionarioAsync(COLETA_PERFIL_CODIGO, objetoQuestionarioJson, codigoInternoColaborador, orgId);
            idbConnectionUnitOfWork.Commit();
            apiResult.Retorno = COLETA_PERFIL_CODIGO;
        }
        catch (ArgumentException)
        {
            idbConnectionUnitOfWork.Rollback();
            throw;
        }
        catch (Exception e)
        {
            idbConnectionUnitOfWork.Rollback();
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Create, "Coleta de Perfil do colaborador");
        }

        return apiResult;
    }

    private async Task InserirQuestionarioAsync(string codigoQuestionario, string objetoJson, string codigoInternoColaborador, int orgId)
    {
        await questionarioRespostaRepository.InserirRespostaAsync(codigoQuestionario, objetoJson, codigoInternoColaborador, orgId, true);
    }

    private async Task<(string, string)> UploadFileAws(Base64DTO file, string path, string codigoInternoColaborador)
    {
        var data = DateTime.Now.ToString("yyyyMMddHHmmssFFF");
        var fileName = $"{codigoInternoColaborador}_{data}.{file.Tipo.ToString()}";
        var pathS3 = path + fileName;
      
        await uploadFilesClient.UploadFile(pathS3, file.Base64ToByteArray());
        return (pathS3, fileName);
    }
}