using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Helper;
using Core.Domain.Notificacao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.SolicitacaoParticipacao;
using DataTransferObject.Domain.Notificacao;
using DataTransferObject.Domain.Social;
using DataTransferObject.Domain.Usuario;
using Firebase.Domain.Interfaces;
using Firebase.Domain.Interfaces.Services;
using Logs.Infra.Attributes;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapaDeAlocacao.Domain.Impl.GestaoDeAlocados
{
    [LogDomainClass]
    public class EncontrosService : IEncontrosService
    {
        private readonly IUploadFilesClient _uploadFilesClient;
        private readonly IEncontrosRepository _encontrosRepository;
        private readonly string _pastaKey = "arquivos/encontros/";
        private readonly IEnvioEmail _envioEmail;
        private readonly IFirebaseSDK _firebaseService;
        private readonly INotificacaoRepository _notificacaoRepository;
        private readonly INotificacaoService _notificacaoService;
        public EncontrosService(IUploadFilesClient uploadFilesClient, IEncontrosRepository encontrosRepository, IEnvioEmail envioEmail, IFirebaseSDK firebaseService, INotificacaoService notificacaoService)
        {
            _encontrosRepository = encontrosRepository;
            _uploadFilesClient = uploadFilesClient;
            _envioEmail = envioEmail;
            _firebaseService = firebaseService;
            _notificacaoService = notificacaoService;
        }

        public async Task<ApiGenericResult<ArquivoEncontroDto>> DeletarArquivo(int id)
        {
            try
            {
                // Buscar o arquivo pelo ID para obter o path antes de deletar
                var arquivo = await _encontrosRepository.BuscarArquivoPorId(id);
                if (arquivo == null)
                {
                    return new ApiGenericResult<ArquivoEncontroDto>
                    {
                        Sucesso = false,
                        Mensagem = "Arquivo não encontrado.",
                        Retorno = null
                    };
                }

                // Determinar qual path usar (audio ou imagem)
                var arquivoPath = !string.IsNullOrEmpty(arquivo.LinkAudioInteracao) 
                    ? arquivo.LinkAudioInteracao 
                    : arquivo.LinkImagemInteracao;

                // Deletar do storage se houver path
                if (!string.IsNullOrEmpty(arquivoPath))
                {
                    var status = await _uploadFilesClient.DeleteFile(arquivoPath);
                    if (!status.Sucesso)
                    {
                        return new ApiGenericResult<ArquivoEncontroDto>
                        {
                            Sucesso = false,
                            Mensagem = "Erro ao deletar arquivo do storage.",
                            Retorno = null
                        };
                    }
                }

                // Deletar do banco de dados
                var deletado = await _encontrosRepository.DeletarArquivo(id);
                if (!deletado)
                {
                    return new ApiGenericResult<ArquivoEncontroDto>
                    {
                        Sucesso = false,
                        Mensagem = "Erro ao deletar arquivo do banco de dados.",
                        Retorno = null
                    };
                }

                return new ApiGenericResult<ArquivoEncontroDto>
                {
                    Sucesso = true,
                    Mensagem = "Arquivo deletado com sucesso.",
                    Retorno = arquivo
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception($"Não foi possível deletar o arquivo: {e.Message}", e);
            }
        }

        public async Task<ApiGenericResult<ArquivoEncontroDto>> InserirArquivo(ArquivoEncontroParam param)
        {
            var data = DateTime.Now.ToString("yyyyMMddHHmm");
            var nomeBase = $"{data}_{param.NomeArquivo}";
            var objectKey = $"{_pastaKey}{nomeBase}";

            var bytes = param.bytes; // já é byte[]

            try
            {
                await _uploadFilesClient.UploadFile(objectKey, bytes);


                var linkUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(
                    EnvironmentVariables.SERVICE_MIDIA) + objectKey;


                var res = await _encontrosRepository.InserirArquivo(param, linkUrl);

                if (res == null)
                {
                    return new ApiGenericResult<ArquivoEncontroDto>
                    {
                        Sucesso = false,
                        Mensagem = "Não foi possível salvar o arquivo no banco.",
                        Retorno = null
                    };
                }

                return new ApiGenericResult<ArquivoEncontroDto>
                {
                    Erros = null,
                    Sucesso = true,
                    Mensagem = "Arquivo salvo com sucesso",
                    Retorno = res,
                };

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception($"Não foi possível salvar o arquivo: {e.Message}", e);
            }
        }


        public async Task<ApiGenericResult<InteracoesDTO>> criarEncontro(EncontrosDTO param, UsuarioLogadoDTO user)
        {

            var encontro = await _encontrosRepository.criarEncontro(param, user);
            /*
            if (encontro == null)
            {
                return new ApiGenericResult<EncontrosResponse>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao criar encontro.",
                    Retorno = null
                };
            }
            */
            return new ApiGenericResult<InteracoesDTO>
            {
                Sucesso = true,
                Retorno = encontro
            };

        }

        public async Task<ApiGenericResult<InteracoesDTO>> atualizarEncontro(string encontroId, EncontrosParam param)
        {
            var encontro = await _encontrosRepository.atualizarEncontro(encontroId, param);
            /*
            if (encontro == null)
            {
                return new ApiGenericResult<EncontrosResponse>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao criar encontro.",
                    Retorno = null
                };
            }
            */

            return new ApiGenericResult<InteracoesDTO>
            {
                Sucesso = true,
                Retorno = encontro
            };
        }

        public async Task<ApiGenericResult<EncontrosResponse>> buscarEncontroPorId(string encontroId)
        {

            var encontro = await _encontrosRepository.buscarEncontroPorId(encontroId);
            if (encontro == null)
            {
                return new ApiGenericResult<EncontrosResponse>
                {
                    Sucesso = false,
                    Mensagem = "Encontro não encontrado.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<EncontrosResponse>
            {
                Sucesso = true,
                Retorno = encontro
            };
        }

        public async Task<ApiGenericResult<EncontrosResponseDetalhado>> BuscarEncontroPorIdComComentarios(string encontroId)
        {

            var encontro = await _encontrosRepository.BuscarEncontroPorIdComComentarios(encontroId);
            if (encontro == null)
            {
                return new ApiGenericResult<EncontrosResponseDetalhado>
                {
                    Sucesso = false,
                    Mensagem = "Encontro não encontrado.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<EncontrosResponseDetalhado>
            {
                Sucesso = true,
                Retorno = encontro
            };
        }


        public async Task<ApiGenericResult<List<InteracoesCategoriaSubResponseDTO>>> ListarCategoriasSubPorInteracaoId(int interacaoId)
        {
            var encontro = await _encontrosRepository.ListarCategoriasSubPorInteracaoId(interacaoId);

            if (encontro == null || !encontro.Any())
            {
                return new ApiGenericResult<List<InteracoesCategoriaSubResponseDTO>>
                {
                    Sucesso = false,
                    Mensagem = "Categoria por Interação não encontrada.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<List<InteracoesCategoriaSubResponseDTO>>
            {
                Sucesso = true,
                Retorno = (List<InteracoesCategoriaSubResponseDTO>)encontro
            };
        }


        public async Task<ApiGenericResult<bool>> deletarEncontro(string encontroId)
        {
            var deletarEncontro = await _encontrosRepository.deletarEncontro(encontroId);
            if (deletarEncontro == false)
            {
                return new ApiGenericResult<bool>
                {
                    Sucesso = false,
                    Mensagem = "Nenhum Encontro deletado.",
                    Retorno = false
                };
            }

            return new ApiGenericResult<bool>
            {
                Sucesso = true,
                Retorno = deletarEncontro
            };
        }

        public async Task<ApiGenericResult<EncontrosResponse>> buscarEncontroPorColaboradorId(string colaboradorId)
        {
            var buscarEncontroPorColaboradorId = await _encontrosRepository.buscarEncontroPorColaboradorId(colaboradorId);
            if (buscarEncontroPorColaboradorId == null)
            {
                return new ApiGenericResult<EncontrosResponse>
                {
                    Sucesso = false,
                    Mensagem = "Nenhum Encontro encontrado.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<EncontrosResponse>
            {
                Sucesso = true,
                Retorno = buscarEncontroPorColaboradorId
            };
        }

        public async Task<ApiGenericResult<List<EncontrosResponse>>> buscarTodosEncontros()
        {
            var encontros = await _encontrosRepository.buscarTodosEncontros();
            if (encontros == null || encontros.Count == 0)
            {
                return new ApiGenericResult<List<EncontrosResponse>>
                {
                    Sucesso = false,
                    Mensagem = "Nenhum Encontro encontrado.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<List<EncontrosResponse>>
            {
                Sucesso = true,
                Retorno = encontros
            };
        }

        public async Task<ApiGenericResult<AgendaEncontroResult>> carregarAgenda(int id, string cpfUsuarioLogado)
        {

            var agenda = await _encontrosRepository.carregarAgenda(id, cpfUsuarioLogado);
            if (agenda == null)
            {
                return new ApiGenericResult<AgendaEncontroResult>
                {
                    Sucesso = false,
                    Mensagem = "Agenda não encontrada.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<AgendaEncontroResult>
            {
                Sucesso = true,
                Retorno = agenda
            };
        }

        public async Task<ApiGenericResult<AgendaEncontroResult>> atualizarAgenda(int agendaId, AgendaEncontroParam param, UsuarioLogadoDTO user)
        {
            var agenda = await _encontrosRepository.atualizarAgenda(agendaId, param, user);
            if (agenda == null)
            {
                return new ApiGenericResult<AgendaEncontroResult>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao atualizar agenda.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<AgendaEncontroResult>
            {
                Sucesso = true,
                Retorno = agenda
            };
        }

        public async Task<ApiGenericResult<bool>> deletarAgenda(string agendaId)
        {
            var deletarAgenda = await _encontrosRepository.deletarAgenda(agendaId);
            if (deletarAgenda == false)
            {
                return new ApiGenericResult<bool>
                {
                    Sucesso = false,
                    Mensagem = "Nenhuma agenda deletada.",
                    Retorno = false
                };
            }

            return new ApiGenericResult<bool>
            {
                Sucesso = true,
                Retorno = deletarAgenda
            };
        }

        public async Task<ApiGenericResult<AgendaEncontroResult>> criarAgenda(AgendaEncontroParam param, UsuarioLogadoDTO user)
        {

            var agendaEncontro = await _encontrosRepository.criarAgenda(param, user);
            if (agendaEncontro == null)
            {
                return new ApiGenericResult<AgendaEncontroResult>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao criar agenda de encontro.",
                    Retorno = null
                };
            }

            try
            {
                await EnviarNotificacaoPushAppEmLote(param.CodigosColaboradores, "Nova agenda!", "Você foi convidado para uma agenda. Acesse o app para mais informações.");
                
                //Não é usado para FourSys
                //Não deletar esse trecho, pode ser usado p outros clientes
                /*
                var nomeCliente = await _encontrosRepository.BuscarNomeClienteEmailAgenda(param.CodigoCliente);
                await enviarEmailColaboradorEmLote(param.CodigosColaboradores,
                                                     "Convite de agenda",
                                                     $@"
                                                    <p>Você foi convidado para participar da agenda abaixo:</p>

                                                    <p>
                                                        📅 <strong>Título:</strong> {param.Titulo}<br>
                                                        📝 <strong>Descrição:</strong> {param.Descricao}<br>
                                                        🗓 <strong>Data:</strong> {param.DataInicio.Value.ToShortDateString()} - {param.DataFim.Value.ToShortDateString()}<br>
                                                        🕓 <strong>Horário:</strong> {param.DataInicio.Value.TimeOfDay} - {param.DataFim.Value.TimeOfDay}<br>
                                                        🌐 <strong>Link da reunião:</strong> {param.LinkReuniao}<br>
                                                        🏢 <strong>Cliente:</strong> {nomeCliente}
                                                    </p>

                                                    <p>Em caso de dúvidas, entre em contato com o organizador.</p>

                                                    <p>Atenciosamente,<br>Equipe Foursys</p>
                                                 ");
                */

            }
            catch (Exception)
            {
            }

            return new ApiGenericResult<AgendaEncontroResult>
            {
                Sucesso = true,
                Retorno = agendaEncontro
            };
        }

        private async Task enviarEmailColaborador(string codigoInternoColaborador, string assunto, string mensagem)
        {
            var dadosColaborador = await _encontrosRepository.BuscarDadosColaboradorEmail(codigoInternoColaborador);

            if (dadosColaborador == null)
                return;

            _envioEmail.EnviaEmailTemplateFoursys(dadosColaborador.NomeColaborador, mensagem, assunto, dadosColaborador.Email);
        }

        private async Task enviarEmailColaboradorEmLote(List<string> codigosInternoColaboradores, string assunto, string mensagem)
        {
            foreach (var codigoInternoColaborador in codigosInternoColaboradores)
            {
                await enviarEmailColaborador(codigoInternoColaborador, assunto, mensagem);
            }
        }

        private async Task enviarEmailGestorExterno(string codigoGestorExterno, string assunto, string mensagem)
        {
            var dadosColaborador = await _encontrosRepository.BuscarDadosGestorExternoEmail(codigoGestorExterno);

            if (dadosColaborador == null)
                return;

            _envioEmail.EnviaEmailTemplateFoursys(dadosColaborador.Nome, mensagem, assunto, dadosColaborador.Email);
        }

        private async Task enviarEmailGestorExternoEmLote(List<string> codigosGestoresExterno, string assunto, string mensagem)
        {
            foreach (var codigoInternoColaborador in codigosGestoresExterno)
            {
                await enviarEmailGestorExterno(codigoInternoColaborador, assunto, mensagem);
            }
        }

        private async Task EnviarNotificacaoPushAppEmLote(List<string> codigosColaboradores, string titulo, string mensagem)
        {
            var deviceTokens = await _encontrosRepository.buscarDeviceTokensAppEmLote(codigosColaboradores);

            await _firebaseService.EnviaPushEmLote(deviceTokens, titulo, mensagem);
        }

        private async Task EnviarNotificacaoPushApp(string codigoColaborador, string titulo, string mensagem)
        {
            var deviceToken = await _encontrosRepository.buscarDeviceTokenApp(codigoColaborador);

            await _firebaseService.EnviaPush(deviceToken, titulo, mensagem);
        }

        public async Task<ApiGenericResult<List<AgendaEncontroResult>>> buscarAgendaEncontros(string cpfUsuarioLogado, string dataInicio, string dataFim)
        {
            var agendaEncontros = await _encontrosRepository.buscarAgendaEncontros(cpfUsuarioLogado, dataInicio, dataFim);
            if (agendaEncontros == null)
            {
                return new ApiGenericResult<List<AgendaEncontroResult>>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao retornar agendas.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<List<AgendaEncontroResult>>
            {
                Sucesso = true,
                Retorno = agendaEncontros
            };
        }

        public async Task<ApiGenericResult<List<AgendaEncontroResult>>> buscarAgendaEncontrosPorColaboradorId(string colaboradorId)
        {
            var buscarAgendaEncontrosPorColaboradorId = await _encontrosRepository.buscarAgendaEncontrosPorColaboradorId(colaboradorId);
            if (buscarAgendaEncontrosPorColaboradorId == null)
            {
                return new ApiGenericResult<List<AgendaEncontroResult>>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao criar agenda de encontro.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<List<AgendaEncontroResult>>
            {
                Sucesso = true,
                Retorno = buscarAgendaEncontrosPorColaboradorId
            };
        }

        public async Task<ApiGenericResult<List<AgendaEncontroResult>>> buscarAgendaEncontrosPorData(string data)
        {
            var buscarAgendaEncontrosPorData = await _encontrosRepository.buscarAgendaEncontrosPorData(data);
            if (buscarAgendaEncontrosPorData == null)
            {
                return new ApiGenericResult<List<AgendaEncontroResult>>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao criar agenda de encontro.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<List<AgendaEncontroResult>>
            {
                Sucesso = true,
                Retorno = buscarAgendaEncontrosPorData
            };
        }

        public async Task<ApiGenericResult<List<EncontrosResponse>>> buscarEncontrosPorAgendaId(string agendaId)
        {

            var buscarEncontrosPorAgendaId = await _encontrosRepository.buscarEncontrosPorAgendaId(agendaId);
            if (buscarEncontrosPorAgendaId == null || buscarEncontrosPorAgendaId.Count == 0)
            {
                return new ApiGenericResult<List<EncontrosResponse>>
                {
                    Sucesso = false,
                    Mensagem = "Nenhum encontro encontrado para esta agenda.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<List<EncontrosResponse>>
            {
                Sucesso = true,
                Retorno = buscarEncontrosPorAgendaId
            };
        }

        public async Task<ApiGenericResult<EncontroAi>> CriarEncontroAi(EncontroAi ai)
        {

            var CriarEncontroAi = await _encontrosRepository.CriarEncontroAi(ai);
            if (CriarEncontroAi == null || CriarEncontroAi.EncontroId == 0)
            {
                return new ApiGenericResult<EncontroAi>
                {
                    Sucesso = false,
                    Mensagem = "Nenhum encontro encontrado para esta agenda.",
                    Retorno = null
                };
            }
            try
            {
                if (CriarEncontroAi.Passos != null && CriarEncontroAi.Passos.Count > 0)
                {
                    foreach (var passo in CriarEncontroAi.Passos)
                    {
                        var codigoColaborador = passo.CodigoColaborador;
                        var nomeColaborador = passo.NomeColaborador;

                        // Enviar notificação Push
                        await EnviarNotificacaoPushApp(
                            codigoColaborador,
                            "Notificação ações da Agenda",
                            passo.Texto + " - Até data limite: " + passo.DataLimite?.ToString("dd/MM/yyyy")
                        );

                        // Registrar notificação no Fourmakers                    
                        var notificacao = new NotificacaoDTO
                        {
                            ColaboradorCpf = codigoColaborador,
                            Titulo = "Notificação ações da agenda",
                            Mensagem = passo.Texto + " - Até data limite: " + passo.DataLimite?.ToString("dd/MM/yyyy"),
                            MensagemHtml = null,
                            Lida = false,
                            DataEnvio = DateTime.Now,
                            DataLeitura = null,
                            TbFuncionalidadeSistemaId = (int)FuncionalidadeSistemaEnum.NOTIFICACOES_AGENDA_ACOES,
                            OrgId = 2,
                            Rota = null,
                            RotaCompleta = null
                        };

                        await _notificacaoService.InserirNotificacaoColaborador(notificacao);

                    }
                }

            }
            catch (Exception)
            {
            }

            return new ApiGenericResult<EncontroAi>
            {
                Sucesso = true,
                Retorno = CriarEncontroAi
            };
        }

        public async Task<ApiGenericResult<EncontroAi>> AtualizarEncontroAi(EncontroAi ai)
        {

            var AtualizarEncontroAi = await _encontrosRepository.AtualizarEncontroAi(ai);
            if (AtualizarEncontroAi == null || AtualizarEncontroAi.EncontroId == null)
            {
                return new ApiGenericResult<EncontroAi>
                {
                    Sucesso = false,
                    Mensagem = "Não foi possível atualizar o encontro.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<EncontroAi>
            {
                Sucesso = true,
                Retorno = AtualizarEncontroAi
            };
        }

        public async Task<ApiGenericResult<bool>> DeletarEncontroAi(int id)
        {

            var DeletarEncontroAi = await _encontrosRepository.DeletarEncontroAi(id);
            if (DeletarEncontroAi == null || DeletarEncontroAi == false)
            {
                return new ApiGenericResult<bool>
                {
                    Sucesso = false,
                    Mensagem = "Não foi possível deletar o encontro.",
                    Retorno = false
                };
            }

            return new ApiGenericResult<bool>
            {
                Sucesso = true,
                Retorno = DeletarEncontroAi
            };
        }

        public async Task<ApiGenericResult<EncontroAi>> BuscarEncontroAiPorEncontroId(int encontroId)
        {

            var BuscarEncontroAiPorEncontroId = await _encontrosRepository.BuscarEncontroAiPorEncontroId(encontroId);
            if (BuscarEncontroAiPorEncontroId == null)
            {
                return new ApiGenericResult<EncontroAi>
                {
                    Sucesso = false,
                    Mensagem = "Nenhum encontro encontrado para esta agenda.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<EncontroAi>
            {
                Sucesso = true,
                Retorno = BuscarEncontroAiPorEncontroId
            };
        }

        public async Task<ApiGenericResult<List<SolicitacaoResponse>>> BuscarSolicitacoesAgendas(string cpfUsuarioLogado, int cursor, int limite)
        {

            var solicitacoesAgenda = await _encontrosRepository.BuscarSolicitacoesAgendas(cpfUsuarioLogado, cursor, limite);

            if (solicitacoesAgenda == null)
            {
                return new ApiGenericResult<List<SolicitacaoResponse>>
                {
                    Sucesso = false,
                    Mensagem = "Nenhuma solicitacao encontrada.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<List<SolicitacaoResponse>>
            {
                Sucesso = true,
                Retorno = solicitacoesAgenda
            };
        }

        public async Task<ApiGenericResult<List<SolicitacaoResponse>>> BuscarSolicitacoesMinhasAgendas(string cpfUsuarioLogado, int cursor, int limite)
        {

            var solicitacoesAgenda = await _encontrosRepository.BuscarSolicitacoesMinhasAgendas(cpfUsuarioLogado, cursor, limite);

            if (solicitacoesAgenda == null)
            {
                return new ApiGenericResult<List<SolicitacaoResponse>>
                {
                    Sucesso = false,
                    Mensagem = "Nenhuma solicitacao encontrada.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<List<SolicitacaoResponse>>
            {
                Sucesso = true,
                Retorno = solicitacoesAgenda
            };
        }

        public async Task<ApiGenericResult<List<SolicitacaoResponse>>> BuscarSolicitacoesPorAgenda(int agendaId, int cursor, int limite)
        {

            var solicitacoesAgenda = await _encontrosRepository.BuscarSolicitacoesPorAgenda(agendaId, cursor, limite);

            if (solicitacoesAgenda == null)
            {
                return new ApiGenericResult<List<SolicitacaoResponse>>
                {
                    Sucesso = false,
                    Mensagem = "Nenhuma solicitacao encontrada.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<List<SolicitacaoResponse>>
            {
                Sucesso = true,
                Retorno = solicitacoesAgenda
            };
        }

        public async Task<ApiGenericResult<bool>> AprovarReprovarSolicitacaoAgenda(StatusSolicitacaoParticipante decisaoStatus, int agendaId, string codigoColaboradorSolicitante, string cpfUsuarioLogado)
        {

            var solicitacoesAgenda = await _encontrosRepository.AprovarReprovarSolicitacaoAgenda(decisaoStatus, agendaId, codigoColaboradorSolicitante, cpfUsuarioLogado);

            if (!solicitacoesAgenda)
            {
                return new ApiGenericResult<bool>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao registrar status da solicitacao.",
                    Retorno = false
                };
            }

            if (decisaoStatus == StatusSolicitacaoParticipante.Aprovado)
                await _encontrosRepository.ConvidarColaboradorInternoAgenda(codigoColaboradorSolicitante, agendaId);


            var dadosAgenda = await _encontrosRepository.BuscarAgendaPorId(agendaId);
            var nomeOrganizador = await _encontrosRepository.BuscarNomeColaborador(cpfUsuarioLogado);
            var nomeCliente = await _encontrosRepository.BuscarNomeClienteEmailAgenda(dadosAgenda.CodigoCliente);

            await EnviarNotificacaoPushApp(
                    codigoColaboradorSolicitante,
                    decisaoStatus == StatusSolicitacaoParticipante.Aprovado
                        ? "Sua participação foi aprovada!"
                        : "Sua participação foi recusada!",
                    decisaoStatus == StatusSolicitacaoParticipante.Aprovado
                        ? $"Sua solicitação para participar da agenda de {nomeOrganizador} foi aceita."
                        : $"Sua solicitação para participar da agenda de {nomeOrganizador} foi recusada.");

            var statusTexto = decisaoStatus == StatusSolicitacaoParticipante.Aprovado
                                                ? "aprovou sua participação"
                                                : "recusou sua participação";

            var statusAssunto = decisaoStatus == StatusSolicitacaoParticipante.Aprovado
                                                  ? "Participação de agenda aprovada!"
                                                  : "Participação de agenda recusada!";

            await enviarEmailColaborador(codigoColaboradorSolicitante,
                                                $"{statusAssunto}",
                                                $@"
                                                    <p>{nomeOrganizador} {statusTexto} na agenda abaixo:</p>

                                                    <p>
                                                        📅 <strong>Título:</strong> {dadosAgenda.Titulo}<br>
                                                        📝 <strong>Descrição:</strong> {dadosAgenda.Descricao}<br>
                                                        🗓 <strong>Data:</strong> {dadosAgenda.DataInicio.Value.ToShortDateString()} - {dadosAgenda.DataFim.Value.ToShortDateString()}<br>
                                                        🕓 <strong>Horário:</strong> {dadosAgenda.DataInicio.Value:HH\\:mm} - {dadosAgenda.DataFim.Value:HH\\:mm}<br>
                                                        🌐 <strong>Link da reunião:</strong> {dadosAgenda.LinkReuniao}<br>
                                                        🏢 <strong>Cliente:</strong> {nomeCliente}
                                                    </p>

                                                    <p>Acesse o APP Fourmakers para mais informações.</p>

                                                    <p>Atenciosamente,<br>Equipe Foursys</p>
                                                ");

            return new ApiGenericResult<bool>
            {
                Sucesso = true,
                Retorno = solicitacoesAgenda,
                Mensagem = "Solicitacao atualizado com sucesso."
            };
        }

        public async Task<ApiGenericResult<bool>> AceitarRecusarConviteAgenda(StatusSolicitacaoParticipante decisaoStatus, int agendaId, string codigoColaborador)
        {
            if (decisaoStatus is not (StatusSolicitacaoParticipante.Aprovado or StatusSolicitacaoParticipante.Recusado))
            {
                return new ApiGenericResult<bool>
                {
                    Sucesso = false,
                    Mensagem = "Status deve ser 1-Aceito ou 2-Recusado.",
                    Retorno = false
                };
            }

            var solicitacoesAgenda = await _encontrosRepository.AceitarRecusarConviteAgenda(decisaoStatus, agendaId, codigoColaborador);

            if (!solicitacoesAgenda)
            {
                return new ApiGenericResult<bool>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao atualizar convite da agenda.",
                    Retorno = false
                };
            }

            return new ApiGenericResult<bool>
            {
                Sucesso = true,
                Retorno = solicitacoesAgenda,
                Mensagem = "Convite atualizado com sucesso."
            };
        }

        public async Task<ApiGenericResult<bool>> SolicitarParticipacaoAgenda(int agendaId, string codigoColaboradorSolicitante, string codigoColaboradorCriador)
        {

            var solicitacoesAgenda = await _encontrosRepository.SolicitarParticipacaoAgenda(agendaId, codigoColaboradorSolicitante, codigoColaboradorCriador);

            if (!solicitacoesAgenda)
            {
                return new ApiGenericResult<bool>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao solicitar participacao. O usuário já solicitou participação nesta agenda.",
                    Retorno = false
                };
            }

            var nomeColaboardor = await _encontrosRepository.BuscarNomeColaborador(codigoColaboradorSolicitante);

            await EnviarNotificacaoPushApp(codigoColaboradorCriador, "Alguém quer participar da sua agenda", $"{nomeColaboardor} pediu para participar da sua agenda. Acesse o app para liberar.");

            var dadosAgenda = await _encontrosRepository.BuscarAgendaPorId(agendaId);
            var nomeCliente = await _encontrosRepository.BuscarNomeClienteEmailAgenda(dadosAgenda.CodigoCliente);

            await enviarEmailColaborador(codigoColaboradorCriador,
                                                 "Solicitação de participação em agenda",
                                                 $@"
                                                    <p>{nomeColaboardor} pediu para participar da agenda abaixo:</p>

                                                    <p>
                                                        📅 <strong>Título:</strong> {dadosAgenda.Titulo}<br>
                                                        📝 <strong>Descrição:</strong> {dadosAgenda.Descricao}<br>
                                                        🗓 <strong>Data:</strong> {dadosAgenda.DataInicio.Value.ToShortDateString()} - {dadosAgenda.DataFim.Value.ToShortDateString()}<br>
                                                        🕓 <strong>Horário:</strong> {dadosAgenda.DataInicio.Value.TimeOfDay} - {dadosAgenda.DataFim.Value.TimeOfDay}<br>
                                                        🌐 <strong>Link da reunião:</strong> {dadosAgenda.LinkReuniao}<br>
                                                        🏢 <strong>Cliente:</strong> {nomeCliente}
                                                    </p>

                                                    <p>Acesse o APP Fourmakers para aprovar ou reprovar a solicitação.</p>

                                                    <p>Atenciosamente,<br>Equipe Foursys</p>
                                                 ");

            return new ApiGenericResult<bool>
            {
                Sucesso = true,
                Retorno = solicitacoesAgenda,
                Mensagem = "Solicitacao enviada com sucesso."
            };
        }

        public async Task<ApiGenericResult<ConvidarParticipantesAgendaParam>> ConvidarParticipanteAgenda(ConvidarParticipantesAgendaParam param)
        {
            var dadosAgenda = await _encontrosRepository.BuscarAgendaPorId(param.AgendaId);
            
            //var nomeCliente = await _encontrosRepository.BuscarNomeClienteEmailAgenda(dadosAgenda.CodigoCliente);

            foreach (var codigoParticipante in param.CodigosGestoresExternos)
            {
                await _encontrosRepository.ConvidarGestorExternoAgenda(codigoParticipante, param.AgendaId);

                var dadosGestor = await _encontrosRepository.BuscarDadosGestorExternoEmail(codigoParticipante);
                
                await enviarEmailColaborador(codigoParticipante,
                                                 "Convite para agenda.",
                                                 $@"
                                                    <p>{dadosAgenda.NomeCompletoColaboradorCriador} pediu para você participar da agenda abaixo:</p>

                                                    <p>
                                                        📅 <strong>Título:</strong> {dadosAgenda.Titulo}<br>
                                                        📝 <strong>Descrição:</strong> {dadosAgenda.Descricao}<br>
                                                        🗓 <strong>Data:</strong> {dadosAgenda.DataInicio.Value.ToShortDateString()} - {dadosAgenda.DataFim.Value.ToShortDateString()}<br>
                                                        🕓 <strong>Horário:</strong> {dadosAgenda.DataInicio.Value.TimeOfDay} - {dadosAgenda.DataFim.Value.TimeOfDay}<br>
                                                        🌐 <strong>Link da reunião:</strong> {dadosAgenda.LinkReuniao}<br>
                                                        🏢 <strong>Cliente:</strong> {dadosAgenda.Cliente?.NomeCliente ?? "—"}
                                                    </p>

                                                    <p>Mensagem do organizador: {param.Mensagem}</p>

                                                    <p>Acesse o APP Fourmakers para aprovar ou reprovar a solicitação.</p>

                                                    <p>Atenciosamente,<br>Equipe Foursys</p>
                                                 ");
                
            }

            foreach (var codigoParticipante in param.CodigosInternoColaboradores)
            {
                
                await _encontrosRepository.ConvidarColaboradorInternoAgenda(codigoParticipante, param.AgendaId);

                var nomeColaboardor = await _encontrosRepository.BuscarNomeColaborador(codigoParticipante);
                await EnviarNotificacaoPushApp(codigoParticipante, "Alguém quer participar da sua agenda", $"{nomeColaboardor} pediu para participar da sua agenda. Acesse o app para liberar.");

                await enviarEmailColaborador(codigoParticipante,
                                                 "Solicitação de participação em agenda",
                                                 $@"
                                                    <p>{dadosAgenda.NomeCompletoColaboradorCriador} pediu para você participar da agenda abaixo:</p>

                                                    <p>
                                                        📅 <strong>Título:</strong> {dadosAgenda.Titulo}<br>
                                                        📝 <strong>Descrição:</strong> {dadosAgenda.Descricao}<br>
                                                        🗓 <strong>Data:</strong> {dadosAgenda.DataInicio.Value.ToShortDateString()} - {dadosAgenda.DataFim.Value.ToShortDateString()}<br>
                                                        🕓 <strong>Horário:</strong> {dadosAgenda.DataInicio.Value.TimeOfDay} - {dadosAgenda.DataFim.Value.TimeOfDay}<br>
                                                        🌐 <strong>Link da reunião:</strong> {dadosAgenda.LinkReuniao}<br>
                                                        🏢 <strong>Cliente:</strong> {dadosAgenda.Cliente?.NomeCliente ?? "—"}
                                                    </p>

                                                    <p>Mensagem do organizador: {param.Mensagem}</p>

                                                    <p>Acesse o APP Fourmakers para aprovar ou reprovar a solicitação.</p>

                                                    <p>Atenciosamente,<br>Equipe Foursys</p>
                                                 ");
                  
            }



            return new ApiGenericResult<ConvidarParticipantesAgendaParam>
            {
                Sucesso = true,
                Retorno = param,
                Mensagem = "Convite enviada com sucesso."
            };
        }

        public async Task<ApiGenericResult<List<AgendaEncontroResult>>> BuscarAgendasFilhosCompletas(string agendaId, string cpfUsuarioLogado)
        {
            var agendasFilhos = await _encontrosRepository.BuscarAgendasFilhosCompletas(agendaId, cpfUsuarioLogado);
            if (agendasFilhos == null)
            {
                return new ApiGenericResult<List<AgendaEncontroResult>>
                {
                    Sucesso = false,
                    Mensagem = "Nenhuma agenda filha encontrada.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<List<AgendaEncontroResult>>
            {
                Sucesso = true,
                Retorno = agendasFilhos
            };
        }

        public async Task<ApiGenericResult<List<int>>> BuscarIdAgendasFilhos(string agendaId)
        {
            var agendasFilhos = await _encontrosRepository.BuscarIdAgendasFilhos(agendaId);

            if (agendasFilhos == null)
            {
                return new ApiGenericResult<List<int>>
                {
                    Sucesso = false,
                    Mensagem = "Nenhuma agenda filha encontrada.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<List<int>>
            {
                Sucesso = true,
                Retorno = agendasFilhos
            };
        }

        public async Task<ApiGenericResult<List<AgendaObjetivoDTO>>> ListarTodosObjetivos()
        {
            var objetivos = await _encontrosRepository.ListarTodosObjetivos();

            if (objetivos == null || !objetivos.Any())
            {
                return new ApiGenericResult<List<AgendaObjetivoDTO>>
                {
                    Sucesso = false,
                    Mensagem = "Nenhum objetivo encontrado.",
                    Retorno = new List<AgendaObjetivoDTO>()
                };
            }

            return new ApiGenericResult<List<AgendaObjetivoDTO>>
            {
                Sucesso = true,
                Mensagem = "Objetivos obtidos com sucesso.",
                Retorno = objetivos
            };
        }

    }
};
