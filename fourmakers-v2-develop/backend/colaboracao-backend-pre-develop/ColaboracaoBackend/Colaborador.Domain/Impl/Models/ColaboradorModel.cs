using Colaboracao.Core;
using Colaborador.Domain.Interfaces.Factorys;
using Colaborador.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Colaborador;
using System;
using System.Collections.Generic;

namespace Colaborador.Domain.Impl.Models
{
    public class ColaboradorModel : IColaboradorModel
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IColaboradorRepository<IColaboradorModel, IColaboradorDomainFactory> _repositoryColaboradorModel;

        public ColaboradorModel(ILogCore log, IUnitOfWork unitOfWork, IColaboradorRepository<IColaboradorModel, IColaboradorDomainFactory> repositoryColaboradorModel, /*ISRSClient srsClient,*/ ColaboradorDTO colaboradorDTO, ForcaPerfilDTO forcaPerfil)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryColaboradorModel = repositoryColaboradorModel;
            ColaboradorDTO = colaboradorDTO;
            ForcaPerfil = forcaPerfil;
            SimpleColaboradorDTO = new SimpleColaboradorDTO();
            ListStatusColaboradorDTO = new StatusColaboradorDTO();
            ColaboradorAtivoDTO = new ColaboradorAtivoDTO();
            //_srsClient = srsClient;
        }

        public string Id { get; set; }
        public string Cpf { get; set; }
        public string CpfRequest { get; set; }
        public long? ImagemId { get; set; }
        public string ContatoPrincipalDDI { get; set; }
        public string ContatoPrincipal { get; set; }
        public string NomeCompleto { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Rg { get; set; }
        public string Matricula { get; set; }
        public DateTime? Admissao { get; set; }
        public long? EnderecoId { get; set; }
        public long? DiretoriaId { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string ContatoOutro { get; set; }
        public sbyte Candidato { get; set; }
        public sbyte Ativo { get; set; }
        public int OrgId { get; set; }
        public ColaboradorDTO ColaboradorDTO { get; set; }
        public string PathImagem { get; set; }
        public ForcaPerfilDTO ForcaPerfil { get; set; }
        public StatusColaboradorDTO StatusColaborador { get; set; }
        public SimpleColaboradorDTO SimpleColaboradorDTO { get; set; }
        public StatusColaboradorDTO ListStatusColaboradorDTO { get; set; }
        public bool Administrador { get; set; }
        public ColaboradorAtivoDTO ColaboradorAtivoDTO { get; set; }
        public ColaboradorSaudeDTO ColaboradorSaudeDTO { get; set; }
        public ColaboradorCurriculoDTO ColaboradorCurriculoDTO { get; set; }

        public List<IColaboradorModel> BuscarColaboradores(string cpf, int cursor, int limite, int candidato, int orgId, IColaboradorDomainFactory colaboradorDomainFactory, out int totalResultsCount)
        {
            return _repositoryColaboradorModel.BuscarColaboradores(cpf, cursor, limite, candidato, orgId, colaboradorDomainFactory, out totalResultsCount);
        }

        public IColaboradorModel BuscarNomeColaborador()
        {
            try
            {
                return _repositoryColaboradorModel.BuscarNomeColaborador(this);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<IColaboradorModel> BuscaRowsColaborador(List<string> lstCpf, string cpfSolicitante, int deslocamentoBusca, int candidato, IColaboradorDomainFactory _colaboradorDomainFactory)
        {
            return _repositoryColaboradorModel.BuscaRowsColaborador(lstCpf, cpfSolicitante, deslocamentoBusca, candidato, _colaboradorDomainFactory);
        }

        public int ContarCandidatosMenosEste(string cpf)
        {
            return _repositoryColaboradorModel.ContarCandidatosMenosEste(cpf);
        }

        public int ContarColaboradoresMenosEste(string cpf)
        {
            return _repositoryColaboradorModel.ContarColaboradoresMenosEste(cpf);
        }

        public void EstouSeguindoMeSegue(string cpfRequest)
        {
            var colaboradorModel = _repositoryColaboradorModel.EstouSeguindoMeSegue(this, cpfRequest);

            this.ColaboradorDTO.EstouSeguindo = colaboradorModel.ColaboradorDTO.EstouSeguindo;
            this.ColaboradorDTO.MeSegue = colaboradorModel.ColaboradorDTO.MeSegue;
        }

        public string FollowColaborador(string cpfSeguidor, string cpfSeguir)
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    if (String.IsNullOrEmpty(cpfSeguidor))
                        throw new Exception("O número de cpf do seguidor é obrigatório.");
                    if (String.IsNullOrEmpty(cpfSeguir))
                        throw new Exception("O número de cpf do colaborador é obrigatório.");

                    this.Cpf = cpfSeguidor;
                    var seguidorRow = _repositoryColaboradorModel.GetModel(this);
                    if (seguidorRow == null)
                        throw new Exception("Seguidor não encontrado no sistema.");

                    this.Cpf = cpfSeguir;
                    var colabRow = _repositoryColaboradorModel.GetModel(this);
                    if (colabRow == null)
                        throw new Exception("Colaborador não encontrado no sistema.");

                    _repositoryColaboradorModel.FollowColaborador(cpfSeguidor, cpfSeguir);

                    dbTrans.Commit();

                    return seguidorRow.NomeCompleto;
                }
                catch (Exception e)
                {
                    dbTrans.Rollback();
                    throw e;
                }
            }
        }

        public IColaboradorModel GetModelByCpf(string cpf, int orgId)
        {
            this.Cpf = cpf;
            this.OrgId = orgId;
            var modelBanco = _repositoryColaboradorModel.GetModel(this);

            return modelBanco;
        }

        public IColaboradorModel GetModelByKey(string cpf)
        {
            this.Cpf = cpf;
            var modelBanco = _repositoryColaboradorModel.GetModelByKey(this);

            return modelBanco;
        }

        public List<string> GetSeguidores(string cpf, IColaboradorDomainFactory colaboradorDomainFactory)
        {
            return _repositoryColaboradorModel.GetSeguidores(cpf, colaboradorDomainFactory);
        }

        public List<string> GetSeguindo(string cpf, IColaboradorDomainFactory colaboradorDomainFactory)
        {
            return _repositoryColaboradorModel.GetSeguindo(cpf, colaboradorDomainFactory);
        }

        public void PermissaoAcesso()
        {
            //Permissão de acesso do colaborador
            this.ColaboradorDTO.AcessoBackoffice = false;
            this.ColaboradorDTO.AcessoBuscaAvançada = false;

            var niveis = _repositoryColaboradorModel.PegarNiveisGruposAcesso(this);

            foreach (var nivel in niveis)
            {
                switch (nivel)
                {
                    case 1:
                        this.ColaboradorDTO.AcessoBackoffice = true;
                        this.ColaboradorDTO.AcessoBuscaAvançada = true;
                        break;

                    case 2:
                        this.ColaboradorDTO.AcessoBackoffice = true;
                        break;
                }
            }
        }

        public void UnfollowColaborador(string cpfSeguidor, string cpfSeguir)
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    if (String.IsNullOrEmpty(cpfSeguidor))
                        throw new Exception("O número de cpf do seguidor é obrigatório.");
                    if (String.IsNullOrEmpty(cpfSeguir))
                        throw new Exception("O número de cpf do colaborador é obrigatório.");

                    this.Cpf = cpfSeguidor;
                    var seguidorRow = _repositoryColaboradorModel.GetModel(this);
                    if (seguidorRow == null)
                        throw new Exception("Seguidor não encontrado no sistema.");

                    this.Cpf = cpfSeguir;
                    var colabRow = _repositoryColaboradorModel.GetModel(this);
                    if (colabRow == null)
                        throw new Exception("Colaborador não encontrado no sistema.");

                    _repositoryColaboradorModel.UnfollowColaborador(cpfSeguidor, cpfSeguir);

                    dbTrans.Commit();
                }
                catch (Exception e)
                {
                    dbTrans.Rollback();
                    throw e;
                }
            }
        }

        public IColaboradorModel UpdateModel()
        {
            var ret = _repositoryColaboradorModel.UpdateModel(this);
            this.Id = ret.Id;

            return this;
        }

        public List<IColaboradorModel> BuscarStatusColaborador(IColaboradorDomainFactory colaboradorDomainFactory)
        {
            try
            {
                return _repositoryColaboradorModel.BuscarStatusColaborador(colaboradorDomainFactory);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IColaboradorModel ValidaGrupoAcesso(string cpf)
        {
            this.CpfRequest = cpf;
            return _repositoryColaboradorModel.VerificaPublicoAcesso(this);
        }

        public List<IColaboradorModel> BuscarColaboradoresAtivos(int cursor, int limite, out int totalResultCount, IColaboradorDomainFactory colaboradorDomainFactory)
        {
            try
            {
                return _repositoryColaboradorModel.BuscarColaboradoresAtivos(cursor, limite, out totalResultCount, colaboradorDomainFactory);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IColaboradorModel InserirDadosPCD(EnumPCD pCD, bool grupoDeRisco, string descricaoCondicaoDeSaude, string cpf, IColaboradorDomainFactory _colaboradorDomainFactory)
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var model = _colaboradorDomainFactory.buildColaboradorModel();
                    model.ColaboradorDTO.Saude.CondicaoDeSaudeRelevante = descricaoCondicaoDeSaude;
                    model.ColaboradorDTO.Saude.PCD = pCD;
                    model.ColaboradorDTO.Saude.GrupoDeRiscoCovid = Convert.ToSByte(grupoDeRisco);
                    var ret = _repositoryColaboradorModel.InserirDadosPCD(cpf, model);
                    dbTrans.Commit();
                    return ret;
                }
                catch (Exception e)
                {
                    dbTrans.Rollback();
                    throw new Exception(e.Message);
                }
            }
        }

        public IColaboradorModel UpdateFotoModel()
        {
            var ret = _repositoryColaboradorModel.UpdateFotoModel(this);
            this.Id = ret.Id;

            return this;
        }
        public IColaboradorModel AlterarDadosPCD(EnumPCD pCD, bool grupoDeRisco, string descricaoCondicaoDeSaude, string cpf, IColaboradorDomainFactory colaboradorDomainFactory)
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var model = colaboradorDomainFactory.buildColaboradorModel();
                    model.ColaboradorDTO.Saude.CondicaoDeSaudeRelevante = descricaoCondicaoDeSaude;
                    model.ColaboradorDTO.Saude.PCD = pCD;
                    model.ColaboradorDTO.Saude.GrupoDeRiscoCovid = Convert.ToSByte(grupoDeRisco);
                    dbTrans.Commit();
                    return _repositoryColaboradorModel.AlterarDadosPCD(cpf, model);
                }
                catch (Exception e)
                {
                    dbTrans.Rollback();
                    throw new Exception(e.Message);
                }
            }
        }
    }
}