using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Nivel;
using System.Collections.Generic;

namespace Core.Domain
{
    public interface IHardSkillRepository
    {
        CompetenciaDTO InserirHardSkill(string descricao, long? usuarioCriacaoId);
        CompetenciaDTO AtualizarHardSkill(CompetenciaDTO competencia);
        List<CompetenciaDTO> ListarHardSkillPorDescricao(string descricao);
        List<long> ListarCodigoCompetenciasColaborador(string cpf);
        CompetenciaDTO ObterHardSkillPorId(long competenciaId);
        CompetenciaDTO InserirHardSkillColaborador(long competenciaId, long? nivelId, string cpf);
        bool RemoverHardSkillColaborador(long competenciaColaboradorId);
        long? ObterIdUsuarioPorCpf(string cpf);
        List<CompetenciaDTO> ListarHardSkill(string busca, int cursor, int limite);
        List<NivelDTO> ListarNivelHardSkill();
        CompetenciaColaboradorDTO ObterCompetenciaColaborador(long competenciaId, string cpf);
        CompetenciaColaboradorDTO ObterCompetenciaColaborador(long competenciaColaboradorId);
        List<CertificadoDTO> ListarCertificadosHardSkillColaborador(long competenciaColaboradorId);
        CertificadoDTO ObterCertificadoPorId(long certificadoId);
        CertificadoDTO AtualizarCertificado(CertificadoDTO certificadoDTO);
        long InserirCertificadoHardSkillColaborador(CertificadoDTO certificado, string codigoInternoColaborador, long? competenciaColaboradorId);
        bool RemoverCertificadoHardSkillColaboradorRollback(long certificadoId, long? competenciaColaboradorId);
        bool InativarCertificadoHardSkillColaborador(long certificadoCompetenciaId, string cpf);
        ColaboradorCompetenciaCertificadoDTO ObterColaboradorCompetenciaCertificado(long idCertificadoCompetencia);
        ColaboradorCompetenciaCertificadoDTO ObterColaboradorCompetenciaCertificadoPorCertificadoId(long idCertificado);
        ColaboradorCompetenciaCertificadoDTO BuscarUltimoCertificadoParaACompetenciaDesteCertificadoQueEstaSendoExcluido(long idCertificadoCompetencia);
        ColaboradorCompetenciaCertificadoDTO ObterColaboradorCompetenciaCertificadoPrincipal(long idCertificadoCompetencia);
        ColaboradorCompetenciaCertificadoDTO AtualizarColaboradorCompetenciaCertificado(ColaboradorCompetenciaCertificadoDTO colaboradorCompetenciaCertificadoDTO);
        CompetenciaColaboradorDTO AtualizarCompetenciaColaborador(CompetenciaColaboradorDTO competenciaColaboradorDTO);
        List<long> ListarIdsPorHardSkillId(long id);
        //bool RemoverCertificado (long id, string cpf);
        CompetenciasSumarioResult BuscarColaboradorSumario(FiltroSumarioCompetenciasParam param, string token, int orgId);
        ItensSumarioResult ListarSkillsSumario();
    }
}