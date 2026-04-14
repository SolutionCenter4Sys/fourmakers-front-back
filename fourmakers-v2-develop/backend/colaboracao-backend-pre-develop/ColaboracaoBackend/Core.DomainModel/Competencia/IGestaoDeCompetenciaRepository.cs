using Competencia.Domain.Enums;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Competencia.GestaoDeCompetencia;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.DomainModel.Competencia
{
    public interface IGestaoDeCompetenciaRepository
    {
        Task<SkillItemDTO> VerificaHabilidadeExistentePorIdETipo(int id, int tipoId);
        Task<SkillItemDTO> VerificaHabilidadeExistentePorDescricaoETipo(string descricao, int tipoId);
        Task<List<ColaboradorHabilidadeDTO>> ListarColaboradoresPorSkillId(long id, TipoCompetenciaSRSEnum tipoCategoria);
        Task AtualizarGestorExternoPerfilSkills(int perfilItemIdAntigo, long idSkillAntigo, int perfilItemIdNovo, long idSkillNovo);
        Task AdicionaNovaCompetenciaAoColaborador(AdicionarCompetenciaDTO competencia, long? nivelId, string cpf, TipoCompetenciaSRSEnum tipoCompetenciaEnum);
        Task InativarHabilidadeDoColaborador(long habilidadeId, string CpfColaborador, TipoCompetenciaSRSEnum tipoCompetenciaEnum);
        Task AtualizarPerfilSkills(int perfilItemIdAntigo, long idSkillAntigo, int perfilItemIdNovo, long idSkillNovo);
        Task AtualizarAlocadoSkills(int perfilItemIdAntigo, long idSkillAntigo, int perfilItemIdNovo, long idSkillNovo);
        Task AtualizarSkillsVagaSRS(int perfilItemIdAntigo, long idSkillAntigo, int perfilItemIdNovo, long idSkillNovo);
        Task AtualizarSkillsVaga(int perfilItemIdAntigo, long idSkillAntigo, int perfilItemIdNovo, long idSkillNovo);
        Task AtualizarSkillsVagaCandidato(int perfilItemIdAntigo, long idSkillAntigo, int perfilItemIdNovo, long idSkillNovo);
        Task AtualizarSkillsVagaFourmakers(int perfilItemIdAntigo, long idSkillAntigo, int perfilItemIdNovo, long idSkillNovo);
        Task<int> AlterarIdHabilidadePorTipoGestorExternoPerfilSkills(int skillIdAntiga, int skillId, int perfilItemId);
        Task<int> AlterarIdHabilidadePorTipoAlocadoSkills(int skillIdAntiga, int skillId, int perfilItemId);
        Task<int> AlterarIdHabilidadePorTipoPerfilSkills(int skillIdAntiga, int skillId, int perfilItemId);
        Task<int> AlterarIdHabilidadePorTipoSkillsVaga(int skillIdAntiga, int skillId, int perfilItemId);
        Task<int> AlterarIdHabilidadePorTipoSkillsVagasSRS(int skillIdAntiga, int skillId, int perfilItemId);
        Task<int> AlterarIdHabilidadePorTipoSkillsVagasCandidato(int skillIdAntiga, int skillId, int perfilItemId);
        Task<int> AlterarIdHabilidadePorTipoSkillsVagaFourmakers(int skillIdAntiga, int skillId, int perfilItemId);
        Task<List<SkillParaUnificarDTO>> ObterListaSkillNaoSincronizadasNaCuradoriaVagaFourmakersSkillsPorItemPerfilId(ItemPerfilEnum tipo);
    }
}