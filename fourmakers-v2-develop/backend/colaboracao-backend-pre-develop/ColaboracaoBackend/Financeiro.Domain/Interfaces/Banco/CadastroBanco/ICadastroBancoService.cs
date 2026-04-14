using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Banco.CadastroBanco;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Financeiro.Domain.Interfaces.Banco.CadastroBanco
{
    public interface ICadastroBancoService
    {
        Task<ApiGenericResult<IEnumerable<CadastroBancoResult>>> ListarCadastroBancos(string cpfRequest, int orgId);
        Task<ApiGenericResult<CadastroBancoResult>> ObterCadastroBancoPorCodigo(string codigoBanco, string cpfRequest, int orgId);
        Task<ApiGenericResult<CadastroBancoResult>> InserirCadastroBanco(CadastroBancoInput cadastroBancoInput, string cpfRequest, int orgId);
        Task<ApiGenericResult<CadastroBancoResult>> AtualizarCadastroBanco(CadastroBancoInput cadastroBancoInput, string codigoBanco, string cpfRequest, int orgId);
        Task<ApiGenericResult> DeletarCadastroBanco(string codigoBanco, string cpfRequest, int orgId);
    }
}
