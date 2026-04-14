using System;
using DataTransferObject.Domain.SRS.AdmissaoOrigem;

namespace DataTransferObject.Domain.SRS.Admissao;

/// <summary>Retorno de admissão (<c>tb_admissao</c>).</summary>
public class AdmissaoResult
{
    public Guid Id { get; set; }

    public int TbOrgId { get; set; }

    public Guid AdmissaoPipelineId { get; set; }

    public Guid AdmissaoStatusId { get; set; }

    /// <summary>Código interno do colaborador alvo da admissão.</summary>
    public string CodigoInternoColaborador { get; set; }

    public DateTime DataInicio { get; set; }

    public DateTime? DataFim { get; set; }

    public string Observacao { get; set; }

    public bool Ativo { get; set; }

    public DateTime DataCriacao { get; set; }

    public DateTime DataAtualizacao { get; set; }

    // ── Dados enriquecidos (LEFT JOINs — nulos quando não há vínculo) ────────

    /// <summary>Nome completo do colaborador alvo (tb_colaborador.nome_completo).</summary>
    public string NomeColaborador { get; set; }

    /// <summary>
    /// ID da vaga de origem.
    /// Origem VAGA: id_vaga de tb_vagas_srs (convertido para string).
    /// Origem CANDIDATURA: id de tb_vaga.
    /// </summary>
    public string VagaId { get; set; }

    /// <summary>Título da vaga de origem (tb_vagas_srs.titulo ou tb_vaga.titulo).</summary>
    public string VagaTitulo { get; set; }

    /// <summary>
    /// ID do cliente (tb_cliente_org.id).
    /// Disponível apenas quando a origem é CANDIDATURA com tb_vaga vinculada a um gestor externo com cliente.
    /// </summary>
    public string ClienteId { get; set; }

    /// <summary>Nome do cliente (tb_cliente_org.nome_cliente).</summary>
    public string NomeCliente { get; set; }

    /// <summary>
    /// Vínculo de origem com vaga ou candidatura do módulo de recrutamento.
    /// Nulo quando a admissão foi criada sem o módulo de recrutamento.
    /// </summary>
    public AdmissaoOrigemResult Origem { get; set; }
}
