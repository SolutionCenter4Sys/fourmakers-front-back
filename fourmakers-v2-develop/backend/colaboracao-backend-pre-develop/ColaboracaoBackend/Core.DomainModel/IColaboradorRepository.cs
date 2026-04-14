using DataTransferObject.Domain.Candidato;
using System.Collections.Generic;

namespace Core.Domain
{
    public interface IColaboradorRepository<TModel, TFactory>
    {
        TModel UpdateModel(TModel colaboradorModel);
        TModel GetModel(TModel colaboradorModel);
        TModel EstouSeguindoMeSegue(TModel colaboradorModel, string cpfRequest);
        List<string> GetSeguidores(string cpf, TFactory _colaboradorDomainFactory);
        List<string> GetSeguindo(string cpf, TFactory colaboradorDomainFactory);
        IEnumerable<int> PegarNiveisGruposAcesso(TModel colaboradorModel);
        List<TModel> BuscarColaboradores(string cpf, int cursor, int limite, int candidato, int OrgId, TFactory _colaboradorDomainFactory, out int totalResultsCount);
        IEnumerable<TModel> BuscaRowsColaborador(List<string> lstCpf, string cpfSolicitante, int deslocamentoBusca, int candidato, TFactory _colaboradorDomainFactory);
        int ContarColaboradoresMenosEste(string cpf);
        void FollowColaborador(string cpfSeguidor, string cpfSeguir);
        void UnfollowColaborador(string cpfSeguidor, string cpfSeguir);
        int ContarCandidatosMenosEste(string cpf);
        TModel GetModelByKey(TModel colaboradorModel);
        TModel BuscarNomeColaborador(TModel model);
        List<TModel> BuscarStatusColaborador(TFactory colaboradorDomainFactory);
        TModel VerificaPublicoAcesso(TModel model);
        List<TModel> BuscarColaboradoresAtivos(int cursor, int limite, out int totalResultCount, TFactory _colaboradorDomainFactory);
        TModel InserirDadosPCD(string cpf, TModel model);
        TModel InserirCandidato(string cpf, TModel model, CandidatoDTO candidato);
        TModel UpdateFotoModel(TModel colaboradorModel);
        TModel AlterarDadosPCD(string cpf, TModel model);
    }
}