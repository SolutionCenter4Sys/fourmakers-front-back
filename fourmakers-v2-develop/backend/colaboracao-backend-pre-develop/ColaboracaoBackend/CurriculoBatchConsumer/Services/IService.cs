using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Lote;

namespace CurriculoBatchConsumer.Services
{
    public interface IService
    {
        Task<IEnumerable<ProcessamentoCurriculoLoteDTO>> BuscarLotesCurriculosDeHoje();
        Task<bool> Processar(DataTransferObject.Domain.Colaborador.ImportacaoColaboradorFilaDTO dto, string idLote, string messageReceiptHandle);
        Task<bool> ProcessarImportacaoLinkedin(string body, string idLote, string messageReceiptHandle);
        Task<string> ResumoLotesCurriculos();
        Task<string> ResumoLotesLinkedins();
        void RegistrarOuAtualizarProcessamento(string idLote, string messageReceiptHandle, string mensagemErro, string stackTrace, string bodyMensagem);
    }
}
