using Aws.Infra.Interfaces.S3;
using Colaboracao.Core.Interfaces;
using Core.DomainModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Colaboracao.Core.Impl
{
    public class UploadFiles : IUploadFiles
    {
        private readonly IAmazonS3Uploader _amazonS3Uploader;
        private readonly ITokenFileRepository _tokenFileRepository;

        public UploadFiles(IAmazonS3Uploader amazonS3Uploader, ITokenFileRepository tokenFileRepository)
        {
            _amazonS3Uploader = amazonS3Uploader;
            _tokenFileRepository = tokenFileRepository;
        }

        public async Task<bool> DeleteFile(string keyName)
        {
            try
            {
                var resultado = await _amazonS3Uploader.DeleteFile(keyName);
                if (resultado)
                {
                    await _tokenFileRepository.DeletarTokenArquivo(keyName);
                }
                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Stream> GetFile(string keyName)
        {
            return await _amazonS3Uploader.GetFile(keyName);
        }

        public async Task<bool> UploadFile(Stream file, string fileName)
        {
            try
            {
                var resultado = await _amazonS3Uploader.UploadFile(file, fileName);
                if (resultado)
                {
                    string token = Guid.NewGuid().ToString();
                    await _tokenFileRepository.InserirTokenArquivo(token, fileName);
                }
                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Stream> GetFileByToken(string token, string fileName)
        {
            try
            {
                bool tokenValido = await _tokenFileRepository.ValidarTokenArquivo(token, fileName);
                if (!tokenValido)
                {
                    throw new UnauthorizedAccessException("Token inválido");
                }

                return await _amazonS3Uploader.GetFile(fileName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task RenameFilesAsync(IEnumerable<(string NomeAtual, string NomeNovo)> renames)
        {
            foreach (var (nomeAtual, nomeNovo) in renames)
            {
                var renomeado = await _amazonS3Uploader.RenameFile(nomeAtual, nomeNovo);
                if (!renomeado)
                    throw new InvalidOperationException($"Falha ao renomear arquivo na S3: {nomeAtual} -> {nomeNovo}");

                await _tokenFileRepository.AtualizarNomeArquivo(nomeAtual, nomeNovo);
            }
        }
    }
}