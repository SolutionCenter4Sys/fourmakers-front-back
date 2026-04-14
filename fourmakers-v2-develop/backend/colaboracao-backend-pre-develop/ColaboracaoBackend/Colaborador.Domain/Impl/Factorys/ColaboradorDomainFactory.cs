using Colaboracao.Core;
using Colaborador.Domain.Impl.Models;
using Colaborador.Domain.Interfaces.Factorys;
using Colaborador.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Diretoria;
using System;

namespace Colaborador.Domain.Impl.Factorys
{
    public class ColaboradorDomainFactory : IColaboradorDomainFactory
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IColaboradorRepository<IColaboradorModel, IColaboradorDomainFactory> _repositoryColaboradorModel;

        public ColaboradorDomainFactory(ILogCore log, IUnitOfWork unitOfWork, IColaboradorRepository<IColaboradorModel, IColaboradorDomainFactory> repositoryColaboradorModel)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryColaboradorModel = repositoryColaboradorModel;
        }

        public IColaboradorModel buildColaboradorModel()
        {
            return new ColaboradorModel(_log, _unitOfWork, _repositoryColaboradorModel, new DataTransferObject.Domain.Colaborador.ColaboradorDTO(), new DataTransferObject.Domain.Colaborador.ForcaPerfilDTO());
        }

        public IColaboradorModel buildColaboradorModel(DateTime? admissao, sbyte ativo, sbyte candidato, string contato_outro, string contato_principal, string cpf, DateTime data_alteracao, DateTime data_criacao, DateTime data_nascimento, string diretoria_id, long? endereco_id, long? imagem_id, string matricula, string nome_completo, string rg, string email, string slack_id, string fcmToken, int idCargo, string nomeCargo, string descricaoDiretoria, int idStatus, string nomeStatus, string cep, string endereco, string complemento, int? numero, string bairro, string cidade, string estado)
        {
            return new ColaboradorModel(
                _log,
                _unitOfWork,
                _repositoryColaboradorModel,
                new DataTransferObject.Domain.Colaborador.ColaboradorDTO
                {
                    DataAdmissao = admissao,
                    ContatoOutros = contato_outro,
                    ContatoPrincipal = contato_principal,
                    Cpf = cpf,
                    DataNascimento = data_nascimento,
                    Matricula = matricula,
                    NomeCompleto = nome_completo,
                    Rg = rg,
                    Email = email,
                    Slack_id = slack_id,
                    Cargo = new DataTransferObject.Domain.Cargo.CargoDTO
                    {
                        Id = idCargo,
                        Cargo = nomeCargo
                    },
                    Diretoria = new DataTransferObject.Domain.Diretoria.DiretoriaDTO
                    {
                        Id = diretoria_id,
                        Diretoria = descricaoDiretoria
                    },
                    Status = new DataTransferObject.Domain.Colaborador.StatusColaboradorResult
                    {
                        Id = idStatus,
                        Descricao = nomeStatus
                    },
                    Endereco = new DataTransferObject.Domain.Endereco.EnderecoDTO
                    {
                        Cep = cep,
                        Endereco = endereco,
                        Complemento = complemento,
                        Numero = numero,
                        Cidade = cidade,
                        Estado = estado,
                        Bairro = bairro
                    },

                    //Candidato = new DataTransferObject.Domain.Candidato.CandidatoDTO
                    //{
                    //    CargoAtualUltimo= car

                    //},

                    FcmToken = fcmToken
                },
                new DataTransferObject.Domain.Colaborador.ForcaPerfilDTO()
                );
        }

        public IColaboradorModel buildColaboradorModel(DateTime? admissao, sbyte ativo, sbyte candidato, string contato_outro, string contato_principal, string cpf, DateTime data_alteracao, DateTime data_criacao, DateTime data_nascimento, string diretoria_id, long? endereco_id, long? imagem_id, string matricula, string nome_completo, string rg, string email, string slack_id, int idCargo, string nomeCargo, int id, string descricao, int idStatus, string nomeStatus, string cep, string endereco, string complemento, int? numero)
        {
            throw new NotImplementedException();
        }

        public IColaboradorModel buildColaboradorModel(DateTime? admissao, sbyte ativo, sbyte candidato, string contato_outro, string contato_principal, string cpf, DateTime data_alteracao, DateTime data_criacao, DateTime data_nascimento, string diretoria_id, long? endereco_id, long? imagem_id, string matricula, string nome_completo, string rg, string email, string slack_id, int idCargo, string nomeCargo, int id, string descricao, int idStatus, string nomeStatus, string cep, string endereco, string complemento, int? numero, string bairro, string cidade, string estado)
        {
            throw new NotImplementedException();
        }

        public IColaboradorModel buildColaboradorModel(string cpf, string idDiretoria, string descricaoDiretoria, string nome_completo, string email, string slack_id, int idStatus, string descricaoStatus)
        {
            return new ColaboradorModel(_log, _unitOfWork, _repositoryColaboradorModel, new ColaboradorDTO(), new ForcaPerfilDTO())
            {
                ColaboradorAtivoDTO = new ColaboradorAtivoDTO()
                {
                    Cpf = cpf,
                    NomeCompleto = nome_completo,
                    Email = email,
                    Diretoria = new DiretoriaDTO()
                    {
                        Id = idDiretoria,
                        Diretoria = descricaoDiretoria
                    },
                    Status = new StatusColaboradorResult()
                    {
                        Id = idStatus,
                        Descricao = descricaoStatus
                    },
                    Slack_id = slack_id
                }
            };
        }

        public IColaboradorModel buildStatusColaboradorModel(int id, string descricao)
        {
            return new ColaboradorModel(_log, _unitOfWork, _repositoryColaboradorModel, new ColaboradorDTO(), new ForcaPerfilDTO())
            {
                ListStatusColaboradorDTO = new StatusColaboradorDTO()
                {
                    Id = id,
                    Descricao = descricao,
                }
            };
        }

        public IColaboradorModel buildStatusColaboradorModel()
        {
            return new ColaboradorModel(_log, _unitOfWork, _repositoryColaboradorModel, new ColaboradorDTO(), new ForcaPerfilDTO());
        }
    }
}