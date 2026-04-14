using Colaboracao.Core.Interfaces;
using Core.Domain.Organograma;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Usuario;
using Organograma.Domain.Interfaces;
using System;
using System.Collections.Generic;

using Logs.Infra.Attributes;

namespace Organograma.Domain.Impl
{
    [LogDomainClass]
    public class OrganogramaService : IOrganogramaService
    {
        private readonly IOrganogramaRepository _organogramaRepository;
        private readonly IOrganogramaLogRepository _organogramaLogRepository;
        UsuarioLogadoDTO _usuarioLogado;

        public OrganogramaService(IOrganogramaRepository organogramaRepository, IOrganogramaLogRepository organogramaLogRepository, IAspNetUser aspNetUser)
        {
            _organogramaRepository = organogramaRepository;
            _organogramaLogRepository = organogramaLogRepository;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        //--------------------- Departamento
        public async Task<OrganogramaDeptoListarPorIdResponseDTO> InserirDepartamento(OrganogramaDeptoInserirParamDTO param)
        {
            var result = await _organogramaRepository.InserirDepartamento(param, _usuarioLogado.OrgId);

            if (result == null)
                throw new InvalidOperationException("Erro ao inserir Departamento no Organograma.");

            //Grava Log
            await RegistrarLogAsync(result, null, "INSERT", "DEPARTAMENTO");


            return result;
        }

        public async Task<OrganogramaDeptoListarPorIdResponseDTO> AtualizarDepartamento(OrganogramaDeptoAtualizarParamDTO param)
        {
            var resLog = await _organogramaRepository.ListaDepartamentoPorId(param.Id);

            if (resLog == null)
                throw new InvalidOperationException("Departamento no Organograma não localizado.");

            var result = await _organogramaRepository.AtualizarDepartamento(param, _usuarioLogado.OrgId);

            if (result == null)
                throw new InvalidOperationException("Erro ao atualizar Departamento no Organograma.");

            //Grava Log
            await RegistrarLogAsync(resLog, result, "UPDATE", "DEPARTAMENTO");

            return result;
        }

        public async Task<bool> DeletarDepartamento(string id)
        {
            var resLog = await _organogramaRepository.ListaDepartamentoPorId(id);

            if (resLog == null)
                throw new InvalidOperationException("Não existe este Departamento no Organograma.");

            var resPos = await _organogramaRepository.PosicaoListaPorId(resLog.OrganogramaPosicaoIdLider);

            if (resPos != null)
                throw new InvalidOperationException("Não é possível excluir Departamento, pois existe vínculo com a Posição.");

            bool result = await _organogramaRepository.DeletarDepartamento(id);

            if (result)
            {
                resLog.Ativo = false;

                //Grava Log
                await RegistrarLogAsync(resLog, null, "DELETE", "DEPARTAMENTO");

            }

            return result;
        }

        public async Task<OrganogramaDeptoListarPorIdResponseDTO> ListaDepartamentoPorId(string buscaId)
            => (OrganogramaDeptoListarPorIdResponseDTO)await _organogramaRepository.ListaDepartamentoPorId(buscaId);

        public async Task<List<OrganogramaDeptoListarPorIdResponseDTO>> ListarDepartamentoPorCliente(string codCliente)
            => await _organogramaRepository.ListarDepartamentoPorCliente(codCliente);

        //------------------- Posição
        public async Task<OrganogramaPosicaoListarPorIdResponseDTO> PosicaoInserir(OrganogramaPosicaoInserirParamDTO param)
        {
            ValidarMapaRelacionamentoInfluencia(param.MapaRelacionamentoInfluenciaId);

            var result = await _organogramaRepository.PosicaoInserir(param, _usuarioLogado.OrgId);

            if (result == null)
                throw new InvalidOperationException("Erro ao inserir Posição no Organograma.");

            var final = await _organogramaRepository.PosicaoListaPorId(result.Id);
            await RegistrarLogAsync(final, null, "INSERT", "POSICAO");

            return final;
        }

        public async Task<OrganogramaPosicaoListarPorIdResponseDTO> PosicaoAtualizar(OrganogramaPosicaoAtualizarParamDTO param)
        {
            ValidarMapaRelacionamentoInfluencia(param.MapaRelacionamentoInfluenciaId);

            var resLog = await _organogramaRepository.PosicaoListaPorId(param.Id);

            if (resLog == null)
                throw new InvalidOperationException("Posição no Organograma não localizado.");

            var result = await _organogramaRepository.PosicaoAtualizar(param, _usuarioLogado.OrgId);

            if (result == null)
                throw new InvalidOperationException("Erro ao atualizar Posição no Organograma.");

            var final = await _organogramaRepository.PosicaoListaPorId(param.Id);
            await RegistrarLogAsync(resLog, final, "UPDATE", "POSICAO");

            return final;
        }

        public async Task<bool> PosicaoDeletar(string id)
        {
            var resLog = await _organogramaRepository.PosicaoListaPorId(id);

            if (resLog == null)
                throw new InvalidOperationException("Não existe esta Posição no Organograma.");

            var resDep = await _organogramaRepository.ListaDepartamentoPorId(resLog.DepartamentoId);

            if (resDep != null)
                throw new InvalidOperationException("Não é possível excluir Posição, pois existe vínculo com Departamento.");

            bool result = await _organogramaRepository.PosicaoDeletar(id);

            if (result)
            {
                resLog.Ativo = false;

                //Grava Log
                await RegistrarLogAsync(resLog, null, "DELETE", "POSICAO");

            }

            return result;
        }
        public async Task<OrganogramaPosicaoListarPorIdResponseDTO> PosicaoListaPorId(string buscaId)
            => await _organogramaRepository.PosicaoListaPorId(buscaId);

        public async Task<List<OrganogramaPosicaoListarPorIdResponseDTO>> PosicaoListarPorCliente(string buscaId)
            => await _organogramaRepository.PosicaoListarPorCliente(buscaId);

        //------------------- Alocação
        public async Task<OrganogramaAlocacaoListarPorIdResponseDTO> AlocacaoInserir(OrganogramaAlocacaoInserirParamDTO param)
        {
            var result = await _organogramaRepository.AlocacaoInserir(param, _usuarioLogado.OrgId);

            if (result == null)
                throw new InvalidOperationException("Erro ao inserir Alocação no Organograma.");

            //Grava Log
            await RegistrarLogAsync(result, null, "INSERT", "ALOCACAO");

            return result;
        }

        public async Task<OrganogramaAlocacaoListarPorIdResponseDTO> AlocacaoAtualizar(OrganogramaAlocacaoAtualizarParamDTO param)
        {
            var resLog = await _organogramaRepository.AlocacaoListaPorId(param.Id);

            if (resLog == null)
                throw new InvalidOperationException("Alocação no Organograma não localizado.");

            var result = await _organogramaRepository.AlocacaoAtualizar(param, _usuarioLogado.OrgId);

            if (result == null)
                throw new InvalidOperationException("Erro ao atualizar Alocação no Organograma.");

            //Grava Log
            await RegistrarLogAsync(resLog, result, "UPDATE", "ALOCACAO");

            return result;
        }
        public async Task<bool> AlocacaoDeletar(string id)
        {
            var resLog = await _organogramaRepository.AlocacaoListaPorId(id);

            if (resLog == null)
                throw new InvalidOperationException("Não existe esta Alocação no Organograma.");

            bool result = await _organogramaRepository.AlocacaoDeletar(id);

            if (result)
            {
                resLog.Ativo = false;

                //Grava Log
                await RegistrarLogAsync(resLog, null, "DELETE", "ALOCACAO");

            }

            return result;
        }
        public async Task<OrganogramaAlocacaoListarPorIdResponseDTO> AlocacaoListaPorId(string buscaId)
            => await _organogramaRepository.AlocacaoListaPorId(buscaId);

        //------------------- Perfil Corporativo Alocação
        public async Task<OrganogramaPerfilCorporativoAlocacaoListarPorIdResponseDTO> InserirPerfilCorporativoAlocacao(OrganogramaPerfilCorporativoAlocacaoInserirParamDTO param)
        {
            var result = await _organogramaRepository.InserirPerfilCorporativoAlocacao(param, _usuarioLogado.OrgId);

            if (result == null)
                throw new InvalidOperationException("Erro ao inserir alocação de Perfil Corporativo.");

            await RegistrarLogAsync(result, null, "INSERT", "PERFIL_CORP_ALOC");

            return result;
        }

        public async Task<OrganogramaPerfilCorporativoAlocacaoListarPorIdResponseDTO> AtualizarPerfilCorporativoAlocacao(OrganogramaPerfilCorporativoAlocacaoAtualizarParamDTO param)
        {
            var resLog = await _organogramaRepository.BuscarPerfilCorporativoAlocacaoPorId(param.Id);

            if (resLog == null)
                throw new InvalidOperationException("Alocação de Perfil Corporativo não localizada.");

            var result = await _organogramaRepository.AtualizarPerfilCorporativoAlocacao(param, _usuarioLogado.OrgId);

            if (result == null)
                throw new InvalidOperationException("Erro ao atualizar alocação de Perfil Corporativo.");

            await RegistrarLogAsync(resLog, result, "UPDATE", "PERFIL_CORP_ALOC");

            return result;
        }

        public async Task<bool> DeletarPerfilCorporativoAlocacao(string id)
        {
            var resLog = await _organogramaRepository.BuscarPerfilCorporativoAlocacaoPorId(id);

            if (resLog == null)
                throw new InvalidOperationException("Não existe esta alocação de Perfil Corporativo.");

            bool result = await _organogramaRepository.DeletarPerfilCorporativoAlocacao(id);

            if (result)
            {
                resLog.Ativo = false;
                await RegistrarLogAsync(resLog, null, "DELETE", "PERFIL_CORP_ALOC");
            }

            return result;
        }

        public async Task<OrganogramaPerfilCorporativoAlocacaoListarPorIdResponseDTO> BuscarPerfilCorporativoAlocacaoPorId(string buscaId)
            => await _organogramaRepository.BuscarPerfilCorporativoAlocacaoPorId(buscaId);

        public async Task<IEnumerable<OrganogramaColaboradorAlocadoDTO>> ListarColaboradoresExternosAlocados(int limit, int cursor, string nome)
            => await _organogramaRepository.ListarColaboradoresExternosAlocados(_usuarioLogado.OrgId, limit, cursor, nome);

        public async Task<IEnumerable<OrganogramaColaboradorAlocadoDTO>> ListarColaboradoresExternos(int limit, int cursor, string nome)
            => await _organogramaRepository.ListarColaboradoresExternos(_usuarioLogado.OrgId, limit, cursor, nome);

        public async Task<IEnumerable<OrganogramaColaboradorAlocadoDTO>> ListarColaboradoresExternosPorCliente(string codigoCliente, int limit, int cursor, string nome)
            => await _organogramaRepository.ListarColaboradoresExternosPorCliente(_usuarioLogado.OrgId, codigoCliente, limit, cursor, nome);

        private static void ValidarMapaRelacionamentoInfluencia(int mapaRelacionamentoInfluenciaId)
        {
            if (!MapaRelacionamentoInfluenciaExtensions.IsValid(mapaRelacionamentoInfluenciaId))
                throw new InvalidOperationException(
                    $"Influência (mapa de relacionamento): id {mapaRelacionamentoInfluenciaId} inválido. Utilize um valor de {nameof(MapaRelacionamentoInfluencia)}.");
        }

        private async Task RegistrarLogAsync(object? objAtual, object? objAlterado, string acao, string tabLog)
        {
            var log = new OrganogramaLogDTO
            {
                Id = Guid.NewGuid().ToString(),
                CodigoInternoColaborador = _usuarioLogado.Cpf,
                Acao = acao,
                CodigoInternoColaboradorAlterador = _usuarioLogado.Cpf,
                DataAlteracao = DateTime.Now,
                Objeto = objAtual == null ? "" : System.Text.Json.JsonSerializer.Serialize(objAtual),
                Alteracoes = objAlterado == null ? "" : System.Text.Json.JsonSerializer.Serialize(objAlterado),
            };

            await _organogramaLogRepository.OrganogramaInserirLog(log, tabLog);
        }

        //------------------- Perfil Corporativo
        public async Task<OrganogramaPerfilCorporativoListarPorIdResponseDTO> PerfilCorporativoInserir(OrganogramaPerfilCorporativoInserirParamDTO param)
        {
            var result = await _organogramaRepository.PerfilCorporativoInserir(param, _usuarioLogado.Cpf, _usuarioLogado.OrgId);

            if (result == null)
                throw new InvalidOperationException("Erro ao inserir Perfil Corporativo no Organograma.");

            //Grava Log
            await RegistrarLogAsync(result, null, "INSERT", "PERFIL_CORP");

            return result;
        }

        public async Task<OrganogramaPerfilCorporativoListarPorIdResponseDTO> PerfilCorporativoAtualizar(OrganogramaPerfilCorporativoAtualizarParamDTO param)
        {
            var resLog = await _organogramaRepository.PerfilCorporativoListaPorId(param.Id.ToString());

            if (resLog == null)
                throw new InvalidOperationException("Perfil Corporativo não localizado.");

            var result = await _organogramaRepository.PerfilCorporativoAtualizar(param, _usuarioLogado.Cpf, _usuarioLogado.OrgId);

            if (result == null)
                throw new InvalidOperationException("Erro ao atualizar Perfil Corporativo.");

            //Grava Log
            await RegistrarLogAsync(resLog, result, "UPDATE", "PERFIL_CORP");

            return result;
        }
        public async Task<bool> PerfilCorporativoDeletar(string id)
        {
            var resLog = await _organogramaRepository.PerfilCorporativoListaPorId(id);

            if (resLog == null)
                throw new InvalidOperationException("Não existe este Perfil Corporativo.");

            bool result = await _organogramaRepository.PerfilCorporativoDeletar(id);

            if (result)
            {
                resLog.Ativo = false;

                //Grava Log
                await RegistrarLogAsync(resLog, null, "DELETE", "PERFIL_CORP");

            }

            return result;
        }
        public async Task<OrganogramaPerfilCorporativoListarPorIdResponseDTO> PerfilCorporativoListaPorId(string buscaId)
            => await _organogramaRepository.PerfilCorporativoListaPorId(buscaId);

        public async Task<IEnumerable<OrganogramaPerfilCorporativoListarPorIdResponseDTO>> BuscarPerfisPorOrgId(int orgId)
            => await _organogramaRepository.BuscarPerfisPorOrgId(orgId);


        //------------------- Perfil Corporativo Skill
        public async Task<OrganogramaPerfilCorporativoSkillListarPorIdResponseDTO> PerfilCorporativoSkillInserir(OrganogramaPerfilCorporativoSkillInserirParamDTO param)
        {
            var result = await _organogramaRepository.PerfilCorporativoSkillInserir(param, _usuarioLogado.Cpf, _usuarioLogado.OrgId);

            if (result == null)
                throw new InvalidOperationException("Erro ao inserir Perfil Corporativo Skill no Organograma.");

            //Grava Log
            await RegistrarLogAsync(result, null, "INSERT", "PERFIL_CORP_SKILL");

            return result;
        }

        public async Task<OrganogramaPerfilCorporativoSkillListarPorIdResponseDTO> PerfilCorporativoSkillAtualizar(OrganogramaPerfilCorporativoSkillAtualizarParamDTO param)
        {
            var resLog = await _organogramaRepository.PerfilCorporativoSkillListaPorId(param.Id.ToString());

            if (resLog == null)
                throw new InvalidOperationException("Perfil Corporativo Skill não localizado.");

            var result = await _organogramaRepository.PerfilCorporativoSkillAtualizar(param, _usuarioLogado.Cpf, _usuarioLogado.OrgId);

            if (result == null)
                throw new InvalidOperationException("Erro ao atualizar Perfil Corporativo Skill.");

            //Grava Log
            await RegistrarLogAsync(resLog, result, "UPDATE", "PERFIL_CORP_SKILL");

            return result;
        }
        public async Task<bool> PerfilCorporativoSkillDeletar(string id)
        {
            var resLog = await _organogramaRepository.PerfilCorporativoSkillListaPorId(id);

            if (resLog == null)
                throw new InvalidOperationException("Não existe este Perfil Corporativo Skill.");

            bool result = await _organogramaRepository.PerfilCorporativoSkillDeletar(id);

            if (result)
            {
                resLog.Ativo = false;

                //Grava Log
                await RegistrarLogAsync(resLog, null, "DELETE", "PERFIL_CORP_SKILL");

            }

            return result;
        }
        public async Task<OrganogramaPerfilCorporativoSkillListarPorIdResponseDTO> PerfilCorporativoSkillListaPorId(string buscaId)
            => await _organogramaRepository.PerfilCorporativoSkillListaPorId(buscaId);

        //------------------- Cliente Org
        public async Task<IEnumerable<OrganogramaClienteOrgDTO>> RetornarClientesPorOrgId(int orgId, int limite, int cursor, string busca)
            => await _organogramaRepository.RetornarClientesPorOrgId(orgId, limite, cursor, busca);

        //------------------- Organograma Completo
        public async Task<OrganogramaCompletoResponseDTO> RetornarOrganogramaCompletoPorCliente(string codigoCliente)
            => await _organogramaRepository.RetornarOrganogramaCompletoPorCliente(_usuarioLogado.OrgId, codigoCliente);

        public async Task<OrganogramaCompletoResponseDTO> RetornarOrganogramaCompletoPorClienteCLevel(string codigoCliente)
            => await _organogramaRepository.RetornarOrganogramaCompletoPorClienteCLevel(_usuarioLogado.OrgId, codigoCliente);

    }
}