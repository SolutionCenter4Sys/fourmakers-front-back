using Colaboracao.Core;
using Colaboracao.Helper;
using Core.Domain.Notificacao;
using DataTransferObject.Domain.Notificacao;
using DataTransferObject.Domain.Usuario;
using Firebase.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Firebase.Domain.Impl.Services
{
    [HandleException]
    [LogDomainClass]
    public class NotificacaoService : INotificacaoService
    {
        private readonly INotificacaoRepository _notificacaoRepository;

        public NotificacaoService(INotificacaoRepository notificacaoRepository)
        {
            _notificacaoRepository = notificacaoRepository;
        }

        public async Task<int> ContarNotificacoesNaoLidasColaborador(string cpf, int orgId)
        {
            return await _notificacaoRepository.ContarNotificacoesNaoLidasColaborador(cpf, orgId);
        }

        public async Task<NotificacaoDTO> InserirNotificacaoColaborador(NotificacaoDTO notificacaoDTO)
        {
            var (cpf, titulo, mensagem, org) = notificacaoDTO;
            ValidacaoUtil.ObrigaStringNotNullOrEmpty(cpf);
            ValidacaoUtil.ObrigaStringNotNullOrEmpty(titulo);
            ValidacaoUtil.ObrigaStringNotNullOrEmpty(mensagem);
            ValidacaoUtil.ObrigaIntNotNullOrZero(org);
            notificacaoDTO.Lida = false;
            notificacaoDTO.DataLeitura = null;
            notificacaoDTO.DataEnvio = DateTime.Now;
            return await _notificacaoRepository.InserirNotificacaoColaborador(notificacaoDTO);
        }

        public async Task<NotificacaoDTO> EnviarNotificacaoColaborador(string cpf, int orgId, string titulo, string mensagem, string mensagemHtml, FuncionalidadeSistemaEnum? funcionalidade, string urlCustomizada = null)
        {
            var currentDateTimeInBrasilia = new DateTimeOffset(DateTime.Now).DateTime;
            var notificacao = new NotificacaoDTO
            {
                Id = null,
                ColaboradorCpf = cpf,
                Titulo = titulo,
                Mensagem = mensagem,
                MensagemHtml = mensagemHtml,
                Lida = false,
                DataEnvio = currentDateTimeInBrasilia,
                DataLeitura = null,
                TbFuncionalidadeSistemaId = funcionalidade != null ? (int)funcionalidade : null,
                OrgId = orgId,
                UrlCustomizada = string.IsNullOrWhiteSpace(urlCustomizada) ? null : urlCustomizada.Trim(),
                Rota = null,
                RotaCompleta = null
            };
            return await InserirNotificacaoColaborador(notificacao);
        }

        public async Task<List<NotificacaoDTO>> ListarNotificacoesColaborador(string cpf, int orgId, bool apenasNaoLidas)
        {
            return await _notificacaoRepository.ListarNotificacoesColaborador(cpf, orgId, apenasNaoLidas);
        }

        public async Task<bool> MarcarNotificacoesComoLidasColaborador(string cpf, int orgId)
        {
            return await _notificacaoRepository.MarcarNotificacoesComoLidasColaborador(cpf, orgId);
        }
    }
}