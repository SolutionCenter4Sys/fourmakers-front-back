using System;
using Colaboracao.Core;
using Core.Domain.Social;
using DataTransferObject.Domain.Social;
using DataTransferObject.Domain.Usuario;
using Core.DomainModel;
using Firebase.Domain.Interfaces.Services;
using Logs.Infra.Attributes;
using Social.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;

namespace Social.Domain.Impl
{
    [LogDomainClass]
    public class Feedback360Service : IFeedback360Service
    {
        private readonly IFeedback360Repository _repository;
        private readonly IFirebaseService _firebaseService;
        private readonly INotificacaoService _notificacaoService;
        private readonly IEnvioEmail _envioEmail;
        private readonly IFirebaseRepository _firebaseRepository;
        private static readonly TimeSpan JanelaEdicao = TimeSpan.FromHours(1);
        private const int MaxDestinatariosPorFeedback = 50;

        private const string UrlLojaGooglePlay =
            "https://play.google.com/store/apps/details?id=br.com.portalcolaborador.foursys&pcampaignid=web_share";
        private const string UrlLojaAppStore = "https://apps.apple.com/br/app/fourmakers/id1484778641";
        private const string ImgBadgeGooglePlay =
            "https://fsys2-public.s3.us-east-1.amazonaws.com/disponivel-google-play-badge.png";
        private const string ImgBadgeAppStore =
            "https://fsys2-public.s3.us-east-1.amazonaws.com/disponivel-na-app-store-badge.png";

        public Feedback360Service(
            IFeedback360Repository repository,
            IFirebaseService firebaseService,
            INotificacaoService notificacaoService,
            IEnvioEmail envioEmail,
            IFirebaseRepository firebaseRepository)
        {
            _repository = repository;
            _firebaseService = firebaseService;
            _notificacaoService = notificacaoService;
            _envioEmail = envioEmail;
            _firebaseRepository = firebaseRepository;
        }

        public async Task<Feedback360DTO> EnviarFeedbackAsync(EnviarFeedback360DTO dto, UsuarioLogadoDTO usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario?.Cpf))
                throw new UnauthorizedAccessException("Usuário não autenticado na organização.");

            if (usuario.OrgId <= 0)
                throw new UnauthorizedAccessException("Organização não identificada.");

            var codigosDestinatarios = NormalizarListaDestinatarios(dto?.CodigosInternosColaboradoresDestinatarios);
            if (codigosDestinatarios.Count == 0)
                throw new ArgumentException("Informe ao menos um colaborador destinatário.");

            if (codigosDestinatarios.Count > MaxDestinatariosPorFeedback)
                throw new ArgumentException($"É permitido no máximo {MaxDestinatariosPorFeedback} destinatários por feedback.");

            var guidRemetente = Guid.Parse(usuario.Cpf.Trim());
            foreach (var cod in codigosDestinatarios)
            {
                if (guidRemetente == cod)
                    throw new ArgumentException("Não é permitido enviar feedback para si mesmo.");
            }

            if (string.IsNullOrWhiteSpace(dto.Situacao))
                throw new ArgumentException("Situação é obrigatória.");

            if (string.IsNullOrWhiteSpace(dto.Tarefa))
                throw new ArgumentException("Tarefa é obrigatória.");

            if (string.IsNullOrWhiteSpace(dto.Acao))
                throw new ArgumentException("Ação é obrigatória.");

            if (string.IsNullOrWhiteSpace(dto.Resultado))
                throw new ArgumentException("Resultado é obrigatório.");

            if (dto.DataInteracao.Date > DateTime.UtcNow.Date)
                throw new ArgumentException("Data de interação não pode ser futura. Apenas hoje ou datas passadas.");

            var relacionamentoOutroTexto = ResolverRelacionamentoOutroEspecificacao(
                dto.Feedback360RelacionamentoId,
                dto.RelacionamentoOutroEspecificacao);

            var feedback = new Feedback360DTO
            {
                Id = Guid.NewGuid(),
                CodigoInternoColaboradorRemetente = Guid.Parse(usuario.Cpf),
                OrgId = usuario.OrgId,
                Situacao = dto.Situacao.Trim(),
                Tarefa = dto.Tarefa.Trim(),
                Acao = dto.Acao.Trim(),
                Resultado = dto.Resultado.Trim(),
                Previa = string.IsNullOrWhiteSpace(dto.Previa) ? null : dto.Previa.Trim(),
                DataInteracao = dto.DataInteracao.Date,
                Feedback360RelacionamentoId = dto.Feedback360RelacionamentoId,
                RelacionamentoOutroEspecificacao = relacionamentoOutroTexto,
                Feedback360AvaliacaoId = dto.Feedback360AvaliacaoId,
                DataCriacao = DateTime.UtcNow,
                Editado = false
            };

            await _repository.AddAsync(feedback, codigosDestinatarios);

            var feedbackSalvo = await _repository.GetByIdAsync(feedback.Id);

            await EnviarNotificacoesParaDestinatariosAsync(feedbackSalvo);

            return feedbackSalvo;
        }

        private static List<Guid> NormalizarListaDestinatarios(IReadOnlyList<Guid> codigos)
        {
            var set = new HashSet<Guid>();
            if (codigos != null)
            {
                foreach (var c in codigos)
                {
                    if (c != Guid.Empty)
                        set.Add(c);
                }
            }

            return set.ToList();
        }

        private async Task EnviarNotificacoesParaDestinatariosAsync(Feedback360DTO feedbackSalvo)
        {
            if (feedbackSalvo?.Destinatarios == null || feedbackSalvo.Destinatarios.Count == 0)
                return;

            foreach (var dest in feedbackSalvo.Destinatarios)
                await EnviarNotificacaoParaUmDestinatarioAsync(feedbackSalvo, dest);
        }

        private async Task EnviarNotificacaoParaUmDestinatarioAsync(Feedback360DTO feedbackSalvo, Feedback360DestinatarioItemDTO destinatario)
        {
            if (feedbackSalvo == null || destinatario == null) return;

            try
            {
                var codigoDestinatario = destinatario.CodigoInternoColaborador.ToString();
                var nomeRemetente = feedbackSalvo.NomeRemetente ?? "Alguém";
                var titulo = "Você recebeu um novo reconhecimento!";
                var descricao = $"{nomeRemetente} enviou um reconhecimento para você. Acesse o módulo de Reconhecimentos para visualizá-lo.";

                await _firebaseService.EnviarNotificacaoPushApp(codigoDestinatario, titulo, descricao);

                await _notificacaoService.EnviarNotificacaoColaborador(
                    codigoDestinatario,
                    feedbackSalvo.OrgId,
                    titulo,
                    descricao,
                    string.Empty,
                    FuncionalidadeSistemaEnum.FEEDBACK360_GERAL);

                var emailDestinatario = await _firebaseRepository.BuscarEmailColaborador(codigoDestinatario);

                if (!string.IsNullOrEmpty(emailDestinatario))
                {
                    var assuntoEmail = "Você recebeu um novo reconhecimento";
                    var nomeRemetenteHtml = WebUtility.HtmlEncode(nomeRemetente);
                    var hrefPlay = WebUtility.HtmlEncode(UrlLojaGooglePlay);
                    var hrefAppStore = WebUtility.HtmlEncode(UrlLojaAppStore);
                    var corpoEmail =
                        "<p>Alguém reconheceu seu trabalho — e você vai querer ver isso 😊</p>" +
                        $"<p><strong>{nomeRemetenteHtml}</strong> deixou um reconhecimento para você na plataforma.</p>" +
                        "<p>Corre lá no app para conferir!</p>" +
                        "<table role=\"presentation\" width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"margin-top:16px;max-width:340px;\">" +
                        "<tr>" +
                        "<td width=\"50%\" align=\"center\" style=\"padding:4px 6px 4px 0;vertical-align:middle;\">" +
                        $"<a href=\"{hrefPlay}\" target=\"_blank\" rel=\"noopener noreferrer\" style=\"display:inline-block;max-width:160px;width:100%;\">" +
                        $"<img src=\"{ImgBadgeGooglePlay}\" alt=\"Disponível no Google Play\" width=\"160\" " +
                        "style=\"display:block;width:100%;max-width:160px;height:auto;border:0;\" />" +
                        "</a></td>" +
                        "<td width=\"50%\" align=\"center\" style=\"padding:4px 0 4px 6px;vertical-align:middle;\">" +
                        $"<a href=\"{hrefAppStore}\" target=\"_blank\" rel=\"noopener noreferrer\" style=\"display:inline-block;max-width:160px;width:100%;\">" +
                        $"<img src=\"{ImgBadgeAppStore}\" alt=\"Baixar na App Store\" width=\"160\" " +
                        "style=\"display:block;width:100%;max-width:160px;height:auto;border:0;\" />" +
                        "</a></td></tr></table>";

                    var nomeParaTemplate = destinatario.NomeCompleto ?? string.Empty;
                    _envioEmail.EnviaEmailTemplateFoursys(
                        nomeParaTemplate,
                        corpoEmail,
                        assuntoEmail,
                        emailDestinatario);
                }
            }
            catch
            {
            }
        }

        public async Task<Feedback360DTO> AtualizarFeedbackAsync(Guid id, AtualizarFeedback360DTO dto, UsuarioLogadoDTO usuario)
        {
            var feedback = await _repository.GetByIdAsync(id);
            if (feedback == null)
                throw new ArgumentException("Feedback não encontrado.");

            if (feedback.CodigoInternoColaboradorRemetente.ToString() != usuario.Cpf)
                throw new UnauthorizedAccessException("Somente o autor pode editar o feedback.");

            var tempoDesdeCriacao = DateTime.UtcNow - feedback.DataCriacao;
            if (tempoDesdeCriacao > JanelaEdicao)
                throw new InvalidOperationException("Edição permitida apenas na primeira 1 hora após a criação.");

            if (string.IsNullOrWhiteSpace(dto.Situacao))
                throw new ArgumentException("Situação é obrigatória.");

            if (string.IsNullOrWhiteSpace(dto.Tarefa))
                throw new ArgumentException("Tarefa é obrigatória.");

            if (string.IsNullOrWhiteSpace(dto.Acao))
                throw new ArgumentException("Ação é obrigatória.");

            if (string.IsNullOrWhiteSpace(dto.Resultado))
                throw new ArgumentException("Resultado é obrigatório.");

            if (dto.DataInteracao.Date > DateTime.UtcNow.Date)
                throw new ArgumentException("Data de interação não pode ser futura.");

            var relacionamentoOutroTexto = ResolverRelacionamentoOutroEspecificacao(
                dto.Feedback360RelacionamentoId,
                dto.RelacionamentoOutroEspecificacao);

            var objetoAntigo = JsonSerializer.Serialize(new
            {
                Situacao = feedback.Situacao,
                Tarefa = feedback.Tarefa,
                Acao = feedback.Acao,
                Resultado = feedback.Resultado,
                Previa = feedback.Previa,
                DataInteracao = feedback.DataInteracao,
                Feedback360RelacionamentoId = feedback.Feedback360RelacionamentoId,
                RelacionamentoOutroEspecificacao = feedback.RelacionamentoOutroEspecificacao,
                Feedback360AvaliacaoId = feedback.Feedback360AvaliacaoId
            });

            feedback.Situacao = dto.Situacao.Trim();
            feedback.Tarefa = dto.Tarefa.Trim();
            feedback.Acao = dto.Acao.Trim();
            feedback.Resultado = dto.Resultado.Trim();
            feedback.Previa = string.IsNullOrWhiteSpace(dto.Previa) ? null : dto.Previa.Trim();
            feedback.DataInteracao = dto.DataInteracao.Date;
            feedback.Feedback360RelacionamentoId = dto.Feedback360RelacionamentoId;
            feedback.RelacionamentoOutroEspecificacao = relacionamentoOutroTexto;
            feedback.Feedback360AvaliacaoId = dto.Feedback360AvaliacaoId;
            feedback.DataAlteracao = DateTime.UtcNow;
            feedback.Editado = true;

            var objetoNovo = JsonSerializer.Serialize(new
            {
                Situacao = feedback.Situacao,
                Tarefa = feedback.Tarefa,
                Acao = feedback.Acao,
                Resultado = feedback.Resultado,
                Previa = feedback.Previa,
                DataInteracao = feedback.DataInteracao,
                Feedback360RelacionamentoId = feedback.Feedback360RelacionamentoId,
                RelacionamentoOutroEspecificacao = feedback.RelacionamentoOutroEspecificacao,
                Feedback360AvaliacaoId = feedback.Feedback360AvaliacaoId
            });

            await _repository.UpdateAsync(feedback);

            await _repository.InserirLogEdicaoAsync(
                Guid.NewGuid(),
                id,
                Guid.Parse(usuario.Cpf.Trim()),
                "UPDATE",
                objetoAntigo,
                objetoNovo);

            return await _repository.GetByIdAsync(id);
        }

        public async Task<Feedback360DTO> GetByIdAsync(Guid id) => await _repository.GetByIdAsync(id);

        public async Task<IEnumerable<Feedback360DTO>> ListarEnviadosAsync(UsuarioLogadoDTO usuario, FiltroFeedback360DTO filtro)
        {
            if (string.IsNullOrWhiteSpace(usuario?.Cpf))
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            var codigoInterno = Guid.Parse(usuario.Cpf.Trim());
            return await _repository.ListarEnviadosPorColaboradorAsync(codigoInterno, AplicarDataPadrao(filtro));
        }

        public async Task<Feedback360RecebidosResultadoDTO> ListarRecebidosAsync(UsuarioLogadoDTO usuario, FiltroFeedback360DTO filtro)
        {
            if (string.IsNullOrWhiteSpace(usuario?.Cpf))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            var codigoInterno = Guid.Parse(usuario.Cpf.Trim());
            var feedbacks = (await _repository.ListarRecebidosPorColaboradorAsync(codigoInterno, AplicarDataPadrao(filtro))).ToList();

            return new Feedback360RecebidosResultadoDTO
            {
                TotalFeedbacks = feedbacks.Count,
                ListaFeedbacks = feedbacks
            };
        }

        private static FiltroFeedback360DTO AplicarDataPadrao(FiltroFeedback360DTO filtro)
        {
            filtro ??= new FiltroFeedback360DTO();

            if (!filtro.DataInicio.HasValue && !filtro.DataFim.HasValue)
            {
                filtro.DataFim = DateTime.UtcNow.Date;
                filtro.DataInicio = filtro.DataFim.Value.AddMonths(-6);
            }

            return filtro;
        }

        public async Task<FeedbackGestorResultadoDTO> ListarRecebidosGestorAsync(FiltroFeedbackGestorDTO filtro)
        {
            if (filtro == null || filtro.CodigoInternoColaborador == Guid.Empty)
                throw new ArgumentException("O código interno do colaborador é obrigatório.");

            var dataFim = filtro.DataFim?.Date ?? DateTime.UtcNow.Date;
            var dataInicio = filtro.DataInicio?.Date ?? dataFim.AddMonths(-6);

            if (dataInicio > dataFim)
                throw new ArgumentException("A data de início não pode ser maior que a data de fim.");

            var feedbacks = (await _repository.ListarRecebidosPorColaboradorFiltradoAsync(
                filtro.CodigoInternoColaborador,
                dataInicio,
                dataFim,
                filtro.AvaliacaoId)).ToList();

            return new FeedbackGestorResultadoDTO
            {
                TotalFeedbacks = feedbacks.Count,
                ListaFeedbacks = feedbacks
            };
        }

        public async Task<ListagemColaboradoresGestorFeedbackDTO> ListarColaboradoresGestorAsync(Guid codigoInternoGestor, int orgId)
        {
            if (codigoInternoGestor == Guid.Empty)
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            var colaboradores = await _repository.ListarSubordinadosGestorAsync(codigoInternoGestor, orgId);

            return new ListagemColaboradoresGestorFeedbackDTO
            {
                Colaboradores = colaboradores.ToList()
            };
        }

        public async Task<IEnumerable<Feedback360RelacionamentoDTO>> ListarRelacionamentosAsync() => await _repository.ListarRelacionamentosAsync();

        public async Task<IEnumerable<Feedback360AvaliacaoDTO>> ListarAvaliacoesAsync() => await _repository.ListarAvaliacoesAsync();

        public async Task<GerarModeloStarMoxeResultadoDTO> GerarModeloStarMoxeAsync(GerarModeloStarMoxeDTO dto, UsuarioLogadoDTO usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario?.Cpf))
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            var dataInteracaoStr = dto?.DataInteracao.HasValue == true
                ? dto.DataInteracao.Value.ToString("dd/MM/yyyy")
                : string.Empty;
            return await _repository.GerarModeloStarMoxeAsync(
                dto?.Situacao ?? string.Empty,
                dataInteracaoStr,
                dto?.Contexto ?? string.Empty);
        }

        public async Task<IEnumerable<Feedback360MuralReconhecimentoDTO>> BuscarMuralReconhecimentoAsync(UsuarioLogadoDTO usuario, FiltroFeedback360DTO filtro)
        {
            if (string.IsNullOrWhiteSpace(usuario?.Cpf))
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            if (usuario.OrgId <= 0)
                throw new UnauthorizedAccessException("Organização não identificada.");

            filtro = AplicarDataPadrao(filtro);
            var codigoInternoLogado = Guid.Parse(usuario.Cpf.Trim());
            return await _repository.BuscarMuralReconhecimentoAsync(usuario.OrgId, filtro, codigoInternoLogado);
        }

        public async Task<IEnumerable<Feedback360MuralReacaoDTO>> ListarReacoesMuralAsync() => await _repository.ListarReacoesMuralAsync();

        public async Task<int> DefinirReacaoMuralAsync(Guid feedback360Id, int reacaoId, UsuarioLogadoDTO usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario?.Cpf))
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            if (feedback360Id == Guid.Empty)
                throw new ArgumentException("Id do feedback é obrigatório.");
            if (reacaoId < 0)
                throw new ArgumentException("ReacaoId deve ser 0 (remover) ou o id do emoji (maior que zero).");

            return await _repository.DefinirReacaoMuralAsync(feedback360Id, reacaoId, Guid.Parse(usuario.Cpf.Trim()));
        }

        /// <summary>
        /// Quando o id de relacionamento é <see cref="EnumFeedback360Relacionamentos.Outro"/>, exige texto (1–60 caracteres).
        /// Nos demais casos retorna null e ignora o valor enviado.
        /// </summary>
        private static string ResolverRelacionamentoOutroEspecificacao(int relacionamentoId, string especificacao)
        {
            if (relacionamentoId != (int)EnumFeedback360Relacionamentos.Outro)
                return null;

            var t = (especificacao ?? string.Empty).Trim();
            if (t.Length == 0)
                throw new ArgumentException(@"Quando o relacionamento é ""Outro"", informe a especificação (até 60 caracteres).");
            if (t.Length > 60)
                throw new ArgumentException($@"A especificação do relacionamento ""Outro"" deve ter no máximo 60 caracteres.");

            return t;
        }
    }
}
