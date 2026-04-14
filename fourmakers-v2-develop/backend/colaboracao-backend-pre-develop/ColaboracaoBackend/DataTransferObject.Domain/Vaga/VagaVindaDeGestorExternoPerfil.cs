using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Vaga;
public class VagaVindaDeGestorExternoPerfil
{
    public long Codigo { get; set; }
    public long NumeroDeVagas { get; set; }
    public string StatusVagaCod { get; set; }
    public string IdPerfilGerador { get; set; }
    public string OrgId { get; set; }
    public int? ModeloTrabalhoCod { get; set; }
    public List<VagaSkillRecrutamentoDTO> GestorExternoPerfilSkills { get; set; }
    public string CodGestorExterno { get; set; }
    public string NomePerfil { get; set; }
    public decimal CustoPerfil { get; set; }
    public decimal RatecardPerfil { get; set; }
    public string InformacoesRelevantes { get; set; }
    public Guid PermanenciaId { get; set; }
    public Guid ModeloTrabalhoId { get; set; }
    public Guid ProfissionalLocalidadeId { get; set; }
    public string Cidade { get; set; }
    public string Estado { get; set; }
    public string Cep { get; set; }
    public int? HibridoDias { get; set; }
    
    // Novas propriedades para informações complementares
    public string ColaboradorCodigoInternoColaboradorGestorOrgLogada { get; set; }
    public string PropostaCrm { get; set; }
    public string TipoVagaId { get; set; }
    public int? TipoContratacaoId { get; set; }
    public string UnidadeId { get; set; }
    public List<string> CodColaboradoresEntrevistadores { get; set; }
    public string Localizacao { get; set; }
    public Guid? TipoEmpregoLinkedin { get; set; }
    public Guid? NivelExperienciaLinkedin { get; set; }
    public string? RecrutadorVaga { get; set; }
    public string? Maquina { get; set; }
    public int CandidatosContratados { get; set; }
}

public class VagaVindaDeGestorExternoPerfilSkill
{
    public ItemPerfil ItemPerfil { get; set; }
    public Skill Skill { get; set; }
    public Nivel Nivel { get; set; }
    public DateTime DataCriacao { get; set; }
    public bool Relevante { get; set; }
}

public class ItemPerfil
{
    public string Descricao { get; set; }
    public int Id { get; set; }
}

public class Skill
{
    public string Descricao { get; set; }
    public int Id { get; set; }
}

public class Nivel
{
    public string Descricao { get; set; }
    public int Id { get; set; }
}
