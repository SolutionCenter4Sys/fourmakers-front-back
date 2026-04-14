using ApiClient.Domain;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Core.DomainModel;
using Core.DomainModel.Usuario;
using DataTransferObject.Domain.Cargo;
using DataTransferObject.Domain.Diretoria;
using DataTransferObject.Domain.Foursys;
using Foursys.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SRS.Infra.Constantes;

using Logs.Infra.Attributes;

namespace Foursys.Domain.Impl.Services
{
    [LogDomainClass]
    public class FoursysService : IFoursysService
    {
        private readonly IFoursysDtoRepository _foursysRepository;
        private readonly IAspNetUser _aspNetUser;
        private readonly IEnvioEmail _envioEmail;
        private readonly IUsuarioRelatorioRepository _usuarioRelatorioRepository;
        private readonly IRestricaoDeAcessoService _restricaoDeAcessoService;

        public FoursysService(IFoursysDtoRepository foursysRepository, IAspNetUser aspNetUser, IUsuarioRelatorioRepository usuarioRelatorioRepository, IEnvioEmail envioEmail, IRestricaoDeAcessoService restricaoDeAcessoService)
        {
            _foursysRepository = foursysRepository;
            _aspNetUser = aspNetUser;
            _usuarioRelatorioRepository = usuarioRelatorioRepository;
            _envioEmail = envioEmail;
            _restricaoDeAcessoService = restricaoDeAcessoService;
        }

        public BuscaCargoResult BuscarCargos(string busca, int cursor, int limite)
        {
            return _foursysRepository.BuscarCargos(busca, cursor, limite);
        }

        public BuscaDiretoriaResult BuscarDiretorias(string busca, int cursor, int limite)
        {
            return _foursysRepository.BuscarDiretorias(busca, cursor, limite);
        }

        public CargoDTO InsereCargo(string cargo)
        {
            return _foursysRepository.InsereCargo(cargo);
        }

        public CargoDTO BuscarCargo(int id)
        {
            return _foursysRepository.BuscarCargo(id);
        }

        public CargoDTO AlteraCargo(int id, string cargo)
        {
            return _foursysRepository.AlteraCargo(id, cargo);
        }

        public bool DeletaCargo(int id, string cargo)
        {
            _foursysRepository.DeletaCargo(id);
            return true;
        }

        public List<UnidadesDTO> ListarUnidades()
        {
            return _foursysRepository.ListarUnidades();
        }

        public List<UsuarioAcessoDTO> ListarAcessoUsuarios()
        {
            return _usuarioRelatorioRepository.GetRelatorioAcessoUsuarios(_aspNetUser.GetUsuarioLogado());
        }

        public EnviarEmailCadastroIncompletoResult EnviarEmailCadastroIncompleto(IEnumerable<UsuarioAcessoDTO> usuarios)
        {
            EnviarEmailCadastroIncompletoResult ret = new();
            List<string> emailsEnviados = new();
            List<string> emailsNaoEnviados = new();
            try
            {
                foreach (UsuarioAcessoDTO usuario in usuarios)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(usuario.Email))
                        {
                            _envioEmail.EnviaEmailSemTemplate("", Utils.TemplateEmail.MontaTemplateCadastroIncompleto(VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.URL_BASE)), "Complete seu cadastro no Fourmakers!", usuario.Email);
                            emailsEnviados.Add(usuario.Email);
                        }
                    }
                    catch (Exception)
                    {
                        emailsNaoEnviados.Add(usuario.Email);
                    }
                }
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
            ret.EmailsEnviados = emailsEnviados;
            ret.EmailsNaoEnviados = emailsNaoEnviados;
            ret.Sucesso = true;
            return ret;
        }

        public List<UnidadesDTO> ListarUnidadesPorOrg(int orgId)
        {
            return _foursysRepository.ListarUnidadesPorOrg(orgId);
        }

        public async Task<ListaUnidadesResult> ListarUnidadesPorOrgRestricao(int orgId, string cpfRequest)
        {
            var objetoRetorno = new ListaUnidadesResult();
            var restricoes = await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(cpfRequest, orgId, RestricaoDeAcessoTipoConstants.DIRETORIA);
            var lista = await _foursysRepository.ListarUnidadesPorOrgIdComRestricao(orgId, restricoes);
            objetoRetorno.ListaUnidades.AddRange(lista);
            return objetoRetorno;
        }

        public List<ColaboradoresTotaisOrgIdResult> ListaColaboradoresOrgId(int orgId, int page, int pageSize)
        {
            List<ColaboradoresOrgIdResult> listaColaboradoresOrgId = _usuarioRelatorioRepository.ListaColaboradoresOrgId(orgId, page, pageSize);
            int totalColaboradoresOrgId = _usuarioRelatorioRepository.GetColaboradoresOrgId(orgId);
            string descricaoOrg = _usuarioRelatorioRepository.GetOrgDescricao(orgId);
            var totaisOrg = new List<TotaisOrgDTO>
            {
                new TotaisOrgDTO
                {
                    OrgId = orgId,
                    Descricao = descricaoOrg,
                    Total = totalColaboradoresOrgId
                }
            };

            return new List<ColaboradoresTotaisOrgIdResult>
            {
                new ColaboradoresTotaisOrgIdResult
                {
                    Colaboradores = listaColaboradoresOrgId,
                    TotaisOrg = totaisOrg
                }
            };
        }
    }
}
