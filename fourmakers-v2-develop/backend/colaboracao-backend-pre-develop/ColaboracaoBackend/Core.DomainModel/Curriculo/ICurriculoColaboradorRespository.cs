namespace Core.Domain.Curriculo
{
    public interface ICurriculoColaboradorRespository
    {
        string GetCurriculoColaborador(string cpf);
        void InsereCurriculoColaborador(string path, string cpf);
        void AlterarCurriculoColaborador(string path, string cpf);
        void InsereCurriculoColaboradorDapper(string path, string cpf);
    }
}