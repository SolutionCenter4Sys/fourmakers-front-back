namespace Colaboracao.Helper.Enum
{
    public static class CRUDEnumExtensions
    {
        public static string ObterAcao(this CRUDEnum operacao)
        {
            return operacao switch
            {
                CRUDEnum.Create => "inserir",
                CRUDEnum.Read => "consultar",
                CRUDEnum.Update => "atualizar",
                CRUDEnum.Delete => "deletar",
                _ => "executar"
            };
        }
    }
}