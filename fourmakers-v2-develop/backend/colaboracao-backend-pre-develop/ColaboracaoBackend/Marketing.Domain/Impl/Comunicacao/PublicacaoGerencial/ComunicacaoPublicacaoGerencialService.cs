using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Core.Domain.Marketing.Comunicacao.Grupo;
using Core.Domain.Marketing.Comunicacao.Publicacao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.Publicacao;
using Marketing.Domain.Interfaces.Comunicacao.PublicacaoGerencial;
using System;
using System.Threading.Tasks;

namespace Marketing.Domain.Impl.Comunicacao.PublicacaoGerencial
{
    public class ComunicacaoPublicacaoGerencialService : IComunicacaoPublicacaoGerencialService
    {
        private const string DESCRICAO_ENTIDADE = "Marketing - Comunicacao";
        private readonly IComunicacaoPublicacaoRepository _repository;
        private readonly IComunicacaoGrupoRepository _grupoRepository;

        public ComunicacaoPublicacaoGerencialService(
            IComunicacaoPublicacaoRepository repository,
            IComunicacaoGrupoRepository grupoRepository)
        {
            _repository = repository;
            _grupoRepository = grupoRepository;
        }

        //public async Task<ApiGenericResult<PublicacaoGerencialResponseDTO>> ObterListaPublicacaoAgendadoEAprovacaoAsync(int orgId, string codigoInternoColaborador)
        //{
        //    var result = new ApiGenericResult<PublicacaoGerencialResponseDTO>();
        //    try { result.Retorno = await _repository.ObterListaPublicacaoAgendadoEAprovacaoAsync(orgId, codigoInternoColaborador); }
        //    catch (Exception ex) { ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE); }
        //    return result;
        //}

        public async Task<ApiGenericResult> PublicarAgoraAsync(string publicacaoId, string codigoInternoColaborador, int orgId)
        {
            var result = new ApiGenericResult();
            try
            {
                if (string.IsNullOrWhiteSpace(publicacaoId)) throw new ArgumentException("Id da publicação obrigatório.");
                var sucesso = await _repository.PublicarAgoraAsync(publicacaoId, codigoInternoColaborador, orgId);
                if (!sucesso) throw new ApplicationException("Publicação não encontrada para publicar agora.");
                result.Mensagem = "Publicada com sucesso.";
            }
            catch (Exception ex) { ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE); }
            return result;
        }

        public async Task<ApiGenericResult> AprovarComunicacaoAsync(string publicacaoId, string codigoInternoColaborador, int orgId)
        {
            var result = new ApiGenericResult();
            try
            {
                if (string.IsNullOrWhiteSpace(publicacaoId)) throw new ArgumentException("Id da publicação obrigatório.");
                var podeAprovar = await _grupoRepository.UsuarioPodeAprovarPublicacaoOficialAsync(codigoInternoColaborador, orgId);
                if (!podeAprovar)
                    throw new ApplicationException("Você não possui permissão para aprovar publicações. É necessário estar em um grupo que permita aprovar publicação oficial.");
                var sucesso = await _repository.AprovarComunicacaoAsync(publicacaoId, codigoInternoColaborador, orgId);
                if (!sucesso) throw new ApplicationException("Publicação não encontrada para aprovação.");
                result.Mensagem = "Publicação aprovada com sucesso.";
            }
            catch (Exception ex) { ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE); }
            return result;
        }

        public async Task<ApiGenericResult> RejeitarComunicacaoAsync(string publicacaoId, string codigoInternoColaborador, int orgId, RejeitarComunicacaoRequestDTO request)
        {
            var result = new ApiGenericResult();
            try
            {
                if (string.IsNullOrWhiteSpace(publicacaoId)) throw new ArgumentException("Id da publicação obrigatório.");
                if (request == null || string.IsNullOrWhiteSpace(request.MotivoRejeicao)) throw new ArgumentException("Motivo de rejeição obrigatório.");
                var podeAprovar = await _grupoRepository.UsuarioPodeAprovarPublicacaoOficialAsync(codigoInternoColaborador, orgId);
                if (!podeAprovar)
                    throw new ApplicationException("Você não possui permissão para rejeitar publicações. É necessário estar em um grupo que permita aprovar publicação oficial.");
                var sucesso = await _repository.RejeitarComunicacaoAsync(publicacaoId, codigoInternoColaborador, orgId, request.MotivoRejeicao);
                if (!sucesso) throw new ApplicationException("Publicação não encontrada para rejeição.");
                result.Mensagem = "Publicação foi rejeitada.";
            }
            catch (Exception ex) { ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE); }
            return result;
        }
    }
}
