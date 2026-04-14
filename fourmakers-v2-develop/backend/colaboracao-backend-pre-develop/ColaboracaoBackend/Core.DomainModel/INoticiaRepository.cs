using System.Collections.Generic;

namespace Core.Domain
{
    public interface INoticiaRepository<TModel, TFactory>
    {
        List<TModel> BuscarNoticia(string busca, int cursor, int limite, TModel model, TFactory factory);
        TModel BuscaPorId(TModel model);
        TModel InserirNoticia(string titulo, string descricao, TModel model);
        TModel AtualizarNoticia(TModel model, string titulo, string descricao);
        TModel DesativarNoticia(TModel model);
    }
}