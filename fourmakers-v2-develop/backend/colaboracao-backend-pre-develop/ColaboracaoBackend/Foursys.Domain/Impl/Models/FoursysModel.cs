using Colaboracao.Core;
using Core.Domain;
using DataTransferObject.Domain.Cargo;
using DataTransferObject.Domain.Diretoria;
using DataTransferObject.Domain.Foursys;
using Foursys.Domain.Interfaces.Factorys;
using Foursys.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foursys.Domain.Impl.Models
{
    public class FoursysModel : IFoursysModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFoursysRepository<IFoursysModel, IFoursysDomainFactory> _repositoryFoursys;
        private readonly ILogCore _log;

        public FoursysModel(ILogCore log, IUnitOfWork unitOfWork, IFoursysRepository<IFoursysModel, IFoursysDomainFactory> repositoryFoursys)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryFoursys = repositoryFoursys;
            CargoDTO = new CargoDTO();
            DiretoriaDTO = new DiretoriaDTO();
            UnidadesDTO = new UnidadesDTO();
        }

        public CargoDTO CargoDTO { get; set; }
        public DiretoriaDTO DiretoriaDTO { get; set; }
        public UnidadesDTO UnidadesDTO { get; set; }
        public int FilteredResultCount { get; set; }
        public int TotalResultCount { get; set; }

        public IFoursysModel AlteraCargo(int id, string cargo)
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    this.CargoDTO.Id = id;
                    this.CargoDTO.Cargo = cargo;
                    _repositoryFoursys.AlteraCargo(this);

                    dbTrans.Commit();

                    return this;
                }
                catch (Exception)
                {
                    dbTrans.Rollback();
                    throw;
                }
            }
        }

        public IFoursysModel BuscarCargo(int id)
        {
            try
            {
                this.CargoDTO.Id = id;
                return _repositoryFoursys.BuscarCargo(this);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<IFoursysModel> BuscarCargos(string busca, int cursor, int limite, IFoursysDomainFactory factory)
        {
            try
            {
                return _repositoryFoursys.BuscarCargos(busca, cursor, limite, factory);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<IFoursysModel> BuscarDiretorias(string busca, int cursor, int limite, IFoursysDomainFactory factory)
        {
            try
            {
                return _repositoryFoursys.BuscarDiretorias(busca, cursor, limite, factory);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeletaCargo(int id, string cargo)
        {
            try
            {
                this.CargoDTO.Id = id;
                this.CargoDTO.Cargo = cargo;
                _repositoryFoursys.DeletaCargo(this);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IFoursysModel InsereCargo(string cargo)
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    this.CargoDTO.Cargo = cargo;
                    _repositoryFoursys.InsereCargo(this);

                    dbTrans.Commit();

                    return this;
                }
                catch (Exception)
                {
                    dbTrans.Rollback();
                    throw;
                }
            }
        }

        public List<IFoursysModel> ListarUnidades(IFoursysDomainFactory factory)
        {
            try
            {
                return _repositoryFoursys.ListarUnidades(factory);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<IFoursysModel> ListarUnidadesPorOrg(IFoursysDomainFactory factory, int orgId)
        {
            try
            {
                return _repositoryFoursys.ListarUnidadesPorOrg(factory, orgId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task<List<UnidadesDTO>> ListarUnidadesPorOrgIdComRestricao(int orgId, List<string>? diretorias)
        {
            try
            {
                return await _repositoryFoursys.ListarUnidadesPorOrgIdComRestricao(orgId, diretorias);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}