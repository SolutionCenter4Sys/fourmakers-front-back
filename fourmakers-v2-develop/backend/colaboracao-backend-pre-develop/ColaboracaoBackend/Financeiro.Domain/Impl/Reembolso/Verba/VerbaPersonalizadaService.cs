using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Enum;
using Core.Domain.Reembolso.Verba;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;
using Financeiro.Domain.Interfaces.Reembolso.Validadores;
using Financeiro.Domain.Interfaces.Reembolso.Verba;
using Logs.Infra.Attributes;

namespace Financeiro.Domain.Services.Verba;

[LogDomainClass]
public class VerbaPersonalizadaService : IVerbaPersonalizadaService
{
    private readonly IVerbaPersonalizadaRepository _verbaPersonalizadaRepository;
    private readonly IDBConnectionUnitOfWork _dbConnectionUnitOfWork;
    private readonly IVerbaPersonalizadaValidadorService _verbaPersonalizadaValidadorService;
    private readonly IVerbaRepository _verbaRepository;

    public VerbaPersonalizadaService(IVerbaPersonalizadaRepository verbaPersonalizadaRepository, IDBConnectionUnitOfWork dbConnectionUnitOfWork, IVerbaPersonalizadaValidadorService verbaPersonalizadaValidadorService, IVerbaRepository verbaRepository)
    {
        _verbaPersonalizadaRepository = verbaPersonalizadaRepository;
        _dbConnectionUnitOfWork = dbConnectionUnitOfWork;
        _verbaPersonalizadaValidadorService = verbaPersonalizadaValidadorService;
        _verbaRepository = verbaRepository;
    }

    public async Task<ApiGenericResult<List<SimpleColaboradorDTO>>> ListarColaboradoresAlocadosPorClienteOuProjeto(int orgId, string? clienteId, string? projetoId)
    {
        var apiResult = new ApiGenericResult<List<SimpleColaboradorDTO>>();
        try
        {
            apiResult.Retorno = await _verbaPersonalizadaRepository.ListarColaboradoresAlocadosPorClienteOuProjeto(orgId, clienteId, projetoId);
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Read, "Colaboradores");
        }
        return apiResult;
    }
    
    public async Task<ApiGenericResult<List<VerbaPersonalizadaCompletaDTO>>> ListarVerbasPersonalizadasPorOrg(int orgId, string? clienteId, string? projetoId)
    {
        var apiResult = new ApiGenericResult<List<VerbaPersonalizadaCompletaDTO>>();
        try
        {
            //Garantir que o reposit�rio s� receba o projetoId caso receba projeto e client do front end
            if (projetoId != null && clienteId != null)
            {
                clienteId = null;
            }
            var verbasPersonalizadas = await _verbaPersonalizadaRepository.ListarVerbasPersonalizadasPorOrg(orgId, clienteId, projetoId);
            var listarTodasAsVerbasGerais = await _verbaRepository.ListarAsync(orgId);
            var listaAgrupada = GerarListaOrganizadaEAgrupada(verbasPersonalizadas);
            
            var verbasGeraisNaoCriadas = listarTodasAsVerbasGerais
                .Where(v => listaAgrupada.All(va => va.VerbaGeral.Verba.Id != v.Id))
                .Select(x => new VerbaPersonalizadaCompletaDTO()
                {
                    VerbaGeral = new () {
                        Ativo = false,
                        ProjetoId = null,
                        ClienteId = null,
                        CustoCliente = x.CustoCliente,
                        Valor = x.Valor,
                        Id = 0,
                        Verba = new ()
                        {
                            Id = x.Id,
                            Categoria = x.Categoria,
                        },
                        VerbaTipo = new ()
                        {
                            Id = x.TipoCusto,
                            Descricao = x.TipoCustoDescricao
                        }
                    },
                    ExcecoesColaborador = []
                }).ToList();
            
            //adicionando regras nao criadas para apresentacao no front
            listaAgrupada.AddRange(verbasGeraisNaoCriadas);


            apiResult.Retorno = listaAgrupada;
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Read, "Colaboradores");
        }
        return apiResult;
    }

    public async Task<ApiGenericResult<List<VerbaPersonalizadaCompletaDTO>>> InserirListaDeVerbasPersonalizadas(InserirVerbaPersonalizadaParam param, int orgId)
    {
        var apiResult = new ApiGenericResult<List<VerbaPersonalizadaCompletaDTO>>();
        _dbConnectionUnitOfWork.BeginTransaction();
        try
        {
            var verbas = new List<VerbaPersonalizadaColaboradorDTO>();
            foreach (var verbasCustomizadas in param.VerbasCustomizadas)
            {
                verbasCustomizadas.VerbaGeral.ClienteId = param.ClienteId;
                verbasCustomizadas.VerbaGeral.ProjetoId = param.ProjetoId;
                var validaVerbaGeral = ConverterVerbaPersonalizadaDTOParaInput(verbasCustomizadas.VerbaGeral);
                    
                if (validaVerbaGeral.Id is null || validaVerbaGeral.Id == 0)
                {
                    var insertResult = await InserirVerbaPersonalizada(validaVerbaGeral, orgId);
                    verbas.Add(insertResult.Retorno);
                }
                else
                {
                    var updateResult = await EditarVerbaPersonalizada(validaVerbaGeral, orgId);
                    verbas.Add(updateResult.Retorno);
                }
                
                foreach (var verbaColaborador in verbasCustomizadas.ExcecoesColaborador)
                {
                    verbaColaborador.ClienteId = param.ClienteId;
                    verbaColaborador.ProjetoId = param.ProjetoId;
                    var validaExcecaoColaborador = ConverterVerbaPersonalizdaColaboradorDTOParaInput(verbaColaborador);
                    
                    if (validaExcecaoColaborador.Id is null || validaExcecaoColaborador.Id == 0)
                    {
                        var insertResult = await InserirVerbaPersonalizada(validaExcecaoColaborador, orgId);
                        verbas.Add(insertResult.Retorno);
                    }
                    else
                    {
                        var updateResult = await EditarVerbaPersonalizada(validaExcecaoColaborador, orgId);
                        verbas.Add(updateResult.Retorno);
                    }
                }

            }

            apiResult.Retorno = GerarListaOrganizadaEAgrupada(verbas);
            _dbConnectionUnitOfWork.Commit();
        }
        catch (Exception e)
        {
            _dbConnectionUnitOfWork.Rollback();
            throw;
        }
        return apiResult;
    }

    private VerbaPersonalizadaInput ConverterVerbaPersonalizadaDTOParaInput(VerbaPersonalizadaDTO verba)
    {
        var objeto = new VerbaPersonalizadaInput()
        {
            VerbaId = verba.Verba.Id,
            CustoCliente = verba.CustoCliente,
            Valor = verba.Valor,
            Id = verba.Id,
            ClienteId = verba.ClienteId,
            ProjetoId = verba.ProjetoId,
            Ativo = verba.Ativo,
        };

        return objeto;
    }
    
    private VerbaPersonalizadaInput ConverterVerbaPersonalizdaColaboradorDTOParaInput(VerbaPersonalizadaColaboradorDTO verba)
    {
        if (verba.Colaborador == null || string.IsNullOrEmpty(verba.Colaborador.Cpf))
        {
            throw new ArgumentException("Necessario informar um Colaborador");
        }
        var objeto = new VerbaPersonalizadaInput()
        {
            VerbaId = verba.Verba.Id,
            CustoCliente = verba.CustoCliente,
            Valor = verba.Valor,
            Id = verba.Id,
            ClienteId = verba.ClienteId,
            ProjetoId = verba.ProjetoId,
            Ativo = verba.Ativo,
            CodigoInternoColaborador = verba.Colaborador.Cpf
        };
        
        return objeto;
    }

    public async Task<ApiGenericResult<VerbaPersonalizadaColaboradorDTO>> InserirVerbaPersonalizada(VerbaPersonalizadaInput verbaPersonalizada, int orgId)
    {
        var apiResult = new ApiGenericResult<VerbaPersonalizadaColaboradorDTO>();
        try
        {
            await _verbaPersonalizadaValidadorService.ValidaVerbaPersonalizada(verbaPersonalizada, CRUDEnum.Create, orgId);
            var insert = await _verbaPersonalizadaRepository.InserirVerbaPersonalizada(orgId, verbaPersonalizada.CodigoInternoColaborador, verbaPersonalizada.VerbaId, verbaPersonalizada.ProjetoId, verbaPersonalizada.ClienteId, verbaPersonalizada.Valor, verbaPersonalizada.Ativo, verbaPersonalizada.CustoCliente);
            apiResult.Retorno = insert;
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Create, "Verba Personalizada");
        }
        return apiResult;
    }
    
    public async Task<ApiGenericResult<VerbaPersonalizadaColaboradorDTO>> EditarVerbaPersonalizada(VerbaPersonalizadaInput verbaPersonalizada, int orgId)
    {
        var apiResult = new ApiGenericResult<VerbaPersonalizadaColaboradorDTO>();
        try
        {
            await _verbaPersonalizadaValidadorService.ValidaVerbaPersonalizada(verbaPersonalizada, CRUDEnum.Update, orgId);
            var update = await _verbaPersonalizadaRepository.EditarVerbaPersonalizada(verbaPersonalizada.Id.Value, verbaPersonalizada.CodigoInternoColaborador, verbaPersonalizada.Valor, verbaPersonalizada.Ativo, verbaPersonalizada.CustoCliente);
            apiResult.Retorno = update;
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Update, "Verba Personalizada");
        }
        return apiResult;
    }
    
    public async Task<ApiGenericResult<bool>> InativarVerbaPersonalizada(VerbaPersonalizadaInput verbaPersonalizada, int orgId)
    {
        var apiResult = new ApiGenericResult<bool>();
        try
        {
            await _verbaPersonalizadaValidadorService.ValidaVerbaPersonalizada(verbaPersonalizada, CRUDEnum.Delete, orgId);
            apiResult.Retorno = await _verbaPersonalizadaRepository.InativarVerbaPersonalizadaPorId(verbaPersonalizada.Id.Value);
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Update, "Verba Personalizada");
        }
        return apiResult;
    }

    private List<VerbaPersonalizadaCompletaDTO> GerarListaOrganizadaEAgrupada(List<VerbaPersonalizadaColaboradorDTO> colaboradores)
    {
        var listaRetorno = new List<VerbaPersonalizadaCompletaDTO>();
        var listaDeVerbasGerais = colaboradores.Where(x => x.Colaborador == null).ToList();
        foreach (var verbaGeral in listaDeVerbasGerais)
        {
            var itemList = new VerbaPersonalizadaCompletaDTO();
            itemList.VerbaGeral = verbaGeral;
            itemList.ExcecoesColaborador = colaboradores.Where(x => x.Colaborador != null && x.Verba.Id == verbaGeral.Verba.Id).ToList();
            listaRetorno.Add(itemList);
        }
        return listaRetorno;
    }
}