using CargaCore.Domain.Interfaces;
using Core.Domain;
using Core.Domain.MapaAlocacao;
using Core.Domain.Usuario;
using Core.DomainModel.Projeto;
using DataTransferObject.Domain.Carga;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Org;
using DataTransferObject.Domain.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CargaCore.Domain.Services
{
    public class CargaFourmakerService : ICargaFourmakerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuarioColaboradorRepository _usuarioColaboradorRepository;
        private readonly IProjetoMapaDeAlocacaoRepository _projetoMapaDeAlocacaoRepository;
        private readonly IClienteOrgRepository _clienteRepository;

        public CargaFourmakerService(IUnitOfWork unitOfWork, IUsuarioColaboradorRepository usuarioColaboradorRepository,
            IProjetoMapaDeAlocacaoRepository projetoMapaDeAlocacaoRepository, IClienteOrgRepository clienteOrgRepository)
        {
            _unitOfWork = unitOfWork;
            _usuarioColaboradorRepository = usuarioColaboradorRepository;
            _projetoMapaDeAlocacaoRepository = projetoMapaDeAlocacaoRepository;
            _clienteRepository = clienteOrgRepository;
        }
        public void InsereCargaColaborador(List<ColaboradorCargaDTO> cargaColaborador, int orgId)
        {
            try
            {
                foreach (var colaborador in cargaColaborador)
                {
                    string codigoInternoColaborador = _usuarioColaboradorRepository.GetCodigoInternoByCodExterno(colaborador.IdentificadorColaborador, orgId);
                    if (!_usuarioColaboradorRepository.ExisteColaboradorComCodColaboradorExterno(colaborador.IdentificadorColaborador, orgId))
                    {
                        codigoInternoColaborador = Guid.NewGuid().ToString();
                        _usuarioColaboradorRepository.UpsertUsuarioColaborador(new UsuarioColaboradorDTO
                        {
                            Email = colaborador.Email,
                            Cpf = codigoInternoColaborador,
                            OrgId = orgId
                        }, new ColaboradorDTO
                        {
                            Cpf = codigoInternoColaborador,
                            NomeCompleto = colaborador.NomeCompleto,
                            DocumentoColaborador = colaborador.Cpf
                        }, null);
                    }

                    //Insere na org fourmakers
                    _usuarioColaboradorRepository.UpsertColaboradorOrg(
                        new ColaboradorDTO
                        {
                            Cpf = codigoInternoColaborador,
                            NomeCompleto = colaborador.NomeCompleto,
                            DocumentoColaborador = colaborador.Cpf
                        },
                        new ColaboradorOrgDTO
                        {
                            Cargo = "",
                            CodCargo = "",
                            Cpf = codigoInternoColaborador,
                            CodColaborador = colaborador.IdentificadorColaborador,
                            CodDepartamento = "",
                            CodDiretoria = "",
                            DataAdmissao = DateTime.Now,
                            Departamento = "",
                            Diretoria = "",
                            ModeloContratacao = null,
                            EmpresaRelacionada = null,
                            ModeloTrabalho = null,
                            DiasPorSemana = null,
                            ValorHora = null,
                            CustoHora = null,
                            BaseHoraMes = null,
                            Ativo = colaborador.Ativo,
                            OrgId = 1,
                            CodigoModeloContratacao = null
                        }
                    );
                    //Inativar todos os colabs que não estão na carga, DÉBITO TÉCNICO
                    _usuarioColaboradorRepository.UpsertColaboradorOrg(
                        new ColaboradorDTO
                        {
                            Cpf = codigoInternoColaborador,
                            NomeCompleto = colaborador.NomeCompleto,
                            DocumentoColaborador = colaborador.Cpf
                        },
                        new ColaboradorOrgDTO
                        {
                            Cpf = codigoInternoColaborador,
                            Cargo = colaborador.Cargo,
                            CodCargo = colaborador.IdentificadorCargo,
                            CodColaborador = colaborador.IdentificadorColaborador,
                            CodDepartamento = colaborador.IdentificadorDepartamento,
                            CodDiretoria = colaborador.IdentificadorFilial,
                            DataAdmissao = colaborador.DataAdmissao,
                            Departamento = colaborador.Departamento,
                            Diretoria = colaborador.Filial,
                            Ativo = colaborador.Ativo,
                            ModeloContratacao = colaborador.ModeloContratacao,
                            EmpresaRelacionada = colaborador.EmpresaRelacionada,
                            ModeloTrabalho = colaborador.ModeloTrabalho,
                            DiasPorSemana = colaborador.DiasPorSemana,
                            ValorHora = colaborador.ValorHora,
                            CustoHora = colaborador.CustoHora,
                            BaseHoraMes = colaborador.BaseHoraMes,
                            OrgId = orgId,
                            CodigoModeloContratacao = colaborador.ModeloContratacao,
                            DataInativacao = colaborador.DataInativacao
                        }
                    );

                    //Débito ténico: Atualiza o email do Colaborador, necessário adicionar a coluna tb_org_id em usuário para garantir o acesso do mesmo em várias orgs.
                    _usuarioColaboradorRepository.AtualizaEmailUsuario(new UsuarioColaboradorDTO
                    {
                        Email = colaborador.Email,
                        Cpf = codigoInternoColaborador,
                        OrgId = orgId
                    });

                    _usuarioColaboradorRepository.AtualizaNomelUsuario(codigoInternoColaborador, colaborador.NomeCompleto);
                    if(String.IsNullOrEmpty(colaborador.Cpf))
                    {
                        Console.WriteLine($"Cpf do colaborador {colaborador.NomeCompleto}, CodigoInternoColaborador: {codigoInternoColaborador} é nulo");
                    }
                    _usuarioColaboradorRepository.AtualizaDocumentoUsuario(codigoInternoColaborador, colaborador.Cpf);
                    if (colaborador.DataNascimento != null)
                        _usuarioColaboradorRepository.AtualizaDadosBasicosUsuario(codigoInternoColaborador, (DateTime)colaborador.DataNascimento);
                    _usuarioColaboradorRepository.AtualizaListaColaboradorProjetoOrg(colaborador.Projetos, colaborador.IdentificadorColaborador, orgId);
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void InsereCargaHierarquiaColaborador(List<ColaboradorHierarquiaCargaDTO> cargaHierarquia, int orgId)
        {
            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                _usuarioColaboradorRepository.DeletaHierarquiaColaborador(orgId);
                _usuarioColaboradorRepository.InsereHierarquiaColaborador(cargaHierarquia, orgId);
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw;
            }
        }

        public void InsereCargaHierarquiaProjeto(List<ProjetoGerenteCargaDTO> cargaHierarquia, int orgId)
        {
            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                if (cargaHierarquia.Any())
                    _projetoMapaDeAlocacaoRepository.DeletaProjetosGerentes(cargaHierarquia.ElementAt(0).IdentificadorProjeto, orgId);
                foreach (var gerenteProjeto in cargaHierarquia)
                {
                    _projetoMapaDeAlocacaoRepository.InsereProjetoGerente(gerenteProjeto, orgId);
                }
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw;
            }
        }

        public void InsereCargaProjeto(List<ProjetoCargaDTO> cargaProjeto, int orgId)
        {
            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                foreach (var projeto in cargaProjeto)
                {
                    projeto.IdentificadorProjeto = projeto.IdentificadorProjeto?.Trim();
                    var clienteNaoExiste = _clienteRepository.ObterClientePorCodigo(projeto.IdentificadorCliente, orgId) is null;

                    if (clienteNaoExiste)
                    {
                        var cliente = _clienteRepository.ListarClientesPorNome(projeto.Cliente, orgId).SingleOrDefault();

                        if (cliente is null)
                        {
                            _clienteRepository.CadastrarClienteOrg(projeto.IdentificadorCliente, projeto.Cliente, orgId, true, true, TipoCadastroClienteEnum.CADASTRO_CARGA_CCH);
                        }
                        else
                        {
                            projeto.IdentificadorClienteRefatorado = cliente.CodigoCliente;
                        }
                    }

                    _projetoMapaDeAlocacaoRepository.UpsertProjetoOrg(projeto, TipoCadastroProjetoOrgEnum.CADASTRO_CARGA_CCH, orgId);
                    if (projeto.Propostas != null)
                    {
                        var propostasProjeto = _projetoMapaDeAlocacaoRepository.GetPropostaProjeto(projeto.IdentificadorProjeto, orgId);
                        var projetoDeletados = propostasProjeto.Where(x =>
                                                    !projeto.Propostas.Where(y => y == x.CodProposta).Any()
                                                ).ToList();
                        var projetoAdicionados = projeto.Propostas?.Where(x =>
                                                    !propostasProjeto.Where(y => y.CodProposta == x).Any()
                                                ).ToList() ?? new List<string>();
                        foreach (var proposta in projetoDeletados)
                            _projetoMapaDeAlocacaoRepository.RemovePropostaProjeto(proposta.CodProposta, projeto.IdentificadorProjeto, orgId);
                        foreach (var proposta in projetoAdicionados)
                            _projetoMapaDeAlocacaoRepository.InserePropostaProjeto(proposta, projeto.IdentificadorProjeto, orgId);
                    }
                }
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}