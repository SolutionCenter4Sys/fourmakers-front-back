using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Enum;
using Core.DomainModel;
using DataTransferObject.Domain.Arquivo.TokenFileTemp;
using DataTransferObject.Domain.Base;
using Logs.Infra.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UploadFiles.Domain.Helpers;
using UploadFiles.Domain.Interfaces.Services;

namespace UploadFiles.Domain.Services
{
    [LogDomainClass]
    public class TokenFileTempService : ITokenFileTempService
    {
        private readonly ITokenFileTempRepository _repository;
        private readonly IUploadFiles _uploadFiles;

        public TokenFileTempService(ITokenFileTempRepository repository, IUploadFiles uploadFiles)
        {
            _repository = repository;
            _uploadFiles = uploadFiles;
        }

        public async Task<ApiGenericResult<List<TokenFileTempDTO>>> ListarAsync()
        {
            var result = new ApiGenericResult<List<TokenFileTempDTO>>();
            try
            {
                var lista = await _repository.ListarAsync();
                result.Retorno = lista.ToList();
                result.Sucesso = true;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, "TokenFileTemp");
            }
            return result;
        }

        public async Task<ApiGenericResult<TokenFileTempDTO>> ObterAsync(string token)
        {
            var result = new ApiGenericResult<TokenFileTempDTO>();
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Token é obrigatório.";
                    return result;
                }

                var item = await _repository.ObterPorTokenAsync(token.Trim());
                if (item == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Registro não encontrado.";
                    return result;
                }

                result.Retorno = item;
                result.Sucesso = true;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, "TokenFileTemp");
            }
            return result;
        }

        public async Task<ApiGenericResult<TokenFileTempDTO>> InserirComUploadAsync(Stream stream, string fileNameOriginal, long fileLength, string? arquivoBase = null)
        {
            var result = new ApiGenericResult<TokenFileTempDTO>();
            try
            {
                if (stream == null || !stream.CanRead)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Arquivo inválido.";
                    return result;
                }

                if (fileLength <= 0)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Arquivo vazio.";
                    return result;
                }

                if (!TryObterExtensaoPermitida(fileNameOriginal, out var extNormalizada))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Tipo de arquivo não permitido. Envie apenas PDF, PNG ou JPG.";
                    return result;
                }

                var (prefix, prefixErr) = TokenFileTempS3PathHelper.ResolveArquivoPrefix(arquivoBase);
                if (prefixErr != null)
                {
                    result.Sucesso = false;
                    result.Mensagem = prefixErr;
                    return result;
                }

                var tokenGuid = Guid.NewGuid();
                var token = tokenGuid.ToString();
                var (s3Key, keyErr) = TokenFileTempS3PathHelper.BuildObjectKey(prefix!, tokenGuid, extNormalizada);
                if (keyErr != null)
                {
                    result.Sucesso = false;
                    result.Mensagem = keyErr;
                    return result;
                }

                if (stream.CanSeek)
                    stream.Position = 0;

                var uploadOk = await _uploadFiles.UploadFile(stream, s3Key);
                if (!uploadOk)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Falha ao enviar o arquivo para o armazenamento.";
                    return result;
                }

                await _repository.InserirAsync(token, s3Key);
                var criado = await _repository.ObterPorTokenAsync(token);
                result.Retorno = criado;
                result.Sucesso = true;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, "TokenFileTemp");
            }
            return result;
        }

        private static bool TryObterExtensaoPermitida(string fileName, out string extensaoNormalizada)
        {
            extensaoNormalizada = null;
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext))
                return false;

            if (ext == ".jpeg")
                ext = ".jpg";

            if (ext is ".pdf" or ".png" or ".jpg")
            {
                extensaoNormalizada = ext;
                return true;
            }

            return false;
        }

        public async Task<ApiGenericResult> AtualizarAsync(string token, TokenFileTempAtualizarDTO dto)
        {
            var result = new ApiGenericResult();
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Token é obrigatório.";
                    return result;
                }

                if (dto == null || string.IsNullOrWhiteSpace(dto.NomeArquivo))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Nome do arquivo é obrigatório.";
                    return result;
                }

                var existente = await _repository.ObterPorTokenAsync(token.Trim());
                if (existente == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Registro não encontrado.";
                    return result;
                }

                var linhas = await _repository.AtualizarAsync(token.Trim(), dto.NomeArquivo.Trim());
                if (linhas == 0)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Não foi possível atualizar o registro.";
                    return result;
                }

                result.Sucesso = true;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, "TokenFileTemp");
            }
            return result;
        }

        public async Task<ApiGenericResult> ExcluirAsync(string token)
        {
            var result = new ApiGenericResult();
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Token é obrigatório.";
                    return result;
                }

                var linhas = await _repository.ExcluirAsync(token.Trim());
                if (linhas == 0)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Registro não encontrado.";
                    return result;
                }

                result.Sucesso = true;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Delete, "TokenFileTemp");
            }
            return result;
        }
    }
}
