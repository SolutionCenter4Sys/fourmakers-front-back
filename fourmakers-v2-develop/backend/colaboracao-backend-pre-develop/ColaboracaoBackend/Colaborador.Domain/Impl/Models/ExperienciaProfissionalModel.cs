using Colaboracao.Core;
using Colaborador.Domain.Interfaces.Factorys;
using Colaborador.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Experiencia;
using System.Collections.Generic;

namespace Colaborador.Domain.Impl.Models
{
    public class ExperienciaProfissionalModel : IExperienciaProfissionalModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExperienciaProfissionalRepository<IExperienciaProfissionalModel, IExperienciaProfissionalDomainFactory> _repositoryExperienciaProfissional;
        private readonly IExperienciaProfissionalDomainFactory _factory;
        private readonly ILogCore _log;

        public ExperienciaProfissionalModel(IUnitOfWork unitOfWork, IExperienciaProfissionalRepository<IExperienciaProfissionalModel, IExperienciaProfissionalDomainFactory> repositoryExperienciaProfissional, ILogCore log, IExperienciaProfissionalDomainFactory factory)
        {
            _unitOfWork = unitOfWork;
            _repositoryExperienciaProfissional = repositoryExperienciaProfissional;
            _factory = factory;
            _log = log;
        }
        public ExperienciaDTO ExperienciaDTO { get; set; }
        public ListaExperienciaDTO ListaExperienciaDTO { get; set; }
        public UpdateExperienciaDTO UpdateExperienciaDTO { get; set; }
        public IExperienciaProfissionalModel Create()
        {
            return _repositoryExperienciaProfissional.Save(this);
        }

        public IExperienciaProfissionalModel GetById(long id)
        {
            this.ExperienciaDTO.Id = id;
            return _repositoryExperienciaProfissional.GetModel(this);
        }

        public List<IExperienciaProfissionalModel> List(string busca, int cursor, int limite, string cpf)
        {
            return _repositoryExperienciaProfissional.Listar(busca, cursor, limite, _factory, cpf);
        }

        public void RemoveExperienciaProfissional()
        {
            _repositoryExperienciaProfissional.DeleteExperienciaProfissioanlModel(this);
        }

        public IExperienciaProfissionalModel Update()
        {
            return _repositoryExperienciaProfissional.Update(this);
        }
    }
}