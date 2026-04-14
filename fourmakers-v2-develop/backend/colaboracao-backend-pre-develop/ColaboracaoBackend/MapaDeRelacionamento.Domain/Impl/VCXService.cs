using Core.Domain.Organograma;
using Core.Domain.VCX;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeRelacionamento.VCX;
using Logs.Infra.Attributes;
using MapaDeRelacionamento.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace MapaDeRelacionamento.Domain.Impl;

[LogDomainClass]
public class VCXService : IVCXService
{
    private readonly IVCXRepository _vcxRepository;
    private readonly IOrganogramaRepository _organogramaRepository;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = false };

    public VCXService(IVCXRepository vcxRepository, IOrganogramaRepository organogramaRepository)
    {
        _vcxRepository = vcxRepository;
        _organogramaRepository = organogramaRepository;
    }

    public async Task<ApiGenericResult<IEnumerable<VCXDorDTO>>> GetDoresByPosicaoIdAsync(Guid posicaoId)
    {
        var result = new ApiGenericResult<IEnumerable<VCXDorDTO>>();
        
        result.Retorno = await _vcxRepository.GetDoresByPosicaoIdAsync(posicaoId);
        return result;
    }

    public async Task<ApiGenericResult<IEnumerable<VCXIniciativaDTO>>> GetIniciativasByPosicaoIdAsync(Guid posicaoId, int orgId)
    {
        var result = new ApiGenericResult<IEnumerable<VCXIniciativaDTO>>();
        
        result.Retorno = await _vcxRepository.GetIniciativasByPosicaoIdAsync(posicaoId, orgId);
        return result;
    }

    public async Task<ApiGenericResult<VCXPosicaoDoresIniciativasResultDTO>> GetDoresAndIniciativasByPosicaoIdAsync(Guid posicaoId, int orgId)
    {
        var result = new ApiGenericResult<VCXPosicaoDoresIniciativasResultDTO>();
        
        result.Retorno = await _vcxRepository.GetDoresAndIniciativasByPosicaoIdAsync(posicaoId, orgId);
        return result;
    }

    public async Task<ApiGenericResult<VCXHistoricoDoresIniciativasResultDTO>> GetHistoricoDoresIniciativasByPosicaoIdAsync(Guid posicaoId)
    {
        var result = new ApiGenericResult<VCXHistoricoDoresIniciativasResultDTO>();

        result.Retorno = await _vcxRepository.GetHistoricoDoresIniciativasByPosicaoIdAsync(posicaoId);
        return result;
    }

    public async Task<ApiGenericResult<VCXDorDTO>> GetDorByIdAsync(Guid id)
    {
        var result = new ApiGenericResult<VCXDorDTO>();

        var dor = await _vcxRepository.GetDorByIdAsync(id);
        if (dor == null)
            throw new InvalidOperationException("Dor não encontrada.");
        result.Retorno = dor;
        return result;
    }

    public async Task<ApiGenericResult<VCXDorDTO>> CreateDorAsync(VCXDorInputDTO input, string codigoInternoColaboradorAlterador)
    {
        var result = new ApiGenericResult<VCXDorDTO>();
        ValidarInputDor(input);

        var id = await _vcxRepository.InsertDorAsync(input);
        var created = await _vcxRepository.GetDorByIdAsync(id);
        if (created == null) throw new InvalidOperationException("Dor não encontrada após inserção.");

        await _vcxRepository.InserirDoresLogAsync(new VCXLogDTO
        {
            ColaboradorCodigoInternoColaboradorAlterador = Guid.Parse(codigoInternoColaboradorAlterador),
            OrganogramaPosicaoId = input.OrganogramaPosicaoId,
            Acao = "INSERT",
            Objeto = null,
            Alteracao = JsonSerializer.Serialize(created, JsonOptions)
        });

        result.Retorno = created;
        return result;
    }

    public async Task<ApiGenericResult<VCXDorDTO>> UpdateDorAsync(VCXDorInputDTO input, string codigoInternoColaboradorAlterador)
    {
        var result = new ApiGenericResult<VCXDorDTO>();
        if (input?.Id == null || input.Id == Guid.Empty)
            throw new ArgumentException("O Id da Dor é obrigatório para atualização.", nameof(input));
        ValidarInputDor(input);

        var id = input.Id.Value;
        var antigo = await _vcxRepository.GetDorByIdAsync(id);
        if (antigo == null) throw new InvalidOperationException("Dor não encontrada.");

        await _vcxRepository.UpdateDorAsync(id, input);
        var atualizado = await _vcxRepository.GetDorByIdAsync(id);
        if (atualizado == null) throw new InvalidOperationException("Dor não encontrada após atualização.");

        await _vcxRepository.InserirDoresLogAsync(new VCXLogDTO
        {
            ColaboradorCodigoInternoColaboradorAlterador = Guid.Parse(codigoInternoColaboradorAlterador),
            OrganogramaPosicaoId = input.OrganogramaPosicaoId,
            Acao = "UPDATE",
            Objeto = JsonSerializer.Serialize(antigo, JsonOptions),
            Alteracao = JsonSerializer.Serialize(atualizado, JsonOptions)
        });

        result.Retorno = atualizado;
        return result;
    }

    public async Task<ApiGenericResult> DeleteDorAsync(Guid id, string codigoInternoColaboradorAlterador)
    {
        var result = new ApiGenericResult();

        var antigo = await _vcxRepository.GetDorByIdAsync(id);
        if (antigo == null) throw new InvalidOperationException("Dor não encontrada.");

        var posicaoId = antigo.OrganogramaPosicaoId;
        await _vcxRepository.DeleteDorAsync(id);

        await _vcxRepository.InserirDoresLogAsync(new VCXLogDTO
        {
            ColaboradorCodigoInternoColaboradorAlterador = Guid.Parse(codigoInternoColaboradorAlterador),
            OrganogramaPosicaoId = posicaoId,
            Acao = "DELETE",
            Objeto = JsonSerializer.Serialize(antigo, JsonOptions),
            Alteracao = null
        });

        return result;
    }

    public async Task<ApiGenericResult<VCXIniciativaDTO>> GetIniciativaByIdAsync(Guid id, int orgId)
    {
        var result = new ApiGenericResult<VCXIniciativaDTO>();
        
        var iniciativa = await _vcxRepository.GetIniciativaByIdAsync(id, orgId);
        if (iniciativa == null)
            throw new InvalidOperationException("Iniciativa não encontrada.");
        result.Retorno = iniciativa;
        return result;
    }

    public async Task<ApiGenericResult<VCXIniciativaDTO>> CreateIniciativaAsync(VCXIniciativaInputDTO input, string codigoInternoColaboradorAlterador, int orgId)
    {
        var result = new ApiGenericResult<VCXIniciativaDTO>();
        ValidarInputIniciativa(input);

        var id = await _vcxRepository.InsertIniciativaAsync(input);
        var created = await _vcxRepository.GetIniciativaByIdAsync(id, orgId);
        if (created == null) throw new InvalidOperationException("Iniciativa não encontrada após inserção.");

        await _vcxRepository.InserirIniciativasLogAsync(new VCXLogDTO
        {
            ColaboradorCodigoInternoColaboradorAlterador = Guid.Parse(codigoInternoColaboradorAlterador),
            OrganogramaPosicaoId = input.OrganogramaPosicaoId,
            Acao = "INSERT",
            Objeto = null,
            Alteracao = JsonSerializer.Serialize(created, JsonOptions)
        });

        result.Retorno = created;
        return result;
    }

    public async Task<ApiGenericResult<VCXIniciativaDTO>> UpdateIniciativaAsync(VCXIniciativaInputDTO input, string codigoInternoColaboradorAlterador, int orgId)
    {
        var result = new ApiGenericResult<VCXIniciativaDTO>();
        if (input?.Id == null || input.Id == Guid.Empty)
            throw new ArgumentException("O Id da Iniciativa é obrigatório para atualização.", nameof(input));
        ValidarInputIniciativa(input);

        var id = input.Id.Value;
        var antigo = await _vcxRepository.GetIniciativaByIdAsync(id, orgId);
        if (antigo == null) throw new InvalidOperationException("Iniciativa não encontrada.");

        await _vcxRepository.UpdateIniciativaAsync(id, input);
        var atualizado = await _vcxRepository.GetIniciativaByIdAsync(id, orgId);
        if (atualizado == null) throw new InvalidOperationException("Iniciativa não encontrada após atualização.");

        await _vcxRepository.InserirIniciativasLogAsync(new VCXLogDTO
        {
            ColaboradorCodigoInternoColaboradorAlterador = Guid.Parse(codigoInternoColaboradorAlterador),
            OrganogramaPosicaoId = input.OrganogramaPosicaoId,
            Acao = "UPDATE",
            Objeto = JsonSerializer.Serialize(antigo, JsonOptions),
            Alteracao = JsonSerializer.Serialize(atualizado, JsonOptions)
        });

        result.Retorno = atualizado;
        return result;
    }

    public async Task<ApiGenericResult> DeleteIniciativaAsync(Guid id, string codigoInternoColaboradorAlterador, int orgId)
    {
        var result = new ApiGenericResult();

        var antigo = await _vcxRepository.GetIniciativaByIdAsync(id, orgId);
        if (antigo == null) throw new InvalidOperationException("Iniciativa não encontrada.");

        var posicaoId = antigo.OrganogramaPosicaoId;
        await _vcxRepository.DeleteIniciativaAsync(id);

        await _vcxRepository.InserirIniciativasLogAsync(new VCXLogDTO
        {
            ColaboradorCodigoInternoColaboradorAlterador = Guid.Parse(codigoInternoColaboradorAlterador),
            OrganogramaPosicaoId = posicaoId,
            Acao = "DELETE",
            Objeto = JsonSerializer.Serialize(antigo, JsonOptions),
            Alteracao = null
        });

        return result;
    }

    public async Task<ApiGenericResult<IEnumerable<VCXNotaBastidoresDTO>>> GetNotasBastidoresByPosicaoIdAsync(Guid posicaoId, int orgId)
    {
        var result = new ApiGenericResult<IEnumerable<VCXNotaBastidoresDTO>>();
        result.Retorno = await _vcxRepository.GetNotasBastidoresByPosicaoIdAsync(posicaoId, orgId);
        return result;
    }

    public async Task<ApiGenericResult<VCXNotaBastidoresDTO>> GetNotaBastidoresByIdAsync(Guid id, int orgId)
    {
        var result = new ApiGenericResult<VCXNotaBastidoresDTO>();
        var nota = await _vcxRepository.GetNotaBastidoresByIdAsync(id, orgId);
        if (nota == null)
            throw new InvalidOperationException("Nota de bastidores não encontrada.");
        result.Retorno = nota;
        return result;
    }

    public async Task<ApiGenericResult<VCXNotaBastidoresDTO>> CreateNotaBastidoresAsync(VCXNotaBastidoresCriarInputDTO input, string codigoInternoColaboradorAlterador, int orgId)
    {
        var result = new ApiGenericResult<VCXNotaBastidoresDTO>();
        ValidarInputNotaBastidoresCriar(input);

        var id = await _vcxRepository.InsertNotaBastidoresAsync(input);
        var created = await _vcxRepository.GetNotaBastidoresByIdAsync(id, orgId);
        if (created == null) throw new InvalidOperationException("Nota de bastidores não encontrada após inserção.");

        await _vcxRepository.InserirNotasBastidoresLogAsync(new VCXLogDTO
        {
            ColaboradorCodigoInternoColaboradorAlterador = Guid.Parse(codigoInternoColaboradorAlterador),
            OrganogramaPosicaoId = input.OrganogramaPosicaoId,
            Acao = "INSERT",
            Objeto = null,
            Alteracao = JsonSerializer.Serialize(created, JsonOptions)
        });

        result.Retorno = created;
        return result;
    }

    public async Task<ApiGenericResult<VCXNotaBastidoresDTO>> UpdateNotaBastidoresAsync(VCXNotaBastidoresAtualizarInputDTO input, string codigoInternoColaboradorAlterador, int orgId)
    {
        var result = new ApiGenericResult<VCXNotaBastidoresDTO>();
        if (input == null)
            throw new ArgumentException("Os dados da nota de bastidores são obrigatórios.", nameof(input));
        if (input.Id == Guid.Empty)
            throw new ArgumentException("O Id da nota de bastidores é obrigatório para atualização.", nameof(input));
        ValidarInputNotaBastidoresAtualizar(input);

        var id = input.Id;
        var antigo = await _vcxRepository.GetNotaBastidoresByIdAsync(id, orgId);
        if (antigo == null) throw new InvalidOperationException("Nota de bastidores não encontrada.");

        await _vcxRepository.UpdateNotaBastidoresAsync(id, input);
        var atualizado = await _vcxRepository.GetNotaBastidoresByIdAsync(id, orgId);
        if (atualizado == null) throw new InvalidOperationException("Nota de bastidores não encontrada após atualização.");

        await _vcxRepository.InserirNotasBastidoresLogAsync(new VCXLogDTO
        {
            ColaboradorCodigoInternoColaboradorAlterador = Guid.Parse(codigoInternoColaboradorAlterador),
            OrganogramaPosicaoId = antigo.OrganogramaPosicaoId,
            Acao = "UPDATE",
            Objeto = JsonSerializer.Serialize(antigo, JsonOptions),
            Alteracao = JsonSerializer.Serialize(atualizado, JsonOptions)
        });

        result.Retorno = atualizado;
        return result;
    }

    public async Task<ApiGenericResult> DeleteNotaBastidoresAsync(Guid id, string codigoInternoColaboradorAlterador, int orgId)
    {
        var result = new ApiGenericResult();

        var antigo = await _vcxRepository.GetNotaBastidoresByIdAsync(id, orgId);
        if (antigo == null) throw new InvalidOperationException("Nota de bastidores não encontrada.");

        var posicaoId = antigo.OrganogramaPosicaoId;
        await _vcxRepository.DeleteNotaBastidoresAsync(id);

        await _vcxRepository.InserirNotasBastidoresLogAsync(new VCXLogDTO
        {
            ColaboradorCodigoInternoColaboradorAlterador = Guid.Parse(codigoInternoColaboradorAlterador),
            OrganogramaPosicaoId = posicaoId,
            Acao = "DELETE",
            Objeto = JsonSerializer.Serialize(antigo, JsonOptions),
            Alteracao = null
        });

        return result;
    }

    public async Task<ApiGenericResult<IEnumerable<VCXNotaBastidoresLogDTO>>> GetHistoricoNotasBastidoresByPosicaoIdAsync(Guid posicaoId)
    {
        var result = new ApiGenericResult<IEnumerable<VCXNotaBastidoresLogDTO>>();
        result.Retorno = await _vcxRepository.GetHistoricoNotasBastidoresByPosicaoIdAsync(posicaoId);
        return result;
    }

    public async Task<ApiGenericResult<IEnumerable<VCXTemaDTO>>> GetTemasByOrgIdAsync(int orgId, string? descricao = null)
    {
        var result = new ApiGenericResult<IEnumerable<VCXTemaDTO>>();
        result.Retorno = await _vcxRepository.GetTemasByOrgIdAsync(orgId, descricao);
        return result;
    }

    public async Task<ApiGenericResult<VCXTemaDTO>> CreateTemaAsync(VCXTemaInputDTO input, int orgId)
    {
        var result = new ApiGenericResult<VCXTemaDTO>();
        ValidarInputTema(input);

        var id = await _vcxRepository.InsertTemaAsync(input.Descricao, orgId);
        result.Retorno = new VCXTemaDTO { Id = id, Descricao = input.Descricao, OrgId = orgId };
        return result;
    }

    public async Task<ApiGenericResult<IEnumerable<VCXImpactoDTO>>> GetImpactosAsync()
    {
        var result = new ApiGenericResult<IEnumerable<VCXImpactoDTO>>();
        result.Retorno = await _vcxRepository.GetImpactosAsync();
        return result;
    }

    public async Task<ApiGenericResult<IEnumerable<VCXUrgenciaDTO>>> GetUrgenciasAsync()
    {
        var result = new ApiGenericResult<IEnumerable<VCXUrgenciaDTO>>();
        result.Retorno = await _vcxRepository.GetUrgenciasAsync();
        return result;
    }

    public async Task<ApiGenericResult<IEnumerable<VCXStatusDTO>>> GetStatusAsync()
    {
        var result = new ApiGenericResult<IEnumerable<VCXStatusDTO>>();
        result.Retorno = await _vcxRepository.GetStatusAsync();
        return result;
    }

    public async Task<ApiGenericResult<VCXAgendasPorColaboradorClienteResultDTO>> GetAgendasPorColaboradorClienteAsync(string codigoColaborador, string codigoCliente, int cursor, int limit)
    {
        var result = new ApiGenericResult<VCXAgendasPorColaboradorClienteResultDTO>();
        result.Retorno = await _vcxRepository.GetAgendasPorColaboradorClienteAsync(codigoColaborador, codigoCliente, cursor, limit);
        return result;
    }

    public async Task<ApiGenericResult<List<OrganogramaPosicaoOrcamentoHistoricoResponseDTO>>> ListarOrcamentoHistoricoPorPosicaoIdAsync(Guid posicaoId, int orgId)
    {
        var posicaoIdStr = posicaoId.ToString();
        var posicao = await _organogramaRepository.PosicaoListaPorId(posicaoIdStr);
        if (posicao == null || posicao.OrgId != orgId)
            throw new InvalidOperationException("Posição no Organograma não localizada.");

        var result = new ApiGenericResult<List<OrganogramaPosicaoOrcamentoHistoricoResponseDTO>>();
        result.Retorno = await _vcxRepository.PosicaoOrcamentoHistoricoListarPorPosicao(posicaoIdStr);
        return result;
    }

    public async Task<ApiGenericResult<OrganogramaPosicaoOrcamentoHistoricoResponseDTO?>> BuscarUltimoOrcamentoPorPosicaoIdAsync(Guid posicaoId, int orgId)
    {
        var posicaoIdStr = posicaoId.ToString();
        var posicao = await _organogramaRepository.PosicaoListaPorId(posicaoIdStr);
        if (posicao == null || posicao.OrgId != orgId)
            throw new InvalidOperationException("Posição no Organograma não localizada.");

        var ultimo = await _vcxRepository.PosicaoOrcamentoHistoricoObterUltimoPorPosicao(posicaoIdStr);
        var result = new ApiGenericResult<OrganogramaPosicaoOrcamentoHistoricoResponseDTO?>();
        result.Retorno = ultimo;
        return result;
    }

    public async Task<ApiGenericResult<OrganogramaPosicaoOrcamentoHistoricoResponseDTO>> CriarOrcamentoHistoricoPorPosicaoAsync(Guid posicaoId, OrganogramaPosicaoOrcamentoHistoricoInserirParamDTO param, string codigoInternoColaboradorAlterador, int orgId)
    {
        if (param == null)
            throw new ArgumentException("Os dados do orçamento são obrigatórios.", nameof(param));
        if (string.IsNullOrWhiteSpace(codigoInternoColaboradorAlterador))
            throw new InvalidOperationException("codigo_interno_colaborador é obrigatório para registrar orçamento da posição.");

        var posicaoIdStr = posicaoId.ToString();
        var posicao = await _organogramaRepository.PosicaoListaPorId(posicaoIdStr);
        if (posicao == null || posicao.OrgId != orgId)
            throw new InvalidOperationException("Posição no Organograma não localizada.");

        var id = Guid.NewGuid().ToString();
        await _vcxRepository.InserirHistoricoOrcamentoPosicao(id, posicaoIdStr, orgId, param.Orcamento, param.DataInicio, param.DataFim, codigoInternoColaboradorAlterador);

        var criado = await _vcxRepository.PosicaoOrcamentoHistoricoObterUltimoPorPosicao(posicaoIdStr);
        if (criado == null)
            throw new InvalidOperationException("Orçamento registrado mas não foi possível carregar o histórico.");

        var result = new ApiGenericResult<OrganogramaPosicaoOrcamentoHistoricoResponseDTO>();
        result.Retorno = criado;
        return result;
    }

    private static void ValidarInputDor(VCXDorInputDTO input)
    {
        if (input == null)
            throw new ArgumentException("Os dados da dor são obrigatórios.", nameof(input));
        if (input.OrganogramaPosicaoId == Guid.Empty)
            throw new ArgumentException("O campo OrganogramaPosicaoId é obrigatório.", nameof(input));
        if (string.IsNullOrWhiteSpace(input.Titulo))
            throw new ArgumentException("O campo Titulo é obrigatório.", nameof(input));
        if (input.Titulo.Length > 255)
            throw new ArgumentException("O campo Titulo deve ter no máximo 255 caracteres.", nameof(input));
        if (input.Descricao != null && input.Descricao.Length > 65535)
            throw new ArgumentException("O campo Descricao deve ter no máximo 65535 caracteres.", nameof(input));
    }

    private static void ValidarInputIniciativa(VCXIniciativaInputDTO input)
    {
        if (input == null)
            throw new ArgumentException("Os dados da iniciativa são obrigatórios.", nameof(input));
        if (input.OrganogramaPosicaoId == Guid.Empty)
            throw new ArgumentException("O campo OrganogramaPosicaoId é obrigatório.", nameof(input));
        if (string.IsNullOrWhiteSpace(input.Titulo))
            throw new ArgumentException("O campo Titulo é obrigatório.", nameof(input));
        if (input.Titulo.Length > 255)
            throw new ArgumentException("O campo Titulo deve ter no máximo 255 caracteres.", nameof(input));
        if (input.Descricao != null && input.Descricao.Length > 65535)
            throw new ArgumentException("O campo Descricao deve ter no máximo 65535 caracteres.", nameof(input));
    }

    private static void ValidarInputTema(VCXTemaInputDTO input)
    {
        if (input == null)
            throw new ArgumentException("Os dados do tema são obrigatórios.", nameof(input));
        if (string.IsNullOrWhiteSpace(input.Descricao))
            throw new ArgumentException("O campo Descricao é obrigatório.", nameof(input));
        if (input.Descricao.Length > 65535)
            throw new ArgumentException("O campo Descricao deve ter no máximo 65535 caracteres.", nameof(input));
    }

    private static void ValidarInputNotaBastidoresCriar(VCXNotaBastidoresCriarInputDTO input)
    {
        if (input == null)
            throw new ArgumentException("Os dados da nota de bastidores são obrigatórios.", nameof(input));
        if (input.OrganogramaPosicaoId == Guid.Empty)
            throw new ArgumentException("O campo OrganogramaPosicaoId é obrigatório.", nameof(input));
        ValidarDescricaoNotaBastidores(input.Descricao);
    }

    private static void ValidarInputNotaBastidoresAtualizar(VCXNotaBastidoresAtualizarInputDTO input)
    {
        ValidarDescricaoNotaBastidores(input.Descricao);
    }

    private static void ValidarDescricaoNotaBastidores(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("O campo Descricao é obrigatório.", nameof(descricao));
        if (descricao.Length > 2000)
            throw new Exception("A nota nao pode ter mais que 2000 caracteres");
    }
}
