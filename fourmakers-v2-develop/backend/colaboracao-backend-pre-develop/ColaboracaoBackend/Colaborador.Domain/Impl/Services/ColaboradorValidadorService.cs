using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Colaborador.Domain.Interfaces.Services;
using Core.Domain.Usuario;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.Usuario;
using Foursys.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Logs.Infra.Attributes;

namespace Colaborador.Domain.Impl
{
    [LogDomainClass]
    public class ColaboradorValidadorService : IColaboradorValidadorService
    {
        private readonly IUsuarioColaboradorRepository _usuarioColaboradorRepository;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly IBuscaParametroConfiguracaoService _buscaParametroConfiguracaoService;

        private const string DDI_BRASIL = "55";

        public ColaboradorValidadorService(IUsuarioColaboradorRepository usuarioColaboradorRepository, IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository, IBuscaParametroConfiguracaoService buscaParametroConfiguracaoService)
        {
            _usuarioColaboradorRepository = usuarioColaboradorRepository;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _buscaParametroConfiguracaoService = buscaParametroConfiguracaoService;
        }

        public void ValidaColaboradorCadastro(CadastroColaboradorInput cadastroColaboradorInput, string cpfUsuario, int orgId, ApiGenericResult<string> ret)
        {
            ValidaColaborador(cadastroColaboradorInput, CRUDEnum.Create, cpfUsuario, orgId, ret);
        }

        public void ValidaColaboradorEditar(CadastroColaboradorInput cadastroColaboradorInput, string cpfUsuario, int orgId, ApiGenericResult<string> ret)
        {
            ValidaColaborador(cadastroColaboradorInput, CRUDEnum.Update, cpfUsuario, orgId, ret);
        }

        private void ValidaColaborador(CadastroColaboradorInput cadastroColaboradorInput, CRUDEnum acao, string cpf, int orgId, ApiGenericResult<string> ret)
        {
            ValidaAcessoCadastroDePessoas(cpf, orgId, FuncionalidadeSistemaEnum.CADASTRO_PESSOAS);
            var parametrosNaoObrigatorios = _buscaParametroConfiguracaoService.GetParametroConfiguracao<string>(ParametroOrgCodigoEnum.CAMPOS_NAO_OBRIGATORIOS_CADASTRO_COLABORADOR, orgId, cpf);
            var arrayParametrosNaoObrigatorios = parametrosNaoObrigatorios.Split(";");

            ret.Erros = new List<string>();

            if (acao == CRUDEnum.Create || acao == CRUDEnum.Update)
            {
                if (string.IsNullOrEmpty(cadastroColaboradorInput.Email) && DeveValidarEsteCampo(arrayParametrosNaoObrigatorios, "Email"))
                {
                    ret.Erros.Add("Email é obrigatório.");
                }

                if (string.IsNullOrEmpty(cadastroColaboradorInput.CodGestor) && DeveValidarEsteCampo(arrayParametrosNaoObrigatorios, "Gestor"))
                {
                    ret.Erros.Add("Código Gestor é obrigatório.");
                }

                if ((string.IsNullOrEmpty(cadastroColaboradorInput.CodDiretoria) || string.IsNullOrEmpty(cadastroColaboradorInput.Diretoria)) && DeveValidarEsteCampo(arrayParametrosNaoObrigatorios, "Diretoria"))
                {
                    ret.Erros.Add("O campo de código ou nome da diretoria não pode estar vazio. Por favor, preencha ambos os campos corretamente.");
                }

                if ((string.IsNullOrEmpty(cadastroColaboradorInput.CodDepartamento) || string.IsNullOrEmpty(cadastroColaboradorInput.Departamento)) && DeveValidarEsteCampo(arrayParametrosNaoObrigatorios, "Departamento"))
                {
                    ret.Erros.Add("O campo de código ou nome do departamento não pode estar vazio. Por favor, preencha ambos os campos corretamente.");
                }

                if (!cadastroColaboradorInput.ContatoPrincipal.IsEmpty() && cadastroColaboradorInput.ContatoPrincipalDDI.IsEmpty())
                {
                    ret.Erros.Add("Telefone Celular obriga preenchimento DDI.");
                }

                if (!String.IsNullOrEmpty(cadastroColaboradorInput.ContatoPrincipal)
                    && ((cadastroColaboradorInput.ContatoPrincipalDDI == DDI_BRASIL && cadastroColaboradorInput.ContatoPrincipal.Length < 10)
                    || cadastroColaboradorInput.ContatoPrincipal.Length < 4))
                {
                    ret.Erros.Add("Telefone Celular inválido. Deve conter mais que 10 dígitos.");
                }

                if (cadastroColaboradorInput.DataAdmissao.IsEmpty() && DeveValidarEsteCampo(arrayParametrosNaoObrigatorios, "DataAdmissao"))
                {
                    ret.Erros.Add("Data de admissão é obrigatória.");
                }

                if (string.IsNullOrEmpty(cadastroColaboradorInput.CodColaborador))
                {
                    ret.Erros.Add("O código do colaborador é obrigatório.");
                }

                if (string.IsNullOrEmpty(cadastroColaboradorInput.NomeColaborador))
                {
                    ret.Erros.Add("Nome do colaborador é obrigatório.");
                }

                if (!string.IsNullOrEmpty(cadastroColaboradorInput.CodColaborador) && cadastroColaboradorInput.CodColaborador.Length > 14)
                {
                    ret.Erros.Add("O código do colaborador é inválido. Deve ter no máximo 10 caracteres.");
                }

                if (!string.IsNullOrEmpty(cadastroColaboradorInput.NomeColaborador) && !Regex.IsMatch(cadastroColaboradorInput.NomeColaborador, @"^[a-zA-ZÀ-ÿ]+(?:\s[a-zA-ZÀ-ÿ]+)+$"))
                {
                    ret.Erros.Add("Nome do colaborador inválido. Deve ter pelo menos dois caracteres e um espaço. Exemplo: mínimo 'A A'.");
                }

                if (!string.IsNullOrEmpty(cadastroColaboradorInput.Email) && !Colaboracao.Helper.ToolsUtil.ValidaEmail(cadastroColaboradorInput.Email))
                {
                    ret.Erros.Add("Email inválido. Deve ser fornecido e estar em um formato válido (exemplo: 'a@a.aa').");
                }

                if (!string.IsNullOrEmpty(cadastroColaboradorInput.CodGestor))
                {
                    ValidaSeExisteGestor(cadastroColaboradorInput, orgId, ret);
                }
            }

            if (acao == CRUDEnum.Create)
            {
                ValidaSeJaExisteColaborador(cadastroColaboradorInput, orgId, ret);
            }

            if (acao == CRUDEnum.Update)
            {
                ValidaSeJaExisteOutroColaborador(cadastroColaboradorInput, orgId, ret);
            }

            ret.Sucesso = ret.Erros.Count == 0; //se não tem erro é sucesso
            ret.Mensagem = ret.Sucesso ? "Colaborador cadastrado com sucesso!" : "Erro ao cadastrar colaborador!";
        }

        private void ValidaSeExisteGestor(CadastroColaboradorInput cadastroColaboradorInput, int orgId, ApiGenericResult<string> ret)
        {
            // Verificar se existe um colaborador para esta organização com o cod gestor que está sendo informado
            if (!String.IsNullOrEmpty(cadastroColaboradorInput.CodGestor)
                && !_usuarioColaboradorRepository.ExisteColaboradorComCodColaboradorExterno(cadastroColaboradorInput.CodGestor, orgId))
            {
                ret.Erros.Add("Código Gestor inexistente em nossa base de dados.");
            }
        }

        private bool DeveValidarEsteCampo(string[] configuracao, string comparacao)
        {
            return !configuracao.Contains(comparacao);
        }

        private void ValidaAcessoCadastroDePessoas(string cpf, int orgId, FuncionalidadeSistemaEnum funcionalidadesAcesso)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpf, orgId, funcionalidadesAcesso).Result;

            if (!isValid)
            {
                throw new UnauthorizedAccessException("Acesso negado para Cadastro Colaborador");
            }
        }

        private void ValidaAcessoInativarPessoas(string cpf, int orgId, FuncionalidadeSistemaEnum funcionalidadesAcesso)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpf, orgId, funcionalidadesAcesso).Result;

            if (!isValid)
            {
                throw new UnauthorizedAccessException("Acesso negado para Inativar Pessoas");
            }
        }

        private void ValidaSeJaExisteColaborador(CadastroColaboradorInput cadastroColaboradorInput, int orgId, ApiGenericResult<string> ret)
        {
            // Verificar se já existe um colaborador com o CPF especificado na organização
            if (!String.IsNullOrEmpty(cadastroColaboradorInput.DocumentoColaborador) && _usuarioColaboradorRepository.ExisteColaboradorComCPF(cadastroColaboradorInput.DocumentoColaborador, orgId))
            {
                ret.Erros.Add("CPF já cadastrado para esta organização.");
            }

            // Verificar se já existe um colaborador com o e-mail especificado na organização
            if (_usuarioColaboradorRepository.ExisteColaboradorComEmail(cadastroColaboradorInput.Email, orgId))
            {
                ret.Erros.Add("E-mail já cadastrado para esta organização.");
            }

            // Verificar se já existe um colaborador com o código de colaborador especificado na organização
            if (_usuarioColaboradorRepository.ExisteColaboradorComCodColaboradorExterno(cadastroColaboradorInput.CodColaborador, orgId))
            {
                ret.Erros.Add("Código Colaborador já cadastrado para esta organização.");
            }
        }

        private void ValidaSeJaExisteOutroColaborador(CadastroColaboradorInput cadastroColaboradorInput, int orgId, ApiGenericResult<string> ret)
        {
            // Verificar se já existe outro colaborador com o código de colaborador especificado na organização
            if (cadastroColaboradorInput.CodColaborador.IsNotEmpty())
            {
                var codigoInterno = _usuarioColaboradorRepository.GetCodigoInternoByCodExterno(cadastroColaboradorInput.CodColaborador, orgId);

                // Se for empty, é pq está ok, não existe colaborador
                if (codigoInterno.IsNotEmpty())
                {
                    if (codigoInterno != cadastroColaboradorInput.Cpf)
                    {
                        ret.Erros.Add("Código Colaborador já cadastrado para outro colaborador desta organização.");
                    }
                }
            }
        }
    }
}