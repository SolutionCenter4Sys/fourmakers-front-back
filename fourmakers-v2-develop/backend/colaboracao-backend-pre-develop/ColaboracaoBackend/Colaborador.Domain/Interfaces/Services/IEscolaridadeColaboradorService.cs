using Core.Domain.Colaborador;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Escolaridade;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaborador.Domain.Interfaces.Services
{
    public interface IEscolaridadeColaboradorService
    {
        Task<EscolaridadeDTO> AdicionarNovaEscolaridade(long formacaoId, string instituicao, DateTime dataInicio, DateTime? dataTermino, string descricao, string colaboradorCpf, int? tipoDiplomaId, byte[] certificado, string fileType, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        List<EscolaridadeDTO> ListarEscolaridadeColaborador(string busca, int cursor, int limite, string cpf);
        EscolaridadeDTO GetEscolaridadeColaboradorById(long id);
        void RemoverEscolaridadeColaborador(long escolaridadeId, string cpf, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        Task<EscolaridadeDTO> AtualizarEscolaridade(long id, long? formacaoId, string instituicao, DateTime dataInicio, DateTime? dataTermino, string descricao, int tipoDiplomaId, string colaboradorCpf, byte[] certificado, string fileType, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
    }
}
