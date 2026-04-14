using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using Aws.Infra.Impl;
using Aws.Infra.Interfaces;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra;
using Colaboracao.Infra.Repositories;
using Colaboracao.Infra.Repositories.BancoDeTalentos;
using Colaboracao.Infra.Repositories.Colaborador;
using Colaboracao.Infra.Repositories.Competencia;
using Colaboracao.Infra.Repositories.Competencia.Dominio;
using Colaboracao.Infra.Repositories.Curriculo;
using Colaboracao.Infra.Repositories.Formacao;
using Colaboracao.Infra.Repositories.Fourmakers;
using Colaboracao.Infra.Repositories.Idioma;
using Colaboracao.Infra.Repositories.LogRepo;
using Colaboracao.Infra.Repositories.Org;
using Colaboracao.Infra.Repositories.SSO;
using Colaboracao.Infra.Repositories.Usuario;
using Colaboracao.Infra.Repositories.Usuario.Permissao;
using Colaborador.Domain;
using Colaborador.Domain.Impl;
using Colaborador.Domain.Impl.Services;
using Colaborador.Domain.Impl.Services.BancoDeTalentos;
using Colaborador.Domain.Interfaces.Services;
using Colaborador.Domain.Interfaces.Services.BancoDeTalentos;
using Competencia.Domain.Impl.Services;
using Competencia.Domain.Impl.Services.Metodologia;
using Competencia.Domain.Interfaces.Services;
using Competencia.Domain.Interfaces.Services.Metodologia;
using Core.Domain;
using Core.Domain.Colaborador;
using Core.Domain.Competencia.Metodologia;
using Core.Domain.Curriculo;
using Core.Domain.Dominio;
using Core.Domain.Formacao;
using Core.Domain.IIdioma;
using Core.Domain.LogRepo;
using Core.Domain.ParametroOrg;
using Core.Domain.Usuario;
using Core.Domain.Usuario.Permissao;
using Core.DomainModel;
using Core.DomainModel.BancoDeTalentos;
using Core.DomainModel.Colaborador;
using Core.DomainModel.Competencia;
using Core.DomainModel.Org;
using Core.DomainModel.Softskill;
using Core.DomainModel.SSO;
using Formacao.Domain.Impl.Services;
using Formacao.Domain.Interfaces.Services;
using Foursys.Domain.Impl.Services;
using Foursys.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using SRS.Infra.Impl;
using SRS.Infra.Interfaces;
using Colaboracao.Infra.Repositories.Colaborador.Colaborador.Colaborador;
using Colaborador.Domain.Impl.Services.Colaborador.DepartamentoOrg;
using Colaborador.Domain.Interfaces.Colaborador.DepartamentoOrg;
using Core.Domain.Colaborador.Colaborador;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Colaboracao.Core;
using Colaborador.Domain.Interfaces.Validadores;
using Core.Domain.Social;
using Colaboracao.Infra.Repositories.Social;
using Colaboracao.Infra.Repositories.Log;
using Colaboracao.Infra.Repositories.Vaga;
using Core.Domain.Vaga;
using Aws.Infra.Interfaces.S3;
using Amazon.S3;
using Aws.Infra.Impl.S3;
using Logs.Infra.Extensions;
using Colaboracao.Infra.Repositories.Projeto;
using Colaboracao.Infra.Repositories.Questionario;
using Core.Domain.Questionario;
using Core.DomainModel.Projeto;
using ApiClient.Infra.Interfaces;
using Colaboracao.Infra.Repositories.Labs;
using Core.Domain.Labs;
using Labs.Domain.Interfaces;
using Labs.Domain.Impl;
using Labs.Infra;

namespace Colaboracao.Initializer
{
    public static partial class InitializerExtension
    {
        public static void AddColaboradorServicesDependencies(this IServiceCollection services)
        {
            services.TryAddScoped<IDepartamentoOrgService, DepartamentoOrgService>();
            services.TryAddScoped<IDepartamentoOrgValidatorService, DepartamentoOrgValidatorService>();
            services.TryAddScoped<IDepartamentoOrgRepository, DepartamentoOrgRepository>();

            services.TryAddScoped<IFuncionalidadeSistemaRepository, FuncionalidadeSistemaRepository>();


            services.TryAddScoped(typeof(IBuscaColaboradorRepository), typeof(BuscaColaboradorRepository));
            services.TryAddScoped(typeof(ICCHClient), typeof(CCHClient));
            services.TryAddScoped(typeof(IColaboradorClient), typeof(ColaboradorClient));
            services.TryAddScoped(typeof(IColaboradorDepartamentoRepository), typeof(ColaboradorDepartamentoRepository));
            services.TryAddScopedDomainService<IColaboradorDepartamentoService, ColaboradorDepartamentoService>();
            services.TryAddScoped(typeof(IColaboradorDiretoriaRepository), typeof(ColaboradorDiretoriaRepository));
            services.TryAddScopedDomainService<IColaboradorDiretoriaService, ColaboradorDiretoriaService>();
            services.TryAddScoped(typeof(IColaboradorDapperRepository), typeof(ColaboradorDapperRepository));
            services.TryAddScoped(typeof(IColaboradorModaisIgnoradosRepository), typeof(ColaboradorModaisIgnoradosRepository));
            services.TryAddScoped(typeof(IColaboradorModaisIgnoradosService), typeof(ColaboradorModaisIgnoradosService));
            services.AddScopedDomainService<IColaboradorService, ColaboradorService>();
            services.TryAddScoped(typeof(IColaboradorGrupoAcessoConfiguracaoService), typeof(ColaboradorGrupoAcessoConfiguracaoService));
            services.TryAddScoped(typeof(IColaboradorGrupoAcessoConfiguracaoRepository), typeof(ColaboradorGrupoAcessoConfiguracaoRepository));
            services.TryAddScoped(typeof(IGrupoAcessoRepository), typeof(GrupoAcessoRepository));
            services.TryAddScoped(typeof(IUsuarioGrupoAcessoRepository), typeof(UsuarioGrupoAcessoRepository));
            services.TryAddScoped(typeof(IAcessoUsuarioRepository), typeof(AcessoUsuarioRepository));
            services.TryAddScoped(typeof(IUsuarioExternoRepository), typeof(UsuarioExternoRepository));



            services.TryAddScoped(typeof(IComentarioClient), typeof(ComentarioClient));
            services.TryAddScoped(typeof(ICompetenciaClient), typeof(CompetenciaClient));
            services.TryAddScoped(typeof(ICurriculoColaboradorRespository), typeof(CurriculoColaboradorRespository));
            services.TryAddScoped(typeof(IDominioClient), typeof(DominioClient));
            services.TryAddScoped(typeof(IEndossoClient), typeof(EndossoClient));
            services.TryAddScopedDomainService<IEscolaridadeColaboradorService, EscolaridadeColaboradorService>();
            services.TryAddScoped(typeof(IEstatisticasRepository), typeof(EstatisticasRepository));
            services.TryAddScopedDomainService<IExperienciaProfissionalService, ExperienciaProfissionalService>();
            services.TryAddScoped(typeof(IFirebaseClient), typeof(FirebaseClient));
            services.TryAddScoped(typeof(IForcaPerfilRepository), typeof(ForcaPerfilRepository));
            services.TryAddScoped(typeof(IFormacaoClient), typeof(FormacaoClient));
            services.TryAddScoped(typeof(IHobbyClient), typeof(HobbyClient));
            services.TryAddScoped(typeof(IInteresseClient), typeof(InteresseClient));
            services.TryAddScoped(typeof(ILogRepository), typeof(LogRepository));
            services.TryAddScoped(typeof(IMetodologiaClient), typeof(MetodologiaClient));
            services.TryAddScopedDomainService<INacionalidadeService, NacionalidadeService>();
            services.TryAddScopedDomainService<IPaisService, PaisService>();
            services.TryAddScoped(typeof(IPassaporteColaboradorRepository), typeof(PassaporteColaboradorRepository));
            services.TryAddScoped(typeof(IPassaporteColaboradorService), typeof(PassaporteColaboradorService));
            services.TryAddScoped(typeof(ISoftskillClient), typeof(SoftskillClient));
            services.TryAddScoped(typeof(ISoftskillClient), typeof(SoftskillClient));
            services.TryAddScoped(typeof(IIdiomaClient), typeof(IdiomaClient));
            services.TryAddScoped(typeof(ISRSClient), typeof(SRSClient));
            services.TryAddScoped(typeof(ISRSInfraClient), typeof(SRSInfraClient));
            services.TryAddScoped(typeof(ISRSInfraClient), typeof(SRSInfraClient));
            services.TryAddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.TryAddScoped(typeof(IUploadFilesClient), typeof(UploadFilesClient));
            services.TryAddScoped(typeof(IVerificaSeCpfESistemico), typeof(VerificaSeCpfESistemico));
            services.TryAddScoped(typeof(IVistoColaboradorRepository), typeof(VistoColaboradorRepository));
            services.TryAddScopedDomainService<IVistoColaboradorService, VistoColaboradorService>();
            services.TryAddScopedDomainService<IPassaporteColaboradorService, PassaporteColaboradorService>();
            services.TryAddScoped(typeof(IPassaporteColaboradorRepository), typeof(PassaporteColaboradorRepository));
            services.TryAddScopedDomainService<IColaboradorDiretoriaService, ColaboradorDiretoriaService>();
            services.TryAddScoped(typeof(IColaboradorDiretoriaRepository), typeof(ColaboradorDiretoriaRepository));
            services.TryAddScopedDomainService<IColaboradorDepartamentoService, ColaboradorDepartamentoService>();
            services.TryAddScoped(typeof(IColaboradorDepartamentoRepository), typeof(ColaboradorDepartamentoRepository));
            services.TryAddScoped(typeof(IUsuarioColaboradorRepository), typeof(UsuarioColaboradorRepository));
            services.TryAddKeyedScoped<IUsuarioColaboradorRepository, UsuarioColaboradorDapperRepository>("Dapper");
            services.TryAddScoped(typeof(ISSORepository), typeof(SSORepository));
            services.TryAddScoped(typeof(IColaboradorValidadorService), typeof(ColaboradorValidadorService));
            services.TryAddScoped(typeof(IFuncionalidadeSistemaRepository), typeof(FuncionalidadeSistemaRepository));
            services.TryAddScoped(typeof(IOrgRepository), typeof(OrgRepository));
            services.TryAddScoped(typeof(IContatoEmergenciaRepository), typeof(ContatoEmergenciaRepository));
            services.TryAddScoped(typeof(IConnectionStringCore), typeof(ConnectionStringCore));
            services.TryAddScoped(typeof(ICurriculoClient), typeof(CurriculoClient));
            services.TryAddScopedDomainService<ILinkedinService, LinkedinService>();

            // Idioma
            services.TryAddScoped(typeof(IIdiomaColaboradorRepository), typeof(IdiomaColaboradorRepository));
            services.TryAddScoped(typeof(IIdiomaNivelRepository), typeof(IdiomaNivelRepository));
            services.TryAddScoped(typeof(IIdiomaRepository), typeof(IdiomaRepository));
            services.TryAddScoped(typeof(IIdiomaRepositoryGenerico), typeof(IdiomaRepositoryGenerico));
            services.TryAddScopedDomainService<IIdiomaService, IdiomaService>();

            //Formacao
            services.TryAddScopedDomainService<IFormacaoService, FormacaoService>();
            services.TryAddScoped(typeof(IFormacaoRepository), typeof(FormacaoRepository));
            services.TryAddScoped(typeof(IFormacaoGenericoRepository), typeof(FormacaoGenericoRepository));

            services.TryAddScopedDomainService<ICompetenciaService, CompetenciaService>();
            services.TryAddScopedDomainService<ICompetenciaHistoricoService, CompetenciaHistoricoService>();
            services.TryAddScoped(typeof(ICompetenciaHistoricoRepository), typeof(CompetenciaHistoricoRepository));
            services.TryAddScoped(typeof(ICompetenciaColaboradorRepository), typeof(Colaboracao.Infra.Repositories.Competencia.CompetenciaColaboradorRepository));
            services.TryAddScopedDomainService<IHardSkillService, HardSkillService>();
            services.TryAddScoped(typeof(IHardSkillRepository), typeof(HardSkillRepository));
            services.TryAddScoped(typeof(IFoursysClient), typeof(FoursysClient));
            services.TryAddScoped(typeof(IPerfilAlocacaoRepository), typeof(PerfilAlocacaoRepository));

            //Soft skill
            services.TryAddScoped(typeof(ISoftskillRepository), typeof(SoftskillRepository));
            services.TryAddScoped(typeof(ISoftskillColaboradorRepository), typeof(SoftskillColaboradorRepository));
            services.TryAddScopedDomainService<ISoftskillService, SoftskillService>();
            services.TryAddScoped(typeof(ISoftskillNivelRepository), typeof(SoftskillNivelRepository));
            // Dominio
            services.TryAddScoped(typeof(IDominioRepository), typeof(DominioRepository));
            services.TryAddScopedDomainService<IDominioService, DominioService>();

            //Metodologia
            services.TryAddScopedDomainService<IMetodologiasService, MetodologiasService>();
            services.TryAddScoped(typeof(IMetodologiasRepository), typeof(Colaboracao.Infra.Repositories.Competencia.MetodologiasRepository));

            //Skill desconhecida
            services.TryAddScopedDomainService<ISkillDesconhecidaService, SkillDesconhecidaService>();
            services.TryAddScoped(typeof(ISkillDesconhecidaColaboradorRepository), typeof(SkillDesconhecidaColaboradorRepository));
            services.TryAddScoped(typeof(ISkillDesconhecidaRepository), typeof(SkillDesconhecidaRepository));

            services.TryAddScopedDomainService<IColaboradorSugestaoService, ColaboradorSugestaoService>();
            services.TryAddScoped(typeof(IColaboradorSugestaoRepository), typeof(ColaboradorSugestaoRepository));

            services.TryAddScoped(typeof(IRealizacaoColaboradorRepository), typeof(RealizacaoColaboradorRepository));

            services.TryAddScoped(typeof(IParametroConfiguracaoRepository), typeof(ParametroConfiguracaoRepository));
            services.TryAddScopedDomainService<IBuscaParametroConfiguracaoService, BuscaParametroConfiguracaoService>();

            //BancoDeTalentos
            services.TryAddScoped(typeof(IBancoDeTalentosRepository), typeof(BancoDeTalentosRepository));
            services.TryAddScopedDomainService<IBancoDeTalentosService, BancoDeTalentosService>();
            services.TryAddScoped<IBancoDeTalentosValidatorService, BancoDeTalentosValidatorService>();

            services.TryAddScopedDomainService<IImportacaoColaboradorService, ImportacaoColaboradorService>();
            services.TryAddScoped(typeof(ICandidaturaRepository), typeof(CandidaturaRepository));

            services.TryAddScoped(typeof(IProcessamentoCurriculoLoteRepository), typeof(ProcessamentoCurriculoLoteRepository));
            services.TryAddScoped(typeof(IQueueProducer), typeof(AmazonSQSProducer));

            services.TryAddScoped(typeof(ILogCore), typeof(LogCore));

            services.TryAddScoped(typeof(IImportacaoCurriculoValidatorService), typeof(ImportacaoCurriculoValidatorService));
            services.TryAddScoped(typeof(IComentarioCandidaturaRepository), typeof(ComentarioCandidaturaRepository));

            services.TryAddScoped(typeof(IGestaoDeCompetenciaRepository), typeof(GestaoDeCompetenciaRepository));
            // Registrar o repository específico do Banco de Talentos SRS
            services.TryAddScoped(typeof(ILogBancoTalentoSRSRepository), typeof(LogBancoTalentoSRSRepository));

            //Match
            services.TryAddScoped(typeof(IMatchClient), typeof(MatchClient));

            // Match Semântico (best_candidates/hyde) — usado por BancoDeTalentosService.BuscarBancoTalentosComPromptMatch
            services.TryAddScoped(typeof(IApiClient), typeof(ApiClient.Infra.Impl.ApiClient));
            services.TryAddScoped<IMatchSemanticoClient, MatchSemanticoClient>();
            services.TryAddScoped<ILabsLogMatchSemanticoRepository, LabsLogMatchSemanticoRepository>();
            services.TryAddScoped<ILabsLogRankCandidatesIdsRepository, LabsLogRankCandidatesIdsRepository>();
            services.TryAddScoped<ILabsLogExtractorExtractVagaRepository, LabsLogExtractorExtractVagaRepository>();
            services.TryAddScoped<ILabsLogScoreSingleCandidatesRepository, LabsLogScoreSingleCandidatesRepository>();
            services.TryAddScopedDomainService<IMatchSemanticoService, MatchSemanticoService>();
            services.TryAddScopedDomainService<IMatchService, MatchService>();
            services.TryAddScopedDomainService<IExtractorService, ExtractorService>();

            services.TryAddScoped(typeof(IFormacaoNivelRepository), typeof(FormacaoNivelRepository));
            
            services.TryAddScoped(typeof(IVagaFourmakersRepository), typeof(VagaFourmakersRepository));
            services.TryAddScoped(typeof(IUploadFiles), typeof(UploadFiles));
            services.AddTransient(typeof(IAmazonS3Uploader), typeof(AmazonS3Uploader));
            services.AddTransient(typeof(IAmazonS3), typeof(AmazonS3Client)); 
            services.AddTransient(typeof(ITokenFileRepository), typeof(TokenFileRepository));

            services.TryAddScoped(typeof(IColaboradorPerfilRepository), typeof(ColaboradorPerfilRepository));
            services.TryAddScopedDomainService<ISkillGenericService, SkillGenericService>();
            
            services.TryAddScoped(typeof(IEnderecoDapperRepository), typeof(EnderecoDapperRepository));
            services.TryAddScoped(typeof(IEnderecoService), typeof(EnderecoService));
            services.TryAddScoped(typeof(IColaboradorOrgRepository), typeof(ColaboradorOrgRepository));
            
            services.TryAddScoped<IClienteOrgRepository, ClienteOrgRepository>();
            services.AddScoped(typeof(ICertificadoRepository), typeof(CertificadoRepository));
            services.AddScoped(typeof(IEnderecoDapperRepository), typeof(EnderecoDapperRepository));
            services.AddScoped(typeof(IEnderecoService), typeof(EnderecoService));
            services.AddScoped(typeof(IColaboradorOrgRepository), typeof(ColaboradorOrgRepository));

            // DTO Repository registrations (new non-generic interfaces)
            // Colaborador
            services.TryAddScoped<IColaboradorDtoRepository, ColaboradorRepository>();
            services.TryAddScoped<ISimpleColaboradorDtoRepository, SimpleColaboradorRepository>();
            services.TryAddScoped<IDependenteDtoRepository, DependenteColaboradorRepository>();
            services.TryAddScoped<IEnderecoDtoRepository, EnderecoRepository>();
            services.TryAddScoped<IFotoDtoRepository, FotoRepository>();
            services.TryAddScoped<IExperienciaProfissionalDtoRepository, ExperienciaProfissionalRepository>();
            services.TryAddScoped<IEscolaridadeColaboradorDtoRepository, EscolaridadeColaboradorRepository>();
            services.TryAddScoped<INacionalidadeDtoRepository, NacionalidadeRepository>();
            services.TryAddScoped<IPaisDtoRepository, PaisRepository>();
            services.TryAddScoped<IStatusDtoRepository, StatusRepository>();

            // Usuario
            services.TryAddScoped<IUsuarioDtoRepository, UsuarioRepository>();
            services.TryAddScoped<ITokenDtoRepository, TokenRepository>();
            services.TryAddScoped<ITokenSSODtoRepository, TokenSSORepository>();
            services.TryAddScoped<ITokenSistemaDtoRepository, TokenSistemaRepositoryDiscontinued>();
            services.TryAddScoped<ITokenUsuarioAcessoDtoRepository, TokenUsuarioAcessoRepository>();

            // Competencia
            services.TryAddScoped<ICompetenciaDtoRepository, Colaboracao.Infra.Repositories.CompetenciaRepository>();
            services.TryAddScoped<ICompetenciaColaboradorDtoRepository, Colaboracao.Infra.Repositories.CompetenciaColaboradorRepository>();
            services.TryAddScoped<ICompetenciaNivelDtoRepository, CompetenciaNivelRepository>();
            services.TryAddScoped<ICompetenciaEndossoDtoRepository, CompetenciaEndossoRepository>();
            services.TryAddScoped<ICompetenciaEndossoColaboradorDtoRepository, CompetenciaEndossoColaboradorRepository>();
            services.TryAddScoped<ICompetenciaTipoEndossoDtoRepository, CompetenciaTipoEndossoRepository>();
            services.TryAddScoped<ICompetenciaCertificadoDtoRepository, CompetenciaCertificadoRepository>();
            services.TryAddScoped<ICompetenciaGenericoDtoRepository, CompetenciaGenericoRepository>();

            // Dominio
            services.TryAddScoped<IDominioColaboradorDtoRepository, DominioColaboradorRepository>();
            services.TryAddScoped<IDominioEndossoColaboradorDtoRepository, DominioEndossoColaboradorRepository>();
            services.TryAddScoped<IDominioEndossoDtoRepository, DominioEndossoRepository>();
            services.TryAddScoped<IDominioAntigoDtoRepository, DominioAntigoRepository>();
            services.TryAddScoped<IDominioNivelDtoRepository, DominioNivelRepository>();
            services.TryAddScoped<IDominioTipoEndossoDtoRepository, DominioTipoEndossoRepository>();
            services.TryAddScoped<IDominioGenericoDtoRepository, DominioGenericoRepository>();

            // Hobby / Interesse / Foursys
            services.TryAddScoped<IFoursysDtoRepository, FoursysRepository>();
            services.TryAddScoped<IInteresseDtoRepository, InteresseRepository>();
            services.TryAddScoped<IInteresseColaboradorDtoRepository, InteresseColaboradorRepository>();

        }
    }
}
