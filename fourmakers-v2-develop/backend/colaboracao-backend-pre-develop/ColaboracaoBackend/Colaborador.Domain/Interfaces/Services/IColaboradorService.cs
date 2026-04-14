using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Comentario;
using DataTransferObject.Domain.Dependentes;
using DataTransferObject.Domain.Endereco;
using DataTransferObject.Domain.Endosso;
using DataTransferObject.Domain.LG.Holerite;
using DataTransferObject.Domain.Usuario;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Colaborador.ModeloTrabalho;
using DataTransferObject.Domain.Foursys;

namespace Colaborador.Domain.Interfaces.Services
{
    public interface IColaboradorService
    {
        Task<bool> AlterarFotoColaborador(string cpf, int orgId, byte[] imagem, bool gerarThumb);
        DependentesDTO AlterarDadosDependenteColaborador(AlterarDependentesDTO colabInfo, string cpf);
        DependentesDTO AdicionarDadosDependenteColaborador(AdicionarDependentesDTO colabInfo, string cpf);
        void RemoveDependenteColaborador(long dependenteId, string cpf);
        ColaboradorDTO BuscarDadosColaborador(string cpf, int orgId, string cpfRequest, string token);
        Task<UsuarioColaboradorDTO> BuscarDadosUsuario(string token);
        RedeColaboradorDTO BuscarRedeColaborador(string cpf, int orgId, string cpfRequest, string token);
        Task<PerfilProfissionalDTO> BuscarPerfilProfissional(string cpf);
        Task<List<PedidoEndossoDTO>> GetPedidosEndossoColaborador(string cpfRequest);
        Task<ForcaPerfilDTO> BuscarForcaPerfil(string cpfRequest, int orgId);
        List<ColaboradorDTO> BuscarColaborador(string cpf, int orgId, string busca, int cursor, int limite, out int filteredResultsCount, out int totalResultsCount);
        Task<List<ItemPerfilResult>> AdicionarCompetenciaColaborador(List<AdicionarRemoverItemDTO> dtos);
        Task RemoveCertificadoCompetenciaColaborador(string cpf, long id);
        Task AlteraCertificadoPrincipalColaborador(string cpf, long id);
        Task<StatusResult> RemoverCompetenciaColaborador(string token, string cpf, long id);
        Task<List<ItemPerfilResult>> InserirFormacaoColaborador(string token, List<AdicionarRemoverItemDTO> dtos);
        Task<StatusResult> RemoverFormacaoColaborador(string token, string cpf, long id);
        Task<StatusResult> RemoverDominioColaborador(string token, string cpf, long id);
        Task<StatusResult> RemoverInteresseColaborador(string token, string cpf, long id);
        Task<StatusResult> RemoverHobbieColaborador(string token, string cpf, long id);
        Task<List<ComentarioTipoDTO>> ListarTipoComentarios();
        Task<ApiGenericResult<string>> InserirColaborador(CadastroColaboradorInput colaborador, string cpfUsuario, int orgId);
        Task<ComentarioDTO> InserirComentario(string token, int type, string texto);
        Task<CertificadoDTO> InserirCertificadoFormacaColaborador(string token, long formacaoColaboradorId, byte[] file, TipoCertificadoEnum tipo);
        Task<List<ItemPerfilResult>> InserirDominioColaborador(string token, List<AdicionarRemoverItemDTO> dtos);
        Task<List<ItemPerfilResult>> InserirInteresseColaborador(string token, List<AdicionarRemoverItemDTO> dtos);
        Task<List<ItemPerfilResult>> InserirHobbieColaborador(string token, List<AdicionarRemoverItemDTO> dtos);
        List<ColaboradorDTO> BuscarCandidato(string cpf, int orgId, string busca, int cursor, int limite, out int filteredResultCount, out int totalResultCount);
        SimpleColaboradorDTO BuscarNomeColaborador(string cpfColaborador);
        Task<ItemPerfilResult> AlterarCompetenciaColaborador(AdicionarRemoverItemDTO dtos);
        Task<ItemPerfilResult> AlterarFormacaoColaborador(AdicionarRemoverItemDTO dtos);
        Task<StatusResult> MergeFormacaoColaborador(MergeItemPerfilDTO dtos);
        Task<ItemPerfilResult> AlterarDominioColaborador(AdicionarRemoverItemDTO dtos);
        Task<StatusResult> MergeCompetenciaColaborador(MergeItemPerfilDTO dtos);
        Task<StatusResult> MergeDominioColaborador(MergeItemPerfilDTO dtos);
        Task<StatusResult> MergeInteresseColaborador(MergeItemPerfilDTO dtos);
        Task<StatusResult> MergeHobbieColaborador(MergeItemPerfilDTO dtos);
        ListColaboradoresResult BuscarListaColaboradores(string nomeCompleto);
        List<StatusColaboradorDTO> BuscarStatusColaborador();
        Task<bool> ValidaAcessoGrupoFuncionalidade(string cpfRequest, FuncionalidadeSistemaEnum enumFuncionalidadeSistema);
        List<ColaboradorAtivoDTO> BuscarColaboradoresAtivos(int cursor, int limite, out int filteredResultCount, out int totalResultCount);
        Task<List<ItemPerfilResult>> InserirSoftskillColaborador(string token, List<AdicionarRemoverItemDTO> dtos);
        Task<StatusResult> RemoverSoftskillColaborador(string token, string cpf, long id);
        Task<ItemPerfilResult> AlterarSoftskillColaborador(AdicionarRemoverItemDTO dtos);
        List<DependentesDTO> ListarDadosColaboradorDependente(string cpf);
        Task<CertificadoDTO> InserirCertificadoCompetenciaColaborador(string cpfRequest, long competenciaColaboradorId, byte[] file, TipoCertificadoEnum tipo, DateTime dataConclusao, string instituicao, string descricao, int cargaHoraria);
        List<TipoDependenteDTO> ListarTipoDependente();
        Task InserirContatoEmergencia(ContatoEmergenciaDTO srsCandidate, string cpf);
        Task AlterarContatoEmergencia(ContatoEmergenciaDTO srsCandidate, string cpf);
        Task<List<ContatoEmergenciaDTO>> ListarContatoEmergencia(string cpf);
        Task DeletarContatoEmergencia(string contactOrder, string cpf);
        Task InserirDadosPCD(EnumPCD pCD, int orgId, bool grupoDeRisco, string descricaoCondicaoDeSaude, string cpf);
        Task AlterarDadosPCD(EnumPCD pCD, int orgId, bool grupoDeRisco, string descricaoCondicaoDeSaude, string cpf);
        Task InserirDadosDemograficos(DadosDemograficosColaboradorDTO dadosDemograficos, string cpf);
        Task AlterarDadosDemograficos(DadosDemograficosColaboradorDTO dadosDemograficos, string cpf);
        Task<DadosDemograficosColaboradorDTO> ObterDadosDemograficos(string cpf);
        Task AlterarFormularioColaborador(string nome, DateTime? data_nascimento, string cpf, string rg, string estado_civil, string escolaridade, string etnia,
            string genero, string orientacao_sexual, bool pessoa_refugiada, string email, string emailAlternativo, string celular, string passaporte, EnderecoDTO endereco, ColaboradorSaudeDTO saude, string documentoColaborador, int orgId);
        Task AlterarCurriculoColaborador(byte[] file, string cpf);
        Task<string> GetCurriculoColaborador(string cpf);
        Task<bool> AlterarDadosColaboradorBase(ColaboradorDTO colabInfo, int orgId, byte[] imagem);
        Task<string> InsereCurriculoColaborador(byte[] file, string cpf);
        List<SimpleColaboradorDTO> AutoCompleteColaboradorPorDiretoria(string nome, string cpf, string diretoria, int cursor, int limite, string codGestor, int orgId);
        Task<List<EstatisticasProcDTO>> EstatisticasEtnia();
        Task<List<EstatisticasProcDTO>> EstatisticasIdade();
        Task<List<EstatisticasProcDTO>> EstatisticasTempoCasa();
        Task<List<EstatisticasProcDTO>> EstatisticasOrientacaoSexual();
        Task<List<EstatisticasProcDTO>> EstatisticasEscolaridade();
        Task<List<EstatisticasProcDTO>> EstatisticasGenero();
        List<HoleriteSimplesDTO> BuscarHolerites(string cpf);
        Task<List<TotalizadoresDTO>> TotalizadoresPorUnidade();
        Task<List<ColaboradoresOrgDTO>> ListaColaboradoresOrgAsync(int orgId, int cursor, int limite, bool fourtalents, string nomeOuEmail = "", string codExterno = "", string cpfRequest = null!);
        Task<ApiGenericResult<string>> EditarColaborador(CadastroColaboradorInput colaborador, string cpf, int orgId);
        Task<ApiGenericResult<FileContentResult>> RelatorioColaboradoresSkills(bool comHardskill);
        Task<EstatisticasModeloTrabalhoResult> EstatisticasModeloTrabalho();
        Task<bool> AlterarIdiomaPadrao(string idioma, string cpf, int orgId);
        Task<List<AniversariantesSemanaColaboradorDTO>> RetornaListaAniversariantesSemana(int orgId, string? codDiretoria);
        Task<List<DadosColaboradorDTO>> ListarDadosDoColaboradorPorIds(List<string> cpfs, int orgId, string cpfRequest);
        Task<bool> EnviarCvEmMassa(EnviarCvEmMassaParam param);
        Task<List<AnalistaResponsavelDTO>> BuscarAnalistasResponsaveis(int orgId);
        Task<bool> ExisteColaboradorComEsteEmail(string email);
        Task<bool> ExisteColaboradorComEsteLinkedin(string perfilIN);
        void InserirDadosIA(ColaboradorIADTO colaboradorIA);
        Task<bool> QualificarColaborador(string cpf, string cpfUsuarioLogado);
        Task<bool> DesqualificarColaborador(string cpf, string cpfUsuarioLogado);
        Task<List<ColaboradorQualificadoDTO>> GetColaboradoresQualificadosPorMim(string codigoInternoColaborador);
        Task<List<ColaboradorQualificadoDTO>> GetColaboradoresQualificadosPorColaborador(string codColaboradorRecrutador);
        bool SalvarDispositivoColaboradorApp(string codigoInternoColaborador, string deviceToken, int orgId);
        Task<List<ModeloContratacaoDTO>> ListarModelosDeContratacaoPorOrg(int orgId);
        void AtualizarEnderecoColaborador(string codigoInternoColaborador, string cidade, string estado);
        Task<IEnumerable<OrigemColaboradorDTO>> ListarOrigensColaborador();
        Task<ApiGenericResult<List<CargoColaboradorOrgDTO>>> ListarCargosPorOrgIdAsync(int orgId, string cpfRequest);
        
        // Métodos para edição de dados do colaborador com log
        Task<EditarColaboradorDTO> ObterColaboradorPorCodigoAsync(string codigoInternoColaborador);
        Task<EditarColaboradorDTO> AtualizarColaboradorAsync(EditarColaboradorDTO colaborador, string codigoInternoColaboradorAlterador);
        Task<bool> ExisteColaboradorAsync(string codigoInternoColaborador);
        Task<List<ColaboradorLogDTO>> ListarLogsPorColaboradorAsync(string codigoInternoColaborador, int limite, int cursor);
        Task<List<ColaboradorLogDTO>> ListarLogsPorAlteradorAsync(string codigoInternoColaboradorAlterador, int limite, int cursor);
        Task<List<ColaboradorLogDTO>> ListarLogsPorPeriodoAsync(DateTime dataInicio, DateTime dataFim, int limite, int cursor);
        Task<EditarColaboradorDTO> EditarCandidaturaColaboradorComDadosDemograficos(
            EditarColaboradorInputDTO colaboradorInput,
            DadosDemograficosColaboradorDTO dadosDemograficos,
            DadosCandidaturaInputDTO dadosCandidatura,
            string codigoInternoColaboradorAlterador);

        Task EditarModeloTrabalhoAsync(ModeloTrabalhoColaboradorDTO input, int orgId, string codigoInternoColaborador);
        Task EditarTelefoneColaboradorAsync(string telefone, string ddi, string codigoInternoColaborador);
    }
}