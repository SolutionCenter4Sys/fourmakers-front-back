using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.ClienteOrg;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.ColaboradoresAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao.Permanencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil.ModeloTrabalho;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil.ProfissionalLocalidade;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.RateCardsDosPerfis;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.SincronizarCRM;
using DataTransferObject.Domain.MapaDeAlocacao.Perfil;
using DataTransferObject.Domain.Pricing.Equipe;
using DataTransferObject.Domain.Vaga;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapaDeAlocacao.Domain.Interfaces.Perfil
{
    public interface IPerfilService
    {
        Task<BuscarQuantidadesPessoasMatchSkillResult> BuscarQuantidadesPessoasMatchSkill(BuscarQuantidadesPessoasMatchSkillInput input, int orgId);
        Task<int> CountPerfis(int orgId);
        Task<ApiGenericResult<IEnumerable<ListarPerfisResult>>> ListarPerfis(string dataInicio, string dataFim, string cliente, string cpf, int limite, int cursor, string busca, int orgId);
        Task<ObterEstatisticasMatchSkillSenioridadeResult> ObterEstatisticasMatchSkillSenioridade(ObterEstatisticasMatchSkillSenioridadeInput input);
        Task<ExtrairPerfilDeUmPromptResponse> ExtrairPerfilDeUmPrompt(ExtrairPerfilDeUmPromptRequest request, int orgId, string? codigoInternoColaborador = null);
    }
}