using Core.Domain.Social.AtendimentoFourmakers;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using Logs.Infra.Attributes;
using Social.Domain.Interfaces.AtendimentoFourmakers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Social.Domain.Impl.AtendimentoFourmakers
{
    [LogDomainClass]
    public class MaterialAreaService : IMaterialAreaService
    {
        private readonly IMaterialAreaRepository _materialAreaRepository;
        private readonly IKbFonteMetaRepository _fonteMetaRepository;

        public MaterialAreaService(IMaterialAreaRepository materialAreaRepository, IKbFonteMetaRepository fonteMetaRepository)
        {
            _materialAreaRepository = materialAreaRepository;
            _fonteMetaRepository = fonteMetaRepository;
        }

        public async Task<ApiGenericResult<List<MaterialAreaResult>>> ListarAsync(int orgId)
        {
            var result = new ApiGenericResult<List<MaterialAreaResult>>();
            var areas = await _materialAreaRepository.ListarAsync(orgId);
            result.Retorno = areas.ToList();
            return result;
        }

        public async Task<ApiGenericResult<MaterialAreaResult>> CriarAsync(CriarMaterialAreaInput input, int orgId)
        {
            var result = new ApiGenericResult<MaterialAreaResult>();

            if (string.IsNullOrWhiteSpace(input?.Nome))
                throw new ArgumentException("Nome é obrigatório.");

            var nome = input.Nome.Trim();
            var slug = string.IsNullOrWhiteSpace(input.Slug) ? GerarSlug(nome) : GerarSlug(input.Slug);
            var ordem = input.Ordem ?? 0;

            if (await _materialAreaRepository.SlugExisteAsync(slug, orgId))
                throw new InvalidOperationException("Já existe uma área com este slug.");

            var id = await _materialAreaRepository.InserirAsync(new MaterialAreaInsertInput
            {
                Nome = nome,
                Slug = slug,
                Ordem = ordem
            }, orgId);

            result.Retorno = await _materialAreaRepository.ObterPorIdAsync(id, orgId);
            return result;
        }

        public async Task<ApiGenericResult<MaterialAreaResult>> AtualizarAsync(string id, AtualizarMaterialAreaInput input, int orgId)
        {
            var result = new ApiGenericResult<MaterialAreaResult>();

            if (input == null || (input.Nome == null && input.Slug == null && !input.Ordem.HasValue))
                throw new ArgumentException("Pelo menos um campo deve ser fornecido.");

            var existente = await _materialAreaRepository.ObterPorIdAsync(id, orgId);
            if (existente == null)
                throw new ApplicationException("Área não encontrada.");

            var updateInput = new MaterialAreaUpdateInput();
            if (input.Nome != null) updateInput.Nome = input.Nome.Trim();
            if (input.Slug != null) updateInput.Slug = GerarSlug(input.Slug);
            if (input.Ordem.HasValue) updateInput.Ordem = input.Ordem;

            if (updateInput.Slug != null && await _materialAreaRepository.SlugExisteAsync(updateInput.Slug, orgId, id))
                throw new InvalidOperationException("Já existe uma área com este slug.");

            await _materialAreaRepository.AtualizarAsync(id, updateInput, orgId);
            result.Retorno = await _materialAreaRepository.ObterPorIdAsync(id, orgId);
            return result;
        }

        public async Task<ApiGenericResult<bool>> RemoverAsync(string id, int orgId)
        {
            var result = new ApiGenericResult<bool>();

            var existente = await _materialAreaRepository.ObterPorIdAsync(id, orgId);
            if (existente == null)
                throw new ApplicationException("Área não encontrada.");

            var materiaisVinculados = await _fonteMetaRepository.ContarPorAreaAsync(id, orgId);
            if (materiaisVinculados > 0)
                throw new InvalidOperationException("Não é possível excluir: existem materiais vinculados.");

            result.Retorno = await _materialAreaRepository.DeletarAsync(id, orgId);
            return result;
        }

        private static string GerarSlug(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return "area";

            var normalized = texto.ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var semAcentos = new string(normalized
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());

            var slug = Regex.Replace(semAcentos, @"[^a-z0-9]+", "-").Trim('-');
            return string.IsNullOrEmpty(slug) ? "area" : slug;
        }
    }
}
