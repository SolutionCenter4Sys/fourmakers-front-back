using Colaboracao.Core;
using Competencia.Domain.Impl.Models;
using Competencia.Domain.Interfaces.Factorys;
using Competencia.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Dominio;
using DataTransferObject.Domain.Endosso;
using DataTransferObject.Domain.Nivel;
using System;

namespace Competencia.Domain.Impl.Factorys
{
    public class DominioDomainFactory : IDominioDomainFactory
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<IDominioModel, IDominioDomainFactory> _repositoryDominio;
        private readonly IRepository<IDominioColaboradorModel, IDominioDomainFactory> _repositoryDominioColaborador;
        private readonly IRepository<IDominioNivelModel, IDominioDomainFactory> _repositoryDominioNivel;
        private readonly IRepository<IDominioEndossoModel, IDominioDomainFactory> _repositoryDominioEndosso;
        private readonly IRepository<IDominioEndossoColaboradorModel, IDominioDomainFactory> _repositoryDominioEndossoColaborador;
        private readonly IRepository<IDominioTipoEndossoModel, IDominioDomainFactory> _repositoryDominioTipoEndosso;
        private readonly IDominioGenericoRepository<IDominioColaboradorModel, IDominioDomainFactory> _repositoryDominioGenereico;

        public DominioDomainFactory(ILogCore log, IUnitOfWork unitOfWork,
                         IRepository<IDominioModel, IDominioDomainFactory> repositoryDominio,
                         IRepository<IDominioColaboradorModel, IDominioDomainFactory> repositoryDominioColaborador,
                         IRepository<IDominioNivelModel, IDominioDomainFactory> repositoryDominioNivel,
                         IRepository<IDominioEndossoModel, IDominioDomainFactory> repositoryDominioEndosso,
                         IRepository<IDominioEndossoColaboradorModel, IDominioDomainFactory> repositoryDominioEndossoColaborador,
                         IRepository<IDominioTipoEndossoModel, IDominioDomainFactory> repositoryDominioTipoEndosso,
                         IDominioGenericoRepository<IDominioColaboradorModel, IDominioDomainFactory> repositoryDominioGenereico)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryDominio = repositoryDominio;
            _repositoryDominioColaborador = repositoryDominioColaborador;
            _repositoryDominioNivel = repositoryDominioNivel;
            _repositoryDominioEndosso = repositoryDominioEndosso;
            _repositoryDominioEndossoColaborador = repositoryDominioEndossoColaborador;
            _repositoryDominioTipoEndosso = repositoryDominioTipoEndosso;
            _repositoryDominioGenereico = repositoryDominioGenereico;
        }

        public IDominioModel buildDominioModel()
        {
            return new DominioModel(_log, _unitOfWork, _repositoryDominio);
        }

        public IDominioModel buildDominioModel(long id, string descricao, long usuarioCriacaoId, bool pendente)
        {
            return new DominioModel(_log, _unitOfWork, _repositoryDominio)
            {
                DominioDTO = new DominioDTO()
                {
                    Id = id,
                    Descricao = descricao,
                    UsuarioCriacaoId = usuarioCriacaoId,
                    Pendente = pendente
                }
            };
        }

        public IDominioColaboradorModel buildDominioColaboradorModel()
        {
            return new DominioColaboradorModel(_log, _unitOfWork, _repositoryDominioColaborador, _repositoryDominioGenereico);
        }

        public IDominioColaboradorModel buildDominioColaboradorModel(long idDominioColaborador, long idDominio, string colaboradorCpf, long? idNivel, DateTime dataAlteracao)
        {
            return new DominioColaboradorModel(_log, _unitOfWork, _repositoryDominioColaborador, _repositoryDominioGenereico)
            {
                DominioColaboradorDTO = new DominioColaboradorDTO()
                {
                    Id = idDominioColaborador,
                    IdDominio = idDominio,
                    ColaboradorCpf = colaboradorCpf,
                    Data = dataAlteracao,
                    Nivel = new NivelDTO { Id = idNivel }
                }
            };
        }
        public IDominioColaboradorModel buildDominioColaboradorModel(long idDominioColaborador, long idDominio, string colaboradorCpf, DateTime dataAlteracao)
        {
            return new DominioColaboradorModel(_log, _unitOfWork, _repositoryDominioColaborador, _repositoryDominioGenereico)
            {
                DominioColaboradorDTO = new DominioColaboradorDTO()
                {
                    Id = idDominioColaborador,
                    IdDominio = idDominio,
                    ColaboradorCpf = colaboradorCpf,
                    Data = dataAlteracao
                }
            };
        }

        public IDominioNivelModel buildDominioNivelModel()
        {
            return new DominioNivelModel(_log, _unitOfWork, _repositoryDominioNivel);
        }

        public IDominioNivelModel buildDominioNivelModel(long? nivelId)
        {
            return new DominioNivelModel(_log, _unitOfWork, _repositoryDominioNivel)
            {
                NivelDTO = new NivelDTO { Id = nivelId }
            };
        }

        public IDominioNivelModel buildDominioNivelModel(long nivelId, string nivelDescricao)
        {
            return new DominioNivelModel(_log, _unitOfWork, _repositoryDominioNivel)
            {
                NivelDTO = new NivelDTO { Id = nivelId, Descricao = nivelDescricao }
            };
        }

        public IDominioEndossoModel buildDominioEndossoModel(long idDominioColaborador)
        {
            return new DominioEndossoModel(_log, _unitOfWork, _repositoryDominioEndosso) { IdDominioColaborador = idDominioColaborador };
        }

        public IDominioEndossoColaboradorModel buildDominioEndossoColaboradorModel(long idDominioColaborador)
        {
            return new DominioEndossoColaboradorModel(_log, _unitOfWork, _repositoryDominioEndossoColaborador) { IdDominioColaborador = idDominioColaborador };
        }

        public IDominioEndossoColaboradorModel buildDominioEndossoColaboradorModel(string cpfColaborador, DateTime dataEndosso, int? idTipoEndosso)
        {
            return new DominioEndossoColaboradorModel(_log, _unitOfWork, _repositoryDominioEndossoColaborador)
            {
                cpfColaborador = cpfColaborador,
                dataEndosso = dataEndosso,
                TipoEndossoId = idTipoEndosso
            };
        }

        public IDominioTipoEndossoModel buildDominioTipoEndossoModel(int? tipoEndossoId)
        {
            return new DominioTipoEndossoModel(_log, _unitOfWork, _repositoryDominioTipoEndosso)
            {
                TipoEndossoDTO = new TipoEndossoDTO() { Id = tipoEndossoId }
            };
        }

        public IDominioModel buildDominioModel(long id, string descricao, long usuarioCriacaoId)
        {
            return new DominioModel(_log, _unitOfWork, _repositoryDominio)
            {
                DominioDTO = new DominioDTO()
                {
                    Id = id,
                    Descricao = descricao,
                    UsuarioCriacaoId = usuarioCriacaoId,
                }
            };
        }

        public IDominioColaboradorModel buildDominioColaboradorModel(long idDominioColaborador, long idDominio, string colaboradorCpf, long? idNivel, DateTime dataAlteracao, bool pendente)
        {
            return new DominioColaboradorModel(_log, _unitOfWork, _repositoryDominioColaborador, _repositoryDominioGenereico)
            {
                DominioColaboradorDTO = new DominioColaboradorDTO()
                {
                    Id = idDominioColaborador,
                    IdDominio = idDominio,
                    ColaboradorCpf = colaboradorCpf,
                    Data = dataAlteracao,
                    Nivel = new NivelDTO()
                    {
                        Id = idNivel,
                    },
                    Dominio = new ItemPerfilDTO()
                    {
                        Pendente = pendente,
                    }
                }
            };
        }

        public IDominioColaboradorModel buildDominioColaboradorModel(long idDominioColaborador, long idDominio, string colaboradorCpf, DateTime dataAlteracao, bool pendente)
        {
            return new DominioColaboradorModel(_log, _unitOfWork, _repositoryDominioColaborador, _repositoryDominioGenereico)
            {
                DominioColaboradorDTO = new DominioColaboradorDTO()
                {
                    Id = idDominioColaborador,
                    IdDominio = idDominio,
                    ColaboradorCpf = colaboradorCpf,
                    Data = dataAlteracao,
                    Dominio = new ItemPerfilDTO()
                    {
                        Pendente = pendente,
                    }
                }
            };
        }
    }
}