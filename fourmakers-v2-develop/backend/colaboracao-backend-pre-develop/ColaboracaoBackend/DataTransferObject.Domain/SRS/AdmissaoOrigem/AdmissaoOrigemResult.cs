using System;

namespace DataTransferObject.Domain.SRS.AdmissaoOrigem;

/// <summary>Retorno do vínculo entre admissão e vaga/candidatura.</summary>
public class AdmissaoOrigemResult
{
    public Guid Id { get; set; }

    public Guid AdmissaoId { get; set; }

    public AdmissaoOrigemTipoEnum OrigemTipo { get; set; }

    /// <summary>ID da vaga (tb_vaga.id). Preenchido quando OrigemTipo = VAGA.</summary>
    public Guid? TbVagaId { get; set; }

    /// <summary>ID da candidatura (tb_candidato_vaga.id). Preenchido quando OrigemTipo = CANDIDATURA.</summary>
    public string TbCandidatoVagaId { get; set; }

    public DateTime DataCriacao { get; set; }
}
