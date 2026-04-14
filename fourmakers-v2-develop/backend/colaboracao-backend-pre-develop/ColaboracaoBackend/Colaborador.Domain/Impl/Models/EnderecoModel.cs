using Colaboracao.Core;
using Colaborador.Domain.Interfaces.Factorys;
using Colaborador.Domain.Interfaces.Models;
using Core.Domain;

namespace Colaborador.Domain.Impl.Models
{
    public class EnderecoModel : IEnderecoModel
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<IEnderecoModel, IEnderecoDomainFactory> _repositoryEnderecoModel;

        public EnderecoModel(ILogCore log, IUnitOfWork unitOfWork, IRepository<IEnderecoModel, IEnderecoDomainFactory> repositoryEnderecoModel)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryEnderecoModel = repositoryEnderecoModel;
        }

        public long Id { get; set; }
        public string Cep { get; set; }
        public string Endereco { get; set; }
        public string Complemento { get; set; }
        public int? Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string ComQuemMora { get; set; }
        public string InternacionalLinhaUm { get; set; }
        public string InternacionalLinhaDois { get; set; }

        public IEnderecoModel GetByCpfColaborador(string cpf, IEnderecoDomainFactory enderecoDomainFactory)
        {
            return _repositoryEnderecoModel.GetModelByKey(cpf, enderecoDomainFactory);
        }

        public IEnderecoModel SaveModel()
        {
            var ret = _repositoryEnderecoModel.SaveModel(this);
            this.Id = ret.Id;
            return this;
        }

        public IEnderecoModel UpdateModel()
        {
            var ret = _repositoryEnderecoModel.UpdateModel(this);
            this.Id = ret.Id;

            return this;
        }
    }
}