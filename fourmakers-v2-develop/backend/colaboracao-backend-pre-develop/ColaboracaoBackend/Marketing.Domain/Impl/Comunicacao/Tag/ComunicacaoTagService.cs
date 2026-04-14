using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Core.Domain.Marketing.Comunicacao.Tag;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.Tag;
using Marketing.Domain.Interfaces.Comunicacao.Tag;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketing.Domain.Impl.Comunicacao.Tag
{
    public class ComunicacaoTagService : IComunicacaoTagService
    {
        private const string DESCRICAO_ENTIDADE = "Marketing - Comunicacao";
        private readonly IComunicacaoTagRepository _repository;
        private readonly IDBConnectionUnitOfWork _dbConnectionUnitOfWork;

        public ComunicacaoTagService(
            IComunicacaoTagRepository repository,
            IDBConnectionUnitOfWork dbConnectionUnitOfWork)
        {
            _repository = repository;
            _dbConnectionUnitOfWork = dbConnectionUnitOfWork;
        }

        public async Task<ApiGenericResult<IEnumerable<TagResumoDTO>>> ObterTodasTagsAsync(int orgId)
        {
            var result = new ApiGenericResult<IEnumerable<TagResumoDTO>>();
            try
            {
                result.Retorno = await _repository.ObterTodasPorOrgAsync(orgId);
            }
            catch (Exception ex) { ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE); }
            return result;
        }

        public async Task<ApiGenericResult> AtualizarTagAsync(string tagId, AtualizarTagRequestDTO request, int orgId)
        {
            var result = new ApiGenericResult();
            try
            {
                if (string.IsNullOrWhiteSpace(tagId)) throw new ArgumentException("Id da tag obrigatório.");
                if (request == null || string.IsNullOrWhiteSpace(request.Nome)) throw new ArgumentException("Nome da tag obrigatório.");
                var sucesso = await _repository.AtualizarTagAsync(tagId, request.Nome, orgId);
                if (!sucesso) throw new ApplicationException("Tag não encontrada para atualização.");
                result.Mensagem = "Tag atualizada com sucesso.";
            }
            catch (Exception ex) { ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE); }
            return result;
        }

        public async Task<ApiGenericResult> RemoverTagAsync(string tagId, int orgId)
        {
            var result = new ApiGenericResult();
            _dbConnectionUnitOfWork.BeginTransaction();
            try
            {
                if (string.IsNullOrWhiteSpace(tagId)) throw new ArgumentException("Id da tag obrigatório.");
                var sucesso = await _repository.RemoverTagAsync(tagId, orgId);
                if (!sucesso) throw new ApplicationException("Tag não encontrada para remoção.");
                result.Mensagem = "Tag deletada com sucesso.";

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
