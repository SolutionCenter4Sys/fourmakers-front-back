using Competencia.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados
{
    // ──────────────── Minha Jornada ────────────────
    public class MinhaJornadaDTO
    {
        public string CodigoInternoGestorAdm { get; set; }
        public string NomeGestorAdm { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string NomeCompleto { get; set; }
        public string CodigoProjeto { get; set; }
        public int IdAlocacao { get; set; }
        public List<ClientesAlocacaoDTO> Clientes { get; set; } = new();
    }

    // ──────────────── Para uso em Listas Clientes ────────────────
    public class ClientesAlocacaoDTO
    {
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public string PerfilId { get; set; }
        public string Perfil { get; set; }
        public string CodGestorCliente { get; set; }
        public string NomeGestorCliente { get; set; }
        public List<HabilidadesDTO> Habilidades { get; set; } = new();
    }

    // ──────────────── Listas Auxiliares ────────────────
    public class HabilidadesDTO
    {
        public string CodigoCliente { get; set; }
        public int? PerfilTipoId { get; set; }
        public string TipoPerfil { get; set; }
        public int? SkillId { get; set; }
        public string Habilidade { get; set; }
        public int? SenioridadeId { get; set; }
        public string Senioridade { get; set; }
        public int? Interesse { get; set; }
    }
    public class MinhaJornadaColaboradorDTO
    {
        public string CodigoInternoColaborador { get; set; }
        public string NomeCompleto { get; set; }
        public List<HabilidadesColaboradorDTO> ColaboradorHabilidades { get; set; } = new();
    }

    public class HabilidadesColaboradorDTO
    {
        public string CodigoInternoColaborador { get; set; }
        public int? PerfilTipoId { get; set; }
        public string TipoPerfil { get; set; }
        public int? SkillId { get; set; }
        public string Habilidade { get; set; }
        public int? SenioridadeId { get; set; }
        public string Senioridade { get; set; }
        public int? Interesse { get; set; }
    }
    // DTO Principal - Apenas dados do Colaborador
    public class ColaboradorJornadaDTO
    {
        public string CodigoInternoColaborador { get; set; }
        public string NomeCompleto { get; set; }
        public List<AlocacaoJornadaDTO> Alocacoes { get; set; } = new List<AlocacaoJornadaDTO>();
    }

    // DTO de Alocação - Contém os dados da alocação e a lista de clientes
    public class AlocacaoJornadaDTO
    {
        public string CodigoProjeto { get; set; }
        public int IdAlocacao { get; set; }
        public List<ClientesAlocacaoDTO> Clientes { get; set; } = new List<ClientesAlocacaoDTO>();
    }

    public class SugestaoParamDTO
    {
        public string CodigoInternoColaborador { get; set; }
        //public string CodigoInternoColaboradorSugeriu { get; set; }
        public string CodigoGestorAdm { get; set; }
        //public string CodigoGestorOper { get; set; }
        public string CodigoCliente { get; set; }
        public ItemPerfilEnum Tipo_Id { get; set; }
        public int Skill_Id { get; set; }
        public string Perfil_Id { get; set; }
        public int? Senioridade_Id { get; set; }
        public DateTime? Data { get; set; }
        public bool Ativo { get; set; }
        public string GestorExternoPerfil { get; set; }
    }
    public class SugestaoDTO : SugestaoParamDTO
    {
        public string Id { get; set; }
    }
    public class SugestaoHistoricoDTO
    {
        public string Id { get; set; }
        public string CodigoInternoColaboradorAvaliador { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public bool? Aprovado { get; set; }
        public int TbStatusSugestaoId { get; set; }
        public string Perfil_Id { get; set; }
        public string Observacao { get; set; }
        public DateTime? Data { get; set; }
    }

    public class SugestaoStatusDTO
    {
        public long Id { get; set; }
        public string Descricao { get; set; }
    }

    //Parametro
    public class SugestaoHistoricoParamDTO
    {
        //public string Id { get; set; }
        public string SugestaoId { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public bool? Aprovado { get; set; }
        public int TbStatusSugestaoId { get; set; }
        public string Perfil_Id { get; set; }
        public string Observacao { get; set; }
        public long SkillId { get; set; }
        public int ItemPerfil { get; set; }
        public int NivelId { get; set; }
        public DateTime? Data { get; set; }
    }
    public class SugestaoAtualizacaoParamDTO
    {
        public string Id { get; set; }
        public string CodigoGestorAdm { get; set; }
        //public string CodigoGestorOper { get; set; }
        public string CodigoCliente { get; set; }
        public int Tipo_Id { get; set; }
        public int Skill_Id { get; set; }
        public string Perfil_Id { get; set; }
        public int? Senioridade_Id { get; set; }
        public DateTime? Data { get; set; }
        public bool Ativo { get; set; }
    }
    //Response
    public class SugestaoResponseDTO
    {
        public string Id { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public int TipoId { get; set; }
        public int SkillId { get; set; }
        public int? SenioridadeId { get; set; }
        public bool Ativo { get; set; }
        public List<SugestaoHistoricoDTO> HistoricoSugestao { get; set; } = new();
    }
    public class SugestaoSkillResponseDTO
    {
        public string Id { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string CodigoGestorAdm { get; set; }
        public string CodigoCliente { get; set; }
        public int Tipo_Id { get; set; }
        public string DescricaoTipo { get; set; }
        public int Skill_Id { get; set; }
        public string DescricaoSkill { get; set; }
        public string Perfil_Id { get; set; }
        public int? Senioridade_Id { get; set; }
        public string Senioridade { get; set; }
        public DateTime? Data { get; set; }
        public bool Ativo { get; set; }
    }
    public class SugestaoSkilleHistoricoResponseDTO : SugestaoSkillResponseDTO
    {
        [JsonPropertyOrder(10)]
        public List<SugestaoHistoricoDTO> HistoricoSugestao { get; set; } = new();

    }
    public class SugestaoSkilleHistoricoOrdemResponseDTO
    {
        public string Id { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string CodigoGestorAdm { get; set; }
        public string CodigoCliente { get; set; }
        public int Tipo_Id { get; set; }
        public string DescricaoTipo { get; set; }
        public int Skill_Id { get; set; }
        public string DescricaoSkill { get; set; }
        public string Perfil_Id { get; set; }
        public int? Senioridade_Id { get; set; }
        public string Senioridade { get; set; }
        public DateTime? Data { get; set; }
        public bool Ativo { get; set; }

        public List<SugestaoHistoricoDTO> HistoricoSugestao { get; set; } = new();

    }

}


