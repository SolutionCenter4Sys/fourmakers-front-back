using System;
using System.Threading.Tasks;
using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Helper;
using Colaborador.Domain.Interfaces.Services;
using Core.Domain;
using Core.DomainModel.Projeto;
using DataTransferObject.Domain.Arquivo;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Endereco;

namespace Colaborador.Domain.Impl.Services;

public class EnderecoService(IEnderecoDapperRepository enderecoDapperRepository, IUploadFilesClient uploadFilesClient) : IEnderecoService
{
    public async Task<ApiGenericResult<EnderecoDTO>> EditarEnderecoAsync(EnderecoInputDTO enderecoDTO, string codigoInternoColaborador)
    {
        var apiGenericResult = new ApiGenericResult<EnderecoDTO>();
        
        var enderecoColaborador = await enderecoDapperRepository.ObterEnderecoPorCodigoInternoColaboradorAsync(codigoInternoColaborador);
        
        if (enderecoColaborador == null)
        {
            enderecoColaborador = await enderecoDapperRepository.CriarEnderecoAsync(enderecoDTO);
        }
        else
        {
            enderecoColaborador = await enderecoDapperRepository.AtualizarEnderecoAsync(enderecoColaborador.Id!.Value, enderecoDTO);
        }
        
        
        apiGenericResult.Retorno = enderecoColaborador;
        return apiGenericResult;
    }

    public async Task<ApiGenericResult<EnderecoDTO>> ObterEnderecoPorCodigoInternoColaboradorAsync(string codigoInternoColaborador)
    {
        var apiGenericResult = new ApiGenericResult<EnderecoDTO>();
        var queryResult = await enderecoDapperRepository.ObterEnderecoPorCodigoInternoColaboradorAsync(codigoInternoColaborador);
        apiGenericResult.Retorno = queryResult;
        return apiGenericResult;
    }
    
    public async Task<ApiGenericResult<EnderecoDTO>> ObterEnderecoPorCodigoAsync(long id)
    {
        var apiGenericResult = new ApiGenericResult<EnderecoDTO>();
        var queryResult = await enderecoDapperRepository.ObterEnderecoPorIdAsync(id);
        apiGenericResult.Retorno = queryResult;
        return apiGenericResult;
    }
}