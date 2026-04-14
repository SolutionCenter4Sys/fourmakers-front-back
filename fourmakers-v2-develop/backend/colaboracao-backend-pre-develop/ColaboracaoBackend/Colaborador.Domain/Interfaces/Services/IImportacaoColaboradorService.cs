using Colaborador.Domain.Interfaces.Models;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Comentario;
using DataTransferObject.Domain.Dependentes;
using DataTransferObject.Domain.Endereco;
using DataTransferObject.Domain.Endosso;
using DataTransferObject.Domain.LG.Holerite;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.Usuario;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaborador.Domain.Interfaces.Services
{
    public interface IImportacaoColaboradorService
    {
        Task<string> ImportarColaborador(byte[] arquivoBytes, int orgId, string codigoInternoColaboradorCadastrante, bool importacaoLote = false);
        Task<string> ImportarColaboradorLinkedin(string perfilIN, int orgId, string codigoInternoColaboradorCadastrante);
        Task<string> ImportarColaboradorLinkedinLote(string body, int orgId, string colaboradorCadastrante);
        Task<ApiGenericResult<ProcessamentoZipResult>> ImportarColaboradorLote(IFormFile file, int orgId, string cpf);
        Task<ApiGenericResult<ProcessamentoPlanilhaResult>> ImportarColaboradorLoteLinkedin(List<string> strings, int orgId, string cpf);
        Task<ColaboradorDTO> SincronizarColaboradorBancoTalentos(string perfilIn, int orgId, string token, string email = null, string telefone= null, string codVaga = null);
        Task RegistrarLogBancoTalentoSRSAsync(string urlLinkedin, bool sucesso, string mensagemRetorno = null, string stackTrace = null);
    }
}