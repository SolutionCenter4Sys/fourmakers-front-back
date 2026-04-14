using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain;
using Core.DomainModel.Projeto;
using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Projeto.ProjetoOrg;
using Foursys.Domain.Interfaces.Services;
using Logs.Infra.Attributes;
using Projeto.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using SRS.Infra.Constantes;

namespace Projeto.Domain.Impl.Services
{
    [LogDomainClass]
    public class ProjetoOrgService : IProjetoOrgService
    {
        private IProjetoOrgRepository _projetoOrgRepository;
        private IAtividadeProjetoService _atividadeProjetoService;
        private IClienteOrgService _clienteProjetoService;
        private IBuscaParametroConfiguracaoService _buscaParametroConfiguracaoService;
        private IUnitOfWork _unitOfWork;
        private IDBConnectionUnitOfWork _dbConnectionUnitOfWork;
        private IRestricaoDeAcessoService _restricaoDeAcessoService;

        public ProjetoOrgService(IProjetoOrgRepository projetoOrgRepository,
                                 IAtividadeProjetoService atividadeProjetoService,
                                 IClienteOrgService clienteOrgService,
                                 IBuscaParametroConfiguracaoService buscaParametroConfiguracaoService,
                                 IUnitOfWork unitOfWork, IDBConnectionUnitOfWork dbConnectionUnitOfWork, IRestricaoDeAcessoService restricaoDeAcessoService)
        {
            _projetoOrgRepository = projetoOrgRepository;
            _atividadeProjetoService = atividadeProjetoService;
            _clienteProjetoService = clienteOrgService;
            _unitOfWork = unitOfWork;
            _dbConnectionUnitOfWork = dbConnectionUnitOfWork;
            _restricaoDeAcessoService = restricaoDeAcessoService;
            _buscaParametroConfiguracaoService = buscaParametroConfiguracaoService;
        }
        public Task<IEnumerable<ProjetosOrgResult>> ListarProjetos(int orgId, int cursor, int limite, int codStatus, string nomeProjeto)
        {
            try
            {
                return _projetoOrgRepository.ListarProjetos(orgId, cursor, limite, codStatus, nomeProjeto);
            }
            catch
            {
                throw;
            }
        }
        public async Task<ProjetoOrgDetalhesDTO> CadastrarProjeto(ProjetoOrgDTO param, string cpf, int orgId)
        {
            param.CodProjeto = param.CodProjeto?.Trim();
            ValidacaoCriarEditarProjeto(param, orgId, CRUDEnum.Create);

            ValidarDadosRepetidos(param);

            _dbConnectionUnitOfWork.BeginTransaction();           
            try
            {
                var diretoria = await _projetoOrgRepository.BuscaDadosDiretoriaAsync(param.CodigoColaboradorGerente[0], orgId);

                var status = await _projetoOrgRepository.BuscaDadosStatusAsync(param.CodStatus, orgId);

                var cliente = await _clienteProjetoService.ObterClientePorCodigoAsync(param.ClienteProjeto.CodigoCliente, orgId);

                bool clienteExiste = cliente != null;

                if (clienteExiste)
                {
                    if (!param.ClienteProjeto.NomeCliente.Equals(cliente.NomeCliente, StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new ArgumentException($"Já existe um cliente cadastrado para o código informado {cliente.CodigoCliente}. Nome do cliente atual cadastrado: {cliente.NomeCliente}");
                    }
                }

                bool cadastraClienteOcultoNaGestaoAlocados = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoEnum.CADASTRA_CLIENTE_OCULTO_NA_GESTAO_ALOCADOS, orgId, cpf);

                await _projetoOrgRepository.CadastrarProjeto(param, orgId, diretoria.codDiretoria, diretoria.diretoria, status, TipoCadastroProjetoOrgEnum.CADASTRO_MANUAL_USUARIO, clienteExiste, cadastraClienteOcultoNaGestaoAlocados);

                var devePularGerenciaClientesProjeto = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoEnum.DEVE_PULAR_GERENCIA_CLIENTES_PROJETO, orgId, cpf);

                if (!devePularGerenciaClientesProjeto)
                {
                    await GerenciarClienteAssociadosAoProjeto(param.ClienteProjeto.CodigoCliente, CRUDEnum.Create, orgId);
                }

                var projetoEditadoDTO = await _projetoOrgRepository.ObterProjetoPorCodigo(param.CodProjeto, orgId);

                _dbConnectionUnitOfWork.Commit();
                return projetoEditadoDTO;
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.Rollback();
                throw;
            }
        }
        private static void ValidarDadosRepetidos(ProjetoOrgDTO param)
        {
            var properties = typeof(ProjetoOrgDTO).GetProperties();

            foreach (var prop in properties)
            {
                var propValue = prop.GetValue(param) as IList<string>;

                if (propValue != null && propValue.Count != propValue.Distinct().Count())
                {
                    throw new Exception($"{prop.Name} duplicado(s)");
                }
            }
        }

        public async Task<ProjetoOrgDetalhesDTO> EditarProjeto(ProjetoOrgDTO param, string cpf, int orgId)
        {
            param.CodProjeto = param.CodProjeto?.Trim();
            ValidacaoCriarEditarProjeto(param, orgId, CRUDEnum.Update);
            _dbConnectionUnitOfWork.BeginTransaction();
            try
            {
                var diretoria = await _projetoOrgRepository.BuscaDadosDiretoriaAsync(param.CodigoColaboradorGerente[0], orgId);
                var status = await _projetoOrgRepository.BuscaDadosStatusAsync(param.CodStatus, orgId);

                var cliente = await _clienteProjetoService.ObterClientePorCodigoAsync(param.ClienteProjeto.CodigoCliente, orgId);

                bool clienteExiste = cliente != null;

                if (clienteExiste)
                {
                    if (!param.ClienteProjeto.NomeCliente.Equals(cliente.NomeCliente, StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new ArgumentException($"Já existe um cliente cadastrado para o código informado {cliente.CodigoCliente}. Nome do cliente atual cadastrado: {cliente.NomeCliente}");
                    }
                }

                var projeto = _projetoOrgRepository.ObterProjetoPorCodigo(param.CodProjeto, orgId).Result;
                var codigoClienteAntigo = projeto.ClienteProjeto.CodigoCliente;

                // busca valor do banco, pois (a princípio) este dado não será alterado
                var codClienteRegistroCarga = projeto.CodClienteRegistroCarga;
                var nomeClienteRegistroCarga = projeto.NomeClienteRegistroCarga;
                var tipoCadastro = projeto.TipoCadastro;

                bool deveOcultarNaGestaoDeAlocados = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoEnum.CADASTRA_CLIENTE_OCULTO_NA_GESTAO_ALOCADOS, orgId, cpf);

                await _projetoOrgRepository.EditarProjetoAsync(param, orgId, diretoria.codDiretoria, diretoria.diretoria, status, codClienteRegistroCarga, nomeClienteRegistroCarga, tipoCadastro, clienteExiste, deveOcultarNaGestaoDeAlocados);

                var devePularGerenciaClientesProjeto = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoEnum.DEVE_PULAR_GERENCIA_CLIENTES_PROJETO, orgId, cpf);

                if (!devePularGerenciaClientesProjeto)
                {
                    await GerenciarClienteAssociadosAoProjeto(param.ClienteProjeto.CodigoCliente, CRUDEnum.Update, orgId, codigoClienteAntigo);
                }

                await _projetoOrgRepository.CadastraAtividade(orgId, param.AtividadeProjeto, param.CodProjeto);
                var projetoEditadoDTO = await _projetoOrgRepository.ObterProjetoPorCodigo(param.CodProjeto, orgId);
                
                _dbConnectionUnitOfWork.Commit();
                return projetoEditadoDTO;
            }
            catch (Exception)
            {
                _dbConnectionUnitOfWork.Rollback();
                throw;
            }
        }

        public async Task<ProjetoOrgDetalhesDTO> ObterProjetoPorCodigo(string codProjeto, int orgId)
        {
            try
            {
                var projetoEditadoDTO = await _projetoOrgRepository.ObterProjetoPorCodigo(codProjeto, orgId);
                return projetoEditadoDTO;
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Erro na Service: ObterProjetoPorCodigo");
                System.Console.WriteLine(e.Message);
                throw;
            }
        }
        
        public async Task<IEnumerable<ProjetoSimplesDTO>> ListarProjetosDoCliente(string codCliente, int orgId)
        {
            try
            {
                return await _projetoOrgRepository.ListarProjetosDoCliente(codCliente, orgId);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Erro na Service: ListarProjetosDoCliente");
                System.Console.WriteLine(e.Message);
                throw;
            }
        }

        private void ValidacaoCriarEditarProjeto(ProjetoOrgDTO param, int orgId, CRUDEnum cRUDEnum)
        {
            string MSG_CODIGO_PROJETO_INVALIDO = "O código do status não pode ser menor ou igual a zero.";
            string MSG_NOME_PROJETO_OBRIGATORIO = "O nome do projeto é obrigatório.";
            string MSG_CODIGO_PROJETO_OBRIGATORIO = "O código do projeto é obrigatório.";
            string MSG_PROJETO_JA_EXISTE = "Esse projeto já existe.";
            string MSG_DATA_FIM_MENOR_QUE_DATA_INICIO = "A data início do projeto deve ser maior ou igual à data fim.";
            string MSG_GERENTE_OBRIGATORIO = "Deve-se designar ao menos um gerente para o projeto.";

            if (string.IsNullOrEmpty(param.CodProjeto))
            {
                throw new ArgumentException(MSG_CODIGO_PROJETO_OBRIGATORIO);
            }

            if (param.CodStatus.ToIntOuZero() <= 0)
            {
                throw new ArgumentException(MSG_CODIGO_PROJETO_INVALIDO);
            }

            if (cRUDEnum == CRUDEnum.Create)
            {
                if (_projetoOrgRepository.ProjetoExiste(param.CodProjeto, orgId))
                {
                    throw new ArgumentException(MSG_PROJETO_JA_EXISTE);
                }
            }

            if (String.IsNullOrEmpty(param.NomeProjeto))
            {
                throw new ArgumentException(MSG_NOME_PROJETO_OBRIGATORIO);
            }

            if (param.DataFim != null)
            {
                if (!DateTimeUtil.VerificarSeDataFimEhMaiorOuIgualQueDataInicio(param.DataInicio, (DateTime)param.DataFim))
                {
                    throw new ArgumentException(MSG_DATA_FIM_MENOR_QUE_DATA_INICIO);
                }
            }

            if (param.CodigoColaboradorGerente is null || param.CodigoColaboradorGerente.Count == 0)
            {
                throw new ArgumentException(MSG_GERENTE_OBRIGATORIO);
            }

            // validar se o gerente existe no banco e se é relacionado ao orgId
        }

        private async Task GerenciarClienteAssociadosAoProjeto(string codigoClienteAtual, CRUDEnum cRUDEnum, int orgId, string codigoClienteAntigo = null)
        {
            if (string.IsNullOrEmpty(codigoClienteAtual) || (cRUDEnum == CRUDEnum.Update && codigoClienteAntigo == null))
            {
                throw new Exception("Erro: Impossível gerenciar clientes associados aos projetos, parâmetros insuficientes."); //erro interno de codificacao
            }

            await _clienteProjetoService.AtivarClienteCasoEstejaAssociadoAAlgumProjetoAsync(codigoClienteAtual, orgId);
            if (cRUDEnum == CRUDEnum.Update)
            {
                await _clienteProjetoService.InativarClienteCasoNaoEstejaAssociadoANenhumProjetoAsync(codigoClienteAntigo, orgId);
            }
        }

        public async Task<List<ProjetoOrgColaboradorGerenteDetalhesDTO>> ListarGestoresProjeto(string codigoDiretoria, string codigoDepartamento, string codigoGestorAdm, int orgId, string cpfRequest)
        {
            var restricaoDiretoria =
                await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(cpfRequest, orgId, RestricaoDeAcessoTipoConstants.DIRETORIA, codigoDiretoria);
            return await _projetoOrgRepository.ListarGestoresProjeto(restricaoDiretoria, codigoDepartamento.ToNullSeTextoNullOuZero(), codigoGestorAdm.ToNullSeTextoNullOuZero(), orgId);
        }
    }
}