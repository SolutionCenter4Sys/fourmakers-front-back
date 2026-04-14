namespace Colaboracao.Infra.Repositories.Formacao
{
    public record FormacaoConstants
    {
        public const string CONFIRMADO = "Confirmado";
        public const string PENDENTE = "Pendente";

        public const int ATIVO = 1;
        public const int NAO_ATIVO = 0;

        public const int STATUS_ENDOSSO_ID_CONFIRMADO = 2;

        public const int ITEM_PERFIL_ID_FORMACAO = 2;
    }
}