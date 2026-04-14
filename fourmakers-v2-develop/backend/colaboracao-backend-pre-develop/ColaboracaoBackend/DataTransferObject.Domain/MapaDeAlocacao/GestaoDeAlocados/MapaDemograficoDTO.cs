using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados
{
    // ──────────────── Mapa Demografico ────────────────
    public class MapaDemograficoDTO
    {
        public string NomeCompleto { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public DateTime? DataNascimento { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public int? TempoCasa { get; set; }
        public string Genero { get; set; }
        public string Etnia { get; set; }
        public string OrientacaoSexual { get; set; }
        public string Escolaridade { get; set; }
        public string LocalVisto { get; set; }
        public string ValidadeVisto { get; set; }
        public string Email { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string ModeloTrabalho { get; set; }
        public string Unidade { get; set; }
        public int OrgId { get; set; }
        public string Organizacao { get; set; }
        public string Talento { get; set; }
        public int Ativo { get; set; }

        // ──────────────── Para uso em Listas Auxiliares ────────────────
        public List<HardSkillsDTO> HardSkills { get; set; } = new();
        public List<SoftSkillsDTO> SoftSkills { get; set; } = new();
        public List<MetodologiasDTO> Metodologias { get; set; } = new();
        public List<FormacoesDTO> Formacoes { get; set; } = new();
        public List<LinguasDTO> Linguas { get; set; } = new();
        public List<CargosDTO> Cargos { get; set; } = new();
        public List<NacionalidadesDTO> Nacionalidades { get; set; } = new();
        public List<ClientesDTO> Clientes { get; set; } = new();
        public List<GestoresDTO> Gestores { get; set; } = new();
    }

    // ──────────────── Listas Auxiliares ────────────────
    public class CompetenciasDTO
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
    }
    public class HardSkillsDTO
    {        
        public int Id { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string DescricaoHardSkill { get; set; }
        public string Senioridade { get; set; }
    }
    public class SoftSkillsDTO
    {        
        public int Id { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string DescricaoSoftSkill { get; set; }
        public string Senioridade { get; set; }
    }
    public class LinguasDTO
    {        
        public int Id { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string DescricaoLingua { get; set; }
        public string Senioridade { get; set; }
    }
    public class FormacoesDTO
    {
        public int Id { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string DescricaoFormacao { get; set; }
        public string Senioridade { get; set; }
    }
    public class CargosDTO
    {
        public int Id { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string DescricaoCargo { get; set; }
      }
    public class MetodologiasDTO
    {
        public int Id { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string DescricaoMetodologia { get; set; }
        public string Senioridade { get; set; }
    }
    public class NacionalidadesDTO
    {
        public int Id { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string DescricaoNacionalidade { get; set; }
    }
    public class ClientesDTO
    {
        public string Id { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string NomeCliente { get; set; }
    }
    public class GestoresDTO
    {
        public string CodigoInternoColaborador { get; set; }
        public string CodigoInternoGestor { get; set; }
        public string NomeGestor { get; set; }
    }
}

