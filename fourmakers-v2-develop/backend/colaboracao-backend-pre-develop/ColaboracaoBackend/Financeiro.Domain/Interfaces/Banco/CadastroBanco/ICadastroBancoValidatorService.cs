using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Financeiro.Banco.CadastroBanco;
using System.Threading.Tasks;

namespace Financeiro.Domain.Interfaces.Banco.CadastroBanco
{
    public interface ICadastroBancoValidatorService
    {
        Task ValidaCadastroBanco(CadastroBancoInput cadastroBancoInput, CRUDEnum create);
    }
}
