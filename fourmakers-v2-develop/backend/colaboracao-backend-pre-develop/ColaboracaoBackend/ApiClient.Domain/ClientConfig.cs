using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace ApiClient.Domain
{
    public static class ClientConfig
    {
        public static Clients Clients { get; } = new Clients();
        public static void SetConfiguration(ref IConfiguration config)
        {
            stepSetConfiguration(ref config, Clients, new List<string>() { "Clients" });
        }
        private static void stepSetConfiguration(ref IConfiguration config, Object item, List<string> lstLabel)
        {
            foreach (var key in item.GetType().GetProperties())
            {
                if (typeof(string) == key.PropertyType)
                {
                    config[levelLabel(lstLabel, key.Name)] = item.GetType().GetProperty(key.Name).GetValue(item).ToString();
                }
                else
                {
                    var lstAux = new List<string>();
                    lstAux.AddRange(lstLabel);
                    lstAux.Add(key.Name);
                    stepSetConfiguration(ref config, item.GetType().GetProperty(key.Name).GetValue(item), lstAux);
                }
            }
            return;
        }
        private static string levelLabel(List<string> lstLabel, string key)
        {
            var ret = "";
            foreach (var label in lstLabel)
            {
                ret += label + ":";
            }
            return ret + key;
        }
    }
    public class Usuario
    {
        public string ValidarToken { get; } = "Usuario/ValidaToken";
        public string ValidarTokenSSO { get; } = "Usuario/ValidaTokenSSO";
        public string ValidaAcessoGrupoFuncionalidade { get; } = "Usuario/ValidaAcessoGrupoFuncionalidade";
        public string ConfirmaPrimeiroAcessoCandidato { get; } = "Usuario/ConfirmaPrimeiroAcessoCandidato";
        public string ShowMe { get; } = "Usuario/ShowMe";
    }

    public class Colaborador
    {
        public string BuscarDadosColaboradorAdmin { get; } = "Colaborador/BuscarDadosColaboradorAdmin";
        public string AlterarDadosColaborador { get; } = "Colaborador/AlterarDadosColaborador";
        public string BuscarDadosColaboradorSimple { get; } = "Colaborador/ColaboradorSimple";
        public string BuscarCandidato { get; } = "Colaborador/BuscarCandidato";
        public string InsereColaboradorCandidato { get; } = "Colaborador/InsereColaboradorCandidato";
        public string BuscarRedeColaborador { get; } = "Colaborador/BuscarRedeColaborador";
        public string BuscarNomeColaborador { get; } = "Colaborador/BuscarNomeColaborador";
        public string CpfAdmin { get; } = "00000000000";
        public string RemoveCertificadoCompetenciaColaborador { get; } = "Colaborador/RemoverCertificadoCompetenciaColaborador";
        public string EnviaPushNotificationRedeColaborador { get; } = "Colaborador/EnviaPushNotificationRedeColaborador";
        public string BuscarColaborador { get; } = "Colaborador/BuscarColaborador";
        public string BuscarCargaMapaAlocacao { get; } = "Colaborador/BuscarCargaMapaAlocacao";
    }

    public class CurriculoColaborador
    {
        public string SincronizarPerfilLinkedinServicoExterno { get; } = "CurriculoColaborador/SincronizarPerfilLinkedinServicoExterno";
    }

    public class Hobby
    {
        public string ListarHobbiesColaborador { get; } = "Competencia/Hobbie/ListarHobbiesColaborador";
        public string AdicionarHobbieColaborador { get; } = "Competencia/Hobbie/AdicionarHobbieColaborador";
        public string RemoverHobbieColaborador { get; } = "Competencia/Hobbie/RemoverHobbieColaborador";
        public string GetHobbyById { get; } = "Competencia/Hobbie/GetHobbyById";
        public string MergeHobbieColaborador { get; } = "Competencia/Hobbie/MergeHobbieColaborador";
    }

    public class Softskill
    {
        public string ListarSoftskillColaborador { get; } = "Competencia/SoftSkill/ListarSoftskillColaborador";
        public string AdicionarSoftskillColaborador { get; } = "Competencia/SoftSkill/AdicionarSoftskillColaborador";
        public string RemoverSoftskillColaborador { get; } = "Competencia/SoftSkill/RemoverSoftskillColaborador";
        public string GetSoftskillById { get; } = "Competencia/SoftSkill/GetSoftskillById";
        public string AlterarSoftskillColaborador { get; } = "Competencia/SoftSkill/AtualizaSoftskillColaborador";
        public string GetSoftSkillInfoByDescricao { get; } = "Competencia/SoftSkill/GetSoftSkillInfoByDescricao";
    }

    public class Competencia
    {
        public string ListarCompetenciaColaborador { get; } = "Competencia/HardSkill/ListarCompetenciaColaborador";
        public string AdicionarCompetenciaColaborador { get; } = "Competencia/HardSkill/AdicionarCompetenciaColaborador";
        public string RemoverCompetenciaColaborador { get; } = "Competencia/HardSkill/RemoverCompetenciaColaborador";
        public string GetCompetenciaById { get; } = "Competencia/HardSkill/GetCompetenciaById";
        public string InserirCertificadoCompetenciaColaborador { get; } = "Competencia/IHardSkill/nserirCertificadoCompetenciaColaborador";
        public string AlterarCompetenciaColaborador { get; } = "Competencia/HardSkill/AtualizaCompetenciaColaborador";
        public string MergeCompetenciaColaborador { get; } = "Competencia/HardSkill/MergeCompetenciaColaborador";
        public string AlteraCertificadoPrincipalColaborador { get; } = "Competencia/HardSkill/AlteraCertificadoPrincipalColaborador";
        public string AlteraCertificadoColaborador { get; } = "Competencia/HardSkill/AlteraCertificadoColaborador";
        public string RemoveCertificadoCompetenciaColaborador { get; } = "Competencia/HardSkill/RemoveCertificadoCompetenciaColaborador";
        public string ListarIdsPorCompetenciaId { get; } = "Competencia/HardSkill/ListarIdsPorCompetenciaId";
        public string GetHardSkillInfoByDescricao { get; } = "Competencia/HardSkill/GetHardSkillInfoByDescricao";
        public string ListarCertificadoColaboradorPorCodigoInterno { get; } = "Competencia/HardSkill/ListarCertificadoColaboradorPorCodigoInterno";
        public string AdicionarCompetencia { get; } = "Competencia/HardSkill/AdicionarCompetencia";
        public string ListarNomeDeSkillsPorTipo { get; } = "Competencia/ListarNomeDeSkillsPorTipo";
        public string AlterarNomeCompetencia { get; } = "Competencia/AlterarNomeCompetencia";
    }

    public class Formacao
    {
        public string ListarFormacaoColaborador { get; } = "Formacao/ListarFormacaoColaborador";
        public string AdicionarFormacaoColaborador { get; } = "Formacao/AdicionarFormacaoColaborador";
        public string RemoverFormacaoColaborador { get; } = "Formacao/RemoverFormacaoColaborador";
        public string InserirCertificadoFormacaColaborador { get; } = "Formacao/InserirCertificadoFormacaColaborador";
        public string GetFormacaoById { get; } = "Formacao/GetFormacaoById";
        public string AlterarFormacaoColaborador { get; } = "Formacao/AtualizaFormacaoColaborador";
        public string MergeFormacaoColaborador { get; } = "Formacao/MergeFormacaoColaborador";
    }
    public class Foursys
    {
        public string Listarunidades { get; } = "Foursys/ListarUnidades";
        public string ListarunidadesPorOrg { get; } = "Foursys/ListarUnidadesPorOrg";
    }

    public class SRS
    {
        public string EditarCriarVagaFourmakersSRS { get; } = "SRS/EditarCriarVagaFourmakersSRS";
        public string CadastroCandidatoFourmakersLinkedin { get; } = "SRS/CadastroCandidatoFourmakersLinkedin";
        public string ObterVagaPorId { get; } = "SRS/ObterVagaPorId";
        public string AlterarCategoriaHabilidade { get; } = "SRS/AlterarCategoriaHabilidade";
    }

    public class Dominio
    {
        public string ListarDominioColaborador { get; } = "Competencia/Dominio/ListarDominioColaborador";
        public string AdicionarDominioColaborador { get; } = "Competencia/Dominio/AdicionarDominioColaborador";
        public string RemoverDominioColaborador { get; } = "Competencia/Dominio/RemoverDominioColaborador";
        public string GetDominioById { get; } = "Competencia/Dominio/GetDominioById";
        public string AlterarDominioColaborador { get; } = "Competencia/Dominio/AtualizaDominioColaborador";
        public string MergeDominioColaborador { get; } = "Competencia/Dominio/MergeDominioColaborador";
        public string GetDominioInfoByDescricao { get; } = "Competencia/Dominio/GetDominioInfoByDescricao";
    }

    public class Metodologia
    {
        public string ListarMetodologiasColaborador { get; } = "Competencia/Metodologia/ListarMetodologiasColaborador";
        public string AdicionarMetodologiaColaborador { get; } = "Competencia/Metodologia/AdicionarMetodologiaColaborador";
        public string RemoverMetodologiaColaborador { get; } = "Competencia/Metodologia/RemoverMetodologiaColaborador";
        public string GetMetodologiaById { get; } = "Competencia/Metodologia/GetMetodologiaById";
        public string AlterarMetodologiaColaborador { get; } = "Competencia/Metodologia/AtualizaMetodologiaColaborador";
        public string MergeMetodologiaColaborador { get; } = "Competencia/Metodologia/MergeMetodologiaColaborador";
        public string GetMetodologiaInfoByDescricao { get; } = "Competencia/Metodologia/GetMetodologiaInfoByDescricao";
    }

    public class Interesse
    {
        public string ListarInteressesColaborador { get; } = "Competencia/Interesse/ListarInteressesColaborador";
        public string AdicionarInteresseColaborador { get; } = "Competencia/Interesse/AdicionarInteresseColaborador";
        public string RemoverInteresseColaborador { get; } = "Competencia/Interesse/RemoverInteresseColaborador";
        public string GetInteresseById { get; } = "Competencia/Interesse/GetInteresseById";
        public string MergeInteresseColaborador { get; } = "Competencia/Interesse/MergeInteresseColaborador";
    }

    public class Endosso
    {
        public string ListarEndossosPorColaborador { get; } = "Endosso/ListarEndossosPorColaborador";
        public string ListarEndossosPorCompetencia { get; } = "Endosso/GetEndossosPorCompetencia";
    }
    public class UploadFile
    {
        public string UploadFileUrl { get; } = "Archive/UploadFile";
        public string DeleteFileUrl { get; } = "Archive/DeleteFile";
    }

    public class Firebase
    {
        public string EnviaPush { get; } = "Firebase/EnviaPushDoFirebaseParaApenasUmColaborador";
        public string EnviaPushEmLote { get; } = "Firebase/EnviaPushDoFirebaseEmLote";
    }

    public class Comentario
    {
        public string InserirComentario { get; } = "Comentario/InserirComentario";
        public string ListarTipoComentarios { get; } = "Comentario/ListarTiposDeComentario";
    }

    public class Noticia
    {
        public string BuscarNoticia { get; } = "Noticia/BuscarNoticia";
    }

    public class BI
    {
        public string GraficoCompetenciaEndosso { get; } = "BI/GraficoCompetenciaEndosso";
        public string ArmazenaTrending { get; } = "BI/ArmazenaTrending";
    }

    public class Idioma
    {
        public string GetIdiomaInfoByDescricao { get; } = "Competencia/Idioma/GetIdiomaInfoByDescricao";
        public string ListarIdiomaColaborador { get; } = "Competencia/Idioma/ListarIdiomaColaborador";
    }

    public class Curriculo
    {
        public string RefatorarTexto { get; } = "analise-documental/assistente-texto/refatorar-texto";
        public string Profile { get; } = "Linkedin/profile";
        public string ProfileExterno { get; } = "Linkedin/profileExterno";
        public string Classify { get; } = "Skill/classify";
        public string Document { get; } = "DocumentOcr/curriculo_pdf";
        public string ImageAnalysis { get; } = "ImageAnalysis/imageanalysis";
        public string FolhaSumario { get; } = "FolhaPonto/folhaponto/sumario";
        public string FolhaAnaliseColaborador { get; } = "FolhaPonto/folhaponto/analise_colaborador";
        public string HoleriteSumario { get; } = "AnaliseFinanceiro/sumario";
        public string HoleriteAnaliseColaborador { get; } = "AnaliseFinanceiro/analise-completa";
        public string RubricaCarga { get; } = "Rubrica/carga";
        public string RubricaCargaUnimed { get; } = "Rubrica/extrair/unimed";
        public string RubricaCargaAmil { get; } = "Rubrica/extrair/amil";
        public string RubricaCargaPortoSeguroOdonto { get; } = "Rubrica/extrair/porto-seguro-odonto";
        public string RubricaCargaProfarma { get; } = "Rubrica/extrair/profarma";
        public string RubricaCargaGenerico { get; } = "Rubrica/extrair/generico";
        public string RubricaCargaXlsxGenerico { get; } = "Rubrica/extrair/xlsx";
        public string NotaFiscalValor { get; } = "AnaliseNF/notafiscal/valor";
    }

    public class LG
    {
        public string ListarColaboradoresAtivos { get; } = "LG/ListarColaboradoresAtivos";
        public string BuscarHoleriteLGPorID { get; } = "LG/BuscarHoleriteLGPorID";
    }

    public class SkillDesconhecida
    {
        public string GetSkillDesconhecidaInfoByDescricao { get; } = "Competencia/SkillDesconhecida/GetSkillDesconhecidaInfoByDescricao";
    }

    public class Clients
    {
        public Usuario Usuario { get; } = new Usuario();
        public Colaborador Colaborador { get; } = new Colaborador();
        public CurriculoColaborador CurriculoColaborador { get; } = new CurriculoColaborador();
        public Hobby Hobby { get; } = new Hobby();
        public Softskill Softskill { get; } = new Softskill();
        public Competencia Competencia { get; } = new Competencia();
        public Formacao Formacao { get; } = new Formacao();
        public Foursys Foursys { get; } = new Foursys();
        public SRS SRS { get; } = new SRS();
        public Dominio Dominio { get; } = new Dominio();
        public Metodologia Metodologia { get; } = new Metodologia();
        public Interesse Interesse { get; } = new Interesse();
        public Endosso Endosso { get; } = new Endosso();
        public UploadFile UploadFile { get; } = new UploadFile();
        public Firebase Firebase { get; } = new Firebase();
        public Comentario Comentario { get; } = new Comentario();
        public Noticia Noticia { get; } = new Noticia();
        public BI BI { get; } = new BI();
        public Curriculo Curriculo { get; } = new Curriculo();
        public LG LG { get; } = new LG();
        public Idioma Idioma { get; } = new Idioma();
        public SkillDesconhecida SkillDesconhecida { get; } = new SkillDesconhecida();
    }
}