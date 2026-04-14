using Colaborador.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;

namespace Colaborador.Domain.Interfaces.Factorys
{
    public interface IExperienciaProfissionalDomainFactory
    {
        IExperienciaProfissionalModel BuildModel(long id, string descricao, string cpf, string empresa, string titulo, DateTime dataInicio, DateTime? dataSaida, bool ativo);
        IExperienciaProfissionalModel BuildModel(long id, string descricao, string cpf, string empresa, string titulo, DateTime dataInicio, DateTime? dataSaida, List<string> projetos);
        IExperienciaProfissionalModel BuildModel(string descricao, string cpf, string empresa, string titulo, DateTime dataInicio, DateTime? dataSaida);
        IExperienciaProfissionalModel BuildModel();
        IExperienciaProfissionalModel BuildModel(string descricao, string cpf, string titulo, string empresa, DateTime dataInicio, DateTime? dataSaida, List<string> projetos);
        IExperienciaProfissionalModel BuildModelLista(long id, string descricao, string cpf, string empresa, string titulo, DateTime dataInicio, DateTime? dataSaida, List<string> projetos);
        IExperienciaProfissionalModel BuildModelLista();
        IExperienciaProfissionalModel BuildModelUpdate(long id, string descricao, string cpf, string empresa, string titulo, DateTime dataInicio, DateTime? dataSaida);
        IExperienciaProfissionalModel BuildModelUpdate();
    }
}