using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.SRS.AdmissaoPipeline;

namespace Admissao.Domain.Interfaces.Service;

public interface IAdmissaoPipelineValidatorService
{
    Task ValidarAcessoListarAsync(string cpf, int orgId);
    Task ValidarAcessoCriarEditarAsync(string cpf, int orgId);

    /// <summary>
    /// Valida regras de negócio puras (estrutura dos itens) e existência de cada status na organização.
    /// Lança <see cref="InvalidOperationException"/> com a mensagem de erro quando inválido.
    /// Não faz nada quando <paramref name="itens"/> é <c>null</c>.
    /// </summary>
    Task ValidarStatusItensAsync(int orgId, IReadOnlyList<AdmissaoPipelineStatusItemInput> itens);
}
