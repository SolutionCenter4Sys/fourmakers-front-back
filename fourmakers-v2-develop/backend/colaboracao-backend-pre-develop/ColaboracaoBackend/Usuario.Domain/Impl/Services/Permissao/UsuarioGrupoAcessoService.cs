using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Usuario.Permissao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usuario.Domain.Interfaces.Services.Permissao;

using Logs.Infra.Attributes;

namespace Usuario.Domain.Impl.Services.Permissao
{
    [LogDomainClass]
    public class UsuarioGrupoAcessoService : IUsuarioGrupoAcessoService
    {
        private IUsuarioGrupoAcessoRepository _usuarioGrupoAcessoRepository;
        private IGrupoAcessoRepository _grupoAcessoRepository;
        private IPermissaoLogRepository _PermissaoLogRepository;
        private IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;

        public UsuarioGrupoAcessoService(IUsuarioGrupoAcessoRepository usuarioGrupoAcessoRepository,
                                         IGrupoAcessoRepository grupoAcessoRepository,
                                         IPermissaoLogRepository PermissaoLogRepository,
                                         IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository)
        {
            _usuarioGrupoAcessoRepository = usuarioGrupoAcessoRepository;
            _grupoAcessoRepository = grupoAcessoRepository;
            _PermissaoLogRepository = PermissaoLogRepository;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
        }

        public async Task<UsuarioGrupoAcessoDTO> ObterGruposAcessoPorUsuario(int usuarioId, string cpfRequest, int orgId)
        {
            ValidaAcessoPermissaoUsuario(cpfRequest, orgId);

            var result = await _usuarioGrupoAcessoRepository.ObterGruposAcessoPorUsuario(usuarioId, orgId);

            var groupedResult = result.GroupBy(x => new { x.UsuarioId, x.NomeCompleto, x.Cpf })
                                        .Select(group => new UsuarioGrupoAcessoDTO
                                        {
                                            Id = group.Key.UsuarioId,
                                            Nome = group.Key.NomeCompleto,
                                            Cpf = group.Key.Cpf,
                                            GrupoAcesso = group.Select(x => new GrupoAcessoDTO
                                            {
                                                Id = x.GrupoAcessoId,
                                                Descricao = x.Descricao,
                                                Ativo = ObjectExtension.ToBool(x.Ativo),
                                                OrgId = x.OrgId,
                                                DataCriacao = x.DataCriacao,
                                                DataAlteracao = x.DataAlteracao
                                            }).OrderBy(ga => ga.Id)
                                        })
                                        .FirstOrDefault();

            return groupedResult;
        }

        public async Task<UsuarioGrupoAcessoFuncionalidadeSistemaDTO> ObterGruposAcessoFuncionalidadesSistemaPorUsuario(int usuarioId, string cpfRequest, int orgId)
        {
            ValidaAcessoPermissaoUsuario(cpfRequest, orgId);

            var usuarioGrupoAcesso = await ObterGruposAcessoPorUsuario(usuarioId, cpfRequest, orgId);

            if (usuarioGrupoAcesso == null)
                return null;

            var funcionalidadesSistema = await _grupoAcessoRepository.ListarGruposAcessoFuncionalidadesSistema(orgId);

            var result = new UsuarioGrupoAcessoFuncionalidadeSistemaDTO();
            var listGrupoAcesso = new List<GrupoAcessoFuncionalidadeSistemaDTO>();

            foreach (var grupo in usuarioGrupoAcesso.GrupoAcesso)
            {
                var grupoAcessoFuncionalidades = new GrupoAcessoFuncionalidadeSistemaDTO
                {
                    Id = grupo.Id,
                    Descricao = grupo.Descricao,
                    Ativo = grupo.Ativo,
                    OrgId = grupo.OrgId,
                    DataCriacao = grupo.DataCriacao,
                    DataAlteracao = grupo.DataAlteracao,
                    FuncionalidadeSistema = funcionalidadesSistema.Where(f => f.GrupoAcessoId == grupo.Id)
                                                                  .Select(x => new FuncionalidadeSistemaDTO
                                                                  {
                                                                      Id = x.FuncionalidadeSistemaId,
                                                                      Descricao = x.FuncionalidadeSistemaDescricao,
                                                                      Ativo = x.FuncionalidadeSistemaAtivo,
                                                                      DataCriacao = x.FuncionalidadeSistemaDataCriacao,
                                                                      DataAlteracao = x.FuncionalidadeSistemaDataAlteracao
                                                                  })
                                                                  .OrderBy(fs => fs.Id)
                };
                listGrupoAcesso.Add(grupoAcessoFuncionalidades);
            }

            result.Id = usuarioGrupoAcesso.Id;
            result.Nome = usuarioGrupoAcesso.Nome;
            result.Cpf = usuarioGrupoAcesso.Cpf;
            result.GrupoAcesso = listGrupoAcesso.OrderBy(ga => ga.Id);

            return result;
        }

        public async Task AdicionarUsuarioGrupoAcesso(int usuarioId, int grupoAcessoId, string cpfRequest, int orgId)
        {
            await ValidaOperacaoUsuarioGrupoAcesso(usuarioId, grupoAcessoId, cpfRequest, orgId, CRUDEnum.Create);

            await _usuarioGrupoAcessoRepository.AdicionarUsuarioGrupoAcesso(usuarioId, grupoAcessoId, cpfRequest);
            await RegistrarLog("AdicionarUsuarioGrupoAcesso", usuarioId, grupoAcessoId, cpfRequest, orgId);
        }

        public async Task RemoverUsuarioGrupoAcesso(int usuarioId, int grupoAcessoId, string cpfRequest, int orgId)
        {
            await ValidaOperacaoUsuarioGrupoAcesso(usuarioId, grupoAcessoId, cpfRequest, orgId, CRUDEnum.Delete);

            await _usuarioGrupoAcessoRepository.RemoverUsuarioGrupoAcesso(usuarioId, grupoAcessoId, cpfRequest);
            await RegistrarLog("RemoverUsuarioGrupoAcesso", usuarioId, grupoAcessoId, cpfRequest, orgId);
        }

        private async Task RegistrarLog(string operacao, int usuarioId, int grupoAcessoId, string cpfRequest, int orgId)
        {
            try
            {
                var log = new UsuarioPermissaoLogDTO
                {
                    OrgId = orgId,
                    ColaboradorCpfCriacao = cpfRequest,
                    Operacao = operacao,
                    GrupoAcessoId = grupoAcessoId,
                    UsuarioId = usuarioId
                };

                await _PermissaoLogRepository.InserirLog(log);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao registrar log da operação '{operacao}'", ex);
            }
        }

        private void ValidaAcessoPermissaoUsuario(string cpf, int orgId)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpf, orgId, FuncionalidadeSistemaEnum.CONTROLE_FUNCIONALIDADE_SISTEMA_USUARIO);

            if (!isValid.Result)
            {
                throw new UnauthorizedAccessException("Acesso negado para acessar Usuários no Controle de Funcionalidade do Sistema.");
            }
        }

        private async Task ValidaOperacaoUsuarioGrupoAcesso(int usuarioId, int grupoAcessoId, string cpfRequest, int orgId, CRUDEnum cRUDEnum)
        {
            ValidaAcessoPermissaoUsuario(cpfRequest, orgId);

            if (await _grupoAcessoRepository.GrupoAcessoNaoPertenceOrg(grupoAcessoId, orgId))
            {
                throw new ArgumentException($"Não foi possível realizar a operação. O grupo de acesso {grupoAcessoId} não pertence à Org {orgId}.");
            }

            if (await _usuarioGrupoAcessoRepository.UsuarioNaoPertenceOrg(usuarioId, orgId))
            {
                throw new ArgumentException($"Não foi possível realizar a operação. Usuário {usuarioId} não pertence à Org {orgId}.");
            }

            if (cRUDEnum == CRUDEnum.Create)
            {
                //if (await UsuarioTentandoRelacionarGrupoAcessoFuncionalidadeSistemaMasNaoPossuiPermissao(usuarioId, grupoAcessoId, cpfRequest, orgId))
                //{
                //    throw new ArgumentException("Não foi possível realizar a operação. Você não possui permissão para se adicionar a um Grupo de Acesso que possui edição de Funcionalidades do Sistema.");
                //}

                if (await _usuarioGrupoAcessoRepository.UsuarioEstaRelacionadoGrupoAcesso(usuarioId, grupoAcessoId))
                {
                    throw new ArgumentException($"Não foi possível realizar a operação. Usuário {usuarioId} já pertence ao Grupo Acesso {grupoAcessoId}.");
                }
            }

            if (cRUDEnum == CRUDEnum.Delete && !await _usuarioGrupoAcessoRepository.UsuarioEstaRelacionadoGrupoAcesso(usuarioId, grupoAcessoId))
            {
                throw new ArgumentException($"Não foi possível realizar a operação. Usuário {usuarioId} não pertence ao Grupo Acesso {grupoAcessoId}.");
            }
        }

        private async Task<bool> UsuarioTentandoRelacionarGrupoAcessoFuncionalidadeSistemaMasNaoPossuiPermissao(int usuarioId, int grupoAcessoId, string cpfRequest, int orgId)
        {
            var resultUsuarioInformado = await ObterGruposAcessoPorUsuario(usuarioId, cpfRequest, orgId);

            // TODO: retirei essa validação pois está com problema, esse método será refeito depois, quando pegarmos o usuário não mais por cpf
            // var usuarioIdLogado = await _usuarioRepository.GetUsuarioPorCPF
            // var resultUsuarioLogado = await ObterGruposAcessoPorUsuario(usuarioIdLogado, cpfRequest, orgId);
            //if(resultUsuarioInformado.Id != resultUsuarioLogado.ID)

            if (resultUsuarioInformado.Id == usuarioId)
            {
                var listaGrupoAcesso = await _grupoAcessoRepository.ListarGruposAcessoFuncionalidadesSistema(orgId);

                var grupoAcessoPossuiPermissaoAlterarGrupoAcesso = listaGrupoAcesso.Any(x => x.GrupoAcessoId == grupoAcessoId && x.FuncionalidadeSistemaId == (int)FuncionalidadeSistemaEnum.CONTROLE_FUNCIONALIDADE_SISTEMA_GRUPO_ACESSO);

                if (grupoAcessoPossuiPermissaoAlterarGrupoAcesso)
                {
                    var usuarioGruposAcessoFuncionalidadesSistema = await ObterGruposAcessoFuncionalidadesSistemaPorUsuario(usuarioId, cpfRequest, orgId);

                    foreach (var usuarioGrupoAcesso in usuarioGruposAcessoFuncionalidadesSistema.GrupoAcesso)
                    {
                        if (usuarioGrupoAcesso.FuncionalidadeSistema.Any(x => x.Id == (int)FuncionalidadeSistemaEnum.CONTROLE_FUNCIONALIDADE_SISTEMA_GRUPO_ACESSO))
                        {
                            return false;
                        }
                    }
                    return true; // Usuario nao possui permissao para se adicionar em um grupo de acesso que pode alterar 'grupo de acesso x funcionalidade sistema'
                }
            }
            return false;
        }
    }
}