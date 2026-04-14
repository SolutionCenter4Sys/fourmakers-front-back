using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.Usuario.GestaoDeAcesso;
using DataTransferObject.Domain.Usuario.GestaoDeAcesso.Recurso;
using DataTransferObject.Domain.Usuario.GestaoDeAcesso.Recurso.Constantes;
using DataTransferObject.Domain.Usuario.GestaoDeAcesso.RecursoMenu;
using DocumentFormat.OpenXml.Office.CustomUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Usuario.Domain.Interfaces.Services.GestaoDeAcesso.Recurso;
using Logs.Infra.Attributes;

namespace Usuario.Domain.Impl.Services.GestaoDeAcesso.Recurso
{
    [LogDomainClass]
    public class RecursoValidatorService : IRecursoValidatorService
    {

        private readonly IRecursoRepository _recursoRepository;
        private readonly IRecursoMenuFuncionalidadeSistemaRepository _recursoMenuFuncionalidadeSistemaRepository;
        private readonly IRecursoMenuRepository _recursoMenuRepository;
        private readonly IRecursoOrgDisponivelRepository _recursoOrgDisponivelRepository;

        public RecursoValidatorService(IRecursoRepository recursoRepository,
                                       IRecursoMenuFuncionalidadeSistemaRepository recursoFuncionalidadeSistemaRepository,
                                       IRecursoMenuRepository recursoMenuRepository,
                                       IRecursoOrgDisponivelRepository recursoOrgDisponivelRepository)
        {
            _recursoRepository = recursoRepository;
            _recursoMenuFuncionalidadeSistemaRepository = recursoFuncionalidadeSistemaRepository;

            _recursoMenuRepository = recursoMenuRepository;
            _recursoOrgDisponivelRepository = recursoOrgDisponivelRepository;
        }

        public async Task ValidaRecurso(RecursoInput input, CRUDEnum cRUDEnum, List<RecursoInput> listaInput = default, bool forcarExclusao = false)
        {
            if (listaInput == default) listaInput = new ();

            // primeiro passo: verificar se existe o objeto no banco
            if (cRUDEnum == CRUDEnum.Update || cRUDEnum == CRUDEnum.Delete)
            {
                await ValidaSeExisteRecurso(input.CodigoRecurso);
            }

            if (cRUDEnum == CRUDEnum.Create || cRUDEnum == CRUDEnum.Update)
            {

                if (cRUDEnum == CRUDEnum.Create)
                {
                    await ValidaSeNaoExisteRecurso(input.CodigoRecurso);
                }

                await CodigoRecursoMenuPaiPrecisaExistirNoBancoOuNaListagemDeInsercao(input.Menu.CodigoRecurso, input.Menu.CodigoRecursoMenuPai, listaInput);
                
                await CodigoRecursoMenuPaiPrecisaSerDoTipoGroup(input.Menu.CodigoRecursoMenuPai, input.CodigoRecurso, listaInput, input.Menu.TipoMenu);

                ValidarFormatoCodigoRecurso(input.Menu.CodigoRecurso);
                ValidarFormatoCodigoRecurso(input.Menu.CodigoRecursoMenu);

                await ValidaCodigoRecursoMenuUnico(input.Menu.CodigoRecursoMenu, listaInput);

                ValidarTipoMenu(input.Menu.TipoMenu);

                await ValidarCamposDeEntrada(input, cRUDEnum); // faz as validações padrões

            }

            if (cRUDEnum == CRUDEnum.Delete)
            {
                await ValidaSeExisteRecursoEmOutraTabelaUtilizando(input.Menu.CodigoRecursoMenu, cRUDEnum, forcarExclusao); 
            }

        }

        private async Task ValidaCodigoRecursoMenuUnico(string codigoRecursoMenu, List<RecursoInput> listaInput)
        {
            if (listaInput.Count(input => input.Menu.CodigoRecursoMenu == codigoRecursoMenu) >= 2) // aquie é 2 pq não conta o próprio objeto que ele encontrará na lista
            {
                throw new ApplicationException($"O código do recurso menu '{codigoRecursoMenu}' já existe na lista de inserção.");
            }

            var recursoExistente = await ObterRecursoMenuPorCodigoAsync(codigoRecursoMenu);
           
            if (recursoExistente != null)
            {
                throw new ApplicationException($"O código do recurso menu '{codigoRecursoMenu}' já existe no banco de dados.");
            }
        }

        // Método para simular a consulta no banco
        private async Task<RecursoMenuResult> ObterRecursoMenuPorCodigoAsync(string codigoRecursoMenu)
        {
            // Simule ou implemente a lógica de acesso ao banco para buscar o recurso pelo código
            return await _recursoMenuRepository.ObterRecursoMenuPorCodigoRecursoAsync(codigoRecursoMenu);
        }


        private async Task<RecursoResult> ObterRecursoNoBancoOuListaAsync(string codigoRecursoMenu, List<RecursoInput> listaInput)
        {
            RecursoResult recurso = null;

            var menu = await _recursoMenuRepository.ObterRecursoMenuPorCodigoRecursoAsync(codigoRecursoMenu);

            if (menu.IsNotNull())
            {
                var recursoResult = new RecursoResult();

                recursoResult.CodigoRecurso = menu.CodigoRecurso;

                if (recursoResult.Menu.IsNull()) recursoResult.Menu = new();

                recursoResult.Menu.CodigoRecurso = menu.CodigoRecurso;
                recursoResult.Menu.CodigoRecursoMenu = menu.CodigoRecursoMenu;
                recursoResult.Menu.CodigoRecursoMenuPai = menu.CodigoRecursoMenuPai;
                recursoResult.Menu.TipoMenu = menu.TipoMenu;

                recurso = recursoResult;
            }

            if (menu.IsNull())
            {
                var recursoInput = listaInput.FirstOrDefault(r => r.Menu.CodigoRecursoMenu == codigoRecursoMenu);

                if (recursoInput.IsNotNull())
                {
                    var recursoResult = new RecursoResult();

                    recursoResult.CodigoRecurso = recursoInput.CodigoRecurso;

                    if (recursoResult.Menu.IsNull()) recursoResult.Menu = new();

                    recursoResult.Menu.CodigoRecurso = codigoRecursoMenu;
                    recursoResult.Menu.CodigoRecurso = codigoRecursoMenu;
                    recursoResult.Menu.CodigoRecursoMenuPai = recursoInput.Menu.CodigoRecursoMenuPai;
                    recursoResult.Menu.TipoMenu = recursoInput.Menu.TipoMenu;

                    recurso = recursoResult;
                }
            }

            return recurso;
        }

        private void ValidarFormatoCodigoRecurso(string codigoRecurso)
        {
            if (string.IsNullOrWhiteSpace(codigoRecurso))
            {
                return; //nao eh responsabilidade deste metodo validar se existe ou nao
            }

            // Regex para validar o formato do código do recurso
            var regex = new Regex(@"^[a-z][a-z0-9_]*$");
            if (!regex.IsMatch(codigoRecurso))
            {
                throw new ApplicationException($"O código do recurso deve conter apenas letras minúsculas, números e underline, não pode começar com números. Código com erro: {codigoRecurso}");
            }
        }

        private void ValidarTipoMenu(string tipoMenu)
        {
            // Verifica se o tipo fornecido corresponde a algum valor válido do enum
            if (!Enum.TryParse(typeof(TipoMenuEnum), tipoMenu, true, out _))
            {
                throw new ApplicationException($"O tipo de menu '{tipoMenu}' é inválido. Tipos permitidos: {string.Join(", ", Enum.GetNames(typeof(TipoMenuEnum)))}.");
            }
        }


        private async Task CodigoRecursoMenuPaiPrecisaExistirNoBancoOuNaListagemDeInsercao(string codigoRecurso, string codigoRecursoMenuPai, List<RecursoInput> listaInput)
        {
            if (codigoRecursoMenuPai.IsNotEmpty())
            {
                var recursoPai = await ObterRecursoNoBancoOuListaAsync(codigoRecursoMenuPai, listaInput);

                // Valida se o recurso pai não foi encontrado
                if (recursoPai.IsNull())
                {
                    throw new ApplicationException($"O recurso pai com 'código recurso menu' {codigoRecursoMenuPai} não foi encontrado para inserção do recurso {codigoRecurso}.");
                }
            }
        }

        private async Task CodigoRecursoMenuPaiPrecisaSerDoTipoGroup(string codigoMenuRecursoPai, string codigoRecursoAtual, List<RecursoInput> listaInput, string tipoMenu)
        {
            if (codigoMenuRecursoPai.IsNotEmpty())
            {
                var recurso = await ObterRecursoNoBancoOuListaAsync(codigoMenuRecursoPai, listaInput);

                if (recurso.IsNull())
                {
                    throw new Exception($"Erro: o recurso pai com código {codigoMenuRecursoPai} não foi encontrado para validação do grupo.");
                }

                if (recurso.Menu.TipoMenu != TipoMenuEnum.group_sidebar.ToString())
                {
                    throw new ApplicationException($"O recurso pai '{codigoMenuRecursoPai}' informado no menu '{codigoRecursoAtual}' deve ser do tipo 'group_sidebar'.");
                }
            }
        }

        private async Task ValidarCamposDeEntrada(RecursoInput input, CRUDEnum cRUDEnum)
        {
            var campos = new List<CampoValidacao>();


            // obrigatório menu pra cadastrar
            if (input.Menu.IsNull())
            {
                campos.Add(new("Menu.CodigoRecurso", string.Empty, TipoValidacaoEnum.Obrigatoriedade));
            }

            if (input.Menu.IsNotNull())
            {
                campos.Add(new("CodigoInternoColaboradorCriacao", input.CodigoInternoColaboradorAlteracao, TipoValidacaoEnum.Obrigatoriedade));
                campos.Add(new("Nome Menu", input.Menu.NomeMenu, TipoValidacaoEnum.Obrigatoriedade));
                campos.Add(new("Ativo", input.Ativo, TipoValidacaoEnum.Obrigatoriedade));
                campos.Add(new("Tipo Menu", input.Ativo, TipoValidacaoEnum.Obrigatoriedade));
                campos.Add(new("Código Recurso", input.CodigoRecurso, TipoValidacaoEnum.Obrigatoriedade));

                //Outras validações
                campos.Add(new("Código Recurso", input.CodigoRecurso, TipoValidacaoEnum.TamanhoMaximo) { TamanhoMaximo = 50 });
                campos.Add(new("Código Recurso Pai", input.CodigoRecurso, TipoValidacaoEnum.TamanhoMaximo) { TamanhoMaximo = 50 });
                campos.Add(new("Nome Menu", input.CodigoRecurso, TipoValidacaoEnum.TamanhoMaximo) { TamanhoMaximo = 100 });
                campos.Add(new("Link Externo", input.Menu.LinkExternoNovaPagina, TipoValidacaoEnum.TamanhoMaximo) { TamanhoMaximo = 500 });
                campos.Add(new("Código Icone", input.Menu.CodigoIcone, TipoValidacaoEnum.TamanhoMaximo) { TamanhoMaximo = 50 });
                campos.Add(new("Ordenação", input.Menu.Ordenacao, TipoValidacaoEnum.ValorMaiorQueZero));
            }
            await ValidadorCamposUtil.ValidaCampos(campos);
        }

        private async Task ValidaSeExisteRecurso(string codRecurso)
        {
            var recurso = await _recursoRepository.ObterRecursoPorCodigoRecursoAsync(codRecurso);

            if (recurso.IsNull())
            {
                throw new ApplicationException($"Recurso com codigo: '{codRecurso.ToString()}' não encontrado.");
            }
        }

        private async Task ValidaSeNaoExisteRecurso(string codigoRecurso)
        {
            var recurso = await _recursoRepository.ObterRecursoPorCodigoRecursoAsync(codigoRecurso);

            if (recurso.IsNotNull())
            {
                throw new ApplicationException($"CodigoRecurso: {codigoRecurso} já existente na tabela {RecursoService.DESCRICAO_ENTIDADE}.");
            }
        }


        private async Task ValidaSeExisteRecursoEmOutraTabelaUtilizando(string codigoRecurso, CRUDEnum cRUDEnum, bool forcarExclusao = false)
        {
            string acao = cRUDEnum == CRUDEnum.Delete ? "deletado" : "manipulado";

            // Verificar se o recurso está em uso em RecursoMenuFuncionalidadeSistema
            var funcionalidadesRelacionadas = await _recursoMenuFuncionalidadeSistemaRepository.ObterRecursoMenuFuncionalidadeSistemaPorCodigoRecursoMenuAsync(codigoRecurso);

            if (funcionalidadesRelacionadas.Any())
            {
                if (!forcarExclusao)
                {
                    throw new ApplicationException($"O recurso '{codigoRecurso}' está em uso em funcionalidades do sistema e não pode ser {acao}.");
                }
            }

            // Verificar se o recurso está em uso em RecursoMenu
            var menuRelacionado = await _recursoMenuRepository
                .ObterRecursoMenuPorCodigoRecursoAsync(codigoRecurso);

            if (menuRelacionado.IsNotNull())
            {
                if (!forcarExclusao)
                {
                    throw new ApplicationException($"O recurso '{codigoRecurso}' está associado a menus e não pode ser {acao}.");
                }
            }

            // Verificar se o recurso está em uso em RecursoOrgDisponivel
            var recursoOrgDisponivels = await _recursoOrgDisponivelRepository
                .ListarRecursoOrgDisponivelAsync();

            if (recursoOrgDisponivels.Where(ro => ro.CodigoRecursoMenu == codigoRecurso).Any())
            {
                if (!forcarExclusao)
                {
                    throw new ApplicationException($"O recurso '{codigoRecurso}' está associado a organizações e não pode ser {acao}.");
                }
            }
        }

    }
}