namespace Core.DomainModel
{
    public interface ITokenDtoRepository
    {
        void SaveModel(string token, System.DateTime validade, sbyte ativo, long usuarioId);
        void DeleteModel(long id);
    }
}
