using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Idioma;
using DataTransferObject.Domain.Nivel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Competencia.Domain.Interfaces.Services
{
    public interface IIdiomaService
    {
        IdiomaDTO AdicionarIdioma(string descricao);
        List<IdiomaDTO> ListarIdioma(string busca, int cursor, int limite);
        IdiomaDTO GetIdiomaById(int id);
        List<NivelDTO> ListaNivelIdioma();
        IdiomaColaboradorDTO AdicionarIdiomaColaborador(int IdiomaId, long? nivelId, string cpf, string gestorExternoPerfil, bool minhaJornada, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        IdiomaColaboradorResult AdicionarIdiomaColaboradorEmLote(List<AddIdiomaColabParam> param, string cpf, bool minhaJornada);
        List<IdiomaColaboradorDTO> ListarIdiomaColaborador(string cpfColaborador);
        IdiomaColaboradorDTO AlterarIdiomaColaborador(string cpf, int id, long? nivelId, string gestorExternoPerfil, bool minhaJornada, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        void RemoveIdiomaColaborador(int IdiomaId, string cpf, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        List<int> ListarIdiomasAtribuidos(string cpfColaborador);
        ListaIdiomaResult ListarIdiomasNaoAtribuidos(string busca, int cursor, int limite, string cpfColaborador);
        List<KeyValuePair<string, long>> GetIdiomaInfoByDescricao(List<string> idiomas);
        Task<ApiGenericResult<List<CompetenciaGroupDTO>>> GetGroupIdiomaByIds(List<long> ids);
    }
}