using ApiClient.Domain.Interfaces;
using Core.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RotinasBackoffice.API
{
    public class RotinaMigracao : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RotinaMigracao(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Começando a migração dos arquivos do Servidor de imagem para o S3 da Foursys na AWS");
            await MigraArquivosDoServidorParaS3DaAws();
        }

        public async Task<bool> MigraArquivosDoServidorParaS3DaAws()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var uploadFilesClient = scope.ServiceProvider.GetRequiredService<IUploadFilesClient>();
                    var repository = scope.ServiceProvider.GetRequiredService<IRotinaMigracaoRepository>();
                    
                    var nomeImagens = repository.ListaTodasImagensDoBanco();
                    var nomeCertificados = repository.ListaTodosCertificadosDoBanco();
                    var nomeCurriculosCandidatos = repository.ListaTodosCurriculosDeCandidatosDoBanco();
                    var contador = 0;

                foreach (var item in nomeImagens)
                {
                    try
                    {
                        var caminhoNome = _configuration["pathBase"] + item;

                        var arquivo = File.ReadAllBytes(caminhoNome);

                        if (arquivo != null && arquivo.Length > 0)
                        {
                            await uploadFilesClient.UploadFile(item, arquivo);
                        }
                    }
                    catch (DirectoryNotFoundException)
                    {
                        contador++;
                        Console.WriteLine("Arquivo [" + item + "] ignorado por falta de diretório!!");
                        continue;
                    }
                    catch (FileNotFoundException)
                    {
                        contador++;
                        Console.WriteLine("Arquivo [" + item + "] ignorado por falta de arquivo!!");
                        continue;
                    }
                }

                        foreach (var item in nomeCertificados)
                    {
                        try
                        {
                            var caminhoNome = _configuration["pathBaseCertificado"] + item;

                            var arquivo = File.ReadAllBytes(caminhoNome);

                            if (arquivo != null && arquivo.Length > 0)
                            {
                                await uploadFilesClient.UploadFile(item, arquivo);
                            }
                    }
                    catch (DirectoryNotFoundException)
                    {
                        contador++;
                        Console.WriteLine("Arquivo [" + item + "] ignorado por falta de diretório!!");
                        continue;
                    }
                    catch (FileNotFoundException)
                    {
                        contador++;
                        Console.WriteLine("Arquivo [" + item + "] ignorado por falta de arquivo!!");
                        continue;
                        }
                    }

                        foreach (var item in nomeCurriculosCandidatos)
                    {
                        try
                        {
                            var caminhoNome = _configuration["pathBase"] + item;

                            var arquivo = File.ReadAllBytes(caminhoNome);

                            if (arquivo != null && arquivo.Length > 0)
                            {
                                await uploadFilesClient.UploadFile(item, arquivo);
                            }
                    }
                    catch (DirectoryNotFoundException)
                    {
                        contador++;
                        Console.WriteLine("Arquivo [" + item + "] ignorado por falta de diretório!!");
                        continue;
                    }
                    catch (FileNotFoundException)
                    {
                        contador++;
                        Console.WriteLine("Arquivo [" + item + "] ignorado por falta de arquivo!!");
                        continue;
                        }
                    }

                    Console.WriteLine("Total de arquivos que não subiram para o S3 pois não havia no Backup: " + contador);
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}