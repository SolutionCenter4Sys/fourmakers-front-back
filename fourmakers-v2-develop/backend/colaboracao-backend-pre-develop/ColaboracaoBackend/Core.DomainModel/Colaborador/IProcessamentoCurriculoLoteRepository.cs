using DataTransferObject.Domain.BancoDeTalentos;
using DataTransferObject.Domain.Colaborador;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Colaborador
{
    public interface IProcessamentoCurriculoLoteRepository
    {
        void AtualizarQuantidadeDeArquivosAProcessar(Guid idLote, int arquivosAProcessar);
        Task<IEnumerable<ProcessamentoCurriculoLoteDTO>> BuscarLotesDiariosPorTipo(DateTime utcNow, string identificador_fila);
        Task<IEnumerable<ProcessamentoCurriculoLoteDTO>> BuscarMeusLotes(string codInternoColaborador, int orgId);
        Task<IEnumerable<BuscarPessoasQueEuCadastrei>> BuscarPessoasCadastradasDesteLote(string idLote);
        ProcessamentoCurriculoLoteDTO BuscarPorId(string idLote);
        void Cadastrar(Guid idLote, int orgId, DateTime utcNow, int totalArquivos, string codColaborador, string identificadorFila);
        void CadastrarTabelaAuxiliar(string messageReceiptHandle, string idLote, string codColaborador = null, string nomeArquivoCv = null);
        void AtualizarProcessamento(string idLote, string messageReceiptHandle, string mensagemErro, string stackTrace, string bodyMensagem);
        bool ExisteRegistro(string idLote, string messageReceiptHandle);
        Task<IEnumerable<ErroProcessamentoCurriculoDTO>> BuscarErrosPorLote(string idLote);
        Task<IEnumerable<ErroProcessamentoCurriculoDTO>> BuscarErrosPorMessageReceiptHandle(string messageReceiptHandle);
        Task<IEnumerable<ErroProcessamentoCurriculoDTO>> BuscarErrosPorPeriodo(DateTime dataInicio, DateTime dataFim);
        Task<IEnumerable<ErroProcessamentoCurriculoDTO>> BuscarTodosErros();
        void IncrementarQuantidadeProcessada(string idLote, int quantidadeProcessadaAtualizada);
        void IniciaProcessamento(string idLote);
        void MarcarComoProcessado(string idLote);
        void InserirCodColaboradorNaTabelaAuxiliar(string messageReceiptHandle, string idLote, string codColaborador);
        Task<IEnumerable<string>> BuscarNomesArquivos(string idLote);
        Task<IEnumerable<ErroProcessamentoCurriculoDTO>> BuscarErrosPorLoteInformacoesBasicas(string idLote);
    }
}