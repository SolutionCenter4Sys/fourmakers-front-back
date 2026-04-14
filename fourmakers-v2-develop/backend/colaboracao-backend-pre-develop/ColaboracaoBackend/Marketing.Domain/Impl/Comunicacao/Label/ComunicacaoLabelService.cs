using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Core.Domain.Marketing.Comunicacao.Label;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.Label;
using Marketing.Domain.Interfaces.Comunicacao.Label;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketing.Domain.Impl.Comunicacao.Label
{
    public class ComunicacaoLabelService : IComunicacaoLabelService
    {
        private const string DESCRICAO_ENTIDADE = "Marketing - Comunicacao";
        private readonly IComunicacaoLabelRepository _repository;
        private readonly IDBConnectionUnitOfWork _dbConnectionUnitOfWork;

        public ComunicacaoLabelService(
            IComunicacaoLabelRepository repository,
            IDBConnectionUnitOfWork dbConnectionUnitOfWork)
        {
            _repository = repository;
            _dbConnectionUnitOfWork = dbConnectionUnitOfWork;
        }

        public async Task<ApiGenericResult<IEnumerable<LabelResumoDTO>>> ObterTodasLabelsAsync(int orgId, IEnumerable<string> tipos = null)
        {
            var result = new ApiGenericResult<IEnumerable<LabelResumoDTO>>();
            try
            {
                result.Retorno = await _repository.ObterTodasPorOrgAsync(orgId, tipos ?? new[] { "informativo", "documento" });
            }
            catch (Exception ex) { ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE); }
            return result;
        }

        public async Task<ApiGenericResult> AtualizarLabelAsync(string labelId, AtualizarLabelRequestDTO request, int orgId)
        {
            var result = new ApiGenericResult();
            try
            {
                if (string.IsNullOrWhiteSpace(labelId)) throw new ArgumentException("Id da label obrigatório.");
                if (request == null || string.IsNullOrWhiteSpace(request.Nome)) throw new ArgumentException("Nome da label obrigatório.");
                var sucesso = await _repository.AtualizarLabelAsync(labelId, request.Nome, request.Tipo ?? "informativo", orgId);
                if (!sucesso) throw new ApplicationException("Label não encontrada para atualização.");
                result.Mensagem = "Label atualizada com sucesso.";
            }
            catch (Exception ex) { ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE); }
            return result;
        }

        public async Task<ApiGenericResult> RemoverLabelAsync(string labelId, int orgId)
        {
            var result = new ApiGenericResult();
            _dbConnectionUnitOfWork.BeginTransaction();
            try
            {
                if (string.IsNullOrWhiteSpace(labelId)) throw new ArgumentException("Id da label obrigatório.");
                var sucesso = await _repository.RemoverLabelAsync(labelId, orgId);
                if (!sucesso) throw new ApplicationException("Label não encontrada para remoção.");
                result.Mensagem = "Label deletada com sucesso.";

                _dbConnectionUnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.Rollback();
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Delete, DESCRICAO_ENTIDADE);
            }
            return result;
        }
    }
}
