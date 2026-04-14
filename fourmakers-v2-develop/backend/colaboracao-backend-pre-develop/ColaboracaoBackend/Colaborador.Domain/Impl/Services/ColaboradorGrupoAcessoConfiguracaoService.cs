using Colaboracao.Core.Interfaces;
using Colaborador.Domain.Interfaces.Services;
using Core.Domain;
using Core.Domain.Colaborador;
using Core.Domain.Usuario;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using Logs.Infra.Attributes;
using System;
using System.Threading.Tasks;
using Ubiety.Dns.Core;

namespace Colaborador.Domain.Impl.Services
{
    [LogDomainClass]
    public class ColaboradorGrupoAcessoConfiguracaoService : IColaboradorGrupoAcessoConfiguracaoService
    {
        private readonly IColaboradorGrupoAcessoConfiguracaoRepository _colaboradorGrupoAcessoConfiguracaoRepository;
        private readonly IBuscaColaboradorRepository _buscaColaboradorRepository;
        private readonly IGrupoAcessoRepository _grupoAcessoRepository;
        private readonly IUsuarioGrupoAcessoRepository _usuarioGrupoAcessoRepository;
        private readonly IAcessoUsuarioRepository _acessoUsuarioRepository;
        private readonly IUsuarioExternoRepository _usuarioExternoRepository;

        private const string ADICIONAR = "adicionar";
        private const string REMOVER = "remover";

        public ColaboradorGrupoAcessoConfiguracaoService(IColaboradorGrupoAcessoConfiguracaoRepository repository,
                                                        IBuscaColaboradorRepository buscaColaboradorRepository,
                                                        IGrupoAcessoRepository grupoAcessoRepository,
                                                        IUsuarioGrupoAcessoRepository usuarioGrupoAcessoRepository,
                                                        IAcessoUsuarioRepository acessoUsuarioRepository,
                                                        IUsuarioExternoRepository usuarioExternoRepository)
        {
            _colaboradorGrupoAcessoConfiguracaoRepository = repository;
            _buscaColaboradorRepository = buscaColaboradorRepository;
            _usuarioGrupoAcessoRepository = usuarioGrupoAcessoRepository;
            _acessoUsuarioRepository = acessoUsuarioRepository;
            _usuarioExternoRepository = usuarioExternoRepository;
            _grupoAcessoRepository = grupoAcessoRepository;
            
        }

        public async Task<ApiGenericResult> VerificarConfiguracaoGrupoAcesso(int orgId)
        {
            var result = new ApiGenericResult() { Sucesso = true }; //se não existir nao tem problema, apenas não faz nada

            var configList = await _colaboradorGrupoAcessoConfiguracaoRepository.ObterConfiguracaoPorOrgAsync(orgId);

            foreach (var config in configList)
            {
                var grupoAcessoNaoPertenceOrg = await _grupoAcessoRepository.GrupoAcessoNaoPertenceOrg(config.GrupoAcessoId, orgId);

                if (grupoAcessoNaoPertenceOrg)
                {
                    result.Sucesso = false;
                    result.Erros.Add("Falha na associação automática de grupo: grupo de acesso não pertence a organização.");
                }
            }

            return result;
        }
        

        public async Task AssociarGrupoAcessoPorConfiguracaoAsync(int orgId, string codigoInternoColaborador, string cpfRequest)
        {
            var result = new ApiGenericResult() { Sucesso = true };

            try
            {
                // 1️º Buscar regra de configuração
                var configList = await _colaboradorGrupoAcessoConfiguracaoRepository.ObterConfiguracaoPorOrgAsync(orgId);

                foreach (var config in configList)
                {
                    // 2º Pegar valor da coluna dinamicamente
                    var valorColuna = await _colaboradorGrupoAcessoConfiguracaoRepository.ObterValorColunaEAcaoAsync(
                        config.Tabela,
                        config.Coluna,
                        config.Condicao,
                        orgId,
                        codigoInternoColaborador
                    );

                    // 3️º Comparar com a chave e chamar método de associação
                    if (valorColuna == config.Chave)
                    {
                        await AssociarOuDesassociarGrupoAcessoAsync(orgId, codigoInternoColaborador, config.GrupoAcessoId, config.Acao, cpfRequest);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar e associar grupo: {ex.Message}");
            }
        }

        private async Task AssociarOuDesassociarGrupoAcessoAsync(int orgId, string codigoInternoColaborador, int grupoAcessoId, string acao, string cpfRequest)
        {
            var colab = _buscaColaboradorRepository.GetColaboradorOrg(codigoInternoColaborador, orgId);

            if (await _grupoAcessoRepository.GrupoAcessoNaoPertenceOrg(grupoAcessoId, orgId))
            {
                throw new ApplicationException("Falha na associação automática de grupo: grupo de acesso não pertence a organização.");
            }

            var usuario = _usuarioExternoRepository.GetUserByCpfEOrgId(codigoInternoColaborador, orgId);
            var usuarioId = (int)usuario.UsuarioId;

            if (acao.ToLowerInvariant() == ADICIONAR.ToLowerInvariant())
            {
                await _usuarioGrupoAcessoRepository.AdicionarUsuarioGrupoAcesso(usuarioId, grupoAcessoId, cpfRequest);
            }

            if (acao.ToLowerInvariant() == REMOVER.ToLowerInvariant())
            {
                await _usuarioGrupoAcessoRepository.RemoverUsuarioGrupoAcesso(usuarioId, grupoAcessoId, cpfRequest);
            }
        }
    }
}
