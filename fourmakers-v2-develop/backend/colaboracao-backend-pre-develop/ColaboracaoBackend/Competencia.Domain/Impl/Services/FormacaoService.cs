using Colaboracao.Core.Interfaces;
using Core.Domain.Formacao;
using DataTransferObject.Domain.Formacao;
using Formacao.Domain.Interfaces.Services;
using Logs.Infra.Attributes;
using System;
using System.Collections.Generic;

namespace Formacao.Domain.Impl.Services
{
    [LogDomainClass]
    public class FormacaoService : IFormacaoService
    {
        private readonly IAspNetUser _aspNetUser;
        private readonly IFormacaoRepository _formacaoRepository;

        public FormacaoService(IAspNetUser aspNetUser,
                               IFormacaoRepository formacaoRepository)
        {
            _aspNetUser = aspNetUser;
            _formacaoRepository = formacaoRepository;
        }

        public FormacaoDTO AddFormacao(string descricao)
        {
            try
            {
                var formacao = new FormacaoDTO
                {
                    Descricao = descricao
                };
                return _formacaoRepository.AddFormacao(formacao, _aspNetUser.GetUsuarioLogado() is null ? ApiClient.Domain.ClientConfig.Clients.Colaborador.CpfAdmin : _aspNetUser.GetUsuarioLogado().Cpf);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<FormacaoDTO> ListarFormacao(string busca, int cursor, int limite)
        {
            try
            {
                return _formacaoRepository.ListarFormacao(busca, cursor, limite);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public FormacaoDTO GetFormacaoById(long id)
        {
            try
            {
                return _formacaoRepository.GetFormacaoById((int)id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}