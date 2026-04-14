using System.Collections.Generic;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Candidato;

namespace Core.DomainModel
{
    public interface IColaboradorDtoRepository
    {
        ColaboradorDTO UpdateModel(ColaboradorDTO colaboradorModel);
        ColaboradorDTO GetModel(ColaboradorDTO colaboradorModel);
        ColaboradorDTO EstouSeguindoMeSegue(ColaboradorDTO colaboradorModel, string cpfRequest);
        List<string> GetSeguidores(string cpf);
        List<string> GetSeguindo(string cpf);
        IEnumerable<int> PegarNiveisGruposAcesso(ColaboradorDTO colaboradorModel);
        List<ColaboradorDTO> BuscarColaboradores(string cpf, int cursor, int limite, int candidato, int OrgId, out int totalResultsCount);
        IEnumerable<ColaboradorDTO> BuscaRowsColaborador(List<string> lstCpf, string cpfSolicitante, int deslocamentoBusca, int candidato);
        int ContarColaboradoresMenosEste(string cpf);
        void FollowColaborador(string cpfSeguidor, string cpfSeguir);
        void UnfollowColaborador(string cpfSeguidor, string cpfSeguir);
        int ContarCandidatosMenosEste(string cpf);
        ColaboradorDTO GetModelByKey(ColaboradorDTO colaboradorModel);
        ColaboradorDTO BuscarNomeColaborador(ColaboradorDTO model);
        List<ColaboradorDTO> BuscarStatusColaborador();
        ColaboradorDTO VerificaPublicoAcesso(ColaboradorDTO model);
        List<ColaboradorDTO> BuscarColaboradoresAtivos(int cursor, int limite, out int totalResultCount);
        ColaboradorDTO InserirDadosPCD(string cpf, ColaboradorDTO model);
        ColaboradorDTO InserirCandidato(string cpf, ColaboradorDTO model, CandidatoDTO candidato);
        ColaboradorDTO UpdateFotoModel(ColaboradorDTO colaboradorModel);
        ColaboradorDTO AlterarDadosPCD(string cpf, ColaboradorDTO model);
    }
}
