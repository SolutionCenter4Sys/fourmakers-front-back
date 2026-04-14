using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.ParametroOrg;
using Core.Domain.Reembolso.Parametro;
using Core.Domain.Reembolso.Solicitacao;
using Core.Domain.Reembolso.Verba;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao.Aprovacao;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;
using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.Util.Enum;
using Financeiro.Domain.Interfaces.Reembolso.Validadores;
using Foursys.Domain.Interfaces.Services;
using Logs.Infra.Attributes;

namespace Financeiro.Domain.Services.Reembolso.Validadores;

[LogDomainClass]
public class SolicitacaoReembolsoValidadorService : ISolicitacaoReembolsoValidadorService
{
    private readonly IVerbaRepository _verbaRepository;
    private readonly IVerbaTipoRepository _verbaTipoRepository;
    private readonly IParametroReembolsoRepository _parametroReembolsoRepository;
    private readonly ISolicitacaoReembolsoRepository _solicitacaoReembolsoRepository;
    private readonly IBuscaParametroConfiguracaoService _buscaParametroConfiguracaoService;

    public SolicitacaoReembolsoValidadorService(IVerbaRepository verbaRepository, IVerbaTipoRepository verbaTipoRepository, IParametroReembolsoRepository parametroReembolsoRepository, ISolicitacaoReembolsoRepository solicitacaoReembolsoRepository, IBuscaParametroConfiguracaoService buscaParametroConfiguracaoService)
    {
        _verbaRepository = verbaRepository;
        _verbaTipoRepository = verbaTipoRepository;
        _parametroReembolsoRepository = parametroReembolsoRepository;
        _solicitacaoReembolsoRepository = solicitacaoReembolsoRepository;
        _buscaParametroConfiguracaoService = buscaParametroConfiguracaoService;
    }

    public async Task ValidarSolicitacao(SolicitacaoReembolsoDTO input, CRUDEnum operation, int orgId, string cpfRequest)
    {
        if (operation == CRUDEnum.Create)
        {
            await ValidaPersistenciaVerba(input);
            ValidaData(input.DataDespesa);
            ValidaPermissaoSemProjetoECliente(input.ProjetoId, input.ClienteId, orgId, cpfRequest);
        }
    }

    private void ValidaPermissaoSemProjetoECliente(string? projetoId, string? clienteId, int orgId, string cpfRequest)
    {
        var configuracaoPermissao = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoFrontEndEnum.REEMBOLSO_PERMITIR_SOLICITACAO_SEM_PROJETO , orgId, cpfRequest);
        if (!configuracaoPermissao && projetoId?.ToNullSeTextoNull() == null)
        {
            throw new ArgumentException("O campo 'Projeto' é obrigatório.");
        }
        if (!configuracaoPermissao && clienteId?.ToNullSeTextoNull() == null)
        {
            throw new ArgumentException("O campo 'Cliente' é obrigatório.");
        }
    }

    public void ValidarAgrupador(InserirSolicitacaoReembolsoListaDTO input)
    {
        if (string.IsNullOrWhiteSpace(input.Objetivo))
        {
            throw new ArgumentException("O campo 'Objetivo' é obrigatório.");
        }

        if (input.DataInicio == null || input.DataInicio == DateTime.MinValue)
        {
            throw new ArgumentException("O campo 'DataInicio' é obrigatório e deve ser válido.");
        }

        if (input.DataFim == null || input.DataFim == DateTime.MinValue)
        {
            throw new ArgumentException("O campo 'DataFim' é obrigatório e deve ser válido.");
        }

        if (input.DataFim < input.DataInicio)
        {
            throw new ArgumentException("A data de término não pode ser anterior à data de início.");
        }
    }

    public async Task<SolicitacaoAprovadorDetalheDTO> ValidarAprovacao(int solicitacaoId, string codGestor, string? observacao, SolicitacaoStatusEnum status, int orgId)
    {
        var solicitacao = await _solicitacaoReembolsoRepository.BuscarSolicitacaoGestorPorId(solicitacaoId, codGestor);
        var parametrosOrg = await _parametroReembolsoRepository.BuscarPorOrgIdAsync(orgId);
        if (solicitacao == null)
        {
            throw new ArgumentException("Solicitação não encontrada");
        }

        if (solicitacao.StatusId == (int)SolicitacaoStatusEnum.Aprovado)
        {
            throw new ArgumentException("Solicitação já aprovada");
        }

        if (parametrosOrg != null && !parametrosOrg.PermitirAprovarMinhasSolicitacoes)
        {
            if (solicitacao.CodigoColaborador == codGestor)
            {
                throw new ArgumentException("Não é possivel aprovar suas proprias solicitações");
            }
        }

        if (status == SolicitacaoStatusEnum.Reprovado)
        {
            if (string.IsNullOrWhiteSpace(observacao))
            {
                throw new ArgumentException("Ao reprovar observação é obrigatoria.");
            }
        }

        return solicitacao;
    }
    

    private async Task ValidaPersistenciaVerba(SolicitacaoReembolsoDTO input)
    {
        var buscarVerba = await _verbaRepository.ObterPorIdAsync(input.VerbaId);
        if (buscarVerba == null)
        {
            throw new ArgumentException("Verba não encontrada");
        }
        
        var buscarTipo = await _verbaTipoRepository.BuscarPorIdAsync(buscarVerba.TipoCusto);
        if (buscarTipo == null)
        {
            throw new ArgumentException("Tipo da verba não encontrado");
        }
        ValidaTipoVerba(buscarTipo, input.Valor, input.ValorUnidade, input.Quantidade);
    }

    private void ValidaTipoVerba(VerbaTipoDTO tipo, decimal valor, decimal? valorUnidade, int? quantidade)
    {
        if (valor == 0)
        {
            throw new ArgumentException("Valor é obrigatório");
        }
        if (tipo.TipoCodigo == VerbaTipoCustoEnum.Fixo)
        {
            if (valorUnidade == null || valorUnidade == 0)
            {
                throw new ArgumentException("Valor da unidade é obrigatório");
            }

            if (quantidade == null || quantidade == 0)
            {
                throw new ArgumentException("Quantidade é obrigatório");
            }
        }
    }

    private void ValidaData(DateTime dataDespesa)
    {
        if (dataDespesa == DateTime.MinValue || dataDespesa == null)
        {
            throw new ArgumentException("Data invalida");
        }
    }

    public async Task<List<string>> GerarListaDeErrosReferenteAComprovante(List<SolicitacaoReembolsoDTO> input, int orgId)
    {
        List<string> erros = [];

        var parametroOrg = await _parametroReembolsoRepository.BuscarPorOrgIdAsync(orgId);
        var verbas = await _verbaRepository.ListarAsync(orgId);
        if (parametroOrg == null)
        {
            throw new ArgumentException("Organização ainda não tem uma configuração de reembolso.");
        }

        foreach (var solicitacao in input)
        {
            var verba = verbas.FirstOrDefault(x => x.Id == solicitacao.VerbaId);
            if (verba == null)
            {
                erros.Add("Uma das solicitações referencia uma verba inválida.");
                continue;
            }

            if (verba.ExigirComprovante && solicitacao.Arquivos.Count == 0)
            {
                erros.Add($"Categoria: ({verba.Categoria}), Insira pelo menos 1 arquivo.");
            }

            foreach (var arq in solicitacao.Arquivos)
            {
                if(verba.ExigirComprovante == false) continue;
                var t = arq.TokenArquivoTemp?.Trim();
                if (string.IsNullOrEmpty(t) || !Guid.TryParse(t, out _))
                    erros.Add($"Categoria: ({verba.Categoria}), Cada comprovante deve informar TokenArquivoTemp (GUID válido) obtido no upload prévio.");
            }
            
            // Manter logica aqui, futuramente ter essa opção de configuração no parametro? parametroOrg.impedirSolicitacaoDataExcedente
            // if (parametroOrg.ValidadeComprovanteDias > 0)
            // {
            //     var dataLimite = DateTime.Now.AddDays(-parametroOrg.ValidadeComprovanteDias);
            //     if (solicitacao.DataDespesa < dataLimite)
            //     {
            //         erros.Add($"Categoria: ({categoria}), Data do comprovante excedeu {parametroOrg.ValidadeComprovanteDias} dias.");
            //     }
            // }
        }

        return erros;
    }
}