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
    public class GrupoAcessoService : IGrupoAcessoService
    {
        private IGrupoAcessoRepository _grupoAcessoRepository;
        private IPermissaoLogRepository _PermissaoLogRepository;
        private IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;

        public GrupoAcessoService(IGrupoAcessoRepository grupoAcessoRepository,
                                  IPermissaoLogRepository PermissaoLogRepository,
                                  IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository)
        {
            _grupoAcessoRepository = grupoAcessoRepository;
            _PermissaoLogRepository = PermissaoLogRepository;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
        }

        public Task<IEnumerable<GrupoAcessoDTO>> ListarGruposAcesso(string cpfRequest, int orgId)
        {
            ValidaAcessoPermissaoGrupoAcessoGrupoAcessoOuUsuario(cpfRequest, orgId);

            return _grupoAcessoRepository.ListarGruposAcesso(orgId);
        }

        public async Task<IEnumerable<GrupoAcessoFuncionalidadeSistemaDTO>> ListarGruposAcessoFuncionalidadesSistema(string cpfRequest, int orgId)
        {
            ValidaAcessoPermissaoGrupoAcessoGrupoAcessoOuUsuario(cpfRequest, orgId);

            var result = await _grupoAcessoRepository.ListarGruposAcessoFuncionalidadesSistema(orgId);

            var groupedResult = result.GroupBy(x => new { x.GrupoAcessoId, x.GrupoAcessoDescricao, x.GrupoAcessoAtivo, x.GrupoAcessoOrgId, x.GrupoAcessoDataCriacao, x.GrupoAcessoDataAlteracao, x.GrupoAcessoAcessoTodosClientes })
                .Select(group => new GrupoAcessoFuncionalidadeSistemaDTO
                {
                    Id = group.Key.GrupoAcessoId,
                    Descricao = group.Key.GrupoAcessoDescricao,
                    Ativo = group.Key.GrupoAcessoAtivo,
                    OrgId = group.Key.GrupoAcessoOrgId,
                    DataCriacao = group.Key.GrupoAcessoDataCriacao,
                    DataAlteracao = group.Key.GrupoAcessoDataAlteracao,
                    AcessoTodosClientes = group.Key.GrupoAcessoAcessoTodosClientes,
                    FuncionalidadeSistema = group.Where(x => x.FuncionalidadeSistemaId != 0)
                                                 .Select(x => new FuncionalidadeSistemaDTO
                                                 {
                                                     Id = x.FuncionalidadeSistemaId,
                                                     Descricao = x.FuncionalidadeSistemaDescricao,
                                                     Ativo = x.FuncionalidadeSistemaAtivo,
                                                     DataCriacao = x.FuncionalidadeSistemaDataCriacao,
                                                     DataAlteracao = x.FuncionalidadeSistemaDataAlteracao
                                                 })
                                                 .OrderBy(fs => fs.Id)
                });

            return groupedResult.OrderBy(x => x.Id);
        }

        public async Task AdicionarGrupoAcessoFuncionalidadeSistema(int grupoAcessoId, int funcionalidadeSistemaId, string cpfRequest, int orgId)
        {
            await ValidaOperacaoGrupoAcessoFuncionalidadeSistema(grupoAcessoId, funcionalidadeSistemaId, cpfRequest, orgId, CRUDEnum.Create);

            await _grupoAcessoRepository.AdicionarGrupoAcessoFuncionalidadeSistema(grupoAcessoId, funcionalidadeSistemaId, cpfRequest);
            await RegistrarLog("AdicionarGrupoAcessoFuncionalidadeSistema", grupoAcessoId, funcionalidadeSistemaId, cpfRequest, orgId);
        }

        public async Task RemoverGrupoAcessoFuncionalidadeSistema(int grupoAcessoId, int funcionalidadeSistemaId, string cpfRequest, int orgId)
        {
            await ValidaOperacaoGrupoAcessoFuncionalidadeSistema(grupoAcessoId, funcionalidadeSistemaId, cpfRequest, orgId, CRUDEnum.Delete);

            await _grupoAcessoRepository.RemoverGrupoAcessoFuncionalidadeSistema(grupoAcessoId, funcionalidadeSistemaId, cpfRequest);
            await RegistrarLog("RemoverGrupoAcessoFuncionalidadeSistema", grupoAcessoId, funcionalidadeSistemaId, cpfRequest, orgId);
        }

        private async Task RegistrarLog(string operacao, int grupoAcessoId, int funcionalidadeSistemaId, string cpfRequest, int orgId)
        {
            try
            {
                var log = new UsuarioPermissaoLogDTO
                {
                    OrgId = orgId,
                    ColaboradorCpfCriacao = cpfRequest,
                    Operacao = operacao,
                    GrupoAcessoId = grupoAcessoId,
                    FuncionalidadeSistemaId = funcionalidadeSistemaId
                };

                await _PermissaoLogRepository.InserirLog(log);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao registrar log da operação '{operacao}'", ex);
            }
        }

        private async Task ValidaOperacaoGrupoAcessoFuncionalidadeSistema(int grupoAcessoId, int funcionalidadeSistemaId, string cpfRequest, int orgId, CRUDEnum cRUDEnum)
        {
            ValidaAcessoPermissaoGrupoAcesso(cpfRequest, orgId);

            if (await _grupoAcessoRepository.GrupoAcessoNaoPertenceOrg(grupoAcessoId, orgId))
            {
                throw new ArgumentException($"Não foi possível realizar a operação. O grupo de acesso {grupoAcessoId} não pertence à Org {orgId}.");
            }

            if (await _funcionalidadeSistemaRepository.FuncionalidadeNaoExiste(funcionalidadeSistemaId))
            {
                throw new ArgumentException($"Não foi possível realizar a operação. Funcionalidade Sistema {funcionalidadeSistemaId} não está cadastrada no banco de dados.");
            }

            if (cRUDEnum == CRUDEnum.Create && await _grupoAcessoRepository.GrupoAcessoEstaRelacionadoFuncionalidadeSistema(grupoAcessoId, funcionalidadeSistemaId))
            {
                throw new ArgumentException($"Não foi possível realizar a operação. Grupo de acesso {grupoAcessoId} já está relacionado à Funcionalidade do Sistema {funcionalidadeSistemaId}.");
            }

            if (cRUDEnum == CRUDEnum.Delete && !await _grupoAcessoRepository.GrupoAcessoEstaRelacionadoFuncionalidadeSistema(grupoAcessoId, funcionalidadeSistemaId))
            {
                throw new ArgumentException($"Não foi possível realizar a operação. Grupo de acesso {grupoAcessoId} não está relacionado à Funcionalidade do Sistema {funcionalidadeSistemaId}.");
            }
        }

        private void ValidaAcessoPermissaoGrupoAcesso(string cpf, int orgId)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpf, orgId, FuncionalidadeSistemaEnum.CONTROLE_FUNCIONALIDADE_SISTEMA_GRUPO_ACESSO);

            if (!isValid.Result)
            {
                throw new UnauthorizedAccessException("Acesso negado para acessar Grupo Acesso no Controle de Funcionalidade do Sistema.");
            }
        }

        private void ValidaAcessoPermissaoGrupoAcessoGrupoAcessoOuUsuario(string cpf, int orgId)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpf, orgId, FuncionalidadeSistemaEnum.CONTROLE_FUNCIONALIDADE_SISTEMA_GRUPO_ACESSO).Result
                || _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpf, orgId, FuncionalidadeSistemaEnum.CONTROLE_FUNCIONALIDADE_SISTEMA_USUARIO).Result;

            if (!isValid)
            {
                throw new UnauthorizedAccessException("Acesso negado para acessar Grupo Acesso no Controle de Funcionalidade do Sistema.");
            }
        }

        public async Task<GrupoAcessoDTO> CriarGrupoAcesso(CriarGrupoAcessoInput input, string cpfRequest, int orgId)
        {
            ValidaAcessoPermissaoGrupoAcesso(cpfRequest, orgId);

            // Validar campos obrigatórios
            if (string.IsNullOrWhiteSpace(input.Descricao))
            {
                throw new ArgumentException("Descrição do grupo de acesso é obrigatória.");
            }

            // Validar se descrição já existe
            if (await _grupoAcessoRepository.DescricaoGrupoAcessoJaExiste(input.Descricao, orgId))
            {
                throw new ArgumentException($"Já existe um grupo de acesso com a descrição '{input.Descricao}' nesta organização.");
            }

            // Validar se todos os clientes existem
            if (input.Clientes != null && input.Clientes.Any())
            {
                foreach (var cliente in input.Clientes)
                {
                    if (string.IsNullOrWhiteSpace(cliente.CodigoCliente))
                    {
                        throw new ArgumentException("Código do cliente não pode ser vazio.");
                    }

                    if (!await _grupoAcessoRepository.ClienteExiste(cliente.CodigoCliente, orgId))
                    {
                        throw new ArgumentException($"Cliente com código '{cliente.CodigoCliente}' não encontrado ou inativo nesta organização.");
                    }
                }
            }

            // Determinar se o grupo terá acesso a todos os clientes
            bool acessoTodosClientes = input.Clientes == null || !input.Clientes.Any();

            // Criar grupo de acesso
            var grupoAcessoId = await _grupoAcessoRepository.CriarGrupoAcesso(input.Descricao, orgId, cpfRequest, acessoTodosClientes);
            
            // Registrar log no PermissaoLogRepository
            await RegistrarLogGrupoAcesso("CriarGrupoAcesso", grupoAcessoId, cpfRequest, orgId);

            // Adicionar clientes ao grupo apenas se não tiver acesso a todos
            // Se acesso_todos_clientes = true, não criar relacionamentos na tb_grupo_acesso_cliente
            if (!acessoTodosClientes)
            {
                // Se foram passados clientes específicos, adicionar apenas esses
                foreach (var cliente in input.Clientes)
                {
                    await _grupoAcessoRepository.AdicionarClienteAoGrupoAcesso(grupoAcessoId, cliente.CodigoCliente, orgId, cpfRequest);
                    // Registrar log no PermissaoLogRepository para cada cliente adicionado
                    await RegistrarLogGrupoAcesso("AdicionarClienteAoGrupoAcesso", grupoAcessoId, cpfRequest, orgId);
                }
            }
            // Se acesso_todos_clientes = true, não adicionar nenhum relacionamento na tb_grupo_acesso_cliente

            // Buscar grupo criado para retornar
            var grupos = await _grupoAcessoRepository.ListarGruposAcesso(orgId);
            return grupos.FirstOrDefault(g => g.Id == grupoAcessoId);
        }

        public async Task AtualizarGrupoAcesso(EditarGrupoAcessoInput input, string cpfRequest, int orgId)
        {
            ValidaAcessoPermissaoGrupoAcesso(cpfRequest, orgId);

            // Validar campos obrigatórios
            if (input.Id <= 0)
            {
                throw new ArgumentException("ID do grupo de acesso é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(input.Descricao))
            {
                throw new ArgumentException("Descrição do grupo de acesso é obrigatória.");
            }

            // Validar se grupo pertence à organização
            if (await _grupoAcessoRepository.GrupoAcessoNaoPertenceOrg(input.Id, orgId))
            {
                throw new ArgumentException($"O grupo de acesso {input.Id} não pertence à organização {orgId}.");
            }

            // Validar se descrição já existe (excluindo o próprio grupo)
            if (await _grupoAcessoRepository.DescricaoGrupoAcessoJaExiste(input.Descricao, orgId, input.Id))
            {
                throw new ArgumentException($"Já existe outro grupo de acesso com a descrição '{input.Descricao}' nesta organização.");
            }

            // Validar se todos os clientes existem
            if (input.Clientes != null && input.Clientes.Any())
            {
                foreach (var cliente in input.Clientes)
                {
                    if (string.IsNullOrWhiteSpace(cliente.CodigoCliente))
                    {
                        throw new ArgumentException("Código do cliente não pode ser vazio.");
                    }

                    if (!await _grupoAcessoRepository.ClienteExiste(cliente.CodigoCliente, orgId))
                    {
                        throw new ArgumentException($"Cliente com código '{cliente.CodigoCliente}' não encontrado ou inativo nesta organização.");
                    }
                }
            }

            // Determinar se o grupo terá acesso a todos os clientes
            bool acessoTodosClientes = input.Clientes == null || !input.Clientes.Any();

            // Atualizar descrição do grupo e flag de acesso a todos os clientes
            await _grupoAcessoRepository.AtualizarGrupoAcesso(input.Id, input.Descricao, cpfRequest, acessoTodosClientes);
            
            // Registrar log no PermissaoLogRepository
            await RegistrarLogGrupoAcesso("AtualizarGrupoAcesso", input.Id, cpfRequest, orgId);

            // Remover todos os clientes atuais
            await _grupoAcessoRepository.RemoverTodosClientesDoGrupoAcesso(input.Id, orgId, cpfRequest);
            
            // Registrar log no PermissaoLogRepository para remoção de clientes
            await RegistrarLogGrupoAcesso("RemoverTodosClientesDoGrupoAcesso", input.Id, cpfRequest, orgId);

            // Adicionar clientes ao grupo apenas se não tiver acesso a todos
            // Se acesso_todos_clientes = true, não criar relacionamentos na tb_grupo_acesso_cliente
            if (!acessoTodosClientes)
            {
                // Se foram passados clientes específicos, adicionar apenas esses
                foreach (var cliente in input.Clientes)
                {
                    await _grupoAcessoRepository.AdicionarClienteAoGrupoAcesso(input.Id, cliente.CodigoCliente, orgId, cpfRequest);
                    // Registrar log no PermissaoLogRepository para cada cliente adicionado
                    await RegistrarLogGrupoAcesso("AdicionarClienteAoGrupoAcesso", input.Id, cpfRequest, orgId);
                }
            }
            // Se acesso_todos_clientes = true, não adicionar nenhum relacionamento na tb_grupo_acesso_cliente
        }

        public async Task<IEnumerable<PessoaGrupoAcessoDTO>> ListarPessoasPorGrupoAcesso(string cpfRequest, int orgId, int? grupoId = null)
        {
            ValidaAcessoPermissaoGrupoAcessoGrupoAcessoOuUsuario(cpfRequest, orgId);

            return await _grupoAcessoRepository.ListarPessoasPorGrupoAcesso(orgId, grupoId);
        }

        private async Task RegistrarLogGrupoAcesso(string operacao, int grupoAcessoId, string cpfRequest, int orgId)
        {
            try
            {
                var log = new UsuarioPermissaoLogDTO
                {
                    OrgId = orgId,
                    ColaboradorCpfCriacao = cpfRequest,
                    Operacao = operacao,
                    GrupoAcessoId = grupoAcessoId
                };

                await _PermissaoLogRepository.InserirLog(log);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao registrar log da operação '{operacao}'", ex);
            }
        }
    }
}