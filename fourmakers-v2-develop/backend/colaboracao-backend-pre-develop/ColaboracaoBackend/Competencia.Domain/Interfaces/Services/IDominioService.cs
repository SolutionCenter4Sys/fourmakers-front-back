using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Dominio;
using DataTransferObject.Domain.Nivel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Competencia.Domain.Interfaces.Services
{
    public interface IDominioService
    {
        DominioDTO AddDominio(string descricao);
        DominioDTO AddDominio(string descricao, string codInternoColaborador, bool importacaoLote);
        List<ItemPerfilDTO> ListDominio(string busca, int cursor, int limite);
        ListaDominioResult ListarDominiosNaoAtribuidos(string busca, int cursor, int limite);
        ItemPerfilDTO GetDominioById(long id);
        List<DominioColaboradorDTO> ListDominioColaborador(string cpfColaborador);
        bool RemoveDominioColaborador(long dominioId, string cpf, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        DominioColaboradorDTO AddDominioColaborador(long dominioId, long? nivelId, string cpf, bool minhaJornada, string gestorExternoPerfil, string usuarioLogado, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        List<NivelDTO> ListaNivelDominio();
        DominioColaboradorDTO AlterarDominioColaborador(string cpf, long id, long? nivelId, string gestorExternoPerfil, bool minhaJornada, string usuarioLogado, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        List<long> ListarDominiosAtribuidos();
        List<long> ListarDominiosAtribuidos(string cpf);
        DominioColaboradorResult AdicionarDominioColaboradorEmLote(List<AddDominioColabParam> param, bool minhaJornada, string usuarioLogado);
        Task<List<KeyValuePair<string, long>>> GetDominioInfoByDescricao(List<string> dominios);

        Task<ApiGenericResult<List<CompetenciaGroupDTO>>> GetGroupDominioByIds(List<long> ids);
    }
}