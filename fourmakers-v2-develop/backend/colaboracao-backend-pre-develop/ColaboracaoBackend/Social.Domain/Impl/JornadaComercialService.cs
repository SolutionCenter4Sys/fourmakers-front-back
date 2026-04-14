using System.Collections.Generic;
using System.Linq;
using Core.Domain;
using Core.Domain.Social;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Social;
using DataTransferObject.Domain.Usuario;
using MapaDeAlocacao.Domain.Impl.GestaoDeAlocados;
using Social.Domain.Interfaces;

using Logs.Infra.Attributes;

namespace Social.Domain.Impl
{
    [LogDomainClass]
    public class JornadaComercialAppService : IJornadaComercialAppService
    {
        private readonly IJornadaComercialAppRepository _jornadaComercialAppRepository;

        public JornadaComercialAppService(IJornadaComercialAppRepository jornadaComercialAppRepository)
        {
            _jornadaComercialAppRepository = jornadaComercialAppRepository;
        }

        public async Task<KanbanEncontrosAcoesComerciais> BuscarKanbanEncontrosAcoesComerciais(string busca, DateTime? dataInicio, DateTime? dataFim, int orgIdUsuarioLogado)
        {
            var dataInicio1 = dataInicio;
            var dataFim1 = dataFim;

            if (dataInicio != null)
                dataInicio1 = new DateTime(dataInicio.Value.Year, dataInicio.Value.Month, dataInicio.Value.Day);

            if (dataFim != null)
                dataFim1 = new DateTime(dataFim.Value.Year, dataFim.Value.Month, dataFim.Value.Day);

            return await _jornadaComercialAppRepository.BuscarKanbanEncontrosAcoesComerciais(busca, dataInicio1, dataFim1, orgIdUsuarioLogado);
        }

        public async Task<EncontroAiPassos> AtualizarStatusAcoes(AtualizarIteracoesAcoesParam p)
        {
            return await _jornadaComercialAppRepository.AtualizarStatusAcoes(p);
        }

        public async Task<InteracaoAcoesResponseDTO> ListarInteracaoAcao(int interacaoAcaoId)
        {
            return await _jornadaComercialAppRepository.ListarInteracaoAcao(interacaoAcaoId);
        }

        public async Task<ApiGenericResult<ComentariosAcoesResponseDTO>> InsercaoComentariosAcoes(ComentariosAcoesParamDTO param, UsuarioLogadoDTO user)
        {
            var comentario = await _jornadaComercialAppRepository.InsercaoComentariosAcoes(param, user);
           
            if (comentario == null)
            {
                return new ApiGenericResult<ComentariosAcoesResponseDTO>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao inserir coment�rio.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<ComentariosAcoesResponseDTO>
            {
                Sucesso = true,
                Retorno = comentario
            };
        }
        public async Task<ApiGenericResult<ComentariosAcoesResponseDTO>> AtualizarComentario(ComentariosAcoesResponseDTO param, UsuarioLogadoDTO user)
        {
            var result = await _jornadaComercialAppRepository.AtualizarComentario(param, user);

            if (result == null)
            {
                return new ApiGenericResult<ComentariosAcoesResponseDTO>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao atualizar coment�rio.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<ComentariosAcoesResponseDTO>
            {
                Sucesso = true,
                Retorno = result
            };
        }

        public async Task<ApiGenericResult<bool>> DeletarComentario(long comentarioId)
        {
            var sucesso = await _jornadaComercialAppRepository.DeletarComentario(comentarioId);

            return new ApiGenericResult<bool>
            {
                Sucesso = sucesso,
                Mensagem = sucesso ? "Coment�rio removido com sucesso!" : "Erro ao excluir coment�rio.",
                Retorno = sucesso
            };
        }
        public async Task<ApiGenericResult<InteracaoCategoriaSubResponseDTO>> InsercaoInteracaoCategoria(CategoriaInsercaoParamDTO param)
        {
            var result = await _jornadaComercialAppRepository.InsercaoInteracaoCategoria(param);

            if (result == null)
            {
                return new ApiGenericResult<InteracaoCategoriaSubResponseDTO>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao inserir Intera��o Categoria/Sub .",
                    Retorno = null
                };
            }

            return new ApiGenericResult<InteracaoCategoriaSubResponseDTO>
            {
                Sucesso = true,
                Retorno = result
            };
        }
        public async Task<ApiGenericResult<InteracaoCategoriaSubResponseDTO>> AtualizarInteracaoCategoria(CategoriaAtualizacaoParamDTO param)
        {
            var result = await _jornadaComercialAppRepository.AtualizarInteracaoCategoria(param);

            if (result == null)
            {
                return new ApiGenericResult<InteracaoCategoriaSubResponseDTO>
                {
                    Sucesso = false,
                    Mensagem = "Atualiza��o Intera��o Categoria/Sub efetuado com sucesso.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<InteracaoCategoriaSubResponseDTO>
            {
                Sucesso = true,
                Retorno = result
            };
        }
      
        public async Task<ApiGenericResult<bool>> DeletarInteracaoCategoria(long interacaoCategoriaId)
        {
            var sucesso = await _jornadaComercialAppRepository.DeletarInteracaoCategoria(interacaoCategoriaId);

            return new ApiGenericResult<bool>
            {
                Sucesso = sucesso,
                Mensagem = sucesso ? "Intera��o Categoria/Sub removido com sucesso!" : "Erro ao excluir coment�rio.",
                Retorno = sucesso
            };
        }

        public async Task<ApiGenericResult<List<ComentariosAcoesResponseDTO>>> ListarComentariosPorInteracao(int interacaoAcoesId)
        {
            var lista = await _jornadaComercialAppRepository.ListarComentariosPorInteracao(interacaoAcoesId);

            return new ApiGenericResult<List<ComentariosAcoesResponseDTO>>
            {
                Sucesso = true,
                Retorno = lista
            };
        }
        public async Task<InteracaiAIResponse> InserirInteracaoIA(EncontroAiParamInclusao p)
        {
            return await _jornadaComercialAppRepository.InserirInteracaoIA(p);
        }
        public async Task<InteracaiAIResponse> AtualizarInteracaoIA(EncontroAiParamAtualizacao p)
        {
            return await _jornadaComercialAppRepository.AtualizarInteracaoIA(p);
        }
        public async Task<bool> DeletarInteracaoIA(int interacaoIA)
        {
            return await _jornadaComercialAppRepository.DeletarInteracaoIA(interacaoIA);
        }

        public async Task<IEnumerable<CategoriaAssuntoDTO>> ListarCategorias()
            => await _jornadaComercialAppRepository.ListarCategorias();

        public async Task<IEnumerable<SubcategoriaAssuntoDTO>> ListarSubcategoriasPorCategoria(int categoriaId)
            => await _jornadaComercialAppRepository.ListarSubcategoriasPorCategoria(categoriaId);

        public async Task<IEnumerable<CategoriaAssuntoComSubDTO>> ListarCategoriasComSub()
            => await _jornadaComercialAppRepository.ListarCategoriasComSub();

       public async Task<ApiGenericResult<PaginadoDTO<AgendaHierarquicaDTO>>> FiltroAgendasInteracoesCategoriaSub(
                    string nomeCliente, DateTime? dataAgendadaInicio, DateTime? dataAgendadaFim,
                    int? categoriaId, int? subCategoriaId, int pagina, int limite)
        {
            var (data, total) = await _jornadaComercialAppRepository.FiltroAgendasInteracoesCategoriaSub(
                nomeCliente, dataAgendadaInicio, dataAgendadaFim, categoriaId, subCategoriaId, pagina, limite);

            return new ApiGenericResult<PaginadoDTO<AgendaHierarquicaDTO>>
            {
                Sucesso = true,
                Retorno = new PaginadoDTO<AgendaHierarquicaDTO>
                {
                    Pagina = pagina,
                    Limite = limite,
                    TotalRegistros = total,
                    Dados = data
                }
            };
        }

        public async Task<ApiGenericResult<IntegracaoMoxeResponseDTO>> BuscarProximosPassosIntegracaoMoxe(IntegracaoMoxeRequestDTO request)
        {
            var resultado = await _jornadaComercialAppRepository.BuscarProximosPassosIntegracaoMoxe(request);

            return new ApiGenericResult<IntegracaoMoxeResponseDTO>
            {
                Sucesso = resultado.Sucesso,
                Mensagem = resultado.Mensagem,
                Retorno = resultado
            };
        }

        public async Task<ApiGenericResult<AgendaSolicitanteDTO>> SolicitarParticiparAgenda(int agendaId, string codigoColaboradorSolicitante, string codigoColaboradorCriador)
        {
            if (agendaId <= 0 || string.IsNullOrWhiteSpace(codigoColaboradorSolicitante) || string.IsNullOrWhiteSpace(codigoColaboradorCriador))
            {
                return new ApiGenericResult<AgendaSolicitanteDTO>
                {
                    Sucesso = false,
                    Mensagem = "Parâmetros da requisição inválido.",
                    Retorno = null
                };
            }

            try
            {
                var retorno = await _jornadaComercialAppRepository.SolicitarParticiparAgenda(
                    agendaId,
                    codigoColaboradorSolicitante,
                    codigoColaboradorCriador);

                return new ApiGenericResult<AgendaSolicitanteDTO>
                {
                    Sucesso = true,
                    Mensagem = "Solicitação registrada com sucesso.",
                    Retorno = retorno
                };
            }
            catch (Exception ex)
            {
                return new ApiGenericResult<AgendaSolicitanteDTO>
                {
                    Sucesso = false,
                    Mensagem = ex.Message,
                    Retorno = null
                };
            }
        }

        public async Task<ApiGenericResult<AgendaSolicitanteDTO>> ObterAgendaSolicitantePorId(int id)
        {
            try
            {
                var retorno = await _jornadaComercialAppRepository.ObterAgendaSolicitantePorId(id);
                return new ApiGenericResult<AgendaSolicitanteDTO>
                {
                    Sucesso = true,
                    Retorno = retorno
                };
            }
            catch (Exception ex)
            {
                return new ApiGenericResult<AgendaSolicitanteDTO>
                {
                    Sucesso = false,
                    Mensagem = ex.Message,
                    Retorno = null
                };
            }
        }

        public async Task<ApiGenericResult<List<AgendaSolicitanteDTO>>> ListarAgendaSolicitantesPorAgendaComercial(int tbAgendasComerciaisId)
        {
            try
            {
                var lista = await _jornadaComercialAppRepository.ListarAgendaSolicitantesPorAgendaComercial(tbAgendasComerciaisId);
                return new ApiGenericResult<List<AgendaSolicitanteDTO>>
                {
                    Sucesso = true,
                    Retorno = lista.ToList()
                };
            }
            catch (Exception ex)
            {
                return new ApiGenericResult<List<AgendaSolicitanteDTO>>
                {
                    Sucesso = false,
                    Mensagem = ex.Message,
                    Retorno = null
                };
            }
        }

        public async Task<ApiGenericResult<AgendaSolicitanteDTO>> AceitarRecusarSolicitanteNaAgenda(AgendaSolicitanteAtualizacaoDTO param)
        {
            if (param == null || param.TbAgendasComerciaisId <= 0 || string.IsNullOrWhiteSpace(param.CodigoColaboradorExterno))
            {
                return new ApiGenericResult<AgendaSolicitanteDTO>
                {
                    Sucesso = false,
                    Mensagem = "Parâmetros da requisição inválido.",
                    Retorno = null
                };
            }

            if (param.TbStatusAppId != 1 && param.TbStatusAppId != 2)
            {
                return new ApiGenericResult<AgendaSolicitanteDTO>
                {
                    Sucesso = false,
                    Mensagem = "Status deve ser 1-Aceito ou 2-Recusado.",
                    Retorno = null
                };
            }

            try
            {
                var retorno = await _jornadaComercialAppRepository.AceitarRecusarSolicitanteNaAgenda(param);
                return new ApiGenericResult<AgendaSolicitanteDTO>
                {
                    Sucesso = true,
                    Mensagem = "Solicitação Aceito/Recusado.",
                    Retorno = retorno
                };
            }
            catch (Exception ex)
            {
                return new ApiGenericResult<AgendaSolicitanteDTO>
                {
                    Sucesso = false,
                    Mensagem = ex.Message,
                    Retorno = null
                };
            }
        }

        public async Task<ApiGenericResult<bool>> ExcluirAgendaSolicitante(int tbAgendasComerciaisId, string codigoColaboradorExterno)
        {
            if (tbAgendasComerciaisId <= 0 || string.IsNullOrWhiteSpace(codigoColaboradorExterno))
            {
                return new ApiGenericResult<bool>
                {
                    Sucesso = false,
                    Mensagem = "Parâmetros da requisição inválido.",
                    Retorno = false
                };
            }

            try
            {
                var ok = await _jornadaComercialAppRepository.ExcluirAgendaSolicitante(tbAgendasComerciaisId, codigoColaboradorExterno);
                return new ApiGenericResult<bool>
                {
                    Sucesso = ok,
                    Mensagem = ok ? "Solicitação removido com sucesso." : "Solicitação não encontrado!",
                    Retorno = ok
                };
            }
            catch (Exception ex)
            {
                return new ApiGenericResult<bool>
                {
                    Sucesso = false,
                    Mensagem = ex.Message,
                    Retorno = false
                };
            }
        }

        public async Task<ApiGenericResult<List<AgendaConvidadoDTO>>> ConvidarParaAgenda(ConvidarParaAgendaRequestDTO param)
        {
            if (param == null)
            {
                return new ApiGenericResult<List<AgendaConvidadoDTO>>
                {
                    Sucesso = false,
                    Mensagem = "Parâmetro da requisição inválido.",
                    Retorno = null
                };
            }

            if (string.IsNullOrWhiteSpace(param.CodigoColaboradorInternoExterno))
            {
                return new ApiGenericResult<List<AgendaConvidadoDTO>>
                {
                    Sucesso = false,
                    Mensagem = "Informe ao menos um código de colaborador ou gestor (separados por vírgula).",
                    Retorno = null
                };
            }

            var codigos = param.CodigoColaboradorInternoExterno
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToList();

            if (codigos.Count == 0)
            {
                return new ApiGenericResult<List<AgendaConvidadoDTO>>
                {
                    Sucesso = false,
                    Mensagem = "Nenhum código válido após separar por vírgula.",
                    Retorno = null
                };
            }

            try
            {
                var retorno = await _jornadaComercialAppRepository.ConvidarParaAgenda(
                    param.TbAgendasComerciaisId,
                    codigos);

                var msg = retorno.Count == 1
                    ? "Convite registrado com sucesso."
                    : $"{retorno.Count} convites registrados com sucesso.";

                return new ApiGenericResult<List<AgendaConvidadoDTO>>
                {
                    Sucesso = true,
                    Mensagem = msg,
                    Retorno = retorno.ToList()
                };
            }
            catch (Exception ex)
            {
                return new ApiGenericResult<List<AgendaConvidadoDTO>>
                {
                    Sucesso = false,
                    Mensagem = ex.Message,
                    Retorno = null
                };
            }
        }

        public async Task<ApiGenericResult<AgendaConvidadoDTO>> ObterAgendaConvidadoPorId(int id)
        {
            try
            {
                var retorno = await _jornadaComercialAppRepository.ObterAgendaConvidadoPorId(id);
                return new ApiGenericResult<AgendaConvidadoDTO>
                {
                    Sucesso = true,
                    Retorno = retorno
                };
            }
            catch (Exception ex)
            {
                return new ApiGenericResult<AgendaConvidadoDTO>
                {
                    Sucesso = false,
                    Mensagem = ex.Message,
                    Retorno = null
                };
            }
        }

        public async Task<ApiGenericResult<List<AgendaConvidadoDTO>>> ListarAgendaConvidadosPorAgendaComercial(int tbAgendasComerciaisId)
        {
            try
            {
                var lista = await _jornadaComercialAppRepository.ListarAgendaConvidadosPorAgendaComercial(tbAgendasComerciaisId);
                return new ApiGenericResult<List<AgendaConvidadoDTO>>
                {
                    Sucesso = true,
                    Retorno = lista.ToList()
                };
            }
            catch (Exception ex)
            {
                return new ApiGenericResult<List<AgendaConvidadoDTO>>
                {
                    Sucesso = false,
                    Mensagem = ex.Message,
                    Retorno = null
                };
            }
        }

        public async Task<ApiGenericResult<AgendaConvidadoDTO>> AceitarRecusarConvidadoNaAgenda(AgendaConvidadoAtualizacaoDTO param)
        {
            if (param == null || param.TbAgendasComerciaisId <= 0 || string.IsNullOrWhiteSpace(param.CodigoColaboradorExterno))
            {
                return new ApiGenericResult<AgendaConvidadoDTO>
                {
                    Sucesso = false,
                    Mensagem = "Parâmetros da requisição inválido.",
                    Retorno = null
                };
            }

            if (param.TbStatusAppId != 1 && param.TbStatusAppId != 2)
            {
                return new ApiGenericResult<AgendaConvidadoDTO>
                {
                    Sucesso = false,
                    Mensagem = "Status deve ser 1-Aceito ou 2-Recusado.",
                    Retorno = null
                };
            }

            try
            {
                var retorno = await _jornadaComercialAppRepository.AceitarRecusarConvidadoNaAgenda(param);
                return new ApiGenericResult<AgendaConvidadoDTO>
                {
                    Sucesso = true,
                    Mensagem = "Convite Aceito/Recusado.",
                    Retorno = retorno
                };
            }
            catch (Exception ex)
            {
                return new ApiGenericResult<AgendaConvidadoDTO>
                {
                    Sucesso = false,
                    Mensagem = ex.Message,
                    Retorno = null
                };
            }
        }

        public async Task<ApiGenericResult<bool>> ExcluirAgendaConvidado(int tbAgendasComerciaisId, string codigoColaboradorExterno)
        {
            if (tbAgendasComerciaisId <= 0 || string.IsNullOrWhiteSpace(codigoColaboradorExterno))
            {
                return new ApiGenericResult<bool>
                {
                    Sucesso = false,
                    Mensagem = "Parâmetros da requisição inválido.",
                    Retorno = false
                };
            }

            try
            {
                var ok = await _jornadaComercialAppRepository.ExcluirAgendaConvidado(tbAgendasComerciaisId, codigoColaboradorExterno);
                return new ApiGenericResult<bool>
                {
                    Sucesso = ok,
                    Mensagem = ok ? "Convite removido com sucesso." : "Não foi possível remover o registro.",
                    Retorno = ok
                };
            }
            catch (Exception ex)
            {
                return new ApiGenericResult<bool>
                {
                    Sucesso = false,
                    Mensagem = ex.Message,
                    Retorno = false
                };
            }
        }

    }
} 