using Core.Domain;
using Core.Domain.Vaga;
using DataTransferObject.Domain.Vaga;
using RotinasBackoffice.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;

namespace RotinasBackoffice.Domain.Impl.Services
{
    public class VagasSRSServices : IVagasSRSServices
    {
        private readonly IVagaFourmakersRepository _vagaFourmakersRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VagasSRSServices(IVagaFourmakersRepository vagaFourmakersRepository, IUnitOfWork unitOfWork)
        {
            _vagaFourmakersRepository = vagaFourmakersRepository;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<VagaFourmakersSRSDTO> ObterTodasAsVagas()
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var vagas = _vagaFourmakersRepository.ObterTodasAsVagas();
                    dbTrans.Commit();
                    return vagas;
                }
                catch (Exception e)
                {
                    dbTrans.Rollback();
                    throw;
                }
            }
        }

        public void AtualizarVagas(List<VagaFourmakersSRSDTO> vagas)
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    _vagaFourmakersRepository.AtualizarVagas(vagas);

                    dbTrans.Commit();
                }
                catch (Exception)
                {
                    dbTrans.Rollback();
                    throw;
                }
            }
        }

        public void InserirVagas(List<VagaFourmakersSRSDTO> vagasParaInserir)
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    _vagaFourmakersRepository.SalvarVagasSRS(vagasParaInserir);

                    dbTrans.Commit();
                }
                catch (Exception)
                {
                    dbTrans.Rollback();
                    throw;
                }
            }
        }
    }
}