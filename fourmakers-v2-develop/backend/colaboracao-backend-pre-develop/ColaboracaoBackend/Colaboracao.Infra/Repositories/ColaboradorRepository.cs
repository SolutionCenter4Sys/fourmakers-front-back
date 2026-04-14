using ApiClient.Domain;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Infra.Context;
using Colaboracao.Infra.Repositories.Colaborador;
using Core.Domain;
using Core.DomainModel;
using DataTransferObject.Domain.Candidato;
using DataTransferObject.Domain.Colaborador;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class ColaboradorRepository : IColaboradorDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        private readonly IConfiguration _configuration;
        private HistoricoCVRepository _historicoCV;

        public ColaboradorRepository(ColaboradorContext colaboradorContext, IConfiguration configuration, IDBConnection dbConnection)
        {
            this._colaboradorContext = colaboradorContext;
            this._configuration = configuration;
            this._historicoCV = new HistoricoCVRepository(dbConnection);
        }

        public int ContarColaboradoresMenosEste(string cpf)
        {
            return _colaboradorContext.tb_colaborador.Where(x => x.ativo == 1 && x.codigo_interno_colaborador != cpf && x.candidato == 0).Count();
        }

        public void FollowColaborador(string cpfSeguidor, string cpfSeguir)
        {
            var relacionamentoRow = _colaboradorContext.tb_contato_colaborador.Where(x => x.seguidor_codigo_interno_colaborador == cpfSeguidor && x.seguindo_codigo_interno_colaborador == cpfSeguir).FirstOrDefault();
            if (relacionamentoRow == null)
            {
                relacionamentoRow = new tb_contato_colaborador();
                relacionamentoRow.seguidor_codigo_interno_colaborador = cpfSeguidor;
                relacionamentoRow.seguindo_codigo_interno_colaborador = cpfSeguir;
                relacionamentoRow.ativo = 1;
                _colaboradorContext.tb_contato_colaborador.Add(relacionamentoRow);
            }
            else
            {
                relacionamentoRow.ativo = 1;
                _colaboradorContext.tb_contato_colaborador.Update(relacionamentoRow);
            }

            _colaboradorContext.SaveChanges();
        }

        public void UnfollowColaborador(string cpfSeguidor, string cpfSeguir)
        {
            var relacionamentoRow = _colaboradorContext.tb_contato_colaborador.Where(x => x.seguidor_codigo_interno_colaborador == cpfSeguidor && x.seguindo_codigo_interno_colaborador == cpfSeguir).FirstOrDefault();
            if (relacionamentoRow != null)
            {
                relacionamentoRow.ativo = 0;
                _colaboradorContext.tb_contato_colaborador.Update(relacionamentoRow);
            }

            _colaboradorContext.SaveChanges();
        }

        public int ContarCandidatosMenosEste(string cpf)
        {
            return _colaboradorContext.tb_colaborador.Where(x => x.ativo == 1 && x.codigo_interno_colaborador != cpf && x.candidato == 1).Count();
        }

        private string ConverterPCDParaBanco(EnumPCD pcd)
        {
            if (pcd == EnumPCD.PsicossocialMental)
                return "Psicossocial / Mental";

            return pcd.ToString();
        }

        private EnumPCD ConverterPCDDoBanco(string valor)
        {
            if (valor == "Psicossocial / Mental")
                return EnumPCD.PsicossocialMental;

            if (Enum.TryParse(valor, out EnumPCD pcd))
                return pcd;

            return EnumPCD.Nenhuma;
        }

        public ColaboradorDTO UpdateModel(ColaboradorDTO colaboradorModel)
        {
            try
            {
                var registroDb = _colaboradorContext.tb_colaborador
                    .Where(x => x.codigo_interno_colaborador == colaboradorModel.Cpf)
                    .FirstOrDefault();

                if (registroDb == null)
                    throw new Exception("Colaborador não encontrado no sistema.");

                registroDb.nome_completo = colaboradorModel.NomeCompleto;
                registroDb.data_nascimento = colaboradorModel.DataNascimento.HasValue ? colaboradorModel.DataNascimento : null;
                registroDb.rg = colaboradorModel.Rg;
                registroDb.matricula = colaboradorModel.Matricula;
                registroDb.data_alteracao = DateTime.Now;
                registroDb.contato_principal_ddi = colaboradorModel.ContatoPrincipalDDI;
                registroDb.contato_principal = colaboradorModel.ContatoPrincipal;
                registroDb.contato_outro = colaboradorModel.ContatoOutros;
                registroDb.documento_colaborador = colaboradorModel.DocumentoColaborador;
                registroDb.passaporte = colaboradorModel.Passaporte;
                registroDb.estado_civil = colaboradorModel.EstadoCivil;
                registroDb.genero = colaboradorModel.Genero;
                registroDb.etnia = colaboradorModel.Etnia;

                if (registroDb.escolaridade != colaboradorModel.Escolaridade)
                {
                    var origemHistorico = _colaboradorContext.tb_origem_historico_cv.FirstOrDefault(x => x.descricao == OrigemAlteracaoCVEnum.MANUAL.ToString());
                    var tipoHistorico = _colaboradorContext.tb_tipo_historico_cv.FirstOrDefault(x => x.descricao == TipoItemCVEnum.UPDATE.ToString());

                    if (origemHistorico == null)
                        throw new Exception("Origem Historico não encontrado");
                    if (tipoHistorico == null)
                        throw new Exception("Tipo Historico não encontrado");

                    var historicoCv = new tb_historico_cv
                    {
                        id = Guid.NewGuid().ToString(),
                        codigo_interno_colaborador = colaboradorModel.Cpf,
                        tb_item_perfil_id = _historicoCV.GetItemCVId(ItemCVEnum.ESCOLARIDADE),
                        tb_origem_historico_cv_id = origemHistorico.id,
                        tb_tipo_historico_cv_id = tipoHistorico.id,
                        tb_skill_id = null,
                        tb_nivel_id = null,
                        data_criacao = DateTime.Now
                    };
                    _colaboradorContext.tb_historico_cv.Add(historicoCv);
                }

                registroDb.escolaridade = colaboradorModel.Escolaridade;
                registroDb.orientacao_sexual = colaboradorModel.OrientacaoSexual;
                registroDb.refugiado = (sbyte)(colaboradorModel.PessoaRefugiada ? 1 : 0);
                registroDb.email_alternativo = colaboradorModel.EmailAlternativo;
                registroDb.nacionalidade = colaboradorModel.Nacionalidade;

                if (registroDb.colaborador_saude != null && colaboradorModel.Saude != null)
                {
                    registroDb.colaborador_saude.pcd = ConverterPCDParaBanco(colaboradorModel.Saude.PCD);
                    registroDb.colaborador_saude.condicao_saude_relevante = colaboradorModel.Saude.CondicaoDeSaudeRelevante;
                    registroDb.colaborador_saude.grupo_risco_covid = colaboradorModel.Saude.GrupoDeRiscoCovid;
                }
                else if (colaboradorModel.Saude != null)
                {
                    InserirDadosPCD(colaboradorModel.Cpf, colaboradorModel);
                }

                if (colaboradorModel.Candidato != null)
                    registroDb.candidato = 1;
                if (colaboradorModel.FlagAtivo)
                    registroDb.ativo = 1;

                _colaboradorContext.tb_colaborador.Update(registroDb);
                _colaboradorContext.SaveChanges();

                var cargoColabDb = _colaboradorContext.tb_colaborador_cargo
                    .FirstOrDefault(x => x.codigo_interno_colaborador == colaboradorModel.Cpf);
                if (cargoColabDb != null && colaboradorModel.Cargo != null)
                {
                    cargoColabDb.cargo_id = colaboradorModel.Cargo.Id;
                    cargoColabDb.data_alteracao = DateTime.Now;
                    _colaboradorContext.tb_colaborador_cargo.Update(cargoColabDb);
                }

                var usuarioDb = _colaboradorContext.tb_usuario
                    .FirstOrDefault(x => x.codigo_interno_colaborador == colaboradorModel.Cpf);
                if (usuarioDb != null)
                {
                    usuarioDb.email = colaboradorModel.Email;
                    _colaboradorContext.tb_usuario.Update(usuarioDb);
                }

                if (colaboradorModel.Status != null)
                {
                    var statusColabDb = _colaboradorContext.tb_colaborador_status
                        .FirstOrDefault(x => x.codigo_interno_colaborador == colaboradorModel.Cpf);
                    if (statusColabDb != null)
                    {
                        statusColabDb.status_colaborador_id = (int)colaboradorModel.Status.Id;
                        _colaboradorContext.tb_colaborador_status.Update(statusColabDb);
                    }
                }

                _colaboradorContext.SaveChanges();
                return colaboradorModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ColaboradorDTO GetModel(ColaboradorDTO colaboradorModel)
        {
            var registroDb = _colaboradorContext.tb_colaborador.FirstOrDefault(x => x.codigo_interno_colaborador == colaboradorModel.Cpf);
            if (registroDb == null)
                return null;

            var colaboradorOrg = registroDb.tb_colaborador_org.FirstOrDefault();

            colaboradorModel.Cpf = registroDb.codigo_interno_colaborador;
            colaboradorModel.NomeCompleto = registroDb.nome_completo;
            colaboradorModel.DataNascimento = registroDb.data_nascimento;
            colaboradorModel.Rg = registroDb.rg;
            colaboradorModel.Matricula = registroDb.matricula;
            colaboradorModel.ContatoPrincipalDDI = registroDb.contato_principal_ddi;
            colaboradorModel.ContatoPrincipal = registroDb.contato_principal;
            colaboradorModel.ContatoOutros = registroDb.contato_outro;
            colaboradorModel.FlagCandidato = registroDb.candidato == 1;
            colaboradorModel.Passaporte = registroDb.passaporte;
            colaboradorModel.EmailAlternativo = registroDb.email_alternativo;
            colaboradorModel.EstadoCivil = registroDb.estado_civil;
            colaboradorModel.Genero = registroDb.genero;
            colaboradorModel.Etnia = registroDb.etnia;
            colaboradorModel.OrientacaoSexual = registroDb.orientacao_sexual;
            colaboradorModel.Escolaridade = registroDb.escolaridade;
            colaboradorModel.PessoaRefugiada = registroDb.refugiado == 1;
            colaboradorModel.Nacionalidade = registroDb.nacionalidade;

            if (colaboradorOrg != null)
            {
                colaboradorModel.DataAdmissao = colaboradorOrg.data_admissao;
                colaboradorModel.Diretoria.Id = colaboradorOrg.cod_diretoria;
                colaboradorModel.Diretoria.Diretoria = colaboradorOrg.diretoria;
            }

            var status = registroDb.tb_colaborador_status
                .Where(m => m.ativo == 1)
                .OrderByDescending(m => m.data_alteracao)
                .FirstOrDefault();
            colaboradorModel.FlagAtivo = status != null && status.status_colaborador.id == 1;

            if (registroDb.colaborador_saude != null && registroDb.colaborador_saude_id != 0)
            {
                colaboradorModel.Saude.PCD = ConverterPCDDoBanco(registroDb.colaborador_saude.pcd);
                colaboradorModel.Saude.CondicaoDeSaudeRelevante = registroDb.colaborador_saude.condicao_saude_relevante;
                colaboradorModel.Saude.GrupoDeRiscoCovid = Convert.ToSByte(registroDb.colaborador_saude.grupo_risco_covid);
            }

            var usuarioDb = registroDb.tb_usuario.FirstOrDefault();
            if (usuarioDb != null)
            {
                colaboradorModel.Email = usuarioDb.email;
                colaboradorModel.Slack_id = usuarioDb.slack_id;
                colaboradorModel.FcmToken = usuarioDb.fcm_token;
                colaboradorModel.EmpresasVinculadas = new List<VinculoEmpresaColaboradorDTO>();
                if (usuarioDb.tb_empresa_usuario.Count() > 0)
                {
                    foreach (var rowEmpresa in usuarioDb.tb_empresa_usuario.ToList())
                    {
                        colaboradorModel.EmpresasVinculadas.Add(new VinculoEmpresaColaboradorDTO
                        {
                            Cnpj = rowEmpresa.tb_empresa_cnpj,
                            Confirmado = rowEmpresa.confirmado == (sbyte)1,
                            Pendente = rowEmpresa.pendente == (sbyte)1,
                            DataConvite = rowEmpresa.data_convite,
                            NomeFantasia = rowEmpresa.tb_empresa_cnpjNavigation.nome_fantasia
                        });
                    }
                }
            }

            var enderecoDb = registroDb.endereco;
            if (enderecoDb != null)
            {
                colaboradorModel.Endereco.Cep = enderecoDb.cep;
                colaboradorModel.Endereco.Endereco = enderecoDb.endereco;
                colaboradorModel.Endereco.Complemento = enderecoDb.complemento;
                colaboradorModel.Endereco.Numero = enderecoDb.numero;
                colaboradorModel.Endereco.Bairro = enderecoDb.bairro;
                colaboradorModel.Endereco.Cidade = enderecoDb.cidade;
                colaboradorModel.Endereco.Estado = enderecoDb.estado;
                colaboradorModel.Endereco.ComQuemMora = enderecoDb.com_quem_mora;
                colaboradorModel.Endereco.InternacionalLinhaUm = enderecoDb.internacional_linha_um;
                colaboradorModel.Endereco.InternacionalLinhaDois = enderecoDb.internacional_linha_dois;
            }
            else
            {
                colaboradorModel.Endereco = null;
            }

            var cargoDb = registroDb.tb_colaborador_cargo.FirstOrDefault(x => x.ativo == 1);
            if (cargoDb != null)
            {
                colaboradorModel.Cargo.Id = cargoDb.cargo_id;
                colaboradorModel.Cargo.Cargo = _colaboradorContext.tb_cargo.FirstOrDefault(x => x.id == cargoDb.cargo_id)?.descricao;
            }
            else
            {
                colaboradorModel.Cargo = null;
            }

            var statusColaboradorDb = registroDb.tb_colaborador_status.FirstOrDefault(x => x.ativo == 1);
            if (statusColaboradorDb != null)
            {
                colaboradorModel.Status.Id = statusColaboradorDb.status_colaborador_id;
                colaboradorModel.Status.Descricao = _colaboradorContext.tb_status_colaborador.FirstOrDefault(x => x.id == statusColaboradorDb.status_colaborador_id)?.descricao;
            }
            else
            {
                colaboradorModel.Status = null;
            }

            var candidatoDb = _colaboradorContext.tb_candidato
                .Where(x => x.codigo_interno_colaborador == colaboradorModel.Cpf && x.ativo == 1)
                .OrderByDescending(x => x.data_alteracao)
                .FirstOrDefault();
            if (candidatoDb != null)
            {
                colaboradorModel.Candidato.DataEstagioProcesso = candidatoDb.data_estagio_processo;
                colaboradorModel.Candidato.DescricaoEstagioProcesso = candidatoDb.estagio_processo_seletivo.descricao;
                colaboradorModel.Candidato.PathCurriculo = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + candidatoDb.path_curriculo;
                colaboradorModel.Candidato.PretencaoSalarial = candidatoDb.pretensao_salarial;
                colaboradorModel.Candidato.CargoAtualUltimo = candidatoDb.cargo_atual_ultimo;
                colaboradorModel.Candidato.SalarioAtualUltimo = candidatoDb.salario_atual_ultimo;
                colaboradorModel.Candidato.TipoContratoAtualUltimo = candidatoDb.tipo_contrato_atual_ultimo;
                colaboradorModel.Candidato.ModalidadeAtualUltima = candidatoDb.modalidade_atual_ultima.HasValue
                    ? (ModalidadeEnum)candidatoDb.modalidade_atual_ultima.Value
                    : ModalidadeEnum.VAZIO;
                colaboradorModel.Candidato.AceitaSugestoes = candidatoDb.aceita_sugestoes_vagas;
            }
            else
            {
                colaboradorModel.Candidato = null;
            }

            var imagemDb = registroDb.imagem;
            if (imagemDb != null)
            {
                colaboradorModel.UrlFoto = imagemDb.path;
            }

            return colaboradorModel;
        }

        public ColaboradorDTO EstouSeguindoMeSegue(ColaboradorDTO colaboradorModel, string cpfRequest)
        {
            colaboradorModel.EstouSeguindo = _colaboradorContext.tb_contato_colaborador
                .FirstOrDefault(x => x.seguindo_codigo_interno_colaborador == cpfRequest && x.seguidor_codigo_interno_colaborador == colaboradorModel.Cpf && x.ativo == 1) != null;
            colaboradorModel.MeSegue = _colaboradorContext.tb_contato_colaborador
                .FirstOrDefault(x => x.seguindo_codigo_interno_colaborador == colaboradorModel.Cpf && x.seguidor_codigo_interno_colaborador == cpfRequest && x.ativo == 1) != null;
            return colaboradorModel;
        }

        public List<string> GetSeguidores(string cpf)
        {
            var tabelaContato = _colaboradorContext.tb_contato_colaborador.Where(x => x.ativo == 1 && x.seguindo_codigo_interno_colaborador == cpf).Select(x => x.seguidor_codigo_interno_colaborador).ToList();
            return tabelaContato;
        }

        public List<string> GetSeguindo(string cpf)
        {
            var tabelaContato = _colaboradorContext.tb_contato_colaborador.Where(x => x.ativo == 1 && x.seguidor_codigo_interno_colaborador == cpf).Select(x => x.seguindo_codigo_interno_colaborador).ToList();
            return tabelaContato;
        }

        public IEnumerable<int> PegarNiveisGruposAcesso(ColaboradorDTO colaboradorModel)
        {
            var tabelaUsuario = _colaboradorContext.tb_usuario.FirstOrDefault(m => m.codigo_interno_colaborador == colaboradorModel.Cpf);
            if (tabelaUsuario == null)
                return new List<int>();

            var grupos = tabelaUsuario.tb_usuario_grupo_acesso
                .Where(m => m.tb_usuario_id == tabelaUsuario.id && m.ativo == 1)
                .Join
                (
                    _colaboradorContext.tb_grupo_acesso,
                    tb_usuario_grupo_acesso => tb_usuario_grupo_acesso.tb_grupo_acesso_id,
                    tb_grupo_acesso => tb_grupo_acesso.id,
                    (tb_usuario_grupo_acesso, tb_grupo_acesso) => new
                    {
                        Nivel = tb_grupo_acesso.nivel
                    }
                );

            var niveis = new List<int>();
            foreach (var grupo in grupos)
            {
                niveis.Add(grupo.Nivel);
            }

            return niveis;
        }

        public List<ColaboradorDTO> BuscarColaboradores(string cpf, int cursor, int limite, int candidato, int OrgId, out int totalResultsCount)
        {
            var tabelaAuxiliar = _colaboradorContext.tb_colaborador.Where(x => x.ativo == 1 && x.codigo_interno_colaborador != cpf && x.candidato == candidato);
            totalResultsCount = tabelaAuxiliar.Count();
            var tabela = tabelaAuxiliar.OrderBy(x => x.nome_completo).Skip(cursor).Take(limite).ToList();

            var colaboradores = new List<ColaboradorDTO>();
            foreach (var linhaColaborador in tabela)
            {
                var informacoesCargo = linhaColaborador.tb_colaborador_cargo
                    .Where(m => m.codigo_interno_colaborador == linhaColaborador.codigo_interno_colaborador && m.ativo == 1)
                    .Join(_colaboradorContext.tb_cargo,
                        tb_colaborador_cargo => tb_colaborador_cargo.cargo_id,
                        tb_cargo => tb_cargo.id,
                        (tb_colaborador_cargo, tb_cargo) => new { IdCargo = tb_cargo.id, NomeCargo = tb_cargo.descricao })
                    .FirstOrDefault();

                var informacoesStatus = linhaColaborador.tb_colaborador_status
                    .Where(m => m.codigo_interno_colaborador == linhaColaborador.codigo_interno_colaborador)
                    .Join(_colaboradorContext.tb_status_colaborador,
                        tb_colaborador_status => tb_colaborador_status.status_colaborador_id,
                        tb_status_colaborador => tb_status_colaborador.id,
                        (tb_colaborador_status, tb_status_colaborador) => new { IdStatus = tb_status_colaborador.id, NomeStatus = tb_status_colaborador.descricao })
                    .FirstOrDefault();

                var usuario = linhaColaborador.tb_usuario.FirstOrDefault(x => x.tb_org_id == OrgId) ?? linhaColaborador.tb_usuario.FirstOrDefault();

                colaboradores.Add(new ColaboradorDTO
                {
                    Cpf = linhaColaborador.codigo_interno_colaborador,
                    NomeCompleto = linhaColaborador.nome_completo,
                    DataNascimento = linhaColaborador.data_nascimento,
                    Rg = linhaColaborador.rg,
                    Matricula = linhaColaborador.matricula,
                    Email = usuario?.email,
                    Slack_id = usuario?.slack_id,
                    FcmToken = usuario?.fcm_token,
                    Cargo = informacoesCargo == null ? null : new DataTransferObject.Domain.Cargo.CargoDTO
                    {
                        Id = informacoesCargo.IdCargo,
                        Cargo = informacoesCargo.NomeCargo
                    },
                    Status = informacoesStatus == null ? null : new StatusColaboradorResult
                    {
                        Id = informacoesStatus.IdStatus,
                        Descricao = informacoesStatus.NomeStatus
                    }
                });
            }

            return colaboradores;
        }

        public IEnumerable<ColaboradorDTO> BuscaRowsColaborador(List<string> lstCpf, string cpfSolicitante, int deslocamentoBusca, int candidato)
        {
            var tabela = _colaboradorContext.tb_colaborador
                .Where(x => x.ativo == 1
                    && x.codigo_interno_colaborador != cpfSolicitante
                    && x.candidato == candidato
                    && lstCpf.Contains(x.codigo_interno_colaborador))
                .OrderBy(x => x.nome_completo)
                .Take(deslocamentoBusca)
                .ToList();

            var colaboradores = new List<ColaboradorDTO>();
            foreach (var linhaColaborador in tabela)
            {
                var informacoesCargo = linhaColaborador.tb_colaborador_cargo
                    .Where(m => m.codigo_interno_colaborador == linhaColaborador.codigo_interno_colaborador && m.ativo == 1)
                    .Join(_colaboradorContext.tb_cargo,
                        tb_colaborador_cargo => tb_colaborador_cargo.cargo_id,
                        tb_cargo => tb_cargo.id,
                        (tb_colaborador_cargo, tb_cargo) => new { IdCargo = tb_cargo.id, NomeCargo = tb_cargo.descricao })
                    .FirstOrDefault();

                var informacoesStatus = _colaboradorContext.tb_colaborador_status
                    .Where(m => m.codigo_interno_colaborador == linhaColaborador.codigo_interno_colaborador)
                    .Join(_colaboradorContext.tb_status_colaborador,
                        tb_colaborador_status => tb_colaborador_status.status_colaborador_id,
                        tb_status_colaborador => tb_status_colaborador.id,
                        (tb_colaborador_status, tb_status_colaborador) => new { IdStatus = tb_status_colaborador.id, NomeStatus = tb_status_colaborador.descricao })
                    .FirstOrDefault();

                var usuario = linhaColaborador.tb_usuario.FirstOrDefault();

                colaboradores.Add(new ColaboradorDTO
                {
                    Cpf = linhaColaborador.codigo_interno_colaborador,
                    NomeCompleto = linhaColaborador.nome_completo,
                    DataNascimento = linhaColaborador.data_nascimento,
                    Rg = linhaColaborador.rg,
                    Matricula = linhaColaborador.matricula,
                    Email = usuario?.email,
                    Slack_id = usuario?.slack_id,
                    FcmToken = usuario?.fcm_token,
                    Cargo = informacoesCargo == null ? null : new DataTransferObject.Domain.Cargo.CargoDTO
                    {
                        Id = informacoesCargo.IdCargo,
                        Cargo = informacoesCargo.NomeCargo
                    },
                    Status = informacoesStatus == null ? null : new StatusColaboradorResult
                    {
                        Id = informacoesStatus.IdStatus,
                        Descricao = informacoesStatus.NomeStatus
                    }
                });
            }

            return colaboradores;
        }

        public ColaboradorDTO GetModelByKey(ColaboradorDTO colaboradorModel)
        {
            return GetModel(colaboradorModel);
        }

        public ColaboradorDTO BuscarNomeColaborador(ColaboradorDTO model)
        {
            try
            {
                var colaboradorRow = _colaboradorContext.tb_colaborador.FirstOrDefault(x => x.documento_colaborador == model.Cpf);
                if (colaboradorRow == null)
                {
                    colaboradorRow = _colaboradorContext.tb_colaborador.FirstOrDefault(x => x.codigo_interno_colaborador == model.Cpf);
                }

                if (colaboradorRow == null)
                    throw new Exception("Colaborador não encontrado no sistema.");

                model.Cpf = colaboradorRow.codigo_interno_colaborador;
                model.NomeCompleto = colaboradorRow.nome_completo;
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ColaboradorDTO> BuscarStatusColaborador()
        {
            try
            {
                var ret = new List<ColaboradorDTO>();
                var listaStatusColab = _colaboradorContext.tb_status_colaborador.Where(x => x.ativo == 1).OrderBy(x => x.descricao).ToList();
                foreach (var item in listaStatusColab)
                {
                    ret.Add(new ColaboradorDTO
                    {
                        Status = new StatusColaboradorResult
                        {
                            Id = item.id,
                            Descricao = item.descricao
                        }
                    });
                }
                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ColaboradorDTO VerificaPublicoAcesso(ColaboradorDTO model)
        {
            try
            {
                var usuarioRow = _colaboradorContext.tb_usuario.FirstOrDefault(x => x.codigo_interno_colaborador == model.Cpf);
                if (usuarioRow == null)
                    return model;

                var gruposAcesso = _colaboradorContext.tb_usuario_grupo_acesso.Where(x => x.tb_usuario_id == usuarioRow.id && x.ativo == 1).ToList();
                if (gruposAcesso != null && gruposAcesso.Count > 0)
                {
                    foreach (var grupoAcesso in gruposAcesso)
                    {
                        switch (grupoAcesso.tb_grupo_acesso_id)
                        {
                            case 1:
                                model.AcessoBackoffice = true;
                                break;
                            case 2:
                            case 3:
                                model.AcessoBackoffice = false;
                                break;
                        }
                    }
                }
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ColaboradorDTO> BuscarColaboradoresAtivos(int cursor, int limite, out int totalResultCount)
        {
            try
            {
                var ret = new List<ColaboradorDTO>();
                var rows = new List<tb_colaborador>();

                var colaboradores = _colaboradorContext.tb_colaborador_status.Where(x => x.ativo == 1 && x.status_colaborador_id == (int)StatusColaboradorEnum.ATIVO).ToList();
                foreach (var item in colaboradores)
                {
                    var row = _colaboradorContext.tb_colaborador.FirstOrDefault(x => x.ativo == 1 && x.codigo_interno_colaborador == item.codigo_interno_colaborador);
                    if (row != null)
                    {
                        rows.Add(row);
                    }
                }

                totalResultCount = rows.Count();
                if (rows.Count > 0)
                {
                    var aux = rows.OrderBy(x => x.nome_completo).Skip(cursor).Take(limite).ToList();
                    foreach (var row in aux)
                    {
                        var usuarioDb = _colaboradorContext.tb_usuario.FirstOrDefault(x => x.codigo_interno_colaborador == row.codigo_interno_colaborador);

                        var informacoesStatus = row.tb_colaborador_status
                            .Where(m => m.codigo_interno_colaborador == row.codigo_interno_colaborador)
                            .Join(_colaboradorContext.tb_status_colaborador,
                                tb_colaborador_status => tb_colaborador_status.status_colaborador_id,
                                tb_status_colaborador => tb_status_colaborador.id,
                                (tb_colaborador_status, tb_status_colaborador) => new { IdStatus = tb_status_colaborador.id, NomeStatus = tb_status_colaborador.descricao })
                            .FirstOrDefault();

                        ret.Add(new ColaboradorDTO
                        {
                            Cpf = row.codigo_interno_colaborador,
                            NomeCompleto = row.nome_completo,
                            Email = usuarioDb?.email,
                            Slack_id = usuarioDb?.slack_id,
                            Status = informacoesStatus == null ? null : new StatusColaboradorResult
                            {
                                Id = informacoesStatus.IdStatus,
                                Descricao = informacoesStatus.NomeStatus
                            }
                        });
                    }
                }

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ColaboradorDTO InserirDadosPCD(string cpf, ColaboradorDTO model)
        {
            try
            {
                var rowColab = _colaboradorContext.tb_colaborador.Find(cpf);
                if (rowColab == null)
                    throw new Exception("Colaborador não encontrado no sistema.");

                if (rowColab.colaborador_saude_id != null && rowColab.colaborador_saude_id != 0)
                    throw new Exception("Este colaborador já possui dados de PCD.");

                var row = new tb_colaborador_saude
                {
                    pcd = ConverterPCDParaBanco(model.Saude.PCD),
                    condicao_saude_relevante = model.Saude.CondicaoDeSaudeRelevante,
                    grupo_risco_covid = model.Saude.GrupoDeRiscoCovid
                };

                _colaboradorContext.tb_colaborador_saude.Add(row);
                _colaboradorContext.SaveChanges();
                rowColab.colaborador_saude_id = row.id;
                _colaboradorContext.tb_colaborador.Update(rowColab);
                _colaboradorContext.SaveChanges();
                return model;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ColaboradorDTO InserirCandidato(string cpf, ColaboradorDTO model, CandidatoDTO candidato)
        {
            try
            {
                var rowColab = _colaboradorContext.tb_colaborador.Find(cpf);
                if (rowColab == null)
                    throw new Exception("Colaborador não encontrado no sistema.");

                var novoCandidato = new tb_candidato
                {
                    codigo_interno_colaborador = cpf,
                    path_curriculo = candidato.PathCurriculo,
                    pretensao_salarial = candidato.PretencaoSalarial,
                    cargo_atual_ultimo = candidato.CargoAtualUltimo,
                    salario_atual_ultimo = candidato.SalarioAtualUltimo,
                    tipo_contrato_atual_ultimo = candidato.TipoContratoAtualUltimo,
                    modalidade_atual_ultima = candidato.ModalidadeAtualUltima != null ? (int)candidato.ModalidadeAtualUltima : (int?)null,
                    aceita_sugestoes_vagas = candidato.AceitaSugestoes,
                    estagio_processo_seletivo_id = 1
                };

                _colaboradorContext.tb_candidato.Add(novoCandidato);
                try
                {
                    _colaboradorContext.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    var innerException = ex.InnerException;
                    while (innerException != null)
                    {
                        Console.WriteLine(innerException.Message);
                        innerException = innerException.InnerException;
                    }
                    throw;
                }

                return model;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ColaboradorDTO UpdateFotoModel(ColaboradorDTO colaboradorModel)
        {
            try
            {
                var registroDb = _colaboradorContext.tb_colaborador
                    .FirstOrDefault(x => x.codigo_interno_colaborador == colaboradorModel.Cpf);

                if (registroDb == null)
                    throw new Exception("Colaborador não encontrado no sistema.");

                var imagemId = _colaboradorContext.tb_imagem
                    .Where(x => x.ativo == 1)
                    .OrderByDescending(x => x.id)
                    .Select(x => x.id)
                    .FirstOrDefault();

                if (imagemId > 0)
                {
                    registroDb.imagem_id = imagemId;
                    _colaboradorContext.tb_colaborador.Update(registroDb);
                    _colaboradorContext.SaveChanges();
                }

                return colaboradorModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ColaboradorDTO AlterarDadosPCD(string cpf, ColaboradorDTO model)
        {
            try
            {
                var rowColab = _colaboradorContext.tb_colaborador.Find(cpf);
                if (rowColab == null)
                    throw new Exception("Colaborador não encontrado no sistema.");

                var row = rowColab.colaborador_saude;
                if (row == null)
                    throw new Exception("Dados de PCD não encontrados.");

                row.pcd = ConverterPCDParaBanco(model.Saude.PCD);
                row.condicao_saude_relevante = model.Saude.CondicaoDeSaudeRelevante;
                row.grupo_risco_covid = model.Saude.GrupoDeRiscoCovid;

                _colaboradorContext.tb_colaborador_saude.Update(row);
                _colaboradorContext.SaveChanges();

                rowColab.colaborador_saude_id = row.id;
                _colaboradorContext.tb_colaborador.Update(rowColab);
                _colaboradorContext.SaveChanges();

                return model;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
