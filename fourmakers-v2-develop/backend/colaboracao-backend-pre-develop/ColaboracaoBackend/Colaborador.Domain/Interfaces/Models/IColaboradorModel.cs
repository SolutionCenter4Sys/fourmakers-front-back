using Colaborador.Domain.Interfaces.Factorys;
using DataTransferObject.Domain.Colaborador;
using System;
using System.Collections.Generic;

namespace Colaborador.Domain.Interfaces.Models
{
    public interface IColaboradorModel
    {
        string Id { get; set; }
        string Cpf { get; set; }
        string CpfRequest { get; set; }
        long? ImagemId { get; set; }
        string ContatoPrincipalDDI { get; set; }
        string ContatoPrincipal { get; set; }
        string NomeCompleto { get; set; }
        DateTime DataNascimento { get; set; }
        string Rg { get; set; }
        string Matricula { get; set; }
        DateTime? Admissao { get; set; }
        long? EnderecoId { get; set; }
        long? DiretoriaId { get; set; }
        DateTime DataCriacao { get; set; }
        DateTime DataAlteracao { get; set; }
        string ContatoOutro { get; set; }
        sbyte Candidato { get; set; }
        sbyte Ativo { get; set; }
        string PathImagem { get; set; }
        ColaboradorDTO ColaboradorDTO { get; set; }
        ForcaPerfilDTO ForcaPerfil { get; set; }
        SimpleColaboradorDTO SimpleColaboradorDTO { get; set; }
        ColaboradorSaudeDTO ColaboradorSaudeDTO { get; set; }
        ColaboradorCurriculoDTO ColaboradorCurriculoDTO { get; set; }
        bool Administrador { get; set; }
        int OrgId { get; set; }
        StatusColaboradorDTO ListStatusColaboradorDTO { get; set; }
        ColaboradorAtivoDTO ColaboradorAtivoDTO { get; set; }

        IColaboradorModel GetModelByCpf(string cpf, int orgId);
        IColaboradorModel ValidaGrupoAcesso(string cpf);
        IColaboradorModel GetModelByKey(string cpf);
        IColaboradorModel UpdateModel();
        void EstouSeguindoMeSegue(string cpfRequest);
        List<string> GetSeguidores(string cpf, IColaboradorDomainFactory _fotoDomainFactory);
        List<string> GetSeguindo(string cpf, IColaboradorDomainFactory colaboradorDomainFactory);
        void PermissaoAcesso();
        List<IColaboradorModel> BuscarColaboradores(string cpf, int cursor, int limite, int candidato, int orgId, IColaboradorDomainFactory _colaboradorDomainFactory, out int totalResultsCount);
        IEnumerable<IColaboradorModel> BuscaRowsColaborador(List<string> lstCpf, string cpfSolicitante, int deslocamentoBusca, int candidato, IColaboradorDomainFactory _colaboradorDomainFactory);
        int ContarColaboradoresMenosEste(string cpf);
        string FollowColaborador(string cpfSeguidor, string cpfSeguir);
        void UnfollowColaborador(string cpfSeguidor, string cpfSeguir);
        int ContarCandidatosMenosEste(string cpf);
        IColaboradorModel BuscarNomeColaborador();
        List<IColaboradorModel> BuscarStatusColaborador(IColaboradorDomainFactory colaboradorDomainFactory);
        List<IColaboradorModel> BuscarColaboradoresAtivos(int cursor, int limite, out int totalResultCount, IColaboradorDomainFactory colaboradorDomainFactory);
        //SRSCandidateGetResult MontaInfoCandidateGet(SRSCandidateGetResult srsCandidateGetResult);
        IColaboradorModel InserirDadosPCD(EnumPCD pCD, bool grupoDeRisco, string descricaoCondicaoDeSaude, string cpf, IColaboradorDomainFactory _colaboradorDomainFactory);
        IColaboradorModel UpdateFotoModel();
        IColaboradorModel AlterarDadosPCD(EnumPCD pCD, bool grupoDeRisco, string descricaoCondicaoDeSaude, string cpf, IColaboradorDomainFactory colaboradorDomainFactory);
    }
}