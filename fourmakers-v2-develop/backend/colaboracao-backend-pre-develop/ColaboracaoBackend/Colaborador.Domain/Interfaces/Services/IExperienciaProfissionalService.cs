using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Experiencia;
using System;
using System.Collections.Generic;

namespace Colaborador.Domain.Interfaces.Services
{
    public interface IExperienciaProfissionalService
    {
        ExperienciaDTO AdicionarNovaExperiencia(string descricao, string cpf, string cargo, string empresa, DateTime dataInicio, DateTime? dataSaida, List<string> projetos, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL,bool useClassificacao = true);
        List<ListaExperienciaDTO> ListarExperienciaProfissional(string busca, int cursor, int limite, string cpf);
        List<ExperienciaEmpresaDTO> ListarExperienciaProfissionalAgrupada(string busca, int cursor, int limite, string cpf, int orgId);
        ExperienciaDTO GetExperinciaProfissionalById(long id);
        void RemoverExperienciaProfissional(long experienciaId, string cpf, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL, bool useClassificacao = true);
        UpdateExperienciaDTO AtualizarExperiencia(long id, string descricao, string colaboradorCpf, string titulo, string empresa, DateTime dataInicio, DateTime? dataSaida, List<string> projetos, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL, bool useClassificacao = true);
        List<string> AutoCompleteEmpresa(string nomeEmpresa, int limite, int cursor);
        List<string> AutoCompleteProjeto(string nomeProjeto, int limite, int cursor);
        void AdicionarSobre(string cpf, string sobre, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL, bool useClassificacao = true);
        ColaboradorSobreDTO BuscarSobre(string cpf);
        void RemoverSobre(string cpf, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL, bool useClassificacao = true);
        bool VerificarSeDataFimEhMaiorOuIgualQueDataInicio(DateTime dataInicio, DateTime? dataSaida);
    }
}