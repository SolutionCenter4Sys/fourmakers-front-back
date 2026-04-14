using Core.Domain.MapaAlocacao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.Tbd;
using MapaDeAlocacao.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace MapaDeAlocacao.Domain.Impl
{
    [LogDomainClass]
    public class TbdService : ITbdService
    {
        private ITbdRepository _tbdRepository;

        public TbdService(ITbdRepository tbdRepository)
        {
            _tbdRepository = tbdRepository;
        }
        public async Task<IEnumerable<TbdAlocacaoDTO>> ListarTbd(string codGestor, int orgId)
        {
            return await _tbdRepository.ListarTbd(codGestor, orgId);
        }

        public async Task<TbdAlocacaoDTO> ObterTbdPorCodigo(int orgId, int codTbd)
        {
            var tbd = await _tbdRepository.ObterTbdPorCodigo(orgId, codTbd);
            if (tbd == null)
            {
                throw new Exception("Erro: TBD não encontrado");
            }
            return tbd;
        }
        public async Task<TbdAlocacaoDTO> InserirTbd(TbdAlocacaoParam param, int orgId)
        {
            if (await ValidacaoGestorExistenteFalhou(param.CodGestor, orgId))
            {
                throw new Exception("Erro: Não é possível inserir. Motivo: Código do gestor inexistente ou não está ativo.");
            }

            return await _tbdRepository.InserirTbd(param, orgId);
        }
        public async Task<TbdAlocacaoDTO> AtualizarTbd(TbdAlocacaoParam param, int orgId)
        {
            return await _tbdRepository.AtualizarTbd(param, orgId);
        }
        public async Task<StatusResult> DeletarTbd(int codTbd, int orgId)
        {
            if (!ExisteAlocacaoNoTbd(codTbd, orgId))
            {
                return await _tbdRepository.DeletarTbd(codTbd, orgId);
            }

            throw new ArgumentException("Só é possível excluir TBD sem alocação");
        }
        private async Task<bool> ValidacaoGestorExistenteFalhou(string codGestor, int orgId)
        {
            return !await _tbdRepository.ExisteColaboradorAtivo(codGestor, orgId);
        }
        public bool ExisteAlocacaoNoTbd(int codTbd, int orgId)
        {
            return _tbdRepository.ExisteAlocacaoNoTbd(codTbd, orgId);
        }
    }
}