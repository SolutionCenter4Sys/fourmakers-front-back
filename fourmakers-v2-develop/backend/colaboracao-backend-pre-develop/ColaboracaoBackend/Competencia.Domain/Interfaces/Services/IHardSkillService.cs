using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Foursys;
using DataTransferObject.Domain.Nivel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Competencia.Domain.Interfaces.Services
{
    public interface IHardSkillService
    {
        CompetenciaColaboradorResult InserirHardSkillColaboradorEmLote(List<AddCompetenciaColabParam> param, string cpf, bool minhaJornada);
        CompetenciaDTO InserirHardSkill(string descricao, string cpf);
        CompetenciaDTO ObterHardSkillPorId(long competenciaId);
        CompetenciaDTO InserirHardSkillColaborador(long competenciaId, long? nivelId, string cpf, bool minhaJornada, string gestorExternoPerfil, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        List<ItemPerfilDTO> ListarHardSkill(string busca, int cursor, int limite);
        Task<List<CompetenciaDTO>> ListarHardSkillColaborador(string cpfColaborador);
        bool RemoverHardSkillColaborador(long competenciaId, string cpf, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        List<NivelDTO> ListarNivelHardSkill();
        bool RemoverCertificadoHardSkillColaborador(long certificadoCompetenciaId, string cpf);
        bool RemoverCertificado(long id, string cpf, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        bool AlterarCertificadoPrincipalColaborador(long id, string cpf);
        CompetenciaColaboradorDTO AlterarHardSkillColaborador(string cpf, long id, long? nivelId, bool minhaJornada, string gestorExternoPerfil, string usuarioLogado, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        List<long> ListarIdsPorHardSkillId(long id);
        Task<CertificadoDTO> InserirCertificadoHardSkillColaborador(string cpf, long? competenciaColaboradorId, byte[] imagem, TipoCertificadoEnum tipo, DateTime conclusao, string descricao, string instituicao, int cargaHoraria, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        Task<CertificadoDTO> AlterarCertificado(string cpf, long certificadoId, byte[] imagem, TipoCertificadoEnum tipo, DateTime conclusao, string descricao, string instituicao, int cargaHoraria, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        CompetenciasSumarioResult SumarioCompetencias(FiltroSumarioCompetenciasParam param);
        ItensSumarioResult SumarioCompetenciasListarSkills();
        List<long> ListarHardSkillAtribuidas(string cpfColaborador);
        List<UnidadesDTO> ListUnidadesComDefaultPorOrg(string token, int orgId);
        List<UnidadesDTO> ListUnidadesPorOrg(string token, int orgId);
        CompetenciaColaboradorDTO ObterHardSkillColaborador(long competenciaId, string cpf);
        CompetenciaColaboradorDTO ObterHardSkillColaborador(long competenciaColaboradorId);
        List<CompetenciaColaboradorDTO> ListarHardSkillColaboradorCompleto(string cpf);
        List<CertificadoDTO> ListarCertificadosHardSkillColaborador(long competenciaColaboradorId);
        ColaboradorCompetenciaCertificadoDTO ObterColaboradorCompetenciaCertificado(long idCertificadoCompetencia);
        ColaboradorCompetenciaCertificadoDTO AtualizarColaboradorCompetenciaCertificado(ColaboradorCompetenciaCertificadoDTO colaboradorCompetenciaCertificadoDTO);
        ListaCompetenciaResult ListarCompetenciaNaoAtribuidas(string busca, int cursor, int limite, string cpf);
        List<CertificadoColaboradorDTO> ListaCertificadoColaborador(string cpfColaborador);
        List<CertificadoColaboradorDTO> ListaCertificadoColaboradorPorCodigoInterno(string cpfColaborador, string tokenSistema);
        Task<List<KeyValuePair<string, long>>> GetHardSkillInfoByDescricaoAsync(List<string> skills);
        Task<ApiGenericResult<List<CompetenciaGroupDTO>>> GetGroupCompetenciaByIds(List<long> ids);
    }
}