using Core.Domain.Social;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Social;
using Social.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Social.Domain.Impl
{
    public class EncontrosBigNumbersService : IEncontrosBigNumbersService
    {
        private readonly IEncontrosBigNumbersRepository _encontrosBigNumbersRepository;

        public EncontrosBigNumbersService(IEncontrosBigNumbersRepository encontrosBigNumbersRepository)
        {
            _encontrosBigNumbersRepository = encontrosBigNumbersRepository;
        }

        public async Task<ApiGenericResult<EncontrosBigNumbers>> ObterBigNumbers(EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var result = await _encontrosBigNumbersRepository.ObterBigNumbers(param, orgIdUsuarioLogado);
            return new ApiGenericResult<EncontrosBigNumbers>
            {
                Sucesso = true,
                Mensagem = "BigNumbers obtidos com sucesso.",
                Retorno = result
            };
        }

        public async Task<ApiGenericResult<List<EncontrosBigNumbersCategoria>>> ObterBigNumbersCategoria(EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var result = await _encontrosBigNumbersRepository.ObterBigNumbersCategoria(param, orgIdUsuarioLogado);
            return new ApiGenericResult<List<EncontrosBigNumbersCategoria>>
            {
                Sucesso = true,
                Mensagem = "BigNumbers por Categoria obtidos com sucesso.",
                Retorno = result
            };
        }

        public async Task<ApiGenericResult<List<EncontrosBigNumbersObjetivo>>> ObterBigNumbersObjetivo(EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var result = await _encontrosBigNumbersRepository.ObterBigNumbersObjetivo(param, orgIdUsuarioLogado);
            return new ApiGenericResult<List<EncontrosBigNumbersObjetivo>>
            {
                Sucesso = true,
                Mensagem = "BigNumbers por Objetivo obtidos com sucesso.",
                Retorno = result
            };
        }

        public async Task<ApiGenericResult<List<AgendaRealizadaDetalhe>>> AgendaRealizadaDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var result = await _encontrosBigNumbersRepository.AgendaRealizadaDetalhe(param, orgIdUsuarioLogado);
            return new ApiGenericResult<List<AgendaRealizadaDetalhe>>
            {
                Sucesso = true,
                Mensagem = "Agendas realizadas obtidas com sucesso.",
                Retorno = result
            };
        }

        public async Task<ApiGenericResult<List<AgendaSemInteracaoDetalhe>>> AgendaSemInteracaoDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var result = await _encontrosBigNumbersRepository.AgendaSemInteracaoDetalhe(param, orgIdUsuarioLogado);
            return new ApiGenericResult<List<AgendaSemInteracaoDetalhe>>
            {
                Sucesso = true,
                Mensagem = "Agendas sem interação obtidas com sucesso.",
                Retorno = result
            };
        }

        public async Task<ApiGenericResult<List<ClientesImpactadosDetalhe>>> ClientesImpactadosDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var result = await _encontrosBigNumbersRepository.ClientesImpactadosDetalhe(param, orgIdUsuarioLogado);
            return new ApiGenericResult<List<ClientesImpactadosDetalhe>>
            {
                Sucesso = true,
                Mensagem = "Clientes impactados obtidos com sucesso.",
                Retorno = result
            };
        }

        public async Task<ApiGenericResult<List<GestoresImpactadosDetalhe>>> GestoresImpactadosDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var result = await _encontrosBigNumbersRepository.GestoresImpactadosDetalhe(param, orgIdUsuarioLogado);
            return new ApiGenericResult<List<GestoresImpactadosDetalhe>>
            {
                Sucesso = true,
                Mensagem = "Gestores impactados obtidos com sucesso.",
                Retorno = result
            };
        }

        public async Task<ApiGenericResult<List<CategoriaComInteracaoDetalhe>>> CategoriaComInteracaoDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var result = await _encontrosBigNumbersRepository.CategoriaComInteracaoDetalhe(param, orgIdUsuarioLogado);

            return new ApiGenericResult<List<CategoriaComInteracaoDetalhe>>
            {
                Sucesso = true,
                Mensagem = "Categorias com interação obtidas com sucesso.",
                Retorno = result
            };
        }

        public async Task<ApiGenericResult<List<ObjetivosAgendaDetalhe>>> ObjetivosAgendaDetalhe(string objetivoIds, EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var result = await _encontrosBigNumbersRepository.ObjetivosAgendaDetalhe(objetivoIds, param, orgIdUsuarioLogado);

            return new ApiGenericResult<List<ObjetivosAgendaDetalhe>>
            {
                Sucesso = true,
                Mensagem = "Objetivos com agendas obtidos com sucesso.",
                Retorno = result
            };
        }

        public async Task<ApiGenericResult<List<AcoesEmAtrasoDetalhe>>> AcoesEmAtrasoDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var result = await _encontrosBigNumbersRepository.AcoesEmAtrasoDetalhe(param, orgIdUsuarioLogado);

            return new ApiGenericResult<List<AcoesEmAtrasoDetalhe>>
            {
                Sucesso = true,
                Mensagem = "Ações em atraso obtidas com sucesso.",
                Retorno = result
            };
        }

        public async Task<ApiGenericResult<CategoriaEmFocoDetalhe>> CategoriaEmFocoDetalhe(int categoriaId, EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var result = await _encontrosBigNumbersRepository.CategoriaEmFocoDetalhe(categoriaId, param, orgIdUsuarioLogado);

            return new ApiGenericResult<CategoriaEmFocoDetalhe>
            {
                Sucesso = true,
                Mensagem = "Categoria em foco obtida com sucesso.",
                Retorno = result
            };
        }

        public async Task<ApiGenericResult<AgendaVersaoDTO>> AtualizaVersaoApp(AtualizaVersaoAppParam param)
        {
            if (string.IsNullOrWhiteSpace(param.Descricao))
            {
                return new ApiGenericResult<AgendaVersaoDTO>
                {
                    Sucesso = false,
                    Mensagem = "A descrição é obrigatória.",
                    Retorno = null
                };
            }

            if (string.IsNullOrWhiteSpace(param.Versao))
            {
                return new ApiGenericResult<AgendaVersaoDTO>
                {
                    Sucesso = false,
                    Mensagem = "A versão é obrigatória.",
                    Retorno = null
                };
            }

            var result = await _encontrosBigNumbersRepository.AtualizaVersaoApp(param.Descricao, param.Versao);

            return new ApiGenericResult<AgendaVersaoDTO>
            {
                Sucesso = true,
                Mensagem = "Versão do app atualizada com sucesso.",
                Retorno = result
            };
        }

        public async Task<ApiGenericResult<AgendaVersaoDTO>> ListaVersaoApp()
        {
            var result = await _encontrosBigNumbersRepository.ListaVersaoApp();

            if (result == null)
            {
                return new ApiGenericResult<AgendaVersaoDTO>
                {
                    Sucesso = false,
                    Mensagem = "Nenhuma versão do app cadastrada.",
                    Retorno = null
                };
            }

            return new ApiGenericResult<AgendaVersaoDTO>
            {
                Sucesso = true,
                Mensagem = "Versão do app obtida com sucesso.",
                Retorno = result
            };
        }
    }
}
