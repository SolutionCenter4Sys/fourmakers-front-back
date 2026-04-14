using DataTransferObject.Domain.Colaborador;

namespace Core.Domain.Colaborador
{
    public interface IHistoricoCVRepository
    {
        void InserirHistoricoCV(string codInternoColaborador, OrigemAlteracaoCVEnum origem, TipoItemCVEnum tipo, long? skillId, long? nivelId, ItemCVEnum itemPerfil);
        AtualizacaoCVDTO GetUltimaAtualizacao(string codInternoColaborador);
        int GetItemCVId(ItemCVEnum itemCv);
    }
}