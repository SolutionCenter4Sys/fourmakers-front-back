using DataTransferObject.Domain.Financeiro.Banco.CadastroBanco;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Financeiro.Banco
{
    public interface ICadastroBancoRepository
    {
        Task<IEnumerable<CadastroBancoResult>> ListarCadastroBancosAsync();
        Task<CadastroBancoResult> ObterCadastroBancoPorCodigoAsync(string codigo);
        Task<CadastroBancoResult> InserirCadastroBancoAsync(CadastroBancoInput parametroRepositoryInput);
        Task<CadastroBancoResult> AtualizarCadastroBancoAsync(CadastroBancoInput parametroRepositoryInput);
        Task<bool> DeletarCadastroBancoAsync(string codigoBanco);
    }

}
