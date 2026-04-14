namespace Core.Domain
{
    public interface IForcaPerfilRepository
    {
        int GetQuantidadeExperienciaProfissional(string cpf);
        int GetQuantidadeCompetencia(string cpf);
        int GetQuantidadeSoftskill(string cpf);
        bool PossuiMetodologia(string cpf);
        bool PossuiDominioNegocio(string cpf);
        bool PossuiIdioma(string cpf);
        bool PossuiCurriculo(string cpf);
        bool PossuiEscolaridade(string cpf);
        int GetNumeroCertificadosCompetencia(string cpf);
    }
}