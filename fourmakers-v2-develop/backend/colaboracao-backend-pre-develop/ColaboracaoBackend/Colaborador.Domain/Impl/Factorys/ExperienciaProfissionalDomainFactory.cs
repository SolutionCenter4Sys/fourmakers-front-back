using Colaboracao.Core;
using Colaborador.Domain.Impl.Models;
using Colaborador.Domain.Interfaces.Factorys;
using Colaborador.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Experiencia;
using System;
using System.Collections.Generic;

namespace Colaborador.Domain.Impl.Factorys
{
    public class ExperienciaProfissionalDomainFactory : IExperienciaProfissionalDomainFactory
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExperienciaProfissionalRepository<IExperienciaProfissionalModel, IExperienciaProfissionalDomainFactory> _repositoryExperienciaProfissional;

        public ExperienciaProfissionalDomainFactory(ILogCore log, IUnitOfWork unitOfWork,
                         IExperienciaProfissionalRepository<IExperienciaProfissionalModel, IExperienciaProfissionalDomainFactory> repositoryExperienciaProfissional)

        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryExperienciaProfissional = repositoryExperienciaProfissional;
        }
        public IExperienciaProfissionalModel BuildModel(long id, string descricao, string cpf, string titulo, string empresa, DateTime dataInicio, DateTime? dataSaida, List<string> projetos)
        {
            var ret = BuildModel();
            ret.ExperienciaDTO = new ExperienciaDTO
            {
                Id = id,
                Atividades = descricao,
                ColaboradorCpf = cpf,
                Empresa = empresa,
                Funcao = titulo,
                DataInicio = dataInicio,
                DataSaida = dataSaida,
                Projetos = projetos
            };
            return ret;
        }

        public IExperienciaProfissionalModel BuildModel(string descricao, string cpf, string titulo, string empresa, DateTime dataInicio, DateTime? dataSaida)
        {
            var ret = BuildModel();

            ret.ExperienciaDTO = new ExperienciaDTO
            {
                Atividades = descricao,
                ColaboradorCpf = cpf,
                Empresa = empresa,
                Funcao = titulo,
                DataInicio = dataInicio,
                DataSaida = dataSaida
            };
            return ret;
        }

        public IExperienciaProfissionalModel BuildModel()
        {
            var ret = new ExperienciaProfissionalModel(_unitOfWork, _repositoryExperienciaProfissional, _log, this);
            ret.ExperienciaDTO = new ExperienciaDTO();
            return ret;
        }
        public IExperienciaProfissionalModel BuildModelLista()
        {
            var ret = new ExperienciaProfissionalModel(_unitOfWork, _repositoryExperienciaProfissional, _log, this);
            ret.ListaExperienciaDTO = new ListaExperienciaDTO();
            return ret;
        }
        public IExperienciaProfissionalModel BuildModelLista(long id, string descricao, string cpf, string empresa, string titulo, DateTime dataInicio, DateTime? dataSaida)
        {
            var ret = BuildModelLista();
            ret.ListaExperienciaDTO = new ListaExperienciaDTO
            {
                Id = id,
                Atividades = descricao,
                ColaboradorCpf = cpf,
                Empresa = empresa,
                Funcao = titulo,
                DataInicio = dataInicio,
                DataSaida = dataSaida
            };
            return ret;
        }

        public IExperienciaProfissionalModel BuildModel(long id, string descricao, string cpf, string empresa, string titulo, DateTime dataInicio, DateTime? dataSaida, bool ativo)
        {
            var ret = BuildModel();

            ret.ExperienciaDTO = new ExperienciaDTO
            {
                Id = id,
                Atividades = descricao,
                ColaboradorCpf = cpf,
                Empresa = empresa,
                Funcao = titulo,
                DataInicio = dataInicio,
                DataSaida = dataSaida,
                Ativo = ativo
            };
            return ret;
        }

        public IExperienciaProfissionalModel BuildModelUpdate(long id, string descricao, string cpf, string empresa, string titulo, DateTime dataInicio, DateTime? dataSaida)
        {
            var ret = BuildModelUpdate();
            ret.UpdateExperienciaDTO = new UpdateExperienciaDTO
            {
                Id = id,
                Atividades = descricao,
                ColaboradorCpf = cpf,
                Empresa = empresa,
                Funcao = titulo,
                DataInicio = dataInicio,
                DataSaida = dataSaida,
            };
            return ret;
        }

        public IExperienciaProfissionalModel BuildModelUpdate()
        {
            var ret = new ExperienciaProfissionalModel(_unitOfWork, _repositoryExperienciaProfissional, _log, this);
            ret.UpdateExperienciaDTO = new UpdateExperienciaDTO();
            return ret;
        }

        public IExperienciaProfissionalModel BuildModelUpdate(long id, string descricao, string cpf, string empresa, string titulo, DateTime dataInicio, DateTime? dataSaida, bool ativo)
        {
            throw new NotImplementedException();
        }

        public IExperienciaProfissionalModel BuildModel(string descricao, string cpf, string titulo, string empresa, DateTime dataInicio, DateTime? dataSaida, List<string> projetos)
        {
            var ret = BuildModel();

            ret.ExperienciaDTO = new ExperienciaDTO
            {
                Atividades = descricao,
                ColaboradorCpf = cpf,
                Empresa = empresa,
                Funcao = titulo,
                DataInicio = dataInicio,
                DataSaida = dataSaida,
                Projetos = projetos
            };
            return ret;
        }

        public IExperienciaProfissionalModel BuildModelLista(long id, string descricao, string cpf, string empresa, string titulo, DateTime dataInicio, DateTime? dataSaida, List<string> projetos)
        {
            var ret = BuildModelLista();
            ret.ListaExperienciaDTO = new ListaExperienciaDTO
            {
                Id = id,
                Atividades = descricao,
                ColaboradorCpf = cpf,
                Empresa = empresa,
                Funcao = titulo,
                DataInicio = dataInicio,
                DataSaida = dataSaida,
                Projetos = projetos
            };
            return ret;
        }
    }
}