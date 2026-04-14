using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Escolaridade;
using DataTransferObject.Domain.Experiencia;
using DataTransferObject.Domain.Formacao;
using DataTransferObject.Domain.Idioma;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Colaborador
{
    public interface IColaboradorPerfilRepository
    {
        /// <summary>
        /// Atualiza a seção "Sobre" do colaborador
        /// </summary>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <param name="sobre">Texto da seção sobre</param>
        /// <returns>True se atualizado com sucesso</returns>
        Task<bool> AtualizarSobreColaboradorAsync(string codigoInternoColaborador, string sobre);

        /// <summary>
        /// Lista as escolaridades do colaborador
        /// </summary>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <returns>Lista de escolaridades</returns>
        Task<List<EscolaridadeDTO>> ListarEscolaridadeColaboradorAsync(string codigoInternoColaborador);

        /// <summary>
        /// Busca múltiplas formações por descrições
        /// </summary>
        /// <param name="descricoes">Lista de descrições</param>
        /// <returns>Dicionário com descrição (normalizada) e FormacaoDTO</returns>
        Task<Dictionary<string, FormacaoDTO>> BuscarFormacoesEmLoteAsync(List<string> descricoes);

        /// <summary>
        /// Cria múltiplas formações de uma vez
        /// </summary>
        /// <param name="descricoes">Lista de descrições das formações</param>
        /// <param name="cpfUsuarioCriacao">CPF do usuário que está criando</param>
        /// <returns>Dicionário com descrição (normalizada) e FormacaoDTO criada</returns>
        Task<Dictionary<string, FormacaoDTO>> CriarFormacoesEmLoteAsync(List<string> descricoes, string cpfUsuarioCriacao);

        /// <summary>
        /// Adiciona múltiplas escolaridades ao colaborador de uma vez
        /// </summary>
        /// <param name="escolaridades">Lista de escolaridades a adicionar</param>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <param name="origem">Origem da alteração</param>
        /// <returns>Número de escolaridades adicionadas</returns>
        Task<int> AdicionarEscolaridadesEmLoteAsync(List<EscolaridadeDTO> escolaridades, string codigoInternoColaborador, OrigemAlteracaoCVEnum origem);

        /// <summary>
        /// Lista as experiências profissionais do colaborador
        /// </summary>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <returns>Lista de experiências profissionais</returns>
        Task<List<ListaExperienciaDTO>> ListarExperienciasColaboradorAsync(string codigoInternoColaborador);

        /// <summary>
        /// Adiciona múltiplas experiências profissionais ao colaborador de uma vez
        /// </summary>
        /// <param name="experiencias">Lista de experiências a adicionar</param>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <param name="origem">Origem da alteração</param>
        /// <returns>Número de experiências adicionadas</returns>
        Task<int> AdicionarExperienciasEmLoteAsync(List<ExperienciaDTO> experiencias, string codigoInternoColaborador, OrigemAlteracaoCVEnum origem);

        /// <summary>
        /// Lista os idiomas do colaborador
        /// </summary>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <returns>Lista de idiomas do colaborador</returns>
        Task<List<IdiomaColaboradorDTO>> ListarIdiomasColaboradorAsync(string codigoInternoColaborador);

        /// <summary>
        /// Busca múltiplos idiomas por descrições
        /// </summary>
        /// <param name="descricoes">Lista de descrições de idiomas</param>
        /// <returns>Dicionário com descrição (normalizada) e IdiomaDTO</returns>
        Task<Dictionary<string, IdiomaDTO>> BuscarIdiomasEmLoteAsync(List<string> descricoes);

        /// <summary>
        /// Cria múltiplos idiomas de uma vez
        /// </summary>
        /// <param name="descricoes">Lista de descrições dos idiomas</param>
        /// <returns>Dicionário com descrição (normalizada) e IdiomaDTO criado</returns>
        Task<Dictionary<string, IdiomaDTO>> CriarIdiomasEmLoteAsync(List<string> descricoes);

        /// <summary>
        /// Adiciona múltiplos idiomas ao colaborador de uma vez
        /// </summary>
        /// <param name="idiomas">Lista de idiomas a adicionar (ID e Nível)</param>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <param name="origem">Origem da alteração</param>
        /// <returns>Número de idiomas adicionados</returns>
        Task<int> AdicionarIdiomasColaboradorEmLoteAsync(List<(int idiomaId, long? nivelId)> idiomas, string codigoInternoColaborador, OrigemAlteracaoCVEnum origem);

        /// <summary>
        /// Lista as skills do colaborador de forma genérica (hardskill, softskill, metodologia, domínio, skill desconhecida)
        /// </summary>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <param name="tipoSkill">Tipo de skill</param>
        /// <returns>Lista de skills do colaborador</returns>
        Task<List<SkillGenericaDTO>> ListarSkillsColaboradorAsync(string codigoInternoColaborador, TipoSkillEnum tipoSkill);

        /// <summary>
        /// Busca múltiplas skills por descrições de forma genérica
        /// </summary>
        /// <param name="descricoes">Lista de descrições</param>
        /// <param name="tipoSkill">Tipo de skill</param>
        /// <returns>Dicionário com descrição (normalizada) e SkillGenericaDTO</returns>
        Task<Dictionary<string, SkillGenericaDTO>> BuscarSkillsEmLoteAsync(List<string> descricoes, TipoSkillEnum tipoSkill);

        /// <summary>
        /// Cria múltiplas skills de uma vez de forma genérica
        /// </summary>
        /// <param name="descricoes">Lista de descrições das skills</param>
        /// <param name="tipoSkill">Tipo de skill</param>
        /// <param name="cpfUsuarioCriacao">CPF do usuário que está criando</param>
        /// <returns>Dicionário com descrição (normalizada) e SkillGenericaDTO criada</returns>
        Task<Dictionary<string, SkillGenericaDTO>> CriarSkillsEmLoteAsync(List<string> descricoes, TipoSkillEnum tipoSkill, string cpfUsuarioCriacao);

        /// <summary>
        /// Adiciona múltiplas skills ao colaborador de uma vez de forma genérica
        /// </summary>
        /// <param name="skills">Lista de skills a adicionar (ID e Nível)</param>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <param name="tipoSkill">Tipo de skill</param>
        /// <param name="origem">Origem da alteração</param>
        /// <returns>Número de skills adicionadas</returns>
        Task<int> AdicionarSkillsColaboradorEmLoteAsync(List<(long skillId, long? nivelId)> skills, string codigoInternoColaborador, TipoSkillEnum tipoSkill, OrigemAlteracaoCVEnum origem);
    }
}

