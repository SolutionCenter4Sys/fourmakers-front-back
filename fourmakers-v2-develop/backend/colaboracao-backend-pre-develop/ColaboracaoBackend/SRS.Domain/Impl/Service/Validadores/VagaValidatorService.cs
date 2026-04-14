using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Competencia.Domain.Enums;
using Core.Domain.Usuario.Permissao;
using Core.Domain.Vaga;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.CRM;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.ItemPerfil;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.SRS.Vagas;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Vaga;
using Dapper;
using SRS.Domain.Interfaces.Service.Validadores;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Logs.Infra.Attributes;

namespace SRS.Domain.Impl.Service.Validadores
{
    [LogDomainClass]
    public class VagaValidatorService : IVagaValidatorService
    {
        private readonly IVagaFourmakersRepository _vagaFourmakersRepository;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly IDBConnection _dapperConnection;

        public VagaValidatorService(
            IVagaFourmakersRepository vagaFourmakersRepository,
            IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
            IDBConnection dapperConnection)
        {
            _vagaFourmakersRepository = vagaFourmakersRepository;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _dapperConnection = dapperConnection;
        }

        public async Task ValidaVaga(int vagaId, VagaFourmakersDTO vaga, int orgId, CRUDEnum cRUDEnum)
        {
            if (cRUDEnum == CRUDEnum.Read
                || cRUDEnum == CRUDEnum.Update)
            {
                if (vagaId == 0)
                {
                    throw new ApplicationException($"Vaga não informada.");
                }

                if (vaga.PerfilId.IsEmpty())
                {
                    throw new ApplicationException($"Perfil não informado.");
                }

                await ValidaSeExisteVagaNoFourmakers(vagaId);
                await ValidaSeExisteVagaERelacionamentoAoGestorExternoPerfilId(vagaId, vaga.PerfilId, orgId);
            }

            if (cRUDEnum == CRUDEnum.Create
                || cRUDEnum == CRUDEnum.Update)
            {
                await ValidarCamposDeEntrada(vaga);
            }
        }
 
        public async Task ValidarCamposEnvioCandidatosAderencia(RecomendarCandidatosParam param)
        {
            var campos = new List<CampoValidacao>();

            campos.Add(new("Assunto", param.Assunto, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("VagaId", param.VagaId, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("FormatoNomeExibicao", param.FormatoNomeExibicao, TipoValidacaoEnum.Obrigatoriedade));

            if(param.Emails.IsNotNull())
                campos.Add(new("Emails", param.Emails, TipoValidacaoEnum.ContemAoMenosUmNaLista));

            if (param.Colaboradores.IsNotNull())
                campos.Add(new("Colaboradores", param.Colaboradores, TipoValidacaoEnum.ContemAoMenosUmNaLista));

            await ValidadorCamposUtil.ValidaCampos(campos);
        }

        private async Task ValidaSeExisteVagaERelacionamentoAoGestorExternoPerfilId(int vagaId, string perfilId, int orgId)
        {
            var vaga = await _vagaFourmakersRepository.ObterVagaPorIdEPerfilId(vagaId, perfilId, orgId);

            if (vaga is null)
            {
                throw new ApplicationException($"Vaga não está associada ao Perfil informado.");
            }
        }

        private async Task ValidaSeExisteVagaNoFourmakers(int vagaId)
        {
            var vaga = await _vagaFourmakersRepository.ObterVagaPorId(vagaId);

            if (vagaId == 0)
            {
                throw new ApplicationException($"Vaga não cadastrada no banco Fourmakers.");
            }
        }

        private async Task ValidarCamposDeEntrada(VagaFourmakersDTO param)
        {
            var campos = new List<CampoValidacao>();

            campos.Add(new("Unidade", param.Unidade, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Perfil", param.PerfilId, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Cargo", param.Cargo, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Descricão Cargo", param.DescricaoCargo, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Título", param.Titulo, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Tipo de Localização", param.TipoLocalizacao, TipoValidacaoEnum.Obrigatoriedade));

            if (param.TipoLocalizacao?.ToUpper() == "HÍBRIDO" || param.TipoLocalizacao?.ToUpper() == "HIBRIDO")
            {
                campos.Add(new("Estado", param.Estado, TipoValidacaoEnum.Obrigatoriedade));
                campos.Add(new("Cidade", param.Cidade, TipoValidacaoEnum.Obrigatoriedade));
                campos.Add(new("Frequência", param.Frequencia, TipoValidacaoEnum.Obrigatoriedade));
            }

            //PBI - 12528
            //if (param.TipoLocalizacao != "HÍBRIDO" && param.TipoLocalizacao != "HOME OFFICE")
            //{
            //    campos.Add(new("Estado", param.Estado, TipoValidacaoEnum.Obrigatoriedade));
            //    campos.Add(new("Cidade", param.Cidade, TipoValidacaoEnum.Obrigatoriedade));
            //}

            campos.Add(new("Data Prevista Início", param.DataPrevistaInicio, TipoValidacaoEnum.ValidarData));
            campos.Add(new("Aprovador", param.Aprovador, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Carga Horária", param.CargaHoraria, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Tipo Vaga", param.Tipo, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Solicitante", param.Solicitante, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Termômetro", param.Termometro, TipoValidacaoEnum.Obrigatoriedade));
            
            if(string.IsNullOrWhiteSpace(param.MaquinaCliente) && string.IsNullOrWhiteSpace(param.MaquinaFour))
            {
                campos.Add(new("Máquina Cliente ou Máquina Foursys","" , TipoValidacaoEnum.Obrigatoriedade));
            }
            
            if (!string.IsNullOrWhiteSpace(param.MaquinaFour))
            {
                campos.Add(new("Stack Principal", param.StackPrincipal, TipoValidacaoEnum.Obrigatoriedade));
                campos.Add(new("Configuração Máquina", param.ConfiguracaoMaquina, TipoValidacaoEnum.Obrigatoriedade));
            }
            
            campos.Add(new("Duração Contrato", param.DuracaoContrato, TipoValidacaoEnum.Obrigatoriedade));

            if (param.DuracaoContrato == "D")
            {
                campos.Add(new("Duração Contrato Determinado", param.DuracaoContratoDeterminado.ToStringOuVazio(), TipoValidacaoEnum.Obrigatoriedade));
            }

            campos.Add(new("Hardskills", param.Hardskills?.Count.ToIntOuZero() > 0 ? "valido" : null, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Softskills", param.Softskills?.Count.ToIntOuZero() > 0 ? "valido" : null, TipoValidacaoEnum.Obrigatoriedade));

            campos.Add(new("Tipo Contratação", param.TipoContratacao?.Count.ToIntOuZero() > 0 ? "valido" : null, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Número De Vagas", param.NumeroDeVagas.ToIntOuZero() > 0 ? "valido" : null, TipoValidacaoEnum.Obrigatoriedade));

            campos.Add(new("Taxa Máxima Por Hora", param.TaxaMaximaPorHora.ToDecimalOuZero() >= 0 ? "valido" : null, TipoValidacaoEnum.Obrigatoriedade));

            await ValidadorCamposUtil.ValidaCampos(campos);
        }

        public async Task ValidarDatas(string dataInicio, string dataFim)
        {
            var campos = new List<CampoValidacao>();

            campos.Add(new("Data Inicio", dataInicio, TipoValidacaoEnum.ValidarData));
            campos.Add(new("Data Fim", dataFim, TipoValidacaoEnum.ValidarData));

            await ValidadorCamposUtil.ValidaCampos(campos);
        }

        public async Task ValidarCadastroVagaRecrutamento(InserirVagaRecrutamentoDTO vagaDto)
        {
            var campos = new List<CampoValidacao>();

            campos.Add(new("Título", vagaDto.Titulo, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Número de Vagas", vagaDto.NumeroDeVagas, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Descrição", vagaDto.Descricao, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Código Gestor", vagaDto.CodigoGestor, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("OrgId", vagaDto.OrgId, TipoValidacaoEnum.Obrigatoriedade));

            if (vagaDto.ModeloTrabalhoCod == ModalidadeEnum.HIBRIDO.ToInt() || vagaDto.ModeloTrabalhoCod == ModalidadeEnum.PRESENCIAL.ToInt())
            {
                campos.Add(new("Estado", vagaDto.Estado, TipoValidacaoEnum.Obrigatoriedade));
                campos.Add(new("Cidade", vagaDto.Cidade, TipoValidacaoEnum.Obrigatoriedade));
            }

            await ValidadorCamposUtil.ValidaCampos(campos);
        }

        public async Task ValidarAtualizacaoVagaRecrutamento(VagaVindaDeGestorExternoPerfil atualizarVagaDTO)
        {
            if(atualizarVagaDTO.Codigo == 0)
                throw new ArgumentException($"Codigo da Vaga nao informado.");

            var campos = new List<CampoValidacao>();

            campos.Add(new("NomePerfil", atualizarVagaDTO.NomePerfil, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Número de Vagas", atualizarVagaDTO.NumeroDeVagas, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Informacoes Relevantes", atualizarVagaDTO.InformacoesRelevantes, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Código Gestor", atualizarVagaDTO.CodGestorExterno, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("OrgId", atualizarVagaDTO.OrgId, TipoValidacaoEnum.Obrigatoriedade));

            if (atualizarVagaDTO.ModeloTrabalhoCod == ModalidadeEnum.HIBRIDO.ToInt() || atualizarVagaDTO.ModeloTrabalhoCod == ModalidadeEnum.PRESENCIAL.ToInt())
            {
                campos.Add(new("ProfissionalLocalidadeId", atualizarVagaDTO.ProfissionalLocalidadeId, TipoValidacaoEnum.Obrigatoriedade));
                campos.Add(new("Estado", atualizarVagaDTO.Estado, TipoValidacaoEnum.Obrigatoriedade));
                campos.Add(new("Cidade", atualizarVagaDTO.Cidade, TipoValidacaoEnum.Obrigatoriedade));
            }

            await ValidadorCamposUtil.ValidaCampos(campos);
        }

        public async Task ValidaMudancaVagaParaEmRefinamento(VagaRecrutamentoDTO vagaDto)
        {
            var campos = new List<CampoValidacao>();

            campos.Add(new("Título", vagaDto.Titulo, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Codigo Cliente", vagaDto.CodigoCliente, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Código Gestor", vagaDto.CodigoGestor, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Descrição", vagaDto.Descricao, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Modelo Trabalho", vagaDto.ModeloTrabalhoCod, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Custo Profissional", vagaDto.CustoProfissional, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("OrgId", vagaDto.OrgId, TipoValidacaoEnum.Obrigatoriedade));

            if (!(vagaDto.ModeloTrabalhoCod == ModalidadeEnum.REMOTO.ToInt()))
            {
                campos.Add(new("Estado", vagaDto.Estado, TipoValidacaoEnum.Obrigatoriedade));
                campos.Add(new("Cidade", vagaDto.Cidade, TipoValidacaoEnum.Obrigatoriedade));
            }

            campos.Add(new("Skills", vagaDto.Skills, TipoValidacaoEnum.ContemAoMenosDoisNaLista));

            await ValidadorCamposUtil.ValidaCampos(campos);

            var hardSkills = vagaDto.Skills.Where(m => m.TipoSkillId == ItemPerfilEnum.COMPETENCIA.ToInt()).ToList();
            if (hardSkills is null)
                throw new ApplicationException("Exige-se ao menos duas hard skills para entrar em refinamento");
            if (hardSkills.Count() < 2)
                throw new ApplicationException("Exige-se ao menos duas hard skills para entrar em refinamento");
        }

        public async Task<List<string>> ValidarAcessoEListarClientesPermitidos(string cpf, int orgId)
        {
            // 1. Validar acesso à funcionalidade RECRUTAMENTO_LISTAR_VAGAS
            var temAcesso = await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpf, 
                orgId, 
                FuncionalidadeSistemaEnum.RECRUTAMENTO_LISTAR_VAGAS
            );

            if (!temAcesso)
            {
                throw new UnauthorizedAccessException("Acesso negado para listar vagas de recrutamento.");
            }

            // 2. Obter grupos de acesso do usuário
            var gruposIds = await ObterGruposAcessoUsuario(cpf, orgId);

            if (gruposIds == null || gruposIds.Count == 0)
            {
                // Usuário sem grupos = sem acesso a nenhum cliente
                return new List<string>();
            }

            // 3. Obter clientes permitidos aos grupos
            var clientesPermitidos = await ObterClientesPermitidos(gruposIds, orgId);

            return clientesPermitidos;
        }

        private async Task<List<int>> ObterGruposAcessoUsuario(string cpf, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT DISTINCT tga.id
                FROM tb_usuario_grupo_acesso tuga
                INNER JOIN tb_grupo_acesso tga ON tuga.tb_grupo_acesso_id = tga.id
                INNER JOIN tb_usuario u ON tuga.tb_usuario_id = u.id
                WHERE u.codigo_interno_colaborador = @Cpf
                  AND u.tb_org_id = @OrgId
                  AND tuga.ativo = 1
                  AND tga.ativo = 1;
            ";

            var result = await connection.QueryAsync<int>(query, new { Cpf = cpf, OrgId = orgId });
            return result.ToList();
        }

        private async Task<List<string>> ObterClientesPermitidos(List<int> gruposIds, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            // Primeiro verificar se algum grupo tem acesso a todos os clientes
            var queryVerificaTodos = @"
                SELECT COUNT(1)
                FROM tb_grupo_acesso
                WHERE id IN @GruposIds
                  AND tb_org_id = @OrgId
                  AND ativo = 1
                  AND acesso_todos_clientes = 1;
            ";

            var temAcessoTodos = await connection.ExecuteScalarAsync<int>(queryVerificaTodos, new { GruposIds = gruposIds, OrgId = orgId }) > 0;

            if (temAcessoTodos)
            {
                // Se algum grupo tem acesso a todos, retornar todos os clientes ativos da organização
                var queryTodosClientes = @"
                    SELECT DISTINCT codigo_cliente
                    FROM tb_cliente_org
                    WHERE tb_org_id = @OrgId
                      AND ativo = 1;
                ";

                var result = await connection.QueryAsync<string>(queryTodosClientes, new { OrgId = orgId });
                return result.ToList();
            }
            else
            {
                // Se nenhum grupo tem acesso a todos, buscar apenas os clientes específicos na tb_grupo_acesso_cliente
                var query = @"
                    SELECT DISTINCT gac.codigo_cliente
                    FROM tb_grupo_acesso_cliente gac
                    INNER JOIN tb_grupo_acesso tga ON gac.tb_grupo_acesso_id = tga.id
                    WHERE gac.tb_grupo_acesso_id IN @GruposIds
                      AND gac.tb_org_id = @OrgId
                      AND gac.ativo = 1
                      AND tga.acesso_todos_clientes = 0;
                ";

                var result = await connection.QueryAsync<string>(query, new { GruposIds = gruposIds, OrgId = orgId });
                return result.ToList();
            }
        }

        public async Task ValidarCandidatarSeRecrutamento(CandidatarSeRecrutamentoParam param)
        {
            var campos = new List<CampoValidacao>();

            campos.Add(new("CodigoVaga", param.CodigoVaga, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("OpcoesContatoIds", param.OpcoesContatoIds, TipoValidacaoEnum.ContemAoMenosUmNaLista));

            await ValidadorCamposUtil.ValidaCampos(campos);
        }
    }
}